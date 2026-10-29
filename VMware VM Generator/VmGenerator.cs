using System;
using System.IO;
using System.Text;
using System.Diagnostics;

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
            sb.AppendLine($"ethernet0.connectionType = \"{config.NetworkType}\"");

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
                if (isIsoSlot)
                {
                    sb.AppendLine($"{ideKey}.deviceType = \"cdrom-image\"");
                    sb.AppendLine($"{ideKey}.fileName = \"{config.IsoPath}\"");
                    sb.AppendLine($"{ideKey}.startConnected = \"TRUE\"");
                }
                else
                {
                    sb.AppendLine($"{ideKey}.deviceType = \"cdrom-image\"");
                    sb.AppendLine($"{ideKey}.startConnected = \"FALSE\"");
                }
                ideAssignedCount++;
            }

            // 9. SCSI CD/DVD drives configuration (Max 10 drives, skipping reserved scsi0:7)
            int scsiCdRequested = Math.Max(config.ScsiCdCount, 0);
            int totalScsiDrives = (hasIso && isoCtrl == "scsi") ? Math.Max(scsiCdRequested, 1) : scsiCdRequested;
            int scsiCdLimit = Math.Min(totalScsiDrives, 10);
            int scsiAssignedCount = 0;

            for (int i = 1; scsiAssignedCount < scsiCdLimit && i <= 11; i++)
            {
                if (i == 7) continue; // Index 7 is reserved for the SCSI controller itself

                string scsiKey = $"scsi0:{i}";
                bool isIsoSlot = (scsiAssignedCount == 0 && hasIso && isoCtrl == "scsi");

                sb.AppendLine($"{scsiKey}.present = \"TRUE\"");
                if (isIsoSlot)
                {
                    sb.AppendLine($"{scsiKey}.deviceType = \"cdrom-image\"");
                    sb.AppendLine($"{scsiKey}.fileName = \"{config.IsoPath}\"");
                    sb.AppendLine($"{scsiKey}.startConnected = \"TRUE\"");
                }
                else
                {
                    sb.AppendLine($"{scsiKey}.deviceType = \"cdrom-image\"");
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
                    if (isIsoSlot)
                    {
                        sb.AppendLine($"{sataKey}.deviceType = \"cdrom-image\"");
                        sb.AppendLine($"{sataKey}.fileName = \"{config.IsoPath}\"");
                        sb.AppendLine($"{sataKey}.startConnected = \"TRUE\"");
                    }
                    else
                    {
                        sb.AppendLine($"{sataKey}.deviceType = \"cdrom-image\"");
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
        /// Automatically generates helper scripts inside the virtual machine folder to display the shortcut generator UI form.
        /// </summary>
        public static void CreateShortcutUtilityScript(string vmFolder)
        {
            try
            {
                string scriptPath = Path.Combine(vmFolder, "CreateShortcuts.bat");
                string psScriptPath = Path.Combine(vmFolder, "CreateShortcuts.ps1");

                StringBuilder ps = new StringBuilder();
                ps.AppendLine("Add-Type -AssemblyName System.Windows.Forms");
                ps.AppendLine("Add-Type -AssemblyName System.Drawing");
                ps.AppendLine("$CurrentDir = $PSScriptRoot");
                ps.AppendLine("if (-not $CurrentDir) { $CurrentDir = Split-Path -Parent $MyInvocation.MyCommand.Definition }");

                // Flexible VMX search: check current folder, then parent folder, or prompt user via OpenFileDialog if not found
                ps.AppendLine("$VmxFile = Get-ChildItem -Path $CurrentDir -Filter \"*.vmx\" | Select-Object -First 1");
                ps.AppendLine("if (-not $VmxFile) {");
                ps.AppendLine("    $ParentDir = Split-Path -Parent $CurrentDir");
                ps.AppendLine("    $VmxFile = Get-ChildItem -Path $ParentDir -Filter \"*.vmx\" | Select-Object -First 1");
                ps.AppendLine("}");
                ps.AppendLine("if (-not $VmxFile) {");
                ps.AppendLine("    [System.Windows.Forms.MessageBox]::Show(\"VMX file not found automatically. Please select the VMX file manually.\", \"Notice\", 0, 64)");
                ps.AppendLine("    $ofd = New-Object System.Windows.Forms.OpenFileDialog");
                ps.AppendLine("    $ofd.Filter = \"VMware VMX Files (*.vmx)|*.vmx\"");
                ps.AppendLine("    $ofd.InitialDirectory = $CurrentDir");
                ps.AppendLine("    if ($ofd.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {");
                ps.AppendLine("        $VmxFile = Get-Item $ofd.FileName");
                ps.AppendLine("    }");
                ps.AppendLine("}");
                ps.AppendLine("if (-not $VmxFile) {");
                ps.AppendLine("    [System.Windows.Forms.MessageBox]::Show(\"No VMX file selected. Exiting.\", \"Error\", 0, 16)");
                ps.AppendLine("    exit");
                ps.AppendLine("}");

                ps.AppendLine("$DefaultName = $VmxFile.BaseName");
                ps.AppendLine("$VmxPath = $VmxFile.FullName");

                // Build compact WinForms dialog matching Wizard Step 6
                ps.AppendLine("$form = New-Object System.Windows.Forms.Form");
                ps.AppendLine("$form.Text = \"Create Virtual Machine Shortcuts\"");
                ps.AppendLine("$form.Size = New-Object System.Drawing.Size(460, 415)");
                ps.AppendLine("$form.FormBorderStyle = 'FixedDialog'");
                ps.AppendLine("$form.MaximizeBox = $false");
                ps.AppendLine("$form.MinimizeBox = $false");
                ps.AppendLine("$form.StartPosition = 'CenterScreen'");

                // Shortcut Name Label & TextBox
                ps.AppendLine("$lbl = New-Object System.Windows.Forms.Label");
                ps.AppendLine("$lbl.Text = \"Shortcut name:\"");
                ps.AppendLine("$lbl.Location = New-Object System.Drawing.Point(20, 20)");
                ps.AppendLine("$lbl.AutoSize = $true");
                ps.AppendLine("$form.Controls.Add($lbl)");

                ps.AppendLine("$txt = New-Object System.Windows.Forms.TextBox");
                ps.AppendLine("$txt.Location = New-Object System.Drawing.Point(140, 17)");
                ps.AppendLine("$txt.Size = New-Object System.Drawing.Size(280, 23)");
                ps.AppendLine("$txt.Text = $DefaultName");
                ps.AppendLine("$form.Controls.Add($txt)");

                // GroupBox for Shortcut Type
                ps.AppendLine("$gb = New-Object System.Windows.Forms.GroupBox");
                ps.AppendLine("$gb.Text = \"Shortcut Type\"");
                ps.AppendLine("$gb.Location = New-Object System.Drawing.Point(20, 55)");
                ps.AppendLine("$gb.Size = New-Object System.Drawing.Size(400, 95)");
                ps.AppendLine("$form.Controls.Add($gb)");

                ps.AppendLine("$rbDirect = New-Object System.Windows.Forms.RadioButton");
                ps.AppendLine("$rbDirect.Text = \"Direct VMX file shortcut\"");
                ps.AppendLine("$rbDirect.Location = New-Object System.Drawing.Point(15, 20)");
                ps.AppendLine("$rbDirect.AutoSize = $true");
                ps.AppendLine("$gb.Controls.Add($rbDirect)");

                ps.AppendLine("$rbKvm = New-Object System.Windows.Forms.RadioButton");
                ps.AppendLine("$rbKvm.Text = \"KVM shortcut\"");
                ps.AppendLine("$rbKvm.Location = New-Object System.Drawing.Point(15, 43)");
                ps.AppendLine("$rbKvm.AutoSize = $true");
                ps.AppendLine("$gb.Controls.Add($rbKvm)");

                ps.AppendLine("$rbWs = New-Object System.Windows.Forms.RadioButton");
                ps.AppendLine("$rbWs.Text = \"Custom Workstation shortcut\"");
                ps.AppendLine("$rbWs.Location = New-Object System.Drawing.Point(15, 66)");
                ps.AppendLine("$rbWs.AutoSize = $true");
                ps.AppendLine("$rbWs.Checked = $true");
                ps.AppendLine("$gb.Controls.Add($rbWs)");

                // Advanced Workstation Checkboxes
                ps.AppendLine("$chkNewWin = New-Object System.Windows.Forms.CheckBox");
                ps.AppendLine("$chkNewWin.Text = \"Always open in a new window\"");
                ps.AppendLine("$chkNewWin.Location = New-Object System.Drawing.Point(35, 160)");
                ps.AppendLine("$chkNewWin.AutoSize = $true");
                ps.AppendLine("$chkNewWin.Checked = $true");
                ps.AppendLine("$form.Controls.Add($chkNewWin)");

                ps.AppendLine("$chkStart = New-Object System.Windows.Forms.CheckBox");
                ps.AppendLine("$chkStart.Text = \"Start the virtual machine automatically\"");
                ps.AppendLine("$chkStart.Location = New-Object System.Drawing.Point(35, 185)");
                ps.AppendLine("$chkStart.AutoSize = $true");
                ps.AppendLine("$chkStart.Checked = $true");
                ps.AppendLine("$form.Controls.Add($chkStart)");

                ps.AppendLine("$chkFull = New-Object System.Windows.Forms.CheckBox");
                ps.AppendLine("$chkFull.Text = \"Full screen\"");
                ps.AppendLine("$chkFull.Location = New-Object System.Drawing.Point(55, 210)");
                ps.AppendLine("$chkFull.AutoSize = $true");
                ps.AppendLine("$form.Controls.Add($chkFull)");

                ps.AppendLine("$chkClose = New-Object System.Windows.Forms.CheckBox");
                ps.AppendLine("$chkClose.Text = \"Close automatically after power off\"");
                ps.AppendLine("$chkClose.Location = New-Object System.Drawing.Point(35, 235)");
                ps.AppendLine("$chkClose.AutoSize = $true");
                ps.AppendLine("$form.Controls.Add($chkClose)");

                // Location Checkboxes (Desktop & Start Menu)
                ps.AppendLine("$chkDesk = New-Object System.Windows.Forms.CheckBox");
                ps.AppendLine("$chkDesk.Text = \"Add desktop shortcut\"");
                ps.AppendLine("$chkDesk.Location = New-Object System.Drawing.Point(20, 270)");
                ps.AppendLine("$chkDesk.AutoSize = $true");
                ps.AppendLine("$chkDesk.Checked = $true");
                ps.AppendLine("$form.Controls.Add($chkDesk)");

                ps.AppendLine("$chkStartMenu = New-Object System.Windows.Forms.CheckBox");
                ps.AppendLine("$chkStartMenu.Text = \"Add start menu shortcut\"");
                ps.AppendLine("$chkStartMenu.Location = New-Object System.Drawing.Point(220, 270)");
                ps.AppendLine("$chkStartMenu.AutoSize = $true");
                ps.AppendLine("$chkStartMenu.Checked = $true");
                ps.AppendLine("$form.Controls.Add($chkStartMenu)");

                // Enable/Disable logic for Workstation options
                ps.AppendLine("$updateWs = {");
                ps.AppendLine("    $isWs = $rbWs.Checked");
                ps.AppendLine("    $chkNewWin.Enabled = $isWs");
                ps.AppendLine("    $chkStart.Enabled = $isWs");
                ps.AppendLine("    $chkFull.Enabled = $isWs -and $chkStart.Checked");
                ps.AppendLine("    $chkClose.Enabled = $isWs");
                ps.AppendLine("}");
                ps.AppendLine("$rbDirect.Add_CheckedChanged($updateWs)");
                ps.AppendLine("$rbKvm.Add_CheckedChanged($updateWs)");
                ps.AppendLine("$rbWs.Add_CheckedChanged($updateWs)");
                ps.AppendLine("$chkStart.Add_CheckedChanged({ $chkFull.Enabled = $rbWs.Checked -and $chkStart.Checked; if (-not $chkStart.Checked) { $chkFull.Checked = $false } })");

                // Buttons
                ps.AppendLine("$btnCreate = New-Object System.Windows.Forms.Button");
                ps.AppendLine("$btnCreate.Text = \"Create\"");
                ps.AppendLine("$btnCreate.Location = New-Object System.Drawing.Point(254, 330)");
                ps.AppendLine("$btnCreate.Size = New-Object System.Drawing.Size(75, 23)");
                ps.AppendLine("$form.AcceptButton = $btnCreate");
                ps.AppendLine("$form.Controls.Add($btnCreate)");

                ps.AppendLine("$btnCancel = New-Object System.Windows.Forms.Button");
                ps.AppendLine("$btnCancel.Text = \"Cancel\"");
                ps.AppendLine("$btnCancel.Location = New-Object System.Drawing.Point(345, 330)");
                ps.AppendLine("$btnCancel.Size = New-Object System.Drawing.Size(75, 23)");
                ps.AppendLine("$btnCancel.DialogResult = [System.Windows.Forms.DialogResult]::Cancel");
                ps.AppendLine("$form.CancelButton = $btnCancel");
                ps.AppendLine("$form.Controls.Add($btnCancel)");

                // Auto-detect VMware path function
                ps.AppendLine("function Get-VMwarePath {");
                ps.AppendLine("    try {");
                ps.AppendLine("        $key = Get-ItemProperty -Path 'HKLM:\\SOFTWARE\\VMware, Inc.\\VMware Workstation' -ErrorAction Stop");
                ps.AppendLine("        if ($key -and $key.InstallPath) {");
                ps.AppendLine("            $p = Join-Path $key.InstallPath \"vmware.exe\"");
                ps.AppendLine("            if (Test-Path $p) { return $p }");
                ps.AppendLine("        }");
                ps.AppendLine("    } catch {}");
                ps.AppendLine("    foreach ($path in @(\"C:\\Program Files (x86)\\VMware\\VMware Workstation\\vmware.exe\", \"C:\\Program Files\\VMware\\VMware Workstation\\vmware.exe\")) {");
                ps.AppendLine("        if (Test-Path $path) { return $path }");
                ps.AppendLine("    }");
                ps.AppendLine("    return \"C:\\Program Files (x86)\\VMware\\VMware Workstation\\vmware.exe\"");
                ps.AppendLine("}");

                // Create Button Click Action
                ps.AppendLine("$btnCreate.Add_Click({");
                ps.AppendLine("    $name = $txt.Text.Trim()");
                ps.AppendLine("    if (-not $name) { $name = $DefaultName }");
                ps.AppendLine("    $vmwareExe = Get-VMwarePath");
                ps.AppendLine("    $target = $vmwareExe");
                ps.AppendLine("    $args = \"\"");
                ps.AppendLine("    if ($rbDirect.Checked) {");
                ps.AppendLine("        $target = $VmxPath");
                ps.AppendLine("    } elseif ($rbKvm.Checked) {");
                ps.AppendLine("        $target = \"kvm\"");
                ps.AppendLine("        $args = \"`\"$VmxPath`\"\"");
                ps.AppendLine("    } else {");
                ps.AppendLine("        if ($chkNewWin.Checked) { $args += \"-n \" }");
                ps.AppendLine("        if ($chkStart.Checked) {");
                ps.AppendLine("            if ($chkFull.Checked) { $args += \"-X \" } else { $args += \"-x \" }");
                ps.AppendLine("        }");
                ps.AppendLine("        if ($chkClose.Checked) { $args += \"-q \" }");
                ps.AppendLine("        $args += \"`\"$VmxPath`\"\"");
                ps.AppendLine("    }");
                ps.AppendLine("    $ico = Join-Path $CurrentDir \"$DefaultName.ico\"");
                ps.AppendLine("    if (-not (Test-Path $ico)) { $ico = $VmxPath }");
                ps.AppendLine("    try {");
                ps.AppendLine("        $shell = New-Object -ComObject WScript.Shell");
                ps.AppendLine("        if ($chkDesk.Checked) {");
                ps.AppendLine("            $scPath = Join-Path ([Environment]::GetFolderPath('Desktop')) \"$name.lnk\"");
                ps.AppendLine("            $sc = $shell.CreateShortcut($scPath)");
                ps.AppendLine("            $sc.TargetPath = $target");
                ps.AppendLine("            $sc.Arguments = $args");
                ps.AppendLine("            $sc.WorkingDirectory = $CurrentDir");
                ps.AppendLine("            if (Test-Path $ico) { $sc.IconLocation = \"$ico,0\" }");
                ps.AppendLine("            $sc.Save()");
                ps.AppendLine("        }");
                ps.AppendLine("        if ($chkStartMenu.Checked) {");
                ps.AppendLine("            $smDir = Join-Path ([Environment]::GetFolderPath('Programs')) \"My Virtual Machines\"");
                ps.AppendLine("            if (-not (Test-Path $smDir)) { New-Item -ItemType Directory -Path $smDir | Out-Null }");
                ps.AppendLine("            $scPath = Join-Path $smDir \"$name.lnk\"");
                ps.AppendLine("            $sc = $shell.CreateShortcut($scPath)");
                ps.AppendLine("            $sc.TargetPath = $target");
                ps.AppendLine("            $sc.Arguments = $args");
                ps.AppendLine("            $sc.WorkingDirectory = $CurrentDir");
                ps.AppendLine("            if (Test-Path $ico) { $sc.IconLocation = \"$ico,0\" }");
                ps.AppendLine("            $sc.Save()");
                ps.AppendLine("        }");
                ps.AppendLine("        [System.Windows.Forms.MessageBox]::Show(\"Shortcuts created successfully!\", \"Complete\", 0, 64)");
                ps.AppendLine("        $form.Close()");
                ps.AppendLine("    } catch {");
                ps.AppendLine("        [System.Windows.Forms.MessageBox]::Show(\"Error: $_\", \"Error\", 0, 16)");
                ps.AppendLine("    }");
                ps.AppendLine("})");

                ps.AppendLine("[void]$form.ShowDialog()");

                File.WriteAllText(psScriptPath, ps.ToString(), new UTF8Encoding(false));

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
        public static void CreateVirtualMachine(string vmxFilePath, VmConfig config, string vmwareExePath = null)
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
