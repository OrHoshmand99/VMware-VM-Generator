using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace VMware_VM_Generator
{
    public static class VirtualMachineGenerator
    {
        public class VmConfig
        {
            public string VmName { get; set; }
            public int MemoryMb { get; set; }
            public int NumVcpus { get; set; }
            public int CoresPerSocket { get; set; }
            public string Firmware { get; set; }
            public string NetworkType { get; set; }
            public string SoundVirtualDev { get; set; }
            public string StorageControllerType { get; set; }
            public string ScsiVirtualDev { get; set; }
            public string DiskPath { get; set; }
            public string IsoPath { get; set; }
            public string IsoController { get; set; }
            public int FloppyCount { get; set; }
            public int IdeCdCount { get; set; }
            public int ScsiCdCount { get; set; }
            public int SataCdCount { get; set; }
            public string IconPath { get; set; }

            public string HardwareVersion { get; set; } = "19";
            public string GuestOs { get; set; } = "other-64";
            public string EthernetVirtualDev { get; set; }

            public int DiskCapacityGB { get; set; } = 40;
            public bool AllocateAllSpace { get; set; } = false;
            public bool SplitIntoFiles { get; set; } = false;

            /// <summary>
            /// Returns disk type mapping integer for vmware-vdiskmanager.
            /// </summary>
            public int GetDiskType()
            {
                if (!AllocateAllSpace && !SplitIntoFiles) return 0;
                if (!AllocateAllSpace && SplitIntoFiles) return 1;
                if (AllocateAllSpace && !SplitIntoFiles) return 2;
                if (AllocateAllSpace && SplitIntoFiles) return 3;
                return 0;
            }
        }

        /// <summary>
        /// Creates the VMX configuration file for the virtual machine following the mandatory template.
        /// </summary>
        public static void CreateVmxFile(string vmxFilePath, VmConfig config)
        {
            StringBuilder sb = new StringBuilder();

            // 1. Mandatory base configuration lines
            sb.AppendLine(".encoding = \"UTF-8\"");
            sb.AppendLine("config.version = \"8\"");
            sb.AppendLine("virtualHW.productCompatibility = \"hosted\"");
            sb.AppendLine("pciBridge0.present = \"TRUE\"");
            sb.AppendLine("pciBridge4.present = \"TRUE\"");
            sb.AppendLine("pciBridge4.virtualDev = \"pcieRootPort\"");
            sb.AppendLine("pciBridge4.functions = \"8\"");
            sb.AppendLine("pciBridge5.present = \"TRUE\"");
            sb.AppendLine("pciBridge5.virtualDev = \"pcieRootPort\"");
            sb.AppendLine("pciBridge5.functions = \"8\"");
            sb.AppendLine("pciBridge6.present = \"TRUE\"");
            sb.AppendLine("pciBridge6.virtualDev = \"pcieRootPort\"");
            sb.AppendLine("pciBridge6.functions = \"8\"");
            sb.AppendLine("pciBridge7.present = \"TRUE\"");
            sb.AppendLine("pciBridge7.virtualDev = \"pcieRootPort\"");
            sb.AppendLine("pciBridge7.functions = \"8\"");
            sb.AppendLine("vmci0.present = \"TRUE\"");
            sb.AppendLine("hpet0.present = \"TRUE\"");
            sb.AppendLine("powerType.powerOff = \"soft\"");
            sb.AppendLine("powerType.powerOn = \"soft\"");
            sb.AppendLine("powerType.suspend = \"soft\"");
            sb.AppendLine("powerType.reset = \"soft\"");
            sb.AppendLine("sound.present = \"TRUE\"");
            sb.AppendLine("sound.fileName = \"-1\"");
            sb.AppendLine("sound.autodetect = \"TRUE\"");
            sb.AppendLine("scsi0.present = \"TRUE\"");
            sb.AppendLine("ethernet0.present = \"TRUE\"");
            sb.AppendLine("ethernet0.startConnected = \"TRUE\"");

            // 2. Configurable hardware version and guest OS
            string hwVersion = string.IsNullOrWhiteSpace(config.HardwareVersion) ? "19" : config.HardwareVersion.Trim();
            string guestOs = string.IsNullOrWhiteSpace(config.GuestOs) ? "other-64" : config.GuestOs.Trim();

            sb.AppendLine($"virtualHW.version = \"{hwVersion}\"");
            sb.AppendLine($"guestOS = \"{guestOs}\"");

            // 3. Rigid preset optional lines
            sb.AppendLine("mks.enable3d = \"TRUE\"");
            sb.AppendLine("svga.graphicsMemoryKB = \"8388608\"");
            sb.AppendLine("chipset.useAcpiBattery = \"TRUE\"");
            sb.AppendLine("chipset.useApmBattery = \"TRUE\"");
            sb.AppendLine("tools.syncTime = \"TRUE\"");
            sb.AppendLine("isolation.tools.hgfs.disable = \"FALSE\"");
            sb.AppendLine("hgfs.mapRootShare = \"TRUE\"");
            sb.AppendLine("usb.present = \"TRUE\"");
            sb.AppendLine("usb.generic.allowHID = \"TRUE\"");
            sb.AppendLine("ehci.present = \"TRUE\"");
            sb.AppendLine("usb_xhci.present = \"TRUE\"");
            sb.AppendLine("usb.vbluetooth.startConnected = \"TRUE\"");
            sb.AppendLine("serial0.fileType = \"thinprint\"");
            sb.AppendLine("serial0.fileName = \"thinprint\"");
            sb.AppendLine("serial0.present = \"TRUE\"");

            // 4. Conditional network adapter type injection
            if (!string.IsNullOrWhiteSpace(config.EthernetVirtualDev))
            {
                sb.AppendLine($"ethernet0.virtualDev = \"{config.EthernetVirtualDev.Trim()}\"");
            }

            // 5. User-defined settings
            int sockets = Math.Max(config.NumVcpus, 1);
            int coresPerSocket = Math.Max(config.CoresPerSocket, 1);
            int totalVcpus = sockets * coresPerSocket;

            sb.AppendLine($"displayName = \"{config.VmName}\"");
            sb.AppendLine($"nvram = \"{config.VmName}.nvram\"");
            sb.AppendLine($"firmware = \"{config.Firmware}\"");
            sb.AppendLine($"numvcpus = \"{totalVcpus}\"");
            sb.AppendLine($"cpuid.coresPerSocket = \"{coresPerSocket}\"");
            sb.AppendLine($"memsize = \"{config.MemoryMb}\"");
            sb.AppendLine($"scsi0.virtualDev = \"{(string.IsNullOrEmpty(config.ScsiVirtualDev) ? "lsilogic" : config.ScsiVirtualDev)}\"");
            sb.AppendLine($"sound.virtualDev = \"{config.SoundVirtualDev}\"");
            sb.AppendLine("ethernet0.connectionType = \"nat\"");

            // 6. Primary Storage Controller and Disk configuration
            string primaryController = config.StorageControllerType.ToLower();

            if (primaryController == "ide")
            {
                sb.AppendLine("ide0:0.present = \"TRUE\"");
                sb.AppendLine("ide0:0.deviceType = \"disk\"");
                sb.AppendLine($"ide0:0.fileName = \"{Path.GetFileName(config.DiskPath)}\"");
            }
            else if (primaryController == "scsi")
            {
                sb.AppendLine("scsi0.present = \"TRUE\"");
                sb.AppendLine("scsi0:0.present = \"TRUE\"");
                sb.AppendLine("scsi0:0.deviceType = \"scsi-hardDisk\"");
                sb.AppendLine($"scsi0:0.fileName = \"{Path.GetFileName(config.DiskPath)}\"");
            }
            else if (primaryController == "sata")
            {
                sb.AppendLine("sata0.present = \"TRUE\"");
                sb.AppendLine("sata0:0.present = \"TRUE\"");
                sb.AppendLine("sata0:0.deviceType = \"sata-hardDisk\"");
                sb.AppendLine($"sata0:0.fileName = \"{Path.GetFileName(config.DiskPath)}\"");
            }
            else if (primaryController == "nvme")
            {
                sb.AppendLine("nvme0.present = \"TRUE\"");
                sb.AppendLine("nvme0:0.present = \"TRUE\"");
                sb.AppendLine("nvme0:0.deviceType = \"disk\"");
                sb.AppendLine($"nvme0:0.fileName = \"{Path.GetFileName(config.DiskPath)}\"");
            }

            // 7. Floppy drives configuration
            if (config.FloppyCount > 0)
            {
                sb.AppendLine("floppy0.present = \"TRUE\"");
                sb.AppendLine("floppy0.startConnected = \"FALSE\"");
            }
            if (config.FloppyCount > 1)
            {
                sb.AppendLine("floppy1.present = \"TRUE\"");
                sb.AppendLine("floppy1.startConnected = \"FALSE\"");
            }

            string isoCtrl = (config.IsoController ?? "").Trim().ToLower();
            bool hasIso = !string.IsNullOrEmpty(config.IsoPath);

            if (hasIso && string.IsNullOrEmpty(isoCtrl))
            {
                if (config.SataCdCount > 0 || primaryController == "sata" || primaryController == "nvme")
                    isoCtrl = "sata";
                else if (config.IdeCdCount > 0 || primaryController == "ide")
                    isoCtrl = "ide";
                else if (config.ScsiCdCount > 0 || primaryController == "scsi")
                    isoCtrl = "scsi";
                else
                    isoCtrl = "sata";
            }

            // 8. IDE CD/DVD drives configuration
            int ideCdRequested = Math.Max(config.IdeCdCount, 0);
            int totalIdeDrives = (hasIso && isoCtrl == "ide") ? Math.Max(ideCdRequested, 1) : ideCdRequested;
            int ideCdLimit = Math.Min(totalIdeDrives, 3);
            int ideAssignedCount = 0;

            for (int i = 0; i < ideCdLimit; i++)
            {
                string ideKey = i == 0 ? "ide0:1" : (i == 1 ? "ide1:0" : "ide1:1");
                bool isIsoSlot = (ideAssignedCount == 0 && hasIso && isoCtrl == "ide");

                sb.AppendLine($"{ideKey}.present = \"TRUE\"");
                sb.AppendLine($"{ideKey}.deviceType = \"cdrom-image\"");
                if (isIsoSlot)
                {
                    sb.AppendLine($"{ideKey}.fileName = \"{config.IsoPath}\"");
                    sb.AppendLine($"{ideKey}.startConnected = \"TRUE\"");
                }
                else
                {
                    sb.AppendLine($"{ideKey}.startConnected = \"FALSE\"");
                }
                ideAssignedCount++;
            }

            // 9. SCSI CD/DVD drives configuration
            int scsiCdRequested = Math.Max(config.ScsiCdCount, 0);
            int totalScsiDrives = (hasIso && isoCtrl == "scsi") ? Math.Max(scsiCdRequested, 1) : scsiCdRequested;
            int scsiCdLimit = Math.Min(totalScsiDrives, 10);
            int scsiAssignedCount = 0;

            for (int i = 1; scsiAssignedCount < scsiCdLimit && i <= 11; i++)
            {
                if (i == 7) continue;

                string scsiKey = $"scsi0:{i}";
                bool isIsoSlot = (scsiAssignedCount == 0 && hasIso && isoCtrl == "scsi");

                sb.AppendLine($"{scsiKey}.present = \"TRUE\"");
                sb.AppendLine($"{scsiKey}.deviceType = \"cdrom-image\"");
                if (isIsoSlot)
                {
                    sb.AppendLine($"{scsiKey}.fileName = \"{config.IsoPath}\"");
                    sb.AppendLine($"{scsiKey}.startConnected = \"TRUE\"");
                }
                else
                {
                    sb.AppendLine($"{scsiKey}.startConnected = \"FALSE\"");
                }
                scsiAssignedCount++;
            }

            // 10. SATA CD/DVD drives configuration
            int sataCdRequested = Math.Max(config.SataCdCount, 0);
            int totalSataDrives = (hasIso && isoCtrl == "sata") ? Math.Max(sataCdRequested, 1) : sataCdRequested;
            int sataCdLimit = Math.Min(totalSataDrives, 10);
            int sataAssignedCount = 0;

            if (sataCdLimit > 0)
            {
                if (primaryController != "sata")
                {
                    sb.AppendLine("sata0.present = \"TRUE\"");
                }

                int startSataIndex = (primaryController == "sata") ? 1 : 0;

                for (int i = 0; i < sataCdLimit; i++)
                {
                    int index = startSataIndex + i;
                    if (index > 10) break;

                    string sataKey = $"sata0:{index}";
                    bool isIsoSlot = (sataAssignedCount == 0 && hasIso && isoCtrl == "sata");

                    sb.AppendLine($"{sataKey}.present = \"TRUE\"");
                    sb.AppendLine($"{sataKey}.deviceType = \"cdrom-image\"");
                    if (isIsoSlot)
                    {
                        sb.AppendLine($"{sataKey}.fileName = \"{config.IsoPath}\"");
                        sb.AppendLine($"{sataKey}.startConnected = \"TRUE\"");
                    }
                    else
                    {
                        sb.AppendLine($"{sataKey}.startConnected = \"FALSE\"");
                    }
                    sataAssignedCount++;
                }
            }

            // 11. Custom icon if present
            if (!string.IsNullOrEmpty(config.IconPath))
            {
                string iconFileName = Path.GetFileName(config.IconPath);
                sb.AppendLine($"gui.customizedIcon = \"{iconFileName}\"");
            }

            File.WriteAllText(vmxFilePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Centralized shortcut creation utility method using WScript.Shell.
        /// </summary>
        public static void CreateShortcut(string shortcutPath, string vmxFilePath, string iconPath, string targetExecutable, string arguments)
        {
            Type wshShellType = Type.GetTypeFromProgID("WScript.Shell");
            if (wshShellType != null)
            {
                object wshShell = Activator.CreateInstance(wshShellType);
                object shortcut = wshShellType.InvokeMember(
                    "CreateShortcut",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null,
                    wshShell,
                    new object[] { shortcutPath }
                );

                shortcut.GetType().InvokeMember("TargetPath", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { targetExecutable });
                shortcut.GetType().InvokeMember("Arguments", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { arguments ?? string.Empty });
                shortcut.GetType().InvokeMember("WorkingDirectory", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { Path.GetDirectoryName(vmxFilePath) });

                if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
                {
                    shortcut.GetType().InvokeMember("IconLocation", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { iconPath + ",0" });
                }

                shortcut.GetType().InvokeMember("Save", System.Reflection.BindingFlags.InvokeMethod, null, shortcut, null);
            }
        }

        /// <summary>
        /// Automatically generates helper scripts inside the virtual machine folder to display the shortcut generator UI form, fully localized via INI language files with dynamic RTL and availability tagging.
        /// </summary>
        public static void CreateShortcutUtilityScript(string vmFolder)
        {
            try
            {
                string scriptPath = Path.Combine(vmFolder, "CreateShortcuts.bat");
                string psScriptPath = Path.Combine(vmFolder, "CreateShortcuts.ps1");

                // Read settings.ini and language file from the main application directory at build time
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string iniPath = Path.Combine(appDir, "settings.ini");
                string langFilePath = "";

                if (File.Exists(iniPath))
                {
                    foreach (var line in File.ReadAllLines(iniPath, Encoding.UTF8))
                    {
                        if (line.Trim().StartsWith("LanguageFile", StringComparison.OrdinalIgnoreCase))
                        {
                            var parts = line.Split('=');
                            if (parts.Length > 1) langFilePath = parts[1].Trim();
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(langFilePath) && !Path.IsPathRooted(langFilePath))
                {
                    langFilePath = Path.Combine(appDir, langFilePath);
                }

                Dictionary<string, string> langDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrEmpty(langFilePath) && File.Exists(langFilePath))
                {
                    foreach (var line in File.ReadAllLines(langFilePath, Encoding.UTF8))
                    {
                        var idx = line.IndexOf('=');
                        if (idx > 0)
                        {
                            string k = line.Substring(0, idx).Trim();
                            string v = line.Substring(idx + 1).Trim();
                            langDict[k] = v;
                        }
                    }
                }

                string GetText(string key, string def)
                {
                    return langDict.TryGetValue(key, out var val) ? val : def;
                }

                string direction = "LTR";
                if (langDict.TryGetValue("Direction", out var dirVal))
                {
                    direction = dirVal;
                }

                string wTitle = GetText("ShortcutWindowTitle", "Create Virtual Machine Shortcuts");
                string sNameLbl = GetText("ShortcutNameLabel", "Shortcut name:");
                string gbType = GetText("groupBoxShortcutType", "Shortcut Type");
                string rbDirText = GetText("radioShortcutDirect", "Direct VMX file shortcut");
                string rbKvmText = GetText("radioShortcutKvm", "KVM shortcut");
                string rbPlText = GetText("radioShortcutPlayer", "VMware Player shortcut");
                string rbWsText = GetText("radioShortcutWorkstation", "Custom Workstation shortcut");
                string chkNewWinText = GetText("checkBoxNewWindow", "Always open in a new window");
                string chkStartText = GetText("checkBoxShortStart", "Start the virtual machine automatically");
                string chkFullText = GetText("checkBoxFullScreen", "Full screen");
                string chkCloseText = GetText("checkBoxClose", "Close automatically after power off");
                string chkDeskText = GetText("checkBoxDesktop", "Add desktop shortcut");
                string chkStartMenuText = GetText("checkBoxStartMenu", "Add start menu shortcut");
                string notAvailText = GetText("NotAvailableText", "(Not available)");
                string btnCreateText = GetText("btnCreate", "Create");
                string btnCancelText = GetText("btnWelcomeCancel", "Cancel");
                string successMsg = GetText("ShortcutSuccessMsg", "Shortcuts created successfully!");
                string successTitle = GetText("ShortcutSuccessTitle", "Complete");
                string errTitle = GetText("ErrorTitle", "Error");
                string errNoVmx = GetText("ErrorNoVmxSelected", "No VMX file selected. Exiting.");
                string errVmxNotFound = GetText("ErrorVmxNotFound", "VMX file not found automatically. Please select the VMX file manually.");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Add-Type -AssemblyName System.Windows.Forms");
                sb.AppendLine("Add-Type -AssemblyName System.Drawing");
                sb.AppendLine("$CurrentDir = $PSScriptRoot");
                sb.AppendLine("if (-not $CurrentDir) { $CurrentDir = Split-Path -Parent $MyInvocation.MyCommand.Definition }");

                sb.AppendLine("$VmxFile = Get-ChildItem -Path $CurrentDir -Filter '*.vmx' | Select-Object -First 1");
                sb.AppendLine("if (-not $VmxFile) {");
                sb.AppendLine("    $ParentDir = Split-Path -Parent $CurrentDir");
                sb.AppendLine("    $VmxFile = Get-ChildItem -Path $ParentDir -Filter '*.vmx' | Select-Object -First 1");
                sb.AppendLine("}");
                sb.AppendLine("if (-not $VmxFile) {");
                sb.AppendLine($"    [System.Windows.Forms.MessageBox]::Show('{errVmxNotFound.Replace("'", "''")}', '{errTitle.Replace("'", "''")}', 0, 64)");
                sb.AppendLine("    $ofd = New-Object System.Windows.Forms.OpenFileDialog");
                sb.AppendLine("    $ofd.Filter = 'VMware VMX Files (*.vmx)|*.vmx'");
                sb.AppendLine("    $ofd.InitialDirectory = $CurrentDir");
                sb.AppendLine("    if ($ofd.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {");
                sb.AppendLine("        $VmxFile = Get-Item $ofd.FileName");
                sb.AppendLine("    }");
                sb.AppendLine("}");
                sb.AppendLine("if (-not $VmxFile) {");
                sb.AppendLine($"    [System.Windows.Forms.MessageBox]::Show('{errNoVmx.Replace("'", "''")}', '{errTitle.Replace("'", "''")}', 0, 16)");
                sb.AppendLine("    exit");
                sb.AppendLine("}");

                sb.AppendLine("$DefaultName = $VmxFile.BaseName");
                sb.AppendLine("$VmxPath = $VmxFile.FullName");

                sb.AppendLine("$VmwarePath = 'C:\\Program Files (x86)\\VMware\\VMware Workstation\\vmware.exe'");
                sb.AppendLine("try {");
                sb.AppendLine("    $key = Get-ItemProperty -Path 'HKLM:\\SOFTWARE\\VMware, Inc.\\VMware Workstation' -ErrorAction Stop");
                sb.AppendLine("    if ($key -and $key.InstallPath) {");
                sb.AppendLine("        $p = Join-Path $key.InstallPath 'vmware.exe'");
                sb.AppendLine("        if (Test-Path $p) { $VmwarePath = $p }");
                sb.AppendLine("    }");
                sb.AppendLine("} catch {}");

                sb.AppendLine("$VmwareDir = Split-Path -Parent $VmwarePath");
                sb.AppendLine("$HasWorkstation = Test-Path (Join-Path $VmwareDir 'vmware.exe')");
                sb.AppendLine("$HasPlayer = Test-Path (Join-Path $VmwareDir 'vmplayer.exe')");
                sb.AppendLine("$HasKvm = Test-Path (Join-Path $VmwareDir 'vmware-kvm.exe')");

                sb.AppendLine("$form = New-Object System.Windows.Forms.Form");
                sb.AppendLine($"$form.Text = '{wTitle.Replace("'", "''")}'");
                sb.AppendLine("$form.Size = New-Object System.Drawing.Size(460, 445)");
                sb.AppendLine("$form.FormBorderStyle = 'FixedDialog'");
                sb.AppendLine("$form.MaximizeBox = $false");
                sb.AppendLine("$form.MinimizeBox = $false");
                sb.AppendLine("$form.StartPosition = 'CenterScreen'");

                // Dynamic RTL and layout direction injection
                sb.AppendLine($"$direction = '{direction}'");
                sb.AppendLine("if ($direction -eq 'RTL') {");
                sb.AppendLine("    $form.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes");
                sb.AppendLine("    $form.RightToLeftLayout = $true");
                sb.AppendLine("}");

                sb.AppendLine("$lbl = New-Object System.Windows.Forms.Label");
                sb.AppendLine($"$lbl.Text = '{sNameLbl.Replace("'", "''")}'");
                sb.AppendLine("$lbl.Location = New-Object System.Drawing.Point(20, 20)");
                sb.AppendLine("$lbl.AutoSize = $true");
                sb.AppendLine("$form.Controls.Add($lbl)");

                sb.AppendLine("$txt = New-Object System.Windows.Forms.TextBox");
                sb.AppendLine("$txt.Location = New-Object System.Drawing.Point(140, 17)");
                sb.AppendLine("$txt.Size = New-Object System.Drawing.Size(280, 23)");
                sb.AppendLine("$txt.Text = $DefaultName");
                sb.AppendLine("$form.Controls.Add($txt)");

                sb.AppendLine("$gb = New-Object System.Windows.Forms.GroupBox");
                sb.AppendLine($"$gb.Text = '{gbType.Replace("'", "''")}'");
                sb.AppendLine("$gb.Location = New-Object System.Drawing.Point(20, 55)");
                sb.AppendLine("$gb.Size = New-Object System.Drawing.Size(400, 120)");
                sb.AppendLine("if ($direction -eq 'RTL') { $gb.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes }");
                sb.AppendLine("$form.Controls.Add($gb)");

                // Default selection set to Direct VMX shortcut (rbDirect) with full RTL support
                sb.AppendLine("$rbDirect = New-Object System.Windows.Forms.RadioButton");
                sb.AppendLine("$rbDirect.Name = 'rbDirect'");
                sb.AppendLine($"$rbDirect.Text = '{rbDirText.Replace("'", "''")}'");
                sb.AppendLine("$rbDirect.Location = New-Object System.Drawing.Point(15, 20)");
                sb.AppendLine("$rbDirect.AutoSize = $true");
                sb.AppendLine("$rbDirect.Checked = $true");
                sb.AppendLine("if ($direction -eq 'RTL') { $rbDirect.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes }");
                sb.AppendLine("$gb.Controls.Add($rbDirect)");

                sb.AppendLine("$rbKvm = New-Object System.Windows.Forms.RadioButton");
                sb.AppendLine("$rbKvm.Name = 'rbKvm'");
                sb.AppendLine($"$rbKvm.Text = '{rbKvmText.Replace("'", "''")}'");
                sb.AppendLine("$rbKvm.Location = New-Object System.Drawing.Point(15, 43)");
                sb.AppendLine("$rbKvm.AutoSize = $true");
                sb.AppendLine("$rbKvm.Enabled = $HasKvm");
                sb.AppendLine("if ($direction -eq 'RTL') { $rbKvm.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes }");
                sb.AppendLine("$gb.Controls.Add($rbKvm)");

                sb.AppendLine("$rbPlayer = New-Object System.Windows.Forms.RadioButton");
                sb.AppendLine("$rbPlayer.Name = 'rbPlayer'");
                sb.AppendLine($"$rbPlayer.Text = '{rbPlText.Replace("'", "''")}'");
                sb.AppendLine("$rbPlayer.Location = New-Object System.Drawing.Point(15, 66)");
                sb.AppendLine("$rbPlayer.AutoSize = $true");
                sb.AppendLine("$rbPlayer.Enabled = $HasPlayer");
                sb.AppendLine("if ($direction -eq 'RTL') { $rbPlayer.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes }");
                sb.AppendLine("$gb.Controls.Add($rbPlayer)");

                sb.AppendLine("$rbWs = New-Object System.Windows.Forms.RadioButton");
                sb.AppendLine("$rbWs.Name = 'rbWs'");
                sb.AppendLine($"$rbWs.Text = '{rbWsText.Replace("'", "''")}'");
                sb.AppendLine("$rbWs.Location = New-Object System.Drawing.Point(15, 89)");
                sb.AppendLine("$rbWs.AutoSize = $true");
                sb.AppendLine("$rbWs.Enabled = $HasWorkstation");
                sb.AppendLine("if ($direction -eq 'RTL') { $rbWs.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes }");
                sb.AppendLine("$gb.Controls.Add($rbWs)");

                sb.AppendLine("$chkNewWin = New-Object System.Windows.Forms.CheckBox");
                sb.AppendLine($"$chkNewWin.Text = '{chkNewWinText.Replace("'", "''")}'");
                sb.AppendLine("$chkNewWin.Location = New-Object System.Drawing.Point(35, 185)");
                sb.AppendLine("$chkNewWin.AutoSize = $true");
                sb.AppendLine("$chkNewWin.Checked = $true");
                sb.AppendLine("$form.Controls.Add($chkNewWin)");

                sb.AppendLine("$chkStart = New-Object System.Windows.Forms.CheckBox");
                sb.AppendLine($"$chkStart.Text = '{chkStartText.Replace("'", "''")}'");
                sb.AppendLine("$chkStart.Location = New-Object System.Drawing.Point(35, 210)");
                sb.AppendLine("$chkStart.AutoSize = $true");
                sb.AppendLine("$chkStart.Checked = $true");
                sb.AppendLine("$form.Controls.Add($chkStart)");

                sb.AppendLine("$chkFull = New-Object System.Windows.Forms.CheckBox");
                sb.AppendLine($"$chkFull.Text = '{chkFullText.Replace("'", "''")}'");
                sb.AppendLine("$chkFull.Location = New-Object System.Drawing.Point(55, 235)");
                sb.AppendLine("$chkFull.AutoSize = $true");
                sb.AppendLine("$form.Controls.Add($chkFull)");

                sb.AppendLine("$chkClose = New-Object System.Windows.Forms.CheckBox");
                sb.AppendLine($"$chkClose.Text = '{chkCloseText.Replace("'", "''")}'");
                sb.AppendLine("$chkClose.Location = New-Object System.Drawing.Point(35, 260)");
                sb.AppendLine("$chkClose.AutoSize = $true");
                sb.AppendLine("$chkClose.Checked = $true");
                sb.AppendLine("$form.Controls.Add($chkClose)");

                sb.AppendLine("$chkDesk = New-Object System.Windows.Forms.CheckBox");
                sb.AppendLine($"$chkDesk.Text = '{chkDeskText.Replace("'", "''")}'");
                sb.AppendLine("$chkDesk.Location = New-Object System.Drawing.Point(20, 295)");
                sb.AppendLine("$chkDesk.AutoSize = $true");
                sb.AppendLine("$chkDesk.Checked = $true");
                sb.AppendLine("$form.Controls.Add($chkDesk)");

                sb.AppendLine("$chkStartMenu = New-Object System.Windows.Forms.CheckBox");
                sb.AppendLine($"$chkStartMenu.Text = '{chkStartMenuText.Replace("'", "''")}'");
                sb.AppendLine("$chkStartMenu.Location = New-Object System.Drawing.Point(220, 295)");
                sb.AppendLine("$chkStartMenu.AutoSize = $true");
                sb.AppendLine("$chkStartMenu.Checked = $true");
                sb.AppendLine("$form.Controls.Add($chkStartMenu)");

                sb.AppendLine("if ($direction -eq 'RTL') {");
                sb.AppendLine("    $chkNewWin.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes");
                sb.AppendLine("    $chkStart.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes");
                sb.AppendLine("    $chkFull.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes");
                sb.AppendLine("    $chkClose.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes");
                sb.AppendLine("    $chkDesk.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes");
                sb.AppendLine("    $chkStartMenu.RightToLeft = [System.Windows.Forms.RightToLeft]::Yes");
                sb.AppendLine("}");

                sb.AppendLine("function Refresh-SuffixLabels {");
                sb.AppendLine("    $radioButtons = @($rbWs, $rbPlayer, $rbKvm)");
                sb.AppendLine("    foreach ($rb in $radioButtons) {");
                sb.AppendLine("        $parent = $rb.Parent");
                sb.AppendLine("        $labelName = $rb.Name + '_SuffixLabel'");
                sb.AppendLine("        $existing = $parent.Controls | Where-Object { $_.Name -eq $labelName }");
                sb.AppendLine("        if ($existing) { $parent.Controls.Remove($existing); $existing.Dispose() }");
                sb.AppendLine("        if (-not $rb.Enabled) {");
                sb.AppendLine("            $lblSuffix = New-Object System.Windows.Forms.Label");
                sb.AppendLine("            $lblSuffix.Name = $labelName");
                sb.AppendLine($"            $lblSuffix.Text = '{notAvailText.Replace("'", "''")}'");
                sb.AppendLine("            $lblSuffix.AutoSize = $true");
                sb.AppendLine("            $lblSuffix.Font = New-Object System.Drawing.Font($form.Font.FontFamily, 8, [System.Drawing.FontStyle]::Italic)");
                sb.AppendLine("            $lblSuffix.ForeColor = [System.Drawing.Color]::Gray");
                sb.AppendLine("            $textSize = [System.Windows.Forms.TextRenderer]::MeasureText($rb.Text, $rb.Font)");
                sb.AppendLine("            $x = $rb.Left + 20 + $textSize.Width + 6");
                sb.AppendLine("            $y = $rb.Top + [Math]::Max(0, ($rb.Height - 15) / 2)");
                sb.AppendLine("            $lblSuffix.Location = New-Object System.Drawing.Point($x, $y)");
                sb.AppendLine("            $parent.Controls.Add($lblSuffix)");
                sb.AppendLine("            $lblSuffix.BringToFront()");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                // Align radio buttons properly to the right side inside GroupBox when RTL is enabled
                sb.AppendLine("$form.add_Shown({");
                sb.AppendLine("    Refresh-SuffixLabels");
                sb.AppendLine("    if ($direction -eq 'RTL') {");
                sb.AppendLine("        foreach($rb in @($rbDirect, $rbKvm, $rbPlayer, $rbWs)) {");
                sb.AppendLine("            $rb.Left = $gb.Width - 15 - $rb.Width");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("})");

                sb.AppendLine("$updateWs = {");
                sb.AppendLine("    $isWs = $rbWs.Checked");
                sb.AppendLine("    $isPlayer = $rbPlayer.Checked");
                sb.AppendLine("    $chkNewWin.Enabled = $isWs");
                sb.AppendLine("    $chkStart.Enabled = $isWs");
                sb.AppendLine("    $chkClose.Enabled = $isWs");
                sb.AppendLine("    if ($isPlayer) {");
                sb.AppendLine("        $chkFullScreenEnabled = $true");
                sb.AppendLine("    } else {");
                sb.AppendLine("        $chkFullScreenEnabled = $isWs -and $chkStart.Checked");
                sb.AppendLine("    }");
                sb.AppendLine("    $chkFull.Enabled = $chkFullScreenEnabled");
                sb.AppendLine("}");

                sb.AppendLine("$rbDirect.Add_CheckedChanged($updateWs)");
                sb.AppendLine("$rbKvm.Add_CheckedChanged($updateWs)");
                sb.AppendLine("$rbPlayer.Add_CheckedChanged($updateWs)");
                sb.AppendLine("$rbWs.Add_CheckedChanged($updateWs)");
                sb.AppendLine("$chkStart.Add_CheckedChanged({ if ($rbWs.Checked) { $chkFull.Enabled = $chkStart.Checked; if (-not $chkStart.Checked) { $chkFull.Checked = $false } } })");

                sb.AppendLine("&$updateWs");

                sb.AppendLine("$btnCreate = New-Object System.Windows.Forms.Button");
                sb.AppendLine($"$btnCreate.Text = '{btnCreateText.Replace("'", "''")}'");
                sb.AppendLine("$btnCreate.Location = New-Object System.Drawing.Point(254, 355)");
                sb.AppendLine("$btnCreate.Size = New-Object System.Drawing.Size(75, 23)");
                sb.AppendLine("$form.AcceptButton = $btnCreate");
                sb.AppendLine("$form.Controls.Add($btnCreate)");

                sb.AppendLine("$btnCancel = New-Object System.Windows.Forms.Button");
                sb.AppendLine($"$btnCancel.Text = '{btnCancelText.Replace("'", "''")}'");
                sb.AppendLine("$btnCancel.Location = New-Object System.Drawing.Point(345, 355)");
                sb.AppendLine("$btnCancel.Size = New-Object System.Drawing.Size(75, 23)");
                sb.AppendLine("$btnCancel.DialogResult = [System.Windows.Forms.DialogResult]::Cancel");
                sb.AppendLine("$form.CancelButton = $btnCancel");
                sb.AppendLine("$form.Controls.Add($btnCancel)");

                sb.AppendLine("$btnCreate.Add_Click({");
                sb.AppendLine("    $name = $txt.Text.Trim()");
                sb.AppendLine("    if (-not $name) { $name = $DefaultName }");
                sb.AppendLine("    $target = $VmwarePath");
                sb.AppendLine("    $args = ''");
                sb.AppendLine("    if ($rbDirect.Checked) {");
                sb.AppendLine("        $target = $VmxPath");
                sb.AppendLine("    } elseif ($rbKvm.Checked) {");
                sb.AppendLine("        $target = 'kvm'");
                sb.AppendLine("        $args = \"`\"$VmxPath`\"\"");
                sb.AppendLine("    } elseif ($rbPlayer.Checked) {");
                sb.AppendLine("        $playerPath = Join-Path $VmwareDir 'vmplayer.exe'");
                sb.AppendLine("        if (Test-Path $playerPath) { $target = $playerPath }");
                sb.AppendLine("        if ($chkFull.Checked) { $args += '-X ' }");
                sb.AppendLine("        $args += \"`\"$VmxPath`\"\"");
                sb.AppendLine("    } else {");
                sb.AppendLine("        if ($chkNewWin.Checked) { $args += '-n ' }");
                sb.AppendLine("        if ($chkStart.Checked) {");
                sb.AppendLine("            if ($chkFull.Checked) { $args += '-X ' } else { $args += '-x ' }");
                sb.AppendLine("        }");
                sb.AppendLine("        if ($chkClose.Checked) { $args += '-q ' }");
                sb.AppendLine("        $args += \"`\"$VmxPath`\"\"");
                sb.AppendLine("    }");
                sb.AppendLine("    $ico = Join-Path $CurrentDir \"$DefaultName.ico\"");
                sb.AppendLine("    if (-not (Test-Path $ico)) { $ico = $VmxPath }");
                sb.AppendLine("    try {");
                sb.AppendLine("        $shell = New-Object -ComObject WScript.Shell");
                sb.AppendLine("        if ($chkDesk.Checked) {");
                sb.AppendLine("            $scPath = Join-Path ([Environment]::GetFolderPath('Desktop')) \"$name.lnk\"");
                sb.AppendLine("            $sc = $shell.CreateShortcut($scPath)");
                sb.AppendLine("            $sc.TargetPath = $target");
                sb.AppendLine("            $sc.Arguments = $args");
                sb.AppendLine("            $sc.WorkingDirectory = $CurrentDir");
                sb.AppendLine("            if (Test-Path $ico) { $sc.IconLocation = \"$ico,0\" }");
                sb.AppendLine("            $sc.Save()");
                sb.AppendLine("        }");
                sb.AppendLine("        if ($chkStartMenu.Checked) {");
                sb.AppendLine("            $smDir = Join-Path ([Environment]::GetFolderPath('Programs')) 'My Virtual Machines'");
                sb.AppendLine("            if (-not (Test-Path $smDir)) { New-Item -ItemType Directory -Path $smDir | Out-Null }");
                sb.AppendLine("            $scPath = Join-Path $smDir \"$name.lnk\"");
                sb.AppendLine("            $sc = $shell.CreateShortcut($scPath)");
                sb.AppendLine("            $sc.TargetPath = $target");
                sb.AppendLine("            $sc.Arguments = $args");
                sb.AppendLine("            $sc.WorkingDirectory = $CurrentDir");
                sb.AppendLine("            if (Test-Path $ico) { $sc.IconLocation = \"$ico,0\" }");
                sb.AppendLine("            $sc.Save()");
                sb.AppendLine("        }");
                sb.AppendLine($"        [System.Windows.Forms.MessageBox]::Show('{successMsg.Replace("'", "''")}', '{successTitle.Replace("'", "''")}', 0, 64)");
                sb.AppendLine("        $form.Close()");
                sb.AppendLine("    } catch {");
                sb.AppendLine($"        [System.Windows.Forms.MessageBox]::Show(\"Error: $_\", '{errTitle.Replace("'", "''")}', 0, 16)");
                sb.AppendLine("    }");
                sb.AppendLine("})");

                sb.AppendLine("[void]$form.ShowDialog()");

                File.WriteAllText(psScriptPath, sb.ToString(), new UTF8Encoding(true));

                string batContent = "@echo off\r\ncd /d \"%~dp0\"\r\npowershell -ExecutionPolicy Bypass -WindowStyle Hidden -File \"%~dp0CreateShortcuts.ps1\"";
                File.WriteAllText(scriptPath, batContent, Encoding.Default);
            }
            catch { }
        }

        /// <summary>
        /// Creates the real virtual disk using vmware-vdiskmanager.exe.
        /// </summary>
        public static void CreateVirtualDisk(VmConfig config, string vmwareExePath)
        {
            string vmwareDir = Path.GetDirectoryName(vmwareExePath);
            string vdiskManagerPath = Path.Combine(vmwareDir, "vmware-vdiskmanager.exe");

            if (!File.Exists(vdiskManagerPath))
            {
                string[] fallbacks = {
                    @"C:\Program Files (x86)\VMware\VMware Workstation\vmware-vdiskmanager.exe",
                    @"C:\Program Files\VMware\VMware Workstation\vmware-vdiskmanager.exe"
                };
                foreach (var fb in fallbacks)
                    if (File.Exists(fb)) { vdiskManagerPath = fb; break; }
            }

            if (!File.Exists(vdiskManagerPath))
                throw new FileNotFoundException("VDISKMANAGER_NOT_FOUND:" + vdiskManagerPath);

            if (File.Exists(config.DiskPath))
            {
                try { File.Delete(config.DiskPath); } catch { }
            }

            int t = config.GetDiskType();
            string args = $"-c -s {config.DiskCapacityGB}GB -a lsilogic -t {t} \"{config.DiskPath}\"";

            var psi = new ProcessStartInfo(vdiskManagerPath, args)
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
                    string output = proc.StandardError.ReadToEnd() + "\n" + proc.StandardOutput.ReadToEnd();
                    string lower = output.ToLower();

                    if (lower.Contains("already exists") || lower.Contains("already_exists") || lower.Contains("file exists") || lower.Contains("0xc"))
                    {
                        throw new IOException("FILE_ALREADY_EXISTS: File already exists - " + output);
                    }
                    else if (lower.Contains("disk is full") || lower.Contains("filling the disk") || lower.Contains("0x8") || lower.Contains("not enough space"))
                    {
                        throw new IOException("DISK_FULL:" + output);
                    }
                    else
                    {
                        throw new IOException($"VDISKMANAGER_FAILED:{proc.ExitCode}:{output}");
                    }
                }
            }
        }

        /// <summary>
        /// Unified method to generate VMX, virtual disk, and shortcut utility script.
        /// </summary>
        public static void CreateVirtualMachine(string vmxFilePath, VmConfig config, string vmwareExePath)
        {
            CreateVmxFile(vmxFilePath, config);

            string vmFolder = Path.GetDirectoryName(vmxFilePath);
            if (!string.IsNullOrEmpty(vmFolder))
            {
                CreateShortcutUtilityScript(vmFolder);
            }

            if (!string.IsNullOrEmpty(vmwareExePath))
            {
                CreateVirtualDisk(config, vmwareExePath);
            }
        }

        /// <summary>
        /// Backward compatibility method to generate VMX and shortcut utility script without disk creation.
        /// </summary>
        public static void CreateVirtualMachine(string vmxFilePath, VmConfig config)
        {
            CreateVmxFile(vmxFilePath, config);

            string vmFolder = Path.GetDirectoryName(vmxFilePath);
            if (!string.IsNullOrEmpty(vmFolder))
            {
                CreateShortcutUtilityScript(vmFolder);
            }
        }
    }
}
