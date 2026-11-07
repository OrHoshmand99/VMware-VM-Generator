using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace VMware_VM_Generator_Studio
{
    public static class BuildManager
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr lpName, ushort wLanguage, byte[] lpData, uint cbData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool EndUpdateResource(IntPtr hUpdate, bool bDiscard);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
        private const uint SHCNE_ASSOCCHANGED = 0x08000000;
        private const uint SHCNF_FLUSH = 0x1000;

        private static readonly IntPtr RT_ICON = (IntPtr)3;
        private static readonly IntPtr RT_GROUP_ICON = (IntPtr)14;

        public static bool BuildVirtualMachine(ProjectData data, string projectFilePath, byte[]? customIconBytes = null)
        {
            // 1. Prompt user to select destination parent folder using OpenFileDialog
            string selectedDestinationPath = string.Empty;
            using (var folderDialog = new OpenFileDialog())
            {
                folderDialog.Title = "Select destination folder for the virtual machine generator wizard package";
                folderDialog.CheckFileExists = false;
                folderDialog.CheckPathExists = true;
                folderDialog.FileName = "Select Folder";
                folderDialog.Filter = "Folders|*.none";

                if (!string.IsNullOrWhiteSpace(data.VmLocation))
                {
                    try
                    {
                        string expandedInitialPath = Environment.ExpandEnvironmentVariables(data.VmLocation);
                        if (Directory.Exists(expandedInitialPath))
                        {
                            folderDialog.InitialDirectory = expandedInitialPath;
                        }
                    }
                    catch { }
                }

                if (folderDialog.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }

                selectedDestinationPath = Path.GetDirectoryName(folderDialog.FileName);
                if (string.IsNullOrEmpty(selectedDestinationPath))
                {
                    return false;
                }
            }

            string defaultVmName = string.IsNullOrWhiteSpace(data.VmName) ? "New Virtual Machine" : data.VmName.Trim();

            // 2. Prompt user to choose/edit the folder name
            string folderNameInput = Interaction.InputBox(
                "Enter the name for the virtual machine folder:",
                "Folder Name",
                defaultVmName);

            if (string.IsNullOrWhiteSpace(folderNameInput))
            {
                return false;
            }

            string vmName = folderNameInput.Trim();

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                vmName = vmName.Replace(c, '_');
            }

            string wizardPackagePath = Path.Combine(selectedDestinationPath, vmName);

            string actualVmFolder = selectedDestinationPath;
            if (!string.IsNullOrWhiteSpace(data.VmLocation))
            {
                try
                {
                    string expandedLoc = Environment.ExpandEnvironmentVariables(data.VmLocation);
                    if (!string.IsNullOrEmpty(expandedLoc))
                    {
                        actualVmFolder = expandedLoc;
                    }
                }
                catch { }
            }

            string targetVmSpecificFolder = Path.Combine(actualVmFolder, vmName);

            // 3. הצגת חלון התקדמות (Progress Dialog) בזמן ביצוע תהליך ה-Build
            bool buildSuccess = false;
            string errorMessage = string.Empty;

            using (var progressForm = new ProgressForm())
            {
                var buildTask = Task.Run(() =>
                {
                    try
                    {
                        progressForm.UpdateStatus("Creating directories...", 10);
                        Directory.CreateDirectory(wizardPackagePath);
                        Directory.CreateDirectory(targetVmSpecificFolder);

                        progressForm.UpdateStatus("Generating VMX configuration file...", 20);
                        string vmxFilePath = Path.Combine(targetVmSpecificFolder, $"{vmName}.vmx");
                        string vmxContent = GenerateVmxContent(data);
                        File.WriteAllText(vmxFilePath, vmxContent, Encoding.UTF8);

                        progressForm.UpdateStatus("Copying core application files...", 35);
                        string baseAppDir = AppDomain.CurrentDomain.BaseDirectory;
                        string sourceResourcesDir = Path.Combine(baseAppDir, "resources");

                        string[] resourceFilesToCopy = new string[]
                        {
                            "VMware VM Generator.exe",
                            "VMware VM Generator.dll",
                            "VMware VM Generator.runtimeconfig.json",
                            "WizardBackground.png",
                            "WizardTop.png",
                            "WizardBottom.png",
                            "WizardLeft.png",
                            "WizardTopRight.png",
                            "iconWelcome.png",
                            "iconFolder.png",
                            "iconSound.png",
                            "iconNetwork.png",
                            "iconFloppy.png",
                            "iconCD.png",
                            "iconFirmware.png",
                            "iconProcessor.png",
                            "iconMemory.png",
                            "iconSCSI.png"
                        };

                        foreach (var fileName in resourceFilesToCopy)
                        {
                            string sourceFilePath = Path.Combine(sourceResourcesDir, fileName);
                            if (File.Exists(sourceFilePath))
                            {
                                string destFilePath = Path.Combine(wizardPackagePath, fileName);
                                File.Copy(sourceFilePath, destFilePath, true);
                            }
                        }

                        // תיקון: העתקת קובץ השפה בשמו המקורי (למשל Hebrew.ini) תואם למה שכתוב ב-settings.ini
                        string languagesDir = Path.Combine(sourceResourcesDir, "languages");
                        string selectedLangName = !string.IsNullOrWhiteSpace(data.SelectedLanguage) ? data.SelectedLanguage.Trim() : "English";
                        string specificLangFile = Path.Combine(languagesDir, $"{selectedLangName}.ini");
                        if (File.Exists(specificLangFile))
                        {
                            File.Copy(specificLangFile, Path.Combine(wizardPackagePath, $"{selectedLangName}.ini"), true);
                        }

                        progressForm.UpdateStatus("Generating settings.ini...", 50);
                        GenerateSettingsIni(data, wizardPackagePath, actualVmFolder);

                        progressForm.UpdateStatus("Extracting graphic assets and icons...", 65);
                        if (data.AssetImages != null && data.AssetImages.Count > 0)
                        {
                            var assetFileNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                            {
                                { "Window Background", "WizardBackground.png" },
                                { "Header Background", "WizardTop.png" },
                                { "Footer Background", "WizardBottom.png" },
                                { "Banner", "WizardLeft.png" },
                                { "Header Icon", "WizardTopRight.png" },
                                { "Welcome Icon", "iconWelcome.png" },
                                { "Folder Icon", "iconFolder.png" },
                                { "Firmware Icon", "iconFirmware.png" },
                                { "Processor Icon", "iconProcessor.png" },
                                { "Memory Icon", "iconMemory.png" },
                                { "SCSI Icon", "iconSCSI.png" },
                                { "Sound Icon", "iconSound.png" },
                                { "Network Icon", "iconNetwork.png" },
                                { "Hard Drive Icon", "iconHardDrive.png" },
                                { "Floppy Icon", "iconFloppy.png" },
                                { "CD/DVD Icon", "iconCD.png" },
                                { "Finish Icon", "iconFinish.png" }
                            };

                            foreach (var asset in data.AssetImages)
                            {
                                if (string.Equals(asset.Key, "VM_Default_Icon", StringComparison.OrdinalIgnoreCase))
                                {
                                    try
                                    {
                                        byte[] rawBytes = Convert.FromBase64String(asset.Value);
                                        byte[] validIcoBytes = EnsureValidIcoBytes(rawBytes);
                                        File.WriteAllBytes(Path.Combine(wizardPackagePath, $"{vmName}.ico"), validIcoBytes);
                                        customIconBytes = validIcoBytes;
                                    }
                                    catch { }
                                }
                                else if (assetFileNameMap.TryGetValue(asset.Key, out string targetFileName))
                                {
                                    try
                                    {
                                        byte[] imgBytes = Convert.FromBase64String(asset.Value);
                                        File.WriteAllBytes(Path.Combine(wizardPackagePath, targetFileName), imgBytes);
                                    }
                                    catch { }
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(data.IsoImagePath) && File.Exists(data.IsoImagePath))
                        {
                            progressForm.UpdateStatus("Copying ISO image file (this may take a while)...", 75);
                            string targetIsoDestPath = Path.Combine(wizardPackagePath, "OS.iso");
                            File.Copy(data.IsoImagePath, targetIsoDestPath, true);
                        }

                        progressForm.UpdateStatus("Injecting icon into executables...", 90);
                        if (customIconBytes != null && customIconBytes.Length > 0)
                        {
                            string targetExePath = Path.Combine(wizardPackagePath, "VMware VM Generator.exe");
                            string targetDllPath = Path.Combine(wizardPackagePath, "VMware VM Generator.dll");

                            if (File.Exists(targetExePath)) InjectIconIntoExe(targetExePath, customIconBytes);
                            if (File.Exists(targetDllPath)) InjectIconIntoExe(targetDllPath, customIconBytes);

                            try
                            {
                                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
                            }
                            catch { }
                        }

                        progressForm.UpdateStatus("Finalizing package...", 100);
                        buildSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        buildSuccess = false;
                    }
                });

                progressForm.ShowDialog();
            }

            if (buildSuccess)
            {
                ShowBuildSuccessDialog(wizardPackagePath);
                return true;
            }
            else if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show($"An error occurred during the build process:\n{errorMessage}", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return false;
        }

        private class ProgressForm : Form
        {
            private Label lblStatus;
            private ProgressBar progressBar;

            public ProgressForm()
            {
                Text = "Building Virtual Machine Package...";
                Size = new Size(450, 160);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                StartPosition = FormStartPosition.CenterScreen;
                ControlBox = false;

                lblStatus = new Label
                {
                    Text = "Initializing build process...",
                    AutoSize = false,
                    Location = new Point(20, 20),
                    Size = new Size(390, 25),
                    Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 9.5f, FontStyle.Regular)
                };

                progressBar = new ProgressBar
                {
                    Location = new Point(20, 60),
                    Size = new Size(390, 25),
                    Style = ProgressBarStyle.Continuous,
                    Minimum = 0,
                    Maximum = 100,
                    Value = 10
                };

                Controls.Add(lblStatus);
                Controls.Add(progressBar);
            }

            public void UpdateStatus(string message, int progressPercentage)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(() => UpdateStatus(message, progressPercentage)));
                    return;
                }

                lblStatus.Text = message;
                if (progressPercentage >= 0 && progressPercentage <= 100)
                {
                    progressBar.Value = progressPercentage;
                }

                if (progressPercentage >= 100)
                {
                    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                    timer.Interval = 300;
                    timer.Tick += (s, e) => {
                        timer.Stop();
                        Close();
                    };
                    timer.Start();
                }
            }
        }

        private static byte[] EnsureValidIcoBytes(byte[] rawBytes)
        {
            try
            {
                if (rawBytes.Length > 4 && rawBytes[0] == 0 && rawBytes[1] == 0 && rawBytes[2] == 1 && rawBytes[3] == 0)
                {
                    return rawBytes;
                }

                using var msInput = new MemoryStream(rawBytes);
                using var originalImage = new Bitmap(msInput);

                int[] sizes = { 256, 48, 32, 16 };
                var encodedImages = new List<byte[]>();

                foreach (int size in sizes)
                {
                    using var bmp = new Bitmap(originalImage, new Size(size, size));
                    using var msBmp = new MemoryStream();
                    bmp.Save(msBmp, System.Drawing.Imaging.ImageFormat.Png);
                    encodedImages.Add(msBmp.ToArray());
                }

                using var msIco = new MemoryStream();
                using var bw = new BinaryWriter(msIco);

                bw.Write((ushort)0);
                bw.Write((ushort)1);
                bw.Write((ushort)encodedImages.Count);

                int offset = 6 + (16 * encodedImages.Count);

                for (int i = 0; i < encodedImages.Count; i++)
                {
                    byte[] imgData = encodedImages[i];
                    int size = sizes[i];
                    bw.Write((byte)(size >= 256 ? 0 : size));
                    bw.Write((byte)(size >= 256 ? 0 : size));
                    bw.Write((byte)0);
                    bw.Write((byte)0);
                    bw.Write((ushort)1);
                    bw.Write((ushort)32);
                    bw.Write((uint)imgData.Length);
                    bw.Write((uint)offset);
                    offset += imgData.Length;
                }

                foreach (var imgData in encodedImages)
                {
                    bw.Write(imgData);
                }

                return msIco.ToArray();
            }
            catch
            {
                return rawBytes;
            }
        }

        private static void ShowBuildSuccessDialog(string wizardPackagePath)
        {
            using (var successForm = new Form())
            {
                successForm.Text = "Build Complete";
                successForm.Size = new Size(500, 220);
                successForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                successForm.MaximizeBox = false;
                successForm.MinimizeBox = false;
                successForm.StartPosition = FormStartPosition.CenterScreen;

                var lblMessage = new Label
                {
                    Text = $"Virtual machine generator wizard package successfully built at:\n{wizardPackagePath}",
                    AutoSize = false,
                    Location = new Point(20, 20),
                    Size = new Size(440, 45)
                };

                var btnOpenFolder = new Button
                {
                    Text = "Open Folder",
                    Location = new Point(20, 95),
                    Size = new Size(135, 38)
                };
                btnOpenFolder.Click += (s, e) => {
                    try { Process.Start("explorer.exe", wizardPackagePath); } catch { }
                    successForm.Close();
                };

                var btnLaunchWizard = new Button
                {
                    Text = "Launch Wizard",
                    Location = new Point(165, 95),
                    Size = new Size(135, 38)
                };
                btnLaunchWizard.Click += (s, e) => {
                    try
                    {
                        string exePath = Path.Combine(wizardPackagePath, "VMware VM Generator.exe");
                        if (File.Exists(exePath))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = exePath,
                                WorkingDirectory = wizardPackagePath
                            });
                        }
                    }
                    catch { }
                    successForm.Close();
                };

                var btnClose = new Button
                {
                    Text = "Close",
                    Location = new Point(310, 95),
                    Size = new Size(150, 38),
                    DialogResult = DialogResult.Cancel
                };
                btnClose.Click += (s, e) => successForm.Close();

                successForm.Controls.Add(lblMessage);
                successForm.Controls.Add(btnOpenFolder);
                successForm.Controls.Add(btnLaunchWizard);
                successForm.Controls.Add(btnClose);
                successForm.CancelButton = btnClose;

                successForm.ShowDialog();
            }
        }

        private static void GenerateSettingsIni(ProjectData data, string targetFolder, string actualVmLocation)
        {
            var sb = new StringBuilder();

            sb.AppendLine("[UI]");
            sb.AppendLine($"TextColor={data.TextColorHex}");
            sb.AppendLine($"FontName={data.TextFontName}");
            sb.AppendLine();

            string langFileName = !string.IsNullOrWhiteSpace(data.SelectedLanguage) ? $"{data.SelectedLanguage}.ini" : "English.ini";
            sb.AppendLine("[General]");
            sb.AppendLine($"LanguageFile={langFileName}");
            sb.AppendLine($"OS-Type={data.GuestOSID}");
            sb.AppendLine($"OS-Name={data.OSDisplayName}");
            sb.AppendLine($"VM-Name={data.VmName}");
            sb.AppendLine($"VM-Folder={actualVmLocation}");
            sb.AppendLine();

            sb.AppendLine("[Hardware Version]");
            sb.AppendLine($"HardwareVersion={data.HardwareVersion}");
            sb.AppendLine();

            string firmwareStr = "BIOS";
            bool isUefi = string.Equals(data.FirmwareType, "UEFI", StringComparison.OrdinalIgnoreCase);
            if (isUefi && data.FirmwareForced) firmwareStr = "UEFI-f";
            else if (isUefi) firmwareStr = "UEFI";
            else if (data.FirmwareForced) firmwareStr = "BIOS-f";

            sb.AppendLine("[Firmware]");
            sb.AppendLine($"Firmware={firmwareStr}");
            sb.AppendLine();

            sb.AppendLine("[CPU]");
            sb.AppendLine($"Processors={data.NumberOfProcessors}");
            sb.AppendLine($"CoresPerProcessor={data.CoresPerProcessor}");
            sb.AppendLine();

            sb.AppendLine("[Memory]");
            sb.AppendLine($"RecommendedMin={data.RecommendedMinMemory}");
            sb.AppendLine($"RecommendedDefault={data.RecommendedDefaultMemory}");
            sb.AppendLine($"RecommendedMax={data.RecommendedMaxMemory}");
            sb.AppendLine();

            string defaultScsi = "BusLogic";
            if (data.SCSIBusLogicSelected) defaultScsi = "BusLogic";
            else if (data.SCSILSILogicSelected) defaultScsi = "LSILogic";
            else if (data.SCSILSILogicSASSelected) defaultScsi = "LSILogicSAS";
            else if (data.SCSIParavirtualizedSelected) defaultScsi = "Paravirtualized";

            sb.AppendLine("[SCSI I/O Controller]");
            sb.AppendLine($"SCSI I/O Controller={defaultScsi}");
            sb.AppendLine($"BusLogic={data.SCSIBusLogicChecked.ToString().ToLower()}");
            sb.AppendLine($"LSILogic={data.SCSILSILogicChecked.ToString().ToLower()}");
            sb.AppendLine($"LSILogicSAS={data.SCSILSILogicSASChecked.ToString().ToLower()}");
            sb.AppendLine($"Paravirtualized={data.SCSIParavirtualizedChecked.ToString().ToLower()}");
            sb.AppendLine();

            string defaultSound = "SB16";
            if (data.SoundCardSB16Selected) defaultSound = "SB16";
            else if (data.SoundCardES1371Selected) defaultSound = "ES1371";
            else if (data.SoundCardHDAudioSelected) defaultSound = "HDAudio";

            sb.AppendLine("[Sound Card]");
            sb.AppendLine($"DefaultSoundCard={defaultSound}");
            sb.AppendLine($"SB16={data.SoundCardSB16Checked.ToString().ToLower()}");
            sb.AppendLine($"ES1371={data.SoundCardES1371Checked.ToString().ToLower()}");
            sb.AppendLine($"HDAudio={data.SoundCardHDAudioChecked.ToString().ToLower()}");
            sb.AppendLine();

            string defaultNet = "e1000";
            if (data.NetworkE1000PCISelected) defaultNet = "e1000";
            else if (data.NetworkE1000PCIeSelected) defaultNet = "e1000e";
            else if (data.NetworkPCNet32Selected) defaultNet = "pcnet32";
            else if (data.NetworkVMXNET3Selected) defaultNet = "vmxnet3";

            sb.AppendLine("[Network]");
            sb.AppendLine($"DefaultNetwork={defaultNet}");
            sb.AppendLine($"PCNet32={data.NetworkPCNet32Checked.ToString().ToLower()}");
            sb.AppendLine($"VMXNET3={data.NetworkVMXNET3Checked.ToString().ToLower()}");
            sb.AppendLine($"E1000PCI={data.NetworkE1000PCIChecked.ToString().ToLower()}");
            sb.AppendLine($"E1000PCIe={data.NetworkE1000PCIeChecked.ToString().ToLower()}");
            sb.AppendLine();

            string defaultDrive = "IDE";
            if (data.HardDriveIDESelected) defaultDrive = "IDE";
            else if (data.HardDriveSCSISelected) defaultDrive = "SCSI";
            else if (data.HardDriveSATASelected) defaultDrive = "SATA";
            else if (data.HardDriveNVMeSelected) defaultDrive = "NVMe";

            sb.AppendLine("[Virtual Drive]");
            sb.AppendLine($"DefaultDriveType={defaultDrive}");
            sb.AppendLine($"DefaultCapacity={data.HardDriveCapacityGB}");
            sb.AppendLine($"IDE={data.HardDriveIDEChecked.ToString().ToLower()}");
            sb.AppendLine($"SCSI={data.HardDriveSCSIChecked.ToString().ToLower()}");
            sb.AppendLine($"SATA={data.HardDriveSATAChecked.ToString().ToLower()}");
            sb.AppendLine($"NVMe={data.HardDriveNVMeChecked.ToString().ToLower()}");
            sb.AppendLine();

            sb.AppendLine("[Drives]");
            sb.AppendLine($"FloppyCount={data.FloppyDriveCount}");
            sb.AppendLine($"IDECDCount={data.IDECDCount}");
            sb.AppendLine($"SCSICDCount={data.SCSICDCount}");
            sb.AppendLine($"SATACDCount={data.SATACDCount}");
            sb.AppendLine();

            sb.AppendLine("[BootCD]");
            sb.AppendLine("OS-ISO=OS.iso");
            sb.AppendLine($"OS-Controller={data.IsoControllerType}");

            File.WriteAllText(Path.Combine(targetFolder, "settings.ini"), sb.ToString(), Encoding.UTF8);
        }

        private static void InjectIconIntoExe(string filePath, byte[] icoBytes)
        {
            try
            {
                IntPtr hUpdate = BeginUpdateResource(filePath, false);
                if (hUpdate == IntPtr.Zero) return;

                if (icoBytes.Length > 6)
                {
                    int imageCount = BitConverter.ToUInt16(icoBytes, 4);
                    int headerSize = 6;
                    int directoryEntrySize = 16;

                    using var msGroup = new MemoryStream();
                    using var bwGroup = new BinaryWriter(msGroup);

                    bwGroup.Write(BitConverter.ToUInt16(icoBytes, 0));
                    bwGroup.Write(BitConverter.ToUInt16(icoBytes, 2));
                    bwGroup.Write((ushort)imageCount);

                    List<byte[]> imagePayloads = new List<byte[]>();

                    for (int i = 0; i < imageCount; i++)
                    {
                        int entryOffset = headerSize + (i * directoryEntrySize);
                        byte width = icoBytes[entryOffset];
                        byte height = icoBytes[entryOffset + 1];
                        byte colorCount = icoBytes[entryOffset + 2];
                        byte reserved = icoBytes[entryOffset + 3];
                        ushort planes = BitConverter.ToUInt16(icoBytes, entryOffset + 4);
                        ushort bitCount = BitConverter.ToUInt16(icoBytes, entryOffset + 6);
                        uint bytesInRes = BitConverter.ToUInt32(icoBytes, entryOffset + 8);
                        uint imageOffset = BitConverter.ToUInt32(icoBytes, entryOffset + 12);

                        bwGroup.Write(width);
                        bwGroup.Write(height);
                        bwGroup.Write(colorCount);
                        bwGroup.Write(reserved);
                        bwGroup.Write(planes);
                        bwGroup.Write(bitCount);
                        bwGroup.Write(bytesInRes);
                        bwGroup.Write((ushort)(i + 1));

                        byte[] imageData = new byte[bytesInRes];
                        Buffer.BlockCopy(icoBytes, (int)imageOffset, imageData, 0, (int)bytesInRes);
                        imagePayloads.Add(imageData);
                    }

                    for (int i = 0; i < imagePayloads.Count; i++)
                    {
                        IntPtr resId = (IntPtr)(i + 1);
                        UpdateResource(hUpdate, RT_ICON, resId, 0, imagePayloads[i], (uint)imagePayloads[i].Length);
                    }

                    byte[] groupData = msGroup.ToArray();
                    IntPtr groupId = (IntPtr)1;
                    UpdateResource(hUpdate, RT_GROUP_ICON, groupId, 0, groupData, (uint)groupData.Length);
                }

                EndUpdateResource(hUpdate, false);
            }
            catch { }
        }

        private static string GenerateVmxContent(ProjectData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@".encoding = ""UTF-8""");
            sb.AppendLine(@"config.version = ""8""");
            sb.AppendLine($@"virtualHW.version = ""{data.HardwareVersion}""");
            sb.AppendLine($@"displayName = ""{data.VmName}""");
            sb.AppendLine($@"guestOS = ""{data.GuestOSID}""");

            int totalCores = Math.Max(1, data.NumberOfProcessors) * Math.Max(1, data.CoresPerProcessor);
            sb.AppendLine($@"numvcpus = ""{totalCores}""");
            if (data.CoresPerProcessor > 1)
            {
                sb.AppendLine($@"cpuid.coresPerSocket = ""{data.CoresPerProcessor}""");
            }

            string firmware = string.Equals(data.FirmwareType, "UEFI", StringComparison.OrdinalIgnoreCase) ? "efi" : "bios";
            sb.AppendLine($@"firmware = ""{firmware}""");

            if (!string.IsNullOrWhiteSpace(data.IsoImagePath))
            {
                string ctrl = data.IsoControllerType?.ToLower() ?? "ide";
                if (string.Equals(ctrl, "sata", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine(@"sata0.present = ""TRUE""");
                    sb.AppendLine(@"sata0:0.present = ""TRUE""");
                    sb.AppendLine(@"sata0:0.deviceType = ""cdrom-image""");
                    sb.AppendLine(@"sata0:0.fileName = ""OS.iso""");
                }
                else if (string.Equals(ctrl, "scsi", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine(@"scsi0.present = ""TRUE""");
                    sb.AppendLine($"scsi0:0.present = \"TRUE\"");
                    sb.AppendLine(@"scsi0:0.deviceType = ""cdrom-image""");
                    sb.AppendLine(@"scsi0:0.fileName = ""OS.iso""");
                }
                else
                {
                    sb.AppendLine(@"ide1:0.present = ""TRUE""");
                    sb.AppendLine(@"ide1:0.deviceType = ""cdrom-image""");
                    sb.AppendLine(@"ide1:0.fileName = ""OS.iso""");
                }
            }

            return sb.ToString();
        }
    }
}