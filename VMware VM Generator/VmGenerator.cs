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
            public int FloppyCount { get; set; }
            public int IdeCdCount { get; set; }
            public int ScsiCdCount { get; set; }
            public int SataCdCount { get; set; }
            public string IconPath { get; set; }

            // New: Required fields for real disk creation
            public int DiskCapacityGB { get; set; } = 40;
            public bool AllocateAllSpace { get; set; } = false; // checkBoxAllocate
            public bool SplitIntoFiles { get; set; } = false; // radioButtonSplit

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

        // Create VMX file only
        public static void CreateVmxFile(string vmxFilePath, VmConfig config)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(".encoding = \"UTF-8\"");
            sb.AppendLine("config.version = \"8\"");
            sb.AppendLine("virtualHW.version = \"19\"");
            sb.AppendLine($"displayname = \"{config.VmName}\"");
            sb.AppendLine($"memsize = \"{config.MemoryMb}\"");
            sb.AppendLine($"numvcpus = \"{config.NumVcpus}\"");
            sb.AppendLine($"cpuid.coresPerSocket = \"{config.CoresPerSocket}\"");
            sb.AppendLine($"firmware = \"{config.Firmware}\"");

            // Sound Card
            if (!string.IsNullOrEmpty(config.SoundVirtualDev) && config.SoundVirtualDev != "none")
            {
                sb.AppendLine("sound.present = \"TRUE\"");
                sb.AppendLine("sound.autoDetect = \"TRUE\"");
                sb.AppendLine($"sound.virtualDev = \"{config.SoundVirtualDev}\"");
            }
            else
            {
                sb.AppendLine("sound.present = \"FALSE\"");
            }

            // Network Connection
            if (config.NetworkType != "none")
            {
                sb.AppendLine("ethernet0.present = \"TRUE\"");
                sb.AppendLine("ethernet0.virtualDev = \"e1000e\"");
                sb.AppendLine($"ethernet0.connectionType = \"{config.NetworkType}\"");
                sb.AppendLine("ethernet0.startConnected = \"TRUE\"");
            }
            else
            {
                sb.AppendLine("ethernet0.present = \"FALSE\"");
            }

            // Storage Controller & Disk
            string primaryController = config.StorageControllerType.ToLower();

            if (primaryController == "ide")
            {
                sb.AppendLine("ide0:0.present = \"TRUE\"");
                sb.AppendLine($"ide0:0.fileName = \"{Path.GetFileName(config.DiskPath)}\"");
                sb.AppendLine("ide0:0.deviceType = \"disk\"");
            }
            else if (primaryController == "scsi")
            {
                sb.AppendLine("scsi0.present = \"TRUE\"");
                sb.AppendLine($"scsi0.virtualDev = \"{config.ScsiVirtualDev}\"");
                sb.AppendLine("scsi0:0.present = \"TRUE\"");
                sb.AppendLine($"scsi0:0.fileName = \"{Path.GetFileName(config.DiskPath)}\"");
                sb.AppendLine("scsi0:0.deviceType = \"disk\"");
            }
            else if (primaryController == "sata")
            {
                sb.AppendLine("sata0.present = \"TRUE\"");
                sb.AppendLine("sata0:0.present = \"TRUE\"");
                sb.AppendLine($"sata0:0.fileName = \"{Path.GetFileName(config.DiskPath)}\"");
                sb.AppendLine("sata0:0.deviceType = \"disk\"");
            }
            else if (primaryController == "nvme")
            {
                sb.AppendLine("nvme0.present = \"TRUE\"");
                sb.AppendLine("nvme0:0.present = \"TRUE\"");
                sb.AppendLine($"nvme0:0.fileName = \"{Path.GetFileName(config.DiskPath)}\"");
                sb.AppendLine("nvme0:0.deviceType = \"disk\"");
            }

            // Floppy drives
            for (int i = 0; i < config.FloppyCount; i++)
                sb.AppendLine($"floppy{i}.present = \"TRUE\"");

            // IDE CD/DVD drives
            for (int i = 0; i < config.IdeCdCount; i++)
            {
                sb.AppendLine($"ide1:{i}.present = \"TRUE\"");
                sb.AppendLine($"ide1:{i}.deviceType = \"cdrom-image\"");
                sb.AppendLine($"ide1:{i}.autodetect = \"TRUE\"");
            }

            // SCSI CD/DVD drives
            for (int i = 0; i < config.ScsiCdCount; i++)
            {
                sb.AppendLine($"scsi0:{i + 1}.present = \"TRUE\"");
                sb.AppendLine($"scsi0:{i + 1}.deviceType = \"cdrom-image\"");
            }

            // SATA CD/DVD drives
            for (int i = 0; i < config.SataCdCount; i++)
            {
                sb.AppendLine($"sata0:{i + 1}.present = \"TRUE\"");
                sb.AppendLine($"sata0:{i + 1}.deviceType = \"cdrom-image\"");
            }

            // Custom icon if present
            if (!string.IsNullOrEmpty(config.IconPath))
            {
                string iconFileName = Path.GetFileName(config.IconPath);
                sb.AppendLine($"gui.customizedIcon = \"{iconFileName}\"");
            }

            File.WriteAllText(vmxFilePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Create real virtual disk using vmware-vdiskmanager.exe
        /// Throws exception with marker that will be caught in Form1 and translated via Hebrew.ini
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

                    // So Form1 can identify error type via Hebrew.ini - order matters
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

        // Unified method - this is what Form1 should call
        public static void CreateVirtualMachine(string vmxFilePath, VmConfig config, string vmwareExePath = null)
        {
            // 1. Create VMX
            CreateVmxFile(vmxFilePath, config);

            // 2. If VMware path is provided, also create real disk
            if (!string.IsNullOrEmpty(vmwareExePath))
            {
                CreateVirtualDisk(config, vmwareExePath);
            }
        }

        // Backward compatibility - old call without path
        public static void CreateVirtualMachine(string vmxFilePath, VmConfig config)
        {
            CreateVmxFile(vmxFilePath, config);
            // No disk creation to avoid breaking old code. Previous dummy file bug removed.
        }
    }
}
