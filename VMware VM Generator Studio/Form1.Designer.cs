using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VMware_VM_Generator;

namespace VMware_VM_Generator_Studio
{
    partial class Form1 : Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator = new ToolStripSeparator();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            saveAsTemplateMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            buildToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            customizeToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            contentsToolStripMenuItem = new ToolStripMenuItem();
            indexToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            newToolStripButton = new ToolStripButton();
            openToolStripButton = new ToolStripButton();
            saveToolStripButton = new ToolStripButton();
            toolStripSeparator6 = new ToolStripSeparator();
            buildToolStripButton = new ToolStripButton();
            toolStripSeparator7 = new ToolStripSeparator();
            helpToolStripButton = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            tabControl1 = new TabControl();
            tabPageGeneral = new TabPage();
            pictureBoxIcon = new PictureBox();
            buttonChangeIcon = new Button();
            buttonVMLocationBrowse = new Button();
            buttonVMLocationVariables = new Button();
            textBoxVMLocation = new TextBox();
            textBoxVMName = new TextBox();
            labelVirtualMachineIcon = new Label();
            labelVMLocation = new Label();
            labelVMName = new Label();
            tabPageUI = new TabPage();
            labelTextFont = new Label();
            labelTextColor = new Label();
            comboBoxTextFont = new ComboBox();
            comboBoxLanguage = new ComboBox();
            labelLanguage = new Label();
            buttonTextColor = new Button();
            labelImageAssets = new Label();
            listBoxImageAssets = new ListBox();
            buttonSelectImageAsset = new Button();
            labelSelectedImageName = new Label();
            labelSelectedImageSize = new Label();
            pictureBoxSelectedImage = new PictureBox();
            labelImagePreview = new Label();
            tabPageHardware = new TabPage();
            groupBoxHardwareVersion = new GroupBox();
            labelHardwareVersion = new Label();
            numericUpDownHardwareVersion = new NumericUpDown();
            groupBoxOpticalAndFloppy = new GroupBox();
            labelFloppyCount = new Label();
            numericUpDownFloppyCount = new NumericUpDown();
            labelIDECDCount = new Label();
            numericUpDownIDECDCount = new NumericUpDown();
            labelSCSICDCount = new Label();
            numericUpDownSCSICDCount = new NumericUpDown();
            labelSATACDCount = new Label();
            numericUpDownSATACDCount = new NumericUpDown();
            groupBoxHardDrive = new GroupBox();
            labelHardDriveCapacity = new Label();
            numericUpDownHardDriveCapacity = new NumericUpDown();
            checkBoxNVMe = new CheckBox();
            checkBoxSATA = new CheckBox();
            checkBoxSCSI = new CheckBox();
            checkBoxIDE = new CheckBox();
            radioButtonNVMe = new RadioButton();
            radioButtonSATA = new RadioButton();
            radioButtonSCSI = new RadioButton();
            radioButtonIDE = new RadioButton();
            groupBoxSCSIController = new GroupBox();
            checkBoxParavirtualized = new CheckBox();
            checkBoxLSILogicSAS = new CheckBox();
            checkBoxLSILogic = new CheckBox();
            checkBoxBusLogic = new CheckBox();
            radioButtonParavirtualized = new RadioButton();
            radioButtonLSILogicSAS = new RadioButton();
            radioButtonLSILogic = new RadioButton();
            radioButtonBusLogic = new RadioButton();
            groupBox1 = new GroupBox();
            checkBoxradioButtonVMXNET3 = new CheckBox();
            checkBoxPCNet32 = new CheckBox();
            checkBoxE1000PCIe = new CheckBox();
            checkBoxE1000PCI = new CheckBox();
            radioButtonradioButtonVMXNET3 = new RadioButton();
            radioButtonPCNet32 = new RadioButton();
            radioButtonE1000PCIe = new RadioButton();
            radioButtonE1000PCI = new RadioButton();
            groupBoxSoundCard = new GroupBox();
            checkBoxHDAudio = new CheckBox();
            checkBoxES1371 = new CheckBox();
            checkBoxSB16 = new CheckBox();
            radioButtonHDAudio = new RadioButton();
            radioButtonES1371 = new RadioButton();
            radioButtonSB16 = new RadioButton();
            groupBoxMemory = new GroupBox();
            labelMinMemory = new Label();
            trackBarMinMemory = new CustomTrackBar();
            numericUpDownMinMemory = new NumericUpDown();
            labelDefaultMemory = new Label();
            trackBarDefaultMemory = new CustomTrackBar();
            numericUpDownDefaultMemory = new NumericUpDown();
            labelMaxMemory = new Label();
            trackBarMaxMemory = new CustomTrackBar();
            numericUpDownMaxMemory = new NumericUpDown();
            groupBoxProcessors = new GroupBox();
            labelTotalCoresNumber = new Label();
            labelTotalCores = new Label();
            labelNumberOfCoresPerProcessor = new Label();
            labelNumberOfProcessors = new Label();
            comboBoxNumberOfCoresPerProcessor = new ComboBox();
            comboBoxNumberOfProcessors = new ComboBox();
            groupBoxFirmware = new GroupBox();
            checkBoxFirmwareForced = new CheckBox();
            radioButtonUEFI = new RadioButton();
            radioButtonBIOS = new RadioButton();
            tabPageOS = new TabPage();
            groupBoxIsoMount = new GroupBox();
            radioButtonIsoSATA = new RadioButton();
            radioButtonIsoSCSI = new RadioButton();
            radioButtonIsoIDE = new RadioButton();
            labelIsoController = new Label();
            buttonIsoBrowse = new Button();
            textBoxIsoPath = new TextBox();
            labelIsoPath = new Label();
            groupBoxOSDetails = new GroupBox();
            textBoxGuestOS = new TextBox();
            buttonOSMenu = new Button();
            labelGuestOSID = new Label();
            textBoxOSDisplayName = new TextBox();
            labelOSDisplayName = new Label();
            fontDialog1 = new FontDialog();
            colorDialog1 = new ColorDialog();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPageGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).BeginInit();
            tabPageUI.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSelectedImage).BeginInit();
            tabPageHardware.SuspendLayout();
            groupBoxHardwareVersion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHardwareVersion).BeginInit();
            groupBoxOpticalAndFloppy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownFloppyCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownIDECDCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownSCSICDCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownSATACDCount).BeginInit();
            groupBoxHardDrive.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHardDriveCapacity).BeginInit();
            groupBoxSCSIController.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBoxSoundCard.SuspendLayout();
            groupBoxMemory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMinMemory).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDefaultMemory).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMaxMemory).BeginInit();
            groupBoxProcessors.SuspendLayout();
            groupBoxFirmware.SuspendLayout();
            tabPageOS.SuspendLayout();
            groupBoxIsoMount.SuspendLayout();
            groupBoxOSDetails.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(784, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripSeparator, saveToolStripMenuItem, saveAsToolStripMenuItem, saveAsTemplateMenuItem, toolStripSeparator1, buildToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Image = (Image)resources.GetObject("newToolStripMenuItem.Image");
            newToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(257, 22);
            newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Image = (Image)resources.GetObject("openToolStripMenuItem.Image");
            openToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(257, 22);
            openToolStripMenuItem.Text = "&Open";
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new Size(254, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(257, 22);
            saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            saveAsToolStripMenuItem.Size = new Size(257, 22);
            saveAsToolStripMenuItem.Text = "Save As...";
            // 
            // saveAsTemplateMenuItem
            // 
            saveAsTemplateMenuItem.Name = "saveAsTemplateMenuItem";
            saveAsTemplateMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.T;
            saveAsTemplateMenuItem.Size = new Size(257, 22);
            saveAsTemplateMenuItem.Text = "Save As a Template...";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(254, 6);
            // 
            // buildToolStripMenuItem
            // 
            buildToolStripMenuItem.Image = (Image)resources.GetObject("buildToolStripMenuItem.Image");
            buildToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            buildToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.B;
            buildToolStripMenuItem.Size = new Size(257, 22);
            buildToolStripMenuItem.Text = "&Build";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(254, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(257, 22);
            exitToolStripMenuItem.Text = "E&xit";
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { customizeToolStripMenuItem, optionsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(47, 20);
            toolsToolStripMenuItem.Text = "&Tools";
            toolsToolStripMenuItem.Visible = false;
            // 
            // customizeToolStripMenuItem
            // 
            customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            customizeToolStripMenuItem.Size = new Size(130, 22);
            customizeToolStripMenuItem.Text = "&Customize";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(130, 22);
            optionsToolStripMenuItem.Text = "&Options";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { contentsToolStripMenuItem, indexToolStripMenuItem, searchToolStripMenuItem, toolStripSeparator5, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            helpToolStripMenuItem.Visible = false;
            // 
            // contentsToolStripMenuItem
            // 
            contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            contentsToolStripMenuItem.Size = new Size(122, 22);
            contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            indexToolStripMenuItem.Size = new Size(122, 22);
            indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(122, 22);
            searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(119, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(122, 22);
            aboutToolStripMenuItem.Text = "&About...";
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.CanOverflow = false;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { newToolStripButton, openToolStripButton, saveToolStripButton, toolStripSeparator6, buildToolStripButton, toolStripSeparator7, helpToolStripButton });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(784, 28);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // newToolStripButton
            // 
            newToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            newToolStripButton.Image = (Image)resources.GetObject("newToolStripButton.Image");
            newToolStripButton.ImageTransparentColor = Color.Magenta;
            newToolStripButton.Name = "newToolStripButton";
            newToolStripButton.Size = new Size(23, 25);
            newToolStripButton.Text = "&New";
            // 
            // openToolStripButton
            // 
            openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            openToolStripButton.Image = (Image)resources.GetObject("openToolStripButton.Image");
            openToolStripButton.ImageTransparentColor = Color.Magenta;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Size = new Size(23, 25);
            openToolStripButton.Text = "&Open";
            // 
            // saveToolStripButton
            // 
            saveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            saveToolStripButton.Image = (Image)resources.GetObject("saveToolStripButton.Image");
            saveToolStripButton.ImageTransparentColor = Color.Magenta;
            saveToolStripButton.Name = "saveToolStripButton";
            saveToolStripButton.Size = new Size(23, 25);
            saveToolStripButton.Text = "&Save";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 28);
            // 
            // buildToolStripButton
            // 
            buildToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            buildToolStripButton.Image = (Image)resources.GetObject("buildToolStripButton.Image");
            buildToolStripButton.ImageTransparentColor = Color.Magenta;
            buildToolStripButton.Name = "buildToolStripButton";
            buildToolStripButton.Size = new Size(23, 25);
            buildToolStripButton.Text = "&Build";
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(6, 28);
            toolStripSeparator7.Visible = false;
            // 
            // helpToolStripButton
            // 
            helpToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            helpToolStripButton.Image = (Image)resources.GetObject("helpToolStripButton.Image");
            helpToolStripButton.ImageTransparentColor = Color.Magenta;
            helpToolStripButton.Name = "helpToolStripButton";
            helpToolStripButton.Size = new Size(23, 25);
            helpToolStripButton.Text = "He&lp";
            helpToolStripButton.Visible = false;
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(0, 419);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(784, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPageGeneral);
            tabControl1.Controls.Add(tabPageUI);
            tabControl1.Controls.Add(tabPageHardware);
            tabControl1.Controls.Add(tabPageOS);
            tabControl1.Location = new Point(0, 55);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(784, 364);
            tabControl1.TabIndex = 3;
            // 
            // tabPageGeneral
            // 
            tabPageGeneral.Controls.Add(pictureBoxIcon);
            tabPageGeneral.Controls.Add(buttonChangeIcon);
            tabPageGeneral.Controls.Add(buttonVMLocationBrowse);
            tabPageGeneral.Controls.Add(buttonVMLocationVariables);
            tabPageGeneral.Controls.Add(textBoxVMLocation);
            tabPageGeneral.Controls.Add(textBoxVMName);
            tabPageGeneral.Controls.Add(labelVirtualMachineIcon);
            tabPageGeneral.Controls.Add(labelVMLocation);
            tabPageGeneral.Controls.Add(labelVMName);
            tabPageGeneral.Location = new Point(4, 24);
            tabPageGeneral.Name = "tabPageGeneral";
            tabPageGeneral.Padding = new Padding(3);
            tabPageGeneral.Size = new Size(776, 336);
            tabPageGeneral.TabIndex = 0;
            tabPageGeneral.Text = "General";
            tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // pictureBoxIcon
            // 
            pictureBoxIcon.Location = new Point(173, 129);
            pictureBoxIcon.Name = "pictureBoxIcon";
            pictureBoxIcon.Size = new Size(32, 32);
            pictureBoxIcon.TabIndex = 3;
            pictureBoxIcon.TabStop = false;
            // 
            // buttonChangeIcon
            // 
            buttonChangeIcon.Location = new Point(211, 133);
            buttonChangeIcon.Name = "buttonChangeIcon";
            buttonChangeIcon.Size = new Size(104, 23);
            buttonChangeIcon.TabIndex = 2;
            buttonChangeIcon.Text = "Change Icon";
            buttonChangeIcon.UseVisualStyleBackColor = true;
            // 
            // buttonVMLocationBrowse
            // 
            buttonVMLocationBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonVMLocationBrowse.Location = new Point(690, 73);
            buttonVMLocationBrowse.Name = "buttonVMLocationBrowse";
            buttonVMLocationBrowse.Size = new Size(78, 23);
            buttonVMLocationBrowse.TabIndex = 2;
            buttonVMLocationBrowse.Text = "Browse...";
            buttonVMLocationBrowse.UseVisualStyleBackColor = true;
            // 
            // buttonVMLocationVariables
            // 
            buttonVMLocationVariables.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonVMLocationVariables.Location = new Point(662, 72);
            buttonVMLocationVariables.Name = "buttonVMLocationVariables";
            buttonVMLocationVariables.Size = new Size(22, 23);
            buttonVMLocationVariables.TabIndex = 2;
            buttonVMLocationVariables.Text = ">";
            buttonVMLocationVariables.UseVisualStyleBackColor = true;
            // 
            // textBoxVMLocation
            // 
            textBoxVMLocation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxVMLocation.Location = new Point(173, 73);
            textBoxVMLocation.Name = "textBoxVMLocation";
            textBoxVMLocation.Size = new Size(483, 23);
            textBoxVMLocation.TabIndex = 1;
            // 
            // textBoxVMName
            // 
            textBoxVMName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxVMName.Location = new Point(173, 33);
            textBoxVMName.Name = "textBoxVMName";
            textBoxVMName.Size = new Size(595, 23);
            textBoxVMName.TabIndex = 1;
            // 
            // labelVirtualMachineIcon
            // 
            labelVirtualMachineIcon.AutoSize = true;
            labelVirtualMachineIcon.Location = new Point(8, 135);
            labelVirtualMachineIcon.Name = "labelVirtualMachineIcon";
            labelVirtualMachineIcon.Size = new Size(119, 15);
            labelVirtualMachineIcon.TabIndex = 0;
            labelVirtualMachineIcon.Text = "Virtual machine icon:";
            // 
            // labelVMLocation
            // 
            labelVMLocation.AutoSize = true;
            labelVMLocation.Location = new Point(8, 76);
            labelVMLocation.Name = "labelVMLocation";
            labelVMLocation.Size = new Size(139, 15);
            labelVMLocation.TabIndex = 0;
            labelVMLocation.Text = "Virtual machine location:";
            // 
            // labelVMName
            // 
            labelVMName.AutoSize = true;
            labelVMName.Location = new Point(8, 36);
            labelVMName.Name = "labelVMName";
            labelVMName.Size = new Size(126, 15);
            labelVMName.TabIndex = 0;
            labelVMName.Text = "Virtual machine name:";
            // 
            // tabPageUI
            // 
            tabPageUI.AutoScroll = true;
            tabPageUI.Controls.Add(labelTextFont);
            tabPageUI.Controls.Add(labelTextColor);
            tabPageUI.Controls.Add(comboBoxTextFont);
            tabPageUI.Controls.Add(comboBoxLanguage);
            tabPageUI.Controls.Add(labelLanguage);
            tabPageUI.Controls.Add(buttonTextColor);
            tabPageUI.Controls.Add(labelImageAssets);
            tabPageUI.Controls.Add(listBoxImageAssets);
            tabPageUI.Controls.Add(buttonSelectImageAsset);
            tabPageUI.Controls.Add(labelSelectedImageName);
            tabPageUI.Controls.Add(labelSelectedImageSize);
            tabPageUI.Controls.Add(pictureBoxSelectedImage);
            tabPageUI.Controls.Add(labelImagePreview);
            tabPageUI.Location = new Point(4, 24);
            tabPageUI.Name = "tabPageUI";
            tabPageUI.Padding = new Padding(3);
            tabPageUI.Size = new Size(776, 336);
            tabPageUI.TabIndex = 1;
            tabPageUI.Text = "UI Design";
            tabPageUI.UseVisualStyleBackColor = true;
            // 
            // labelTextFont
            // 
            labelTextFont.AutoSize = true;
            labelTextFont.Location = new Point(197, 55);
            labelTextFont.Name = "labelTextFont";
            labelTextFont.Size = new Size(56, 15);
            labelTextFont.TabIndex = 2;
            labelTextFont.Text = "Text font:";
            // 
            // labelTextColor
            // 
            labelTextColor.AutoSize = true;
            labelTextColor.Location = new Point(8, 55);
            labelTextColor.Name = "labelTextColor";
            labelTextColor.Size = new Size(61, 15);
            labelTextColor.TabIndex = 2;
            labelTextColor.Text = "Text color:";
            // 
            // comboBoxTextFont
            // 
            comboBoxTextFont.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxTextFont.FormattingEnabled = true;
            comboBoxTextFont.Location = new Point(274, 52);
            comboBoxTextFont.Name = "comboBoxTextFont";
            comboBoxTextFont.Size = new Size(476, 23);
            comboBoxTextFont.TabIndex = 1;
            // 
            // comboBoxLanguage
            // 
            comboBoxLanguage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxLanguage.FormattingEnabled = true;
            comboBoxLanguage.Location = new Point(90, 20);
            comboBoxLanguage.Name = "comboBoxLanguage";
            comboBoxLanguage.Size = new Size(660, 23);
            comboBoxLanguage.TabIndex = 1;
            // 
            // labelLanguage
            // 
            labelLanguage.AutoSize = true;
            labelLanguage.Location = new Point(8, 23);
            labelLanguage.Name = "labelLanguage";
            labelLanguage.Size = new Size(62, 15);
            labelLanguage.TabIndex = 0;
            labelLanguage.Text = "Language:";
            // 
            // buttonTextColor
            // 
            buttonTextColor.BackColor = Color.Black;
            buttonTextColor.FlatAppearance.BorderColor = SystemColors.ControlDark;
            buttonTextColor.FlatStyle = FlatStyle.Flat;
            buttonTextColor.ForeColor = Color.Black;
            buttonTextColor.Location = new Point(90, 50);
            buttonTextColor.Name = "buttonTextColor";
            buttonTextColor.Size = new Size(82, 24);
            buttonTextColor.TabIndex = 3;
            buttonTextColor.UseVisualStyleBackColor = false;
            // 
            // labelImageAssets
            // 
            labelImageAssets.AutoSize = true;
            labelImageAssets.Location = new Point(8, 90);
            labelImageAssets.Name = "labelImageAssets";
            labelImageAssets.Size = new Size(77, 15);
            labelImageAssets.TabIndex = 4;
            labelImageAssets.Text = "Image assets:";
            // 
            // listBoxImageAssets
            // 
            listBoxImageAssets.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            listBoxImageAssets.FormattingEnabled = true;
            listBoxImageAssets.Location = new Point(90, 90);
            listBoxImageAssets.Name = "listBoxImageAssets";
            listBoxImageAssets.Size = new Size(660, 259);
            listBoxImageAssets.TabIndex = 5;
            // 
            // buttonSelectImageAsset
            // 
            buttonSelectImageAsset.Location = new Point(90, 353);
            buttonSelectImageAsset.Name = "buttonSelectImageAsset";
            buttonSelectImageAsset.Size = new Size(110, 23);
            buttonSelectImageAsset.TabIndex = 6;
            buttonSelectImageAsset.Text = "Select Image";
            buttonSelectImageAsset.UseVisualStyleBackColor = true;
            // 
            // labelSelectedImageName
            // 
            labelSelectedImageName.AutoSize = true;
            labelSelectedImageName.Location = new Point(390, 130);
            labelSelectedImageName.Name = "labelSelectedImageName";
            labelSelectedImageName.Size = new Size(0, 15);
            labelSelectedImageName.TabIndex = 7;
            // 
            // labelSelectedImageSize
            // 
            labelSelectedImageSize.AutoSize = true;
            labelSelectedImageSize.Location = new Point(390, 150);
            labelSelectedImageSize.Name = "labelSelectedImageSize";
            labelSelectedImageSize.Size = new Size(0, 15);
            labelSelectedImageSize.TabIndex = 8;
            // 
            // pictureBoxSelectedImage
            // 
            pictureBoxSelectedImage.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxSelectedImage.Location = new Point(90, 401);
            pictureBoxSelectedImage.Name = "pictureBoxSelectedImage";
            pictureBoxSelectedImage.Size = new Size(589, 369);
            pictureBoxSelectedImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxSelectedImage.TabIndex = 9;
            pictureBoxSelectedImage.TabStop = false;
            // 
            // labelImagePreview
            // 
            labelImagePreview.AutoSize = true;
            labelImagePreview.Location = new Point(90, 383);
            labelImagePreview.Name = "labelImagePreview";
            labelImagePreview.Size = new Size(51, 15);
            labelImagePreview.TabIndex = 10;
            labelImagePreview.Text = "Preview:";
            // 
            // tabPageHardware
            // 
            tabPageHardware.AutoScroll = true;
            tabPageHardware.Controls.Add(groupBoxHardwareVersion);
            tabPageHardware.Controls.Add(groupBoxOpticalAndFloppy);
            tabPageHardware.Controls.Add(groupBoxHardDrive);
            tabPageHardware.Controls.Add(groupBoxSCSIController);
            tabPageHardware.Controls.Add(groupBox1);
            tabPageHardware.Controls.Add(groupBoxSoundCard);
            tabPageHardware.Controls.Add(groupBoxMemory);
            tabPageHardware.Controls.Add(groupBoxProcessors);
            tabPageHardware.Controls.Add(groupBoxFirmware);
            tabPageHardware.Location = new Point(4, 24);
            tabPageHardware.Name = "tabPageHardware";
            tabPageHardware.Padding = new Padding(3);
            tabPageHardware.Size = new Size(776, 336);
            tabPageHardware.TabIndex = 2;
            tabPageHardware.Text = "Hardware";
            tabPageHardware.UseVisualStyleBackColor = true;
            // 
            // groupBoxHardwareVersion
            // 
            groupBoxHardwareVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxHardwareVersion.Controls.Add(labelHardwareVersion);
            groupBoxHardwareVersion.Controls.Add(numericUpDownHardwareVersion);
            groupBoxHardwareVersion.Location = new Point(8, 6);
            groupBoxHardwareVersion.Name = "groupBoxHardwareVersion";
            groupBoxHardwareVersion.Size = new Size(740, 60);
            groupBoxHardwareVersion.TabIndex = 0;
            groupBoxHardwareVersion.TabStop = false;
            groupBoxHardwareVersion.Text = "Hardware Version";
            // 
            // labelHardwareVersion
            // 
            labelHardwareVersion.AutoSize = true;
            labelHardwareVersion.Location = new Point(19, 26);
            labelHardwareVersion.Name = "labelHardwareVersion";
            labelHardwareVersion.Size = new Size(102, 15);
            labelHardwareVersion.TabIndex = 0;
            labelHardwareVersion.Text = "Hardware version:";
            // 
            // numericUpDownHardwareVersion
            // 
            numericUpDownHardwareVersion.Location = new Point(185, 24);
            numericUpDownHardwareVersion.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numericUpDownHardwareVersion.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownHardwareVersion.Name = "numericUpDownHardwareVersion";
            numericUpDownHardwareVersion.Size = new Size(80, 23);
            numericUpDownHardwareVersion.TabIndex = 1;
            numericUpDownHardwareVersion.Value = new decimal(new int[] { 19, 0, 0, 0 });
            // 
            // groupBoxOpticalAndFloppy
            // 
            groupBoxOpticalAndFloppy.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxOpticalAndFloppy.Controls.Add(labelFloppyCount);
            groupBoxOpticalAndFloppy.Controls.Add(numericUpDownFloppyCount);
            groupBoxOpticalAndFloppy.Controls.Add(labelIDECDCount);
            groupBoxOpticalAndFloppy.Controls.Add(numericUpDownIDECDCount);
            groupBoxOpticalAndFloppy.Controls.Add(labelSCSICDCount);
            groupBoxOpticalAndFloppy.Controls.Add(numericUpDownSCSICDCount);
            groupBoxOpticalAndFloppy.Controls.Add(labelSATACDCount);
            groupBoxOpticalAndFloppy.Controls.Add(numericUpDownSATACDCount);
            groupBoxOpticalAndFloppy.Location = new Point(8, 851);
            groupBoxOpticalAndFloppy.Name = "groupBoxOpticalAndFloppy";
            groupBoxOpticalAndFloppy.Size = new Size(740, 124);
            groupBoxOpticalAndFloppy.TabIndex = 7;
            groupBoxOpticalAndFloppy.TabStop = false;
            groupBoxOpticalAndFloppy.Text = "CD/DVD and Floppy Drives";
            // 
            // labelFloppyCount
            // 
            labelFloppyCount.AutoSize = true;
            labelFloppyCount.Location = new Point(19, 32);
            labelFloppyCount.Name = "labelFloppyCount";
            labelFloppyCount.Size = new Size(108, 15);
            labelFloppyCount.TabIndex = 0;
            labelFloppyCount.Text = "Floppy drives (0-2):";
            // 
            // numericUpDownFloppyCount
            // 
            numericUpDownFloppyCount.Location = new Point(185, 29);
            numericUpDownFloppyCount.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            numericUpDownFloppyCount.Name = "numericUpDownFloppyCount";
            numericUpDownFloppyCount.Size = new Size(80, 23);
            numericUpDownFloppyCount.TabIndex = 1;
            numericUpDownFloppyCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // labelIDECDCount
            // 
            labelIDECDCount.AutoSize = true;
            labelIDECDCount.Location = new Point(380, 32);
            labelIDECDCount.Name = "labelIDECDCount";
            labelIDECDCount.Size = new Size(136, 15);
            labelIDECDCount.TabIndex = 2;
            labelIDECDCount.Text = "IDE CD/DVD drives (0-3):";
            // 
            // numericUpDownIDECDCount
            // 
            numericUpDownIDECDCount.Location = new Point(540, 29);
            numericUpDownIDECDCount.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            numericUpDownIDECDCount.Name = "numericUpDownIDECDCount";
            numericUpDownIDECDCount.Size = new Size(80, 23);
            numericUpDownIDECDCount.TabIndex = 3;
            numericUpDownIDECDCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // labelSCSICDCount
            // 
            labelSCSICDCount.AutoSize = true;
            labelSCSICDCount.Location = new Point(19, 75);
            labelSCSICDCount.Name = "labelSCSICDCount";
            labelSCSICDCount.Size = new Size(148, 15);
            labelSCSICDCount.TabIndex = 4;
            labelSCSICDCount.Text = "SCSI CD/DVD drives (0-10):";
            // 
            // numericUpDownSCSICDCount
            // 
            numericUpDownSCSICDCount.Location = new Point(185, 72);
            numericUpDownSCSICDCount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownSCSICDCount.Name = "numericUpDownSCSICDCount";
            numericUpDownSCSICDCount.Size = new Size(80, 23);
            numericUpDownSCSICDCount.TabIndex = 5;
            // 
            // labelSATACDCount
            // 
            labelSATACDCount.AutoSize = true;
            labelSATACDCount.Location = new Point(380, 75);
            labelSATACDCount.Name = "labelSATACDCount";
            labelSATACDCount.Size = new Size(152, 15);
            labelSATACDCount.TabIndex = 6;
            labelSATACDCount.Text = "SATA CD/DVD drives (0-10):";
            // 
            // numericUpDownSATACDCount
            // 
            numericUpDownSATACDCount.Location = new Point(540, 72);
            numericUpDownSATACDCount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownSATACDCount.Name = "numericUpDownSATACDCount";
            numericUpDownSATACDCount.Size = new Size(80, 23);
            numericUpDownSATACDCount.TabIndex = 7;
            // 
            // groupBoxHardDrive
            // 
            groupBoxHardDrive.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxHardDrive.Controls.Add(labelHardDriveCapacity);
            groupBoxHardDrive.Controls.Add(numericUpDownHardDriveCapacity);
            groupBoxHardDrive.Controls.Add(checkBoxNVMe);
            groupBoxHardDrive.Controls.Add(checkBoxSATA);
            groupBoxHardDrive.Controls.Add(checkBoxSCSI);
            groupBoxHardDrive.Controls.Add(checkBoxIDE);
            groupBoxHardDrive.Controls.Add(radioButtonNVMe);
            groupBoxHardDrive.Controls.Add(radioButtonSATA);
            groupBoxHardDrive.Controls.Add(radioButtonSCSI);
            groupBoxHardDrive.Controls.Add(radioButtonIDE);
            groupBoxHardDrive.Location = new Point(266, 705);
            groupBoxHardDrive.Name = "groupBoxHardDrive";
            groupBoxHardDrive.Size = new Size(482, 140);
            groupBoxHardDrive.TabIndex = 6;
            groupBoxHardDrive.TabStop = false;
            groupBoxHardDrive.Text = "Hard Drive";
            // 
            // labelHardDriveCapacity
            // 
            labelHardDriveCapacity.AutoSize = true;
            labelHardDriveCapacity.Location = new Point(199, 32);
            labelHardDriveCapacity.Name = "labelHardDriveCapacity";
            labelHardDriveCapacity.Size = new Size(112, 15);
            labelHardDriveCapacity.TabIndex = 2;
            labelHardDriveCapacity.Text = "Drive Capacity (GB):";
            // 
            // numericUpDownHardDriveCapacity
            // 
            numericUpDownHardDriveCapacity.Location = new Point(324, 29);
            numericUpDownHardDriveCapacity.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDownHardDriveCapacity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownHardDriveCapacity.Name = "numericUpDownHardDriveCapacity";
            numericUpDownHardDriveCapacity.Size = new Size(89, 23);
            numericUpDownHardDriveCapacity.TabIndex = 3;
            numericUpDownHardDriveCapacity.Value = new decimal(new int[] { 40, 0, 0, 0 });
            // 
            // checkBoxNVMe
            // 
            checkBoxNVMe.AutoSize = true;
            checkBoxNVMe.Checked = true;
            checkBoxNVMe.CheckState = CheckState.Checked;
            checkBoxNVMe.Location = new Point(23, 107);
            checkBoxNVMe.Name = "checkBoxNVMe";
            checkBoxNVMe.Size = new Size(15, 14);
            checkBoxNVMe.TabIndex = 1;
            checkBoxNVMe.UseVisualStyleBackColor = true;
            // 
            // checkBoxSATA
            // 
            checkBoxSATA.AutoSize = true;
            checkBoxSATA.Checked = true;
            checkBoxSATA.CheckState = CheckState.Checked;
            checkBoxSATA.Location = new Point(23, 82);
            checkBoxSATA.Name = "checkBoxSATA";
            checkBoxSATA.Size = new Size(15, 14);
            checkBoxSATA.TabIndex = 1;
            checkBoxSATA.UseVisualStyleBackColor = true;
            // 
            // checkBoxSCSI
            // 
            checkBoxSCSI.AutoSize = true;
            checkBoxSCSI.Checked = true;
            checkBoxSCSI.CheckState = CheckState.Checked;
            checkBoxSCSI.Location = new Point(23, 57);
            checkBoxSCSI.Name = "checkBoxSCSI";
            checkBoxSCSI.Size = new Size(15, 14);
            checkBoxSCSI.TabIndex = 1;
            checkBoxSCSI.UseVisualStyleBackColor = true;
            // 
            // checkBoxIDE
            // 
            checkBoxIDE.AutoSize = true;
            checkBoxIDE.Checked = true;
            checkBoxIDE.CheckState = CheckState.Checked;
            checkBoxIDE.Location = new Point(23, 32);
            checkBoxIDE.Name = "checkBoxIDE";
            checkBoxIDE.Size = new Size(15, 14);
            checkBoxIDE.TabIndex = 1;
            checkBoxIDE.UseVisualStyleBackColor = true;
            // 
            // radioButtonNVMe
            // 
            radioButtonNVMe.AutoSize = true;
            radioButtonNVMe.Location = new Point(44, 104);
            radioButtonNVMe.Name = "radioButtonNVMe";
            radioButtonNVMe.Size = new Size(114, 19);
            radioButtonNVMe.TabIndex = 0;
            radioButtonNVMe.Text = "NVMe Controller";
            radioButtonNVMe.UseVisualStyleBackColor = true;
            // 
            // radioButtonSATA
            // 
            radioButtonSATA.AutoSize = true;
            radioButtonSATA.Location = new Point(44, 79);
            radioButtonSATA.Name = "radioButtonSATA";
            radioButtonSATA.Size = new Size(108, 19);
            radioButtonSATA.TabIndex = 0;
            radioButtonSATA.Text = "SATA Controller";
            radioButtonSATA.UseVisualStyleBackColor = true;
            // 
            // radioButtonSCSI
            // 
            radioButtonSCSI.AutoSize = true;
            radioButtonSCSI.Location = new Point(44, 54);
            radioButtonSCSI.Name = "radioButtonSCSI";
            radioButtonSCSI.Size = new Size(104, 19);
            radioButtonSCSI.TabIndex = 0;
            radioButtonSCSI.Text = "SCSI Controller";
            radioButtonSCSI.UseVisualStyleBackColor = true;
            // 
            // radioButtonIDE
            // 
            radioButtonIDE.AutoSize = true;
            radioButtonIDE.Checked = true;
            radioButtonIDE.Location = new Point(44, 29);
            radioButtonIDE.Name = "radioButtonIDE";
            radioButtonIDE.Size = new Size(98, 19);
            radioButtonIDE.TabIndex = 0;
            radioButtonIDE.TabStop = true;
            radioButtonIDE.Text = "IDE Controller";
            radioButtonIDE.UseVisualStyleBackColor = true;
            // 
            // groupBoxSCSIController
            // 
            groupBoxSCSIController.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxSCSIController.Controls.Add(checkBoxParavirtualized);
            groupBoxSCSIController.Controls.Add(checkBoxLSILogicSAS);
            groupBoxSCSIController.Controls.Add(checkBoxLSILogic);
            groupBoxSCSIController.Controls.Add(checkBoxBusLogic);
            groupBoxSCSIController.Controls.Add(radioButtonParavirtualized);
            groupBoxSCSIController.Controls.Add(radioButtonLSILogicSAS);
            groupBoxSCSIController.Controls.Add(radioButtonLSILogic);
            groupBoxSCSIController.Controls.Add(radioButtonBusLogic);
            groupBoxSCSIController.Location = new Point(266, 568);
            groupBoxSCSIController.Name = "groupBoxSCSIController";
            groupBoxSCSIController.Size = new Size(482, 132);
            groupBoxSCSIController.TabIndex = 5;
            groupBoxSCSIController.TabStop = false;
            groupBoxSCSIController.Text = "SCSI I/O Controller";
            // 
            // checkBoxParavirtualized
            // 
            checkBoxParavirtualized.AutoSize = true;
            checkBoxParavirtualized.Checked = true;
            checkBoxParavirtualized.CheckState = CheckState.Checked;
            checkBoxParavirtualized.Location = new Point(23, 107);
            checkBoxParavirtualized.Name = "checkBoxParavirtualized";
            checkBoxParavirtualized.Size = new Size(15, 14);
            checkBoxParavirtualized.TabIndex = 1;
            checkBoxParavirtualized.UseVisualStyleBackColor = true;
            // 
            // checkBoxLSILogicSAS
            // 
            checkBoxLSILogicSAS.AutoSize = true;
            checkBoxLSILogicSAS.Checked = true;
            checkBoxLSILogicSAS.CheckState = CheckState.Checked;
            checkBoxLSILogicSAS.Location = new Point(23, 82);
            checkBoxLSILogicSAS.Name = "checkBoxLSILogicSAS";
            checkBoxLSILogicSAS.Size = new Size(15, 14);
            checkBoxLSILogicSAS.TabIndex = 1;
            checkBoxLSILogicSAS.UseVisualStyleBackColor = true;
            // 
            // checkBoxLSILogic
            // 
            checkBoxLSILogic.AutoSize = true;
            checkBoxLSILogic.Checked = true;
            checkBoxLSILogic.CheckState = CheckState.Checked;
            checkBoxLSILogic.Location = new Point(23, 57);
            checkBoxLSILogic.Name = "checkBoxLSILogic";
            checkBoxLSILogic.Size = new Size(15, 14);
            checkBoxLSILogic.TabIndex = 1;
            checkBoxLSILogic.UseVisualStyleBackColor = true;
            // 
            // checkBoxBusLogic
            // 
            checkBoxBusLogic.AutoSize = true;
            checkBoxBusLogic.Checked = true;
            checkBoxBusLogic.CheckState = CheckState.Checked;
            checkBoxBusLogic.Location = new Point(23, 32);
            checkBoxBusLogic.Name = "checkBoxBusLogic";
            checkBoxBusLogic.Size = new Size(15, 14);
            checkBoxBusLogic.TabIndex = 1;
            checkBoxBusLogic.UseVisualStyleBackColor = true;
            // 
            // radioButtonParavirtualized
            // 
            radioButtonParavirtualized.AutoSize = true;
            radioButtonParavirtualized.Location = new Point(44, 104);
            radioButtonParavirtualized.Name = "radioButtonParavirtualized";
            radioButtonParavirtualized.Size = new Size(128, 19);
            radioButtonParavirtualized.TabIndex = 0;
            radioButtonParavirtualized.Text = "Paravirtualized SCSI";
            radioButtonParavirtualized.UseVisualStyleBackColor = true;
            // 
            // radioButtonLSILogicSAS
            // 
            radioButtonLSILogicSAS.AutoSize = true;
            radioButtonLSILogicSAS.Location = new Point(44, 79);
            radioButtonLSILogicSAS.Name = "radioButtonLSILogicSAS";
            radioButtonLSILogicSAS.Size = new Size(95, 19);
            radioButtonLSILogicSAS.TabIndex = 0;
            radioButtonLSILogicSAS.Text = "LSI Logic SAS";
            radioButtonLSILogicSAS.UseVisualStyleBackColor = true;
            // 
            // radioButtonLSILogic
            // 
            radioButtonLSILogic.AutoSize = true;
            radioButtonLSILogic.Location = new Point(44, 54);
            radioButtonLSILogic.Name = "radioButtonLSILogic";
            radioButtonLSILogic.Size = new Size(72, 19);
            radioButtonLSILogic.TabIndex = 0;
            radioButtonLSILogic.Text = "LSI Logic";
            radioButtonLSILogic.UseVisualStyleBackColor = true;
            // 
            // radioButtonBusLogic
            // 
            radioButtonBusLogic.AutoSize = true;
            radioButtonBusLogic.Checked = true;
            radioButtonBusLogic.Location = new Point(44, 29);
            radioButtonBusLogic.Name = "radioButtonBusLogic";
            radioButtonBusLogic.Size = new Size(238, 19);
            radioButtonBusLogic.TabIndex = 0;
            radioButtonBusLogic.TabStop = true;
            radioButtonBusLogic.Text = "BusLogic (Maximum disk capacity: 2 TB)";
            radioButtonBusLogic.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBoxradioButtonVMXNET3);
            groupBox1.Controls.Add(checkBoxPCNet32);
            groupBox1.Controls.Add(checkBoxE1000PCIe);
            groupBox1.Controls.Add(checkBoxE1000PCI);
            groupBox1.Controls.Add(radioButtonradioButtonVMXNET3);
            groupBox1.Controls.Add(radioButtonPCNet32);
            groupBox1.Controls.Add(radioButtonE1000PCIe);
            groupBox1.Controls.Add(radioButtonE1000PCI);
            groupBox1.Location = new Point(8, 706);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(252, 139);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Network Adapter";
            // 
            // checkBoxradioButtonVMXNET3
            // 
            checkBoxradioButtonVMXNET3.AutoSize = true;
            checkBoxradioButtonVMXNET3.Checked = true;
            checkBoxradioButtonVMXNET3.CheckState = CheckState.Checked;
            checkBoxradioButtonVMXNET3.Location = new Point(23, 106);
            checkBoxradioButtonVMXNET3.Name = "checkBoxradioButtonVMXNET3";
            checkBoxradioButtonVMXNET3.Size = new Size(15, 14);
            checkBoxradioButtonVMXNET3.TabIndex = 1;
            checkBoxradioButtonVMXNET3.UseVisualStyleBackColor = true;
            // 
            // checkBoxPCNet32
            // 
            checkBoxPCNet32.AutoSize = true;
            checkBoxPCNet32.Checked = true;
            checkBoxPCNet32.CheckState = CheckState.Checked;
            checkBoxPCNet32.Location = new Point(23, 82);
            checkBoxPCNet32.Name = "checkBoxPCNet32";
            checkBoxPCNet32.Size = new Size(15, 14);
            checkBoxPCNet32.TabIndex = 1;
            checkBoxPCNet32.UseVisualStyleBackColor = true;
            // 
            // checkBoxE1000PCIe
            // 
            checkBoxE1000PCIe.AutoSize = true;
            checkBoxE1000PCIe.Checked = true;
            checkBoxE1000PCIe.CheckState = CheckState.Checked;
            checkBoxE1000PCIe.Location = new Point(23, 57);
            checkBoxE1000PCIe.Name = "checkBoxE1000PCIe";
            checkBoxE1000PCIe.Size = new Size(15, 14);
            checkBoxE1000PCIe.TabIndex = 1;
            checkBoxE1000PCIe.UseVisualStyleBackColor = true;
            // 
            // checkBoxE1000PCI
            // 
            checkBoxE1000PCI.AutoSize = true;
            checkBoxE1000PCI.Checked = true;
            checkBoxE1000PCI.CheckState = CheckState.Checked;
            checkBoxE1000PCI.Location = new Point(23, 32);
            checkBoxE1000PCI.Name = "checkBoxE1000PCI";
            checkBoxE1000PCI.Size = new Size(15, 14);
            checkBoxE1000PCI.TabIndex = 1;
            checkBoxE1000PCI.UseVisualStyleBackColor = true;
            // 
            // radioButtonradioButtonVMXNET3
            // 
            radioButtonradioButtonVMXNET3.AutoSize = true;
            radioButtonradioButtonVMXNET3.Location = new Point(44, 103);
            radioButtonradioButtonVMXNET3.Name = "radioButtonradioButtonVMXNET3";
            radioButtonradioButtonVMXNET3.Size = new Size(124, 19);
            radioButtonradioButtonVMXNET3.TabIndex = 0;
            radioButtonradioButtonVMXNET3.Text = "VMware VMXNET3";
            radioButtonradioButtonVMXNET3.UseVisualStyleBackColor = true;
            // 
            // radioButtonPCNet32
            // 
            radioButtonPCNet32.AutoSize = true;
            radioButtonPCNet32.Location = new Point(44, 79);
            radioButtonPCNet32.Name = "radioButtonPCNet32";
            radioButtonPCNet32.Size = new Size(104, 19);
            radioButtonPCNet32.TabIndex = 0;
            radioButtonPCNet32.Text = "AMD PCnet-32";
            radioButtonPCNet32.UseVisualStyleBackColor = true;
            // 
            // radioButtonE1000PCIe
            // 
            radioButtonE1000PCIe.AutoSize = true;
            radioButtonE1000PCIe.Location = new Point(44, 54);
            radioButtonE1000PCIe.Name = "radioButtonE1000PCIe";
            radioButtonE1000PCIe.Size = new Size(130, 19);
            radioButtonE1000PCIe.TabIndex = 0;
            radioButtonE1000PCIe.Text = "Intel PRO/1000 PCIe";
            radioButtonE1000PCIe.UseVisualStyleBackColor = true;
            // 
            // radioButtonE1000PCI
            // 
            radioButtonE1000PCI.AutoSize = true;
            radioButtonE1000PCI.Checked = true;
            radioButtonE1000PCI.Location = new Point(44, 29);
            radioButtonE1000PCI.Name = "radioButtonE1000PCI";
            radioButtonE1000PCI.Size = new Size(124, 19);
            radioButtonE1000PCI.TabIndex = 0;
            radioButtonE1000PCI.TabStop = true;
            radioButtonE1000PCI.Text = "Intel PRO/1000 PCI";
            radioButtonE1000PCI.UseVisualStyleBackColor = true;
            // 
            // groupBoxSoundCard
            // 
            groupBoxSoundCard.Controls.Add(checkBoxHDAudio);
            groupBoxSoundCard.Controls.Add(checkBoxES1371);
            groupBoxSoundCard.Controls.Add(checkBoxSB16);
            groupBoxSoundCard.Controls.Add(radioButtonHDAudio);
            groupBoxSoundCard.Controls.Add(radioButtonES1371);
            groupBoxSoundCard.Controls.Add(radioButtonSB16);
            groupBoxSoundCard.Location = new Point(8, 568);
            groupBoxSoundCard.Name = "groupBoxSoundCard";
            groupBoxSoundCard.Size = new Size(252, 132);
            groupBoxSoundCard.TabIndex = 4;
            groupBoxSoundCard.TabStop = false;
            groupBoxSoundCard.Text = "Sound Card";
            // 
            // checkBoxHDAudio
            // 
            checkBoxHDAudio.AutoSize = true;
            checkBoxHDAudio.Checked = true;
            checkBoxHDAudio.CheckState = CheckState.Checked;
            checkBoxHDAudio.Location = new Point(23, 82);
            checkBoxHDAudio.Name = "checkBoxHDAudio";
            checkBoxHDAudio.Size = new Size(15, 14);
            checkBoxHDAudio.TabIndex = 1;
            checkBoxHDAudio.UseVisualStyleBackColor = true;
            // 
            // checkBoxES1371
            // 
            checkBoxES1371.AutoSize = true;
            checkBoxES1371.Checked = true;
            checkBoxES1371.CheckState = CheckState.Checked;
            checkBoxES1371.Location = new Point(23, 57);
            checkBoxES1371.Name = "checkBoxES1371";
            checkBoxES1371.Size = new Size(15, 14);
            checkBoxES1371.TabIndex = 1;
            checkBoxES1371.UseVisualStyleBackColor = true;
            // 
            // checkBoxSB16
            // 
            checkBoxSB16.AutoSize = true;
            checkBoxSB16.Checked = true;
            checkBoxSB16.CheckState = CheckState.Checked;
            checkBoxSB16.Location = new Point(23, 32);
            checkBoxSB16.Name = "checkBoxSB16";
            checkBoxSB16.Size = new Size(15, 14);
            checkBoxSB16.TabIndex = 1;
            checkBoxSB16.UseVisualStyleBackColor = true;
            // 
            // radioButtonHDAudio
            // 
            radioButtonHDAudio.AutoSize = true;
            radioButtonHDAudio.Location = new Point(44, 79);
            radioButtonHDAudio.Name = "radioButtonHDAudio";
            radioButtonHDAudio.Size = new Size(167, 19);
            radioButtonHDAudio.TabIndex = 0;
            radioButtonHDAudio.Text = "Intel High Definition Audio";
            radioButtonHDAudio.UseVisualStyleBackColor = true;
            // 
            // radioButtonES1371
            // 
            radioButtonES1371.AutoSize = true;
            radioButtonES1371.Location = new Point(44, 54);
            radioButtonES1371.Name = "radioButtonES1371";
            radioButtonES1371.Size = new Size(166, 19);
            radioButtonES1371.TabIndex = 0;
            radioButtonES1371.Text = "Creative Ensoniq AudioPCI";
            radioButtonES1371.UseVisualStyleBackColor = true;
            // 
            // radioButtonSB16
            // 
            radioButtonSB16.AutoSize = true;
            radioButtonSB16.Checked = true;
            radioButtonSB16.Location = new Point(44, 29);
            radioButtonSB16.Name = "radioButtonSB16";
            radioButtonSB16.Size = new Size(185, 19);
            radioButtonSB16.TabIndex = 0;
            radioButtonSB16.TabStop = true;
            radioButtonSB16.Text = "Creative Labs Sound Blaster 16";
            radioButtonSB16.UseVisualStyleBackColor = true;
            // 
            // groupBoxMemory
            // 
            groupBoxMemory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxMemory.Controls.Add(labelMinMemory);
            groupBoxMemory.Controls.Add(trackBarMinMemory);
            groupBoxMemory.Controls.Add(numericUpDownMinMemory);
            groupBoxMemory.Controls.Add(labelDefaultMemory);
            groupBoxMemory.Controls.Add(trackBarDefaultMemory);
            groupBoxMemory.Controls.Add(numericUpDownDefaultMemory);
            groupBoxMemory.Controls.Add(labelMaxMemory);
            groupBoxMemory.Controls.Add(trackBarMaxMemory);
            groupBoxMemory.Controls.Add(numericUpDownMaxMemory);
            groupBoxMemory.Location = new Point(8, 197);
            groupBoxMemory.Name = "groupBoxMemory";
            groupBoxMemory.Size = new Size(740, 365);
            groupBoxMemory.TabIndex = 3;
            groupBoxMemory.TabStop = false;
            groupBoxMemory.Text = "Memory";
            // 
            // labelMinMemory
            // 
            labelMinMemory.AutoSize = true;
            labelMinMemory.Location = new Point(19, 35);
            labelMinMemory.Name = "labelMinMemory";
            labelMinMemory.Size = new Size(224, 15);
            labelMinMemory.TabIndex = 0;
            labelMinMemory.Text = "Recommended minimum memory (MB):";
            // 
            // trackBarMinMemory
            // 
            trackBarMinMemory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarMinMemory.BackColor = Color.Transparent;
            trackBarMinMemory.Location = new Point(19, 65);
            trackBarMinMemory.Name = "trackBarMinMemory";
            trackBarMinMemory.Size = new Size(701, 45);
            trackBarMinMemory.TabIndex = 1;
            // 
            // numericUpDownMinMemory
            // 
            numericUpDownMinMemory.Location = new Point(272, 33);
            numericUpDownMinMemory.Name = "numericUpDownMinMemory";
            numericUpDownMinMemory.Size = new Size(100, 23);
            numericUpDownMinMemory.TabIndex = 2;
            // 
            // labelDefaultMemory
            // 
            labelDefaultMemory.AutoSize = true;
            labelDefaultMemory.Location = new Point(19, 135);
            labelDefaultMemory.Name = "labelDefaultMemory";
            labelDefaultMemory.Size = new Size(208, 15);
            labelDefaultMemory.TabIndex = 3;
            labelDefaultMemory.Text = "Recommended default memory (MB):";
            // 
            // trackBarDefaultMemory
            // 
            trackBarDefaultMemory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarDefaultMemory.BackColor = Color.Transparent;
            trackBarDefaultMemory.Location = new Point(19, 165);
            trackBarDefaultMemory.Name = "trackBarDefaultMemory";
            trackBarDefaultMemory.Size = new Size(701, 45);
            trackBarDefaultMemory.TabIndex = 4;
            // 
            // numericUpDownDefaultMemory
            // 
            numericUpDownDefaultMemory.Location = new Point(272, 133);
            numericUpDownDefaultMemory.Name = "numericUpDownDefaultMemory";
            numericUpDownDefaultMemory.Size = new Size(100, 23);
            numericUpDownDefaultMemory.TabIndex = 5;
            // 
            // labelMaxMemory
            // 
            labelMaxMemory.AutoSize = true;
            labelMaxMemory.Location = new Point(19, 235);
            labelMaxMemory.Name = "labelMaxMemory";
            labelMaxMemory.Size = new Size(225, 15);
            labelMaxMemory.TabIndex = 6;
            labelMaxMemory.Text = "Recommended maximum memory (MB):";
            // 
            // trackBarMaxMemory
            // 
            trackBarMaxMemory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarMaxMemory.BackColor = Color.Transparent;
            trackBarMaxMemory.Location = new Point(19, 265);
            trackBarMaxMemory.Name = "trackBarMaxMemory";
            trackBarMaxMemory.Size = new Size(701, 45);
            trackBarMaxMemory.TabIndex = 7;
            // 
            // numericUpDownMaxMemory
            // 
            numericUpDownMaxMemory.Location = new Point(272, 233);
            numericUpDownMaxMemory.Name = "numericUpDownMaxMemory";
            numericUpDownMaxMemory.Size = new Size(100, 23);
            numericUpDownMaxMemory.TabIndex = 8;
            // 
            // groupBoxProcessors
            // 
            groupBoxProcessors.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxProcessors.Controls.Add(labelTotalCoresNumber);
            groupBoxProcessors.Controls.Add(labelTotalCores);
            groupBoxProcessors.Controls.Add(labelNumberOfCoresPerProcessor);
            groupBoxProcessors.Controls.Add(labelNumberOfProcessors);
            groupBoxProcessors.Controls.Add(comboBoxNumberOfCoresPerProcessor);
            groupBoxProcessors.Controls.Add(comboBoxNumberOfProcessors);
            groupBoxProcessors.Location = new Point(234, 72);
            groupBoxProcessors.Name = "groupBoxProcessors";
            groupBoxProcessors.Size = new Size(514, 119);
            groupBoxProcessors.TabIndex = 2;
            groupBoxProcessors.TabStop = false;
            groupBoxProcessors.Text = "Processors";
            // 
            // labelTotalCoresNumber
            // 
            labelTotalCoresNumber.AutoSize = true;
            labelTotalCoresNumber.Location = new Point(208, 83);
            labelTotalCoresNumber.Name = "labelTotalCoresNumber";
            labelTotalCoresNumber.Size = new Size(71, 15);
            labelTotalCoresNumber.TabIndex = 3;
            labelTotalCoresNumber.Text = "{TotalCores}";
            // 
            // labelTotalCores
            // 
            labelTotalCores.AutoSize = true;
            labelTotalCores.Location = new Point(19, 83);
            labelTotalCores.Name = "labelTotalCores";
            labelTotalCores.Size = new Size(67, 15);
            labelTotalCores.TabIndex = 3;
            labelTotalCores.Text = "Total cores:";
            // 
            // labelNumberOfCoresPerProcessor
            // 
            labelNumberOfCoresPerProcessor.AutoSize = true;
            labelNumberOfCoresPerProcessor.Location = new Point(19, 53);
            labelNumberOfCoresPerProcessor.Name = "labelNumberOfCoresPerProcessor";
            labelNumberOfCoresPerProcessor.Size = new Size(173, 15);
            labelNumberOfCoresPerProcessor.TabIndex = 3;
            labelNumberOfCoresPerProcessor.Text = "Number of cores per processor:";
            // 
            // labelNumberOfProcessors
            // 
            labelNumberOfProcessors.AutoSize = true;
            labelNumberOfProcessors.Location = new Point(19, 23);
            labelNumberOfProcessors.Name = "labelNumberOfProcessors";
            labelNumberOfProcessors.Size = new Size(127, 15);
            labelNumberOfProcessors.TabIndex = 3;
            labelNumberOfProcessors.Text = "Number of processors:";
            // 
            // comboBoxNumberOfCoresPerProcessor
            // 
            comboBoxNumberOfCoresPerProcessor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxNumberOfCoresPerProcessor.FormattingEnabled = true;
            comboBoxNumberOfCoresPerProcessor.Location = new Point(208, 50);
            comboBoxNumberOfCoresPerProcessor.Name = "comboBoxNumberOfCoresPerProcessor";
            comboBoxNumberOfCoresPerProcessor.Size = new Size(300, 23);
            comboBoxNumberOfCoresPerProcessor.TabIndex = 2;
            // 
            // comboBoxNumberOfProcessors
            // 
            comboBoxNumberOfProcessors.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxNumberOfProcessors.Location = new Point(208, 20);
            comboBoxNumberOfProcessors.Name = "comboBoxNumberOfProcessors";
            comboBoxNumberOfProcessors.Size = new Size(300, 23);
            comboBoxNumberOfProcessors.TabIndex = 2;
            // 
            // groupBoxFirmware
            // 
            groupBoxFirmware.Controls.Add(checkBoxFirmwareForced);
            groupBoxFirmware.Controls.Add(radioButtonUEFI);
            groupBoxFirmware.Controls.Add(radioButtonBIOS);
            groupBoxFirmware.Location = new Point(8, 72);
            groupBoxFirmware.Name = "groupBoxFirmware";
            groupBoxFirmware.Size = new Size(220, 119);
            groupBoxFirmware.TabIndex = 1;
            groupBoxFirmware.TabStop = false;
            groupBoxFirmware.Text = "Firmware";
            // 
            // checkBoxFirmwareForced
            // 
            checkBoxFirmwareForced.AutoSize = true;
            checkBoxFirmwareForced.Location = new Point(15, 83);
            checkBoxFirmwareForced.Name = "checkBoxFirmwareForced";
            checkBoxFirmwareForced.Size = new Size(62, 19);
            checkBoxFirmwareForced.TabIndex = 1;
            checkBoxFirmwareForced.Text = "Forced";
            checkBoxFirmwareForced.UseVisualStyleBackColor = true;
            // 
            // radioButtonUEFI
            // 
            radioButtonUEFI.AutoSize = true;
            radioButtonUEFI.Location = new Point(15, 53);
            radioButtonUEFI.Name = "radioButtonUEFI";
            radioButtonUEFI.Size = new Size(48, 19);
            radioButtonUEFI.TabIndex = 0;
            radioButtonUEFI.TabStop = true;
            radioButtonUEFI.Text = "UEFI";
            radioButtonUEFI.UseVisualStyleBackColor = true;
            // 
            // radioButtonBIOS
            // 
            radioButtonBIOS.AutoSize = true;
            radioButtonBIOS.Location = new Point(15, 23);
            radioButtonBIOS.Name = "radioButtonBIOS";
            radioButtonBIOS.Size = new Size(50, 19);
            radioButtonBIOS.TabIndex = 0;
            radioButtonBIOS.TabStop = true;
            radioButtonBIOS.Text = "BIOS";
            radioButtonBIOS.UseVisualStyleBackColor = true;
            // 
            // tabPageOS
            // 
            tabPageOS.Controls.Add(groupBoxIsoMount);
            tabPageOS.Controls.Add(groupBoxOSDetails);
            tabPageOS.Location = new Point(4, 24);
            tabPageOS.Name = "tabPageOS";
            tabPageOS.Padding = new Padding(3);
            tabPageOS.Size = new Size(776, 336);
            tabPageOS.TabIndex = 3;
            tabPageOS.Text = "Operating System";
            tabPageOS.UseVisualStyleBackColor = true;
            // 
            // groupBoxIsoMount
            // 
            groupBoxIsoMount.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxIsoMount.Controls.Add(radioButtonIsoSATA);
            groupBoxIsoMount.Controls.Add(radioButtonIsoSCSI);
            groupBoxIsoMount.Controls.Add(radioButtonIsoIDE);
            groupBoxIsoMount.Controls.Add(labelIsoController);
            groupBoxIsoMount.Controls.Add(buttonIsoBrowse);
            groupBoxIsoMount.Controls.Add(textBoxIsoPath);
            groupBoxIsoMount.Controls.Add(labelIsoPath);
            groupBoxIsoMount.Location = new Point(8, 130);
            groupBoxIsoMount.Name = "groupBoxIsoMount";
            groupBoxIsoMount.Size = new Size(750, 130);
            groupBoxIsoMount.TabIndex = 1;
            groupBoxIsoMount.TabStop = false;
            groupBoxIsoMount.Text = "Installation Media (ISO)";
            // 
            // radioButtonIsoSATA
            // 
            radioButtonIsoSATA.AutoSize = true;
            radioButtonIsoSATA.Location = new Point(465, 76);
            radioButtonIsoSATA.Name = "radioButtonIsoSATA";
            radioButtonIsoSATA.Size = new Size(52, 19);
            radioButtonIsoSATA.TabIndex = 6;
            radioButtonIsoSATA.Text = "SATA";
            radioButtonIsoSATA.UseVisualStyleBackColor = true;
            // 
            // radioButtonIsoSCSI
            // 
            radioButtonIsoSCSI.AutoSize = true;
            radioButtonIsoSCSI.Location = new Point(340, 76);
            radioButtonIsoSCSI.Name = "radioButtonIsoSCSI";
            radioButtonIsoSCSI.Size = new Size(48, 19);
            radioButtonIsoSCSI.TabIndex = 5;
            radioButtonIsoSCSI.Text = "SCSI";
            radioButtonIsoSCSI.UseVisualStyleBackColor = true;
            // 
            // radioButtonIsoIDE
            // 
            radioButtonIsoIDE.AutoSize = true;
            radioButtonIsoIDE.Checked = true;
            radioButtonIsoIDE.Location = new Point(215, 76);
            radioButtonIsoIDE.Name = "radioButtonIsoIDE";
            radioButtonIsoIDE.Size = new Size(42, 19);
            radioButtonIsoIDE.TabIndex = 4;
            radioButtonIsoIDE.TabStop = true;
            radioButtonIsoIDE.Text = "IDE";
            radioButtonIsoIDE.UseVisualStyleBackColor = true;
            // 
            // labelIsoController
            // 
            labelIsoController.AutoSize = true;
            labelIsoController.Location = new Point(19, 78);
            labelIsoController.Name = "labelIsoController";
            labelIsoController.Size = new Size(131, 15);
            labelIsoController.TabIndex = 3;
            labelIsoController.Text = "ISO CD/DVD Controller:";
            // 
            // buttonIsoBrowse
            // 
            buttonIsoBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonIsoBrowse.Location = new Point(647, 31);
            buttonIsoBrowse.Name = "buttonIsoBrowse";
            buttonIsoBrowse.Size = new Size(78, 23);
            buttonIsoBrowse.TabIndex = 2;
            buttonIsoBrowse.Text = "Browse...";
            buttonIsoBrowse.UseVisualStyleBackColor = true;
            // 
            // textBoxIsoPath
            // 
            textBoxIsoPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxIsoPath.Location = new Point(215, 32);
            textBoxIsoPath.Name = "textBoxIsoPath";
            textBoxIsoPath.Size = new Size(426, 23);
            textBoxIsoPath.TabIndex = 1;
            // 
            // labelIsoPath
            // 
            labelIsoPath.AutoSize = true;
            labelIsoPath.Location = new Point(19, 35);
            labelIsoPath.Name = "labelIsoPath";
            labelIsoPath.Size = new Size(83, 15);
            labelIsoPath.TabIndex = 0;
            labelIsoPath.Text = "ISO image file:";
            // 
            // groupBoxOSDetails
            // 
            groupBoxOSDetails.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxOSDetails.Controls.Add(textBoxGuestOS);
            groupBoxOSDetails.Controls.Add(buttonOSMenu);
            groupBoxOSDetails.Controls.Add(labelGuestOSID);
            groupBoxOSDetails.Controls.Add(textBoxOSDisplayName);
            groupBoxOSDetails.Controls.Add(labelOSDisplayName);
            groupBoxOSDetails.Location = new Point(8, 6);
            groupBoxOSDetails.Name = "groupBoxOSDetails";
            groupBoxOSDetails.Size = new Size(750, 110);
            groupBoxOSDetails.TabIndex = 0;
            groupBoxOSDetails.TabStop = false;
            groupBoxOSDetails.Text = "Operating System Identification";
            // 
            // textBoxGuestOS
            // 
            textBoxGuestOS.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxGuestOS.Location = new Point(215, 65);
            textBoxGuestOS.Name = "textBoxGuestOS";
            textBoxGuestOS.Size = new Size(482, 23);
            textBoxGuestOS.TabIndex = 3;
            // 
            // buttonOSMenu
            // 
            buttonOSMenu.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonOSMenu.Location = new Point(703, 65);
            buttonOSMenu.Name = "buttonOSMenu";
            buttonOSMenu.Size = new Size(22, 23);
            buttonOSMenu.TabIndex = 4;
            buttonOSMenu.Text = ">";
            buttonOSMenu.UseVisualStyleBackColor = true;
            // 
            // labelGuestOSID
            // 
            labelGuestOSID.AutoSize = true;
            labelGuestOSID.Location = new Point(19, 68);
            labelGuestOSID.Name = "labelGuestOSID";
            labelGuestOSID.Size = new Size(129, 15);
            labelGuestOSID.TabIndex = 2;
            labelGuestOSID.Text = "Operating system type:";
            // 
            // textBoxOSDisplayName
            // 
            textBoxOSDisplayName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxOSDisplayName.Location = new Point(215, 27);
            textBoxOSDisplayName.Name = "textBoxOSDisplayName";
            textBoxOSDisplayName.Size = new Size(510, 23);
            textBoxOSDisplayName.TabIndex = 1;
            // 
            // labelOSDisplayName
            // 
            labelOSDisplayName.AutoSize = true;
            labelOSDisplayName.Location = new Point(19, 30);
            labelOSDisplayName.Name = "labelOSDisplayName";
            labelOSDisplayName.Size = new Size(176, 15);
            labelOSDisplayName.TabIndex = 0;
            labelOSDisplayName.Text = "Operating system display name:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 441);
            Controls.Add(tabControl1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(800, 480);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPageGeneral.ResumeLayout(false);
            tabPageGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).EndInit();
            tabPageUI.ResumeLayout(false);
            tabPageUI.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSelectedImage).EndInit();
            tabPageHardware.ResumeLayout(false);
            groupBoxHardwareVersion.ResumeLayout(false);
            groupBoxHardwareVersion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHardwareVersion).EndInit();
            groupBoxOpticalAndFloppy.ResumeLayout(false);
            groupBoxOpticalAndFloppy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownFloppyCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownIDECDCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownSCSICDCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownSATACDCount).EndInit();
            groupBoxHardDrive.ResumeLayout(false);
            groupBoxHardDrive.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHardDriveCapacity).EndInit();
            groupBoxSCSIController.ResumeLayout(false);
            groupBoxSCSIController.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBoxSoundCard.ResumeLayout(false);
            groupBoxSoundCard.PerformLayout();
            groupBoxMemory.ResumeLayout(false);
            groupBoxMemory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMinMemory).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDefaultMemory).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMaxMemory).EndInit();
            groupBoxProcessors.ResumeLayout(false);
            groupBoxProcessors.PerformLayout();
            groupBoxFirmware.ResumeLayout(false);
            groupBoxFirmware.PerformLayout();
            tabPageOS.ResumeLayout(false);
            groupBoxIsoMount.ResumeLayout(false);
            groupBoxIsoMount.PerformLayout();
            groupBoxOSDetails.ResumeLayout(false);
            groupBoxOSDetails.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStrip toolStrip1;
        private StatusStrip statusStrip1;
        private TabControl tabControl1;
        private TabPage tabPageGeneral;
        private TabPage tabPageUI;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsTemplateMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem buildToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem customizeToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem contentsToolStripMenuItem;
        private ToolStripMenuItem indexToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripButton newToolStripButton;
        private ToolStripButton openToolStripButton;
        private ToolStripButton buildToolStripButton;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton helpToolStripButton;
        private TabPage tabPageHardware;
        private TabPage tabPageOS;
        private Label labelVMName;
        private Button buttonVMLocationBrowse;
        private Button buttonVMLocationVariables;
        private TextBox textBoxVMLocation;
        private TextBox textBoxVMName;
        private Label labelVirtualMachineIcon;
        private Label labelVMLocation;
        private PictureBox pictureBoxIcon;
        private Button buttonChangeIcon;
        private ComboBox comboBoxLanguage;
        private Label labelLanguage;
        private Label labelTextFont;
        private Label labelTextColor;
        private Button buttonTextColor;
        private ComboBox comboBoxTextFont;
        private Label labelImageAssets;
        private ListBox listBoxImageAssets;
        private Button buttonSelectImageAsset;
        private Label labelSelectedImageName;
        private Label labelSelectedImageSize;
        private PictureBox pictureBoxSelectedImage;
        private Label labelImagePreview;
        private FontDialog fontDialog1;
        private ColorDialog colorDialog1;
        private GroupBox groupBoxFirmware;
        private CheckBox checkBoxFirmwareForced;
        private RadioButton radioButtonUEFI;
        private RadioButton radioButtonBIOS;
        private GroupBox groupBoxProcessors;
        private ComboBox comboBoxNumberOfCoresPerProcessor;
        private ComboBox comboBoxNumberOfProcessors;
        private Label labelNumberOfCoresPerProcessor;
        private Label labelNumberOfProcessors;
        private Label labelTotalCores;
        private Label labelTotalCoresNumber;
        private GroupBox groupBoxMemory;
        private Label labelMinMemory;
        private CustomTrackBar trackBarMinMemory;
        private NumericUpDown numericUpDownMinMemory;
        private Label labelDefaultMemory;
        private CustomTrackBar trackBarDefaultMemory;
        private NumericUpDown numericUpDownDefaultMemory;
        private Label labelMaxMemory;
        private CustomTrackBar trackBarMaxMemory;
        private NumericUpDown numericUpDownMaxMemory;
        private GroupBox groupBoxSoundCard;
        private RadioButton radioButtonSB16;
        private RadioButton radioButtonES1371;
        private RadioButton radioButtonHDAudio;
        private CheckBox checkBoxHDAudio;
        private CheckBox checkBoxES1371;
        private CheckBox checkBoxSB16;
        private GroupBox groupBoxSCSIController;
        private CheckBox checkBoxBusLogic;
        private CheckBox checkBoxLSILogic;
        private CheckBox checkBoxLSILogicSAS;
        private CheckBox checkBoxParavirtualized;
        private RadioButton radioButtonBusLogic;
        private RadioButton radioButtonLSILogic;
        private RadioButton radioButtonLSILogicSAS;
        private RadioButton radioButtonParavirtualized;
        private GroupBox groupBoxHardDrive;
        private CheckBox checkBoxIDE;
        private CheckBox checkBoxSCSI;
        private CheckBox checkBoxSATA;
        private CheckBox checkBoxNVMe;
        private RadioButton radioButtonIDE;
        private RadioButton radioButtonSCSI;
        private RadioButton radioButtonSATA;
        private RadioButton radioButtonNVMe;
        private NumericUpDown numericUpDownHardDriveCapacity;
        private Label labelHardDriveCapacity;
        private GroupBox groupBoxOpticalAndFloppy;
        private Label labelFloppyCount;
        private NumericUpDown numericUpDownFloppyCount;
        private Label labelIDECDCount;
        private NumericUpDown numericUpDownIDECDCount;
        private Label labelSCSICDCount;
        private NumericUpDown numericUpDownSCSICDCount;
        private Label labelSATACDCount;
        private NumericUpDown numericUpDownSATACDCount;
        private GroupBox groupBoxHardwareVersion;
        private Label labelHardwareVersion;
        private NumericUpDown numericUpDownHardwareVersion;
        private GroupBox groupBoxOSDetails;

        private TextBox textBoxGuestOS;
        private Button buttonOSMenu;

        private Label labelGuestOSID;
        private TextBox textBoxOSDisplayName;
        private Label labelOSDisplayName;
        private GroupBox groupBoxIsoMount;
        private RadioButton radioButtonIsoSATA;
        private RadioButton radioButtonIsoSCSI;
        private RadioButton radioButtonIsoIDE;
        private Label labelIsoController;
        private Button buttonIsoBrowse;
        private TextBox textBoxIsoPath;
        private Label labelIsoPath;
        private GroupBox groupBox1;
        private CheckBox checkBoxE1000PCI;
        private CheckBox checkBoxE1000PCIe;
        private CheckBox checkBoxPCNet32;
        private CheckBox checkBoxradioButtonVMXNET3;
        private RadioButton radioButtonE1000PCI;
        private RadioButton radioButtonE1000PCIe;
        private RadioButton radioButtonPCNet32;
        private RadioButton radioButtonradioButtonVMXNET3;
        private ToolStripButton saveToolStripButton;
    }


}