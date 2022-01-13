/*
 * MIT License
 * 
 * Copyright (c) 2022 plexdata.de
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Plexdata.FileWiper
{
    partial class SettingsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.btnDefault = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tabSettings = new Plexdata.FileWiper.TabControlEx();
            this.tcpBehaviour = new System.Windows.Forms.TabPage();
            this.chkIncludeFolderNames = new Plexdata.FileWiper.FocusCheckBox();
            this.txtBehaviourDescription = new Plexdata.FileWiper.RichTextBoxEx();
            this.lblBehaviourDescription = new System.Windows.Forms.Label();
            this.chkAllowAutoRelaunch = new Plexdata.FileWiper.FocusCheckBox();
            this.chkSuppressCancelQuestion = new Plexdata.FileWiper.FocusCheckBox();
            this.chkAutoPauseWiping = new Plexdata.FileWiper.FocusCheckBox();
            this.chkUseFullResources = new Plexdata.FileWiper.FocusCheckBox();
            this.chkAllowAutoClose = new Plexdata.FileWiper.FocusCheckBox();
            this.tcpWipingSettings = new System.Windows.Forms.TabPage();
            this.grpAlgorithm = new System.Windows.Forms.GroupBox();
            this.cmbAlgorithmAlgorithm = new System.Windows.Forms.ComboBox();
            this.numAlgorithmRepeats = new System.Windows.Forms.NumericUpDown();
            this.lblAlgorithmDescription = new System.Windows.Forms.Label();
            this.txtAlgorithmDescription = new Plexdata.FileWiper.RichTextBoxEx();
            this.lblAlgorithmRepeats = new System.Windows.Forms.Label();
            this.lblAlgorithmAlgorithm = new System.Windows.Forms.Label();
            this.grpParallelProcessing = new System.Windows.Forms.GroupBox();
            this.lblParallelWipings = new System.Windows.Forms.Label();
            this.numParallelWipings = new System.Windows.Forms.NumericUpDown();
            this.chkParallelProcessing = new System.Windows.Forms.CheckBox();
            this.tcpShellExtension = new System.Windows.Forms.TabPage();
            this.lstIconShow = new System.Windows.Forms.ListView();
            this.txtHelpString = new System.Windows.Forms.TextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.lblHelpString = new System.Windows.Forms.Label();
            this.btnUnregister = new System.Windows.Forms.Button();
            this.txtDisplayText = new System.Windows.Forms.TextBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.lblDisplayText = new System.Windows.Forms.Label();
            this.chkEnableIcon = new System.Windows.Forms.CheckBox();
            this.tabSettings.SuspendLayout();
            this.tcpBehaviour.SuspendLayout();
            this.tcpWipingSettings.SuspendLayout();
            this.grpAlgorithm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAlgorithmRepeats)).BeginInit();
            this.grpParallelProcessing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numParallelWipings)).BeginInit();
            this.tcpShellExtension.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDefault
            // 
            this.btnDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefault.Location = new System.Drawing.Point(104, 302);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(75, 25);
            this.btnDefault.TabIndex = 1;
            this.btnDefault.Text = "&Default";
            this.btnDefault.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.OnDefaultButtonClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(266, 302);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(185, 302);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            this.btnOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSettings.Controls.Add(this.tcpBehaviour);
            this.tabSettings.Controls.Add(this.tcpWipingSettings);
            this.tabSettings.Controls.Add(this.tcpShellExtension);
            this.tabSettings.Location = new System.Drawing.Point(12, 12);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(329, 284);
            this.tabSettings.TabIndex = 0;
            // 
            // tcpBehaviour
            // 
            this.tcpBehaviour.Controls.Add(this.chkIncludeFolderNames);
            this.tcpBehaviour.Controls.Add(this.txtBehaviourDescription);
            this.tcpBehaviour.Controls.Add(this.lblBehaviourDescription);
            this.tcpBehaviour.Controls.Add(this.chkAllowAutoRelaunch);
            this.tcpBehaviour.Controls.Add(this.chkSuppressCancelQuestion);
            this.tcpBehaviour.Controls.Add(this.chkAutoPauseWiping);
            this.tcpBehaviour.Controls.Add(this.chkUseFullResources);
            this.tcpBehaviour.Controls.Add(this.chkAllowAutoClose);
            this.tcpBehaviour.Location = new System.Drawing.Point(4, 22);
            this.tcpBehaviour.Name = "tcpBehaviour";
            this.tcpBehaviour.Padding = new System.Windows.Forms.Padding(3);
            this.tcpBehaviour.Size = new System.Drawing.Size(321, 258);
            this.tcpBehaviour.TabIndex = 2;
            this.tcpBehaviour.Text = "Behaviour";
            this.tcpBehaviour.UseVisualStyleBackColor = true;
            // 
            // chkIncludeFolderNames
            // 
            this.chkIncludeFolderNames.AutoSize = true;
            this.chkIncludeFolderNames.Location = new System.Drawing.Point(9, 54);
            this.chkIncludeFolderNames.Name = "chkIncludeFolderNames";
            this.chkIncludeFolderNames.Size = new System.Drawing.Size(129, 17);
            this.chkIncludeFolderNames.TabIndex = 2;
            this.chkIncludeFolderNames.Tag = resources.GetString("chkIncludeFolderNames.Tag");
            this.chkIncludeFolderNames.Text = "Include Folder Names";
            this.chkIncludeFolderNames.UseVisualStyleBackColor = true;
            // 
            // txtBehaviourDescription
            // 
            this.txtBehaviourDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBehaviourDescription.BackColor = System.Drawing.Color.White;
            this.txtBehaviourDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBehaviourDescription.Location = new System.Drawing.Point(9, 167);
            this.txtBehaviourDescription.Name = "txtBehaviourDescription";
            this.txtBehaviourDescription.ReadOnly = true;
            this.txtBehaviourDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtBehaviourDescription.Size = new System.Drawing.Size(305, 83);
            this.txtBehaviourDescription.TabIndex = 7;
            this.txtBehaviourDescription.TabStop = false;
            this.txtBehaviourDescription.Text = "";
            // 
            // lblBehaviourDescription
            // 
            this.lblBehaviourDescription.AutoSize = true;
            this.lblBehaviourDescription.Location = new System.Drawing.Point(6, 151);
            this.lblBehaviourDescription.Name = "lblBehaviourDescription";
            this.lblBehaviourDescription.Size = new System.Drawing.Size(60, 13);
            this.lblBehaviourDescription.TabIndex = 6;
            this.lblBehaviourDescription.Text = "Description";
            // 
            // chkAllowAutoRelaunch
            // 
            this.chkAllowAutoRelaunch.AutoSize = true;
            this.chkAllowAutoRelaunch.Location = new System.Drawing.Point(9, 100);
            this.chkAllowAutoRelaunch.Name = "chkAllowAutoRelaunch";
            this.chkAllowAutoRelaunch.Size = new System.Drawing.Size(125, 17);
            this.chkAllowAutoRelaunch.TabIndex = 4;
            this.chkAllowAutoRelaunch.Tag = resources.GetString("chkAllowAutoRelaunch.Tag");
            this.chkAllowAutoRelaunch.Text = "Allow Auto Relaunch";
            this.chkAllowAutoRelaunch.UseVisualStyleBackColor = true;
            // 
            // chkSuppressCancelQuestion
            // 
            this.chkSuppressCancelQuestion.AutoSize = true;
            this.chkSuppressCancelQuestion.Location = new System.Drawing.Point(9, 123);
            this.chkSuppressCancelQuestion.Name = "chkSuppressCancelQuestion";
            this.chkSuppressCancelQuestion.Size = new System.Drawing.Size(151, 17);
            this.chkSuppressCancelQuestion.TabIndex = 5;
            this.chkSuppressCancelQuestion.Tag = resources.GetString("chkSuppressCancelQuestion.Tag");
            this.chkSuppressCancelQuestion.Text = "Suppress Cancel Question";
            this.chkSuppressCancelQuestion.UseVisualStyleBackColor = true;
            // 
            // chkAutoPauseWiping
            // 
            this.chkAutoPauseWiping.AutoSize = true;
            this.chkAutoPauseWiping.Location = new System.Drawing.Point(9, 77);
            this.chkAutoPauseWiping.Name = "chkAutoPauseWiping";
            this.chkAutoPauseWiping.Size = new System.Drawing.Size(117, 17);
            this.chkAutoPauseWiping.TabIndex = 3;
            this.chkAutoPauseWiping.Tag = "Enable this option to perform an automatically pausing of file wiping as soon as " +
    "the main window of the program becomes visible.";
            this.chkAutoPauseWiping.Text = "Auto Pause Wiping";
            this.chkAutoPauseWiping.UseVisualStyleBackColor = true;
            // 
            // chkUseFullResources
            // 
            this.chkUseFullResources.AutoSize = true;
            this.chkUseFullResources.Location = new System.Drawing.Point(9, 31);
            this.chkUseFullResources.Name = "chkUseFullResources";
            this.chkUseFullResources.Size = new System.Drawing.Size(118, 17);
            this.chkUseFullResources.TabIndex = 1;
            this.chkUseFullResources.Tag = resources.GetString("chkUseFullResources.Tag");
            this.chkUseFullResources.Text = "Use Full Resources";
            this.chkUseFullResources.UseVisualStyleBackColor = true;
            // 
            // chkAllowAutoClose
            // 
            this.chkAllowAutoClose.AutoSize = true;
            this.chkAllowAutoClose.Location = new System.Drawing.Point(9, 8);
            this.chkAllowAutoClose.Name = "chkAllowAutoClose";
            this.chkAllowAutoClose.Size = new System.Drawing.Size(105, 17);
            this.chkAllowAutoClose.TabIndex = 0;
            this.chkAllowAutoClose.Tag = "Enable this option if you want to automatically close the program as soon as wipi" +
    "ng has finished. But note, the program is only closed if it is running minimized" +
    " and in background.";
            this.chkAllowAutoClose.Text = "Allow Auto Close";
            this.chkAllowAutoClose.UseVisualStyleBackColor = true;
            // 
            // tcpWipingSettings
            // 
            this.tcpWipingSettings.Controls.Add(this.grpAlgorithm);
            this.tcpWipingSettings.Controls.Add(this.grpParallelProcessing);
            this.tcpWipingSettings.Location = new System.Drawing.Point(4, 22);
            this.tcpWipingSettings.Name = "tcpWipingSettings";
            this.tcpWipingSettings.Padding = new System.Windows.Forms.Padding(6);
            this.tcpWipingSettings.Size = new System.Drawing.Size(321, 258);
            this.tcpWipingSettings.TabIndex = 0;
            this.tcpWipingSettings.Text = "Wiping Settings";
            this.tcpWipingSettings.UseVisualStyleBackColor = true;
            // 
            // grpAlgorithm
            // 
            this.grpAlgorithm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAlgorithm.Controls.Add(this.cmbAlgorithmAlgorithm);
            this.grpAlgorithm.Controls.Add(this.numAlgorithmRepeats);
            this.grpAlgorithm.Controls.Add(this.lblAlgorithmDescription);
            this.grpAlgorithm.Controls.Add(this.txtAlgorithmDescription);
            this.grpAlgorithm.Controls.Add(this.lblAlgorithmRepeats);
            this.grpAlgorithm.Controls.Add(this.lblAlgorithmAlgorithm);
            this.grpAlgorithm.Location = new System.Drawing.Point(9, 87);
            this.grpAlgorithm.Name = "grpAlgorithm";
            this.grpAlgorithm.Size = new System.Drawing.Size(303, 162);
            this.grpAlgorithm.TabIndex = 1;
            this.grpAlgorithm.TabStop = false;
            this.grpAlgorithm.Text = "Wiping Algorithm";
            // 
            // cmbAlgorithmAlgorithm
            // 
            this.cmbAlgorithmAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlgorithmAlgorithm.FormattingEnabled = true;
            this.cmbAlgorithmAlgorithm.Location = new System.Drawing.Point(72, 19);
            this.cmbAlgorithmAlgorithm.Name = "cmbAlgorithmAlgorithm";
            this.cmbAlgorithmAlgorithm.Size = new System.Drawing.Size(120, 21);
            this.cmbAlgorithmAlgorithm.TabIndex = 1;
            this.cmbAlgorithmAlgorithm.SelectedIndexChanged += new System.EventHandler(this.OnAlgorithmComboBoxSelectedIndexChanged);
            // 
            // numAlgorithmRepeats
            // 
            this.numAlgorithmRepeats.Location = new System.Drawing.Point(72, 46);
            this.numAlgorithmRepeats.Name = "numAlgorithmRepeats";
            this.numAlgorithmRepeats.Size = new System.Drawing.Size(120, 20);
            this.numAlgorithmRepeats.TabIndex = 3;
            this.numAlgorithmRepeats.ValueChanged += new System.EventHandler(this.OnRepeatsNumericSpinValueChanged);
            // 
            // lblAlgorithmDescription
            // 
            this.lblAlgorithmDescription.AutoSize = true;
            this.lblAlgorithmDescription.Location = new System.Drawing.Point(6, 75);
            this.lblAlgorithmDescription.Name = "lblAlgorithmDescription";
            this.lblAlgorithmDescription.Size = new System.Drawing.Size(60, 13);
            this.lblAlgorithmDescription.TabIndex = 4;
            this.lblAlgorithmDescription.Text = "Description";
            // 
            // txtAlgorithmDescription
            // 
            this.txtAlgorithmDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAlgorithmDescription.BackColor = System.Drawing.Color.White;
            this.txtAlgorithmDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAlgorithmDescription.Location = new System.Drawing.Point(72, 72);
            this.txtAlgorithmDescription.Name = "txtAlgorithmDescription";
            this.txtAlgorithmDescription.ReadOnly = true;
            this.txtAlgorithmDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtAlgorithmDescription.Size = new System.Drawing.Size(225, 84);
            this.txtAlgorithmDescription.TabIndex = 5;
            this.txtAlgorithmDescription.TabStop = false;
            this.txtAlgorithmDescription.Text = "";
            // 
            // lblAlgorithmRepeats
            // 
            this.lblAlgorithmRepeats.AutoSize = true;
            this.lblAlgorithmRepeats.Location = new System.Drawing.Point(6, 48);
            this.lblAlgorithmRepeats.Name = "lblAlgorithmRepeats";
            this.lblAlgorithmRepeats.Size = new System.Drawing.Size(47, 13);
            this.lblAlgorithmRepeats.TabIndex = 2;
            this.lblAlgorithmRepeats.Text = "Repeats";
            // 
            // lblAlgorithmAlgorithm
            // 
            this.lblAlgorithmAlgorithm.AutoSize = true;
            this.lblAlgorithmAlgorithm.Location = new System.Drawing.Point(6, 22);
            this.lblAlgorithmAlgorithm.Name = "lblAlgorithmAlgorithm";
            this.lblAlgorithmAlgorithm.Size = new System.Drawing.Size(50, 13);
            this.lblAlgorithmAlgorithm.TabIndex = 0;
            this.lblAlgorithmAlgorithm.Text = "Algorithm";
            // 
            // grpParallelProcessing
            // 
            this.grpParallelProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpParallelProcessing.Controls.Add(this.lblParallelWipings);
            this.grpParallelProcessing.Controls.Add(this.numParallelWipings);
            this.grpParallelProcessing.Controls.Add(this.chkParallelProcessing);
            this.grpParallelProcessing.Location = new System.Drawing.Point(9, 9);
            this.grpParallelProcessing.Name = "grpParallelProcessing";
            this.grpParallelProcessing.Size = new System.Drawing.Size(303, 72);
            this.grpParallelProcessing.TabIndex = 0;
            this.grpParallelProcessing.TabStop = false;
            this.grpParallelProcessing.Text = "Parallel Processing";
            // 
            // lblParallelWipings
            // 
            this.lblParallelWipings.AutoSize = true;
            this.lblParallelWipings.Location = new System.Drawing.Point(6, 48);
            this.lblParallelWipings.Name = "lblParallelWipings";
            this.lblParallelWipings.Size = new System.Drawing.Size(134, 13);
            this.lblParallelWipings.TabIndex = 1;
            this.lblParallelWipings.Text = "Number of Parallel Wipings";
            // 
            // numParallelWipings
            // 
            this.numParallelWipings.Location = new System.Drawing.Point(146, 46);
            this.numParallelWipings.Name = "numParallelWipings";
            this.numParallelWipings.Size = new System.Drawing.Size(120, 20);
            this.numParallelWipings.TabIndex = 2;
            // 
            // chkParallelProcessing
            // 
            this.chkParallelProcessing.AutoSize = true;
            this.chkParallelProcessing.Location = new System.Drawing.Point(9, 21);
            this.chkParallelProcessing.Name = "chkParallelProcessing";
            this.chkParallelProcessing.Size = new System.Drawing.Size(143, 17);
            this.chkParallelProcessing.TabIndex = 0;
            this.chkParallelProcessing.Text = "Allow Parallel Processing";
            this.chkParallelProcessing.UseVisualStyleBackColor = true;
            this.chkParallelProcessing.CheckedChanged += new System.EventHandler(this.OnParallelProcessingCheckedChanged);
            // 
            // tcpShellExtension
            // 
            this.tcpShellExtension.Controls.Add(this.lstIconShow);
            this.tcpShellExtension.Controls.Add(this.txtHelpString);
            this.tcpShellExtension.Controls.Add(this.btnRegister);
            this.tcpShellExtension.Controls.Add(this.lblHelpString);
            this.tcpShellExtension.Controls.Add(this.btnUnregister);
            this.tcpShellExtension.Controls.Add(this.txtDisplayText);
            this.tcpShellExtension.Controls.Add(this.lblWarning);
            this.tcpShellExtension.Controls.Add(this.lblDisplayText);
            this.tcpShellExtension.Controls.Add(this.chkEnableIcon);
            this.tcpShellExtension.Location = new System.Drawing.Point(4, 22);
            this.tcpShellExtension.Name = "tcpShellExtension";
            this.tcpShellExtension.Padding = new System.Windows.Forms.Padding(6);
            this.tcpShellExtension.Size = new System.Drawing.Size(321, 258);
            this.tcpShellExtension.TabIndex = 1;
            this.tcpShellExtension.Text = "Shell Extension";
            this.tcpShellExtension.UseVisualStyleBackColor = true;
            // 
            // lstIconShow
            // 
            this.lstIconShow.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lstIconShow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstIconShow.BackColor = System.Drawing.Color.White;
            this.lstIconShow.HideSelection = false;
            this.lstIconShow.Location = new System.Drawing.Point(112, 100);
            this.lstIconShow.MultiSelect = false;
            this.lstIconShow.Name = "lstIconShow";
            this.lstIconShow.Size = new System.Drawing.Size(200, 120);
            this.lstIconShow.TabIndex = 6;
            this.lstIconShow.UseCompatibleStateImageBehavior = false;
            // 
            // txtHelpString
            // 
            this.txtHelpString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHelpString.BackColor = System.Drawing.Color.White;
            this.txtHelpString.Location = new System.Drawing.Point(112, 74);
            this.txtHelpString.Name = "txtHelpString";
            this.txtHelpString.Size = new System.Drawing.Size(200, 20);
            this.txtHelpString.TabIndex = 4;
            // 
            // btnRegister
            // 
            this.btnRegister.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRegister.Location = new System.Drawing.Point(112, 226);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(97, 23);
            this.btnRegister.TabIndex = 7;
            this.btnRegister.Text = "&Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.OnRegisterButtonClick);
            // 
            // lblHelpString
            // 
            this.lblHelpString.AutoSize = true;
            this.lblHelpString.Location = new System.Drawing.Point(9, 77);
            this.lblHelpString.Name = "lblHelpString";
            this.lblHelpString.Size = new System.Drawing.Size(59, 13);
            this.lblHelpString.TabIndex = 3;
            this.lblHelpString.Text = "Help String";
            // 
            // btnUnregister
            // 
            this.btnUnregister.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnregister.Location = new System.Drawing.Point(215, 226);
            this.btnUnregister.Name = "btnUnregister";
            this.btnUnregister.Size = new System.Drawing.Size(97, 23);
            this.btnUnregister.TabIndex = 8;
            this.btnUnregister.Text = "&Unregister";
            this.btnUnregister.UseVisualStyleBackColor = true;
            this.btnUnregister.Click += new System.EventHandler(this.OnUnregisterButtonClick);
            // 
            // txtDisplayText
            // 
            this.txtDisplayText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDisplayText.BackColor = System.Drawing.Color.White;
            this.txtDisplayText.Location = new System.Drawing.Point(112, 48);
            this.txtDisplayText.Name = "txtDisplayText";
            this.txtDisplayText.Size = new System.Drawing.Size(200, 20);
            this.txtDisplayText.TabIndex = 2;
            // 
            // lblWarning
            // 
            this.lblWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.Location = new System.Drawing.Point(9, 6);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(303, 39);
            this.lblWarning.TabIndex = 0;
            this.lblWarning.Text = "Applying the settings on this property page requires administrator privileges!";
            this.lblWarning.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblDisplayText
            // 
            this.lblDisplayText.AutoSize = true;
            this.lblDisplayText.Location = new System.Drawing.Point(9, 51);
            this.lblDisplayText.Name = "lblDisplayText";
            this.lblDisplayText.Size = new System.Drawing.Size(65, 13);
            this.lblDisplayText.TabIndex = 1;
            this.lblDisplayText.Text = "Display Text";
            // 
            // chkEnableIcon
            // 
            this.chkEnableIcon.AutoSize = true;
            this.chkEnableIcon.Checked = true;
            this.chkEnableIcon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableIcon.Location = new System.Drawing.Point(12, 103);
            this.chkEnableIcon.Name = "chkEnableIcon";
            this.chkEnableIcon.Size = new System.Drawing.Size(83, 17);
            this.chkEnableIcon.TabIndex = 5;
            this.chkEnableIcon.Text = "Enable Icon";
            this.chkEnableIcon.UseVisualStyleBackColor = true;
            this.chkEnableIcon.CheckedChanged += new System.EventHandler(this.OnEnableIconUsageCheckedChanged);
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(353, 339);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabSettings);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(369, 377);
            this.Name = "SettingsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.tabSettings.ResumeLayout(false);
            this.tcpBehaviour.ResumeLayout(false);
            this.tcpBehaviour.PerformLayout();
            this.tcpWipingSettings.ResumeLayout(false);
            this.grpAlgorithm.ResumeLayout(false);
            this.grpAlgorithm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAlgorithmRepeats)).EndInit();
            this.grpParallelProcessing.ResumeLayout(false);
            this.grpParallelProcessing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numParallelWipings)).EndInit();
            this.tcpShellExtension.ResumeLayout(false);
            this.tcpShellExtension.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Plexdata.FileWiper.TabControlEx tabSettings;
        private System.Windows.Forms.TabPage tcpWipingSettings;
        private System.Windows.Forms.TabPage tcpShellExtension;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ListView lstIconShow;
        private System.Windows.Forms.TextBox txtHelpString;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Label lblHelpString;
        private System.Windows.Forms.Button btnUnregister;
        private System.Windows.Forms.TextBox txtDisplayText;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Label lblDisplayText;
        private System.Windows.Forms.CheckBox chkEnableIcon;
        private System.Windows.Forms.GroupBox grpAlgorithm;
        private System.Windows.Forms.ComboBox cmbAlgorithmAlgorithm;
        private System.Windows.Forms.NumericUpDown numAlgorithmRepeats;
        private System.Windows.Forms.Label lblAlgorithmDescription;
        private Plexdata.FileWiper.RichTextBoxEx txtAlgorithmDescription;
        private System.Windows.Forms.Label lblAlgorithmRepeats;
        private System.Windows.Forms.Label lblAlgorithmAlgorithm;
        private System.Windows.Forms.GroupBox grpParallelProcessing;
        private System.Windows.Forms.Label lblParallelWipings;
        private System.Windows.Forms.NumericUpDown numParallelWipings;
        private System.Windows.Forms.CheckBox chkParallelProcessing;
        private System.Windows.Forms.TabPage tcpBehaviour;
        private Plexdata.FileWiper.FocusCheckBox chkIncludeFolderNames;
        private Plexdata.FileWiper.RichTextBoxEx txtBehaviourDescription;
        private System.Windows.Forms.Label lblBehaviourDescription;
        private Plexdata.FileWiper.FocusCheckBox chkAllowAutoRelaunch;
        private Plexdata.FileWiper.FocusCheckBox chkSuppressCancelQuestion;
        private Plexdata.FileWiper.FocusCheckBox chkAutoPauseWiping;
        private Plexdata.FileWiper.FocusCheckBox chkUseFullResources;
        private Plexdata.FileWiper.FocusCheckBox chkAllowAutoClose;
    }
}