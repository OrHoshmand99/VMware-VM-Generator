using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

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
        private int currentIconIndex = 0; // Track exact icon index selected by the user

        // Pending action variables for post-wizard execution upon finishing
        private string pendingVmxPath = "";
        private string pendingVmFolder = "";
        private string pendingVmwarePath = "";

        // Per-control suffix overrides loaded from language file
        private readonly Dictionary<string, string> controlRecommendedSuffix =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> controlNotSupportedSuffix =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Localized suffix texts (fallback to English defaults)
        private string localizedNotSupported = "(Not supported)";
        private string localizedRecommended = "(Recommended)";

        // Safe RTL title bar fix: title bar RTL with right-aligned text
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_LAYOUTRTL = 0x00400000;
                const int WS_EX_NOINHERITLAYOUT = 0x00100000;
                const int WS_EX_RTLREADING = 0x00002000;
                const int WS_EX_RIGHT = 0x00001000;

                CreateParams cp = base.CreateParams;
                if (this.RightToLeft == RightToLeft.Yes)
                {
                    cp.ExStyle |= WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT | WS_EX_RTLREADING | WS_EX_RIGHT;
                }
                return cp;
            }
        }

        // Helper method to set title with proper RTL embedding
        private void SetFormTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                this.Text = title;
                return;
            }
            string clean = title.TrimStart((char)0x200F, (char)0x200E, (char)0x202A, (char)0x202B, (char)0x202C, (char)0x202D, (char)0x202E);
            if (this.RightToLeft == RightToLeft.Yes)
            {
                this.Text = ((char)0x202B).ToString() + clean + ((char)0x202C).ToString();
            }
            else
            {
                this.Text = clean;
            }
        }

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

        // P/Invoke for native Windows Icon Picker dialog and high-quality multi-resolution icon extraction
        [DllImport("shell32.dll", EntryPoint = "PickIconDlg", CharSet = CharSet.Auto)]
        private static extern bool PickIconDlg(IntPtr hwndOwner, StringBuilder pszIconPath, int cchIconPath, ref int piIconIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int PrivateExtractIcons(
            string lpszFile,
            int nIconIndex,
            int cxIcon,
            int cyIcon,
            IntPtr[] phicon,
            int[] piconid,
            int nIcons,
            int flags
        );

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);

        private const int WM_SETICON = 0x0080;
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;
        private const int SM_CXSMICON = 49;
        private const int SM_CXICON = 11;
        private const int WM_DPICHANGED = 0x02E0;

        private IntPtr currentSmallIconHandle = IntPtr.Zero;
        private IntPtr currentLargeIconHandle = IntPtr.Zero;

        // P/Invoke for reading PE resources to extract native icon layers and color depths
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool EnumResourceNames(IntPtr hModule, IntPtr lpszType, EnumResNameProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumResNameProc(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);

        public Form1()
        {
            InitializeComponent();
            pictureVMIcon.SizeMode = PictureBoxSizeMode.Zoom;

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
                    string vmFolder = txtVMFolder.Text.Trim();
                    string vmName = txtVMName.Text.Trim();
                    if (string.IsNullOrWhiteSpace(vmName)) vmName = "New Virtual Machine";

                    if (!Directory.Exists(vmFolder))
                    {
                        Directory.CreateDirectory(vmFolder);
                    }

                    string vmxFilePath = Path.Combine(vmFolder, vmName + ".vmx");

                    string networkSelection = "nat";
                    if (radioButtonBridged.Checked) networkSelection = "bridged";
                    else if (radioButtonHostOnly.Checked) networkSelection = "hostonly";
                    else if (radioButtonNoNetwork.Checked) networkSelection = "none";

                    // Read hardware version from the INI file (Hardware Version section), fallback to "19" if empty
                    string hwVersionIni = ini.Read("HardwareVersion", "Hardware Version");
                    if (string.IsNullOrWhiteSpace(hwVersionIni)) hwVersionIni = "19";

                    // Read network adapter type from INI file if explicitly defined
                    string iniEthernetDev = ini.Read("ethernet0.virtualDev", "Network");

                    // Read OS-Type from settings.ini [General] section, fallback to "other-64" if empty
                    string osTypeIni = ini.Read("OS-Type", "General");
                    if (string.IsNullOrWhiteSpace(osTypeIni)) osTypeIni = "other-64";

                    // Read ISO file path from [BootCD] section in settings.ini[cite: 1, 3]
                    string isoFileName = ini.Read("OS-ISO", "BootCD");
                    string isoFullPath = "";
                    if (!string.IsNullOrWhiteSpace(isoFileName))
                    {
                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        string combinedPath = Path.Combine(baseDir, isoFileName);
                        isoFullPath = File.Exists(combinedPath) ? combinedPath : isoFileName;
                    }

                    // Get processor count and cores per processor safely
                    int processors = comboNumberOfProcessors.SelectedItem is int p ? p : (int.TryParse(comboNumberOfProcessors.Text, out int parsedP) ? parsedP : 1);
                    int coresPerProcessor = comboBoxNumberOfCoresPerProcessor.SelectedItem is int c ? c : (int.TryParse(comboBoxNumberOfCoresPerProcessor.Text, out int parsedC) ? parsedC : 1);

                    // Read CD/DVD counts from UI controls
                    int ideCdCount = (int)numericUpDownCDIDECount.Value;
                    int scsiCdCount = (int)numericUpDownCDSCSICount.Value;
                    int sataCdCount = (int)numericUpDownCDSATACount.Value;

                    // Enforce minimum 1 drive for the controller hosting the boot OS ISO to prevent invalid 0 count
                    string bootController = ini.Read("OS-Controller", "BootCD").ToUpper();
                    if (!string.IsNullOrWhiteSpace(isoFullPath))
                    {
                        if (bootController == "IDE" && ideCdCount < 1)
                        {
                            ideCdCount = 1;
                            numericUpDownCDIDECount.Value = 1;
                        }
                        else if (bootController == "SCSI" && scsiCdCount < 1)
                        {
                            scsiCdCount = 1;
                            numericUpDownCDSCSICount.Value = 1;
                        }
                        else if (bootController == "SATA" && sataCdCount < 1)
                        {
                            sataCdCount = 1;
                            numericUpDownCDSATACount.Value = 1;
                        }
                    }

                    var config = new VirtualMachineGenerator.VmConfig
                    {
                        VmName = vmName,
                        MemoryMb = (int)numericUpDownMemory.Value,
                        NumVcpus = processors, // UI socket count; VmGenerator calculates total vCPUs as Sockets * CoresPerSocket
                        CoresPerSocket = coresPerProcessor,

                        Firmware = uefi.Checked ? "efi" : "bios",
                        NetworkType = networkSelection,

                        SoundVirtualDev = radioButtonSB16.Checked ? "sb16" :
                            radioButtonES1371.Checked ? "es1371" :
                            radioButtonHDAudio.Checked ? "hdaudio" : "hdaudio",

                        StorageControllerType = radioButtonIDE.Checked ? "IDE" :
                            radioButtonSCSI.Checked ? "SCSI" :
                            radioButtonNVMe.Checked ? "NVMe" : "SATA",

                        ScsiVirtualDev = radioBusLogic.Checked ? "" :
                            radioLSILogic.Checked ? "lsilogic" :
                            radioLSILogicSAS.Checked ? "lsisas1068" : "pvscsi",

                        DiskPath = Path.Combine(vmFolder, vmName + ".vmdk"),
                        IsoPath = isoFullPath, // Assigned ISO path correctly

                        FloppyCount = (int)numericUpDownFloppyCount.Value,

                        IdeCdCount = ideCdCount,
                        ScsiCdCount = scsiCdCount,
                        SataCdCount = sataCdCount,

                        IconPath = "",
                        HardwareVersion = hwVersionIni, // Assign the hardware version read from INI
                        EthernetVirtualDev = iniEthernetDev, // Assign the network adapter type read from INI dynamically
                        GuestOs = osTypeIni // Assign the OS-Type read from INI dynamically[cite: 1, 3]
                    };

                    VirtualMachineGenerator.CreateVirtualMachine(vmxFilePath, config);

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

                                string langFileNameErrRaw = ini.Read("LanguageFile", "General");
                                string langFileNameErr = !string.IsNullOrWhiteSpace(langFileNameErrRaw) ? Environment.ExpandEnvironmentVariables(langFileNameErrRaw) : "";
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
                        string langFileNameErr2Raw = ini.Read("LanguageFile", "General");
                        string langFileNameErr2 = !string.IsNullOrWhiteSpace(langFileNameErr2Raw) ? Environment.ExpandEnvironmentVariables(langFileNameErr2Raw) : "";
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

                    string targetIconPath = Path.Combine(vmFolder, vmName + ".ico");
                    if (!string.IsNullOrEmpty(currentIconPath) && File.Exists(currentIconPath))
                    {
                        if (currentIconPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                        {
                            File.Copy(currentIconPath, targetIconPath, true);
                        }
                        else
                        {
                            SaveIconWithAllSizes(currentIconPath, currentIconIndex, targetIconPath);
                        }
                        config.IconPath = targetIconPath;
                    }

                    string vmwarePath = GetVmwarePath();

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
                    else
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

                    if (checkBoxDesktop.Checked)
                    {
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        string shortcutPath = Path.Combine(desktopPath, vmName + ".lnk");
                        CreateShortcut(shortcutPath, vmxFilePath, targetIconPath, shortcutArguments, shortcutTarget);
                    }

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

                    pendingVmxPath = vmxFilePath;
                    pendingVmFolder = vmFolder;
                    pendingVmwarePath = vmwarePath;

                    ShowStep(8);
                }
                catch (Exception ex)
                {
                    string langFileNameRaw = ini.Read("LanguageFile", "General");
                    string langFileName = !string.IsNullOrWhiteSpace(langFileNameRaw) ? Environment.ExpandEnvironmentVariables(langFileNameRaw) : "";
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

            buttonChangeIcon.Click += buttonChangeIcon_Click;
            btnBrowseFolder.Click += btnBrowseFolder_Click;

            txtVMName.TextChanged += txtVMName_TextChanged;
            txtVMFolder.TextChanged += txtVMFolder_TextChanged;

            bios.CheckedChanged += bios_CheckedChanged;
            uefi.CheckedChanged += uefi_CheckedChanged;

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

            checkBoxShortStart.CheckedChanged += (s, e) => {
                checkBoxFullScreen.Enabled = radioShortcutWorkstation.Checked && checkBoxShortStart.Checked;
                if (!checkBoxShortStart.Checked)
                {
                    checkBoxFullScreen.Checked = false;
                }
            };

            comboNumberOfProcessors.SelectedIndexChanged += (s, e) => UpdateTotalCores();
            comboBoxNumberOfCoresPerProcessor.SelectedIndexChanged += (s, e) => UpdateTotalCores();

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

            this.Shown += Form1_Shown;

            numericUpDownFloppyCount.ValueChanged += numericUpDownFloppyCount_ValueChanged;
            numericUpDownCDIDECount.ValueChanged += numericUpDownCDIDECount_ValueChanged;
            numericUpDownCDSCSICount.ValueChanged += numericUpDownCDSCSICount_ValueChanged;
            numericUpDownCDSATACount.ValueChanged += numericUpDownCDSATACount_ValueChanged;
        }

        private void SaveIconWithAllSizes(string dllPath, int iconIndex, string targetIcoPath)
        {
            IntPtr hModule = LoadLibraryEx(dllPath, IntPtr.Zero, 0x00000002);
            if (hModule == IntPtr.Zero)
            {
                FallbackExtractIcon(dllPath, iconIndex, targetIcoPath);
                return;
            }

            try
            {
                List<IntPtr> groupIconNames = new List<IntPtr>();
                EnumResNameProc enumProc = (mod, type, name, param) =>
                {
                    groupIconNames.Add(name);
                    return true;
                };

                IntPtr RT_GROUP_ICON = (IntPtr)14;
                EnumResourceNames(hModule, RT_GROUP_ICON, enumProc, IntPtr.Zero);

                if (iconIndex < 0 || iconIndex >= groupIconNames.Count)
                {
                    FreeLibrary(hModule);
                    FallbackExtractIcon(dllPath, iconIndex, targetIcoPath);
                    return;
                }

                IntPtr resName = groupIconNames[iconIndex];
                IntPtr hResInfo = FindResource(hModule, resName, RT_GROUP_ICON);
                if (hResInfo == IntPtr.Zero)
                {
                    FreeLibrary(hModule);
                    FallbackExtractIcon(dllPath, iconIndex, targetIcoPath);
                    return;
                }

                IntPtr hResData = LoadResource(hModule, hResInfo);
                if (hResData == IntPtr.Zero)
                {
                    FreeLibrary(hModule);
                    FallbackExtractIcon(dllPath, iconIndex, targetIcoPath);
                    return;
                }

                IntPtr pResLock = LockResource(hResData);
                uint groupSize = SizeofResource(hModule, hResInfo);
                if (pResLock == IntPtr.Zero || groupSize <= 6)
                {
                    FreeLibrary(hModule);
                    FallbackExtractIcon(dllPath, iconIndex, targetIcoPath);
                    return;
                }

                byte[] groupBuffer = new byte[groupSize];
                Marshal.Copy(pResLock, groupBuffer, 0, (int)groupSize);

                ushort idCount = BitConverter.ToUInt16(groupBuffer, 4);
                var iconEntries = new List<(byte width, byte height, byte colorCount, ushort planes, ushort bitCount, byte[] iconData)>();

                int offset = 6;
                IntPtr RT_ICON = (IntPtr)3;

                for (int i = 0; i < idCount && offset + 14 <= groupBuffer.Length; i++)
                {
                    byte width = groupBuffer[offset];
                    byte height = groupBuffer[offset + 1];
                    byte colorCount = groupBuffer[offset + 2];
                    byte reserved = groupBuffer[offset + 3];
                    ushort planes = BitConverter.ToUInt16(groupBuffer, offset + 4);
                    ushort bitCount = BitConverter.ToUInt16(groupBuffer, offset + 6);
                    uint bytesInRes = BitConverter.ToUInt32(groupBuffer, offset + 8);
                    ushort nID = BitConverter.ToUInt16(groupBuffer, offset + 12);

                    IntPtr hIconResInfo = FindResource(hModule, (IntPtr)nID, RT_ICON);
                    if (hIconResInfo != IntPtr.Zero)
                    {
                        IntPtr hIconResData = LoadResource(hModule, hIconResInfo);
                        if (hIconResData != IntPtr.Zero)
                        {
                            IntPtr pIconLock = LockResource(hIconResData);
                            uint iconDataSize = SizeofResource(hModule, hIconResInfo);
                            if (pIconLock != IntPtr.Zero && iconDataSize > 0)
                            {
                                byte[] iconData = new byte[iconDataSize];
                                Marshal.Copy(pIconLock, iconData, 0, (int)iconDataSize);
                                iconEntries.Add((width, height, colorCount, planes, bitCount, iconData));
                            }
                        }
                    }

                    offset += 14;
                }

                FreeLibrary(hModule);

                if (iconEntries.Count == 0)
                {
                    FallbackExtractIcon(dllPath, iconIndex, targetIcoPath);
                    return;
                }

                using (FileStream fs = new FileStream(targetIcoPath, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        writer.Write((ushort)0);
                        writer.Write((ushort)1);
                        writer.Write((ushort)iconEntries.Count);

                        int currentOffset = 6 + (16 * iconEntries.Count);

                        foreach (var entry in iconEntries)
                        {
                            writer.Write(entry.width);
                            writer.Write(entry.height);
                            writer.Write(entry.colorCount);
                            writer.Write((byte)0);
                            writer.Write(entry.planes);
                            writer.Write(entry.bitCount);
                            writer.Write((uint)entry.iconData.Length);
                            writer.Write((uint)currentOffset);
                            currentOffset += entry.iconData.Length;
                        }

                        foreach (var entry in iconEntries)
                        {
                            writer.Write(entry.iconData);
                        }
                    }
                }
            }
            catch
            {
                try { FreeLibrary(hModule); } catch { }
                FallbackExtractIcon(dllPath, iconIndex, targetIcoPath);
            }
        }

        private void FallbackExtractIcon(string dllPath, int iconIndex, string targetIcoPath)
        {
            int[] sizes = new int[] { 16, 20, 24, 32, 40, 48, 64, 72, 80, 96, 128, 256 };
            var imageInfos = new List<(byte width, byte height, byte[] data)>();

            foreach (int size in sizes)
            {
                IntPtr[] phicon = new IntPtr[1];
                int[] piconid = new int[1];
                PrivateExtractIcons(dllPath, iconIndex, size, size, phicon, piconid, 1, 0);

                if (phicon[0] != IntPtr.Zero)
                {
                    try
                    {
                        using (Icon ico = Icon.FromHandle(phicon[0]))
                        {
                            using (Bitmap bmp = ico.ToBitmap())
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    byte[] pngData = ms.ToArray();
                                    byte w = (size >= 256) ? (byte)0 : (byte)size;
                                    byte h = (size >= 256) ? (byte)0 : (byte)size;
                                    imageInfos.Add((w, h, pngData));
                                }
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        DestroyIcon(phicon[0]);
                    }
                }
            }

            if (imageInfos.Count == 0)
            {
                using (Icon ico = Icon.ExtractAssociatedIcon(dllPath))
                {
                    if (ico != null)
                    {
                        using (var fs = new FileStream(targetIcoPath, FileMode.Create))
                        {
                            ico.Save(fs);
                        }
                    }
                }
                return;
            }

            using (FileStream fs = new FileStream(targetIcoPath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write((ushort)0);
                    writer.Write((ushort)1);
                    writer.Write((ushort)imageInfos.Count);

                    int currentOffset = 6 + (16 * imageInfos.Count);

                    foreach (var img in imageInfos)
                    {
                        writer.Write(img.width);
                        writer.Write(img.height);
                        writer.Write((byte)0);
                        writer.Write((byte)0);
                        writer.Write((ushort)1);
                        writer.Write((ushort)32);
                        writer.Write((uint)img.data.Length);
                        writer.Write((uint)currentOffset);
                        currentOffset += img.data.Length;
                    }

                    foreach (var img in imageInfos)
                    {
                        writer.Write(img.data);
                    }
                }
            }
        }

        private MessageBoxOptions GetRtlOptions(IniFile langFile)
        {
            string dir = langFile != null ? langFile.Read("Direction", "Language") : "";
            bool isRtl = !string.IsNullOrWhiteSpace(dir) && dir.Equals("RTL", StringComparison.OrdinalIgnoreCase);
            return isRtl ? (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) : 0;
        }

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

        private void buttonChangeIcon_Click(object sender, EventArgs e)
        {
            string defaultIconPath = Path.Combine(Environment.SystemDirectory, "shell32.dll");
            int iconIndex = currentIconIndex;
            StringBuilder sbPath = new StringBuilder(defaultIconPath, 260);
            if (!string.IsNullOrEmpty(currentIconPath) && File.Exists(currentIconPath))
                sbPath = new StringBuilder(currentIconPath, 260);

            if (PickIconDlg(this.Handle, sbPath, sbPath.Capacity, ref iconIndex))
            {
                currentIconPath = sbPath.ToString();
                currentIconIndex = iconIndex;
                try
                {
                    string tempIcoPath = Path.Combine(Path.GetTempPath(), "vm_gen_temp_preview.ico");
                    bool isIco = currentIconPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase);
                    if (isIco) tempIcoPath = currentIconPath;
                    else SaveIconWithAllSizes(currentIconPath, currentIconIndex, tempIcoPath);
                    if (File.Exists(tempIcoPath))
                    {
                        SetFormIconSmart(tempIcoPath, isIco ? -1 : currentIconIndex, true);
                        pictureVMIcon.Image = GetBestPreviewBitmapFromIcoPath(tempIcoPath, 32);
                    }
                }
                catch (Exception ex)
                {
                    string langFileNameIconRaw = ini.Read("LanguageFile", "General");
                    string langFileNameIcon = !string.IsNullOrWhiteSpace(langFileNameIconRaw) ? Environment.ExpandEnvironmentVariables(langFileNameIconRaw) : "";
                    IniFile langIcon = !string.IsNullOrWhiteSpace(langFileNameIcon) ? new IniFile(langFileNameIcon) : null;
                    string errTitleIcon = (langIcon != null) ? langIcon.Read("ErrorTitle", "Language") : "Error";
                    if (string.IsNullOrWhiteSpace(errTitleIcon)) errTitleIcon = "Error";
                    MessageBox.Show("Could not load icon from selected file: " + ex.Message, errTitleIcon, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, GetRtlOptions(langIcon));
                }
            }
        }

        private void SetFormIconSmart(string iconPath, int iconIndex, bool pathIsIcoFile)
        {
            int captionSize = GetSystemMetrics(SM_CXSMICON);
            if (captionSize <= 0) captionSize = 16;
            int dpi = 96;
            try { uint d = GetDpiForWindow(this.Handle); if (d > 0) dpi = (int)d; } catch { }
            int taskbarSize = (int)Math.Round(24 * dpi / 96.0);
            if (taskbarSize <= 0) taskbarSize = 24;

            ReleaseCurrentIconHandles();
            IntPtr hSmall = IntPtr.Zero;
            IntPtr hTaskbar = IntPtr.Zero;
            Bitmap src256 = null;
            try
            {
                if (pathIsIcoFile)
                {
                    try { using (var ico = new Icon(iconPath, 256, 256)) src256 = ico.ToBitmap(); } catch { }
                    if (src256 == null) try { using (var ico = new Icon(iconPath, 128, 128)) src256 = ico.ToBitmap(); } catch { }
                    if (src256 == null) try { using (var ico = new Icon(iconPath)) src256 = ico.ToBitmap(); } catch { }
                }
                else
                {
                    IntPtr[] hIcons = new IntPtr[1];
                    int[] ids = new int[1];
                    foreach (int sz in new int[] { 256, 128, 64, 48 })
                    {
                        int n = PrivateExtractIcons(iconPath, iconIndex, sz, sz, hIcons, ids, 1, 0);
                        if (n > 0 && hIcons[0] != IntPtr.Zero)
                        {
                            try { using (Icon ic = Icon.FromHandle(hIcons[0])) src256 = ic.ToBitmap(); } catch { }
                            try { DestroyIcon(hIcons[0]); } catch { }
                            if (src256 != null) break;
                        }
                    }
                }
            }
            catch { }
            if (src256 == null) try { using (var ico = new Icon(iconPath)) src256 = ico.ToBitmap(); } catch { }

            if (src256 != null)
            {
                using (Bitmap smallBmp = HighQualityResize(src256, captionSize, captionSize))
                    hSmall = smallBmp.GetHicon();
                using (Bitmap taskBmp = HighQualityResize(src256, taskbarSize, taskbarSize))
                    hTaskbar = taskBmp.GetHicon();
                src256.Dispose();
            }
            else
            {
                if (pathIsIcoFile)
                {
                    try { using (var ico = new Icon(iconPath, captionSize, captionSize)) hSmall = CopyIconHandle(ico.Handle); } catch { }
                    try { using (var ico = new Icon(iconPath, taskbarSize, taskbarSize)) hTaskbar = CopyIconHandle(ico.Handle); } catch { }
                }
                else
                {
                    hSmall = ExtractIconHandle(iconPath, iconIndex, captionSize);
                    hTaskbar = ExtractIconHandle(iconPath, iconIndex, taskbarSize);
                }
            }

            try
            {
                var oldIcon = this.Icon;
                this.Icon = null;
                oldIcon?.Dispose();
                this.Icon = new Icon(iconPath);
            }
            catch { }

            if (hSmall != IntPtr.Zero)
            {
                SendMessage(this.Handle, WM_SETICON, (IntPtr)ICON_SMALL, hSmall);
                currentSmallIconHandle = hSmall;
            }
            if (hTaskbar != IntPtr.Zero)
            {
                SendMessage(this.Handle, WM_SETICON, (IntPtr)ICON_BIG, hTaskbar);
                currentLargeIconHandle = hTaskbar;
            }
        }

        private IntPtr ExtractIconHandle(string path, int index, int size)
        {
            IntPtr[] hIcons = new IntPtr[1];
            int[] ids = new int[1];
            int n = PrivateExtractIcons(path, index, size, size, hIcons, ids, 1, 0);
            if (n > 0 && hIcons[0] != IntPtr.Zero) return hIcons[0];
            return IntPtr.Zero;
        }

        private IntPtr CopyIconHandle(IntPtr hIcon)
        {
            try
            {
                using (Icon ico = Icon.FromHandle(hIcon))
                using (Bitmap bmp = ico.ToBitmap())
                    return bmp.GetHicon();
            }
            catch { return IntPtr.Zero; }
        }

        private void ReleaseCurrentIconHandles()
        {
            try { if (currentSmallIconHandle != IntPtr.Zero) { DestroyIcon(currentSmallIconHandle); currentSmallIconHandle = IntPtr.Zero; } } catch { }
            try { if (currentLargeIconHandle != IntPtr.Zero) { DestroyIcon(currentLargeIconHandle); currentLargeIconHandle = IntPtr.Zero; } } catch { }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DPICHANGED)
            {
                base.WndProc(ref m);
                try
                {
                    if (!string.IsNullOrEmpty(currentIconPath) && File.Exists(currentIconPath))
                    {
                        string icoPath = currentIconPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ? currentIconPath : Path.Combine(Path.GetTempPath(), "vm_gen_temp_preview.ico");
                        bool isIco = File.Exists(icoPath);
                        SetFormIconSmart(icoPath, currentIconIndex, isIco);
                    }
                }
                catch { }
                return;
            }
            base.WndProc(ref m);
        }

        private Bitmap GetBestPreviewBitmapFromIcoPath(string icoPath, int previewSize)
        {
            try
            {
                foreach (int sz in new int[] { 256, 128, 64, 48, 32 })
                {
                    try
                    {
                        using (var ico = new Icon(icoPath, sz, sz))
                        using (var bmpSrc = ico.ToBitmap())
                        {
                            if (bmpSrc.Width >= previewSize * 0.8)
                                return HighQualityResize(bmpSrc, previewSize, previewSize);
                        }
                    }
                    catch { }
                }
                using (var ico = new Icon(icoPath))
                    return new Bitmap(ico.ToBitmap(), new Size(previewSize, previewSize));
            }
            catch { return null; }
        }

        private Bitmap GetBestPreviewBitmap(string dllPath, int iconIndex, int previewSize)
        {
            foreach (int sz in new int[] { 256, 128, 64, 48 })
            {
                IntPtr[] phicon = new IntPtr[1];
                int[] piconid = new int[1];
                int extracted = PrivateExtractIcons(dllPath, iconIndex, sz, sz, phicon, piconid, 1, 0);
                if (extracted > 0 && phicon[0] != IntPtr.Zero)
                {
                    try
                    {
                        using (Icon ico = Icon.FromHandle(phicon[0]))
                        using (Bitmap src = ico.ToBitmap())
                            return HighQualityResize(src, previewSize, previewSize);
                    }
                    catch { }
                    finally { try { DestroyIcon(phicon[0]); } catch { } }
                }
            }
            try { return Icon.ExtractAssociatedIcon(dllPath)?.ToBitmap(); } catch { return null; }
        }

        private Bitmap HighQualityResize(Bitmap src, int w, int h)
        {
            var dst = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(dst))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.Clear(Color.Transparent);
                g.DrawImage(src, 0, 0, w, h);
            }
            return dst;
        }

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

            if (iconWelcome != null) iconWelcome.Image = GetResourceImage("iconWelcome");
            if (iconFinish != null) iconFinish.Image = GetResourceImage("iconFinish");
            if (iconFolder != null) iconFolder.Image = GetResourceImage("iconFolder");
            if (iconHardDrive != null) iconHardDrive.Image = GetResourceImage("iconHardDrive");
            try { pictureBoxStep1Background.SendToBack(); } catch { }
            try { pictureBoxStep4Background.SendToBack(); } catch { }
            try { panelStep1Background.BringToFront(); } catch { }
            try { panelStep4Background.BringToFront(); } catch { }
            try { iconFolder?.BringToFront(); } catch { }
            try { iconHardDrive?.BringToFront(); } catch { }

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

            LoadWizardIcons();

            string langFileNameRaw = ini.Read("LanguageFile", "General");
            string langFileName = !string.IsNullOrWhiteSpace(langFileNameRaw) ? Environment.ExpandEnvironmentVariables(langFileNameRaw) : "";
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

            string vmFolderRaw = ini.Read("VM-Folder", "General");
            string vmFolder = !string.IsNullOrWhiteSpace(vmFolderRaw) ? Environment.ExpandEnvironmentVariables(vmFolderRaw) : "";

            string iniIconRaw = ini.Read("Icon", "General");
            string iniIcon = !string.IsNullOrWhiteSpace(iniIconRaw) ? Environment.ExpandEnvironmentVariables(iniIconRaw) : "";

            if (string.IsNullOrWhiteSpace(iniIcon))
            {
                currentIconPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            }
            else
            {
                if (Path.IsPathRooted(iniIcon))
                {
                    currentIconPath = iniIcon;
                }
                else
                {
                    currentIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iniIcon);
                }
            }

            // Load multi-resolution icon - FIXED dynamic per actual size
            try
            {
                if (File.Exists(currentIconPath))
                {
                    try { var oi = this.Icon; this.Icon = null; oi?.Dispose(); } catch { }
                    ReleaseCurrentIconHandles();

                    if (currentIconPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                    {
                        SetFormIconSmart(currentIconPath, -1, true);
                        pictureVMIcon.Image = GetBestPreviewBitmapFromIcoPath(currentIconPath, 32);
                    }
                    else
                    {
                        string tempFormIco = Path.Combine(Path.GetTempPath(), "vm_gen_form_init.ico");
                        SaveIconWithAllSizes(currentIconPath, currentIconIndex, tempFormIco);
                        if (File.Exists(tempFormIco))
                        {
                            SetFormIconSmart(tempFormIco, -1, true);
                            pictureVMIcon.Image = GetBestPreviewBitmapFromIcoPath(tempFormIco, 32);
                        }
                        else
                        {
                            SetFormIconSmart(currentIconPath, currentIconIndex, false);
                            pictureVMIcon.Image = GetBestPreviewBitmap(currentIconPath, currentIconIndex, 32);
                        }
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

            SetFormTitle($"VMware {osName} VM Generator");

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
                if (sataCount == 0) sataCount = 1;
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

            ApplyCustomUIsettings();

            ValidateStep1();
        }

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
            catch { }
        }

        private void ApplyUIStyleToControls(Control container, Color? color, string fontName)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is Button || ctrl is CustomButton || ctrl is TextBox || ctrl is ComboBox || ctrl is NumericUpDown)
                {
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

        private void LoadBackground(Control control, string imageName)
        {
            if (control == null) return;

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
                        Bitmap bmp = new Bitmap(tempImg.Width, tempImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(Color.Transparent);
                            g.DrawImage(tempImg, 0, 0, tempImg.Width, tempImg.Height);
                        }

                        if (control is PictureBox pb)
                        {
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
                            if (control.BackgroundImage != null)
                            {
                                var oldBg = control.BackgroundImage;
                                control.BackgroundImage = null;
                                oldBg.Dispose();
                            }
                            control.BackgroundImage = bmp;
                            control.BackgroundImageLayout = ImageLayout.Stretch;
                            control.BackColor = Color.Transparent;
                        }
                    }
                }
                catch { }
            }
        }

        private void LoadPageLayout(string pagePrefix)
        {
            LoadBackground(this.Controls.Find($"panel{pagePrefix}Background", true).FirstOrDefault(), "WizardBackground");
            LoadBackground(this.Controls.Find($"pictureBox{pagePrefix}Background", true).FirstOrDefault(), "WizardBackground");

            LoadBackground(this.Controls.Find($"panel{pagePrefix}Bottom", true).FirstOrDefault(), "WizardBottom");
            LoadBackground(this.Controls.Find($"pictureBox{pagePrefix}Bottom", true).FirstOrDefault(), "WizardBottom");

            LoadBackground(this.Controls.Find($"panel{pagePrefix}Left", true).FirstOrDefault(), "WizardLeft");
            LoadBackground(this.Controls.Find($"pictureBox{pagePrefix}Left", true).FirstOrDefault(), "WizardLeft");

            var panelTop = this.Controls.Find($"panel{pagePrefix}Top", true).FirstOrDefault();
            var picTop = this.Controls.Find($"pictureBox{pagePrefix}Top", true).FirstOrDefault() as PictureBox;
            LoadBackground(panelTop, "WizardTop");
            LoadBackground(picTop, "WizardTop");

            var panelTopRight = this.Controls.Find($"panel{pagePrefix}TopRight", true).FirstOrDefault();
            var picTopRight = this.Controls.Find($"pictureBox{pagePrefix}TopRight", true).FirstOrDefault() as PictureBox;

            LoadBackground(panelTopRight, "WizardTopRight");
            LoadBackground(picTopRight, "WizardTopRight");

            try
            {
                if (picTopRight != null && panelTop != null)
                {
                    if (picTopRight.Parent != panelTop)
                    {
                        picTopRight.Parent = panelTop;
                    }
                    bool isRtl = this.RightToLeft == RightToLeft.Yes;
                    if (isRtl)
                        picTopRight.Location = new Point(0, 0);
                    else
                        picTopRight.Location = new Point(panelTop.Width - picTopRight.Width, 0);

                    picTopRight.BackColor = Color.Transparent;
                    picTopRight.Visible = true;
                    picTopRight.BringToFront();
                }

                if (panelTop != null) panelTop.Visible = true;
                if (picTop != null) picTop.Visible = true;
            }
            catch { }
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
                    var trPic = this.Controls.Find($"pictureBox{pagePrefix}TopRight", true).FirstOrDefault() as PictureBox;
                    if (trPic != null)
                    {
                        trPic.Visible = true;
                        trPic.BringToFront();
                    }
                }
                catch { }

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
                SetFormTitle(customTitleTemplate.Replace("{osName}", osName).Replace("{vmName}", vmNameForTitle));
            }
            else
            {
                SetFormTitle($"VMware {osName} VM Generator");
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
            this.RightToLeftLayout = false;

            foreach (Control ctrl in this.Controls)
                ApplyDirectionToControl(ctrl, rtl);

            if (rtl) MirrorControls(this);

            try
            {
                string currentPrefix = "Welcome";
                if (panelStep1 != null && panelStep1.Visible) currentPrefix = "Step1";
                else if (panelStep2 != null && panelStep2.Visible) currentPrefix = "Step2";
                else if (panelStep3 != null && panelStep3.Visible) currentPrefix = "Step3";
                else if (panelStep4 != null && panelStep4.Visible) currentPrefix = "Step4";
                else if (panelStep5 != null && panelStep5.Visible) currentPrefix = "Step5";
                else if (panelStep6 != null && panelStep6.Visible) currentPrefix = "Step6";
                else if (panelStep7 != null && panelStep7.Visible) currentPrefix = "Step7";
                else if (panelFinish != null && panelFinish.Visible) currentPrefix = "Finish";
                else if (panelWelcome != null && panelWelcome.Visible) currentPrefix = "Welcome";

                LoadPageLayout(currentPrefix);
            }
            catch { }

            try
            {
                SetFormTitle(this.Text);
                if (this.IsHandleCreated)
                {
                    this.RecreateHandle();
                }
            }
            catch { }
        }

        private void MirrorControls(Control container)
        {
            int width = container.ClientSize.Width;
            foreach (Control ctrl in container.Controls.Cast<Control>().ToArray())
            {
                if (ctrl == trackBarMemory) continue;
                if (ctrl is PictureBox pb && pb.Name.EndsWith("TopRight")) continue;
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

            string langFileNameRaw = ini.Read("LanguageFile", "General");
            string langFileName = !string.IsNullOrWhiteSpace(langFileNameRaw) ? Environment.ExpandEnvironmentVariables(langFileNameRaw) : "";
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

            string readyTitle = GetTr("labelSummaryTitle", "Ready to create virtual machine");
            if (!string.IsNullOrWhiteSpace(readyTitle))
                labelSummaryTitle.Text = readyTitle.Replace("{vmName}", vmName).Replace("{osName}", osName);
            else
                labelSummaryTitle.Text = titleWithVm;

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
