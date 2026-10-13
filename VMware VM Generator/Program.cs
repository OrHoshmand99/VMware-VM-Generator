using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace VMware_VM_Generator
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Load language settings dynamically from settings.ini and the language INI file
            IniFile settings = new IniFile("settings.ini");
            string langFileName = settings.Read("LanguageFile", "General");
            IniFile lang = !string.IsNullOrWhiteSpace(langFileName) && File.Exists(langFileName)
                ? new IniFile(langFileName)
                : null;

            // Helper function to get localized text with fallbacks
            string GetText(string key, string defaultText)
            {
                if (lang != null)
                {
                    string val = lang.Read(key, "Language");
                    if (!string.IsNullOrWhiteSpace(val)) return val;
                }
                return defaultText;
            }

            // Check reading direction from language file (default to LTR if not specified)
            string direction = lang != null ? lang.Read("Direction", "Language") : "LTR";
            bool isRtl = !string.IsNullOrWhiteSpace(direction) && direction.Equals("RTL", StringComparison.OrdinalIgnoreCase);

            // Check if VMware Workstation is installed before launching Form1
            if (!CheckVMwareInstallation())
            {
                string title = GetText("VMwareRequiredTitle", "VMware Workstation Required");
                string message = GetText("VMwareRequiredMessage",
                    "VMware Workstation is not installed on this computer.\n\n" +
                    "This application requires VMware Workstation to create and launch virtual machines. " +
                    "Would you like to open the direct download releases page on GitHub now?");

                // Apply RTL options to MessageBox if specified in the language file
                MessageBoxOptions options = 0;
                if (isRtl)
                {
                    options = MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
                }

                DialogResult result = MessageBox.Show(
                    message,
                    title,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    options);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://github.com/201853910/VMwareWorkstation/releases",
                            UseShellExecute = true
                        });
                    }
                    catch
                    {
                        // Fallback if browser opening fails
                    }
                }

                return; // Exits the application immediately
            }

            Application.Run(new Form1());
        }

        private static bool CheckVMwareInstallation()
        {
            // 1. Check Windows Registry
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\VMware, Inc.\VMware Workstation") ??
                                  Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\VMware, Inc.\VMware Workstation"))
                {
                    if (key != null)
                    {
                        string installPath = key.GetValue("InstallPath") as string;
                        if (!string.IsNullOrEmpty(installPath))
                        {
                            string fullPath = Path.Combine(installPath, "vmware-vdiskmanager.exe");
                            if (File.Exists(fullPath))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore registry errors
            }

            // 2. Check standard common paths across drives
            string[] commonDrives = { "C:", "D:", "E:" };
            foreach (string drive in commonDrives)
            {
                string[] standardPaths = {
                    $@"{drive}\Program Files\VMware\VMware Workstation\vmware-vdiskmanager.exe",
                    $@"{drive}\Program Files (x86)\VMware\VMware Workstation\vmware-vdiskmanager.exe"
                };

                foreach (string path in standardPaths)
                {
                    if (File.Exists(path))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}