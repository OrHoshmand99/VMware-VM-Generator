using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace VMware_VM_Generator
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            toolTip1 = new ToolTip(components);
            radioButtonBridged = new RadioButton();
            radioButtonNAT = new RadioButton();
            radioButtonHostOnly = new RadioButton();
            numericUpDownFloppyCount = new NumericUpDown();
            numericUpDownCDIDECount = new NumericUpDown();
            numericUpDownCDSCSICount = new NumericUpDown();
            numericUpDownCDSATACount = new NumericUpDown();
            iconFirmware = new PictureBox();
            iconProcessor = new PictureBox();
            iconMemory = new PictureBox();
            iconSound = new PictureBox();
            iconSCSI = new PictureBox();
            iconNetwork = new PictureBox();
            iconFloppy = new PictureBox();
            iconCDIDE = new PictureBox();
            iconCDSCSI = new PictureBox();
            iconCDSATA = new PictureBox();
            panelWelcome = new Panel();
            btnWelcomeBack = new CustomButton();
            btnWelcomeNext = new CustomButton();
            btnWelcomeCancel = new CustomButton();
            labelWelcomeText = new Label();
            labelWelcomeTitle = new Label();
            panelWelcomeLeft = new Panel();
            panelWelcomeTop = new Panel();
            pictureBoxWelcomeTopRight = new PictureBox();
            panelWelcomeBottom = new Panel();
            panelWelcomeBackground = new Panel();
            iconWelcome = new PictureBox();
            panelStep1 = new Panel();
            labelStep1Text = new Label();
            labelStep1Title = new Label();
            btnStep1Back = new CustomButton();
            btnStep1Next = new CustomButton();
            btnStep1Cancel = new CustomButton();
            btnBrowseFolder = new CustomButton();
            labelVMFolder = new Label();
            txtVMFolder = new TextBox();
            txtVMName = new TextBox();
            labelVMName = new Label();
            pictureBoxStep1Top = new PictureBox();
            pictureBoxStep1Bottom = new PictureBox();
            pictureBoxStep1Left = new PictureBox();
            panelStep1Left = new Panel();
            panelStep1Top = new Panel();
            pictureBoxStep1TopRight = new PictureBox();
            panelStep1Bottom = new Panel();
            pictureBoxStep1Background = new PictureBox();
            panelStep1Background = new Panel();
            iconFolder = new PictureBox();
            panelStep2 = new Panel();
            labelStep2Text = new Label();
            labelStep2Title = new Label();
            processors = new GroupBox();
            comboBoxNumberOfCoresPerProcessor = new ComboBox();
            comboNumberOfProcessors = new ComboBox();
            labelNumberOfCoresPerProcessor = new Label();
            labelTotalOfCoresX = new Label();
            labelTotalOfCores = new Label();
            labelNumberOfProcessors = new Label();
            memory = new GroupBox();
            trackBarMemory = new CustomTrackBar();
            numericUpDownMemory = new NumericUpDown();
            labelMemory = new Label();
            labelRecommendedMin = new Label();
            labelRecommendedDefault = new Label();
            labelRecommendedMax = new Label();
            firmwareType = new GroupBox();
            uefi = new RadioButton();
            bios = new RadioButton();
            btnStep2Back = new CustomButton();
            btnStep2Next = new CustomButton();
            btnStep2Cancel = new CustomButton();
            pictureBoxStep2Top = new PictureBox();
            pictureBoxStep2Bottom = new PictureBox();
            pictureBoxStep2Left = new PictureBox();
            pictureBoxStep2Background = new PictureBox();
            panelStep2Left = new Panel();
            panelStep2Top = new Panel();
            pictureBoxStep2TopRight = new PictureBox();
            panelStep2Bottom = new Panel();
            panelStep2Background = new Panel();
            panelStep3 = new Panel();
            labelStep3Text = new Label();
            labelStep3Title = new Label();
            groupBoxSoundCard = new GroupBox();
            radioButtonHDAudio = new RadioButton();
            radioButtonES1371 = new RadioButton();
            radioButtonSB16 = new RadioButton();
            groupBoxSCSIIO = new GroupBox();
            radioParavirtualizedSCSI = new RadioButton();
            radioLSILogicSAS = new RadioButton();
            radioLSILogic = new RadioButton();
            radioBusLogic = new RadioButton();
            groupBoxNetwork = new GroupBox();
            radioButtonNoNetwork = new RadioButton();
            btnStep3Back = new CustomButton();
            btnStep3Next = new CustomButton();
            btnStep3Cancel = new CustomButton();
            pictureBoxStep3Top = new PictureBox();
            pictureBoxStep3Bottom = new PictureBox();
            pictureBoxStep3Left = new PictureBox();
            pictureBoxStep3Background = new PictureBox();
            panelStep3Left = new Panel();
            panelStep3Top = new Panel();
            pictureBoxStep3TopRight = new PictureBox();
            panelStep3Bottom = new Panel();
            panelStep3Background = new Panel();
            panelStep4 = new Panel();
            labelStep4Text = new Label();
            labelStep4Title = new Label();
            radioButtonSplit = new RadioButton();
            radioButtonSingle = new RadioButton();
            labelSplit = new Label();
            labelAllocate = new Label();
            checkBoxAllocate = new CheckBox();
            numericUpDownDriveCapacity = new NumericUpDown();
            labelRecommendedDriveCapacity = new Label();
            labelDriveCapacity = new Label();
            groupBoxVirtualDriveType = new GroupBox();
            radioButtonNVMe = new RadioButton();
            radioButtonSATA = new RadioButton();
            radioButtonSCSI = new RadioButton();
            radioButtonIDE = new RadioButton();
            btnStep4Back = new CustomButton();
            btnStep4Next = new CustomButton();
            btnStep4Cancel = new CustomButton();
            pictureBoxStep4Top = new PictureBox();
            pictureBoxStep4Bottom = new PictureBox();
            pictureBoxStep4Left = new PictureBox();
            panelStep4Left = new Panel();
            panelStep4Top = new Panel();
            pictureBoxStep4TopRight = new PictureBox();
            panelStep4Bottom = new Panel();
            pictureBoxStep4Background = new PictureBox();
            panelStep4Background = new Panel();
            iconHardDrive = new PictureBox();
            panelStep5 = new Panel();
            labelStep5Text = new Label();
            labelStep5Title = new Label();
            labelCDLimit = new Label();
            labelUsedCDController = new Label();
            labelNumberOfCDSATA = new Label();
            labelNumberOfCDSCSI = new Label();
            labelNumberOfCDIDE = new Label();
            labelNumberOfFloppy = new Label();
            btnStep5Back = new CustomButton();
            btnStep5Next = new CustomButton();
            btnStep5Cancel = new CustomButton();
            pictureBoxStep5Top = new PictureBox();
            pictureBoxStep5Bottom = new PictureBox();
            pictureBoxStep5Left = new PictureBox();
            pictureBoxStep5Background = new PictureBox();
            panelStep5Left = new Panel();
            panelStep5Top = new Panel();
            pictureBoxStep5TopRight = new PictureBox();
            panelStep5Bottom = new Panel();
            panelStep5Background = new Panel();
            panelStep6 = new Panel();
            labelStep6Text = new Label();
            labelStep6Title = new Label();
            pictureVMIcon = new PictureBox();
            labelVMIcon = new Label();
            groupBoxShortcutType = new GroupBox();
            radioShortcutDirect = new RadioButton();
            radioShortcutKvm = new RadioButton();
            radioShortcutWorkstation = new RadioButton();
            checkBoxStartMenu = new CheckBox();
            checkBoxFullScreen = new CheckBox();
            checkBoxClose = new CheckBox();
            checkBoxShortStart = new CheckBox();
            checkBoxNewWindow = new CheckBox();
            checkBoxDesktop = new CheckBox();
            buttonChangeIcon = new CustomButton();
            btnStep6Back = new CustomButton();
            btnStep6Next = new CustomButton();
            btnStep6Cancel = new CustomButton();
            pictureBoxStep6Top = new PictureBox();
            pictureBoxStep6Bottom = new PictureBox();
            pictureBoxStep6Left = new PictureBox();
            pictureBoxStep6Background = new PictureBox();
            panelStep6Left = new Panel();
            panelStep6Top = new Panel();
            pictureBoxStep6TopRight = new PictureBox();
            panelStep6Bottom = new Panel();
            panelStep6Background = new Panel();
            panelStep7 = new Panel();
            SummaryTitleTemplate = new Label();
            labelSummaryText = new Label();
            labelSummaryTitle = new Label();
            txtSummary = new TextBox();
            btnStep7Back = new CustomButton();
            btnCreate = new CustomButton();
            btnStep7Cancel = new CustomButton();
            pictureBoxStep7Top = new PictureBox();
            pictureBoxStep7Bottom = new PictureBox();
            pictureBoxStep7Left = new PictureBox();
            pictureBoxStep7Background = new PictureBox();
            panelStep7Left = new Panel();
            panelStep7Top = new Panel();
            pictureBoxStep7TopRight = new PictureBox();
            panelStep7Bottom = new Panel();
            panelStep7Background = new Panel();
            panelFinish = new Panel();
            checkBoxOpenFolder = new CheckBox();
            checkBoxStartVM = new CheckBox();
            buttonFinish = new CustomButton();
            labelFinishText = new Label();
            labelFinishTitle = new Label();
            pictureBoxFinishTop = new PictureBox();
            pictureBoxFinishBottom = new PictureBox();
            pictureBoxFinishLeft = new PictureBox();
            pictureBoxFinishBackground = new PictureBox();
            panelFinishLeft = new Panel();
            panelFinishTop = new Panel();
            pictureBoxFinishTopRight = new PictureBox();
            panelFinishBottom = new Panel();
            panelFinishBackground = new Panel();
            iconFinish = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDownFloppyCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCDIDECount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCDSCSICount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCDSATACount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconFirmware).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconProcessor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconMemory).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconSound).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconSCSI).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconNetwork).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconFloppy).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconCDIDE).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconCDSCSI).BeginInit();
            ((System.ComponentModel.ISupportInitialize)iconCDSATA).BeginInit();
            panelWelcome.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxWelcomeTopRight).BeginInit();
            panelWelcomeBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconWelcome).BeginInit();
            panelStep1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1Top).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1Bottom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1Left).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1TopRight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1Background).BeginInit();
            panelStep1Background.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconFolder).BeginInit();
            panelStep2.SuspendLayout();
            processors.SuspendLayout();
            memory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMemory).BeginInit();
            firmwareType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2Top).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2Bottom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2Left).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2Background).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2TopRight).BeginInit();
            panelStep3.SuspendLayout();
            groupBoxSoundCard.SuspendLayout();
            groupBoxSCSIIO.SuspendLayout();
            groupBoxNetwork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3Top).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3Bottom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3Left).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3Background).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3TopRight).BeginInit();
            panelStep4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDriveCapacity).BeginInit();
            groupBoxVirtualDriveType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4Top).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4Bottom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4Left).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4TopRight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4Background).BeginInit();
            panelStep4Background.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconHardDrive).BeginInit();
            panelStep5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5Top).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5Bottom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5Left).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5Background).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5TopRight).BeginInit();
            panelStep6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureVMIcon).BeginInit();
            groupBoxShortcutType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6Top).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6Bottom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6Left).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6Background).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6TopRight).BeginInit();
            panelStep7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7Top).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7Bottom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7Left).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7Background).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7TopRight).BeginInit();
            panelFinish.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishTop).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishBottom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishLeft).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishBackground).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishTopRight).BeginInit();
            panelFinishBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconFinish).BeginInit();
            SuspendLayout();
            // 
            // radioButtonBridged
            // 
            radioButtonBridged.Location = new Point(15, 25);
            radioButtonBridged.Name = "radioButtonBridged";
            radioButtonBridged.Size = new Size(150, 24);
            radioButtonBridged.TabIndex = 0;
            radioButtonBridged.TabStop = true;
            radioButtonBridged.Text = "Use bridge networking";
            toolTip1.SetToolTip(radioButtonBridged, "Give the guest operating system direct access to an external Ethernet network.");
            radioButtonBridged.UseVisualStyleBackColor = false;
            // 
            // radioButtonNAT
            // 
            radioButtonNAT.Location = new Point(247, 25);
            radioButtonNAT.Name = "radioButtonNAT";
            radioButtonNAT.Size = new Size(228, 24);
            radioButtonNAT.TabIndex = 0;
            radioButtonNAT.TabStop = true;
            radioButtonNAT.Text = "Use network address translation (NAT)";
            toolTip1.SetToolTip(radioButtonNAT, "Give the guest operating system access to host connection.");
            radioButtonNAT.UseVisualStyleBackColor = false;
            // 
            // radioButtonHostOnly
            // 
            radioButtonHostOnly.Location = new Point(15, 52);
            radioButtonHostOnly.Name = "radioButtonHostOnly";
            radioButtonHostOnly.Size = new Size(150, 24);
            radioButtonHostOnly.TabIndex = 0;
            radioButtonHostOnly.TabStop = true;
            radioButtonHostOnly.Text = "Use host-only networking";
            toolTip1.SetToolTip(radioButtonHostOnly, "Connect to private virtual network.");
            radioButtonHostOnly.UseVisualStyleBackColor = false;
            // 
            // numericUpDownFloppyCount
            // 
            numericUpDownFloppyCount.Location = new Point(432, 214);
            numericUpDownFloppyCount.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            numericUpDownFloppyCount.Name = "numericUpDownFloppyCount";
            numericUpDownFloppyCount.Size = new Size(60, 23);
            numericUpDownFloppyCount.TabIndex = 4;
            // 
            // numericUpDownCDIDECount
            // 
            numericUpDownCDIDECount.Location = new Point(432, 251);
            numericUpDownCDIDECount.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            numericUpDownCDIDECount.Name = "numericUpDownCDIDECount";
            numericUpDownCDIDECount.Size = new Size(60, 23);
            numericUpDownCDIDECount.TabIndex = 5;
            // 
            // numericUpDownCDSCSICount
            // 
            numericUpDownCDSCSICount.Location = new Point(432, 286);
            numericUpDownCDSCSICount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownCDSCSICount.Name = "numericUpDownCDSCSICount";
            numericUpDownCDSCSICount.Size = new Size(60, 23);
            numericUpDownCDSCSICount.TabIndex = 6;
            // 
            // numericUpDownCDSATACount
            // 
            numericUpDownCDSATACount.Location = new Point(432, 323);
            numericUpDownCDSATACount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownCDSATACount.Name = "numericUpDownCDSATACount";
            numericUpDownCDSATACount.Size = new Size(60, 23);
            numericUpDownCDSATACount.TabIndex = 7;
            // 
            // iconFirmware
            // 
            iconFirmware.Location = new Point(152, 1);
            iconFirmware.Name = "iconFirmware";
            iconFirmware.Size = new Size(16, 16);
            iconFirmware.TabIndex = 0;
            iconFirmware.TabStop = false;
            // 
            // iconProcessor
            // 
            iconProcessor.Location = new Point(358, 1);
            iconProcessor.Name = "iconProcessor";
            iconProcessor.Size = new Size(16, 16);
            iconProcessor.TabIndex = 0;
            iconProcessor.TabStop = false;
            // 
            // iconMemory
            // 
            iconMemory.Location = new Point(538, 0);
            iconMemory.Name = "iconMemory";
            iconMemory.Size = new Size(16, 16);
            iconMemory.TabIndex = 0;
            iconMemory.TabStop = false;
            // 
            // iconSound
            // 
            iconSound.Location = new Point(182, 0);
            iconSound.Name = "iconSound";
            iconSound.Size = new Size(16, 16);
            iconSound.TabIndex = 0;
            iconSound.TabStop = false;
            // 
            // iconSCSI
            // 
            iconSCSI.Location = new Point(339, 1);
            iconSCSI.Name = "iconSCSI";
            iconSCSI.Size = new Size(16, 16);
            iconSCSI.TabIndex = 0;
            iconSCSI.TabStop = false;
            // 
            // iconNetwork
            // 
            iconNetwork.Location = new Point(549, 0);
            iconNetwork.Name = "iconNetwork";
            iconNetwork.Size = new Size(16, 16);
            iconNetwork.TabIndex = 0;
            iconNetwork.TabStop = false;
            // 
            // iconFloppy
            // 
            iconFloppy.BackColor = Color.Transparent;
            iconFloppy.Location = new Point(185, 214);
            iconFloppy.Name = "iconFloppy";
            iconFloppy.Size = new Size(16, 16);
            iconFloppy.TabIndex = 0;
            iconFloppy.TabStop = false;
            // 
            // iconCDIDE
            // 
            iconCDIDE.BackColor = Color.Transparent;
            iconCDIDE.Location = new Point(185, 250);
            iconCDIDE.Name = "iconCDIDE";
            iconCDIDE.Size = new Size(16, 16);
            iconCDIDE.TabIndex = 0;
            iconCDIDE.TabStop = false;
            // 
            // iconCDSCSI
            // 
            iconCDSCSI.BackColor = Color.Transparent;
            iconCDSCSI.Location = new Point(185, 288);
            iconCDSCSI.Name = "iconCDSCSI";
            iconCDSCSI.Size = new Size(16, 16);
            iconCDSCSI.TabIndex = 0;
            iconCDSCSI.TabStop = false;
            // 
            // iconCDSATA
            // 
            iconCDSATA.BackColor = Color.Transparent;
            iconCDSATA.Location = new Point(185, 325);
            iconCDSATA.Name = "iconCDSATA";
            iconCDSATA.Size = new Size(16, 16);
            iconCDSATA.TabIndex = 0;
            iconCDSATA.TabStop = false;
            // 
            // panelWelcome
            // 
            panelWelcome.Controls.Add(btnWelcomeBack);
            panelWelcome.Controls.Add(btnWelcomeNext);
            panelWelcome.Controls.Add(btnWelcomeCancel);
            panelWelcome.Controls.Add(labelWelcomeText);
            panelWelcome.Controls.Add(labelWelcomeTitle);
            panelWelcome.Controls.Add(panelWelcomeLeft);
            panelWelcome.Controls.Add(panelWelcomeTop);
            panelWelcome.Controls.Add(pictureBoxWelcomeTopRight);
            panelWelcome.Controls.Add(panelWelcomeBottom);
            panelWelcome.Controls.Add(panelWelcomeBackground);
            panelWelcome.Dock = DockStyle.Fill;
            panelWelcome.Location = new Point(0, 0);
            panelWelcome.Name = "panelWelcome";
            panelWelcome.Size = new Size(707, 513);
            panelWelcome.TabIndex = 0;
            // 
            // btnWelcomeBack
            // 
            btnWelcomeBack.BackColor = Color.FromArgb(245, 245, 245);
            btnWelcomeBack.FlatStyle = FlatStyle.Flat;
            btnWelcomeBack.ForeColor = Color.FromArgb(30, 30, 30);
            btnWelcomeBack.Location = new Point(435, 478);
            btnWelcomeBack.Name = "btnWelcomeBack";
            btnWelcomeBack.Size = new Size(75, 23);
            btnWelcomeBack.TabIndex = 0;
            btnWelcomeBack.Text = "< Back";
            btnWelcomeBack.UseVisualStyleBackColor = true;
            btnWelcomeBack.Visible = false;
            // 
            // btnWelcomeNext
            // 
            btnWelcomeNext.BackColor = Color.FromArgb(245, 245, 245);
            btnWelcomeNext.FlatStyle = FlatStyle.Flat;
            btnWelcomeNext.ForeColor = Color.FromArgb(30, 30, 30);
            btnWelcomeNext.Location = new Point(516, 478);
            btnWelcomeNext.Name = "btnWelcomeNext";
            btnWelcomeNext.Size = new Size(75, 23);
            btnWelcomeNext.TabIndex = 1;
            btnWelcomeNext.Text = "Next >";
            btnWelcomeNext.UseVisualStyleBackColor = true;
            // 
            // btnWelcomeCancel
            // 
            btnWelcomeCancel.BackColor = Color.FromArgb(245, 245, 245);
            btnWelcomeCancel.FlatStyle = FlatStyle.Flat;
            btnWelcomeCancel.ForeColor = Color.FromArgb(30, 30, 30);
            btnWelcomeCancel.Location = new Point(620, 478);
            btnWelcomeCancel.Name = "btnWelcomeCancel";
            btnWelcomeCancel.Size = new Size(75, 23);
            btnWelcomeCancel.TabIndex = 2;
            btnWelcomeCancel.Text = "Cancel";
            btnWelcomeCancel.UseVisualStyleBackColor = true;
            // 
            // labelWelcomeText
            // 
            labelWelcomeText.BackColor = Color.Transparent;
            labelWelcomeText.Location = new Point(240, 186);
            labelWelcomeText.Name = "labelWelcomeText";
            labelWelcomeText.Size = new Size(385, 43);
            labelWelcomeText.TabIndex = 3;
            labelWelcomeText.Text = "This wizard will guide you through creating a virtual machine with {osName}.";
            // 
            // labelWelcomeTitle
            // 
            labelWelcomeTitle.BackColor = Color.Transparent;
            labelWelcomeTitle.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            labelWelcomeTitle.Location = new Point(231, 96);
            labelWelcomeTitle.Name = "labelWelcomeTitle";
            labelWelcomeTitle.Size = new Size(464, 82);
            labelWelcomeTitle.TabIndex = 4;
            labelWelcomeTitle.Text = "Welcome to the VMware VM Generator wizard of {osName}.";
            // 
            // panelWelcomeLeft
            // 
            panelWelcomeLeft.Location = new Point(0, 85);
            panelWelcomeLeft.Name = "panelWelcomeLeft";
            panelWelcomeLeft.Size = new Size(118, 369);
            panelWelcomeLeft.TabIndex = 6;
            // 
            // panelWelcomeTop
            // 
            panelWelcomeTop.Location = new Point(0, 0);
            panelWelcomeTop.Name = "panelWelcomeTop";
            panelWelcomeTop.Size = new Size(707, 85);
            panelWelcomeTop.TabIndex = 6;
            // 
            // pictureBoxWelcomeTopRight
            // 
            pictureBoxWelcomeTopRight.BackColor = Color.Transparent;
            pictureBoxWelcomeTopRight.Location = new Point(622, 0);
            pictureBoxWelcomeTopRight.Name = "pictureBoxWelcomeTopRight";
            pictureBoxWelcomeTopRight.Size = new Size(85, 85);
            pictureBoxWelcomeTopRight.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxWelcomeTopRight.TabIndex = 4;
            pictureBoxWelcomeTopRight.TabStop = false;
            // 
            // panelWelcomeBottom
            // 
            panelWelcomeBottom.Location = new Point(0, 454);
            panelWelcomeBottom.Name = "panelWelcomeBottom";
            panelWelcomeBottom.Size = new Size(707, 59);
            panelWelcomeBottom.TabIndex = 6;
            // 
            // panelWelcomeBackground
            // 
            panelWelcomeBackground.Controls.Add(iconWelcome);
            panelWelcomeBackground.Location = new Point(118, 85);
            panelWelcomeBackground.Name = "panelWelcomeBackground";
            panelWelcomeBackground.Size = new Size(589, 369);
            panelWelcomeBackground.TabIndex = 6;
            // 
            // iconWelcome
            // 
            iconWelcome.Location = new Point(48, 11);
            iconWelcome.Name = "iconWelcome";
            iconWelcome.Size = new Size(32, 32);
            iconWelcome.TabIndex = 0;
            iconWelcome.TabStop = false;
            // 
            // panelStep1
            // 
            panelStep1.Controls.Add(labelStep1Text);
            panelStep1.Controls.Add(labelStep1Title);
            panelStep1.Controls.Add(btnStep1Back);
            panelStep1.Controls.Add(btnStep1Next);
            panelStep1.Controls.Add(btnStep1Cancel);
            panelStep1.Controls.Add(btnBrowseFolder);
            panelStep1.Controls.Add(labelVMFolder);
            panelStep1.Controls.Add(txtVMFolder);
            panelStep1.Controls.Add(txtVMName);
            panelStep1.Controls.Add(labelVMName);
            panelStep1.Controls.Add(pictureBoxStep1Top);
            panelStep1.Controls.Add(pictureBoxStep1Bottom);
            panelStep1.Controls.Add(pictureBoxStep1Left);
            panelStep1.Controls.Add(panelStep1Left);
            panelStep1.Controls.Add(panelStep1Top);
            panelStep1.Controls.Add(pictureBoxStep1TopRight);
            panelStep1.Controls.Add(panelStep1Bottom);
            panelStep1.Controls.Add(pictureBoxStep1Background);
            panelStep1.Controls.Add(panelStep1Background);
            panelStep1.Dock = DockStyle.Fill;
            panelStep1.Location = new Point(0, 0);
            panelStep1.Name = "panelStep1";
            panelStep1.Size = new Size(707, 513);
            panelStep1.TabIndex = 1;
            panelStep1.Visible = false;
            // 
            // labelStep1Text
            // 
            labelStep1Text.BackColor = Color.Transparent;
            labelStep1Text.Location = new Point(151, 117);
            labelStep1Text.Name = "labelStep1Text";
            labelStep1Text.Size = new Size(374, 20);
            labelStep1Text.TabIndex = 3;
            labelStep1Text.Text = "Specify a name and location for the virtual machine.";
            // 
            // labelStep1Title
            // 
            labelStep1Title.BackColor = Color.Transparent;
            labelStep1Title.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelStep1Title.Location = new Point(136, 96);
            labelStep1Title.Name = "labelStep1Title";
            labelStep1Title.Size = new Size(464, 21);
            labelStep1Title.TabIndex = 4;
            labelStep1Title.Text = "Virtual Machine Name and Location";
            // 
            // btnStep1Back
            // 
            btnStep1Back.BackColor = Color.FromArgb(245, 245, 245);
            btnStep1Back.FlatStyle = FlatStyle.Flat;
            btnStep1Back.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep1Back.Location = new Point(435, 478);
            btnStep1Back.Name = "btnStep1Back";
            btnStep1Back.Size = new Size(75, 23);
            btnStep1Back.TabIndex = 0;
            btnStep1Back.Text = "< Back";
            btnStep1Back.UseVisualStyleBackColor = true;
            // 
            // btnStep1Next
            // 
            btnStep1Next.BackColor = Color.FromArgb(245, 245, 245);
            btnStep1Next.FlatStyle = FlatStyle.Flat;
            btnStep1Next.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep1Next.Location = new Point(516, 478);
            btnStep1Next.Name = "btnStep1Next";
            btnStep1Next.Size = new Size(75, 23);
            btnStep1Next.TabIndex = 1;
            btnStep1Next.Text = "Next >";
            btnStep1Next.UseVisualStyleBackColor = true;
            // 
            // btnStep1Cancel
            // 
            btnStep1Cancel.BackColor = Color.FromArgb(245, 245, 245);
            btnStep1Cancel.FlatStyle = FlatStyle.Flat;
            btnStep1Cancel.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep1Cancel.Location = new Point(620, 478);
            btnStep1Cancel.Name = "btnStep1Cancel";
            btnStep1Cancel.Size = new Size(75, 23);
            btnStep1Cancel.TabIndex = 2;
            btnStep1Cancel.Text = "Cancel";
            btnStep1Cancel.UseVisualStyleBackColor = true;
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.BackColor = Color.FromArgb(245, 245, 245);
            btnBrowseFolder.FlatStyle = FlatStyle.Flat;
            btnBrowseFolder.ForeColor = Color.FromArgb(30, 30, 30);
            btnBrowseFolder.Location = new Point(574, 218);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(75, 23);
            btnBrowseFolder.TabIndex = 3;
            btnBrowseFolder.Text = "Browse...";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            // 
            // labelVMFolder
            // 
            labelVMFolder.AutoSize = true;
            labelVMFolder.BackColor = Color.Transparent;
            labelVMFolder.Location = new Point(152, 221);
            labelVMFolder.Name = "labelVMFolder";
            labelVMFolder.Size = new Size(139, 15);
            labelVMFolder.TabIndex = 4;
            labelVMFolder.Text = "Virtual machine location:";
            // 
            // txtVMFolder
            // 
            txtVMFolder.Location = new Point(306, 218);
            txtVMFolder.Name = "txtVMFolder";
            txtVMFolder.Size = new Size(262, 23);
            txtVMFolder.TabIndex = 5;
            // 
            // txtVMName
            // 
            txtVMName.Location = new Point(306, 175);
            txtVMName.Name = "txtVMName";
            txtVMName.Size = new Size(262, 23);
            txtVMName.TabIndex = 6;
            // 
            // labelVMName
            // 
            labelVMName.AutoSize = true;
            labelVMName.BackColor = Color.Transparent;
            labelVMName.Location = new Point(152, 178);
            labelVMName.Name = "labelVMName";
            labelVMName.Size = new Size(126, 15);
            labelVMName.TabIndex = 7;
            labelVMName.Text = "Virtual machine name:";
            // 
            // pictureBoxStep1Top
            // 
            pictureBoxStep1Top.Location = new Point(0, 0);
            pictureBoxStep1Top.Name = "pictureBoxStep1Top";
            pictureBoxStep1Top.Size = new Size(707, 85);
            pictureBoxStep1Top.TabIndex = 4;
            pictureBoxStep1Top.TabStop = false;
            // 
            // pictureBoxStep1Bottom
            // 
            pictureBoxStep1Bottom.Location = new Point(0, 454);
            pictureBoxStep1Bottom.Name = "pictureBoxStep1Bottom";
            pictureBoxStep1Bottom.Size = new Size(707, 59);
            pictureBoxStep1Bottom.TabIndex = 4;
            pictureBoxStep1Bottom.TabStop = false;
            // 
            // pictureBoxStep1Left
            // 
            pictureBoxStep1Left.Location = new Point(0, 85);
            pictureBoxStep1Left.Name = "pictureBoxStep1Left";
            pictureBoxStep1Left.Size = new Size(118, 369);
            pictureBoxStep1Left.TabIndex = 5;
            pictureBoxStep1Left.TabStop = false;
            // 
            // panelStep1Left
            // 
            panelStep1Left.Location = new Point(0, 85);
            panelStep1Left.Name = "panelStep1Left";
            panelStep1Left.Size = new Size(118, 369);
            panelStep1Left.TabIndex = 6;
            // 
            // panelStep1Top
            // 
            panelStep1Top.Location = new Point(0, 0);
            panelStep1Top.Name = "panelStep1Top";
            panelStep1Top.Size = new Size(707, 85);
            panelStep1Top.TabIndex = 6;
            // 
            // pictureBoxStep1TopRight
            // 
            pictureBoxStep1TopRight.BackColor = Color.Transparent;
            pictureBoxStep1TopRight.Location = new Point(622, 0);
            pictureBoxStep1TopRight.Name = "pictureBoxStep1TopRight";
            pictureBoxStep1TopRight.Size = new Size(85, 85);
            pictureBoxStep1TopRight.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStep1TopRight.TabIndex = 4;
            pictureBoxStep1TopRight.TabStop = false;
            // 
            // panelStep1Bottom
            // 
            panelStep1Bottom.Location = new Point(0, 454);
            panelStep1Bottom.Name = "panelStep1Bottom";
            panelStep1Bottom.Size = new Size(707, 59);
            panelStep1Bottom.TabIndex = 6;
            // 
            // pictureBoxStep1Background
            // 
            pictureBoxStep1Background.Location = new Point(118, 85);
            pictureBoxStep1Background.Name = "pictureBoxStep1Background";
            pictureBoxStep1Background.Size = new Size(589, 369);
            pictureBoxStep1Background.TabIndex = 5;
            pictureBoxStep1Background.TabStop = false;
            // 
            // panelStep1Background
            // 
            panelStep1Background.Controls.Add(iconFolder);
            panelStep1Background.Location = new Point(118, 85);
            panelStep1Background.Name = "panelStep1Background";
            panelStep1Background.Size = new Size(589, 369);
            panelStep1Background.TabIndex = 6;
            // 
            // iconFolder
            // 
            iconFolder.Location = new Point(539, 11);
            iconFolder.Name = "iconFolder";
            iconFolder.Size = new Size(32, 32);
            iconFolder.TabIndex = 0;
            iconFolder.TabStop = false;
            // 
            // panelStep2
            // 
            panelStep2.Controls.Add(labelStep2Text);
            panelStep2.Controls.Add(labelStep2Title);
            panelStep2.Controls.Add(processors);
            panelStep2.Controls.Add(memory);
            panelStep2.Controls.Add(firmwareType);
            panelStep2.Controls.Add(btnStep2Back);
            panelStep2.Controls.Add(btnStep2Next);
            panelStep2.Controls.Add(btnStep2Cancel);
            panelStep2.Controls.Add(pictureBoxStep2Top);
            panelStep2.Controls.Add(pictureBoxStep2Bottom);
            panelStep2.Controls.Add(pictureBoxStep2Left);
            panelStep2.Controls.Add(pictureBoxStep2Background);
            panelStep2.Controls.Add(panelStep2Left);
            panelStep2.Controls.Add(panelStep2Top);
            panelStep2.Controls.Add(pictureBoxStep2TopRight);
            panelStep2.Controls.Add(panelStep2Bottom);
            panelStep2.Controls.Add(panelStep2Background);
            panelStep2.Dock = DockStyle.Fill;
            panelStep2.Location = new Point(0, 0);
            panelStep2.Name = "panelStep2";
            panelStep2.Size = new Size(707, 513);
            panelStep2.TabIndex = 0;
            panelStep2.Visible = false;
            // 
            // labelStep2Text
            // 
            labelStep2Text.BackColor = Color.Transparent;
            labelStep2Text.Location = new Point(151, 117);
            labelStep2Text.Name = "labelStep2Text";
            labelStep2Text.Size = new Size(374, 20);
            labelStep2Text.TabIndex = 3;
            labelStep2Text.Text = "Configure processors, memory, and firmware type.";
            // 
            // labelStep2Title
            // 
            labelStep2Title.BackColor = Color.Transparent;
            labelStep2Title.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelStep2Title.Location = new Point(136, 96);
            labelStep2Title.Name = "labelStep2Title";
            labelStep2Title.Size = new Size(464, 21);
            labelStep2Title.TabIndex = 4;
            labelStep2Title.Text = "Virtual Machine Hardware Configuration";
            // 
            // processors
            // 
            processors.Controls.Add(iconProcessor);
            processors.Controls.Add(comboBoxNumberOfCoresPerProcessor);
            processors.Controls.Add(comboNumberOfProcessors);
            processors.Controls.Add(labelNumberOfCoresPerProcessor);
            processors.Controls.Add(labelTotalOfCoresX);
            processors.Controls.Add(labelTotalOfCores);
            processors.Controls.Add(labelNumberOfProcessors);
            processors.Location = new Point(315, 163);
            processors.Name = "processors";
            processors.Size = new Size(380, 77);
            processors.TabIndex = 3;
            processors.TabStop = false;
            processors.Text = "Processors";
            // 
            // comboBoxNumberOfCoresPerProcessor
            // 
            comboBoxNumberOfCoresPerProcessor.FormattingEnabled = true;
            comboBoxNumberOfCoresPerProcessor.Location = new Point(204, 46);
            comboBoxNumberOfCoresPerProcessor.Name = "comboBoxNumberOfCoresPerProcessor";
            comboBoxNumberOfCoresPerProcessor.Size = new Size(64, 23);
            comboBoxNumberOfCoresPerProcessor.TabIndex = 1;
            // 
            // comboNumberOfProcessors
            // 
            comboNumberOfProcessors.FormattingEnabled = true;
            comboNumberOfProcessors.Location = new Point(204, 20);
            comboNumberOfProcessors.Name = "comboNumberOfProcessors";
            comboNumberOfProcessors.Size = new Size(64, 23);
            comboNumberOfProcessors.TabIndex = 1;
            // 
            // labelNumberOfCoresPerProcessor
            // 
            labelNumberOfCoresPerProcessor.AutoSize = true;
            labelNumberOfCoresPerProcessor.Location = new Point(12, 51);
            labelNumberOfCoresPerProcessor.Name = "labelNumberOfCoresPerProcessor";
            labelNumberOfCoresPerProcessor.Size = new Size(173, 15);
            labelNumberOfCoresPerProcessor.TabIndex = 0;
            labelNumberOfCoresPerProcessor.Text = "Number of cores per processor:";
            // 
            // labelTotalOfCoresX
            // 
            labelTotalOfCoresX.AutoSize = true;
            labelTotalOfCoresX.Location = new Point(283, 51);
            labelTotalOfCoresX.Name = "labelTotalOfCoresX";
            labelTotalOfCoresX.Size = new Size(94, 15);
            labelTotalOfCoresX.TabIndex = 0;
            labelTotalOfCoresX.Text = "<Total of cores>";
            // 
            // labelTotalOfCores
            // 
            labelTotalOfCores.AutoSize = true;
            labelTotalOfCores.Location = new Point(283, 25);
            labelTotalOfCores.Name = "labelTotalOfCores";
            labelTotalOfCores.Size = new Size(81, 15);
            labelTotalOfCores.TabIndex = 0;
            labelTotalOfCores.Text = "Total of cores:";
            // 
            // labelNumberOfProcessors
            // 
            labelNumberOfProcessors.AutoSize = true;
            labelNumberOfProcessors.Location = new Point(12, 25);
            labelNumberOfProcessors.Name = "labelNumberOfProcessors";
            labelNumberOfProcessors.Size = new Size(127, 15);
            labelNumberOfProcessors.TabIndex = 0;
            labelNumberOfProcessors.Text = "Number of processors:";
            // 
            // memory
            // 
            memory.Controls.Add(iconMemory);
            memory.Controls.Add(trackBarMemory);
            memory.Controls.Add(numericUpDownMemory);
            memory.Controls.Add(labelMemory);
            memory.Controls.Add(labelRecommendedMin);
            memory.Controls.Add(labelRecommendedDefault);
            memory.Controls.Add(labelRecommendedMax);
            memory.Location = new Point(135, 255);
            memory.Name = "memory";
            memory.Size = new Size(560, 190);
            memory.TabIndex = 3;
            memory.TabStop = false;
            memory.Text = "Memory";
            // 
            // trackBarMemory
            // 
            trackBarMemory.BackColor = Color.Transparent;
            trackBarMemory.Location = new Point(6, 115);
            trackBarMemory.Name = "trackBarMemory";
            trackBarMemory.Size = new Size(543, 45);
            trackBarMemory.TabIndex = 2;
            // 
            // numericUpDownMemory
            // 
            numericUpDownMemory.Increment = new decimal(new int[] { 4, 0, 0, 0 });
            numericUpDownMemory.Location = new Point(236, 27);
            numericUpDownMemory.Name = "numericUpDownMemory";
            numericUpDownMemory.Size = new Size(120, 23);
            numericUpDownMemory.TabIndex = 1;
            // 
            // labelMemory
            // 
            labelMemory.AutoSize = true;
            labelMemory.Location = new Point(39, 29);
            labelMemory.Name = "labelMemory";
            labelMemory.Size = new Size(180, 15);
            labelMemory.TabIndex = 0;
            labelMemory.Text = "Memory for this virtual machine:";
            // 
            // labelRecommendedMin
            // 
            labelRecommendedMin.AutoSize = true;
            labelRecommendedMin.Cursor = Cursors.Hand;
            labelRecommendedMin.Font = new Font("Segoe UI", 6.75F);
            labelRecommendedMin.ForeColor = Color.Red;
            labelRecommendedMin.Location = new Point(17, 70);
            labelRecommendedMin.Name = "labelRecommendedMin";
            labelRecommendedMin.Size = new Size(107, 12);
            labelRecommendedMin.TabIndex = 3;
            labelRecommendedMin.Text = "Recommended Minimum:";
            // 
            // labelRecommendedDefault
            // 
            labelRecommendedDefault.AutoSize = true;
            labelRecommendedDefault.Cursor = Cursors.Hand;
            labelRecommendedDefault.Font = new Font("Segoe UI", 6.75F);
            labelRecommendedDefault.ForeColor = Color.Green;
            labelRecommendedDefault.Location = new Point(199, 70);
            labelRecommendedDefault.Name = "labelRecommendedDefault";
            labelRecommendedDefault.Size = new Size(98, 12);
            labelRecommendedDefault.TabIndex = 4;
            labelRecommendedDefault.Text = "Recommended Default:";
            // 
            // labelRecommendedMax
            // 
            labelRecommendedMax.AutoSize = true;
            labelRecommendedMax.Cursor = Cursors.Hand;
            labelRecommendedMax.Font = new Font("Segoe UI", 6.75F);
            labelRecommendedMax.ForeColor = Color.Blue;
            labelRecommendedMax.Location = new Point(381, 70);
            labelRecommendedMax.Name = "labelRecommendedMax";
            labelRecommendedMax.Size = new Size(109, 12);
            labelRecommendedMax.TabIndex = 5;
            labelRecommendedMax.Text = "Recommended Maximum:";
            // 
            // firmwareType
            // 
            firmwareType.Controls.Add(iconFirmware);
            firmwareType.Controls.Add(uefi);
            firmwareType.Controls.Add(bios);
            firmwareType.Location = new Point(135, 163);
            firmwareType.Name = "firmwareType";
            firmwareType.Size = new Size(174, 77);
            firmwareType.TabIndex = 3;
            firmwareType.TabStop = false;
            firmwareType.Text = "Firmware Type";
            // 
            // uefi
            // 
            uefi.AutoSize = true;
            uefi.Location = new Point(14, 49);
            uefi.Name = "uefi";
            uefi.Size = new Size(48, 19);
            uefi.TabIndex = 0;
            uefi.TabStop = true;
            uefi.Text = "UEFI";
            uefi.UseVisualStyleBackColor = false;
            // 
            // bios
            // 
            bios.AutoSize = true;
            bios.Location = new Point(14, 23);
            bios.Name = "bios";
            bios.Size = new Size(50, 19);
            bios.TabIndex = 1;
            bios.TabStop = true;
            bios.Text = "BIOS";
            bios.UseVisualStyleBackColor = false;
            // 
            // btnStep2Back
            // 
            btnStep2Back.BackColor = Color.FromArgb(245, 245, 245);
            btnStep2Back.FlatStyle = FlatStyle.Flat;
            btnStep2Back.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep2Back.Location = new Point(435, 478);
            btnStep2Back.Name = "btnStep2Back";
            btnStep2Back.Size = new Size(75, 23);
            btnStep2Back.TabIndex = 0;
            btnStep2Back.Text = "< Back";
            btnStep2Back.UseVisualStyleBackColor = true;
            // 
            // btnStep2Next
            // 
            btnStep2Next.BackColor = Color.FromArgb(245, 245, 245);
            btnStep2Next.FlatStyle = FlatStyle.Flat;
            btnStep2Next.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep2Next.Location = new Point(516, 478);
            btnStep2Next.Name = "btnStep2Next";
            btnStep2Next.Size = new Size(75, 23);
            btnStep2Next.TabIndex = 1;
            btnStep2Next.Text = "Next >";
            btnStep2Next.UseVisualStyleBackColor = true;
            // 
            // btnStep2Cancel
            // 
            btnStep2Cancel.BackColor = Color.FromArgb(245, 245, 245);
            btnStep2Cancel.FlatStyle = FlatStyle.Flat;
            btnStep2Cancel.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep2Cancel.Location = new Point(620, 478);
            btnStep2Cancel.Name = "btnStep2Cancel";
            btnStep2Cancel.Size = new Size(75, 23);
            btnStep2Cancel.TabIndex = 2;
            btnStep2Cancel.Text = "Cancel";
            btnStep2Cancel.UseVisualStyleBackColor = true;
            // 
            // pictureBoxStep2Top
            // 
            pictureBoxStep2Top.Location = new Point(0, 0);
            pictureBoxStep2Top.Name = "pictureBoxStep2Top";
            pictureBoxStep2Top.Size = new Size(707, 85);
            pictureBoxStep2Top.TabIndex = 4;
            pictureBoxStep2Top.TabStop = false;
            // 
            // pictureBoxStep2Bottom
            // 
            pictureBoxStep2Bottom.Location = new Point(0, 454);
            pictureBoxStep2Bottom.Name = "pictureBoxStep2Bottom";
            pictureBoxStep2Bottom.Size = new Size(707, 59);
            pictureBoxStep2Bottom.TabIndex = 4;
            pictureBoxStep2Bottom.TabStop = false;
            // 
            // pictureBoxStep2Left
            // 
            pictureBoxStep2Left.Location = new Point(0, 85);
            pictureBoxStep2Left.Name = "pictureBoxStep2Left";
            pictureBoxStep2Left.Size = new Size(118, 369);
            pictureBoxStep2Left.TabIndex = 5;
            pictureBoxStep2Left.TabStop = false;
            // 
            // pictureBoxStep2Background
            // 
            pictureBoxStep2Background.Location = new Point(118, 85);
            pictureBoxStep2Background.Name = "pictureBoxStep2Background";
            pictureBoxStep2Background.Size = new Size(589, 369);
            pictureBoxStep2Background.TabIndex = 5;
            pictureBoxStep2Background.TabStop = false;
            // 
            // panelStep2Left
            // 
            panelStep2Left.Location = new Point(0, 85);
            panelStep2Left.Name = "panelStep2Left";
            panelStep2Left.Size = new Size(118, 369);
            panelStep2Left.TabIndex = 6;
            // 
            // panelStep2Top
            // 
            panelStep2Top.Location = new Point(0, 0);
            panelStep2Top.Name = "panelStep2Top";
            panelStep2Top.Size = new Size(707, 85);
            panelStep2Top.TabIndex = 6;
            // 
            // pictureBoxStep2TopRight
            // 
            pictureBoxStep2TopRight.BackColor = Color.Transparent;
            pictureBoxStep2TopRight.Location = new Point(622, 0);
            pictureBoxStep2TopRight.Name = "pictureBoxStep2TopRight";
            pictureBoxStep2TopRight.Size = new Size(85, 85);
            pictureBoxStep2TopRight.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStep2TopRight.TabIndex = 4;
            pictureBoxStep2TopRight.TabStop = false;
            // 
            // panelStep2Bottom
            // 
            panelStep2Bottom.Location = new Point(0, 454);
            panelStep2Bottom.Name = "panelStep2Bottom";
            panelStep2Bottom.Size = new Size(707, 59);
            panelStep2Bottom.TabIndex = 6;
            // 
            // panelStep2Background
            // 
            panelStep2Background.Location = new Point(118, 85);
            panelStep2Background.Name = "panelStep2Background";
            panelStep2Background.Size = new Size(589, 369);
            panelStep2Background.TabIndex = 6;
            // 
            // panelStep3
            // 
            panelStep3.Controls.Add(labelStep3Text);
            panelStep3.Controls.Add(labelStep3Title);
            panelStep3.Controls.Add(groupBoxSoundCard);
            panelStep3.Controls.Add(groupBoxSCSIIO);
            panelStep3.Controls.Add(groupBoxNetwork);
            panelStep3.Controls.Add(btnStep3Back);
            panelStep3.Controls.Add(btnStep3Next);
            panelStep3.Controls.Add(btnStep3Cancel);
            panelStep3.Controls.Add(pictureBoxStep3Top);
            panelStep3.Controls.Add(pictureBoxStep3Bottom);
            panelStep3.Controls.Add(pictureBoxStep3Left);
            panelStep3.Controls.Add(pictureBoxStep3Background);
            panelStep3.Controls.Add(panelStep3Left);
            panelStep3.Controls.Add(panelStep3Top);
            panelStep3.Controls.Add(pictureBoxStep3TopRight);
            panelStep3.Controls.Add(panelStep3Bottom);
            panelStep3.Controls.Add(panelStep3Background);
            panelStep3.Dock = DockStyle.Fill;
            panelStep3.Location = new Point(0, 0);
            panelStep3.Name = "panelStep3";
            panelStep3.Size = new Size(707, 513);
            panelStep3.TabIndex = 0;
            panelStep3.Visible = false;
            // 
            // labelStep3Text
            // 
            labelStep3Text.BackColor = Color.Transparent;
            labelStep3Text.Location = new Point(151, 117);
            labelStep3Text.Name = "labelStep3Text";
            labelStep3Text.Size = new Size(374, 20);
            labelStep3Text.TabIndex = 3;
            labelStep3Text.Text = "Configure sound cards, SCSI controllers, and networking.";
            // 
            // labelStep3Title
            // 
            labelStep3Title.BackColor = Color.Transparent;
            labelStep3Title.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelStep3Title.Location = new Point(136, 96);
            labelStep3Title.Name = "labelStep3Title";
            labelStep3Title.Size = new Size(464, 21);
            labelStep3Title.TabIndex = 4;
            labelStep3Title.Text = "Peripherals and Network Connection";
            // 
            // groupBoxSoundCard
            // 
            groupBoxSoundCard.Controls.Add(iconSound);
            groupBoxSoundCard.Controls.Add(radioButtonHDAudio);
            groupBoxSoundCard.Controls.Add(radioButtonES1371);
            groupBoxSoundCard.Controls.Add(radioButtonSB16);
            groupBoxSoundCard.Location = new Point(124, 164);
            groupBoxSoundCard.Name = "groupBoxSoundCard";
            groupBoxSoundCard.Size = new Size(204, 178);
            groupBoxSoundCard.TabIndex = 3;
            groupBoxSoundCard.TabStop = false;
            groupBoxSoundCard.Text = "Sound Card";
            // 
            // radioButtonHDAudio
            // 
            radioButtonHDAudio.Location = new Point(15, 122);
            radioButtonHDAudio.Name = "radioButtonHDAudio";
            radioButtonHDAudio.Size = new Size(142, 37);
            radioButtonHDAudio.TabIndex = 0;
            radioButtonHDAudio.TabStop = true;
            radioButtonHDAudio.Text = "Intel High Definition Audio";
            radioButtonHDAudio.UseVisualStyleBackColor = false;
            // 
            // radioButtonES1371
            // 
            radioButtonES1371.Location = new Point(15, 73);
            radioButtonES1371.Name = "radioButtonES1371";
            radioButtonES1371.Size = new Size(120, 37);
            radioButtonES1371.TabIndex = 0;
            radioButtonES1371.TabStop = true;
            radioButtonES1371.Text = "Creative Ensoniq AudioPCI";
            radioButtonES1371.UseVisualStyleBackColor = false;
            // 
            // radioButtonSB16
            // 
            radioButtonSB16.Location = new Point(15, 22);
            radioButtonSB16.Name = "radioButtonSB16";
            radioButtonSB16.Size = new Size(129, 37);
            radioButtonSB16.TabIndex = 0;
            radioButtonSB16.TabStop = true;
            radioButtonSB16.Text = "Creative Labs Sound Blaster 16";
            radioButtonSB16.UseVisualStyleBackColor = false;
            // 
            // groupBoxSCSIIO
            // 
            groupBoxSCSIIO.Controls.Add(iconSCSI);
            groupBoxSCSIIO.Controls.Add(radioParavirtualizedSCSI);
            groupBoxSCSIIO.Controls.Add(radioLSILogicSAS);
            groupBoxSCSIIO.Controls.Add(radioLSILogic);
            groupBoxSCSIIO.Controls.Add(radioBusLogic);
            groupBoxSCSIIO.Location = new Point(334, 163);
            groupBoxSCSIIO.Name = "groupBoxSCSIIO";
            groupBoxSCSIIO.Size = new Size(361, 179);
            groupBoxSCSIIO.TabIndex = 3;
            groupBoxSCSIIO.TabStop = false;
            groupBoxSCSIIO.Text = "SCSI I/O Controller";
            // 
            // radioParavirtualizedSCSI
            // 
            radioParavirtualizedSCSI.AutoSize = true;
            radioParavirtualizedSCSI.Location = new Point(16, 134);
            radioParavirtualizedSCSI.Name = "radioParavirtualizedSCSI";
            radioParavirtualizedSCSI.Size = new Size(128, 19);
            radioParavirtualizedSCSI.TabIndex = 0;
            radioParavirtualizedSCSI.TabStop = true;
            radioParavirtualizedSCSI.Text = "Paravirtualized SCSI";
            radioParavirtualizedSCSI.UseVisualStyleBackColor = false;
            // 
            // radioLSILogicSAS
            // 
            radioLSILogicSAS.AutoSize = true;
            radioLSILogicSAS.Location = new Point(16, 100);
            radioLSILogicSAS.Name = "radioLSILogicSAS";
            radioLSILogicSAS.Size = new Size(95, 19);
            radioLSILogicSAS.TabIndex = 0;
            radioLSILogicSAS.TabStop = true;
            radioLSILogicSAS.Text = "LSI Logic SAS";
            radioLSILogicSAS.UseVisualStyleBackColor = false;
            // 
            // radioLSILogic
            // 
            radioLSILogic.AutoSize = true;
            radioLSILogic.Location = new Point(16, 65);
            radioLSILogic.Name = "radioLSILogic";
            radioLSILogic.Size = new Size(72, 19);
            radioLSILogic.TabIndex = 0;
            radioLSILogic.TabStop = true;
            radioLSILogic.Text = "LSI Logic";
            radioLSILogic.UseVisualStyleBackColor = false;
            // 
            // radioBusLogic
            // 
            radioBusLogic.Location = new Point(16, 32);
            radioBusLogic.Name = "radioBusLogic";
            radioBusLogic.Size = new Size(250, 19);
            radioBusLogic.TabIndex = 0;
            radioBusLogic.TabStop = true;
            radioBusLogic.Text = "BusLogic (Maximum drive capacity: 2 TB)";
            radioBusLogic.UseVisualStyleBackColor = false;
            // 
            // groupBoxNetwork
            // 
            groupBoxNetwork.Controls.Add(iconNetwork);
            groupBoxNetwork.Controls.Add(radioButtonNoNetwork);
            groupBoxNetwork.Controls.Add(radioButtonHostOnly);
            groupBoxNetwork.Controls.Add(radioButtonNAT);
            groupBoxNetwork.Controls.Add(radioButtonBridged);
            groupBoxNetwork.Location = new Point(124, 349);
            groupBoxNetwork.Name = "groupBoxNetwork";
            groupBoxNetwork.Size = new Size(571, 89);
            groupBoxNetwork.TabIndex = 3;
            groupBoxNetwork.TabStop = false;
            groupBoxNetwork.Text = "Network";
            // 
            // radioButtonNoNetwork
            // 
            radioButtonNoNetwork.Location = new Point(247, 52);
            radioButtonNoNetwork.Name = "radioButtonNoNetwork";
            radioButtonNoNetwork.Size = new Size(150, 24);
            radioButtonNoNetwork.TabIndex = 0;
            radioButtonNoNetwork.TabStop = true;
            radioButtonNoNetwork.Text = "Do not use a network connection";
            radioButtonNoNetwork.UseVisualStyleBackColor = false;
            // 
            // btnStep3Back
            // 
            btnStep3Back.BackColor = Color.FromArgb(245, 245, 245);
            btnStep3Back.FlatStyle = FlatStyle.Flat;
            btnStep3Back.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep3Back.Location = new Point(435, 478);
            btnStep3Back.Name = "btnStep3Back";
            btnStep3Back.Size = new Size(75, 23);
            btnStep3Back.TabIndex = 0;
            btnStep3Back.Text = "< Back";
            btnStep3Back.UseVisualStyleBackColor = true;
            // 
            // btnStep3Next
            // 
            btnStep3Next.BackColor = Color.FromArgb(245, 245, 245);
            btnStep3Next.FlatStyle = FlatStyle.Flat;
            btnStep3Next.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep3Next.Location = new Point(516, 478);
            btnStep3Next.Name = "btnStep3Next";
            btnStep3Next.Size = new Size(75, 23);
            btnStep3Next.TabIndex = 1;
            btnStep3Next.Text = "Next >";
            btnStep3Next.UseVisualStyleBackColor = true;
            // 
            // btnStep3Cancel
            // 
            btnStep3Cancel.BackColor = Color.FromArgb(245, 245, 245);
            btnStep3Cancel.FlatStyle = FlatStyle.Flat;
            btnStep3Cancel.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep3Cancel.Location = new Point(620, 478);
            btnStep3Cancel.Name = "btnStep3Cancel";
            btnStep3Cancel.Size = new Size(75, 23);
            btnStep3Cancel.TabIndex = 2;
            btnStep3Cancel.Text = "Cancel";
            btnStep3Cancel.UseVisualStyleBackColor = true;
            // 
            // pictureBoxStep3Top
            // 
            pictureBoxStep3Top.Location = new Point(0, 0);
            pictureBoxStep3Top.Name = "pictureBoxStep3Top";
            pictureBoxStep3Top.Size = new Size(707, 85);
            pictureBoxStep3Top.TabIndex = 4;
            pictureBoxStep3Top.TabStop = false;
            // 
            // pictureBoxStep3Bottom
            // 
            pictureBoxStep3Bottom.Location = new Point(0, 454);
            pictureBoxStep3Bottom.Name = "pictureBoxStep3Bottom";
            pictureBoxStep3Bottom.Size = new Size(707, 59);
            pictureBoxStep3Bottom.TabIndex = 4;
            pictureBoxStep3Bottom.TabStop = false;
            // 
            // pictureBoxStep3Left
            // 
            pictureBoxStep3Left.Location = new Point(0, 85);
            pictureBoxStep3Left.Name = "pictureBoxStep3Left";
            pictureBoxStep3Left.Size = new Size(118, 369);
            pictureBoxStep3Left.TabIndex = 5;
            pictureBoxStep3Left.TabStop = false;
            // 
            // pictureBoxStep3Background
            // 
            pictureBoxStep3Background.Location = new Point(118, 85);
            pictureBoxStep3Background.Name = "pictureBoxStep3Background";
            pictureBoxStep3Background.Size = new Size(589, 369);
            pictureBoxStep3Background.TabIndex = 5;
            pictureBoxStep3Background.TabStop = false;
            // 
            // panelStep3Left
            // 
            panelStep3Left.Location = new Point(0, 85);
            panelStep3Left.Name = "panelStep3Left";
            panelStep3Left.Size = new Size(118, 369);
            panelStep3Left.TabIndex = 6;
            // 
            // panelStep3Top
            // 
            panelStep3Top.Location = new Point(0, 0);
            panelStep3Top.Name = "panelStep3Top";
            panelStep3Top.Size = new Size(707, 85);
            panelStep3Top.TabIndex = 6;
            // 
            // pictureBoxStep3TopRight
            // 
            pictureBoxStep3TopRight.BackColor = Color.Transparent;
            pictureBoxStep3TopRight.Location = new Point(622, 0);
            pictureBoxStep3TopRight.Name = "pictureBoxStep3TopRight";
            pictureBoxStep3TopRight.Size = new Size(85, 85);
            pictureBoxStep3TopRight.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStep3TopRight.TabIndex = 4;
            pictureBoxStep3TopRight.TabStop = false;
            // 
            // panelStep3Bottom
            // 
            panelStep3Bottom.Location = new Point(0, 454);
            panelStep3Bottom.Name = "panelStep3Bottom";
            panelStep3Bottom.Size = new Size(707, 59);
            panelStep3Bottom.TabIndex = 6;
            // 
            // panelStep3Background
            // 
            panelStep3Background.Location = new Point(118, 85);
            panelStep3Background.Name = "panelStep3Background";
            panelStep3Background.Size = new Size(589, 369);
            panelStep3Background.TabIndex = 6;
            // 
            // panelStep4
            // 
            panelStep4.Controls.Add(labelStep4Text);
            panelStep4.Controls.Add(labelStep4Title);
            panelStep4.Controls.Add(radioButtonSplit);
            panelStep4.Controls.Add(radioButtonSingle);
            panelStep4.Controls.Add(labelSplit);
            panelStep4.Controls.Add(labelAllocate);
            panelStep4.Controls.Add(checkBoxAllocate);
            panelStep4.Controls.Add(numericUpDownDriveCapacity);
            panelStep4.Controls.Add(labelRecommendedDriveCapacity);
            panelStep4.Controls.Add(labelDriveCapacity);
            panelStep4.Controls.Add(groupBoxVirtualDriveType);
            panelStep4.Controls.Add(btnStep4Back);
            panelStep4.Controls.Add(btnStep4Next);
            panelStep4.Controls.Add(btnStep4Cancel);
            panelStep4.Controls.Add(pictureBoxStep4Top);
            panelStep4.Controls.Add(pictureBoxStep4Bottom);
            panelStep4.Controls.Add(pictureBoxStep4Left);
            panelStep4.Controls.Add(panelStep4Left);
            panelStep4.Controls.Add(panelStep4Top);
            panelStep4.Controls.Add(pictureBoxStep4TopRight);
            panelStep4.Controls.Add(panelStep4Bottom);
            panelStep4.Controls.Add(pictureBoxStep4Background);
            panelStep4.Controls.Add(panelStep4Background);
            panelStep4.Dock = DockStyle.Fill;
            panelStep4.Location = new Point(0, 0);
            panelStep4.Name = "panelStep4";
            panelStep4.Size = new Size(707, 513);
            panelStep4.TabIndex = 1;
            panelStep4.Visible = false;
            // 
            // labelStep4Text
            // 
            labelStep4Text.BackColor = Color.Transparent;
            labelStep4Text.Location = new Point(151, 117);
            labelStep4Text.Name = "labelStep4Text";
            labelStep4Text.Size = new Size(374, 20);
            labelStep4Text.TabIndex = 3;
            labelStep4Text.Text = "Specify disk type, capacity, and allocation options.";
            // 
            // labelStep4Title
            // 
            labelStep4Title.BackColor = Color.Transparent;
            labelStep4Title.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelStep4Title.Location = new Point(136, 96);
            labelStep4Title.Name = "labelStep4Title";
            labelStep4Title.Size = new Size(464, 21);
            labelStep4Title.TabIndex = 4;
            labelStep4Title.Text = "Virtual Hard Disk";
            // 
            // radioButtonSplit
            // 
            radioButtonSplit.AutoSize = true;
            radioButtonSplit.Location = new Point(397, 372);
            radioButtonSplit.Name = "radioButtonSplit";
            radioButtonSplit.Size = new Size(228, 19);
            radioButtonSplit.TabIndex = 8;
            radioButtonSplit.TabStop = true;
            radioButtonSplit.Text = "Split the virtual drive into multiple files";
            radioButtonSplit.UseVisualStyleBackColor = false;
            // 
            // radioButtonSingle
            // 
            radioButtonSingle.AutoSize = true;
            radioButtonSingle.Location = new Point(146, 372);
            radioButtonSingle.Name = "radioButtonSingle";
            radioButtonSingle.Size = new Size(213, 19);
            radioButtonSingle.TabIndex = 8;
            radioButtonSingle.TabStop = true;
            radioButtonSingle.Text = "Store the virtual drive as a single file";
            radioButtonSingle.UseVisualStyleBackColor = false;
            // 
            // labelSplit
            // 
            labelSplit.BackColor = Color.Transparent;
            labelSplit.Location = new Point(166, 400);
            labelSplit.Name = "labelSplit";
            labelSplit.Size = new Size(459, 38);
            labelSplit.TabIndex = 7;
            labelSplit.Text = "Splitting the drive makes it easier to move the virtual machine to another computer.";
            // 
            // labelAllocate
            // 
            labelAllocate.BackColor = Color.Transparent;
            labelAllocate.Location = new Point(338, 250);
            labelAllocate.Name = "labelAllocate";
            labelAllocate.Size = new Size(354, 79);
            labelAllocate.TabIndex = 7;
            labelAllocate.Text = resources.GetString("labelAllocate.Text");
            // 
            // checkBoxAllocate
            // 
            checkBoxAllocate.AutoSize = true;
            checkBoxAllocate.Location = new Point(321, 221);
            checkBoxAllocate.Name = "checkBoxAllocate";
            checkBoxAllocate.Size = new Size(172, 19);
            checkBoxAllocate.TabIndex = 6;
            checkBoxAllocate.Text = "Allocate all drive space now";
            checkBoxAllocate.UseVisualStyleBackColor = false;
            // 
            // numericUpDownDriveCapacity
            // 
            numericUpDownDriveCapacity.Location = new Point(501, 161);
            numericUpDownDriveCapacity.Name = "numericUpDownDriveCapacity";
            numericUpDownDriveCapacity.Size = new Size(89, 23);
            numericUpDownDriveCapacity.TabIndex = 5;
            // 
            // labelRecommendedDriveCapacity
            // 
            labelRecommendedDriveCapacity.AutoSize = true;
            labelRecommendedDriveCapacity.BackColor = Color.Transparent;
            labelRecommendedDriveCapacity.Location = new Point(321, 194);
            labelRecommendedDriveCapacity.Name = "labelRecommendedDriveCapacity";
            labelRecommendedDriveCapacity.Size = new Size(286, 15);
            labelRecommendedDriveCapacity.TabIndex = 4;
            labelRecommendedDriveCapacity.Text = "Recommended capacity for {osName}: {Capacity} GB";
            // 
            // labelDriveCapacity
            // 
            labelDriveCapacity.AutoSize = true;
            labelDriveCapacity.BackColor = Color.Transparent;
            labelDriveCapacity.Location = new Point(311, 164);
            labelDriveCapacity.Name = "labelDriveCapacity";
            labelDriveCapacity.Size = new Size(146, 15);
            labelDriveCapacity.TabIndex = 4;
            labelDriveCapacity.Text = "Virtual drive capacity (GB):";
            // 
            // groupBoxVirtualDriveType
            // 
            groupBoxVirtualDriveType.Controls.Add(radioButtonNVMe);
            groupBoxVirtualDriveType.Controls.Add(radioButtonSATA);
            groupBoxVirtualDriveType.Controls.Add(radioButtonSCSI);
            groupBoxVirtualDriveType.Controls.Add(radioButtonIDE);
            groupBoxVirtualDriveType.Location = new Point(122, 161);
            groupBoxVirtualDriveType.Name = "groupBoxVirtualDriveType";
            groupBoxVirtualDriveType.Size = new Size(167, 134);
            groupBoxVirtualDriveType.TabIndex = 3;
            groupBoxVirtualDriveType.TabStop = false;
            groupBoxVirtualDriveType.Text = "Virtual Drive Type";
            // 
            // radioButtonNVMe
            // 
            radioButtonNVMe.AutoSize = true;
            radioButtonNVMe.Location = new Point(11, 98);
            radioButtonNVMe.Name = "radioButtonNVMe";
            radioButtonNVMe.Size = new Size(58, 19);
            radioButtonNVMe.TabIndex = 0;
            radioButtonNVMe.TabStop = true;
            radioButtonNVMe.Text = "NVMe";
            radioButtonNVMe.UseVisualStyleBackColor = false;
            // 
            // radioButtonSATA
            // 
            radioButtonSATA.AutoSize = true;
            radioButtonSATA.Location = new Point(11, 72);
            radioButtonSATA.Name = "radioButtonSATA";
            radioButtonSATA.Size = new Size(52, 19);
            radioButtonSATA.TabIndex = 0;
            radioButtonSATA.TabStop = true;
            radioButtonSATA.Text = "SATA";
            radioButtonSATA.UseVisualStyleBackColor = false;
            // 
            // radioButtonSCSI
            // 
            radioButtonSCSI.AutoSize = true;
            radioButtonSCSI.Location = new Point(11, 47);
            radioButtonSCSI.Name = "radioButtonSCSI";
            radioButtonSCSI.Size = new Size(48, 19);
            radioButtonSCSI.TabIndex = 0;
            radioButtonSCSI.TabStop = true;
            radioButtonSCSI.Text = "SCSI";
            radioButtonSCSI.UseVisualStyleBackColor = false;
            // 
            // radioButtonIDE
            // 
            radioButtonIDE.AutoSize = true;
            radioButtonIDE.Location = new Point(11, 22);
            radioButtonIDE.Name = "radioButtonIDE";
            radioButtonIDE.Size = new Size(42, 19);
            radioButtonIDE.TabIndex = 0;
            radioButtonIDE.TabStop = true;
            radioButtonIDE.Text = "IDE";
            radioButtonIDE.UseVisualStyleBackColor = false;
            // 
            // btnStep4Back
            // 
            btnStep4Back.BackColor = Color.FromArgb(245, 245, 245);
            btnStep4Back.FlatStyle = FlatStyle.Flat;
            btnStep4Back.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep4Back.Location = new Point(435, 478);
            btnStep4Back.Name = "btnStep4Back";
            btnStep4Back.Size = new Size(75, 23);
            btnStep4Back.TabIndex = 0;
            btnStep4Back.Text = "< Back";
            btnStep4Back.UseVisualStyleBackColor = true;
            // 
            // btnStep4Next
            // 
            btnStep4Next.BackColor = Color.FromArgb(245, 245, 245);
            btnStep4Next.FlatStyle = FlatStyle.Flat;
            btnStep4Next.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep4Next.Location = new Point(516, 478);
            btnStep4Next.Name = "btnStep4Next";
            btnStep4Next.Size = new Size(75, 23);
            btnStep4Next.TabIndex = 1;
            btnStep4Next.Text = "Next >";
            btnStep4Next.UseVisualStyleBackColor = true;
            // 
            // btnStep4Cancel
            // 
            btnStep4Cancel.BackColor = Color.FromArgb(245, 245, 245);
            btnStep4Cancel.FlatStyle = FlatStyle.Flat;
            btnStep4Cancel.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep4Cancel.Location = new Point(620, 478);
            btnStep4Cancel.Name = "btnStep4Cancel";
            btnStep4Cancel.Size = new Size(75, 23);
            btnStep4Cancel.TabIndex = 2;
            btnStep4Cancel.Text = "Cancel";
            btnStep4Cancel.UseVisualStyleBackColor = true;
            // 
            // pictureBoxStep4Top
            // 
            pictureBoxStep4Top.Location = new Point(0, 0);
            pictureBoxStep4Top.Name = "pictureBoxStep4Top";
            pictureBoxStep4Top.Size = new Size(707, 85);
            pictureBoxStep4Top.TabIndex = 4;
            pictureBoxStep4Top.TabStop = false;
            // 
            // pictureBoxStep4Bottom
            // 
            pictureBoxStep4Bottom.Location = new Point(0, 454);
            pictureBoxStep4Bottom.Name = "pictureBoxStep4Bottom";
            pictureBoxStep4Bottom.Size = new Size(707, 59);
            pictureBoxStep4Bottom.TabIndex = 4;
            pictureBoxStep4Bottom.TabStop = false;
            // 
            // pictureBoxStep4Left
            // 
            pictureBoxStep4Left.Location = new Point(0, 85);
            pictureBoxStep4Left.Name = "pictureBoxStep4Left";
            pictureBoxStep4Left.Size = new Size(118, 369);
            pictureBoxStep4Left.TabIndex = 5;
            pictureBoxStep4Left.TabStop = false;
            // 
            // panelStep4Left
            // 
            panelStep4Left.Location = new Point(0, 85);
            panelStep4Left.Name = "panelStep4Left";
            panelStep4Left.Size = new Size(118, 369);
            panelStep4Left.TabIndex = 6;
            // 
            // panelStep4Top
            // 
            panelStep4Top.Location = new Point(0, 0);
            panelStep4Top.Name = "panelStep4Top";
            panelStep4Top.Size = new Size(707, 85);
            panelStep4Top.TabIndex = 6;
            // 
            // pictureBoxStep4TopRight
            // 
            pictureBoxStep4TopRight.BackColor = Color.Transparent;
            pictureBoxStep4TopRight.Location = new Point(622, 0);
            pictureBoxStep4TopRight.Name = "pictureBoxStep4TopRight";
            pictureBoxStep4TopRight.Size = new Size(85, 85);
            pictureBoxStep4TopRight.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStep4TopRight.TabIndex = 4;
            pictureBoxStep4TopRight.TabStop = false;
            // 
            // panelStep4Bottom
            // 
            panelStep4Bottom.Location = new Point(0, 454);
            panelStep4Bottom.Name = "panelStep4Bottom";
            panelStep4Bottom.Size = new Size(707, 59);
            panelStep4Bottom.TabIndex = 6;
            // 
            // pictureBoxStep4Background
            // 
            pictureBoxStep4Background.Location = new Point(118, 85);
            pictureBoxStep4Background.Name = "pictureBoxStep4Background";
            pictureBoxStep4Background.Size = new Size(589, 369);
            pictureBoxStep4Background.TabIndex = 5;
            pictureBoxStep4Background.TabStop = false;
            // 
            // panelStep4Background
            // 
            panelStep4Background.Controls.Add(iconHardDrive);
            panelStep4Background.Location = new Point(118, 85);
            panelStep4Background.Name = "panelStep4Background";
            panelStep4Background.Size = new Size(589, 369);
            panelStep4Background.TabIndex = 6;
            // 
            // iconHardDrive
            // 
            iconHardDrive.Location = new Point(539, 11);
            iconHardDrive.Name = "iconHardDrive";
            iconHardDrive.Size = new Size(32, 32);
            iconHardDrive.TabIndex = 0;
            iconHardDrive.TabStop = false;
            // 
            // panelStep5
            // 
            panelStep5.Controls.Add(iconFloppy);
            panelStep5.Controls.Add(iconCDIDE);
            panelStep5.Controls.Add(iconCDSCSI);
            panelStep5.Controls.Add(iconCDSATA);
            panelStep5.Controls.Add(labelStep5Text);
            panelStep5.Controls.Add(labelStep5Title);
            panelStep5.Controls.Add(labelCDLimit);
            panelStep5.Controls.Add(labelUsedCDController);
            panelStep5.Controls.Add(numericUpDownFloppyCount);
            panelStep5.Controls.Add(numericUpDownCDIDECount);
            panelStep5.Controls.Add(numericUpDownCDSCSICount);
            panelStep5.Controls.Add(numericUpDownCDSATACount);
            panelStep5.Controls.Add(labelNumberOfCDSATA);
            panelStep5.Controls.Add(labelNumberOfCDSCSI);
            panelStep5.Controls.Add(labelNumberOfCDIDE);
            panelStep5.Controls.Add(labelNumberOfFloppy);
            panelStep5.Controls.Add(btnStep5Back);
            panelStep5.Controls.Add(btnStep5Next);
            panelStep5.Controls.Add(btnStep5Cancel);
            panelStep5.Controls.Add(pictureBoxStep5Top);
            panelStep5.Controls.Add(pictureBoxStep5Bottom);
            panelStep5.Controls.Add(pictureBoxStep5Left);
            panelStep5.Controls.Add(pictureBoxStep5Background);
            panelStep5.Controls.Add(panelStep5Left);
            panelStep5.Controls.Add(panelStep5Top);
            panelStep5.Controls.Add(pictureBoxStep5TopRight);
            panelStep5.Controls.Add(panelStep5Bottom);
            panelStep5.Controls.Add(panelStep5Background);
            panelStep5.Dock = DockStyle.Fill;
            panelStep5.Location = new Point(0, 0);
            panelStep5.Name = "panelStep5";
            panelStep5.Size = new Size(707, 513);
            panelStep5.TabIndex = 2;
            panelStep5.Visible = false;
            // 
            // labelStep5Text
            // 
            labelStep5Text.BackColor = Color.Transparent;
            labelStep5Text.Location = new Point(151, 117);
            labelStep5Text.Name = "labelStep5Text";
            labelStep5Text.Size = new Size(374, 20);
            labelStep5Text.TabIndex = 3;
            labelStep5Text.Text = "Configure additional optical and floppy drives.";
            // 
            // labelStep5Title
            // 
            labelStep5Title.BackColor = Color.Transparent;
            labelStep5Title.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelStep5Title.Location = new Point(136, 96);
            labelStep5Title.Name = "labelStep5Title";
            labelStep5Title.Size = new Size(464, 21);
            labelStep5Title.TabIndex = 4;
            labelStep5Title.Text = "CD/DVD and Floppy Drives";
            // 
            // labelCDLimit
            // 
            labelCDLimit.BackColor = Color.Transparent;
            labelCDLimit.Location = new Point(177, 368);
            labelCDLimit.Name = "labelCDLimit";
            labelCDLimit.Size = new Size(413, 51);
            labelCDLimit.TabIndex = 8;
            labelCDLimit.Text = "The virtual machine can have up to 2 floppy drives, up to 3 IDE CD/DVD drives, up to 10 SCSI CD/DVD drives, and up to 10 SATA CD/DVD drives.";
            // 
            // labelUsedCDController
            // 
            labelUsedCDController.AutoSize = true;
            labelUsedCDController.BackColor = Color.Transparent;
            labelUsedCDController.Location = new Point(177, 175);
            labelUsedCDController.Name = "labelUsedCDController";
            labelUsedCDController.Size = new Size(380, 15);
            labelUsedCDController.TabIndex = 8;
            labelUsedCDController.Text = "The controller used for the disc of the operating system: {CDcontroller}";
            // 
            // labelNumberOfCDSATA
            // 
            labelNumberOfCDSATA.AutoSize = true;
            labelNumberOfCDSATA.BackColor = Color.Transparent;
            labelNumberOfCDSATA.Location = new Point(207, 325);
            labelNumberOfCDSATA.Name = "labelNumberOfCDSATA";
            labelNumberOfCDSATA.Size = new Size(188, 15);
            labelNumberOfCDSATA.TabIndex = 3;
            labelNumberOfCDSATA.Text = "Number of CD/DVD Drives (SATA):";
            // 
            // labelNumberOfCDSCSI
            // 
            labelNumberOfCDSCSI.AutoSize = true;
            labelNumberOfCDSCSI.BackColor = Color.Transparent;
            labelNumberOfCDSCSI.Location = new Point(207, 288);
            labelNumberOfCDSCSI.Name = "labelNumberOfCDSCSI";
            labelNumberOfCDSCSI.Size = new Size(184, 15);
            labelNumberOfCDSCSI.TabIndex = 3;
            labelNumberOfCDSCSI.Text = "Number of CD/DVD Drives (SCSI):";
            // 
            // labelNumberOfCDIDE
            // 
            labelNumberOfCDIDE.AutoSize = true;
            labelNumberOfCDIDE.BackColor = Color.Transparent;
            labelNumberOfCDIDE.Location = new Point(207, 250);
            labelNumberOfCDIDE.Name = "labelNumberOfCDIDE";
            labelNumberOfCDIDE.Size = new Size(178, 15);
            labelNumberOfCDIDE.TabIndex = 3;
            labelNumberOfCDIDE.Text = "Number of CD/DVD Drives (IDE):";
            // 
            // labelNumberOfFloppy
            // 
            labelNumberOfFloppy.AutoSize = true;
            labelNumberOfFloppy.BackColor = Color.Transparent;
            labelNumberOfFloppy.Location = new Point(207, 214);
            labelNumberOfFloppy.Name = "labelNumberOfFloppy";
            labelNumberOfFloppy.Size = new Size(142, 15);
            labelNumberOfFloppy.TabIndex = 3;
            labelNumberOfFloppy.Text = "Number of Floppy Drives:";
            // 
            // btnStep5Back
            // 
            btnStep5Back.BackColor = Color.FromArgb(245, 245, 245);
            btnStep5Back.FlatStyle = FlatStyle.Flat;
            btnStep5Back.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep5Back.Location = new Point(435, 478);
            btnStep5Back.Name = "btnStep5Back";
            btnStep5Back.Size = new Size(75, 23);
            btnStep5Back.TabIndex = 0;
            btnStep5Back.Text = "< Back";
            btnStep5Back.UseVisualStyleBackColor = true;
            // 
            // btnStep5Next
            // 
            btnStep5Next.BackColor = Color.FromArgb(245, 245, 245);
            btnStep5Next.FlatStyle = FlatStyle.Flat;
            btnStep5Next.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep5Next.Location = new Point(516, 478);
            btnStep5Next.Name = "btnStep5Next";
            btnStep5Next.Size = new Size(75, 23);
            btnStep5Next.TabIndex = 1;
            btnStep5Next.Text = "Next >";
            btnStep5Next.UseVisualStyleBackColor = true;
            // 
            // btnStep5Cancel
            // 
            btnStep5Cancel.BackColor = Color.FromArgb(245, 245, 245);
            btnStep5Cancel.FlatStyle = FlatStyle.Flat;
            btnStep5Cancel.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep5Cancel.Location = new Point(620, 478);
            btnStep5Cancel.Name = "btnStep5Cancel";
            btnStep5Cancel.Size = new Size(75, 23);
            btnStep5Cancel.TabIndex = 2;
            btnStep5Cancel.Text = "Cancel";
            btnStep5Cancel.UseVisualStyleBackColor = true;
            // 
            // pictureBoxStep5Top
            // 
            pictureBoxStep5Top.Location = new Point(0, 0);
            pictureBoxStep5Top.Name = "pictureBoxStep5Top";
            pictureBoxStep5Top.Size = new Size(707, 85);
            pictureBoxStep5Top.TabIndex = 4;
            pictureBoxStep5Top.TabStop = false;
            // 
            // pictureBoxStep5Bottom
            // 
            pictureBoxStep5Bottom.Location = new Point(0, 454);
            pictureBoxStep5Bottom.Name = "pictureBoxStep5Bottom";
            pictureBoxStep5Bottom.Size = new Size(707, 59);
            pictureBoxStep5Bottom.TabIndex = 4;
            pictureBoxStep5Bottom.TabStop = false;
            // 
            // pictureBoxStep5Left
            // 
            pictureBoxStep5Left.Location = new Point(0, 85);
            pictureBoxStep5Left.Name = "pictureBoxStep5Left";
            pictureBoxStep5Left.Size = new Size(118, 369);
            pictureBoxStep5Left.TabIndex = 5;
            pictureBoxStep5Left.TabStop = false;
            // 
            // pictureBoxStep5Background
            // 
            pictureBoxStep5Background.Location = new Point(118, 85);
            pictureBoxStep5Background.Name = "pictureBoxStep5Background";
            pictureBoxStep5Background.Size = new Size(589, 369);
            pictureBoxStep5Background.TabIndex = 5;
            pictureBoxStep5Background.TabStop = false;
            // 
            // panelStep5Left
            // 
            panelStep5Left.Location = new Point(0, 85);
            panelStep5Left.Name = "panelStep5Left";
            panelStep5Left.Size = new Size(118, 369);
            panelStep5Left.TabIndex = 6;
            // 
            // panelStep5Top
            // 
            panelStep5Top.Location = new Point(0, 0);
            panelStep5Top.Name = "panelStep5Top";
            panelStep5Top.Size = new Size(707, 85);
            panelStep5Top.TabIndex = 6;
            // 
            // pictureBoxStep5TopRight
            // 
            pictureBoxStep5TopRight.BackColor = Color.Transparent;
            pictureBoxStep5TopRight.Location = new Point(622, 0);
            pictureBoxStep5TopRight.Name = "pictureBoxStep5TopRight";
            pictureBoxStep5TopRight.Size = new Size(85, 85);
            pictureBoxStep5TopRight.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStep5TopRight.TabIndex = 4;
            pictureBoxStep5TopRight.TabStop = false;
            // 
            // panelStep5Bottom
            // 
            panelStep5Bottom.Location = new Point(0, 454);
            panelStep5Bottom.Name = "panelStep5Bottom";
            panelStep5Bottom.Size = new Size(707, 59);
            panelStep5Bottom.TabIndex = 6;
            // 
            // panelStep5Background
            // 
            panelStep5Background.Location = new Point(118, 85);
            panelStep5Background.Name = "panelStep5Background";
            panelStep5Background.Size = new Size(589, 369);
            panelStep5Background.TabIndex = 6;
            // 
            // panelStep6
            // 
            panelStep6.Controls.Add(labelStep6Text);
            panelStep6.Controls.Add(labelStep6Title);
            panelStep6.Controls.Add(pictureVMIcon);
            panelStep6.Controls.Add(labelVMIcon);
            panelStep6.Controls.Add(groupBoxShortcutType);
            panelStep6.Controls.Add(checkBoxStartMenu);
            panelStep6.Controls.Add(checkBoxFullScreen);
            panelStep6.Controls.Add(checkBoxClose);
            panelStep6.Controls.Add(checkBoxShortStart);
            panelStep6.Controls.Add(checkBoxNewWindow);
            panelStep6.Controls.Add(checkBoxDesktop);
            panelStep6.Controls.Add(buttonChangeIcon);
            panelStep6.Controls.Add(btnStep6Back);
            panelStep6.Controls.Add(btnStep6Next);
            panelStep6.Controls.Add(btnStep6Cancel);
            panelStep6.Controls.Add(pictureBoxStep6Top);
            panelStep6.Controls.Add(pictureBoxStep6Bottom);
            panelStep6.Controls.Add(pictureBoxStep6Left);
            panelStep6.Controls.Add(pictureBoxStep6Background);
            panelStep6.Controls.Add(panelStep6Left);
            panelStep6.Controls.Add(panelStep6Top);
            panelStep6.Controls.Add(pictureBoxStep6TopRight);
            panelStep6.Controls.Add(panelStep6Bottom);
            panelStep6.Controls.Add(panelStep6Background);
            panelStep6.Dock = DockStyle.Fill;
            panelStep6.Location = new Point(0, 0);
            panelStep6.Name = "panelStep6";
            panelStep6.Size = new Size(707, 513);
            panelStep6.TabIndex = 3;
            panelStep6.Visible = false;
            // 
            // labelStep6Text
            // 
            labelStep6Text.BackColor = Color.Transparent;
            labelStep6Text.Location = new Point(151, 117);
            labelStep6Text.Name = "labelStep6Text";
            labelStep6Text.Size = new Size(374, 20);
            labelStep6Text.TabIndex = 3;
            labelStep6Text.Text = "Choose icons, shortcuts, and startup preferences.";
            // 
            // labelStep6Title
            // 
            labelStep6Title.BackColor = Color.Transparent;
            labelStep6Title.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelStep6Title.Location = new Point(136, 96);
            labelStep6Title.Name = "labelStep6Title";
            labelStep6Title.Size = new Size(464, 21);
            labelStep6Title.TabIndex = 4;
            labelStep6Title.Text = "Shortcuts and Advanced Options";
            // 
            // pictureVMIcon
            // 
            pictureVMIcon.Location = new Point(352, 150);
            pictureVMIcon.Name = "pictureVMIcon";
            pictureVMIcon.Size = new Size(32, 32);
            pictureVMIcon.TabIndex = 12;
            pictureVMIcon.TabStop = false;
            // 
            // labelVMIcon
            // 
            labelVMIcon.AutoSize = true;
            labelVMIcon.BackColor = Color.Transparent;
            labelVMIcon.Location = new Point(170, 159);
            labelVMIcon.Name = "labelVMIcon";
            labelVMIcon.Size = new Size(119, 15);
            labelVMIcon.TabIndex = 11;
            labelVMIcon.Text = "Virtual machine icon:";
            // 
            // groupBoxShortcutType
            // 
            groupBoxShortcutType.BackColor = Color.Transparent;
            groupBoxShortcutType.Controls.Add(radioShortcutDirect);
            groupBoxShortcutType.Controls.Add(radioShortcutKvm);
            groupBoxShortcutType.Controls.Add(radioShortcutWorkstation);
            groupBoxShortcutType.Location = new Point(150, 195);
            groupBoxShortcutType.Name = "groupBoxShortcutType";
            groupBoxShortcutType.Size = new Size(390, 95);
            groupBoxShortcutType.TabIndex = 13;
            groupBoxShortcutType.TabStop = false;
            groupBoxShortcutType.Text = "Shortcut Type";
            // 
            // radioShortcutDirect
            // 
            radioShortcutDirect.AutoSize = true;
            radioShortcutDirect.Location = new Point(15, 20);
            radioShortcutDirect.Name = "radioShortcutDirect";
            radioShortcutDirect.Size = new Size(150, 19);
            radioShortcutDirect.TabIndex = 0;
            radioShortcutDirect.Text = "Direct VMX file shortcut";
            radioShortcutDirect.UseVisualStyleBackColor = true;
            // 
            // radioShortcutKvm
            // 
            radioShortcutKvm.AutoSize = true;
            radioShortcutKvm.Location = new Point(15, 43);
            radioShortcutKvm.Name = "radioShortcutKvm";
            radioShortcutKvm.Size = new Size(97, 19);
            radioShortcutKvm.TabIndex = 1;
            radioShortcutKvm.Text = "KVM shortcut";
            radioShortcutKvm.UseVisualStyleBackColor = true;
            // 
            // radioShortcutWorkstation
            // 
            radioShortcutWorkstation.AutoSize = true;
            radioShortcutWorkstation.Checked = true;
            radioShortcutWorkstation.Location = new Point(15, 66);
            radioShortcutWorkstation.Name = "radioShortcutWorkstation";
            radioShortcutWorkstation.Size = new Size(181, 19);
            radioShortcutWorkstation.TabIndex = 2;
            radioShortcutWorkstation.TabStop = true;
            radioShortcutWorkstation.Text = "Custom Workstation shortcut";
            radioShortcutWorkstation.UseVisualStyleBackColor = true;
            // 
            // checkBoxStartMenu
            // 
            checkBoxStartMenu.AutoSize = true;
            checkBoxStartMenu.Checked = true;
            checkBoxStartMenu.CheckState = CheckState.Checked;
            checkBoxStartMenu.Location = new Point(371, 422);
            checkBoxStartMenu.Name = "checkBoxStartMenu";
            checkBoxStartMenu.Size = new Size(155, 19);
            checkBoxStartMenu.TabIndex = 10;
            checkBoxStartMenu.Text = "Add start menu shortcut";
            checkBoxStartMenu.UseVisualStyleBackColor = false;
            // 
            // checkBoxFullScreen
            // 
            checkBoxFullScreen.AutoSize = true;
            checkBoxFullScreen.Location = new Point(222, 349);
            checkBoxFullScreen.Name = "checkBoxFullScreen";
            checkBoxFullScreen.Size = new Size(82, 19);
            checkBoxFullScreen.TabIndex = 10;
            checkBoxFullScreen.Text = "Full screen";
            checkBoxFullScreen.UseVisualStyleBackColor = false;
            // 
            // checkBoxClose
            // 
            checkBoxClose.AutoSize = true;
            checkBoxClose.Location = new Point(202, 374);
            checkBoxClose.Name = "checkBoxClose";
            checkBoxClose.Size = new Size(211, 19);
            checkBoxClose.TabIndex = 10;
            checkBoxClose.Text = "Close automatically after power off";
            checkBoxClose.UseVisualStyleBackColor = false;
            // 
            // checkBoxShortStart
            // 
            checkBoxShortStart.AutoSize = true;
            checkBoxShortStart.Checked = true;
            checkBoxShortStart.CheckState = CheckState.Checked;
            checkBoxShortStart.Location = new Point(202, 326);
            checkBoxShortStart.Name = "checkBoxShortStart";
            checkBoxShortStart.Size = new Size(230, 19);
            checkBoxShortStart.TabIndex = 10;
            checkBoxShortStart.Text = "Start the virtual machine automatically";
            checkBoxShortStart.UseVisualStyleBackColor = false;
            // 
            // checkBoxNewWindow
            // 
            checkBoxNewWindow.AutoSize = true;
            checkBoxNewWindow.Checked = true;
            checkBoxNewWindow.CheckState = CheckState.Checked;
            checkBoxNewWindow.Location = new Point(202, 301);
            checkBoxNewWindow.Name = "checkBoxNewWindow";
            checkBoxNewWindow.Size = new Size(185, 19);
            checkBoxNewWindow.TabIndex = 10;
            checkBoxNewWindow.Text = "Always open in a new window";
            checkBoxNewWindow.UseVisualStyleBackColor = false;
            // 
            // checkBoxDesktop
            // 
            checkBoxDesktop.AutoSize = true;
            checkBoxDesktop.Checked = true;
            checkBoxDesktop.CheckState = CheckState.Checked;
            checkBoxDesktop.Location = new Point(170, 422);
            checkBoxDesktop.Name = "checkBoxDesktop";
            checkBoxDesktop.Size = new Size(140, 19);
            checkBoxDesktop.TabIndex = 10;
            checkBoxDesktop.Text = "Add desktop shortcut";
            checkBoxDesktop.UseVisualStyleBackColor = false;
            // 
            // buttonChangeIcon
            // 
            buttonChangeIcon.BackColor = Color.FromArgb(245, 245, 245);
            buttonChangeIcon.FlatStyle = FlatStyle.Flat;
            buttonChangeIcon.ForeColor = Color.FromArgb(30, 30, 30);
            buttonChangeIcon.Location = new Point(398, 155);
            buttonChangeIcon.Name = "buttonChangeIcon";
            buttonChangeIcon.Size = new Size(123, 23);
            buttonChangeIcon.TabIndex = 6;
            buttonChangeIcon.Text = "Change Icon";
            buttonChangeIcon.UseVisualStyleBackColor = true;
            // 
            // btnStep6Back
            // 
            btnStep6Back.BackColor = Color.FromArgb(245, 245, 245);
            btnStep6Back.FlatStyle = FlatStyle.Flat;
            btnStep6Back.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep6Back.Location = new Point(435, 478);
            btnStep6Back.Name = "btnStep6Back";
            btnStep6Back.Size = new Size(75, 23);
            btnStep6Back.TabIndex = 0;
            btnStep6Back.Text = "< Back";
            btnStep6Back.UseVisualStyleBackColor = true;
            // 
            // btnStep6Next
            // 
            btnStep6Next.BackColor = Color.FromArgb(245, 245, 245);
            btnStep6Next.FlatStyle = FlatStyle.Flat;
            btnStep6Next.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep6Next.Location = new Point(516, 478);
            btnStep6Next.Name = "btnStep6Next";
            btnStep6Next.Size = new Size(75, 23);
            btnStep6Next.TabIndex = 1;
            btnStep6Next.Text = "Next >";
            btnStep6Next.UseVisualStyleBackColor = true;
            // 
            // btnStep6Cancel
            // 
            btnStep6Cancel.BackColor = Color.FromArgb(245, 245, 245);
            btnStep6Cancel.FlatStyle = FlatStyle.Flat;
            btnStep6Cancel.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep6Cancel.Location = new Point(620, 478);
            btnStep6Cancel.Name = "btnStep6Cancel";
            btnStep6Cancel.Size = new Size(75, 23);
            btnStep6Cancel.TabIndex = 2;
            btnStep6Cancel.Text = "Cancel";
            btnStep6Cancel.UseVisualStyleBackColor = true;
            // 
            // pictureBoxStep6Top
            // 
            pictureBoxStep6Top.Location = new Point(0, 0);
            pictureBoxStep6Top.Name = "pictureBoxStep6Top";
            pictureBoxStep6Top.Size = new Size(707, 85);
            pictureBoxStep6Top.TabIndex = 4;
            pictureBoxStep6Top.TabStop = false;
            // 
            // pictureBoxStep6Bottom
            // 
            pictureBoxStep6Bottom.Location = new Point(0, 454);
            pictureBoxStep6Bottom.Name = "pictureBoxStep6Bottom";
            pictureBoxStep6Bottom.Size = new Size(707, 59);
            pictureBoxStep6Bottom.TabIndex = 4;
            pictureBoxStep6Bottom.TabStop = false;
            // 
            // pictureBoxStep6Left
            // 
            pictureBoxStep6Left.Location = new Point(0, 85);
            pictureBoxStep6Left.Name = "pictureBoxStep6Left";
            pictureBoxStep6Left.Size = new Size(118, 369);
            pictureBoxStep6Left.TabIndex = 5;
            pictureBoxStep6Left.TabStop = false;
            // 
            // pictureBoxStep6Background
            // 
            pictureBoxStep6Background.Location = new Point(118, 85);
            pictureBoxStep6Background.Name = "pictureBoxStep6Background";
            pictureBoxStep6Background.Size = new Size(589, 369);
            pictureBoxStep6Background.TabIndex = 5;
            pictureBoxStep6Background.TabStop = false;
            // 
            // panelStep6Left
            // 
            panelStep6Left.Location = new Point(0, 85);
            panelStep6Left.Name = "panelStep6Left";
            panelStep6Left.Size = new Size(118, 369);
            panelStep6Left.TabIndex = 6;
            // 
            // panelStep6Top
            // 
            panelStep6Top.Location = new Point(0, 0);
            panelStep6Top.Name = "panelStep6Top";
            panelStep6Top.Size = new Size(707, 85);
            panelStep6Top.TabIndex = 6;
            // 
            // pictureBoxStep6TopRight
            // 
            pictureBoxStep6TopRight.BackColor = Color.Transparent;
            pictureBoxStep6TopRight.Location = new Point(622, 0);
            pictureBoxStep6TopRight.Name = "pictureBoxStep6TopRight";
            pictureBoxStep6TopRight.Size = new Size(85, 85);
            pictureBoxStep6TopRight.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStep6TopRight.TabIndex = 4;
            pictureBoxStep6TopRight.TabStop = false;
            // 
            // panelStep6Bottom
            // 
            panelStep6Bottom.Location = new Point(0, 454);
            panelStep6Bottom.Name = "panelStep6Bottom";
            panelStep6Bottom.Size = new Size(707, 59);
            panelStep6Bottom.TabIndex = 6;
            // 
            // panelStep6Background
            // 
            panelStep6Background.Location = new Point(118, 85);
            panelStep6Background.Name = "panelStep6Background";
            panelStep6Background.Size = new Size(589, 369);
            panelStep6Background.TabIndex = 6;
            // 
            // panelStep7
            // 
            panelStep7.Controls.Add(SummaryTitleTemplate);
            panelStep7.Controls.Add(labelSummaryText);
            panelStep7.Controls.Add(labelSummaryTitle);
            panelStep7.Controls.Add(txtSummary);
            panelStep7.Controls.Add(btnStep7Back);
            panelStep7.Controls.Add(btnCreate);
            panelStep7.Controls.Add(btnStep7Cancel);
            panelStep7.Controls.Add(pictureBoxStep7Top);
            panelStep7.Controls.Add(pictureBoxStep7Bottom);
            panelStep7.Controls.Add(pictureBoxStep7Left);
            panelStep7.Controls.Add(pictureBoxStep7Background);
            panelStep7.Controls.Add(panelStep7Left);
            panelStep7.Controls.Add(panelStep7Top);
            panelStep7.Controls.Add(pictureBoxStep7TopRight);
            panelStep7.Controls.Add(panelStep7Bottom);
            panelStep7.Controls.Add(panelStep7Background);
            panelStep7.Dock = DockStyle.Fill;
            panelStep7.Location = new Point(0, 0);
            panelStep7.Name = "panelStep7";
            panelStep7.Size = new Size(707, 513);
            panelStep7.TabIndex = 5;
            panelStep7.Visible = false;
            // 
            // SummaryTitleTemplate
            // 
            SummaryTitleTemplate.BackColor = Color.Transparent;
            SummaryTitleTemplate.Location = new Point(132, 147);
            SummaryTitleTemplate.Name = "SummaryTitleTemplate";
            SummaryTitleTemplate.Size = new Size(555, 20);
            SummaryTitleTemplate.TabIndex = 7;
            SummaryTitleTemplate.Text = "Summary of the configuration of the virtual machine {vmName}:";
            // 
            // labelSummaryText
            // 
            labelSummaryText.BackColor = Color.Transparent;
            labelSummaryText.Location = new Point(151, 117);
            labelSummaryText.Name = "labelSummaryText";
            labelSummaryText.Size = new Size(374, 20);
            labelSummaryText.TabIndex = 3;
            labelSummaryText.Text = "Review the configuration summary below before creation.";
            // 
            // labelSummaryTitle
            // 
            labelSummaryTitle.BackColor = Color.Transparent;
            labelSummaryTitle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelSummaryTitle.Location = new Point(136, 96);
            labelSummaryTitle.Name = "labelSummaryTitle";
            labelSummaryTitle.Size = new Size(464, 21);
            labelSummaryTitle.TabIndex = 4;
            labelSummaryTitle.Text = "Ready to Create the Virtual Machine";
            // 
            // txtSummary
            // 
            txtSummary.Location = new Point(132, 170);
            txtSummary.Multiline = true;
            txtSummary.Name = "txtSummary";
            txtSummary.ReadOnly = true;
            txtSummary.ScrollBars = ScrollBars.Vertical;
            txtSummary.Size = new Size(555, 274);
            txtSummary.TabIndex = 3;
            // 
            // btnStep7Back
            // 
            btnStep7Back.BackColor = Color.FromArgb(245, 245, 245);
            btnStep7Back.FlatStyle = FlatStyle.Flat;
            btnStep7Back.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep7Back.Location = new Point(435, 478);
            btnStep7Back.Name = "btnStep7Back";
            btnStep7Back.Size = new Size(75, 23);
            btnStep7Back.TabIndex = 0;
            btnStep7Back.Text = "< Back";
            btnStep7Back.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            btnCreate.BackColor = Color.FromArgb(245, 245, 245);
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.ForeColor = Color.FromArgb(30, 30, 30);
            btnCreate.Location = new Point(516, 478);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(75, 23);
            btnCreate.TabIndex = 1;
            btnCreate.Text = "Create";
            btnCreate.UseVisualStyleBackColor = true;
            // 
            // btnStep7Cancel
            // 
            btnStep7Cancel.BackColor = Color.FromArgb(245, 245, 245);
            btnStep7Cancel.FlatStyle = FlatStyle.Flat;
            btnStep7Cancel.ForeColor = Color.FromArgb(30, 30, 30);
            btnStep7Cancel.Location = new Point(620, 478);
            btnStep7Cancel.Name = "btnStep7Cancel";
            btnStep7Cancel.Size = new Size(75, 23);
            btnStep7Cancel.TabIndex = 2;
            btnStep7Cancel.Text = "Cancel";
            btnStep7Cancel.UseVisualStyleBackColor = true;
            // 
            // pictureBoxStep7Top
            // 
            pictureBoxStep7Top.Location = new Point(0, 0);
            pictureBoxStep7Top.Name = "pictureBoxStep7Top";
            pictureBoxStep7Top.Size = new Size(707, 85);
            pictureBoxStep7Top.TabIndex = 4;
            pictureBoxStep7Top.TabStop = false;
            // 
            // pictureBoxStep7Bottom
            // 
            pictureBoxStep7Bottom.Location = new Point(0, 454);
            pictureBoxStep7Bottom.Name = "pictureBoxStep7Bottom";
            pictureBoxStep7Bottom.Size = new Size(707, 59);
            pictureBoxStep7Bottom.TabIndex = 4;
            pictureBoxStep7Bottom.TabStop = false;
            // 
            // pictureBoxStep7Left
            // 
            pictureBoxStep7Left.Location = new Point(0, 85);
            pictureBoxStep7Left.Name = "pictureBoxStep7Left";
            pictureBoxStep7Left.Size = new Size(118, 369);
            pictureBoxStep7Left.TabIndex = 5;
            pictureBoxStep7Left.TabStop = false;
            // 
            // pictureBoxStep7Background
            // 
            pictureBoxStep7Background.Location = new Point(118, 85);
            pictureBoxStep7Background.Name = "pictureBoxStep7Background";
            pictureBoxStep7Background.Size = new Size(589, 369);
            pictureBoxStep7Background.TabIndex = 5;
            pictureBoxStep7Background.TabStop = false;
            // 
            // panelStep7Left
            // 
            panelStep7Left.Location = new Point(0, 85);
            panelStep7Left.Name = "panelStep7Left";
            panelStep7Left.Size = new Size(118, 369);
            panelStep7Left.TabIndex = 6;
            // 
            // panelStep7Top
            // 
            panelStep7Top.Location = new Point(0, 0);
            panelStep7Top.Name = "panelStep7Top";
            panelStep7Top.Size = new Size(707, 85);
            panelStep7Top.TabIndex = 6;
            // 
            // pictureBoxStep7TopRight
            // 
            pictureBoxStep7TopRight.BackColor = Color.Transparent;
            pictureBoxStep7TopRight.Location = new Point(622, 0);
            pictureBoxStep7TopRight.Name = "pictureBoxStep7TopRight";
            pictureBoxStep7TopRight.Size = new Size(85, 85);
            pictureBoxStep7TopRight.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStep7TopRight.TabIndex = 4;
            pictureBoxStep7TopRight.TabStop = false;
            // 
            // panelStep7Bottom
            // 
            panelStep7Bottom.Location = new Point(0, 454);
            panelStep7Bottom.Name = "panelStep7Bottom";
            panelStep7Bottom.Size = new Size(707, 59);
            panelStep7Bottom.TabIndex = 6;
            // 
            // panelStep7Background
            // 
            panelStep7Background.Location = new Point(118, 85);
            panelStep7Background.Name = "panelStep7Background";
            panelStep7Background.Size = new Size(589, 369);
            panelStep7Background.TabIndex = 6;
            // 
            // panelFinish
            // 
            panelFinish.Controls.Add(checkBoxOpenFolder);
            panelFinish.Controls.Add(checkBoxStartVM);
            panelFinish.Controls.Add(buttonFinish);
            panelFinish.Controls.Add(labelFinishText);
            panelFinish.Controls.Add(labelFinishTitle);
            panelFinish.Controls.Add(pictureBoxFinishTop);
            panelFinish.Controls.Add(pictureBoxFinishBottom);
            panelFinish.Controls.Add(pictureBoxFinishLeft);
            panelFinish.Controls.Add(pictureBoxFinishBackground);
            panelFinish.Controls.Add(panelFinishLeft);
            panelFinish.Controls.Add(panelFinishTop);
            panelFinish.Controls.Add(pictureBoxFinishTopRight);
            panelFinish.Controls.Add(panelFinishBottom);
            panelFinish.Controls.Add(panelFinishBackground);
            panelFinish.Dock = DockStyle.Fill;
            panelFinish.Location = new Point(0, 0);
            panelFinish.Name = "panelFinish";
            panelFinish.Size = new Size(707, 513);
            panelFinish.TabIndex = 4;
            panelFinish.Visible = false;
            // 
            // checkBoxOpenFolder
            // 
            checkBoxOpenFolder.AutoSize = true;
            checkBoxOpenFolder.BackColor = Color.Transparent;
            checkBoxOpenFolder.Location = new Point(241, 299);
            checkBoxOpenFolder.Name = "checkBoxOpenFolder";
            checkBoxOpenFolder.Size = new Size(228, 19);
            checkBoxOpenFolder.TabIndex = 10;
            checkBoxOpenFolder.Text = "Open the folder of the virtual machine";
            checkBoxOpenFolder.UseVisualStyleBackColor = false;
            // 
            // checkBoxStartVM
            // 
            checkBoxStartVM.AutoSize = true;
            checkBoxStartVM.BackColor = Color.Transparent;
            checkBoxStartVM.Checked = true;
            checkBoxStartVM.CheckState = CheckState.Checked;
            checkBoxStartVM.Location = new Point(241, 268);
            checkBoxStartVM.Name = "checkBoxStartVM";
            checkBoxStartVM.Size = new Size(155, 19);
            checkBoxStartVM.TabIndex = 10;
            checkBoxStartVM.Text = "Start the virtual machine";
            checkBoxStartVM.UseVisualStyleBackColor = false;
            // 
            // buttonFinish
            // 
            buttonFinish.BackColor = Color.FromArgb(245, 245, 245);
            buttonFinish.FlatStyle = FlatStyle.Flat;
            buttonFinish.ForeColor = Color.FromArgb(30, 30, 30);
            buttonFinish.Location = new Point(516, 478);
            buttonFinish.Name = "buttonFinish";
            buttonFinish.Size = new Size(75, 23);
            buttonFinish.TabIndex = 6;
            buttonFinish.Text = "Finish";
            buttonFinish.UseVisualStyleBackColor = true;
            // 
            // labelFinishText
            // 
            labelFinishText.BackColor = Color.Transparent;
            labelFinishText.Location = new Point(240, 186);
            labelFinishText.Name = "labelFinishText";
            labelFinishText.Size = new Size(385, 43);
            labelFinishText.TabIndex = 8;
            labelFinishText.Text = "You have successfully completed the VMware VM Generator wizard for {osName}.";
            // 
            // labelFinishTitle
            // 
            labelFinishTitle.BackColor = Color.Transparent;
            labelFinishTitle.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold);
            labelFinishTitle.Location = new Point(231, 96);
            labelFinishTitle.Name = "labelFinishTitle";
            labelFinishTitle.Size = new Size(464, 82);
            labelFinishTitle.TabIndex = 9;
            labelFinishTitle.Text = "Completing the VMware VM Generator wizard of {osName}.";
            // 
            // pictureBoxFinishTop
            // 
            pictureBoxFinishTop.Location = new Point(0, 0);
            pictureBoxFinishTop.Name = "pictureBoxFinishTop";
            pictureBoxFinishTop.Size = new Size(707, 85);
            pictureBoxFinishTop.TabIndex = 4;
            pictureBoxFinishTop.TabStop = false;
            // 
            // pictureBoxFinishBottom
            // 
            pictureBoxFinishBottom.Location = new Point(0, 454);
            pictureBoxFinishBottom.Name = "pictureBoxFinishBottom";
            pictureBoxFinishBottom.Size = new Size(707, 59);
            pictureBoxFinishBottom.TabIndex = 4;
            pictureBoxFinishBottom.TabStop = false;
            // 
            // pictureBoxFinishLeft
            // 
            pictureBoxFinishLeft.Location = new Point(0, 85);
            pictureBoxFinishLeft.Name = "pictureBoxFinishLeft";
            pictureBoxFinishLeft.Size = new Size(118, 369);
            pictureBoxFinishLeft.TabIndex = 5;
            pictureBoxFinishLeft.TabStop = false;
            // 
            // pictureBoxFinishBackground
            // 
            pictureBoxFinishBackground.Location = new Point(118, 85);
            pictureBoxFinishBackground.Name = "pictureBoxFinishBackground";
            pictureBoxFinishBackground.Size = new Size(589, 369);
            pictureBoxFinishBackground.TabIndex = 5;
            pictureBoxFinishBackground.TabStop = false;
            // 
            // panelFinishLeft
            // 
            panelFinishLeft.Location = new Point(0, 85);
            panelFinishLeft.Name = "panelFinishLeft";
            panelFinishLeft.Size = new Size(118, 369);
            panelFinishLeft.TabIndex = 6;
            // 
            // panelFinishTop
            // 
            panelFinishTop.Location = new Point(0, 0);
            panelFinishTop.Name = "panelFinishTop";
            panelFinishTop.Size = new Size(707, 85);
            panelFinishTop.TabIndex = 6;
            // 
            // pictureBoxFinishTopRight
            // 
            pictureBoxFinishTopRight.BackColor = Color.Transparent;
            pictureBoxFinishTopRight.Location = new Point(622, 0);
            pictureBoxFinishTopRight.Name = "pictureBoxFinishTopRight";
            pictureBoxFinishTopRight.Size = new Size(85, 85);
            pictureBoxFinishTopRight.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxFinishTopRight.TabIndex = 4;
            pictureBoxFinishTopRight.TabStop = false;
            // 
            // panelFinishBottom
            // 
            panelFinishBottom.Location = new Point(0, 454);
            panelFinishBottom.Name = "panelFinishBottom";
            panelFinishBottom.Size = new Size(707, 59);
            panelFinishBottom.TabIndex = 6;
            // 
            // panelFinishBackground
            // 
            panelFinishBackground.Controls.Add(iconFinish);
            panelFinishBackground.Location = new Point(118, 85);
            panelFinishBackground.Name = "panelFinishBackground";
            panelFinishBackground.Size = new Size(589, 369);
            panelFinishBackground.TabIndex = 6;
            // 
            // iconFinish
            // 
            iconFinish.Location = new Point(48, 11);
            iconFinish.Name = "iconFinish";
            iconFinish.Size = new Size(32, 32);
            iconFinish.TabIndex = 0;
            iconFinish.TabStop = false;
            // 
            // Form1
            // 
            ClientSize = new Size(707, 513);
            Controls.Add(panelStep6);
            Controls.Add(panelStep1);
            Controls.Add(panelStep7);
            Controls.Add(panelStep4);
            Controls.Add(panelStep5);
            Controls.Add(panelStep3);
            Controls.Add(panelStep2);
            Controls.Add(panelWelcome);
            Controls.Add(panelFinish);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "VMware {osName} VM Generator";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDownFloppyCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCDIDECount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCDSCSICount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownCDSATACount).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconFirmware).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconProcessor).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconMemory).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconSound).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconSCSI).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconNetwork).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconFloppy).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconCDIDE).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconCDSCSI).EndInit();
            ((System.ComponentModel.ISupportInitialize)iconCDSATA).EndInit();
            panelWelcome.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxWelcomeTopRight).EndInit();
            panelWelcomeBackground.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)iconWelcome).EndInit();
            panelStep1.ResumeLayout(false);
            panelStep1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1Top).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1Bottom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1Left).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1TopRight).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep1Background).EndInit();
            panelStep1Background.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)iconFolder).EndInit();
            panelStep2.ResumeLayout(false);
            processors.ResumeLayout(false);
            processors.PerformLayout();
            memory.ResumeLayout(false);
            memory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMemory).EndInit();
            firmwareType.ResumeLayout(false);
            firmwareType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2Top).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2Bottom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2Left).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2Background).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep2TopRight).EndInit();
            panelStep3.ResumeLayout(false);
            groupBoxSoundCard.ResumeLayout(false);
            groupBoxSCSIIO.ResumeLayout(false);
            groupBoxSCSIIO.PerformLayout();
            groupBoxNetwork.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3Top).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3Bottom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3Left).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3Background).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep3TopRight).EndInit();
            panelStep4.ResumeLayout(false);
            panelStep4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownDriveCapacity).EndInit();
            groupBoxVirtualDriveType.ResumeLayout(false);
            groupBoxVirtualDriveType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4Top).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4Bottom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4Left).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4TopRight).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep4Background).EndInit();
            panelStep4Background.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)iconHardDrive).EndInit();
            panelStep5.ResumeLayout(false);
            panelStep5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5Top).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5Bottom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5Left).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5Background).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep5TopRight).EndInit();
            panelStep6.ResumeLayout(false);
            panelStep6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureVMIcon).EndInit();
            groupBoxShortcutType.ResumeLayout(false);
            groupBoxShortcutType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6Top).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6Bottom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6Left).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6Background).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep6TopRight).EndInit();
            panelStep7.ResumeLayout(false);
            panelStep7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7Top).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7Bottom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7Left).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7Background).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStep7TopRight).EndInit();
            panelFinish.ResumeLayout(false);
            panelFinish.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishTop).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishBottom).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishLeft).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishBackground).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFinishTopRight).EndInit();
            panelFinishBackground.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)iconFinish).EndInit();
            ResumeLayout(false);
        }

        #endregion

        // Panels and basic controls
        private Panel panelWelcome;
        private CustomButton btnWelcomeBack;
        private CustomButton btnWelcomeNext;
        private CustomButton btnWelcomeCancel;
        private Label labelWelcomeTitle;
        private Label labelWelcomeText;

        private Panel panelStep1;
        private Label labelStep1Title;
        private Label labelStep1Text;
        private CustomButton btnStep1Back;
        private CustomButton btnStep1Next;
        private CustomButton btnStep1Cancel;
        private TextBox txtVMName;
        private Label labelVMName;
        private Label labelVMFolder;
        private TextBox txtVMFolder;
        private CustomButton btnBrowseFolder;

        private Panel panelStep2;
        private Label labelStep2Title;
        private Label labelStep2Text;
        private CustomButton btnStep2Back;
        private CustomButton btnStep2Next;
        private CustomButton btnStep2Cancel;
        private GroupBox firmwareType;
        private RadioButton uefi;
        private RadioButton bios;
        private GroupBox processors;
        private Label labelNumberOfProcessors;
        private ComboBox comboBoxNumberOfCoresPerProcessor;
        private ComboBox comboNumberOfProcessors;
        private Label labelNumberOfCoresPerProcessor;
        private Label labelTotalOfCoresX;
        private Label labelTotalOfCores;
        private GroupBox memory;
        private Label labelMemory;
        private NumericUpDown numericUpDownMemory;
        private CustomTrackBar trackBarMemory;

        private Label labelRecommendedMin;
        private Label labelRecommendedDefault;
        private Label labelRecommendedMax;

        private Panel panelStep3;
        private Label labelStep3Title;
        private Label labelStep3Text;
        private CustomButton btnStep3Back;
        private CustomButton btnStep3Next;
        private CustomButton btnStep3Cancel;

        private Panel panelStep4;
        private Label labelStep4Title;
        private Label labelStep4Text;
        private CustomButton btnStep4Back;
        private CustomButton btnStep4Next;
        private CustomButton btnStep4Cancel;

        private Panel panelStep5;
        private Label labelStep5Title;
        private Label labelStep5Text;
        private CustomButton btnStep5Back;
        private CustomButton btnStep5Next;
        private CustomButton btnStep5Cancel;

        private Panel panelStep6;
        private Label labelStep6Title;
        private Label labelStep6Text;
        private CustomButton btnStep6Back;
        private CustomButton btnStep6Next;
        private CustomButton btnStep6Cancel;
        private PictureBox pictureVMIcon;
        private Label labelVMIcon;
        private GroupBox groupBoxShortcutType;
        private RadioButton radioShortcutDirect;
        private RadioButton radioShortcutKvm;
        private RadioButton radioShortcutWorkstation;
        private CheckBox checkBoxStartMenu;
        private CheckBox checkBoxDesktop;
        private CustomButton buttonChangeIcon;

        private Panel panelStep7;
        private Label labelSummaryTitle;
        private Label labelSummaryText;
        private TextBox txtSummary;
        private CustomButton btnStep7Back;
        private CustomButton btnCreate;
        private CustomButton btnStep7Cancel;

        private GroupBox groupBoxSoundCard;
        private GroupBox groupBoxSCSIIO;
        private GroupBox groupBoxNetwork;
        private RadioButton radioButtonBridged;
        private RadioButton radioButtonNAT;
        private RadioButton radioButtonNoNetwork;
        private RadioButton radioButtonHostOnly;
        private ToolTip toolTip1;
        private RadioButton radioBusLogic;
        private RadioButton radioParavirtualizedSCSI;
        private RadioButton radioLSILogicSAS;
        private RadioButton radioLSILogic;
        private RadioButton radioButtonSB16;
        private RadioButton radioButtonHDAudio;
        private RadioButton radioButtonES1371;
        private GroupBox groupBoxVirtualDriveType;
        private RadioButton radioButtonNVMe;
        private RadioButton radioButtonSATA;
        private RadioButton radioButtonSCSI;
        private RadioButton radioButtonIDE;
        private NumericUpDown numericUpDownDriveCapacity;
        private Label labelRecommendedDriveCapacity;
        private Label labelDriveCapacity;
        private CheckBox checkBoxAllocate;
        private RadioButton radioButtonSingle;
        private Label labelAllocate;
        private RadioButton radioButtonSplit;
        private Label labelSplit;
        private Label labelNumberOfCDSATA;
        private Label labelNumberOfCDSCSI;
        private Label labelNumberOfCDIDE;
        private Label labelNumberOfFloppy;
        private NumericUpDown numericUpDownFloppyCount;
        private NumericUpDown numericUpDownCDIDECount;
        private NumericUpDown numericUpDownCDSCSICount;
        private NumericUpDown numericUpDownCDSATACount;
        private Label labelUsedCDController;
        private Label labelCDLimit;

        private Panel panelFinish;
        private CustomButton buttonFinish;
        private Label labelFinishText;
        private Label labelFinishTitle;
        private CheckBox checkBoxOpenFolder;
        private CheckBox checkBoxStartVM;
        private CheckBox checkBoxShortStart;
        private CheckBox checkBoxNewWindow;
        private CheckBox checkBoxFullScreen;
        private CheckBox checkBoxClose;

        // Background PictureBoxes
        private PictureBox pictureBoxWelcomeTopRight;

        private PictureBox pictureBoxStep1TopRight;
        private PictureBox pictureBoxStep1Top;
        private PictureBox pictureBoxStep1Bottom;
        private PictureBox pictureBoxStep1Left;
        private PictureBox pictureBoxStep1Background;

        private PictureBox pictureBoxStep2TopRight;
        private PictureBox pictureBoxStep2Top;
        private PictureBox pictureBoxStep2Bottom;
        private PictureBox pictureBoxStep2Left;
        private PictureBox pictureBoxStep2Background;

        private PictureBox pictureBoxStep3TopRight;
        private PictureBox pictureBoxStep3Top;
        private PictureBox pictureBoxStep3Bottom;
        private PictureBox pictureBoxStep3Left;
        private PictureBox pictureBoxStep3Background;

        private PictureBox pictureBoxStep4TopRight;
        private PictureBox pictureBoxStep4Top;
        private PictureBox pictureBoxStep4Bottom;
        private PictureBox pictureBoxStep4Left;
        private PictureBox pictureBoxStep4Background;

        private PictureBox pictureBoxStep5TopRight;
        private PictureBox pictureBoxStep5Top;
        private PictureBox pictureBoxStep5Bottom;
        private PictureBox pictureBoxStep5Left;
        private PictureBox pictureBoxStep5Background;

        private PictureBox pictureBoxStep6TopRight;
        private PictureBox pictureBoxStep6Top;
        private PictureBox pictureBoxStep6Bottom;
        private PictureBox pictureBoxStep6Left;
        private PictureBox pictureBoxStep6Background;

        private PictureBox pictureBoxStep7TopRight;
        private PictureBox pictureBoxStep7Top;
        private PictureBox pictureBoxStep7Bottom;
        private PictureBox pictureBoxStep7Left;
        private PictureBox pictureBoxStep7Background;

        private PictureBox pictureBoxFinishTopRight;
        private PictureBox pictureBoxFinishTop;
        private PictureBox pictureBoxFinishBottom;
        private PictureBox pictureBoxFinishLeft;
        private PictureBox pictureBoxFinishBackground;

        // Layout Panels matching PictureBox names per step
        private Panel panelWelcomeBackground, panelWelcomeLeft, panelWelcomeTop, panelWelcomeBottom;
        private Panel panelStep1Background, panelStep1Left, panelStep1Top, panelStep1Bottom;
        private Panel panelStep2Background, panelStep2Left, panelStep2Top, panelStep2Bottom;
        private Panel panelStep3Background, panelStep3Left, panelStep3Top, panelStep3Bottom;
        private Panel panelStep4Background, panelStep4Left, panelStep4Top, panelStep4Bottom;
        private Panel panelStep5Background, panelStep5Left, panelStep5Top, panelStep5Bottom;
        private Panel panelStep6Background, panelStep6Left, panelStep6Top, panelStep6Bottom;
        private Panel panelStep7Background, panelStep7Left, panelStep7Top, panelStep7Bottom;
        private Panel panelFinishBackground, panelFinishLeft, panelFinishTop, panelFinishBottom;
        private Label SummaryTitleTemplate;
        private PictureBox iconWelcome;
        private PictureBox iconFinish;
        private PictureBox iconFirmware;
        private PictureBox iconProcessor;
        private PictureBox iconMemory;
        private PictureBox iconSound;
        private PictureBox iconSCSI;
        private PictureBox iconNetwork;
        private PictureBox iconFloppy;
        private PictureBox iconCDIDE;
        private PictureBox iconCDSCSI;
        private PictureBox iconCDSATA;
        private PictureBox iconHardDrive;
        private PictureBox iconFolder;
    }
}
