using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace VMware_VM_Generator
{
    public partial class Form1 : Form
    {
        private IniFile ini = new IniFile("settings.ini");

        // Flags and base path for dynamic folder naming based on VM name
        private bool isUserPathCustomized = false;
        private bool isUpdatingProgrammatically = false;
        private string baseVmDirectory = "";
        private string currentIconPath = "";

        // Pending action variables for post-wizard execution upon finishing
        private string pendingVmxPath = "";
        private string pendingVmFolder = "";
        private string pendingVmwarePath = "";

        // Per-control suffix overrides loaded from language file
        private readonly System.Collections.Generic.Dictionary<string, string> controlRecommendedSuffix =
      new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly System.Collections.Generic.Dictionary<string, string> controlNotSupportedSuffix =
          new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Localized suffix texts (fallback to English defaults)
        private string localizedNotSupported = "(Not supported)";
        private string localizedRecommended = "(Recommended)";

        private readonly int[] memoryStops = new int[]
        {
            4, 8, 16, 32, 64, 128, 256, 512,
            1024, 2048, 4096, 8192,
            16384, 32768, 65536, 131072
        };

        private Rectangle[] tickHitBoxes;

        private int recommendedMin;
        private int recommendedDefault;
        private int recommendedMax;

        // P/Invoke for native Windows Icon Picker dialog and extraction
        [DllImport("shell32.dll", EntryPoint = "PickIconDlg", CharSet = CharSet.Auto)]
        private static extern bool PickIconDlg(IntPtr hwndOwner, StringBuilder pszIconPath, int cchIconPath, ref int piIconIndex);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int DestroyIcon(IntPtr hIcon);

        public Form1()
        {
            InitializeComponent();

            // Reduce flicker by enabling double-buffering and using WS_EX_COMPOSITED
            try
            {
                this.UpdateStyles();
            }
            catch { }

            // Ensure memory groupbox and label have no visible caption
            try
            {
                memory.Text = "";
                labelMemory.Text = "";
            }
            catch { }

            // Enable double-buffering on container controls (reduce flicker) via non-public DoubleBuffered property
            try
            {
                var prop = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (prop != null)
                {
                    void EnableDoubleBuffer(Control c)
                    {
                        prop.SetValue(c, true);
                        foreach (Control ch in c.Controls)
                            EnableDoubleBuffer(ch);
                    }

                    if (memory != null) EnableDoubleBuffer(memory);
                    if (panelStep2 != null) EnableDoubleBuffer(panelStep2);
                }
            }
            catch { }

            // Force Form1 to use the primary executable's embedded icon resource
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                using (Icon mainExeIcon = Icon.ExtractAssociatedIcon(exePath))
                {
                    if (mainExeIcon != null)
                    {
                        this.Icon = (Icon)mainExeIcon.Clone();
                    }
                }
            }
            catch
            {
                // Keep default if extraction fails
            }

            // Welcome screen buttons
            btnWelcomeBack.Click += btnWelcomeBack_Click;
            btnWelcomeNext.Click += btnWelcomeNext_Click;
            btnWelcomeCancel.Click += btnWelcomeCancel_Click;

            // Step 1 buttons
            btnStep1Back.Click += btnStep1Back_Click;
            btnStep1Next.Click += btnStep1Next_Click;
            btnStep1Cancel.Click += btnStep1Cancel_Click;

            // Step 2 buttons
            btnStep2Back.Click += btnStep2Back_Click;
            btnStep2Next.Click += btnStep2Next_Click;
            btnStep2Cancel.Click += btnStep2Cancel_Click;

            // Step 3 buttons
            btnStep3Back.Click += (s, e) => ShowStep(2);
            btnStep3Next.Click += (s, e) => ShowStep(4);
            btnStep3Cancel.Click += (s, e) => Close();

            // Step 4 buttons
            btnStep4Back.Click += (s, e) => ShowStep(3);
            btnStep4Next.Click += (s, e) => ShowStep(5);
            btnStep4Cancel.Click += (s, e) => Close();

            // Step 5 buttons (Moves to Step 6: Shortcuts & Icons)
            btnStep5Back.Click += (s, e) => ShowStep(4);
            btnStep5Next.Click += (s, e) => ShowStep(6);
            btnStep5Cancel.Click += (s, e) => Close();

            // Step 6 buttons (Shortcuts & Icons -> Generates summary and moves to Step 7)
            btnStep6Back.Click += (s, e) => ShowStep(5);
            btnStep6Cancel.Click += (s, e) => Close();
            btnStep6Next.Click += (s, e) => { GenerateSummary(); ShowStep(7); };

            // Step 7 buttons (Summary & Create)
            btnStep7Back.Click += (s, e) => ShowStep(6);
            btnStep7Cancel.Click += (s, e) => Close();
            btnCreate.Click += (s, e) => {
                try
                {
                    // 1. Construct the target VMX file path based on user input folder and VM name
                    string vmFolder = txtVMFolder.Text.Trim();
                    string vmName = txtVMName.Text.Trim();
                    if (string.IsNullOrWhiteSpace(vmName)) vmName = "New Virtual Machine";

                    // Ensure directory exists
                    if (!Directory.Exists(vmFolder))
                    {
                        Directory.CreateDirectory(vmFolder);
                    }

                    string vmxFilePath = Path.Combine(vmFolder, vmName + ".vmx");

                    // Extract Network selection properly
                    string networkSelection = "nat";
                    if (radioButtonBridged.Checked) networkSelection = "bridged";
                    else if (radioButtonHostOnly.Checked) networkSelection = "hostonly";
                    else if (radioButtonNoNetwork.Checked) networkSelection = "none";

                    // 2. Gather all configuration parameters from Form controls
                    var config = new VirtualMachineGenerator.VmConfig
                    {
                        VmName = vmName,
                        MemoryMb = (int)numericUpDownMemory.Value,
                        NumVcpus = comboNumberOfProcessors.SelectedItem is int p ? p : (int.TryParse(comboNumberOfProcessors.Text, out int parsedP) ? parsedP : 1),
                        CoresPerSocket = comboBoxNumberOfCoresPerProcessor.SelectedItem is int c ? c : (int.TryParse(comboBoxNumberOfCoresPerProcessor.Text, out int parsedC) ? parsedC : 1),

                        // Firmware selection from form radio buttons
                        Firmware = uefi.Checked ? "efi" : "bios",

                        // Network selection
                        NetworkType = networkSelection,

                        // Sound card selection
                        SoundVirtualDev = radioButtonSB16.Checked ? "sb16" :
                    radioButtonES1371.Checked ? "es1371" :
                    radioButtonHDAudio.Checked ? "hdaudio" : "hdaudio",

                        // Storage controller type
                        StorageControllerType = radioButtonIDE.Checked ? "IDE" :
                        radioButtonSCSI.Checked ? "SCSI" :
                        radioButtonNVMe.Checked ? "NVMe" : "SATA",

                        // SCSI specific virtual device type
                        ScsiVirtualDev = radioBusLogic.Checked ? "" :
                    radioLSILogic.Checked ? "lsilogic" :
                    radioLSILogicSAS.Checked ? "lsisas1068" : "pvscsi",

                        // Virtual disk path
                        DiskPath = Path.Combine(vmFolder, vmName + ".vmdk"),

                        // Floppy count
                        FloppyCount = (int)numericUpDownFloppyCount.Value,

                        // CD-ROM counts per controller
                        IdeCdCount = (int)numericUpDownCDIDECount.Value,
                        ScsiCdCount = (int)numericUpDownCDSCSICount.Value,
                        SataCdCount = (int)numericUpDownCDSATACount.Value,

                        // Custom icon path
                        IconPath = currentIconPath
                    };

                    // 3. Call the generator to create ONLY the .vmx file (no dummy VMDK)
                    VirtualMachineGenerator.CreateVirtualMachine(vmxFilePath, config);

                    // 3.1 FIXED: Real VMDK creation with correct -t mapping per user spec
                    // !Allocate + Single = 0 (growable single)
                    // !Allocate + Split  = 1 (growable split 2GB)
                    // Allocate + Single  = 2 (preallocated single)
                    // Allocate + Split   = 3 (preallocated split)
                    try
                    {
                        bool allocate = checkBoxAllocate.Checked;
                        bool isSplit = radioButtonSplit.Checked;
                        int tValue = 0;

                        if (!allocate && !isSplit) tValue = 0;
                        else if (!allocate && isSplit) tValue = 1;
                        else if (allocate && !isSplit) tValue = 2;
                        else if (allocate && isSplit) tValue = 3;

                        int driveCapacity = (int)numericUpDownDriveCapacity.Value;
                        string vmwareExePath = GetVmwarePath();
                        string vmwareDir = Path.GetDirectoryName(vmwareExePath);
                        string vdiskManagerPath = Path.Combine(vmwareDir, "vmware-vdiskmanager.exe");

                        if (!File.Exists(vdiskManagerPath))
                        {
                            string[] fallbacks = {
                                @"C:\Program Files (x86)\VMware\VMware Workstation\vmware-vdiskmanager.exe",
                                @"C:\Program Files\VMware\VMware Workstation\vmware-vdiskmanager.exe"
                            };
                            foreach (var fb in fallbacks) if (File.Exists(fb)) { vdiskManagerPath = fb; break; }
                        }

                        if (File.Exists(config.DiskPath)) { try { File.Delete(config.DiskPath); } catch { } }

                        if (!File.Exists(vdiskManagerPath))
                            throw new FileNotFoundException("vmware-vdiskmanager.exe not found at: " + vdiskManagerPath);

                        string vdiskArgs = $"-c -s {driveCapacity}GB -a lsilogic -t {tValue} \"{config.DiskPath}\"";

                        var psi = new ProcessStartInfo(vdiskManagerPath, vdiskArgs)
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        using (var proc = Process.Start(psi))
                        {
                            proc.WaitForExit();
                            if (proc.ExitCode != 0)
                            {
                                string err = proc.StandardError.ReadToEnd() + proc.StandardOutput.ReadToEnd();
                                string lowerErr = err.ToLower();

                                string langFileNameErr = ini.Read("LanguageFile", "General");
                                IniFile langErr = !string.IsNullOrWhiteSpace(langFileNameErr) ? new IniFile(langFileNameErr) : null;
                                string errTitle = (langErr != null) ? langErr.Read("ErrorTitle", "Language") : "Error";
                                if (string.IsNullOrWhiteSpace(errTitle)) errTitle = "Error";
                                string errTemplate = (langErr != null) ? langErr.Read("ErrorMessageTemplate", "Language") : "Failed to create virtual machine file:\n{error}";
                                if (string.IsNullOrWhiteSpace(errTemplate)) errTemplate = "Failed to create virtual machine file:\n{error}";

                                string translatedError = "";
                                if (lowerErr.Contains("already exists") || lowerErr.Contains("already_exists") || lowerErr.Contains("0xc"))
                                {
                                    translatedError = (langErr != null) ? langErr.Read("ErrorFileAlreadyExists", "Language") : "";
                                    if (string.IsNullOrWhiteSpace(translatedError)) translatedError = "The file already exists.";
                                }
                                else if (lowerErr.Contains("disk is full") || lowerErr.Contains("0x8") || lowerErr.Contains("not enough space"))
                                {
                                    translatedError = (langErr != null) ? langErr.Read("ErrorDiskFull", "Language") : "";
                                    if (string.IsNullOrWhiteSpace(translatedError)) translatedError = "The host disk is full.";
                                }
                                else
                                {
                                    string template = (langErr != null) ? langErr.Read("ErrorVDiskManagerFailed", "Language") : "";
                                    if (string.IsNullOrWhiteSpace(template)) template = "vdiskmanager failed with exit code {0}: {1}";
                                    try { translatedError = string.Format(template, proc.ExitCode, err); }
                                    catch { translatedError = template + $" {proc.ExitCode}: {err}"; }
                                }

                                string finalMsg = errTemplate.Replace("{error}", translatedError);
                                MessageBox.Show(finalMsg, errTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, GetRtlOptions(langErr));
                                return;
                            }
                        }
                    }
                    catch (Exception exDisk)
                    {
                        string langFileNameErr2 = ini.Read("LanguageFile", "General");
                        IniFile langErr2 = !string.IsNullOrWhiteSpace(langFileNameErr2) ? new IniFile(langFileNameErr2) : null;
                        string errTitle2 = (langErr2 != null) ? langErr2.Read("ErrorTitle", "Language") : "Error";
                        if (string.IsNullOrWhiteSpace(errTitle2)) errTitle2 = "Error";

                        string lower = exDisk.Message.ToLower();
                        string finalText;

                        if (lower.Contains("vmware-vdiskmanager") && lower.Contains("not found"))
                        {
                            finalText = (langErr2 != null) ? langErr2.Read("ErrorVDiskManagerNotFound", "Language") : "";
                            if (string.IsNullOrWhiteSpace(finalText)) finalText = "vmware-vdiskmanager.exe not found.";
                        }
                        else
                        {
                            string errTemplate2 = (langErr2 != null) ? langErr2.Read("ErrorMessageTemplate", "Language") : "Failed to create virtual machine file:\n{error}";
                            if (string.IsNullOrWhiteSpace(errTemplate2)) errTemplate2 = "Failed to create virtual machine file:\n{error}";
                            finalText = errTemplate2.Replace("{error}", exDisk.Message);
                        }

                        MessageBox.Show(finalText, errTitle2, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, GetRtlOptions(langErr2));
                        return;
                    }

                    // 4. Handle custom icon extraction and copying
                    string targetIconPath = Path.Combine(vmFolder, vmName + ".ico");
                    if (!string.IsNullOrEmpty(currentIconPath) && File.Exists(currentIconPath))
                    {
                        if (currentIconPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Copy(currentIconPath, targetIconPath, true);
                        }
                        else
                        {
                            using (Icon ico = Icon.ExtractAssociatedIcon(currentIconPath))
                            {
                                if (ico != null)
                                {
                                    using (var fs = new FileStream(targetIconPath, FileMode.Create))
                                    {
                                        ico.Save(fs);
                                    }
                                }
                            }
                        }
                    }

                    string vmwarePath = GetVmwarePath();

                    // Determine target path and arguments based on user's shortcut type selection
                    string shortcutTarget = vmwarePath;
                    string shortcutArguments = "";

                    if (radioShortcutDirect.Checked)
                    {
                        shortcutTarget = vmxFilePath;
                        shortcutArguments = "";
                    }
                    else if (radioShortcutKvm.Checked)
                    {
                        shortcutTarget = "kvm";
                        shortcutArguments = $"\"{vmxFilePath}\"";
                    }
                    else // Custom Workstation shortcut
                    {
                        shortcutTarget = vmwarePath;
                        if (checkBoxNewWindow.Checked) shortcutArguments += "-n ";
                        if (checkBoxShortStart.Checked)
                        {
                            if (checkBoxFullScreen.Checked) shortcutArguments += "-X ";
                            else shortcutArguments += "-x ";
                        }
                        if (checkBoxClose.Checked) shortcutArguments += "-q ";
                        shortcutArguments += $"\"{vmxFilePath}\"";
                    }

                    // 5. Create Desktop Shortcut if requested
                    if (checkBoxDesktop.Checked)
                    {
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        string shortcutPath = Path.Combine(desktopPath, vmName + ".lnk");
                        CreateShortcut(shortcutPath, vmxFilePath, targetIconPath, shortcutArguments, shortcutTarget);
                    }

                    // 6. Create Start Menu Shortcut if requested
                    if (checkBoxStartMenu.Checked)
                    {
                        string programsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                        string startMenuPath = Path.Combine(programsPath, "My Virtual Machines");

                        if (!Directory.Exists(startMenuPath))
                        {
                            Directory.CreateDirectory(startMenuPath);
                        }

                        string shortcutPath = Path.Combine(startMenuPath, vmName + ".lnk");
                        CreateShortcut(shortcutPath, vmxFilePath, targetIconPath, shortcutArguments, shortcutTarget);
                    }

                    // 7. Store base paths for final execution
                    pendingVmxPath = vmxFilePath;
                    pendingVmFolder = vmFolder;
                    pendingVmwarePath = vmwarePath;

                    // 8. Advance to the finish panel (Step 8)
                    ShowStep(8);
                }
                catch (Exception ex)
                {
                    string langFileName = ini.Read("LanguageFile", "General");
                    IniFile lang = !string.IsNullOrWhiteSpace(langFileName) ? new IniFile(langFileName) : null;

                    string errTitle = (lang != null) ? lang.Read("ErrorTitle", "Language") : "";
                    if (string.IsNullOrWhiteSpace(errTitle)) errTitle = "Error";

                    string direction = (lang != null) ? lang.Read("Direction", "Language") : "LTR";
                    bool isRtl = !string.IsNullOrWhiteSpace(direction) && direction.Equals("RTL", StringComparison.OrdinalIgnoreCase);

                    string errorMessage = ex.Message;
                    string lowerEx = errorMessage.ToLower();

                    if (lowerEx.Contains("already exists"))
                    {
                        errorMessage = (lang != null) ? lang.Read("ErrorFileAlreadyExists", "Language") : "";
                        if (string.IsNullOrWhiteSpace(errorMessage)) errorMessage = "The file already exists.";
                    }
                    else if (lowerEx.Contains("disk is full") || lowerEx.Contains("filling the disk"))
                    {
                        errorMessage = (lang != null) ? lang.Read("ErrorDiskFull", "Language") : "";
                        if (string.IsNullOrWhiteSpace(errorMessage)) errorMessage = "The host storage disk is full. Please free up space and try again.";
                    }

                    string errTemplate = (lang != null) ? lang.Read("ErrorMessageTemplate", "Language") : "";
                    if (string.IsNullOrWhiteSpace(errTemplate)) errTemplate = "Failed to create virtual machine file:\n{error}";

                    string finalMessage = errTemplate.Replace("{error}", errorMessage);

                    MessageBoxOptions options = 0;
                    if (isRtl)
                    {
                        options = MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
                    }

                    MessageBox.Show(finalMessage, errTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
                }
            };

            // Hook up Finish button event handler to close wizard and execute actions based on live checkbox states
            buttonFinish.Click += (s, e) => {
                bool shouldOpenFolder = checkBoxOpenFolder.Checked;
                bool shouldStartVm = checkBoxStartVM.Checked;

                this.Close();

                if (shouldOpenFolder && Directory.Exists(pendingVmFolder))
                {
                    try
                    {
                        Process.Start("explorer.exe", pendingVmFolder);
                    }
                    catch { }
                }

                try
                {
                    string vmwareArgs = shouldStartVm ? $"-n -x \"{pendingVmxPath}\"" : $"-n \"{pendingVmxPath}\"";
                    Process.Start(pendingVmwarePath, vmwareArgs);
                }
                catch
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(pendingVmxPath) { UseShellExecute = true });
                    }
                    catch { }
                }
            };

            // Hook up Change Icon button event handler using native Windows PickIconDlg
            buttonChangeIcon.Click += buttonChangeIcon_Click;

            btnBrowseFolder.Click += btnBrowseFolder_Click;

            // Wire up text change events for auto-updating VM folder based on VM name and validating inputs
            txtVMName.TextChanged += txtVMName_TextChanged;
            txtVMFolder.TextChanged += txtVMFolder_TextChanged;

            bios.CheckedChanged += bios_CheckedChanged;
            uefi.CheckedChanged += uefi_CheckedChanged;

            // Wire up check changed event for shortcut type radio buttons to toggle Workstation specific switches
            EventHandler updateWorkstationSwitches = (s, e) => {
                bool isWorkstation = radioShortcutWorkstation.Checked;
                checkBoxNewWindow.Enabled = isWorkstation;
                checkBoxShortStart.Enabled = isWorkstation;
                checkBoxFullScreen.Enabled = isWorkstation && checkBoxShortStart.Checked;
                checkBoxClose.Enabled = isWorkstation;
            };

            radioShortcutDirect.CheckedChanged += updateWorkstationSwitches;
            radioShortcutKvm.CheckedChanged += updateWorkstationSwitches;
            radioShortcutWorkstation.CheckedChanged += updateWorkstationSwitches;

            // Wire up check changed event for short start to toggle full screen availability
            checkBoxShortStart.CheckedChanged += (s, e) => {
                checkBoxFullScreen.Enabled = radioShortcutWorkstation.Checked && checkBoxShortStart.Checked;
                if (!checkBoxShortStart.Checked)
                {
                    checkBoxFullScreen.Checked = false;
                }
            };

            comboNumberOfProcessors.SelectedIndexChanged += (s, e) => UpdateTotalCores();
            comboBoxNumberOfCoresPerProcessor.SelectedIndexChanged += (s, e) => UpdateTotalCores();

            // Also update when user types numbers manually into the combo boxes
            comboNumberOfProcessors.TextChanged += (s, e) => UpdateTotalCores();
            comboBoxNumberOfCoresPerProcessor.TextChanged += (s, e) => UpdateTotalCores();

            trackBarMemory.Scroll += trackBarMemory_Scroll;
            numericUpDownMemory.ValueChanged += numericUpDownMemory_ValueChanged;

            memory.Paint += memory_Paint;
            memory.MouseMove += memory_MouseMove;
            memory.MouseDown += memory_MouseDown;

            labelRecommendedMin.Click += (s, e) => SetMemory(recommendedMin);
            labelRecommendedDefault.Click += (s, e) => SetMemory(recommendedDefault);
            labelRecommendedMax.Click += (s, e) => SetMemory(recommendedMax);

            // Create suffix labels after layout is complete
            this.Shown += Form1_Shown;

            numericUpDownFloppyCount.ValueChanged += numericUpDownFloppyCount_ValueChanged;
            numericUpDownCDIDECount.ValueChanged += numericUpDownCDIDECount_ValueChanged;
            numericUpDownCDSCSICount.ValueChanged += numericUpDownCDSCSICount_ValueChanged;
            numericUpDownCDSATACount.ValueChanged += numericUpDownCDSATACount_ValueChanged;
        }

        // Helper method to get MessageBox RTL options based on language file Direction=RTL
        private MessageBoxOptions GetRtlOptions(IniFile langFile)
        {
            string dir = langFile != null ? langFile.Read("Direction", "Language") : "";
            bool isRtl = !string.IsNullOrWhiteSpace(dir) && dir.Equals("RTL", StringComparison.OrdinalIgnoreCase);
            return isRtl ? (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) : 0;
        }

        // Helper method to retrieve VMware executable installation path from registry or default paths
        private string GetVmwarePath()
        {
            try
            {
                using (var baseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64))
                {
                    using (var key = baseKey.OpenSubKey(@"SOFTWARE\VMware, Inc.\VMware Workstation"))
                    {
                        if (key != null)
                        {
                            object installPath = key.GetValue("Install Path");
                            if (installPath != null)
                            {
                                string fullPath = Path.Combine(installPath.ToString(), "vmware.exe");
                                if (File.Exists(fullPath)) return fullPath;
                            }
                        }
                    }
                }
            }
            catch { }

            try
            {
                using (var baseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32))
                {
                    using (var key = baseKey.OpenSubKey(@"SOFTWARE\VMware, Inc.\VMware Workstation"))
                    {
                        if (key != null)
                        {
                            object installPath = key.GetValue("Install Path");
                            if (installPath != null)
                            {
                                string fullPath = Path.Combine(installPath.ToString(), "vmware.exe");
                                if (File.Exists(fullPath)) return fullPath;
                            }
                        }
                    }
                }
            }
            catch { }

            string[] candidatePaths = new string[]
            {
        @"C:\Program Files (x86)\VMware\VMware Workstation\vmware.exe",
        @"C:\Program Files\VMware\VMware Workstation\vmware.exe"
            };

            foreach (string path in candidatePaths)
            {
                if (File.Exists(path)) return path;
            }

            return @"C:\Program Files (x86)\VMware\VMware Workstation\vmware.exe";
        }

        // Helper method to create a Windows shortcut (.lnk) file using WScript.Shell
        private void CreateShortcut(string shortcutPath, string targetPath, string iconPath, string arguments, string targetExecutable)
        {
            try
            {
                Type wshShellType = Type.GetTypeFromProgID("WScript.Shell");
                if (wshShellType != null)
                {
                    object wshShell = Activator.CreateInstance(wshShellType);
                    object shortcut = wshShellType.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, wshShell, new object[] { shortcutPath });

                    shortcut.GetType().InvokeMember("TargetPath", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { targetExecutable });
                    shortcut.GetType().InvokeMember("Arguments", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { arguments });
                    shortcut.GetType().InvokeMember("WorkingDirectory", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { Path.GetDirectoryName(targetPath) });

                    if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
                    {
                        shortcut.GetType().InvokeMember("IconLocation", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { iconPath + ",0" });
                    }

                    shortcut.GetType().InvokeMember("Save", System.Reflection.BindingFlags.InvokeMethod, null, shortcut, null);
                }
            }
            catch { }
        }

        // Event handler for changing virtual machine custom icon via Windows system icon picker
        private void buttonChangeIcon_Click(object sender, EventArgs e)
        {
            string defaultIconPath = Path.Combine(Environment.SystemDirectory, "shell32.dll");
            int iconIndex = 0;
            StringBuilder sbPath = new StringBuilder(defaultIconPath, 260);

            if (PickIconDlg(this.Handle, sbPath, sbPath.Capacity, ref iconIndex))
            {
                currentIconPath = sbPath.ToString();

                try
                {
                    IntPtr[] phiconLarge = new IntPtr[1];
                    IntPtr[] phiconSmall = new IntPtr[1];
                    ExtractIconEx(currentIconPath, iconIndex, phiconLarge, phiconSmall, 1);

                    if (phiconLarge[0] != IntPtr.Zero)
                    {
                        using (Icon ico = Icon.FromHandle(phiconLarge[0]))
                        {
                            pictureVMIcon.Image = ico.ToBitmap();
                        }

                        if (phiconSmall[0] != IntPtr.Zero)
                            DestroyIcon(phiconSmall[0]);
                    }
                }
                catch (Exception ex)
                {
                    string langFileNameIcon = ini.Read("LanguageFile", "General");
                    IniFile langIcon = !string.IsNullOrWhiteSpace(langFileNameIcon) ? new IniFile(langFileNameIcon) : null;
                    string errTitleIcon = (langIcon != null) ? langIcon.Read("ErrorTitle", "Language") : "Error";
                    if (string.IsNullOrWhiteSpace(errTitleIcon)) errTitleIcon = "Error";
                    MessageBox.Show("Could not load icon from selected file: " + ex.Message, errTitleIcon, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, GetRtlOptions(langIcon));
                }
            }
        }

        // Validate user inputs for Step 1 (Name and Directory)
        private void ValidateStep1()
        {
            bool isNameValid = !string.IsNullOrWhiteSpace(txtVMName.Text);
            bool isFolderValid = false;

            if (!string.IsNullOrWhiteSpace(txtVMFolder.Text))
            {
                try
                {
                    string folderPath = txtVMFolder.Text.Trim();
                    string rootPath = Path.GetPathRoot(folderPath);

                    if (!string.IsNullOrEmpty(rootPath) && Directory.Exists(rootPath))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                        isFolderValid = true;
                    }
                }
                catch
                {
                    isFolderValid = false;
                }
            }

            btnStep1Next.Enabled = isNameValid && isFolderValid;
        }

        // Auto-update folder path when virtual machine name changes, unless customized by user
        private void txtVMName_TextChanged(object sender, EventArgs e)
        {
            if (!isUserPathCustomized && !string.IsNullOrWhiteSpace(baseVmDirectory))
            {
                string currentName = txtVMName.Text.Trim();
                if (string.IsNullOrEmpty(currentName)) currentName = "New Virtual Machine";

                isUpdatingProgrammatically = true;
                txtVMFolder.Text = Path.Combine(baseVmDirectory, currentName);
                isUpdatingProgrammatically = false;
            }

            ValidateStep1();
        }

        private void txtVMFolder_TextChanged(object sender, EventArgs e)
        {
            if (!isUpdatingProgrammatically)
            {
                isUserPathCustomized = true;
            }

            ValidateStep1();
        }

        private void numericUpDownFloppyCount_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownFloppyCount.Value < 0)
                numericUpDownFloppyCount.Value = 0;
        }

        private void numericUpDownCDIDECount_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownCDIDECount.Value < numericUpDownCDIDECount.Minimum)
                numericUpDownCDIDECount.Value = numericUpDownCDIDECount.Minimum;
        }

        private void numericUpDownCDSCSICount_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownCDSCSICount.Value < numericUpDownCDSCSICount.Minimum)
                numericUpDownCDSCSICount.Value = numericUpDownCDSCSICount.Minimum;
        }

        private void numericUpDownCDSATACount_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownCDSATACount.Value < numericUpDownCDSATACount.Minimum)
                numericUpDownCDSATACount.Value = numericUpDownCDSATACount.Minimum;
        }

        // Load wizard icons from the resources directory or output directory and assign them to picture boxes
        private void LoadWizardIcons()
        {
            Image GetResourceImage(string imageName)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", imageName + ".png");
                if (!File.Exists(path))
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageName + ".png");
                }

                if (File.Exists(path))
                {
                    try
                    {
                        using (var tempImg = Image.FromFile(path))
                        {
                            return new Bitmap(tempImg);
                        }
                    }
                    catch { }
                }
                return null;
            }

            // Load 32x32 primary icons
            if (iconWelcome != null) iconWelcome.Image = GetResourceImage("iconWelcome");
            if (iconFinish != null) iconFinish.Image = GetResourceImage("iconFinish");
            if (iconFolder != null) iconFolder.Image = GetResourceImage("iconFolder");
            if (iconHardDrive != null) iconHardDrive.Image = GetResourceImage("iconHardDrive");
            // Ensure icons are above background
            try { pictureBoxStep1Background.SendToBack(); } catch { }
            try { pictureBoxStep4Background.SendToBack(); } catch { }
            try { panelStep1Background.BringToFront(); } catch { }
            try { panelStep4Background.BringToFront(); } catch { }
            try { iconFolder?.BringToFront(); } catch { }
            try { iconHardDrive?.BringToFront(); } catch { }

            // Load 16x16 secondary icons
            if (iconFirmware != null) iconFirmware.Image = GetResourceImage("iconFirmware");
            if (iconProcessor != null) iconProcessor.Image = GetResourceImage("iconProcessor");
            if (iconMemory != null) iconMemory.Image = GetResourceImage("iconMemory");
            if (iconSound != null) iconSound.Image = GetResourceImage("iconSound");
            if (iconSCSI != null) iconSCSI.Image = GetResourceImage("iconSCSI");
            if (iconNetwork != null) iconNetwork.Image = GetResourceImage("iconNetwork");

            if (iconFloppy != null)
            {
                iconFloppy.Image = GetResourceImage("iconFloppy");
                iconFloppy.SizeMode = PictureBoxSizeMode.Zoom;
                iconFloppy.BackColor = Color.Transparent;
            }

            // Use the shared iconCD.png image for all CD/DVD controller icons
            Image cdIcon = GetResourceImage("iconCD");
            PictureBox[] cdPictureBoxes = new PictureBox[] { iconCDIDE, iconCDSCSI, iconCDSATA };
            foreach (var cdPic in cdPictureBoxes)
            {
                if (cdPic != null)
                {
                    cdPic.Image = cdIcon;
                    cdPic.SizeMode = PictureBoxSizeMode.Zoom;
                    cdPic.BackColor = Color.Transparent;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkBoxFullScreen.Enabled = radioShortcutWorkstation.Checked && checkBoxShortStart.Checked;

            trackBarMemory.RightToLeft = RightToLeft.No;

            // Load wizard icons from Resources folder or output directory
            LoadWizardIcons();

            string langFileName = ini.Read("LanguageFile", "General");
            IniFile lang = null;

            if (!string.IsNullOrWhiteSpace(langFileName))
            {
                try { lang = new IniFile(langFileName); } catch { }
            }

            if (lang != null)
            {
                string customNotSupported = lang.Read("NotSupportedText", "Language");
                if (!string.IsNullOrWhiteSpace(customNotSupported))
                    localizedNotSupported = customNotSupported;

                string customRecommended = lang.Read("RecommendedText", "Language");
                if (!string.IsNullOrWhiteSpace(customRecommended))
                    localizedRecommended = customRecommended;
            }

            string osName = ini.Read("OS-Name", "General");
            string vmName = ini.Read("VM-Name", "General");
            string vmFolder = ini.Read("VM-Folder", "General");

            string iniIcon = ini.Read("Icon", "General");
            if (string.IsNullOrWhiteSpace(iniIcon))
            {
                currentIconPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            }
            else
            {
                currentIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iniIcon);
            }

            try
            {
                if (File.Exists(currentIconPath))
                {
                    using (Icon ico = Icon.ExtractAssociatedIcon(currentIconPath))
                    {
                        if (ico != null) pictureVMIcon.Image = ico.ToBitmap();
                    }
                }
            }
            catch { }

            if (string.IsNullOrWhiteSpace(vmName))
            {
                vmName = "My Virtual Machine";
            }
            txtVMName.Text = vmName;

            if (string.IsNullOrWhiteSpace(vmFolder))
            {
                string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                baseVmDirectory = Path.Combine(userHome, "Virtual Machines");
            }
            else
            {
                baseVmDirectory = vmFolder;
            }

            isUpdatingProgrammatically = true;
            txtVMFolder.Text = Path.Combine(baseVmDirectory, vmName);
            isUpdatingProgrammatically = false;

            labelWelcomeTitle.Text = $"Welcome to the VMware VM Generator wizard of {osName}.";
            labelWelcomeText.Text = $"This wizard will guide you through creating a virtual machine with {osName}.";

            labelFinishTitle.Text = $"Completing the VMware VM Generator wizard of {osName}.";
            labelFinishText.Text = $"You have successfully completed the VMware VM Generator wizard for {osName}.";

            this.Text = $"VMware {osName} VM Generator";

            comboNumberOfProcessors.Items.AddRange(new object[]
            {
        1, 2, 4, 8, 16, 32
            });

            comboBoxNumberOfCoresPerProcessor.Items.AddRange(new object[]
            {
        1, 2, 3, 4, 6, 8, 12, 16, 24, 32
            });

            comboNumberOfProcessors.SelectedIndex = 0;
            comboBoxNumberOfCoresPerProcessor.SelectedIndex = 0;

            UpdateTotalCores();

            trackBarMemory.Minimum = 0;
            trackBarMemory.Maximum = 1000;

            numericUpDownMemory.Minimum = 4;
            numericUpDownMemory.Maximum = 131072;

            tickHitBoxes = new Rectangle[memoryStops.Length];

            string iniMin = ini.Read("RecommendedMin", "Memory");
            string iniDefault = ini.Read("RecommendedDefault", "Memory");
            string iniMax = ini.Read("RecommendedMax", "Memory");

            recommendedMin = int.TryParse(iniMin, out int rMin) ? rMin : 0;
            recommendedDefault = int.TryParse(iniDefault, out int rDef) ? rDef : 256;
            recommendedMax = int.TryParse(iniMax, out int rMax) ? rMax : 0;

            if (recommendedMin > 0)
            {
                labelRecommendedMin.Text = $"Recommended Minimum: {FormatMB(recommendedMin)}";
                labelRecommendedMin.Visible = true;
            }
            else labelRecommendedMin.Visible = false;

            if (recommendedDefault > 0)
            {
                labelRecommendedDefault.Text = $"Recommended Default: {FormatMB(recommendedDefault)}";
                labelRecommendedDefault.Visible = true;
            }
            else labelRecommendedDefault.Visible = false;

            if (recommendedMax > 0)
            {
                labelRecommendedMax.Text = $"Recommended Maximum: {FormatMB(recommendedMax)}";
                labelRecommendedMax.Visible = true;
            }
            else labelRecommendedMax.Visible = false;

            trackBarMemory.Value = MBToSlider(recommendedDefault);
            numericUpDownMemory.Value = recommendedDefault;

            string fw = ini.Read("Firmware", "Firmware");

            bios.Text = "BIOS";
            uefi.Text = "UEFI";

            if (fw.Equals("BIOS-F", StringComparison.OrdinalIgnoreCase))
            {
                bios.Enabled = true;
                uefi.Enabled = false;
                bios.Checked = true;
                uefi.Checked = false;
            }
            else if (fw.Equals("UEFI-F", StringComparison.OrdinalIgnoreCase))
            {
                bios.Enabled = false;
                uefi.Enabled = true;
                uefi.Checked = true;
                bios.Checked = false;
            }
            else if (fw.Equals("BIOS", StringComparison.OrdinalIgnoreCase))
            {
                bios.Enabled = true;
                uefi.Enabled = true;
                bios.Checked = true;
                uefi.Checked = false;
            }
            else if (fw.Equals("UEFI", StringComparison.OrdinalIgnoreCase))
            {
                bios.Enabled = true;
                uefi.Enabled = true;
                uefi.Checked = true;
                bios.Checked = false;
            }
            else
            {
                bios.Enabled = true;
                uefi.Enabled = true;
                bios.Checked = true;
                uefi.Checked = false;
            }

            string iniCPUProc = ini.Read("Processors", "CPU");
            string iniCPUCores = ini.Read("CoresPerProcessor", "CPU");

            int cpuProcessors = int.TryParse(iniCPUProc, out int p) ? p : 1;
            int cpuCores = int.TryParse(iniCPUCores, out int c) ? c : 1;

            if (!comboNumberOfProcessors.Items.Contains(cpuProcessors))
                comboNumberOfProcessors.Items.Add(cpuProcessors);

            if (!comboBoxNumberOfCoresPerProcessor.Items.Contains(cpuCores))
                comboBoxNumberOfCoresPerProcessor.Items.Add(cpuCores);

            comboNumberOfProcessors.SelectedItem = cpuProcessors;
            comboBoxNumberOfCoresPerProcessor.SelectedItem = cpuCores;

            UpdateTotalCores();

            ShowStep(0);
            trackBarMemory.TickStyle = TickStyle.None;

            radioButtonNAT.Checked = true;
            radioButtonBridged.Checked = false;
            radioButtonHostOnly.Checked = false;
            radioButtonNoNetwork.Checked = false;

            string netDefault = ini.Read("Default", "Network");

            switch (netDefault.ToUpper())
            {
                case "BRIDGED":
                    radioButtonBridged.Checked = true;
                    break;
                case "HOSTONLY":
                    radioButtonHostOnly.Checked = true;
                    break;
                case "NONE":
                    radioButtonNoNetwork.Checked = true;
                    break;
                default:
                    radioButtonNAT.Checked = true;
                    break;
            }

            string scsiDefault = ini.Read("SCSI I/O Controller", "SCSI I/O Controller").ToUpper();

            string busLogicEnabled = ini.Read("BusLogic", "SCSI I/O Controller");
            string lsiLogicEnabled = ini.Read("LSILogic", "SCSI I/O Controller");
            string lsiLogicSASEnabled = ini.Read("LSILogicSAS", "SCSI I/O Controller");
            string paravirtualizedEnabled = ini.Read("Paravirtualized", "SCSI I/O Controller");

            radioBusLogic.Enabled = true;
            radioLSILogic.Enabled = true;
            radioLSILogicSAS.Enabled = true;
            radioParavirtualizedSCSI.Enabled = true;

            if (!string.IsNullOrWhiteSpace(busLogicEnabled) &&
              (busLogicEnabled.Equals("false", StringComparison.OrdinalIgnoreCase) || busLogicEnabled.Equals("disabled", StringComparison.OrdinalIgnoreCase)))
            {
                radioBusLogic.Enabled = false;
            }

            if (!string.IsNullOrWhiteSpace(lsiLogicEnabled) &&
              (lsiLogicEnabled.Equals("false", StringComparison.OrdinalIgnoreCase) || lsiLogicEnabled.Equals("disabled", StringComparison.OrdinalIgnoreCase)))
            {
                radioLSILogic.Enabled = false;
            }

            if (!string.IsNullOrWhiteSpace(lsiLogicSASEnabled) &&
              (lsiLogicSASEnabled.Equals("false", StringComparison.OrdinalIgnoreCase) || lsiLogicSASEnabled.Equals("disabled", StringComparison.OrdinalIgnoreCase)))
            {
                radioLSILogicSAS.Enabled = false;
            }

            if (!string.IsNullOrWhiteSpace(paravirtualizedEnabled) &&
              (paravirtualizedEnabled.Equals("false", StringComparison.OrdinalIgnoreCase) || paravirtualizedEnabled.Equals("disabled", StringComparison.OrdinalIgnoreCase)))
            {
                radioParavirtualizedSCSI.Enabled = false;
            }

            radioBusLogic.Checked = false;
            radioLSILogic.Checked = false;
            radioLSILogicSAS.Checked = false;
            radioParavirtualizedSCSI.Checked = false;

            switch (scsiDefault)
            {
                case "BUSLOGIC":
                    if (radioBusLogic.Enabled) radioBusLogic.Checked = true;
                    break;
                case "LSILogic":
                    if (radioLSILogic.Enabled) radioLSILogic.Checked = true;
                    break;
                case "PARAVIRTUAL":
                case "PARAVIRTUALIZED":
                    if (radioParavirtualizedSCSI.Enabled) radioParavirtualizedSCSI.Checked = true;
                    break;
                case "SAS":
                case "LSILOGICSAS":
                    if (radioLSILogicSAS.Enabled) radioLSILogicSAS.Checked = true;
                    break;
                default:
                    if (radioLSILogicSAS.Enabled) radioLSILogicSAS.Checked = true;
                    else if (radioBusLogic.Enabled) radioBusLogic.Checked = true;
                    else if (radioLSILogic.Enabled) radioLSILogic.Checked = true;
                    else if (radioParavirtualizedSCSI.Enabled) radioParavirtualizedSCSI.Checked = true;
                    break;
            }

            string soundDefault = ini.Read("Default", "Sound Card").ToLower();

            string sb16Enabled = ini.Read("SB16", "Sound Card");
            string es1371Enabled = ini.Read("ES1371", "Sound Card");
            string hdaEnabled = ini.Read("HDAudio", "Sound Card");

            radioButtonSB16.Enabled = !sb16Enabled.Equals("false", StringComparison.OrdinalIgnoreCase) &&
                    !sb16Enabled.Equals("disabled", StringComparison.OrdinalIgnoreCase);

            radioButtonES1371.Enabled = !es1371Enabled.Equals("false", StringComparison.OrdinalIgnoreCase) &&
                          !es1371Enabled.Equals("disabled", StringComparison.OrdinalIgnoreCase);

            radioButtonHDAudio.Enabled = !hdaEnabled.Equals("false", StringComparison.OrdinalIgnoreCase) &&
                          !hdaEnabled.Equals("disabled", StringComparison.OrdinalIgnoreCase);

            if (soundDefault == "sb16" && radioButtonSB16.Enabled)
                radioButtonSB16.Checked = true;
            else if (soundDefault == "es1371" && radioButtonES1371.Enabled)
                radioButtonES1371.Checked = true;
            else
                radioButtonHDAudio.Checked = true;

            if (!radioButtonSB16.Enabled)
                radioButtonSB16.Tag = localizedNotSupported;

            if (!radioButtonES1371.Enabled)
                radioButtonES1371.Tag = localizedNotSupported;

            if (!radioButtonHDAudio.Enabled)
                radioButtonHDAudio.Tag = localizedNotSupported;

            string driveDefault = ini.Read("DefaultType", "Virtual Drive").ToLower();

            string ideEnabled = ini.Read("IDE", "Virtual Drive");
            string scsiEnabled = ini.Read("SCSI", "Virtual Drive");
            string sataEnabled = ini.Read("SATA", "Virtual Drive");
            string nvmeEnabled = ini.Read("NVMe", "Virtual Drive");

            radioButtonIDE.Enabled = !ideEnabled.Equals("false", StringComparison.OrdinalIgnoreCase) &&
                    !ideEnabled.Equals("disabled", StringComparison.OrdinalIgnoreCase);

            radioButtonSCSI.Enabled = !scsiEnabled.Equals("false", StringComparison.OrdinalIgnoreCase) &&
                       !scsiEnabled.Equals("disabled", StringComparison.OrdinalIgnoreCase);

            radioButtonSATA.Enabled = !sataEnabled.Equals("false", StringComparison.OrdinalIgnoreCase) &&
                       !sataEnabled.Equals("disabled", StringComparison.OrdinalIgnoreCase);

            radioButtonNVMe.Enabled = !nvmeEnabled.Equals("false", StringComparison.OrdinalIgnoreCase) &&
                       !nvmeEnabled.Equals("disabled", StringComparison.OrdinalIgnoreCase);

            if (driveDefault == "ide" && radioButtonIDE.Enabled)
                radioButtonIDE.Checked = true;
            else if (driveDefault == "scsi" && radioButtonSCSI.Checked)
                radioButtonSCSI.Checked = true;
            else if (driveDefault == "nvme" && radioButtonNVMe.Enabled)
                radioButtonNVMe.Checked = true;
            else
                radioButtonSATA.Checked = true;

            string cap = ini.Read("DefaultCapacity", "Virtual Drive");
            int capacity = int.TryParse(cap, out int capValue) ? capValue : 64;
            numericUpDownDriveCapacity.Value = capacity;

            labelRecommendedDriveCapacity.Text = $"Recommended capacity for {osName}: {capacity} GB";

            string fmt = ini.Read("DefaultFormat", "Virtual Drive").ToLower();
            if (fmt == "single")
                radioButtonSingle.Checked = true;
            else
                radioButtonSplit.Checked = true;

            string alloc = ini.Read("AllocateNow", "Virtual Drive");
            checkBoxAllocate.Checked = alloc.Equals("true", StringComparison.OrdinalIgnoreCase);

            if (!radioButtonIDE.Enabled)
                radioButtonIDE.Tag = localizedNotSupported;
            if (!radioButtonSCSI.Enabled)
                radioButtonSCSI.Tag = localizedNotSupported;
            if (!radioButtonSATA.Enabled)
                radioButtonSATA.Tag = localizedNotSupported;
            if (!radioButtonNVMe.Enabled)
                radioButtonNVMe.Tag = localizedNotSupported;

            if (radioButtonIDE.Checked && radioButtonIDE.Enabled)
                radioButtonIDE.Tag = localizedRecommended;
            else if (radioButtonSCSI.Checked && radioButtonSCSI.Enabled)
                radioButtonSCSI.Tag = localizedRecommended;
            else if (radioButtonNVMe.Checked && radioButtonNVMe.Enabled)
                radioButtonNVMe.Tag = localizedRecommended;
            else if (radioButtonSATA.Checked && radioButtonSATA.Enabled)
                radioButtonSATA.Tag = localizedRecommended;

            int floppyCount = int.TryParse(ini.Read("FloppyCount", "Drives"), out int f) ? f : 1;
            int ideCount = int.TryParse(ini.Read("IDECDCount", "Drives"), out int i) ? i : 0;
            int scsiCount = int.TryParse(ini.Read("SCSICDCount", "Drives"), out int s) ? s : 0;
            int sataCount = int.TryParse(ini.Read("SATACDCount", "Drives"), out int a) ? a : 0;

            string bootController = ini.Read("OS-Controller", "BootCD").ToUpper();

            numericUpDownCDIDECount.Minimum = 0;
            numericUpDownCDSCSICount.Minimum = 0;
            numericUpDownCDSATACount.Minimum = 0;

            if (bootController == "IDE")
            {
                if (ideCount == 0) ideCount = 1;
                numericUpDownCDIDECount.Minimum = 1;
            }
            else if (bootController == "SCSI")
            {
                if (scsiCount == 0) scsiCount = 1;
                numericUpDownCDSCSICount.Minimum = 1;
            }
            else if (bootController == "SATA")
            {
                if (scsiCount == 0) sataCount = 1;
                numericUpDownCDSATACount.Minimum = 1;
            }

            numericUpDownFloppyCount.Value = floppyCount;
            numericUpDownCDIDECount.Value = ideCount;
            numericUpDownCDSCSICount.Value = scsiCount;
            numericUpDownCDSATACount.Value = sataCount;

            string translatedController = bootController;

            if (lang != null)
            {
                string transKey = "SumDrive" + bootController;
                string localizedCtrl = lang.Read(transKey, "Language");
                if (!string.IsNullOrWhiteSpace(localizedCtrl))
                    translatedController = localizedCtrl;
            }

            string cdControllerTemplate = lang != null ? lang.Read("labelUsedCDController", "Language") : "";
            if (string.IsNullOrWhiteSpace(cdControllerTemplate))
                cdControllerTemplate = "The controller used for the disc of the operating system: {CDcontroller}";

            labelUsedCDController.Text = cdControllerTemplate.Replace("{CDcontroller}", translatedController);

            if (lang != null)
                ApplyLanguage(lang, osName);
            else
            {
                SetDefaultRecommendedTag();
            }

            // Read and apply custom UI settings (Color and Font Name) safely from settings.ini
            ApplyCustomUIsettings();

            ValidateStep1();
        }

        // Read and apply custom text color and font name safely from settings.ini [UI] section while preserving original control sizes and styles and enforcing color exemptions for controls
        private void ApplyCustomUIsettings()
        {
            try
            {
                string colorName = ini.Read("TextColor", "UI");
                Color? customTextColor = null;
                if (!string.IsNullOrWhiteSpace(colorName))
                {
                    customTextColor = ColorTranslator.FromHtml(colorName);
                }

                string fontName = ini.Read("FontName", "UI");

                ApplyUIStyleToControls(this, customTextColor, fontName);
            }
            catch
            {
                // Fallback if parsing fails
            }
        }

        // Helper method to recursively apply text color and font name while keeping original size and style, and exempting buttons, input fields, and memory labels
        private void ApplyUIStyleToControls(Control container, Color? color, string fontName)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is Button || ctrl is CustomButton || ctrl is TextBox || ctrl is ComboBox || ctrl is NumericUpDown)
                {
                    // Buttons, TextBoxes, ComboBoxes, and NumericUpDowns must always have black text
                    ctrl.ForeColor = Color.Black;
                }
                else if (ctrl == labelRecommendedMin)
                {
                    ctrl.ForeColor = Color.Red;
                }
                else if (ctrl == labelRecommendedDefault)
                {
                    ctrl.ForeColor = Color.Green;
                }
                else if (ctrl == labelRecommendedMax)
                {
                    ctrl.ForeColor = Color.Blue;
                }
                else if (color.HasValue && (ctrl is Label || ctrl is CheckBox || ctrl is RadioButton || ctrl is GroupBox))
                {
                    ctrl.ForeColor = color.Value;
                }

                if (!string.IsNullOrWhiteSpace(fontName))
                {
                    // Preserve each control's original size and style (like Bold for titles)
                    ctrl.Font = new Font(fontName, ctrl.Font.Size, ctrl.Font.Style);
                }

                if (ctrl.HasChildren)
                {
                    ApplyUIStyleToControls(ctrl, color, fontName);
                }
            }
        }

        private void SetDefaultRecommendedTag()
        {
            if (radioButtonHDAudio.Enabled && radioButtonHDAudio.Checked)
                radioButtonHDAudio.Tag = localizedRecommended;
            else if (radioButtonSB16.Enabled && radioButtonSB16.Checked)
                radioButtonSB16.Tag = localizedRecommended;
            else if (radioButtonES1371.Enabled && radioButtonES1371.Checked)
                radioButtonES1371.Tag = localizedRecommended;

            if (radioBusLogic.Enabled && radioBusLogic.Checked)
                radioBusLogic.Tag = localizedRecommended;
            else if (radioLSILogic.Enabled && radioLSILogic.Checked)
                radioLSILogic.Tag = localizedRecommended;
            else if (radioLSILogicSAS.Enabled && radioLSILogicSAS.Checked)
                radioLSILogicSAS.Tag = localizedRecommended;
            else if (radioParavirtualizedSCSI.Enabled && radioParavirtualizedSCSI.Checked)
                radioParavirtualizedSCSI.Tag = localizedRecommended;
        }

        private string FormatMB(int mb)
        {
            if (mb < 1024)
                return $"{mb} MB";
            return $"{mb / 1024} GB";
        }

        // Loads a PNG with preserved alpha channel into a control
        // For PictureBox, uses Image property to keep transparency
        // For Panel, uses BackgroundImage
        private void LoadBackground(Control control, string imageName)
        {
            if (control == null) return;

            // Resolve image path from Resources folder or base directory
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", imageName + ".png");
            if (!File.Exists(path))
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageName + ".png");
            }

            if (File.Exists(path))
            {
                try
                {
                    using (var tempImg = Image.FromFile(path))
                    {
                        // Create 32bpp ARGB bitmap to preserve alpha channel
                        Bitmap bmp = new Bitmap(tempImg.Width, tempImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(Color.Transparent);
                            g.DrawImage(tempImg, 0, 0, tempImg.Width, tempImg.Height);
                        }

                        if (control is PictureBox pb)
                        {
                            // Clean up previous image to avoid GDI leak
                            if (pb.Image != null)
                            {
                                var old = pb.Image;
                                pb.Image = null;
                                old.Dispose();
                            }
                            pb.Image = bmp;
                            pb.SizeMode = PictureBoxSizeMode.StretchImage;
                            pb.BackColor = Color.Transparent;
                        }
                        else
                        {
                            // For panels, use BackgroundImage with alpha support
                            if (control.BackgroundImage != null)
                            {
                                var oldBg = control.BackgroundImage;
                                control.BackgroundImage = null;
                                oldBg.Dispose();
                            }
                            control.BackgroundImage = bmp;
                            control.BackgroundImageLayout = ImageLayout.Stretch;
                            // Panel BackColor must be transparent for child PictureBox to show parent
                            control.BackColor = Color.Transparent;
                        }
                    }
                }
                catch { }
            }
        }

        // Loads all wizard frame images for a given step and fixes transparency hierarchy
        // WinForms transparent BackColor only shows Parent background, not siblings
        // So TopRight must be child of Top to correctly blend
        private void LoadPageLayout(string pagePrefix)
        {
            // Load common background elements
            LoadBackground(this.Controls.Find($"panel{pagePrefix}Background", true).FirstOrDefault(), "WizardBackground");
            LoadBackground(this.Controls.Find($"pictureBox{pagePrefix}Background", true).FirstOrDefault(), "WizardBackground");

            LoadBackground(this.Controls.Find($"panel{pagePrefix}Bottom", true).FirstOrDefault(), "WizardBottom");
            LoadBackground(this.Controls.Find($"pictureBox{pagePrefix}Bottom", true).FirstOrDefault(), "WizardBottom");

            LoadBackground(this.Controls.Find($"panel{pagePrefix}Left", true).FirstOrDefault(), "WizardLeft");
            LoadBackground(this.Controls.Find($"pictureBox{pagePrefix}Left", true).FirstOrDefault(), "WizardLeft");

            // Load top strip first (parent for TopRight)
            var panelTop = this.Controls.Find($"panel{pagePrefix}Top", true).FirstOrDefault();
            var picTop = this.Controls.Find($"pictureBox{pagePrefix}Top", true).FirstOrDefault() as PictureBox;
            LoadBackground(panelTop, "WizardTop");
            LoadBackground(picTop, "WizardTop");

            // Load top-right corner
            var panelTopRight = this.Controls.Find($"panel{pagePrefix}TopRight", true).FirstOrDefault();
            var picTopRight = this.Controls.Find($"pictureBox{pagePrefix}TopRight", true).FirstOrDefault() as PictureBox;

            LoadBackground(panelTopRight, "WizardTopRight");
            LoadBackground(picTopRight, "WizardTopRight");

            // Fix transparency: make TopRight a child of Top so its transparent pixels show Top underneath
            if (picTopRight != null)
            {
                Control parentForTransparency = null;

                // Prefer PictureBox Top as parent if it has an image
                if (picTop != null && picTop.Image != null)
                    parentForTransparency = picTop;
                else if (panelTop != null)
                    parentForTransparency = panelTop;

                if (parentForTransparency != null)
                {
                    // Preserve screen position when reparenting
                    Point screenPos = picTopRight.PointToScreen(Point.Empty);
                    Point parentPos = parentForTransparency.PointToScreen(Point.Empty);
                    Point newLocation = new Point(screenPos.X - parentPos.X, screenPos.Y - parentPos.Y);

                    picTopRight.Parent = parentForTransparency;
                    picTopRight.Location = newLocation;
                    picTopRight.BackColor = Color.Transparent;
                    picTopRight.BringToFront();
                }
            }

            // Same fix for panel version if used
            if (panelTopRight != null && panelTop != null && panelTopRight.Parent != panelTop)
            {
                Point screenPos = panelTopRight.PointToScreen(Point.Empty);
                Point parentPos = panelTop.PointToScreen(Point.Empty);
                Point newLocation = new Point(screenPos.X - parentPos.X, screenPos.Y - parentPos.Y);

                panelTopRight.Parent = panelTop;
                panelTopRight.Location = newLocation;
                panelTopRight.BackColor = Color.Transparent;
                panelTopRight.BringToFront();
            }
        }

        private void MakeControlsTransparent(Control container)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is Label || ctrl is CheckBox || ctrl is RadioButton)
                {
                    ctrl.BackColor = Color.Transparent;
                    ctrl.BringToFront();
                }
                else if (ctrl is GroupBox gb)
                {
                    gb.BackColor = Color.Transparent;
                    MakeControlsTransparent(gb);
                }
                else if (ctrl is Panel pnl)
                {
                    if (pnl.BackgroundImage == null)
                    {
                        pnl.BackColor = Color.Transparent;
                    }
                    MakeControlsTransparent(pnl);
                }
                else if (ctrl.HasChildren)
                {
                    MakeControlsTransparent(ctrl);
                }
            }
        }

        private void ShowStep(int step)
        {
            panelWelcome.Visible = (step == 0);
            panelStep1.Visible = (step == 1);
            panelStep2.Visible = (step == 2);
            panelStep3.Visible = (step == 3);
            panelStep4.Visible = (step == 4);
            panelStep5.Visible = (step == 5);
            panelStep6.Visible = (step == 6);
            panelStep7.Visible = (step == 7);
            panelFinish.Visible = (step == 8);

            string pagePrefix = "";
            Panel activePanel = null;

            switch (step)
            {
                case 0: pagePrefix = "Welcome"; activePanel = panelWelcome; break;
                case 1: pagePrefix = "Step1"; activePanel = panelStep1; break;
                case 2: pagePrefix = "Step2"; activePanel = panelStep2; break;
                case 3: pagePrefix = "Step3"; activePanel = panelStep3; break;
                case 4: pagePrefix = "Step4"; activePanel = panelStep4; break;
                case 5: pagePrefix = "Step5"; activePanel = panelStep5; break;
                case 6: pagePrefix = "Step6"; activePanel = panelStep6; break;
                case 7: pagePrefix = "Step7"; activePanel = panelStep7; break;
                case 8: pagePrefix = "Finish"; activePanel = panelFinish; break;
            }

            if (activePanel != null)
            {
                activePanel.BackColor = Color.Transparent;
                if (!string.IsNullOrEmpty(pagePrefix))
                {
                    LoadPageLayout(pagePrefix);

                    var pb = this.Controls.Find($"pictureBox{pagePrefix}Background", true).FirstOrDefault();
                    var pnlBg = this.Controls.Find($"panel{pagePrefix}Background", true).FirstOrDefault();
                    var pbTop = this.Controls.Find($"pictureBox{pagePrefix}Top", true).FirstOrDefault();
                    var pbLeft = this.Controls.Find($"pictureBox{pagePrefix}Left", true).FirstOrDefault();
                    var pbBottom = this.Controls.Find($"pictureBox{pagePrefix}Bottom", true).FirstOrDefault();
                    var pbTopRight = this.Controls.Find($"pictureBox{pagePrefix}TopRight", true).FirstOrDefault();

                    try
                    {
                        pb?.SendToBack();
                        pnlBg?.SendToBack();
                        pbTop?.SendToBack();
                        pbLeft?.SendToBack();
                        pbBottom?.SendToBack();

                        Control backgroundControl = (Control)pb ?? pnlBg;
                        if (backgroundControl != null)
                        {
                            var children = activePanel.Controls.Cast<Control>().ToArray();
                            foreach (var c in children)
                            {
                                if (c == backgroundControl) continue;
                                if (c is PictureBox pbc && pbc.Name != "pictureVMIcon" && pbc.Name != "iconFloppy" && !pbc.Name.StartsWith("iconCD")) continue;
                                if (c is Panel) continue;

                                if (c is Label || c is CheckBox || c is RadioButton || c is GroupBox || c is TrackBar ||
                                    (c is PictureBox && (c.Name == "pictureVMIcon" || c.Name == "iconFloppy" || c.Name.StartsWith("iconCD"))))
                                {
                                    try
                                    {
                                        var screenPt = c.Parent.PointToScreen(c.Location);
                                        var newLoc = backgroundControl.PointToClient(screenPt);
                                        var oldAnchor = c.Anchor;
                                        var oldDock = c.Dock;
                                        c.Dock = DockStyle.None;
                                        c.Parent = backgroundControl;
                                        c.Location = newLoc;
                                        c.Anchor = oldAnchor;
                                        c.Dock = oldDock;
                                        try { c.BackColor = Color.Transparent; } catch { }
                                        if (c is TrackBar tb)
                                        {
                                            try { tb.BackColor = Color.Transparent; tb.TickStyle = TickStyle.None; } catch { }
                                        }
                                        if (c is PictureBox pic && (pic.Name == "pictureVMIcon" || pic.Name == "iconFloppy" || pic.Name.StartsWith("iconCD")))
                                        {
                                            try { pic.BackColor = Color.Transparent; pic.BringToFront(); } catch { }
                                        }
                                        c.BringToFront();
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch { }
                }

                MakeControlsTransparent(activePanel);

                try
                {
                    var backgroundControl = this.Controls.Find($"pictureBox{pagePrefix}Background", true).FirstOrDefault() as Control
                                         ?? this.Controls.Find($"panel{pagePrefix}Background", true).FirstOrDefault() as Control;
                    if (backgroundControl != null)
                    {
                        void FixIcon(PictureBox icon)
                        {
                            if (icon == null) return;
                            try
                            {
                                var screenPt = icon.Parent.PointToScreen(icon.Location);
                                var newLoc = backgroundControl.PointToClient(screenPt);
                                icon.Parent = backgroundControl;
                                icon.Location = newLoc;
                            }
                            catch { icon.Parent = backgroundControl; }
                            icon.Visible = true;
                            icon.SizeMode = PictureBoxSizeMode.Zoom;
                            icon.BackColor = Color.Transparent;
                            icon.BringToFront();
                        }
                        if (step == 0) FixIcon(iconWelcome);
                        if (step == 1) FixIcon(iconFolder);
                        if (step == 4) FixIcon(iconHardDrive);
                        if (step == 8) FixIcon(iconFinish);
                    }
                }
                catch { }

                foreach (Control c in activePanel.Controls)
                {
                    if (c is Label || c is CheckBox || c is RadioButton || c is GroupBox)
                        c.BringToFront();
                }

                RefreshSuffixLabels();

                activePanel.Invalidate(true);
            }
        }

        private void btnWelcomeNext_Click(object sender, EventArgs e) => ShowStep(1);
        private void btnWelcomeCancel_Click(object sender, EventArgs e) => Close();
        private void btnWelcomeBack_Click(object sender, EventArgs e) => ShowStep(0);

        private void btnStep1Back_Click(object sender, EventArgs e) => ShowStep(0);
        private void btnStep1Next_Click(object sender, EventArgs e) => ShowStep(2);
        private void btnStep1Cancel_Click(object sender, EventArgs e) => Close();

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder to save the virtual machine";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    isUpdatingProgrammatically = true;
                    txtVMFolder.Text = dialog.SelectedPath;
                    isUpdatingProgrammatically = false;

                    isUserPathCustomized = true;

                    ValidateStep1();
                }
            }
        }

        private void btnStep2Back_Click(object sender, EventArgs e) => ShowStep(1);
        private void btnStep2Next_Click(object sender, EventArgs e) => ShowStep(3);
        private void btnStep2Cancel_Click(object sender, EventArgs e) => Close();

        private void bios_CheckedChanged(object sender, EventArgs e)
        {
            if (bios.Checked) uefi.Checked = false;
        }

        private void uefi_CheckedChanged(object sender, EventArgs e)
        {
            if (uefi.Checked) bios.Checked = false;
        }

        private void UpdateTotalCores()
        {
            int processors = 1;
            int cores = 1;

            if (comboNumberOfProcessors.SelectedItem is int p)
                processors = p;
            else if (!int.TryParse(comboNumberOfProcessors.Text, out processors))
                processors = 1;

            if (comboBoxNumberOfCoresPerProcessor.SelectedItem is int c)
                cores = c;
            else if (!int.TryParse(comboBoxNumberOfCoresPerProcessor.Text, out cores))
                cores = 1;

            if (processors > 0 && cores > 0)
            {
                int total = processors * cores;
                labelTotalOfCoresX.Text = total.ToString();
            }
        }

        private int SliderToMB(int sliderValue)
        {
            double t = sliderValue / 1000.0;
            double mb = 4 * Math.Pow(2, t * 15);
            int rounded = (int)Math.Round(mb / 4.0) * 4;
            if (rounded < 4) rounded = 4;
            return rounded;
        }

        private int MBToSlider(int mb)
        {
            if (mb <= 4) return 0;
            double t = Math.Log(mb / 4.0, 2) / 15.0;
            int sliderVal = (int)Math.Round(t * 1000);
            if (sliderVal > 1000) sliderVal = 1000;
            if (sliderVal < 0) sliderVal = 0;
            return sliderVal;
        }

        private void trackBarMemory_Scroll(object sender, EventArgs e)
        {
            numericUpDownMemory.Value = SliderToMB(trackBarMemory.Value);
            memory.Invalidate();
        }

        private void numericUpDownMemory_ValueChanged(object sender, EventArgs e)
        {
            int val = (int)numericUpDownMemory.Value;

            if (val % 4 != 0)
            {
                int adjusted = (int)Math.Round(val / 4.0) * 4;
                if (adjusted < (int)numericUpDownMemory.Minimum) adjusted = (int)numericUpDownMemory.Minimum;
                if (adjusted > (int)numericUpDownMemory.Maximum) adjusted = (int)numericUpDownMemory.Maximum;
                if (numericUpDownMemory.Value != adjusted)
                {
                    numericUpDownMemory.Value = adjusted;
                    return;
                }
            }

            try
            {
                int sliderVal = MBToSlider(val);
                if (sliderVal >= trackBarMemory.Minimum && sliderVal <= trackBarMemory.Maximum)
                {
                    trackBarMemory.Value = sliderVal;
                }
            }
            catch { }
            memory.Invalidate();
        }

        private void SetMemory(int mb)
        {
            if (mb <= 0) return;

            int adjusted = (int)Math.Round(mb / 4.0) * 4;
            if (adjusted < (int)numericUpDownMemory.Minimum) adjusted = (int)numericUpDownMemory.Minimum;
            if (adjusted > (int)numericUpDownMemory.Maximum) adjusted = (int)numericUpDownMemory.Maximum;

            numericUpDownMemory.Value = adjusted;
            int sliderVal = MBToSlider(adjusted);
            if (sliderVal >= trackBarMemory.Minimum && sliderVal <= trackBarMemory.Maximum)
            {
                trackBarMemory.Value = sliderVal;
            }
            memory.Invalidate();
        }

        private void memory_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int left = trackBarMemory.Left + 13;
            int right = trackBarMemory.Right - 13;
            int y = trackBarMemory.Top + trackBarMemory.Height - 3;

            using (Font smallFont = new Font(this.Font.FontFamily, 7))
            {
                for (int i = 0; i < memoryStops.Length; i++)
                {
                    int mb = memoryStops[i];
                    double t = Math.Log(mb / 4.0, 2) / 15.0;
                    int sliderPos = (int)(t * trackBarMemory.Maximum);
                    int x = left + (int)((right - left) * (sliderPos / (double)trackBarMemory.Maximum));

                    g.DrawLine(Pens.Black, x, y, x, y + 6);
                    string label = mb < 1024 ? $"{mb} MB" : $"{mb / 1024} GB";
                    SizeF textSize = g.MeasureString(label, smallFont);
                    g.DrawString(label, smallFont, Brushes.Black, x - textSize.Width / 2, y + 8);

                    tickHitBoxes[i] = new Rectangle(x - 10, y, 20, 20);
                }
            }

            DrawRecommendedTicks(g);
        }

        private void DrawRecommendedTicks(Graphics g)
        {
            int left = trackBarMemory.Left + 13;
            int right = trackBarMemory.Right - 13;
            int y = trackBarMemory.Top + 2;

            if (recommendedMin > 0) DrawColoredTick(g, recommendedMin, Color.Red, left, right, y);
            if (recommendedDefault > 0) DrawColoredTick(g, recommendedDefault, Color.Green, left, right, y);
            if (recommendedMax > 0) DrawColoredTick(g, recommendedMax, Color.Blue, left, right, y);
        }

        private void DrawColoredTick(Graphics g, int mb, Color color, int left, int right, int y)
        {
            double t = Math.Log(mb / 4.0, 2) / 15.0;
            int sliderPos = (int)(t * trackBarMemory.Maximum);
            int x = left + (int)((right - left) * (sliderPos / (double)trackBarMemory.Maximum));

            using (Pen p = new Pen(color, 2))
            {
                g.DrawLine(p, x, y, x, y - 8);
            }
        }

        private void memory_MouseMove(object sender, MouseEventArgs e)
        {
            int left = trackBarMemory.Left + 13;
            int right = trackBarMemory.Right - 13;
            int y = trackBarMemory.Top + 2;

            if (recommendedMin > 0 && IsRecommendedTickClicked(e.Location, recommendedMin, left, right, y)) { Cursor = Cursors.Hand; return; }
            if (recommendedDefault > 0 && IsRecommendedTickClicked(e.Location, recommendedDefault, left, right, y)) { Cursor = Cursors.Hand; return; }
            if (recommendedMax > 0 && IsRecommendedTickClicked(e.Location, recommendedMax, left, right, y)) { Cursor = Cursors.Hand; return; }

            foreach (Rectangle r in tickHitBoxes)
            {
                if (r.Contains(e.Location)) { Cursor = Cursors.Hand; return; }
            }

            Cursor = Cursors.Default;
        }

        private bool IsRecommendedTickClicked(Point mouse, int mb, int left, int right, int y)
        {
            double t = Math.Log(mb / 4.0, 2) / 15.0;
            int sliderPos = (int)(t * trackBarMemory.Maximum);
            int x = left + (int)((right - left) * (sliderPos / (double)trackBarMemory.Maximum));
            Rectangle hitbox = new Rectangle(x - 10, y - 30, 20, 40);
            return hitbox.Contains(mouse);
        }

        private void memory_MouseDown(object sender, MouseEventArgs e)
        {
            int left = trackBarMemory.Left + 13;
            int right = trackBarMemory.Right - 13;
            int y = trackBarMemory.Top + 2;

            if (recommendedMin > 0 && IsRecommendedTickClicked(e.Location, recommendedMin, left, right, y)) { SetMemory(recommendedMin); return; }
            if (recommendedDefault > 0 && IsRecommendedTickClicked(e.Location, recommendedDefault, left, right, y)) { SetMemory(recommendedDefault); return; }
            if (recommendedMax > 0 && IsRecommendedTickClicked(e.Location, recommendedMax, left, right, y)) { SetMemory(recommendedMax); return; }

            for (int i = 0; i < memoryStops.Length; i++)
            {
                if (tickHitBoxes[i].Contains(e.Location))
                {
                    SetMemory(memoryStops[i]);
                    break;
                }
            }
        }

        private void ApplyLanguage(IniFile lang, string osName)
        {
            if (lang == null) return;

            string customTitleTemplate = lang.Read("WindowTitle", "Language");
            if (!string.IsNullOrWhiteSpace(customTitleTemplate))
            {
                string vmNameForTitle = "";
                try { vmNameForTitle = txtVMName != null ? txtVMName.Text.Trim() : ""; } catch { }
                this.Text = customTitleTemplate.Replace("{osName}", osName).Replace("{vmName}", vmNameForTitle);
            }

            string customNotSupported = lang.Read("NotSupportedText", "Language");
            if (!string.IsNullOrWhiteSpace(customNotSupported)) localizedNotSupported = customNotSupported;

            string customRecommended = lang.Read("RecommendedText", "Language");
            if (!string.IsNullOrWhiteSpace(customRecommended)) localizedRecommended = customRecommended;

            SetDefaultRecommendedTag();

            foreach (Control ctrl in this.Controls)
                ApplyLanguageToControl(ctrl, lang, osName);

            string direction = lang.Read("Direction", "Language");
            ApplyDirection(direction);
        }

        private void ApplyLanguageToControl(Control ctrl, IniFile lang, string osName)
        {
            string translated = lang.Read(ctrl.Name, "Language");
            if (!string.IsNullOrWhiteSpace(translated))
            {
                if (ctrl.Name == nameof(labelRecommendedMin) && recommendedMin > 0)
                    ctrl.Text = $"{translated} {FormatMB(recommendedMin)}";
                else if (ctrl.Name == nameof(labelRecommendedDefault) && recommendedDefault > 0)
                    ctrl.Text = $"{translated} {FormatMB(recommendedDefault)}";
                else if (ctrl.Name == nameof(labelRecommendedMax) && recommendedMax > 0)
                    ctrl.Text = $"{translated} {FormatMB(recommendedMax)}";
                else
                {
                    if (ctrl.Name == nameof(labelRecommendedDriveCapacity) && numericUpDownDriveCapacity != null)
                    {
                        int capVal = (int)numericUpDownDriveCapacity.Value;
                        string vmNameForCap = "";
                        try { vmNameForCap = txtVMName != null ? txtVMName.Text.Trim() : ""; } catch { }
                        ctrl.Text = translated.Replace("{osName}", osName).Replace("{capacity}", capVal.ToString()).Replace("{vmName}", vmNameForCap);
                    }
                    else if (ctrl.Name == nameof(labelUsedCDController))
                    {
                        string bootController = ini.Read("OS-Controller", "BootCD").ToUpper();
                        string translatedController = bootController;
                        string transKey = "SumDrive" + bootController;
                        string localizedCtrl = lang.Read(transKey, "Language");
                        if (!string.IsNullOrWhiteSpace(localizedCtrl))
                            translatedController = localizedCtrl;

                        ctrl.Text = translated.Replace("{CDcontroller}", translatedController);
                    }
                    else
                    {
                        string vmNameForLabel = "";
                        try { vmNameForLabel = txtVMName != null ? txtVMName.Text.Trim() : ""; } catch { }
                        ctrl.Text = translated.Replace("{osName}", osName).Replace("{vmName}", vmNameForLabel);
                    }
                }
            }

            string tooltip = lang.Read(ctrl.Name + "_ToolTip", "Language");
            if (!string.IsNullOrWhiteSpace(tooltip)) toolTip1.SetToolTip(ctrl, tooltip);

            string recSuffix = lang.Read(ctrl.Name + "_Suffix", "Language");
            if (!string.IsNullOrWhiteSpace(recSuffix)) controlRecommendedSuffix[ctrl.Name] = recSuffix;

            string notSup = lang.Read(ctrl.Name + "_NotSupported", "Language");
            if (!string.IsNullOrWhiteSpace(notSup)) controlNotSupportedSuffix[ctrl.Name] = notSup;

            foreach (Control child in ctrl.Controls)
                ApplyLanguageToControl(child, lang, osName);
        }

        private void ApplyDirection(string direction)
        {
            if (string.IsNullOrWhiteSpace(direction)) direction = "LTR";
            bool rtl = direction.Equals("RTL", StringComparison.OrdinalIgnoreCase);

            this.RightToLeft = rtl ? RightToLeft.Yes : RightToLeft.No;
            this.RightToLeftLayout = rtl;

            foreach (Control ctrl in this.Controls)
                ApplyDirectionToControl(ctrl, rtl);

            if (rtl) MirrorControls(this);
        }

        private void MirrorControls(Control container)
        {
            int width = container.ClientSize.Width;
            foreach (Control ctrl in container.Controls.Cast<Control>().ToArray())
            {
                if (ctrl == trackBarMemory) continue;
                int newX = width - ctrl.Left - ctrl.Width;
                ctrl.Left = newX;
                if (ctrl.Controls.Count > 0) MirrorControls(ctrl);
            }
        }

        private void ApplyDirectionToControl(Control ctrl, bool rtl)
        {
            if (ctrl == trackBarMemory)
            {
                ctrl.RightToLeft = RightToLeft.No;
                return;
            }

            ctrl.RightToLeft = rtl ? RightToLeft.Yes : RightToLeft.No;
            foreach (Control child in ctrl.Controls)
                ApplyDirectionToControl(child, rtl);
        }

        private void Form1_Shown(object? sender, EventArgs e)
        {
            RefreshSuffixLabels();
        }

        private void RefreshSuffixLabels()
        {
            var oldLabels = this.Controls.Find("*_SuffixLabel", true);
            foreach (var lbl in oldLabels)
            {
                if (lbl is Control c)
                {
                    c.Parent?.Controls.Remove(c);
                    c.Dispose();
                }
            }

            AppendSuffixLabels(this, localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioBusLogic", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioLSILogic", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioLSILogicSAS", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioParavirtualizedSCSI", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioButtonSB16", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioButtonES1371", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioButtonHDAudio", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioButtonIDE", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioButtonSCSI", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioButtonSATA", localizedNotSupported, localizedRecommended);
            AppendSuffixLabelForControlByName("radioButtonNVMe", localizedNotSupported, localizedRecommended);

            foreach (Control ctrl in this.Controls.Find("*_SuffixLabel", true))
            {
                ctrl.BringToFront();
                if (ctrl.Parent != null)
                {
                    ctrl.Parent.Controls.SetChildIndex(ctrl, 0);
                }
            }
        }

        private void AppendSuffixLabelForControlByName(string controlName, string notSupportedText, string recommendedText)
        {
            var matches = this.Controls.Find(controlName, true);
            if (matches == null || matches.Length == 0) return;

            foreach (var ctrl in matches)
            {
                if (ctrl is Label && ctrl.Name.EndsWith("_SuffixLabel")) continue;

                bool isEffectivelyEnabled = ctrl.Enabled;
                for (Control p = ctrl.Parent; p != null; p = p.Parent)
                {
                    isEffectivelyEnabled &= p.Enabled;
                }

                bool isNotSupported = !isEffectivelyEnabled;
                string labelText = null;

                if (isNotSupported)
                {
                    if (controlNotSupportedSuffix.TryGetValue(ctrl.Name, out var perCtrlNotSup) && !string.IsNullOrWhiteSpace(perCtrlNotSup))
                        labelText = perCtrlNotSup;
                    else labelText = notSupportedText;
                }
                else
                {
                    if (controlRecommendedSuffix.TryGetValue(ctrl.Name, out var perCtrlRec) && !string.IsNullOrWhiteSpace(perCtrlRec))
                        labelText = perCtrlRec;
                    else if (ctrl.Tag is string t && !string.IsNullOrWhiteSpace(t))
                        labelText = t;
                }

                if (string.IsNullOrWhiteSpace(labelText)) continue;

                Control parent = ctrl.Parent ?? this;
                string labelName = ctrl.Name + "_SuffixLabel";

                var existingLabel = parent.Controls.OfType<Label>().FirstOrDefault(l => l.Name == labelName);
                if (existingLabel != null)
                {
                    parent.Controls.Remove(existingLabel);
                    existingLabel.Dispose();
                }

                Label lbl = new Label
                {
                    Name = labelName,
                    Text = labelText,
                    AutoSize = true,
                    Font = new Font(this.Font.FontFamily, 8, FontStyle.Italic),
                    ForeColor = isNotSupported ? Color.Gray : Color.Green
                };

                int margin = 6;
                int preferredX, y;

                if (ctrl is RadioButton rb)
                {
                    int radioBoxWidth = 20;

                    if (rb.Height > 30)
                    {
                        y = rb.Top + rb.Height - 4;
                        preferredX = rb.Left + radioBoxWidth;
                    }
                    else
                    {
                        Size textSize = TextRenderer.MeasureText(rb.Text, rb.Font);
                        y = rb.Top + Math.Max(0, (rb.Height - lbl.PreferredHeight) / 2);

                        if (this.RightToLeft == RightToLeft.Yes)
                        {
                            preferredX = rb.Left + rb.Width - radioBoxWidth - textSize.Width - lbl.PreferredWidth - margin;
                        }
                        else
                        {
                            preferredX = rb.Left + radioBoxWidth + textSize.Width + margin;
                        }
                    }
                }
                else
                {
                    if (this.RightToLeft == RightToLeft.Yes)
                    {
                        preferredX = ctrl.Left - lbl.PreferredWidth - margin;
                    }
                    else
                    {
                        preferredX = ctrl.Left + ctrl.Width + margin;
                    }
                    y = ctrl.Top + Math.Max(0, (ctrl.Height - lbl.PreferredHeight) / 2);
                }

                parent.Controls.Add(lbl);
                lbl.Location = new Point(preferredX, y);
                lbl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                lbl.BringToFront();
            }
        }

        private void AppendSuffixLabels(Control container, string notSupportedText, string recommendedText)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is Label && ctrl.Name.EndsWith("_SuffixLabel")) continue;

                string labelText = null;
                bool isEffectivelyEnabled = ctrl.Enabled;
                for (Control p = ctrl.Parent; p != null; p = p.Parent)
                {
                    isEffectivelyEnabled &= p.Enabled;
                }

                bool isNotSupported = !isEffectivelyEnabled;

                if (isNotSupported)
                {
                    if (controlNotSupportedSuffix.TryGetValue(ctrl.Name, out var perCtrlNotSup) && !string.IsNullOrWhiteSpace(perCtrlNotSup))
                        labelText = perCtrlNotSup;
                    else labelText = notSupportedText;
                }
                else
                {
                    if (controlRecommendedSuffix.TryGetValue(ctrl.Name, out var perCtrlRec) && !string.IsNullOrWhiteSpace(perCtrlRec))
                        labelText = perCtrlRec;
                    else if (ctrl.Tag is string t && !string.IsNullOrWhiteSpace(t))
                        labelText = t;
                }

                if (!string.IsNullOrWhiteSpace(labelText))
                {
                    Control parent = ctrl.Parent ?? container;
                    string labelName = ctrl.Name + "_SuffixLabel";

                    var existingLabel = parent.Controls.OfType<Label>().FirstOrDefault(l => l.Name == labelName);
                    if (existingLabel != null)
                    {
                        parent.Controls.Remove(existingLabel);
                        existingLabel.Dispose();
                    }

                    Label lbl = new Label
                    {
                        Name = labelName,
                        Text = labelText,
                        AutoSize = true,
                        Font = new Font(this.Font.FontFamily, 8, FontStyle.Italic),
                        ForeColor = isNotSupported ? Color.Gray : Color.Green
                    };

                    int margin = 6;
                    int preferredX, y;

                    if (ctrl is RadioButton rb)
                    {
                        int radioBoxWidth = 20;

                        if (rb.Height > 30)
                        {
                            y = rb.Top + rb.Height - 4;
                            preferredX = rb.Left + radioBoxWidth;
                        }
                        else
                        {
                            Size textSize = TextRenderer.MeasureText(rb.Text, rb.Font);
                            y = rb.Top + Math.Max(0, (rb.Height - lbl.PreferredHeight) / 2);

                            if (this.RightToLeft == RightToLeft.Yes)
                            {
                                preferredX = rb.Left + rb.Width - radioBoxWidth - textSize.Width - lbl.PreferredWidth - margin;
                            }
                            else
                            {
                                preferredX = rb.Left + radioBoxWidth + textSize.Width + margin;
                            }
                        }
                    }
                    else
                    {
                        if (this.RightToLeft == RightToLeft.Yes)
                        {
                            preferredX = ctrl.Left - lbl.PreferredWidth - margin;
                        }
                        else
                        {
                            preferredX = ctrl.Left + ctrl.Width + margin;
                        }
                        y = ctrl.Top + Math.Max(0, (ctrl.Height - lbl.PreferredHeight) / 2);
                    }

                    parent.Controls.Add(lbl);
                    lbl.Location = new Point(preferredX, y);
                    lbl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    lbl.BringToFront();
                }

                if (ctrl.Controls.Count > 0)
                    AppendSuffixLabels(ctrl, notSupportedText, recommendedText);
            }
        }

        private void GenerateSummary()
        {
            string osName = ini.Read("OS-Name", "General");
            string vmName = txtVMName.Text;
            string vmFolder = txtVMFolder.Text;

            string langFileName = ini.Read("LanguageFile", "General");
            IniFile lang = !string.IsNullOrWhiteSpace(langFileName) ? new IniFile(langFileName) : null;

            string GetTr(string key, string defaultText)
            {
                if (lang != null)
                {
                    string val = lang.Read(key, "Language");
                    if (!string.IsNullOrWhiteSpace(val))
                        return val;
                }
                return defaultText;
            }

            string titleTemplate = GetTr("SummaryTitleTemplate", "Summary of {vmName} virtual machine configuration:");
            string titleWithVm = titleTemplate.Replace("{vmName}", vmName).Replace("{osName}", osName);

            // labelSummaryTitle should show Ready text, not SummaryTitleTemplate
            string readyTitle = GetTr("labelSummaryTitle", "Ready to create virtual machine");
            if (!string.IsNullOrWhiteSpace(readyTitle))
                labelSummaryTitle.Text = readyTitle.Replace("{vmName}", vmName).Replace("{osName}", osName);
            else
                labelSummaryTitle.Text = titleWithVm;

            // Fix the label that displays SummaryTitleTemplate (the one above txtSummary)
            try
            {
                var summaryTemplateCtrl = this.Controls.Find("SummaryTitleTemplate", true).FirstOrDefault();
                if (summaryTemplateCtrl != null)
                    summaryTemplateCtrl.Text = titleWithVm;

                var altNames = new[] { "labelSummaryTemplate", "labelSummarySubtitle", "labelSummaryHeader" };
                foreach (var alt in altNames)
                {
                    var ctrlAlt = this.Controls.Find(alt, true).FirstOrDefault();
                    if (ctrlAlt != null && ctrlAlt.Text.Contains("{vmName}"))
                        ctrlAlt.Text = titleWithVm;
                }

                if (panelStep7 != null)
                {
                    foreach (Control panelCtrl in panelStep7.Controls)
                    {
                        if (panelCtrl is Label lbl && lbl.Text.Contains("{vmName}"))
                            lbl.Text = titleWithVm;
                        foreach (Control child in panelCtrl.Controls)
                        {
                            if (child is Label lbl2 && lbl2.Text.Contains("{vmName}"))
                                lbl2.Text = titleWithVm;
                            foreach (Control grand in child.Controls)
                            {
                                if (grand is Label lbl3 && lbl3.Text.Contains("{vmName}"))
                                    lbl3.Text = titleWithVm;
                            }
                        }
                    }
                }
            }
            catch { }

            int processors = comboNumberOfProcessors.SelectedItem is int p ? p : int.TryParse(comboNumberOfProcessors.Text, out int parsedP) ? parsedP : 1;
            int cores = comboBoxNumberOfCoresPerProcessor.SelectedItem is int coreCountVal ? coreCountVal : int.TryParse(comboBoxNumberOfCoresPerProcessor.Text, out int parsedC) ? parsedC : 1;

            int memoryMB = (int)numericUpDownMemory.Value;
            string memoryStr = memoryMB < 1024 ? $"{memoryMB} MB" : $"{memoryMB / 1024} GB";

            string firmware = bios.Checked ? GetTr("SumBIOS", "BIOS") : GetTr("SumUEFI", "UEFI");

            string netKey = radioButtonBridged.Checked ? "SumNetBridged" :
                    radioButtonHostOnly.Checked ? "SumNetHostOnly" :
                    radioButtonNoNetwork.Checked ? "SumNetNone" : "SumNetNAT";
            string networkType = GetTr(netKey, "NAT");

            string soundKey = radioButtonHDAudio.Enabled && radioButtonHDAudio.Checked ? "SumSoundHDAudio" :
                     radioButtonSB16.Enabled && radioButtonSB16.Checked ? "SumSoundSB16" :
                     radioButtonES1371.Enabled && radioButtonES1371.Checked ? "SumSoundES1371" : "SumSoundNone";
            string soundCard = GetTr(soundKey, "None");

            string scsiKey = radioBusLogic.Enabled && radioBusLogic.Checked ? "SumSCSIBusLogic" :
                    radioLSILogic.Enabled && radioLSILogic.Checked ? "SumSCSILSILogic" :
                    radioLSILogicSAS.Enabled && radioLSILogicSAS.Checked ? "SumSCSILSISAS" : "SumSCSIParavirtual";
            string scsiController = GetTr(scsiKey, "LSI Logic SAS");

            string driveKey = radioButtonIDE.Checked ? "SumDriveIDE" :
                     radioButtonSCSI.Checked ? "SumDriveSCSI" :
                     radioButtonSATA.Checked ? "SumDriveSATA" : "SumDriveNVMe";
            string driveType = GetTr(driveKey, "SATA");

            int driveCapacity = (int)numericUpDownDriveCapacity.Value;
            bool allocateNow = checkBoxAllocate.Checked;
            bool splitFile = radioButtonSplit.Checked;

            int floppyCount = (int)numericUpDownFloppyCount.Value;
            int cdIDECount = (int)numericUpDownCDIDECount.Value;
            int cdSCSICount = (int)numericUpDownCDSCSICount.Value;
            int cdSATACount = (int)numericUpDownCDSATACount.Value;

            string headerSummary = GetTr("SumHeaderSummary", $"=== Virtual Machine Summary ===");
            string sectionGeneral = GetTr("SumSecGeneral", "[General]");
            string lblVMName = GetTr("SumLblVMName", "Virtual Machine Name:");
            string lblFolder = GetTr("SumLblFolder", "Destination Folder:");
            string sectionHardware = GetTr("SumSecHardware", "[System & Hardware]");
            string lblFirmware = GetTr("SumLblFirmware", "Firmware Type:");
            string lblProcessors = GetTr("SumLblProcessors", "Processors:");
            string lblCoresPerProcessor = GetTr("SumLblCoresPerProcessor", "Cores per processor:");
            string lblMemory = GetTr("SumLblMemory", "Memory (RAM):");
            string sectionPeripherals = GetTr("SumSecPeripherals", "[Peripherals & Controllers]");
            string lblNetwork = GetTr("SumLblNetwork", "Network Connection:");
            string lblSound = GetTr("SumLblSound", "Sound Card:");
            string lblSCSI = GetTr("SumLblSCSI", "SCSI Controller:");
            string sectionDisk = GetTr("SumSecDisk", "[Virtual Hard Disk]");
            string lblType = GetTr("SumLblType", "Type:");
            string lblCapacity = GetTr("SumLblCapacity", "Capacity:");
            string lblAllocate = GetTr("SumLblAllocate", "Allocate All Space Now:");
            string lblFormat = GetTr("SumLblFormat", "Storage Format:");
            string valYes = GetTr("SumValYes", "Yes");
            string valNo = GetTr("SumValNo", "No");
            string valSplit = GetTr("SumValSplit", "Split into multiple files");
            string valSingle = GetTr("SumValSingle", "Single file");
            string sectionRemovable = GetTr("SumSecRemovable", "[Removable Media / Drives]");
            string lblFloppy = GetTr("SumLblFloppy", "Floppy Drives:");
            string lblCDIDE = GetTr("SumLblCDIDE", "CD/DVD Drives (IDE):");
            string lblCDSCSI = GetTr("SumLblCDSCSI", "CD/DVD Drives (SCSI):");
            string lblCDSATA = GetTr("SumLblCDSATA", "CD/DVD Drives (SATA):");

            txtSummary.Text =
              $"{headerSummary}\r\n\r\n" +
              $"{sectionGeneral}\r\n" +
              $"• {lblVMName} {vmName}\r\n" +
              $"• {lblFolder} {vmFolder}\r\n\r\n" +
              $"{sectionHardware}\r\n" +
              $"• {lblFirmware} {firmware}\r\n" +
              $"• {lblProcessors} {processors}\r\n" +
              $"• {lblCoresPerProcessor} {cores}\r\n" +
              $"• {lblMemory} {memoryStr}\r\n\r\n" +
              $"{sectionPeripherals}\r\n" +
              $"• {lblNetwork} {networkType}\r\n" +
              $"• {lblSound} {soundCard}\r\n" +
              $"• {lblSCSI} {scsiController}\r\n\r\n" +
              $"{sectionDisk}\r\n" +
              $"• {lblType} {driveType}\r\n" +
              $"• {lblCapacity} {driveCapacity} GB\r\n" +
              $"• {lblAllocate} {(allocateNow ? valYes : valNo)}\r\n" +
              $"• {lblFormat} {(splitFile ? valSplit : valSingle)}\r\n\r\n" +
              $"{sectionRemovable}\r\n" +
              $"• {lblFloppy} {floppyCount}\r\n" +
              $"• {lblCDIDE} {cdIDECount}\r\n" +
              $"• {lblCDSCSI} {cdSCSICount}\r\n" +
              $"• {lblCDSATA} {cdSATACount}\r\n";
        }
    }
}
