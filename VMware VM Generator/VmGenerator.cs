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
            public int NumVcpus { get; set; } // Processor/socket count from UI
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

            // Hardware Version and Guest OS from settings.ini
            public string HardwareVersion { get; set; } = "19";
            public string GuestOs { get; set; } = "other-64";

            // Network adapter type from settings.ini (conditional injection)
            public string EthernetVirtualDev { get; set; }

            // Required fields for real disk creation
            public int DiskCapacityGB { get; set; } = 40;
            public bool AllocateAllSpace { get; set; } = false;
            public bool SplitIntoFiles { get; set; } = false;

            /// <summary>
            /// Correct mapping for -t parameter:
            /// !Allocate + Single = 0 (growable single file)
            /// !Allocate + Split  = 1 (growable split 2GB files)
            /// Allocate + Single  = 2 (preallocated single file)
            /// Allocate + Split   = 3 (preallocated split)
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

        // Create VMX file following the exact mandatory and structural template specifications
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

            // 2. Configurable hardware version and guest OS (with robust validation & fallback)
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

            // 4. Conditional network adapter type (injected ONLY if explicitly defined in settings.ini)
            if (!string.IsNullOrWhiteSpace(config.EthernetVirtualDev))
            {
                sb.AppendLine($"ethernet0.virtualDev = \"{config.EthernetVirtualDev.Trim()}\"");
            }

            // 5. User-defined settings (Calculating total vCPUs as Sockets * CoresPerSocket)
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

            // 7. Floppy drives configuration (Present, empty, and disconnected at power on)
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

            // Auto-detect isoCtrl if empty based on available CD counts or primary controller
            if (hasIso && string.IsNullOrEmpty(isoCtrl))
            {
                if (config.SataCdCount > 0 || primaryController == "sata" || primaryController == "nvme")
                    isoCtrl = "sata";
                else if (config.IdeCdCount > 0 || primaryController == "ide")
                    isoCtrl = "ide";
                else if (config.ScsiCdCount > 0 || primaryController == "scsi")
                    isoCtrl = "scsi";
                else
                    isoCtrl = "sata"; // Default fallback for modern VMs
            }

            // 8. IDE CD/DVD drives configuration (Max 3: ide0:1, ide1:0, ide1:1)
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
                }
                scsiAssignedCount++;
            }

            // 10. SATA CD/DVD drives configuration (Max 10 drives, preventing duplicate empty slots when ISO takes the first slot)
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

                // If primary controller is SATA, disk is sata0:0, so first CD starts at sata0:1.
                // Otherwise, first CD starts at sata0:0.
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
        /// Create real virtual disk using vmware-vdiskmanager.exe
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

            // Delete old dummy file if exists
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

        // Unified method
        public static void CreateVirtualMachine(string vmxFilePath, VmConfig config, string vmwareExePath = null)
        {
            CreateVmxFile(vmxFilePath, config);

            if (!string.IsNullOrEmpty(vmwareExePath))
            {
                CreateVirtualDisk(config, vmwareExePath);
            }
        }

        // Backward compatibility
        public static void CreateVirtualMachine(string vmxFilePath, VmConfig config)
        {
            CreateVmxFile(vmxFilePath, config);
        }
    }
}
