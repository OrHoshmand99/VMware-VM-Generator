using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using VMware_VM_Generator;

namespace VMware_VM_Generator_Studio
{
    public partial class Form1 : Form
    {
        private const string ProjectExtension = ".vmp";
        private const string TemplateExtension = ".vmt";
        private const string DefaultIconRelativePath = @"resources\themes\default\icon.ico";
        private const string ProjectDefaultIconKey = "VM_Default_Icon";

        private GroupBox groupBoxImageAssets;
        private Panel panelPreviewContainer;

        private bool isUpdatingSoundCheckboxes = false;
        private bool isUpdatingSCSICheckboxes = false;
        private bool isUpdatingHardDriveCheckboxes = false;
        private bool isUpdatingNetworkCheckboxes = false;

        private byte[]? originalIconBytes = null;

        // Tracks whether the current project has unsaved changes
        private bool isModified = false;

        private readonly Dictionary<string, string> defaultThemeFiles = new(StringComparer.OrdinalIgnoreCase)
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

        private readonly int[] memoryStops = new int[]
        {
            4, 8, 16, 32, 64, 128, 256, 512,
            1024, 2048, 4096, 8192,
            16384, 32768, 65536, 131072
        };

        [DllImport("shell32.dll", EntryPoint = "#62", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool PickIconDlg(IntPtr hwnd, StringBuilder pszIconPath, int cchIconPath, ref int piIconIndex);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, StringBuilder iconPath, ref int piIconIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        private readonly List<ImageAssetDefinition> imageAssets = new()
        {
            new("Window Background", 589, 369),
            new("Header Background", 707, 85),
            new("Footer Background", 707, 59),
            new("Banner", 118, 369),
            new("Header Icon", 85, 85),
            new("Welcome Icon", 32, 32),
            new("Folder Icon", 32, 32),
            new("Firmware Icon", 16, 16),
            new("Processor Icon", 16, 16),
            new("Memory Icon", 16, 16),
            new("SCSI Icon", 16, 16),
            new("Sound Icon", 16, 16),
            new("Network Icon", 16, 16),
            new("Hard Drive Icon", 32, 32),
            new("Floppy Icon", 16, 16),
            new("CD/DVD Icon", 16, 16),
            new("Finish Icon", 32, 32)
        };

        private readonly Dictionary<string, Image> selectedAssetImages = new(StringComparer.OrdinalIgnoreCase);

        private string currentFilePath = string.Empty;
        private bool isTemplateFile;

        public Form1()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            InitializeComponent();
            InitializeFontPreviewEvents();

            if (toolStrip1 != null)
            {
                toolStrip1.ShowItemToolTips = true;
            }

            if (newToolStripButton != null) newToolStripButton.ToolTipText = "New (Ctrl+N)";
            if (openToolStripButton != null) openToolStripButton.ToolTipText = "Open (Ctrl+O)";
            if (saveToolStripButton != null) saveToolStripButton.ToolTipText = "Save (Ctrl+S)";
            if (buildToolStripButton != null) buildToolStripButton.ToolTipText = "Build (Ctrl+B)";
            if (helpToolStripButton != null) helpToolStripButton.ToolTipText = "Help";

            if (buildToolStripMenuItem != null) buildToolStripMenuItem.Click += (s, e) => PerformBuild();
            if (buildToolStripButton != null) buildToolStripButton.Click += (s, e) => PerformBuild();

            if (pictureBoxSelectedImage != null)
            {
                pictureBoxSelectedImage.SizeMode = PictureBoxSizeMode.Normal;
            }

            if (tabPageHardware != null) tabPageHardware.AutoScroll = true;
            if (tabPageOS != null) tabPageOS.AutoScroll = true;

            try
            {
                var prop = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (prop != null)
                {
                    void EnableDoubleBuffer(Control c)
                    {
                        prop.SetValue(c, true, null);
                        foreach (Control ch in c.Controls) EnableDoubleBuffer(ch);
                    }
                    EnableDoubleBuffer(this);
                }
            }
            catch { }

            this.Load += Form1_Load;
            this.Resize += Form1_Resize;

            newToolStripMenuItem.Click += NewToolStripMenuItem_Click;
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;

            if (saveAsTemplateMenuItem != null) saveAsTemplateMenuItem.Click += SaveAsTemplateToolStripMenuItem_Click;

            newToolStripButton.Click += NewToolStripMenuItem_Click;
            openToolStripButton.Click += OpenToolStripMenuItem_Click;
            saveToolStripButton.Click += SaveToolStripMenuItem_Click;
            buttonChangeIcon.Click += ButtonChangeIcon_Click;
            buttonVMLocationBrowse.Click += ButtonVMLocationBrowse_Click;
            buttonVMLocationVariables.Click += ButtonVMLocationVariables_Click;
            buttonTextColor.Click += ButtonTextColor_Click;
            buttonSelectImageAsset.Click += ButtonSelectImageAsset_Click;
            listBoxImageAssets.SelectedIndexChanged += ListBoxImageAssets_SelectedIndexChanged;

            if (buttonIsoBrowse != null) buttonIsoBrowse.Click += ButtonIsoBrowse_Click;
            if (radioButtonIsoIDE != null) radioButtonIsoIDE.CheckedChanged += RadioButtonIsoController_CheckedChanged;
            if (radioButtonIsoSCSI != null) radioButtonIsoSCSI.CheckedChanged += RadioButtonIsoController_CheckedChanged;
            if (radioButtonIsoSATA != null) radioButtonIsoSATA.CheckedChanged += RadioButtonIsoController_CheckedChanged;

            comboBoxNumberOfProcessors.SelectedIndexChanged += ProcessorSelectionChanged;
            comboBoxNumberOfProcessors.TextChanged += ProcessorSelectionChanged;
            comboBoxNumberOfProcessors.KeyPress += NumericOnly_KeyPress;

            comboBoxNumberOfCoresPerProcessor.SelectedIndexChanged += ProcessorSelectionChanged;
            comboBoxNumberOfCoresPerProcessor.TextChanged += ProcessorSelectionChanged;
            comboBoxNumberOfCoresPerProcessor.KeyPress += NumericOnly_KeyPress;

            if (comboBoxLanguage != null) comboBoxLanguage.SelectedIndexChanged += ComboBoxLanguage_SelectedIndexChanged;

            if (checkBoxSB16 != null) { checkBoxSB16.Click += SoundCardCheckbox_Click; checkBoxSB16.CheckedChanged += SoundCardCheckbox_CheckedChanged; }
            if (checkBoxES1371 != null) { checkBoxES1371.Click += SoundCardCheckbox_Click; checkBoxES1371.CheckedChanged += SoundCardCheckbox_CheckedChanged; }
            if (checkBoxHDAudio != null) { checkBoxHDAudio.Click += SoundCardCheckbox_Click; checkBoxHDAudio.CheckedChanged += SoundCardCheckbox_CheckedChanged; }

            if (checkBoxBusLogic != null) { checkBoxBusLogic.Click += SCSICheckbox_Click; checkBoxBusLogic.CheckedChanged += SCSICheckbox_CheckedChanged; }
            if (checkBoxLSILogic != null) { checkBoxLSILogic.Click += SCSICheckbox_Click; checkBoxLSILogic.CheckedChanged += SCSICheckbox_CheckedChanged; }
            if (checkBoxLSILogicSAS != null) { checkBoxLSILogicSAS.Click += SCSICheckbox_Click; checkBoxLSILogicSAS.CheckedChanged += SCSICheckbox_CheckedChanged; }
            if (checkBoxParavirtualized != null) { checkBoxParavirtualized.Click += SCSICheckbox_Click; checkBoxParavirtualized.CheckedChanged += SCSICheckbox_CheckedChanged; }

            if (checkBoxIDE != null) { checkBoxIDE.Click += HardDriveCheckbox_Click; checkBoxIDE.CheckedChanged += HardDriveCheckbox_CheckedChanged; }
            if (checkBoxSCSI != null) { checkBoxSCSI.Click += HardDriveCheckbox_Click; checkBoxSCSI.CheckedChanged += HardDriveCheckbox_CheckedChanged; }
            if (checkBoxSATA != null) { checkBoxSATA.Click += HardDriveCheckbox_Click; checkBoxSATA.CheckedChanged += HardDriveCheckbox_CheckedChanged; }
            if (checkBoxNVMe != null) { checkBoxNVMe.Click += HardDriveCheckbox_Click; checkBoxNVMe.CheckedChanged += HardDriveCheckbox_CheckedChanged; }

            if (checkBoxE1000PCI != null) { checkBoxE1000PCI.Click += NetworkCheckbox_Click; checkBoxE1000PCI.CheckedChanged += NetworkCheckbox_CheckedChanged; }
            if (checkBoxE1000PCIe != null) { checkBoxE1000PCIe.Click += NetworkCheckbox_Click; checkBoxE1000PCIe.CheckedChanged += NetworkCheckbox_CheckedChanged; }
            if (checkBoxPCNet32 != null) { checkBoxPCNet32.Click += NetworkCheckbox_Click; checkBoxPCNet32.CheckedChanged += NetworkCheckbox_CheckedChanged; }
            if (checkBoxradioButtonVMXNET3 != null) { checkBoxradioButtonVMXNET3.Click += NetworkCheckbox_Click; checkBoxradioButtonVMXNET3.CheckedChanged += NetworkCheckbox_CheckedChanged; }

            InitializeMemoryControls();
            InitializeGuestOSMenu();
            LoadLanguagesFromDirectory();

            if (groupBoxMemory != null)
            {
                groupBoxMemory.Paint += GroupBoxMemory_Paint;
                groupBoxMemory.MouseDown += GroupBoxMemory_MouseDown;
                groupBoxMemory.MouseMove += GroupBoxMemory_MouseMove;
            }

            pictureBoxSelectedImage.Paint += PanelSelectedImage_Paint;

            buttonTextColor.Text = string.Empty;
            buttonTextColor.ForeColor = Color.Black;
            buttonTextColor.BackColor = Color.Black;
            buttonTextColor.UseVisualStyleBackColor = false;
            buttonTextColor.Width = 82;
            buttonTextColor.Height = 24;
            buttonTextColor.FlatStyle = FlatStyle.Flat;
            buttonTextColor.FlatAppearance.BorderColor = SystemColors.ControlDark;
            buttonTextColor.FlatAppearance.BorderSize = 1;

            PopulateInstalledFonts();
            PopulateProcessorOptions();
            ResetFormState();
            PopulateImageAssetList();
            UpdateFileCommandLabels();
            UpdateTotalCoresCalculation();

            // Wire up modification tracking for user inputs
            RegisterModificationEvents();
            isModified = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void MarkAsModified(object sender, EventArgs e)
        {
            if (!isModified)
            {
                isModified = true;
                UpdateFileCommandLabels();
            }
        }

        private void RegisterModificationEvents()
        {
            textBoxVMName.TextChanged += MarkAsModified;
            textBoxVMLocation.TextChanged += MarkAsModified;
            comboBoxLanguage.SelectedIndexChanged += MarkAsModified;
            comboBoxLanguage.TextChanged += MarkAsModified;
            buttonTextColor.BackColorChanged += MarkAsModified;
            comboBoxTextFont.SelectedIndexChanged += MarkAsModified;
            comboBoxTextFont.TextChanged += MarkAsModified;
            numericUpDownHardwareVersion.ValueChanged += MarkAsModified;
            numericUpDownMinMemory.ValueChanged += MarkAsModified;
            numericUpDownDefaultMemory.ValueChanged += MarkAsModified;
            numericUpDownMaxMemory.ValueChanged += MarkAsModified;
            trackBarMinMemory.Scroll += MarkAsModified;
            trackBarDefaultMemory.Scroll += MarkAsModified;
            trackBarMaxMemory.Scroll += MarkAsModified;
            radioButtonBIOS.CheckedChanged += MarkAsModified;
            radioButtonUEFI.CheckedChanged += MarkAsModified;
            checkBoxFirmwareForced.CheckedChanged += MarkAsModified;
            comboBoxNumberOfProcessors.SelectedIndexChanged += MarkAsModified;
            comboBoxNumberOfProcessors.TextChanged += MarkAsModified;
            comboBoxNumberOfCoresPerProcessor.SelectedIndexChanged += MarkAsModified;
            comboBoxNumberOfCoresPerProcessor.TextChanged += MarkAsModified;
            checkBoxSB16.CheckedChanged += MarkAsModified;
            checkBoxES1371.CheckedChanged += MarkAsModified;
            checkBoxHDAudio.CheckedChanged += MarkAsModified;
            radioButtonSB16.CheckedChanged += MarkAsModified;
            radioButtonES1371.CheckedChanged += MarkAsModified;
            radioButtonHDAudio.CheckedChanged += MarkAsModified;
            checkBoxBusLogic.CheckedChanged += MarkAsModified;
            checkBoxLSILogic.CheckedChanged += MarkAsModified;
            checkBoxLSILogicSAS.CheckedChanged += MarkAsModified;
            checkBoxParavirtualized.CheckedChanged += MarkAsModified;
            radioButtonBusLogic.CheckedChanged += MarkAsModified;
            radioButtonLSILogic.CheckedChanged += MarkAsModified;
            radioButtonLSILogicSAS.CheckedChanged += MarkAsModified;
            radioButtonParavirtualized.CheckedChanged += MarkAsModified;
            checkBoxE1000PCI.CheckedChanged += MarkAsModified;
            checkBoxE1000PCIe.CheckedChanged += MarkAsModified;
            checkBoxPCNet32.CheckedChanged += MarkAsModified;
            checkBoxradioButtonVMXNET3.CheckedChanged += MarkAsModified;
            radioButtonE1000PCI.CheckedChanged += MarkAsModified;
            radioButtonE1000PCIe.CheckedChanged += MarkAsModified;
            radioButtonPCNet32.CheckedChanged += MarkAsModified;
            radioButtonradioButtonVMXNET3.CheckedChanged += MarkAsModified;
            checkBoxIDE.CheckedChanged += MarkAsModified;
            checkBoxSCSI.CheckedChanged += MarkAsModified;
            checkBoxSATA.CheckedChanged += MarkAsModified;
            checkBoxNVMe.CheckedChanged += MarkAsModified;
            radioButtonIDE.CheckedChanged += MarkAsModified;
            radioButtonSCSI.CheckedChanged += MarkAsModified;
            radioButtonSATA.CheckedChanged += MarkAsModified;
            radioButtonNVMe.CheckedChanged += MarkAsModified;
            numericUpDownHardDriveCapacity.ValueChanged += MarkAsModified;
            numericUpDownFloppyCount.ValueChanged += MarkAsModified;
            numericUpDownIDECDCount.ValueChanged += MarkAsModified;
            numericUpDownSCSICDCount.ValueChanged += MarkAsModified;
            numericUpDownSATACDCount.ValueChanged += MarkAsModified;
            textBoxOSDisplayName.TextChanged += MarkAsModified;
            textBoxGuestOS.TextChanged += MarkAsModified;
            textBoxIsoPath.TextChanged += MarkAsModified;
            radioButtonIsoIDE.CheckedChanged += MarkAsModified;
            radioButtonIsoSCSI.CheckedChanged += MarkAsModified;
            radioButtonIsoSATA.CheckedChanged += MarkAsModified;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (isModified)
            {
                DialogResult result = MessageBox.Show(
                    this,
                    "Do you want to save the changes you made to the project?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    SaveToolStripMenuItem_Click(this, EventArgs.Empty);
                    // If still modified (e.g. user cancelled Save As dialog), cancel closing
                    if (isModified)
                    {
                        e.Cancel = true;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void PerformBuild()
        {
            int numProcessors = 1;
            int coresPerProc = 1;
            int.TryParse(comboBoxNumberOfProcessors.Text, out numProcessors);
            int.TryParse(comboBoxNumberOfCoresPerProcessor.Text, out coresPerProc);

            string guestOSKey = textBoxGuestOS.Text?.Trim();
            if (string.IsNullOrEmpty(guestOSKey)) guestOSKey = "other";

            string colorHex = ColorTranslator.ToHtml(Color.FromArgb(255, buttonTextColor.BackColor));

            var assetImagesDict = new Dictionary<string, string>();
            foreach (var kvp in selectedAssetImages)
            {
                using var ms = new MemoryStream();
                kvp.Value.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                assetImagesDict[kvp.Key] = Convert.ToBase64String(ms.ToArray());
            }

            if (originalIconBytes != null && originalIconBytes.Length > 0)
            {
                assetImagesDict[ProjectDefaultIconKey] = Convert.ToBase64String(originalIconBytes);
            }

            var data = new ProjectData
            {
                RecommendedMinMemory = (int)numericUpDownMinMemory.Value,
                RecommendedDefaultMemory = (int)numericUpDownDefaultMemory.Value,
                RecommendedMaxMemory = (int)numericUpDownMaxMemory.Value,
                VmName = textBoxVMName.Text,
                VmLocation = textBoxVMLocation.Text,
                IconPath = pictureBoxIcon.Tag?.ToString() ?? string.Empty,
                TextColorHex = colorHex,
                TextFontName = comboBoxTextFont.SelectedItem?.ToString(),
                SelectedLanguage = comboBoxLanguage.SelectedItem?.ToString(),
                HardwareVersion = (int)numericUpDownHardwareVersion.Value,
                OSDisplayName = textBoxOSDisplayName.Text,
                GuestOSID = guestOSKey,
                IsoImagePath = textBoxIsoPath.Text,
                IsoControllerType = radioButtonIsoSCSI.Checked ? "SCSI" : (radioButtonIsoSATA.Checked ? "SATA" : "IDE"),
                FirmwareType = radioButtonUEFI.Checked ? "UEFI" : "BIOS",
                FirmwareForced = checkBoxFirmwareForced.Checked,
                NumberOfProcessors = numProcessors,
                CoresPerProcessor = coresPerProc,
                SoundCardSB16Checked = checkBoxSB16.Checked,
                SoundCardSB16Selected = radioButtonSB16.Checked,
                SoundCardES1371Checked = checkBoxES1371.Checked,
                SoundCardES1371Selected = radioButtonES1371.Checked,
                SoundCardHDAudioChecked = checkBoxHDAudio.Checked,
                SoundCardHDAudioSelected = radioButtonHDAudio.Checked,
                SCSIBusLogicChecked = checkBoxBusLogic.Checked,
                SCSIBusLogicSelected = radioButtonBusLogic.Checked,
                SCSILSILogicChecked = checkBoxLSILogic.Checked,
                SCSILSILogicSelected = radioButtonLSILogic.Checked,
                SCSILSILogicSASChecked = checkBoxLSILogicSAS.Checked,
                SCSILSILogicSASSelected = radioButtonLSILogicSAS.Checked,
                SCSIParavirtualizedChecked = checkBoxParavirtualized.Checked,
                SCSIParavirtualizedSelected = radioButtonParavirtualized.Checked,
                HardDriveIDEChecked = checkBoxIDE.Checked,
                HardDriveIDESelected = radioButtonIDE.Checked,
                HardDriveSCSIChecked = checkBoxSCSI.Checked,
                HardDriveSCSISelected = radioButtonSCSI.Checked,
                HardDriveSATASelected = radioButtonSATA.Checked,
                HardDriveSATAChecked = checkBoxSATA.Checked,
                HardDriveNVMeChecked = checkBoxNVMe.Checked,
                HardDriveNVMeSelected = radioButtonNVMe.Checked,
                NetworkE1000PCIChecked = checkBoxE1000PCI.Checked,
                NetworkE1000PCISelected = radioButtonE1000PCI.Checked,
                NetworkE1000PCIeChecked = checkBoxE1000PCIe.Checked,
                NetworkE1000PCIeSelected = radioButtonE1000PCIe.Checked,
                NetworkPCNet32Checked = checkBoxPCNet32.Checked,
                NetworkPCNet32Selected = radioButtonPCNet32.Checked,
                NetworkVMXNET3Checked = checkBoxradioButtonVMXNET3.Checked,
                NetworkVMXNET3Selected = radioButtonradioButtonVMXNET3.Checked,
                HardDriveCapacityGB = (int)numericUpDownHardDriveCapacity.Value,
                FloppyDriveCount = (int)numericUpDownFloppyCount.Value,
                IDECDCount = (int)numericUpDownIDECDCount.Value,
                SCSICDCount = (int)numericUpDownSCSICDCount.Value,
                SATACDCount = (int)numericUpDownSATACDCount.Value,
                AssetImages = assetImagesDict
            };

            BuildManager.BuildVirtualMachine(data, currentFilePath, originalIconBytes);
        }

        private void LoadDefaultThemeAssets()
        {
            foreach (var asset in imageAssets)
            {
                if (defaultThemeFiles.TryGetValue(asset.Name, out string fileName))
                {
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "themes", "default", fileName);
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            using var selected = Image.FromFile(fullPath);
                            var actualSize = CreateScaledImageWithAlpha(selected, new Size(asset.Width, asset.Height));
                            if (selectedAssetImages.TryGetValue(asset.Name, out var oldImg)) oldImg.Dispose();
                            selectedAssetImages[asset.Name] = actualSize;
                        }
                        catch { }
                    }
                }
            }
        }

        private void InitializeFontPreviewEvents()
        {
            if (comboBoxTextFont != null)
            {
                comboBoxTextFont.DrawMode = DrawMode.OwnerDrawFixed;
                comboBoxTextFont.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBoxTextFont.DrawItem += ComboBoxTextFont_DrawItem;
                comboBoxTextFont.MeasureItem += ComboBoxTextFont_MeasureItem;
            }
        }

        private void ComboBoxTextFont_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            string fontName = comboBoxTextFont.Items[e.Index].ToString();
            try
            {
                using (Font displayFont = new Font(fontName, 10, FontStyle.Regular))
                using (Brush textBrush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(fontName, displayFont, textBrush, e.Bounds.X, e.Bounds.Y + 2);
                }
            }
            catch
            {
                using (Brush textBrush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(fontName, e.Font ?? this.Font, textBrush, e.Bounds.X, e.Bounds.Y + 2);
                }
            }
            e.DrawFocusRectangle();
        }

        private void ComboBoxTextFont_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 22;
        }

        private void LoadLanguagesFromDirectory()
        {
            if (comboBoxLanguage == null) return;
            comboBoxLanguage.Items.Clear();
            string languagesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "languages");

            try
            {
                if (Directory.Exists(languagesFolderPath))
                {
                    var iniFiles = Directory.GetFiles(languagesFolderPath, "*.ini");
                    foreach (var filePath in iniFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        comboBoxLanguage.Items.Add(fileName);
                    }
                }
                else
                {
                    Directory.CreateDirectory(languagesFolderPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error loading languages: {ex.Message}", "Language Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!comboBoxLanguage.Items.Cast<object>().Any(item => string.Equals(item.ToString(), "English", StringComparison.OrdinalIgnoreCase)))
            {
                comboBoxLanguage.Items.Add("English");
            }

            int englishIndex = -1;
            for (int i = 0; i < comboBoxLanguage.Items.Count; i++)
            {
                if (string.Equals(comboBoxLanguage.Items[i].ToString(), "English", StringComparison.OrdinalIgnoreCase))
                {
                    englishIndex = i;
                    break;
                }
            }

            if (englishIndex >= 0)
            {
                comboBoxLanguage.SelectedIndex = englishIndex;
            }
            else if (comboBoxLanguage.Items.Count > 0)
            {
                comboBoxLanguage.SelectedIndex = 0;
            }
        }

        private void ComboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLanguage.SelectedItem == null) return;
            string selectedLang = comboBoxLanguage.SelectedItem.ToString();
            string iniFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "languages", $"{selectedLang}.ini");
            if (File.Exists(iniFilePath))
            {
            }
        }

        private void InitializeGuestOSMenu()
        {
            if (textBoxGuestOS != null && buttonOSMenu != null)
            {
                var osContextMenu = new ContextMenuStrip();

                ToolStripItem AddOsItem(string displayName, string vmxKey)
                {
                    return new ToolStripMenuItem(displayName, null, (s, e) => {
                        if (textBoxOSDisplayName != null)
                            textBoxOSDisplayName.Text = displayName;

                        textBoxGuestOS.Text = vmxKey;
                        textBoxGuestOS.SelectionStart = textBoxGuestOS.Text.Length;
                    });
                }

                var menuWindows = new ToolStripMenuItem("Windows");
                menuWindows.DropDownItems.Add(AddOsItem("Windows 11", "windows11-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 10 (64-Bit)", "windows9-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 10 (32-Bit)", "windows9"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 8.x (64-Bit)", "windows8-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 8.x (32-Bit)", "windows8"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 7 (64-Bit)", "windows7-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 7 (32-Bit)", "windows7"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Vista (64-Bit)", "winVista-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Vista (32-Bit)", "winVista"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows XP Professional (64-Bit)", "winXPPro-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows XP Professional (32-Bit)", "winXPPro"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows XP Home Edition", "winXPHome"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 2000 Professional", "win2000Pro"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows NT 4.0", "winNT"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2025", "windows2025srv-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2022", "windows9srv-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2019", "windows2019srv-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2016", "windows9srv-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2012", "win8srv-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Hyper-V (unsupported)", "winhyperv"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2008 R2", "win7srv-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2008 (64-Bit)", "longhorn-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2008 (32-Bit)", "longhorn"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2003 Standard Edition (64-Bit)", "winNetStandard-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2003 Standard Edition (32-Bit)", "winNetStandard"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2003 Enterprise Edition (64-Bit)", "winNetEnterprise-64"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2003 Enterprise Edition (32-Bit)", "winNetEnterprise"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2003 Small Business", "winNetBusiness"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Server 2003 Web Edition", "winNetWeb"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 2000 Server", "win2000Serv"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 2000 Advanced Server", "win2000AdvServ"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows Me", "winMe"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 98", "win98"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 95", "win95"));
                menuWindows.DropDownItems.Add(AddOsItem("Windows 3.1", "win31"));

                var menuLinux = new ToolStripMenuItem("Linux");
                menuLinux.DropDownItems.Add(AddOsItem("AlmaLinux", "almalinux-64"));

                var menuAsianux = new ToolStripMenuItem("Asianux");
                menuAsianux.DropDownItems.Add(AddOsItem("Asianux 4 (64-Bit)", "asianux4-64"));
                menuAsianux.DropDownItems.Add(AddOsItem("Asianux 4 (32-Bit)", "asianux4"));
                menuAsianux.DropDownItems.Add(AddOsItem("Asianux Server 3 (64-Bit)", "asianux3-64"));
                menuAsianux.DropDownItems.Add(AddOsItem("Asianux Server 3 (32-Bit)", "asianux3"));
                menuLinux.DropDownItems.Add(menuAsianux);

                var menuCentOS = new ToolStripMenuItem("CentOS");
                menuCentOS.DropDownItems.Add(AddOsItem("CentOS 8", "centos8-64"));
                menuCentOS.DropDownItems.Add(AddOsItem("CentOS 7", "centos7-64"));
                menuCentOS.DropDownItems.Add(AddOsItem("CentOS 6 (64-Bit)", "centos-64"));
                menuCentOS.DropDownItems.Add(AddOsItem("CentOS 6 (32-Bit)", "centos"));
                menuCentOS.DropDownItems.Add(AddOsItem("CentOS version 5 and earlier (64-Bit)", "centos5-64"));
                menuCentOS.DropDownItems.Add(AddOsItem("CentOS version 5 and earlier (32-Bit)", "centos5"));
                menuLinux.DropDownItems.Add(menuCentOS);

                var menuDebian = new ToolStripMenuItem("Debian");
                menuDebian.DropDownItems.Add(AddOsItem("Debian 13.x", "debian13-64"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 12.x (64-Bit)", "debian12-64"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 12.x (32-Bit)", "debian12"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 11.x (64-Bit)", "debian11-64"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 11.x (32-Bit)", "debian11"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 10.x (64-Bit)", "debian10-64"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 10.x (32-Bit)", "debian10"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 9.x (64-Bit)", "debian9-64"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 9.x (32-Bit)", "debian9"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 8.x (64-Bit)", "debian8-64"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 8.x (32-Bit)", "debian8"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 7.x (64-Bit)", "debian7-64"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 7.x (32-Bit)", "debian7"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 6 (64-Bit)", "debian6-64"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 6 (32-Bit)", "debian6"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 5 (64-Bit)", "debian5-64"));
                menuDebian.DropDownItems.Add(AddOsItem("Debian 5 (32-Bit)", "debian5"));
                menuLinux.DropDownItems.Add(menuDebian);

                var menuFedora = new ToolStripMenuItem("Fedora");
                menuFedora.DropDownItems.Add(AddOsItem("Fedora (64-Bit)", "fedora-64"));
                menuFedora.DropDownItems.Add(AddOsItem("Fedora (32-Bit)", "fedora"));
                menuLinux.DropDownItems.Add(menuFedora);

                var menuMandriva = new ToolStripMenuItem("Mandriva Linux");
                menuMandriva.DropDownItems.Add(AddOsItem("Mandriva Linux (64-Bit)", "mandrake-64"));
                menuMandriva.DropDownItems.Add(AddOsItem("Mandriva Linux (32-Bit)", "mandrake"));
                menuLinux.DropDownItems.Add(menuMandriva);

                menuLinux.DropDownItems.Add(AddOsItem("Novell Linux Desktop 9", "nld9"));

                var menuOpenSUSE = new ToolStripMenuItem("OpenSUSE");
                menuOpenSUSE.DropDownItems.Add(AddOsItem("OpenSUSE (64-Bit)", "opensuse-64"));
                menuOpenSUSE.DropDownItems.Add(AddOsItem("OpenSUSE (32-Bit)", "opensuse"));
                menuLinux.DropDownItems.Add(menuOpenSUSE);

                var menuOracle = new ToolStripMenuItem("Oracle Linux");
                menuOracle.DropDownItems.Add(AddOsItem("Oracle Linux 10", "oraclelinux10-64"));
                menuOracle.DropDownItems.Add(AddOsItem("Oracle Linux 9", "oraclelinux9-64"));
                menuOracle.DropDownItems.Add(AddOsItem("Oracle Linux 8", "oraclelinux8-64"));
                menuOracle.DropDownItems.Add(AddOsItem("Oracle Linux 7", "oraclelinux7-64"));
                menuOracle.DropDownItems.Add(AddOsItem("Oracle Linux 6 (64-Bit)", "rhel6-64"));
                menuOracle.DropDownItems.Add(AddOsItem("Oracle Linux 6 (32-Bit)", "rhel6"));
                menuOracle.DropDownItems.Add(AddOsItem("Oracle Linux version 5 and earlier (64-Bit)", "rhel5-64"));
                menuOracle.DropDownItems.Add(AddOsItem("Oracle Linux version 5 and earlier (32-Bit)", "rhel5"));
                menuLinux.DropDownItems.Add(menuOracle);

                var menuRHEL = new ToolStripMenuItem("Red Hat Enterprise Linux");
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 10", "rhel10-64"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 9", "rhel9-64"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 8", "rhel8-64"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 7", "rhel7-64"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 6 (64-Bit)", "rhel6-64"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 6 (32-Bit)", "rhel6"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 5 (64-Bit)", "rhel5-64"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 5 (32-Bit)", "rhel5"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 4 (64-Bit)", "rhel4-64"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 4 (32-Bit)", "rhel4"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 3 (64-Bit)", "rhel3-64"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 3 (32-Bit)", "rhel3"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Enterprise Linux 2", "rhel2"));
                menuRHEL.DropDownItems.Add(AddOsItem("Red Hat Linux", "redhat"));
                menuLinux.DropDownItems.Add(menuRHEL);

                menuLinux.DropDownItems.Add(AddOsItem("Rocky Linux", "rockylinux-64"));
                menuLinux.DropDownItems.Add(AddOsItem("Sun Java Desktop System", "java-desktop-system"));

                var menuSUSE = new ToolStripMenuItem("SUSE Linux Enterprise");
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux Enterprise 16", "sles16-64"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux Enterprise 15", "sles15-64"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux Enterprise 12", "sles12-64"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux Enterprise 11 (64-Bit)", "sles11-64"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux Enterprise 11 (32-Bit)", "sles11"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux Enterprise 10 (64-Bit)", "sles10-64"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux Enterprise 10 (32-Bit)", "sles10"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux Enterprise 7/8/9 (64-Bit)", "sles9-64"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux Enterprise 7/8/9 (32-Bit)", "sles9"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux (64-Bit)", "suse-64"));
                menuSUSE.DropDownItems.Add(AddOsItem("SUSE Linux (32-Bit)", "suse"));
                menuLinux.DropDownItems.Add(menuSUSE);

                var menuTurbo = new ToolStripMenuItem("Turbolinux");
                menuTurbo.DropDownItems.Add(AddOsItem("Turbolinux (64-Bit)", "turbolinux-64"));
                menuTurbo.DropDownItems.Add(AddOsItem("Turbolinux (32-Bit)", "turbolinux"));
                menuLinux.DropDownItems.Add(menuTurbo);

                var menuUbuntu = new ToolStripMenuItem("Ubuntu");
                menuUbuntu.DropDownItems.Add(AddOsItem("Ubuntu (64-Bit)", "ubuntu-64"));
                menuUbuntu.DropDownItems.Add(AddOsItem("Ubuntu (32-Bit)", "ubuntu"));
                menuLinux.DropDownItems.Add(menuUbuntu);

                menuLinux.DropDownItems.Add(AddOsItem("VMware Photon OS", "photon-64"));

                var menuOtherLinux = new ToolStripMenuItem("Other Linux Kernels");
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 6.x kernel (64-Bit)", "other6xlinux-64"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 6.x kernel (32-Bit)", "other6xlinux"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 5.x kernel (64-Bit)", "other5xlinux-64"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 5.x kernel (32-Bit)", "other5xlinux"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 4.x kernel (64-Bit)", "other4xlinux-64"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 4.x kernel (32-Bit)", "other4xlinux"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 3.x kernel (64-Bit)", "other3xlinux-64"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 3.x kernel (32-Bit)", "other3xlinux"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 2.6.x kernel (64-Bit)", "other26xlinux-64"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 2.6.x kernel (32-Bit)", "other26xlinux"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 2.4.x kernel (64-Bit)", "other24xlinux-64"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 2.4.x kernel (32-Bit)", "other24xlinux"));
                menuOtherLinux.DropDownItems.Add(AddOsItem("Other Linux 2.2.x kernel", "otherlinux"));
                menuLinux.DropDownItems.Add(menuOtherLinux);

                var menuMac = new ToolStripMenuItem("macOS");
                menuMac.DropDownItems.Add(AddOsItem("macOS 27", "darwin26-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 26", "darwin25-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 15", "darwin24-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 14", "darwin23-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 13", "darwin22-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 12", "darwin21-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 11", "darwin20-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 10.15", "darwin19-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 10.14", "darwin18-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 10.13", "darwin17-64"));
                menuMac.DropDownItems.Add(AddOsItem("macOS 10.12", "darwin16-64"));
                menuMac.DropDownItems.Add(AddOsItem("OS X 10.11", "darwin15-64"));
                menuMac.DropDownItems.Add(AddOsItem("OS X 10.10", "darwin14-64"));
                menuMac.DropDownItems.Add(AddOsItem("OS X 10.9", "darwin13-64"));
                menuMac.DropDownItems.Add(AddOsItem("OS X 10.8", "darwin12-64"));
                menuMac.DropDownItems.Add(AddOsItem("Mac OS X 10.7 (64-Bit)", "darwin11-64"));
                menuMac.DropDownItems.Add(AddOsItem("Mac OS X Server 10.6 (64-Bit)", "darwin10-64"));
                menuMac.DropDownItems.Add(AddOsItem("Mac OS X Server 10.5 (64-Bit)", "darwin-64"));
                menuMac.DropDownItems.Add(AddOsItem("Mac OS X 10.7 (32-Bit)", "darwin11"));
                menuMac.DropDownItems.Add(AddOsItem("Mac OS X Server 10.6 (32-Bit)", "darwin10"));
                menuMac.DropDownItems.Add(AddOsItem("Mac OS X Server 10.5 (32-Bit)", "darwin"));

                var menuEsx = new ToolStripMenuItem("VMware ESX");
                menuEsx.DropDownItems.Add(AddOsItem("VMware ESXi 9 and later", "vmkernel9"));
                menuEsx.DropDownItems.Add(AddOsItem("VMware ESXi 8", "vmkernel8"));
                menuEsx.DropDownItems.Add(AddOsItem("VMware ESXi 7", "vmkernel7"));
                menuEsx.DropDownItems.Add(AddOsItem("VMware ESXi 6.x", "vmkernel6"));
                menuEsx.DropDownItems.Add(AddOsItem("VMware ESXi 6.0", "vmkernel6"));
                menuEsx.DropDownItems.Add(AddOsItem("VMware ESXi 5.x", "vmkernel5"));
                menuEsx.DropDownItems.Add(AddOsItem("VMware ESX/ESXi 4.x", "vmkernel"));

                var menuOther = new ToolStripMenuItem("Other");
                menuOther.DropDownItems.Add(AddOsItem("eComStation2", "ecomstation2"));
                menuOther.DropDownItems.Add(AddOsItem("eComStation", "ecomstation"));

                var menuFreeBSD = new ToolStripMenuItem("FreeBSD");
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD 15", "freebsd15-64"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD 14 (64-Bit)", "freebsd14-64"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD 14 (32-Bit)", "freebsd14"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD 13 (64-Bit)", "freebsd13-64"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD 13 (32-Bit)", "freebsd13"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD 12 (64-Bit)", "freebsd12-64"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD 12 (32-Bit)", "freebsd12"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD 11 (64-Bit)", "freebsd11-64"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD 11 (32-Bit)", "freebsd11"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD version 10 and earlier (64-Bit)", "freebsd64"));
                menuFreeBSD.DropDownItems.Add(AddOsItem("FreeBSD version 10 and earlier (32-Bit)", "freebsd"));
                menuOther.DropDownItems.Add(menuFreeBSD);

                menuOther.DropDownItems.Add(AddOsItem("MS-DOS", "dos"));

                var menuNetWare = new ToolStripMenuItem("NetWare");
                menuNetWare.DropDownItems.Add(AddOsItem("NetWare 6", "netware6"));
                menuNetWare.DropDownItems.Add(AddOsItem("NetWare 5", "netware5"));
                menuOther.DropDownItems.Add(menuNetWare);

                menuOther.DropDownItems.Add(AddOsItem("Other (64-Bit)", "other-64"));
                menuOther.DropDownItems.Add(AddOsItem("Other (32-Bit)", "other"));

                var menuSolaris = new ToolStripMenuItem("Solaris");
                menuSolaris.DropDownItems.Add(AddOsItem("Solaris 11", "solaris11-64"));
                menuSolaris.DropDownItems.Add(AddOsItem("Solaris 10 (64-Bit)", "solaris10-64"));
                menuSolaris.DropDownItems.Add(AddOsItem("Solaris 10 (32-Bit)", "solaris10"));
                menuSolaris.DropDownItems.Add(AddOsItem("Solaris 9 (unsupported)", "solaris9"));
                menuSolaris.DropDownItems.Add(AddOsItem("Solaris 8 (unsupported)", "solaris8"));
                menuOther.DropDownItems.Add(menuSolaris);

                osContextMenu.Items.AddRange(new ToolStripItem[] {
                    menuWindows,
                    menuLinux,
                    menuMac,
                    menuEsx,
                    menuOther
                });

                buttonOSMenu.Click += (s, e) => {
                    osContextMenu.Show(buttonOSMenu, new Point(0, buttonOSMenu.Height));
                };
            }
        }

        private void ButtonIsoBrowse_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Select ISO Image File",
                Filter = "ISO Image Files (*.iso)|*.iso|All Files (*.*)|*.*",
                CheckFileExists = true
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                textBoxIsoPath.Text = dialog.FileName;
            }
        }

        private void RadioButtonIsoController_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb.Checked)
            {
                if (rb == radioButtonIsoIDE && numericUpDownIDECDCount != null && numericUpDownIDECDCount.Value < 1)
                    numericUpDownIDECDCount.Value = 1;
                else if (rb == radioButtonIsoSCSI && numericUpDownSCSICDCount != null && numericUpDownSCSICDCount.Value < 1)
                    numericUpDownSCSICDCount.Value = 1;
                else if (rb == radioButtonIsoSATA && numericUpDownSATACDCount != null && numericUpDownSATACDCount.Value < 1)
                    numericUpDownSATACDCount.Value = 1;
            }
        }

        private void SoundCardCheckbox_Click(object sender, EventArgs e)
        {
            if (sender is CheckBox cb && !cb.Checked)
            {
                int checkedCount = 0;
                if (checkBoxSB16 != null && checkBoxSB16.Checked) checkedCount++;
                if (checkBoxES1371 != null && checkBoxES1371.Checked) checkedCount++;
                if (checkBoxHDAudio != null && checkBoxHDAudio.Checked) checkedCount++;

                bool isRadioChecked = false;
                if (cb == checkBoxSB16 && radioButtonSB16 != null) isRadioChecked = radioButtonSB16.Checked;
                else if (cb == checkBoxES1371 && radioButtonES1371 != null) isRadioChecked = radioButtonES1371.Checked;
                else if (cb == checkBoxHDAudio && radioButtonHDAudio != null) isRadioChecked = radioButtonHDAudio.Checked;

                if (checkedCount == 0 || isRadioChecked)
                {
                    isUpdatingSoundCheckboxes = true;
                    cb.Checked = true;
                    isUpdatingSoundCheckboxes = false;
                }
            }
        }

        private void SoundCardCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdatingSoundCheckboxes) return;

            int checkedCount = 0;
            if (checkBoxSB16 != null && checkBoxSB16.Checked) checkedCount++;
            if (checkBoxES1371 != null && checkBoxES1371.Checked) checkedCount++;
            if (checkBoxHDAudio != null && checkBoxHDAudio.Checked) checkedCount++;

            if (sender is CheckBox cb)
            {
                bool isRadioChecked = false;
                if (cb == checkBoxSB16 && radioButtonSB16 != null) isRadioChecked = radioButtonSB16.Checked;
                else if (cb == checkBoxES1371 && radioButtonES1371 != null) isRadioChecked = radioButtonES1371.Checked;
                else if (cb == checkBoxHDAudio && radioButtonHDAudio != null) isRadioChecked = radioButtonHDAudio.Checked;

                if (!cb.Checked && (checkedCount == 0 || isRadioChecked))
                {
                    isUpdatingSoundCheckboxes = true;
                    cb.Checked = true;
                    isUpdatingSoundCheckboxes = false;
                    return;
                }
            }

            if (checkBoxSB16 != null) radioButtonSB16.Enabled = checkBoxSB16.Checked;
            if (checkBoxES1371 != null) radioButtonES1371.Enabled = checkBoxES1371.Checked;
            if (checkBoxHDAudio != null) radioButtonHDAudio.Enabled = checkBoxHDAudio.Checked;
        }

        private void SCSICheckbox_Click(object sender, EventArgs e)
        {
            if (sender is CheckBox cb && !cb.Checked)
            {
                int checkedCount = 0;
                if (checkBoxBusLogic != null && checkBoxBusLogic.Checked) checkedCount++;
                if (checkBoxLSILogic != null && checkBoxLSILogic.Checked) checkedCount++;
                if (checkBoxLSILogicSAS != null && checkBoxLSILogicSAS.Checked) checkedCount++;
                if (checkBoxParavirtualized != null && checkBoxParavirtualized.Checked) checkedCount++;

                bool isRadioChecked = false;
                if (cb == checkBoxBusLogic && radioButtonBusLogic != null) isRadioChecked = radioButtonBusLogic.Checked;
                else if (cb == checkBoxLSILogic && radioButtonLSILogic != null) isRadioChecked = radioButtonLSILogic.Checked;
                else if (cb == checkBoxLSILogicSAS && radioButtonLSILogicSAS != null) isRadioChecked = radioButtonLSILogicSAS.Checked;
                else if (cb == checkBoxParavirtualized && radioButtonParavirtualized != null) isRadioChecked = radioButtonParavirtualized.Checked;

                if (checkedCount == 0 || isRadioChecked)
                {
                    isUpdatingSCSICheckboxes = true;
                    cb.Checked = true;
                    isUpdatingSCSICheckboxes = false;
                }
            }
        }

        private void SCSICheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdatingSCSICheckboxes) return;

            int checkedCount = 0;
            if (checkBoxBusLogic != null && checkBoxBusLogic.Checked) checkedCount++;
            if (checkBoxLSILogic != null && checkBoxLSILogic.Checked) checkedCount++;
            if (checkBoxLSILogicSAS != null && checkBoxLSILogicSAS.Checked) checkedCount++;
            if (checkBoxParavirtualized != null && checkBoxParavirtualized.Checked) checkedCount++;

            if (sender is CheckBox cb)
            {
                bool isRadioChecked = false;
                if (cb == checkBoxBusLogic && radioButtonBusLogic != null) isRadioChecked = radioButtonBusLogic.Checked;
                else if (cb == checkBoxLSILogic && radioButtonLSILogic != null) isRadioChecked = radioButtonLSILogic.Checked;
                else if (cb == checkBoxLSILogicSAS && radioButtonLSILogicSAS != null) isRadioChecked = radioButtonLSILogicSAS.Checked;
                else if (cb == checkBoxParavirtualized && radioButtonParavirtualized != null) isRadioChecked = radioButtonParavirtualized.Checked;

                if (!cb.Checked && (checkedCount == 0 || isRadioChecked))
                {
                    isUpdatingSCSICheckboxes = true;
                    cb.Checked = true;
                    isUpdatingSCSICheckboxes = false;
                    return;
                }
            }

            if (checkBoxBusLogic != null) radioButtonBusLogic.Enabled = checkBoxBusLogic.Checked;
            if (checkBoxLSILogic != null) radioButtonLSILogic.Enabled = checkBoxLSILogic.Checked;
            if (checkBoxLSILogicSAS != null) radioButtonLSILogicSAS.Enabled = checkBoxLSILogicSAS.Checked;
            if (checkBoxParavirtualized != null) radioButtonParavirtualized.Enabled = checkBoxParavirtualized.Checked;
        }

        private void HardDriveCheckbox_Click(object sender, EventArgs e)
        {
            if (sender is CheckBox cb && !cb.Checked)
            {
                int checkedCount = 0;
                if (checkBoxIDE != null && checkBoxIDE.Checked) checkedCount++;
                if (checkBoxSCSI != null && checkBoxSCSI.Checked) checkedCount++;
                if (checkBoxSATA != null && checkBoxSATA.Checked) checkedCount++;
                if (checkBoxNVMe != null && checkBoxNVMe.Checked) checkedCount++;

                bool isRadioChecked = false;
                if (cb == checkBoxIDE && radioButtonIDE != null) isRadioChecked = radioButtonIDE.Checked;
                else if (cb == checkBoxSCSI && radioButtonSCSI != null) isRadioChecked = radioButtonSCSI.Checked;
                else if (cb == checkBoxSATA && radioButtonSATA != null) isRadioChecked = radioButtonSATA.Checked;
                else if (cb == checkBoxNVMe && radioButtonNVMe != null) isRadioChecked = radioButtonNVMe.Checked;

                if (checkedCount == 0 || isRadioChecked)
                {
                    isUpdatingHardDriveCheckboxes = true;
                    cb.Checked = true;
                    isUpdatingHardDriveCheckboxes = false;
                }
            }
        }

        private void HardDriveCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdatingHardDriveCheckboxes) return;

            int checkedCount = 0;
            if (checkBoxIDE != null && checkBoxIDE.Checked) checkedCount++;
            if (checkBoxSCSI != null && checkBoxSCSI.Checked) checkedCount++;
            if (checkBoxSATA != null && checkBoxSATA.Checked) checkedCount++;
            if (checkBoxNVMe != null && checkBoxNVMe.Checked) checkedCount++;

            if (sender is CheckBox cb)
            {
                bool isRadioChecked = false;
                if (cb == checkBoxIDE && radioButtonIDE != null) isRadioChecked = radioButtonIDE.Checked;
                else if (cb == checkBoxSCSI && radioButtonSCSI != null) isRadioChecked = radioButtonSCSI.Checked;
                else if (cb == checkBoxSATA && radioButtonSATA != null) isRadioChecked = radioButtonSATA.Checked;
                else if (cb == checkBoxNVMe && radioButtonNVMe != null) isRadioChecked = radioButtonNVMe.Checked;

                if (!cb.Checked && (checkedCount == 0 || isRadioChecked))
                {
                    isUpdatingHardDriveCheckboxes = true;
                    cb.Checked = true;
                    isUpdatingHardDriveCheckboxes = false;
                    return;
                }
            }

            if (checkBoxIDE != null) radioButtonIDE.Enabled = checkBoxIDE.Checked;
            if (checkBoxSCSI != null) radioButtonSCSI.Enabled = checkBoxSCSI.Checked;
            if (checkBoxSATA != null) radioButtonSATA.Enabled = checkBoxSATA.Checked;
            if (checkBoxNVMe != null) radioButtonNVMe.Enabled = checkBoxNVMe.Checked;
        }

        private void NetworkCheckbox_Click(object sender, EventArgs e)
        {
            if (sender is CheckBox cb && !cb.Checked)
            {
                int checkedCount = 0;
                if (checkBoxE1000PCI != null && checkBoxE1000PCI.Checked) checkedCount++;
                if (checkBoxE1000PCIe != null && checkBoxE1000PCIe.Checked) checkedCount++;
                if (checkBoxPCNet32 != null && checkBoxPCNet32.Checked) checkedCount++;
                if (checkBoxradioButtonVMXNET3 != null && checkBoxradioButtonVMXNET3.Checked) checkedCount++;

                bool isRadioChecked = false;
                if (cb == checkBoxE1000PCI && radioButtonE1000PCI != null) isRadioChecked = radioButtonE1000PCI.Checked;
                else if (cb == checkBoxE1000PCIe && radioButtonE1000PCIe != null) isRadioChecked = radioButtonE1000PCIe.Checked;
                else if (cb == checkBoxPCNet32 && radioButtonPCNet32 != null) isRadioChecked = radioButtonPCNet32.Checked;
                else if (cb == checkBoxradioButtonVMXNET3 && radioButtonradioButtonVMXNET3 != null) isRadioChecked = radioButtonradioButtonVMXNET3.Checked;

                if (checkedCount == 0 || isRadioChecked)
                {
                    isUpdatingNetworkCheckboxes = true;
                    cb.Checked = true;
                    isUpdatingNetworkCheckboxes = false;
                }
            }
        }

        private void NetworkCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdatingNetworkCheckboxes) return;

            int checkedCount = 0;
            if (checkBoxE1000PCI != null && checkBoxE1000PCI.Checked) checkedCount++;
            if (checkBoxE1000PCIe != null && checkBoxE1000PCIe.Checked) checkedCount++;
            if (checkBoxPCNet32 != null && checkBoxPCNet32.Checked) checkedCount++;
            if (checkBoxradioButtonVMXNET3 != null && checkBoxradioButtonVMXNET3.Checked) checkedCount++;

            if (sender is CheckBox cb)
            {
                bool isRadioChecked = false;
                if (cb == checkBoxE1000PCI && radioButtonE1000PCI != null) isRadioChecked = radioButtonE1000PCI.Checked;
                else if (cb == checkBoxE1000PCIe && radioButtonE1000PCIe != null) isRadioChecked = radioButtonE1000PCIe.Checked;
                else if (cb == checkBoxPCNet32 && radioButtonPCNet32 != null) isRadioChecked = radioButtonPCNet32.Checked;
                else if (cb == checkBoxradioButtonVMXNET3 && radioButtonradioButtonVMXNET3 != null) isRadioChecked = radioButtonradioButtonVMXNET3.Checked;

                if (!cb.Checked && (checkedCount == 0 || isRadioChecked))
                {
                    isUpdatingNetworkCheckboxes = true;
                    cb.Checked = true;
                    isUpdatingNetworkCheckboxes = false;
                    return;
                }
            }

            if (checkBoxE1000PCI != null) radioButtonE1000PCI.Enabled = checkBoxE1000PCI.Checked;
            if (checkBoxE1000PCIe != null) radioButtonE1000PCIe.Enabled = checkBoxE1000PCIe.Checked;
            if (checkBoxPCNet32 != null) radioButtonPCNet32.Enabled = checkBoxPCNet32.Checked;
            if (checkBoxradioButtonVMXNET3 != null) radioButtonradioButtonVMXNET3.Enabled = checkBoxradioButtonVMXNET3.Checked;
        }

        private int SliderToMB(int sliderValue)
        {
            double t = sliderValue / 1000.0;
            double mb = 4 * Math.Pow(2, t * 15);
            int rounded = (int)Math.Round(mb / 4.0) * 4;
            if (rounded < 4) rounded = 4;
            if (rounded > 131072) rounded = 131072;
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

        private void InitializeMemoryControls()
        {
            ConfigureMemoryPair(trackBarMinMemory, numericUpDownMinMemory, 512);
            ConfigureMemoryPair(trackBarDefaultMemory, numericUpDownDefaultMemory, 2048);
            ConfigureMemoryPair(trackBarMaxMemory, numericUpDownMaxMemory, 4096);
        }

        private void ConfigureMemoryPair(CustomTrackBar tb, NumericUpDown nud, int defaultMb)
        {
            if (tb != null && nud != null)
            {
                tb.Minimum = 0;
                tb.Maximum = 1000;
                nud.Minimum = 4;
                nud.Maximum = 131072;
                nud.Increment = 4;

                nud.Value = defaultMb;
                tb.Value = MBToSlider(defaultMb);

                tb.Scroll += (s, e) => {
                    int mb = SliderToMB(tb.Value);
                    if (nud.Value != mb) nud.Value = mb;
                    groupBoxMemory.Invalidate();
                };

                nud.ValueChanged += (s, e) => {
                    int val = ((int)nud.Value + 2) / 4 * 4;
                    if (val < 4) val = 4;
                    if (val > 131072) val = 131072;
                    if (nud.Value != val)
                    {
                        nud.Value = val;
                        return;
                    }
                    int sliderVal = MBToSlider(val);
                    if (tb.Value != sliderVal) tb.Value = sliderVal;
                    groupBoxMemory.Invalidate();
                };
            }
        }

        private void GroupBoxMemory_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Font smallFont = new Font(this.Font.FontFamily, 7))
            {
                DrawTicksForTrackBar(g, trackBarMinMemory, smallFont);
                DrawTicksForTrackBar(g, trackBarDefaultMemory, smallFont);
                DrawTicksForTrackBar(g, trackBarMaxMemory, smallFont);
            }
        }

        private void DrawTicksForTrackBar(Graphics g, CustomTrackBar tb, Font font)
        {
            if (tb == null) return;
            int left = tb.Left + 14;
            int right = tb.Right - 14;
            int y = tb.Top + tb.Height - 3;

            for (int i = 0; i < memoryStops.Length; i++)
            {
                int mb = memoryStops[i];
                double t = Math.Log(mb / 4.0, 2) / 15.0;
                int sliderPos = (int)(t * 1000);
                int x = left + (int)((right - left) * (sliderPos / 1000.0));

                g.DrawLine(Pens.Black, x, y, x, y + 5);
                string label = mb < 1024 ? $"{mb} MB" : $"{mb / 1024} GB";
                SizeF textSize = g.MeasureString(label, font);
                g.DrawString(label, font, Brushes.Black, x - textSize.Width / 2, y + 6);
            }
        }

        private void GroupBoxMemory_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsTickHovered(e.Location, trackBarMinMemory) ||
                IsTickHovered(e.Location, trackBarDefaultMemory) ||
                IsTickHovered(e.Location, trackBarMaxMemory))
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        private bool IsTickHovered(Point pt, CustomTrackBar tb)
        {
            if (tb == null) return false;
            int left = tb.Left + 14;
            int right = tb.Right - 14;
            int y = tb.Top + tb.Height - 3;

            for (int i = 0; i < memoryStops.Length; i++)
            {
                int mb = memoryStops[i];
                double t = Math.Log(mb / 4.0, 2) / 15.0;
                int sliderPos = (int)(t * 1000);
                int x = left + (int)((right - left) * (sliderPos / 1000.0));
                Rectangle hitbox = new Rectangle(x - 10, y, 20, 25);
                if (hitbox.Contains(pt)) return true;
            }
            return false;
        }

        private void GroupBoxMemory_MouseDown(object sender, MouseEventArgs e)
        {
            CheckAndSetTickClick(e.Location, trackBarMinMemory, numericUpDownMinMemory);
            CheckAndSetTickClick(e.Location, trackBarDefaultMemory, numericUpDownDefaultMemory);
            CheckAndSetTickClick(e.Location, trackBarMaxMemory, numericUpDownMaxMemory);
        }

        private bool CheckAndSetTickClick(Point pt, CustomTrackBar tb, NumericUpDown nud)
        {
            if (tb == null || nud == null) return false;
            int left = tb.Left + 14;
            int right = tb.Right - 14;
            int y = tb.Top + tb.Height - 3;

            for (int i = 0; i < memoryStops.Length; i++)
            {
                int mb = memoryStops[i];
                double t = Math.Log(mb / 4.0, 2) / 15.0;
                int sliderPos = (int)(t * 1000);
                int x = left + (int)((right - left) * (sliderPos / 1000.0));
                Rectangle hitbox = new Rectangle(x - 10, y, 20, 25);
                if (hitbox.Contains(pt))
                {
                    tb.Value = MBToSlider(mb);
                    nud.Value = mb;
                    groupBoxMemory.Invalidate();
                    return true;
                }
            }
            return false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            var group = Controls.Find("groupBoxImageAssets", true).FirstOrDefault() as GroupBox;
            var listBox = Controls.Find("listBoxImageAssets", true).FirstOrDefault() as ListBox;
            var panel = Controls.Find("panelPreviewContainer", true).FirstOrDefault() as Panel;
            var btn = Controls.Find("buttonSelectImageAsset", true).FirstOrDefault() as Button;
            var lblName = Controls.Find("labelSelectedImageName", true).FirstOrDefault() as Label;
            var lblSize = Controls.Find("labelSelectedImageSize", true).FirstOrDefault() as Label;

            if (group != null && listBox != null && panel != null && btn != null)
            {
                int margin = 15;
                int topMargin = listBox.Top;
                int buttonHeight = btn.Height;

                int calculatedListHeight = group.Height - topMargin - buttonHeight - (margin * 2);
                if (calculatedListHeight > 50)
                {
                    listBox.Height = calculatedListHeight;
                    btn.Top = listBox.Bottom + 6;
                }

                int availableWidth = group.Width - (margin * 3);
                int calculatedListWidth = Math.Max(170, availableWidth * 38 / 100);

                listBox.Width = calculatedListWidth;
                btn.Width = calculatedListWidth;

                int rightSideLeft = listBox.Right + margin;
                panel.Left = rightSideLeft;
                if (lblName != null) lblName.Left = rightSideLeft;
                if (lblSize != null) lblSize.Left = rightSideLeft;

                int calculatedPanelWidth = group.Width - rightSideLeft - margin;
                int calculatedPanelHeight = group.Height - panel.Top - margin;

                if (calculatedPanelWidth > 100 && calculatedPanelHeight > 100)
                {
                    panel.Width = calculatedPanelWidth;
                    panel.Height = calculatedPanelHeight;
                }
            }

            var groupBoxProc = Controls.Find("groupBoxProcessors", true).FirstOrDefault() as GroupBox;
            var groupBoxFirm = Controls.Find("groupBoxFirmware", true).FirstOrDefault() as GroupBox;
            var groupBoxMem = Controls.Find("groupBoxMemory", true).FirstOrDefault() as GroupBox;

            if (groupBoxProc != null && groupBoxFirm != null && groupBoxMem != null)
            {
                int margin = 12;
                groupBoxFirm.Width = 220;
                int procLeft = groupBoxFirm.Right + margin;
                groupBoxProc.Left = procLeft;

                int targetRight = groupBoxMem.Right;
                int newProcWidth = targetRight - groupBoxProc.Left;

                if (newProcWidth > 250) groupBoxProc.Width = newProcWidth;

                var comboProc = groupBoxProc.Controls.Find("comboBoxNumberOfProcessors", true).FirstOrDefault() as ComboBox;
                var comboCores = groupBoxProc.Controls.Find("comboBoxNumberOfCoresPerProcessor", true).FirstOrDefault() as ComboBox;

                if (comboProc != null && comboCores != null)
                {
                    int internalWidth = groupBoxProc.Width - comboProc.Left - margin;
                    if (internalWidth > 100)
                    {
                        comboProc.Width = internalWidth;
                        comboCores.Width = internalWidth;
                    }
                }
            }
        }

        private void PopulateProcessorOptions()
        {
            if (comboBoxNumberOfProcessors != null)
            {
                comboBoxNumberOfProcessors.Items.Clear();
                comboBoxNumberOfProcessors.Items.AddRange(new object[] { 1, 2, 4, 8, 16, 32 });
                if (comboBoxNumberOfProcessors.Items.Count > 0) comboBoxNumberOfProcessors.SelectedIndex = 0;
            }

            if (comboBoxNumberOfCoresPerProcessor != null)
            {
                comboBoxNumberOfCoresPerProcessor.Items.Clear();
                comboBoxNumberOfCoresPerProcessor.Items.AddRange(new object[] { 1, 2, 3, 4, 6, 8, 12, 16, 24, 32 });
                if (comboBoxNumberOfCoresPerProcessor.Items.Count > 0) comboBoxNumberOfCoresPerProcessor.SelectedIndex = 0;
            }
        }

        private void NumericOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (!char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void ProcessorSelectionChanged(object sender, EventArgs e) => UpdateTotalCoresCalculation();

        private void UpdateTotalCoresCalculation()
        {
            int processors = 1;
            int coresPerProcessor = 1;

            if (comboBoxNumberOfProcessors != null) int.TryParse(comboBoxNumberOfProcessors.Text, out processors);
            if (comboBoxNumberOfCoresPerProcessor != null) int.TryParse(comboBoxNumberOfCoresPerProcessor.Text, out coresPerProcessor);

            int totalCores = processors * coresPerProcessor;
            if (labelTotalCoresNumber != null) labelTotalCoresNumber.Text = totalCores.ToString();
        }

        private static Bitmap CreateScaledImageWithAlpha(Image source, Size size)
        {
            var scaled = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(scaled))
            {
                g.Clear(Color.Transparent);
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(source, new Rectangle(0, 0, size.Width, size.Height));
            }
            return scaled;
        }

        private void PanelSelectedImage_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            int cellSize = 8;
            using var lightBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
            using var darkBrush = new SolidBrush(Color.FromArgb(220, 220, 220));

            for (int y = 0; y < pictureBoxSelectedImage.Height; y += cellSize)
            {
                for (int x = 0; x < pictureBoxSelectedImage.Width; x += cellSize)
                {
                    bool isLight = ((x / cellSize) + (y / cellSize)) % 2 == 0;
                    g.FillRectangle(isLight ? lightBrush : darkBrush, x, y, cellSize, cellSize);
                }
            }

            if (pictureBoxSelectedImage.Tag is Image img)
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(img, 0, 0, img.Width, img.Height);
            }
        }

        private void PopulateImageAssetList()
        {
            listBoxImageAssets.Items.Clear();
            foreach (var asset in imageAssets) listBoxImageAssets.Items.Add($"{asset.Name} ({asset.Width}×{asset.Height})");
            if (listBoxImageAssets.Items.Count > 0) listBoxImageAssets.SelectedIndex = 0;
        }

        private void PopulateInstalledFonts()
        {
            var installedFonts = new InstalledFontCollection();
            var fontNames = installedFonts.Families.Select(f => f.Name).OrderBy(name => name, StringComparer.CurrentCultureIgnoreCase).ToArray();
            if (fontNames.Length == 0) return;

            comboBoxTextFont.Items.Clear();
            comboBoxTextFont.Items.AddRange(fontNames);

            var preferred = fontNames.FirstOrDefault(name => name.Equals("Arial", StringComparison.OrdinalIgnoreCase))
                ?? fontNames.FirstOrDefault(name => name.Equals("Segoe UI", StringComparison.OrdinalIgnoreCase))
                ?? fontNames[0];

            comboBoxTextFont.SelectedItem = preferred;
        }

        private void ApplySelectedAssetPreview(ImageAssetDefinition asset)
        {
            if (selectedAssetImages.TryGetValue(asset.Name, out var selectedImage)) pictureBoxSelectedImage.Tag = selectedImage;
            else pictureBoxSelectedImage.Tag = null;

            labelSelectedImageName.Text = asset.Name;
            labelSelectedImageSize.Text = $"{asset.Width} × {asset.Height}px";
            labelSelectedImageName.Visible = true;
            labelSelectedImageSize.Visible = true;

            pictureBoxSelectedImage.Width = asset.Width + 2;
            pictureBoxSelectedImage.Height = asset.Height + 2;
            pictureBoxSelectedImage.Invalidate();

            if (panelPreviewContainer != null) panelPreviewContainer.AutoScrollPosition = new Point(0, 0);
        }

        private void ClearSelectedAssetPreview()
        {
            pictureBoxSelectedImage.Tag = null;
            pictureBoxSelectedImage.Width = 66;
            pictureBoxSelectedImage.Height = 66;
            pictureBoxSelectedImage.Invalidate();
            labelSelectedImageName.Text = string.Empty;
            labelSelectedImageSize.Text = string.Empty;
        }

        private void ListBoxImageAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxImageAssets.SelectedIndex < 0) return;
            var asset = imageAssets[listBoxImageAssets.SelectedIndex];
            ApplySelectedAssetPreview(asset);
        }

        private void ButtonSelectImageAsset_Click(object sender, EventArgs e)
        {
            if (listBoxImageAssets.SelectedIndex < 0) return;
            var asset = imageAssets[listBoxImageAssets.SelectedIndex];
            using var dialog = new OpenFileDialog
            {
                Title = $"Select {asset.Name}",
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.ico",
                CheckFileExists = true
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                using var selected = Image.FromFile(dialog.FileName);
                var actualSize = CreateScaledImageWithAlpha(selected, new Size(asset.Width, asset.Height));
                if (selectedAssetImages.TryGetValue(asset.Name, out var oldImg)) oldImg.Dispose();
                selectedAssetImages[asset.Name] = actualSize;
                ApplySelectedAssetPreview(asset);
            }
        }

        private void ButtonTextColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = buttonTextColor.BackColor;
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
            {
                buttonTextColor.BackColor = colorDialog1.Color;
                buttonTextColor.UseVisualStyleBackColor = false;
                buttonTextColor.Refresh();
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetFormState();
            currentFilePath = string.Empty;
            isTemplateFile = false;
            isModified = false;
            UpdateFileCommandLabels();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Open project or template",
                Filter = "Virtual Machine Project (*.vmp)|*.vmp|Virtual Machine Template (*.vmt)|*.vmt"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                bool isTemplate = string.Equals(Path.GetExtension(filePath), TemplateExtension, StringComparison.OrdinalIgnoreCase);

                LoadProjectFromFile(filePath);

                if (isTemplate)
                {
                    currentFilePath = string.Empty;
                    isTemplateFile = false;
                }
                else
                {
                    currentFilePath = filePath;
                    isTemplateFile = false;
                }

                isModified = false;
                UpdateFileCommandLabels();
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAsProject();
                return;
            }
            SaveCurrentProject(currentFilePath, isTemplateFile);
            isModified = false;
            UpdateFileCommandLabels();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e) => SaveAsProject();
        private void SaveAsTemplateToolStripMenuItem_Click(object sender, EventArgs e) => SaveAsTemplate();

        private void SaveAsProject()
        {
            using var dialog = new SaveFileDialog
            {
                Title = "Save VM project",
                Filter = "Virtual Machine Project (*.vmp)|*.vmp",
                DefaultExt = ProjectExtension.TrimStart('.')
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                currentFilePath = dialog.FileName;
                isTemplateFile = false;
                SaveCurrentProject(currentFilePath, false);
                isModified = false;
                UpdateFileCommandLabels();
            }
        }

        private void SaveAsTemplate()
        {
            using var dialog = new SaveFileDialog
            {
                Title = "Save VM template",
                Filter = "Virtual Machine Template (*.vmt)|*.vmt",
                DefaultExt = TemplateExtension.TrimStart('.')
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveCurrentProject(dialog.FileName, true);
                currentFilePath = string.Empty;
                isTemplateFile = false;
                isModified = false;
                UpdateFileCommandLabels();
            }
        }

        private void LoadProjectFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            try
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<ProjectData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (data == null) return;

                ResetFormState();
                textBoxVMName.Text = data.VmName ?? string.Empty;
                textBoxVMLocation.Text = data.VmLocation ?? string.Empty;

                bool iconLoaded = false;

                if (data.AssetImages != null && data.AssetImages.TryGetValue(ProjectDefaultIconKey, out var defaultIconBase64))
                {
                    try
                    {
                        byte[] bytes = Convert.FromBase64String(defaultIconBase64);
                        originalIconBytes = bytes;
                        using var ms = new MemoryStream(bytes);
                        try
                        {
                            using var loadedIcon = new Icon(ms);
                            if (pictureBoxIcon.Image != null) pictureBoxIcon.Image.Dispose();
                            pictureBoxIcon.Image = loadedIcon.ToBitmap();
                            iconLoaded = true;
                        }
                        catch
                        {
                            ms.Position = 0;
                            using var loadedImg = Image.FromStream(ms);
                            if (pictureBoxIcon.Image != null) pictureBoxIcon.Image.Dispose();
                            pictureBoxIcon.Image = new Bitmap(loadedImg);
                            iconLoaded = true;
                        }
                        pictureBoxIcon.Tag = null;
                    }
                    catch { }
                }

                if (!iconLoaded && !string.IsNullOrWhiteSpace(data.IconPath))
                {
                    try
                    {
                        pictureBoxIcon.Tag = data.IconPath;
                        if (File.Exists(data.IconPath) && Path.GetExtension(data.IconPath).Equals(".ico", StringComparison.OrdinalIgnoreCase))
                        {
                            originalIconBytes = File.ReadAllBytes(data.IconPath);
                        }

                        StringBuilder sbPath = new StringBuilder(data.IconPath);
                        int defIndex = 0;
                        IntPtr hIcon = ExtractAssociatedIcon(IntPtr.Zero, sbPath, ref defIndex);
                        if (hIcon != IntPtr.Zero)
                        {
                            using (var icon = Icon.FromHandle(hIcon))
                            {
                                if (pictureBoxIcon.Image != null) pictureBoxIcon.Image.Dispose();
                                pictureBoxIcon.Image = icon.ToBitmap();
                            }
                            DestroyIcon(hIcon);
                            iconLoaded = true;
                        }
                        else if (File.Exists(data.IconPath))
                        {
                            pictureBoxIcon.ImageLocation = data.IconPath;
                            pictureBoxIcon.Load();
                            iconLoaded = true;
                        }
                    }
                    catch { }
                }

                if (!iconLoaded)
                {
                    if (pictureBoxIcon.Image != null) { pictureBoxIcon.Image.Dispose(); pictureBoxIcon.Image = null; }
                    string fullIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultIconRelativePath);
                    if (File.Exists(fullIconPath))
                    {
                        try
                        {
                            originalIconBytes = File.ReadAllBytes(fullIconPath);
                            using (var icon = new Icon(fullIconPath))
                            {
                                pictureBoxIcon.Image = icon.ToBitmap();
                            }
                        }
                        catch { }
                    }
                    pictureBoxIcon.Tag = null;
                }

                if (!string.IsNullOrWhiteSpace(data.TextColorHex))
                {
                    try
                    {
                        buttonTextColor.BackColor = ColorTranslator.FromHtml(data.TextColorHex);
                        buttonTextColor.UseVisualStyleBackColor = false;
                    }
                    catch { }
                }

                if (!string.IsNullOrWhiteSpace(data.TextFontName) && comboBoxTextFont.Items.Contains(data.TextFontName))
                    comboBoxTextFont.SelectedItem = data.TextFontName;

                if (!string.IsNullOrWhiteSpace(data.SelectedLanguage) && comboBoxLanguage.Items.Contains(data.SelectedLanguage))
                    comboBoxLanguage.SelectedItem = data.SelectedLanguage;

                if (data.HardwareVersion > 0) numericUpDownHardwareVersion.Value = data.HardwareVersion;
                if (data.RecommendedMinMemory > 0) numericUpDownMinMemory.Value = data.RecommendedMinMemory;
                if (data.RecommendedDefaultMemory > 0) numericUpDownDefaultMemory.Value = data.RecommendedDefaultMemory;
                if (data.RecommendedMaxMemory > 0) numericUpDownMaxMemory.Value = data.RecommendedMaxMemory;

                textBoxOSDisplayName.Text = data.OSDisplayName ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(data.GuestOSID))
                {
                    textBoxGuestOS.Text = data.GuestOSID;
                }

                textBoxIsoPath.Text = data.IsoImagePath ?? string.Empty;

                if (string.Equals(data.IsoControllerType, "SCSI", StringComparison.OrdinalIgnoreCase)) radioButtonIsoSCSI.Checked = true;
                else if (string.Equals(data.IsoControllerType, "SATA", StringComparison.OrdinalIgnoreCase)) radioButtonIsoSATA.Checked = true;
                else radioButtonIsoIDE.Checked = true;

                if (string.Equals(data.FirmwareType, "UEFI", StringComparison.OrdinalIgnoreCase)) radioButtonUEFI.Checked = true;
                else radioButtonBIOS.Checked = true;
                checkBoxFirmwareForced.Checked = data.FirmwareForced;

                if (data.NumberOfProcessors > 0) comboBoxNumberOfProcessors.Text = data.NumberOfProcessors.ToString();
                if (data.CoresPerProcessor > 0) comboBoxNumberOfCoresPerProcessor.Text = data.CoresPerProcessor.ToString();

                isUpdatingSoundCheckboxes = true;
                checkBoxSB16.Checked = data.SoundCardSB16Checked;
                checkBoxES1371.Checked = data.SoundCardES1371Checked;
                checkBoxHDAudio.Checked = data.SoundCardHDAudioChecked;
                isUpdatingSoundCheckboxes = false;

                radioButtonSB16.Checked = data.SoundCardSB16Selected;
                radioButtonSB16.Enabled = checkBoxSB16.Checked;
                radioButtonES1371.Checked = data.SoundCardES1371Selected;
                radioButtonES1371.Enabled = checkBoxES1371.Checked;
                radioButtonHDAudio.Checked = data.SoundCardHDAudioSelected;
                radioButtonHDAudio.Enabled = checkBoxHDAudio.Checked;

                isUpdatingSCSICheckboxes = true;
                checkBoxBusLogic.Checked = data.SCSIBusLogicChecked;
                checkBoxLSILogic.Checked = data.SCSILSILogicChecked;
                checkBoxLSILogicSAS.Checked = data.SCSILSILogicSASChecked;
                checkBoxParavirtualized.Checked = data.SCSIParavirtualizedChecked;
                isUpdatingSCSICheckboxes = false;

                radioButtonBusLogic.Checked = data.SCSIBusLogicSelected;
                radioButtonBusLogic.Enabled = checkBoxBusLogic.Checked;
                radioButtonLSILogic.Checked = data.SCSILSILogicSelected;
                radioButtonLSILogic.Enabled = checkBoxLSILogic.Checked;
                radioButtonLSILogicSAS.Checked = data.SCSILSILogicSASSelected;
                radioButtonLSILogicSAS.Enabled = checkBoxLSILogicSAS.Checked;
                radioButtonParavirtualized.Checked = data.SCSIParavirtualizedSelected;
                radioButtonParavirtualized.Enabled = checkBoxParavirtualized.Checked;

                isUpdatingHardDriveCheckboxes = true;
                checkBoxIDE.Checked = data.HardDriveIDEChecked;
                checkBoxSCSI.Checked = data.HardDriveSCSIChecked;
                checkBoxSATA.Checked = data.HardDriveSATAChecked;
                checkBoxNVMe.Checked = data.HardDriveNVMeChecked;
                isUpdatingHardDriveCheckboxes = false;

                radioButtonIDE.Checked = data.HardDriveIDESelected;
                radioButtonIDE.Enabled = checkBoxIDE.Checked;
                radioButtonSCSI.Checked = data.HardDriveSCSISelected;
                radioButtonSCSI.Enabled = checkBoxSCSI.Checked;
                radioButtonSATA.Checked = data.HardDriveSATASelected;
                radioButtonSATA.Enabled = checkBoxSATA.Checked;
                radioButtonNVMe.Checked = data.HardDriveNVMeSelected;
                radioButtonNVMe.Enabled = checkBoxNVMe.Checked;

                isUpdatingNetworkCheckboxes = true;
                checkBoxE1000PCI.Checked = data.NetworkE1000PCIChecked;
                checkBoxE1000PCIe.Checked = data.NetworkE1000PCIeChecked;
                checkBoxPCNet32.Checked = data.NetworkPCNet32Checked;
                checkBoxradioButtonVMXNET3.Checked = data.NetworkVMXNET3Checked;
                isUpdatingNetworkCheckboxes = false;

                radioButtonE1000PCI.Checked = data.NetworkE1000PCISelected;
                radioButtonE1000PCI.Enabled = checkBoxE1000PCI.Checked;
                radioButtonE1000PCIe.Checked = data.NetworkE1000PCIeSelected;
                radioButtonE1000PCIe.Enabled = checkBoxE1000PCIe.Checked;
                radioButtonPCNet32.Checked = data.NetworkPCNet32Selected;
                radioButtonPCNet32.Enabled = checkBoxPCNet32.Checked;
                radioButtonradioButtonVMXNET3.Checked = data.NetworkVMXNET3Selected;
                radioButtonradioButtonVMXNET3.Enabled = checkBoxradioButtonVMXNET3.Checked;

                if (data.HardDriveCapacityGB > 0) numericUpDownHardDriveCapacity.Value = data.HardDriveCapacityGB;

                numericUpDownFloppyCount.Value = data.FloppyDriveCount;
                numericUpDownIDECDCount.Value = data.IDECDCount;
                numericUpDownSCSICDCount.Value = data.SCSICDCount;
                numericUpDownSATACDCount.Value = data.SATACDCount;

                if (data.AssetImages != null)
                {
                    foreach (var kvp in data.AssetImages)
                    {
                        if (string.Equals(kvp.Key, ProjectDefaultIconKey, StringComparison.OrdinalIgnoreCase)) continue;

                        try
                        {
                            byte[] bytes = Convert.FromBase64String(kvp.Value);
                            using var ms = new MemoryStream(bytes);
                            using var loadedImg = Image.FromStream(ms);
                            var assetDef = imageAssets.FirstOrDefault(a => string.Equals(a.Name, kvp.Key, StringComparison.OrdinalIgnoreCase));
                            if (assetDef != null) selectedAssetImages[assetDef.Name] = new Bitmap(loadedImg);
                        }
                        catch { }
                    }
                }

                if (listBoxImageAssets.SelectedIndex >= 0) ListBoxImageAssets_SelectedIndexChanged(listBoxImageAssets, EventArgs.Empty);
            }
            catch
            {
                MessageBox.Show(this, "The selected file could not be opened.", "Open Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveCurrentProject(string filePath, bool asTemplate)
        {
            var assetImagesDict = new Dictionary<string, string>();
            foreach (var kvp in selectedAssetImages)
            {
                using var ms = new MemoryStream();
                kvp.Value.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                assetImagesDict[kvp.Key] = Convert.ToBase64String(ms.ToArray());
            }

            if (originalIconBytes != null && originalIconBytes.Length > 0)
            {
                assetImagesDict[ProjectDefaultIconKey] = Convert.ToBase64String(originalIconBytes);
            }
            else if (pictureBoxIcon.Image != null)
            {
                using var msIcon = new MemoryStream();
                pictureBoxIcon.Image.Save(msIcon, System.Drawing.Imaging.ImageFormat.Png);
                assetImagesDict[ProjectDefaultIconKey] = Convert.ToBase64String(msIcon.ToArray());
            }

            string iconPathTag = pictureBoxIcon.Tag?.ToString() ?? string.Empty;
            int numProcessors = 1;
            int coresPerProc = 1;
            int.TryParse(comboBoxNumberOfProcessors.Text, out numProcessors);
            int.TryParse(comboBoxNumberOfCoresPerProcessor.Text, out coresPerProc);

            string guestOSKey = textBoxGuestOS.Text?.Trim();
            if (string.IsNullOrEmpty(guestOSKey)) guestOSKey = "other";

            string colorHex = ColorTranslator.ToHtml(Color.FromArgb(255, buttonTextColor.BackColor));

            var data = new ProjectData
            {
                RecommendedMinMemory = (int)numericUpDownMinMemory.Value,
                RecommendedDefaultMemory = (int)numericUpDownDefaultMemory.Value,
                RecommendedMaxMemory = (int)numericUpDownMaxMemory.Value,
                VmName = textBoxVMName.Text,
                VmLocation = textBoxVMLocation.Text,
                IconPath = iconPathTag,
                TextColorHex = colorHex,
                TextFontName = comboBoxTextFont.SelectedItem?.ToString(),
                SelectedLanguage = comboBoxLanguage.SelectedItem?.ToString(),
                IsTemplate = asTemplate,
                AssetImages = assetImagesDict,
                HardwareVersion = (int)numericUpDownHardwareVersion.Value,
                OSDisplayName = textBoxOSDisplayName.Text,
                GuestOSID = guestOSKey,
                IsoImagePath = textBoxIsoPath.Text,
                IsoControllerType = radioButtonIsoSCSI.Checked ? "SCSI" : (radioButtonIsoSATA.Checked ? "SATA" : "IDE"),
                FirmwareType = radioButtonUEFI.Checked ? "UEFI" : "BIOS",
                FirmwareForced = checkBoxFirmwareForced.Checked,
                NumberOfProcessors = numProcessors,
                CoresPerProcessor = coresPerProc,
                SoundCardSB16Checked = checkBoxSB16.Checked,
                SoundCardSB16Selected = radioButtonSB16.Checked,
                SoundCardES1371Checked = checkBoxES1371.Checked,
                SoundCardES1371Selected = radioButtonES1371.Checked,
                SoundCardHDAudioChecked = checkBoxHDAudio.Checked,
                SoundCardHDAudioSelected = radioButtonHDAudio.Checked,
                SCSIBusLogicChecked = checkBoxBusLogic.Checked,
                SCSIBusLogicSelected = radioButtonBusLogic.Checked,
                SCSILSILogicChecked = checkBoxLSILogic.Checked,
                SCSILSILogicSelected = radioButtonLSILogic.Checked,
                SCSILSILogicSASChecked = checkBoxLSILogicSAS.Checked,
                SCSILSILogicSASSelected = radioButtonLSILogicSAS.Checked,
                SCSIParavirtualizedChecked = checkBoxParavirtualized.Checked,
                SCSIParavirtualizedSelected = radioButtonParavirtualized.Checked,
                HardDriveIDEChecked = checkBoxIDE.Checked,
                HardDriveIDESelected = radioButtonIDE.Checked,
                HardDriveSCSIChecked = checkBoxSCSI.Checked,
                HardDriveSCSISelected = radioButtonSCSI.Checked,
                HardDriveSATASelected = radioButtonSATA.Checked,
                HardDriveSATAChecked = checkBoxSATA.Checked,
                HardDriveNVMeChecked = checkBoxNVMe.Checked,
                HardDriveNVMeSelected = radioButtonNVMe.Checked,
                NetworkE1000PCIChecked = checkBoxE1000PCI.Checked,
                NetworkE1000PCISelected = radioButtonE1000PCI.Checked,
                NetworkE1000PCIeChecked = checkBoxE1000PCIe.Checked,
                NetworkE1000PCIeSelected = radioButtonE1000PCIe.Checked,
                NetworkPCNet32Checked = checkBoxPCNet32.Checked,
                NetworkPCNet32Selected = radioButtonPCNet32.Checked,
                NetworkVMXNET3Checked = checkBoxradioButtonVMXNET3.Checked,
                NetworkVMXNET3Selected = radioButtonradioButtonVMXNET3.Checked,
                HardDriveCapacityGB = (int)numericUpDownHardDriveCapacity.Value,
                FloppyDriveCount = (int)numericUpDownFloppyCount.Value,
                IDECDCount = (int)numericUpDownIDECDCount.Value,
                SCSICDCount = (int)numericUpDownSCSICDCount.Value,
                SATACDCount = (int)numericUpDownSATACDCount.Value
            };

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        private void UpdateFileCommandLabels()
        {
            string fileName = string.IsNullOrEmpty(currentFilePath) ? string.Empty : Path.GetFileName(currentFilePath);
            string titleName = string.IsNullOrEmpty(fileName) ? "VMware VM Generator Studio" : $"{fileName} - VMware VM Generator Studio";

            if (isModified)
            {
                titleName += " *";
            }

            this.Text = titleName;

            saveToolStripMenuItem.Text = isTemplateFile ? "Save Template" : "Save";
            saveAsToolStripMenuItem.Text = "Save As...";
        }

        private void ResetFormState()
        {
            foreach (var img in selectedAssetImages.Values) img.Dispose();
            selectedAssetImages.Clear();
            ClearSelectedAssetPreview();

            originalIconBytes = null;

            textBoxVMName.Text = string.Empty;
            textBoxVMLocation.Text = @"%USERPROFILE%\Virtual Machines";

            if (pictureBoxIcon.Image != null) { pictureBoxIcon.Image.Dispose(); pictureBoxIcon.Image = null; }
            string fullIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultIconRelativePath);
            if (File.Exists(fullIconPath))
            {
                try
                {
                    originalIconBytes = File.ReadAllBytes(fullIconPath);
                    using (var icon = new Icon(fullIconPath))
                    {
                        pictureBoxIcon.Image = icon.ToBitmap();
                    }
                }
                catch { }
            }
            pictureBoxIcon.ImageLocation = string.Empty;
            pictureBoxIcon.Tag = null;

            LoadDefaultThemeAssets();

            buttonTextColor.ForeColor = Color.Black;
            buttonTextColor.BackColor = Color.Black;
            buttonTextColor.UseVisualStyleBackColor = false;

            numericUpDownHardwareVersion.Value = 19;

            textBoxOSDisplayName.Text = string.Empty;
            textBoxGuestOS.Text = string.Empty;
            textBoxIsoPath.Text = string.Empty;
            radioButtonIsoIDE.Checked = true;

            radioButtonBIOS.Checked = true;
            radioButtonUEFI.Checked = false;
            checkBoxFirmwareForced.Checked = false;

            if (comboBoxNumberOfProcessors.Items.Count > 0) comboBoxNumberOfProcessors.SelectedIndex = 0;
            if (comboBoxNumberOfCoresPerProcessor.Items.Count > 0) comboBoxNumberOfCoresPerProcessor.SelectedIndex = 0;

            isUpdatingSoundCheckboxes = true;
            checkBoxSB16.Checked = true;
            checkBoxES1371.Checked = true;
            checkBoxHDAudio.Checked = true;
            isUpdatingSoundCheckboxes = false;

            radioButtonSB16.Checked = true;
            radioButtonSB16.Enabled = true;
            radioButtonES1371.Checked = false;
            radioButtonES1371.Enabled = true;
            radioButtonHDAudio.Checked = false;
            radioButtonHDAudio.Enabled = true;

            isUpdatingSCSICheckboxes = true;
            checkBoxBusLogic.Checked = true;
            checkBoxLSILogic.Checked = true;
            checkBoxLSILogicSAS.Checked = true;
            checkBoxParavirtualized.Checked = true;
            isUpdatingSCSICheckboxes = false;

            radioButtonBusLogic.Checked = true;
            radioButtonBusLogic.Enabled = true;
            radioButtonLSILogic.Checked = false;
            radioButtonLSILogic.Enabled = true;
            radioButtonLSILogicSAS.Checked = false;
            radioButtonLSILogicSAS.Enabled = true;
            radioButtonParavirtualized.Checked = false;
            radioButtonParavirtualized.Enabled = true;

            isUpdatingHardDriveCheckboxes = true;
            checkBoxIDE.Checked = true;
            checkBoxSCSI.Checked = true;
            checkBoxSATA.Checked = true;
            checkBoxNVMe.Checked = true;
            isUpdatingHardDriveCheckboxes = false;

            radioButtonIDE.Checked = true;
            radioButtonIDE.Enabled = true;
            radioButtonSCSI.Checked = false;
            radioButtonSCSI.Enabled = true;
            radioButtonSATA.Checked = false;
            radioButtonSATA.Enabled = true;
            radioButtonNVMe.Checked = false;
            radioButtonNVMe.Enabled = true;

            isUpdatingNetworkCheckboxes = true;
            checkBoxE1000PCI.Checked = true;
            checkBoxE1000PCIe.Checked = true;
            checkBoxPCNet32.Checked = true;
            checkBoxradioButtonVMXNET3.Checked = true;
            isUpdatingNetworkCheckboxes = false;

            radioButtonE1000PCI.Checked = true;
            radioButtonE1000PCI.Enabled = true;
            radioButtonE1000PCIe.Checked = false;
            radioButtonE1000PCIe.Enabled = true;
            radioButtonPCNet32.Checked = false;
            radioButtonPCNet32.Enabled = true;
            radioButtonradioButtonVMXNET3.Checked = false;
            radioButtonradioButtonVMXNET3.Enabled = true;

            numericUpDownHardDriveCapacity.Value = 40;

            numericUpDownFloppyCount.Value = 1;
            numericUpDownIDECDCount.Value = 1;
            numericUpDownSCSICDCount.Value = 0;
            numericUpDownSATACDCount.Value = 0;

            if (comboBoxTextFont.Items.Count > 0)
            {
                var defaultFont = comboBoxTextFont.Items.Cast<object>().Select(item => item.ToString())
                    .FirstOrDefault(name => string.Equals(name, "Arial", StringComparison.OrdinalIgnoreCase)) ?? comboBoxTextFont.Items[0].ToString();
                comboBoxTextFont.SelectedItem = defaultFont;
            }

            int englishIndex = -1;
            for (int i = 0; i < comboBoxLanguage.Items.Count; i++)
            {
                if (string.Equals(comboBoxLanguage.Items[i].ToString(), "English", StringComparison.OrdinalIgnoreCase))
                {
                    englishIndex = i;
                    break;
                }
            }

            if (englishIndex >= 0)
            {
                comboBoxLanguage.SelectedIndex = englishIndex;
            }
            else if (comboBoxLanguage.Items.Count > 0)
            {
                comboBoxLanguage.SelectedIndex = 0;
            }

            isModified = false;
            UpdateFileCommandLabels();
        }

        private void ButtonVMLocationBrowse_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK) textBoxVMLocation.Text = dialog.SelectedPath;
        }

        private void ButtonVMLocationVariables_Click(object sender, EventArgs e)
        {
            var menu = new ContextMenuStrip();

            var variables = new Dictionary<string, string>
            {
                { "Current User Directory", "%USERPROFILE%" },
                { "Documents Directory", "%USERPROFILE%\\Documents" },
                { "Application Data Directory", "%APPDATA%" },
                { "Local Application Data Directory", "%LOCALAPPDATA%" },
                { "Temporary Files Directory", "%TEMP%" },
                { "System Directory", "%WINDIR%\\System32" },
                { "Program Files Directory", "%PROGRAMFILES%" },
                { "Public Directory", "%PUBLIC%" },
                { "System Drive", "%SystemDrive%" }
            };

            foreach (var kvp in variables)
            {
                menu.Items.Add(kvp.Key, null, (_, __) => InsertVariable(kvp.Value));
            }

            menu.Show(Cursor.Position);
        }

        private void InsertVariable(string variable)
        {
            if (textBoxVMLocation.Focused) textBoxVMLocation.SelectedText = variable;
            else textBoxVMLocation.Text = variable;
        }

        private void ButtonChangeIcon_Click(object sender, EventArgs e)
        {
            StringBuilder iconPath = new StringBuilder(260);
            string currentTag = pictureBoxIcon.Tag?.ToString();
            if (!string.IsNullOrEmpty(currentTag)) iconPath.Append(currentTag);
            else iconPath.Append("shell32.dll");

            int iconIndex = 0;
            if (PickIconDlg(this.Handle, iconPath, iconPath.Capacity, ref iconIndex))
            {
                string selectedPath = iconPath.ToString();
                try
                {
                    if (File.Exists(selectedPath) && Path.GetExtension(selectedPath).Equals(".ico", StringComparison.OrdinalIgnoreCase))
                    {
                        originalIconBytes = File.ReadAllBytes(selectedPath);
                    }
                    else
                    {
                        originalIconBytes = ExtractIconResourceBytes(selectedPath, iconIndex);
                    }

                    IntPtr hIcon = ExtractAssociatedIcon(IntPtr.Zero, iconPath, ref iconIndex);
                    if (hIcon != IntPtr.Zero)
                    {
                        using (var icon = Icon.FromHandle(hIcon))
                        {
                            if (pictureBoxIcon.Image != null) pictureBoxIcon.Image.Dispose();
                            pictureBoxIcon.Image = icon.ToBitmap();
                        }
                        DestroyIcon(hIcon);
                        pictureBoxIcon.Tag = selectedPath;
                        pictureBoxIcon.ImageLocation = string.Empty;
                        MarkAsModified(sender, e);
                    }
                }
                catch { }
            }
        }

        private byte[]? ExtractIconResourceBytes(string filePath, int iconIndex)
        {
            try
            {
                IntPtr hInst = LoadLibraryEx(filePath, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (hInst == IntPtr.Zero) return null;

                List<IntPtr> groupIconPtrs = new List<IntPtr>();
                EnumResourceNames(hInst, RT_GROUP_ICON, (hModule, lpszType, lpszName, lParam) =>
                {
                    groupIconPtrs.Add(lpszName);
                    return true;
                }, IntPtr.Zero);

                if (iconIndex >= 0 && iconIndex < groupIconPtrs.Count)
                {
                    IntPtr resName = groupIconPtrs[iconIndex];
                    IntPtr hResInfo = FindResource(hInst, resName, RT_GROUP_ICON);
                    IntPtr hResData = LoadResource(hInst, hResInfo);
                    IntPtr pResLock = LockResource(hResData);
                    int size = (int)SizeofResource(hInst, hResInfo);

                    if (pResLock != IntPtr.Zero && size > 0)
                    {
                        byte[] groupBytes = new byte[size];
                        Marshal.Copy(pResLock, groupBytes, 0, size);

                        using (var ms = new MemoryStream())
                        using (var writer = new BinaryWriter(ms))
                        {
                            writer.Write(BitConverter.ToUInt16(groupBytes, 0));
                            writer.Write(BitConverter.ToUInt16(groupBytes, 2));
                            ushort count = BitConverter.ToUInt16(groupBytes, 4);
                            writer.Write(count);

                            int imageEntriesOffset = 6 + (count * 16);
                            List<byte[]> rawIconImages = new List<byte[]>();

                            for (int i = 0; i < count; i++)
                            {
                                int entryOffset = 6 + (i * 14);
                                byte width = groupBytes[entryOffset];
                                byte height = groupBytes[entryOffset + 1];
                                byte colorCount = groupBytes[entryOffset + 2];
                                byte reserved = groupBytes[entryOffset + 3];
                                ushort planes = BitConverter.ToUInt16(groupBytes, entryOffset + 4);
                                ushort bitCount = BitConverter.ToUInt16(groupBytes, entryOffset + 6);
                                uint bytesInRes = BitConverter.ToUInt32(groupBytes, entryOffset + 8);
                                ushort nId = BitConverter.ToUInt16(groupBytes, entryOffset + 12);

                                IntPtr hIconResInfo = FindResource(hInst, (IntPtr)nId, RT_ICON);
                                IntPtr hIconResData = LoadResource(hInst, hIconResInfo);
                                IntPtr pIconLock = LockResource(hIconResData);
                                int iconSize = (int)SizeofResource(hInst, hIconResInfo);

                                byte[] iconData = new byte[iconSize];
                                Marshal.Copy(pIconLock, iconData, 0, iconSize);
                                rawIconImages.Add(iconData);

                                writer.Write(width);
                                writer.Write(height);
                                writer.Write(colorCount);
                                writer.Write(reserved);
                                writer.Write(planes);
                                writer.Write(bitCount);
                                writer.Write(bytesInRes);
                                writer.Write((uint)imageEntriesOffset);
                                imageEntriesOffset += iconSize;
                            }

                            foreach (var imgData in rawIconImages)
                            {
                                writer.Write(imgData);
                            }

                            FreeLibrary(hInst);
                            return ms.ToArray();
                        }
                    }
                }

                FreeLibrary(hInst);
            }
            catch { }
            return null;
        }

        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x0000002;
        private static readonly IntPtr RT_ICON = (IntPtr.Zero + 3);
        private static readonly IntPtr RT_GROUP_ICON = (IntPtr.Zero + 14);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool EnumResourceNames(IntPtr hModule, IntPtr lpszType, EnumResNameProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumResNameProc(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);
    }

    public class ProjectData
    {
        public int RecommendedMinMemory { get; set; } = 512;
        public int RecommendedDefaultMemory { get; set; } = 2048;
        public int RecommendedMaxMemory { get; set; } = 4096;
        public string? VmName { get; set; }
        public string? VmLocation { get; set; }
        public string? IconPath { get; set; }
        public string? TextColorHex { get; set; }
        public string? TextFontName { get; set; }
        public string? SelectedLanguage { get; set; }
        public bool IsTemplate { get; set; }
        public Dictionary<string, string>? AssetImages { get; set; }
        public int HardwareVersion { get; set; } = 19;

        public string? OSDisplayName { get; set; }
        public string? GuestOSID { get; set; }
        public string? IsoImagePath { get; set; }
        public string IsoControllerType { get; set; } = "IDE";

        public string? FirmwareType { get; set; }
        public bool FirmwareForced { get; set; }
        public int NumberOfProcessors { get; set; }
        public int CoresPerProcessor { get; set; }

        public bool SoundCardSB16Checked { get; set; } = true;
        public bool SoundCardSB16Selected { get; set; } = true;
        public bool SoundCardES1371Checked { get; set; } = true;
        public bool SoundCardES1371Selected { get; set; }
        public bool SoundCardHDAudioChecked { get; set; } = true;
        public bool SoundCardHDAudioSelected { get; set; }

        public bool SCSIBusLogicChecked { get; set; } = true;
        public bool SCSIBusLogicSelected { get; set; } = true;
        public bool SCSILSILogicChecked { get; set; } = true;
        public bool SCSILSILogicSelected { get; set; }
        public bool SCSILSILogicSASChecked { get; set; } = true;
        public bool SCSILSILogicSASSelected { get; set; }
        public bool SCSIParavirtualizedChecked { get; set; } = true;
        public bool SCSIParavirtualizedSelected { get; set; }

        public bool HardDriveIDEChecked { get; set; } = true;
        public bool HardDriveIDESelected { get; set; } = true;
        public bool HardDriveSCSIChecked { get; set; } = true;
        public bool HardDriveSCSISelected { get; set; }
        public bool HardDriveSATASelected { get; set; }
        public bool HardDriveSATAChecked { get; set; } = true;
        public bool HardDriveNVMeChecked { get; set; } = true;
        public bool HardDriveNVMeSelected { get; set; }

        public bool NetworkE1000PCIChecked { get; set; } = true;
        public bool NetworkE1000PCISelected { get; set; } = true;
        public bool NetworkE1000PCIeChecked { get; set; } = true;
        public bool NetworkE1000PCIeSelected { get; set; }
        public bool NetworkPCNet32Checked { get; set; } = true;
        public bool NetworkPCNet32Selected { get; set; }
        public bool NetworkVMXNET3Checked { get; set; } = true;
        public bool NetworkVMXNET3Selected { get; set; }

        public int HardDriveCapacityGB { get; set; } = 40;

        public int FloppyDriveCount { get; set; } = 1;
        public int IDECDCount { get; set; } = 1;
        public int SCSICDCount { get; set; } = 0;
        public int SATACDCount { get; set; } = 0;
    }

    public class ImageAssetDefinition
    {
        public ImageAssetDefinition(string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
        }

        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
    }
}
