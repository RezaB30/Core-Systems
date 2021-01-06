
namespace RadiusR.FileManagement.TestUnit
{
    partial class MainForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.UploadAttachmentsButton = new System.Windows.Forms.Button();
            this.AddLocalAttachment = new System.Windows.Forms.Button();
            this.LocalAttachmentsListbox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ClientAttachmentsListbox = new System.Windows.Forms.ListBox();
            this.GetAttachmentsButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ArchiveNoNumeric = new System.Windows.Forms.NumericUpDown();
            this.UploadAttachmentTypeCombobox = new System.Windows.Forms.ComboBox();
            this.MainTabcontrol = new System.Windows.Forms.TabControl();
            this.ClientAttachmentsTabpage = new System.Windows.Forms.TabPage();
            this.AttachmentDetailsPanel = new System.Windows.Forms.Panel();
            this.DeleteClientAttachmentButton = new System.Windows.Forms.Button();
            this.AttachmentFileNameTextbox = new System.Windows.Forms.TextBox();
            this.AttachmentExtentionLabel = new System.Windows.Forms.Label();
            this.AttachmentCreationDateLabel = new System.Windows.Forms.Label();
            this.AttachmentTypeLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SaveClientAttachmentButton = new System.Windows.Forms.Button();
            this.PDFFormsTabpage = new System.Windows.Forms.TabPage();
            this.ContractAppendixGroupbox = new System.Windows.Forms.GroupBox();
            this.ContractAppendixExistsButton = new System.Windows.Forms.Button();
            this.ContractAppendixRemove = new System.Windows.Forms.Button();
            this.ContractAppendixUpload = new System.Windows.Forms.Button();
            this.ContractAppendixDownload = new System.Windows.Forms.Button();
            this.PDFFormsGroupbox = new System.Windows.Forms.GroupBox();
            this.PDFFormExistsButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.RemovePDFFormButton = new System.Windows.Forms.Button();
            this.PDFFormTypeCombobox = new System.Windows.Forms.ComboBox();
            this.DownloadPDFFormButton = new System.Windows.Forms.Button();
            this.UploadPDFFormButton = new System.Windows.Forms.Button();
            this.MailFilesTabpage = new System.Windows.Forms.TabPage();
            this.ContractMailBodyGroupbox = new System.Windows.Forms.GroupBox();
            this.ContractMailBodiesListbox = new System.Windows.Forms.ListBox();
            this.ListContractMailBodiesButton = new System.Windows.Forms.Button();
            this.ContractMailBodyCultureCombobox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.ContractMailBodyRemoveButton = new System.Windows.Forms.Button();
            this.ContractMailBodyUploadButton = new System.Windows.Forms.Button();
            this.ContractMailBodyDownloadButton = new System.Windows.Forms.Button();
            this.BTKLogsTabpage = new System.Windows.Forms.TabPage();
            this.BTKLogsDownloadButton = new System.Windows.Forms.Button();
            this.BTKLogsListbox = new System.Windows.Forms.ListBox();
            this.BTKLogsListButton = new System.Windows.Forms.Button();
            this.BTKLogUploadButton = new System.Windows.Forms.Button();
            this.BTKLogDatetimepicker = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.BTKLogTypeCombobox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ArchiveNoNumeric)).BeginInit();
            this.MainTabcontrol.SuspendLayout();
            this.ClientAttachmentsTabpage.SuspendLayout();
            this.AttachmentDetailsPanel.SuspendLayout();
            this.PDFFormsTabpage.SuspendLayout();
            this.ContractAppendixGroupbox.SuspendLayout();
            this.PDFFormsGroupbox.SuspendLayout();
            this.MailFilesTabpage.SuspendLayout();
            this.ContractMailBodyGroupbox.SuspendLayout();
            this.BTKLogsTabpage.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(489, 299);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "as";
            // 
            // UploadAttachmentsButton
            // 
            this.UploadAttachmentsButton.Location = new System.Drawing.Point(418, 294);
            this.UploadAttachmentsButton.Name = "UploadAttachmentsButton";
            this.UploadAttachmentsButton.Size = new System.Drawing.Size(65, 23);
            this.UploadAttachmentsButton.TabIndex = 7;
            this.UploadAttachmentsButton.Text = "Upload >";
            this.UploadAttachmentsButton.UseVisualStyleBackColor = true;
            this.UploadAttachmentsButton.Click += new System.EventHandler(this.UploadAttachmentsButton_Click);
            // 
            // AddLocalAttachment
            // 
            this.AddLocalAttachment.Location = new System.Drawing.Point(8, 323);
            this.AddLocalAttachment.Name = "AddLocalAttachment";
            this.AddLocalAttachment.Size = new System.Drawing.Size(75, 23);
            this.AddLocalAttachment.TabIndex = 6;
            this.AddLocalAttachment.Text = "Add File";
            this.AddLocalAttachment.UseVisualStyleBackColor = true;
            this.AddLocalAttachment.Click += new System.EventHandler(this.AddLocalAttachment_Click);
            // 
            // LocalAttachmentsListbox
            // 
            this.LocalAttachmentsListbox.FormattingEnabled = true;
            this.LocalAttachmentsListbox.Location = new System.Drawing.Point(8, 66);
            this.LocalAttachmentsListbox.Name = "LocalAttachmentsListbox";
            this.LocalAttachmentsListbox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.LocalAttachmentsListbox.Size = new System.Drawing.Size(404, 251);
            this.LocalAttachmentsListbox.TabIndex = 1;
            this.LocalAttachmentsListbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LocalAttachmentsListbox_KeyUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Local:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(649, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Server:";
            // 
            // ClientAttachmentsListbox
            // 
            this.ClientAttachmentsListbox.FormattingEnabled = true;
            this.ClientAttachmentsListbox.Location = new System.Drawing.Point(652, 66);
            this.ClientAttachmentsListbox.Name = "ClientAttachmentsListbox";
            this.ClientAttachmentsListbox.Size = new System.Drawing.Size(404, 251);
            this.ClientAttachmentsListbox.TabIndex = 3;
            this.ClientAttachmentsListbox.SelectedIndexChanged += new System.EventHandler(this.ClientAttachmentsListbox_SelectedIndexChanged);
            // 
            // GetAttachmentsButton
            // 
            this.GetAttachmentsButton.Location = new System.Drawing.Point(844, 6);
            this.GetAttachmentsButton.Name = "GetAttachmentsButton";
            this.GetAttachmentsButton.Size = new System.Drawing.Size(120, 23);
            this.GetAttachmentsButton.TabIndex = 2;
            this.GetAttachmentsButton.Text = "Get Attachments";
            this.GetAttachmentsButton.UseVisualStyleBackColor = true;
            this.GetAttachmentsButton.Click += new System.EventHandler(this.GetAttachmentsButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(649, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Archive No:";
            // 
            // ArchiveNoNumeric
            // 
            this.ArchiveNoNumeric.Location = new System.Drawing.Point(718, 9);
            this.ArchiveNoNumeric.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.ArchiveNoNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ArchiveNoNumeric.Name = "ArchiveNoNumeric";
            this.ArchiveNoNumeric.Size = new System.Drawing.Size(120, 20);
            this.ArchiveNoNumeric.TabIndex = 0;
            this.ArchiveNoNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // UploadAttachmentTypeCombobox
            // 
            this.UploadAttachmentTypeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.UploadAttachmentTypeCombobox.FormattingEnabled = true;
            this.UploadAttachmentTypeCombobox.Location = new System.Drawing.Point(513, 296);
            this.UploadAttachmentTypeCombobox.Name = "UploadAttachmentTypeCombobox";
            this.UploadAttachmentTypeCombobox.Size = new System.Drawing.Size(133, 21);
            this.UploadAttachmentTypeCombobox.TabIndex = 9;
            // 
            // MainTabcontrol
            // 
            this.MainTabcontrol.Controls.Add(this.ClientAttachmentsTabpage);
            this.MainTabcontrol.Controls.Add(this.PDFFormsTabpage);
            this.MainTabcontrol.Controls.Add(this.MailFilesTabpage);
            this.MainTabcontrol.Controls.Add(this.BTKLogsTabpage);
            this.MainTabcontrol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabcontrol.Location = new System.Drawing.Point(0, 0);
            this.MainTabcontrol.Multiline = true;
            this.MainTabcontrol.Name = "MainTabcontrol";
            this.MainTabcontrol.SelectedIndex = 0;
            this.MainTabcontrol.Size = new System.Drawing.Size(1072, 468);
            this.MainTabcontrol.TabIndex = 1;
            // 
            // ClientAttachmentsTabpage
            // 
            this.ClientAttachmentsTabpage.BackColor = System.Drawing.SystemColors.Control;
            this.ClientAttachmentsTabpage.Controls.Add(this.AttachmentDetailsPanel);
            this.ClientAttachmentsTabpage.Controls.Add(this.AddLocalAttachment);
            this.ClientAttachmentsTabpage.Controls.Add(this.UploadAttachmentTypeCombobox);
            this.ClientAttachmentsTabpage.Controls.Add(this.label2);
            this.ClientAttachmentsTabpage.Controls.Add(this.label1);
            this.ClientAttachmentsTabpage.Controls.Add(this.label4);
            this.ClientAttachmentsTabpage.Controls.Add(this.ArchiveNoNumeric);
            this.ClientAttachmentsTabpage.Controls.Add(this.UploadAttachmentsButton);
            this.ClientAttachmentsTabpage.Controls.Add(this.GetAttachmentsButton);
            this.ClientAttachmentsTabpage.Controls.Add(this.LocalAttachmentsListbox);
            this.ClientAttachmentsTabpage.Controls.Add(this.label3);
            this.ClientAttachmentsTabpage.Controls.Add(this.ClientAttachmentsListbox);
            this.ClientAttachmentsTabpage.Location = new System.Drawing.Point(4, 22);
            this.ClientAttachmentsTabpage.Name = "ClientAttachmentsTabpage";
            this.ClientAttachmentsTabpage.Padding = new System.Windows.Forms.Padding(3);
            this.ClientAttachmentsTabpage.Size = new System.Drawing.Size(1064, 442);
            this.ClientAttachmentsTabpage.TabIndex = 0;
            this.ClientAttachmentsTabpage.Text = "Client Attachments";
            // 
            // AttachmentDetailsPanel
            // 
            this.AttachmentDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AttachmentDetailsPanel.Controls.Add(this.DeleteClientAttachmentButton);
            this.AttachmentDetailsPanel.Controls.Add(this.AttachmentFileNameTextbox);
            this.AttachmentDetailsPanel.Controls.Add(this.AttachmentExtentionLabel);
            this.AttachmentDetailsPanel.Controls.Add(this.AttachmentCreationDateLabel);
            this.AttachmentDetailsPanel.Controls.Add(this.AttachmentTypeLabel);
            this.AttachmentDetailsPanel.Controls.Add(this.label8);
            this.AttachmentDetailsPanel.Controls.Add(this.label7);
            this.AttachmentDetailsPanel.Controls.Add(this.label6);
            this.AttachmentDetailsPanel.Controls.Add(this.label5);
            this.AttachmentDetailsPanel.Controls.Add(this.SaveClientAttachmentButton);
            this.AttachmentDetailsPanel.Location = new System.Drawing.Point(652, 330);
            this.AttachmentDetailsPanel.Name = "AttachmentDetailsPanel";
            this.AttachmentDetailsPanel.Size = new System.Drawing.Size(404, 104);
            this.AttachmentDetailsPanel.TabIndex = 11;
            this.AttachmentDetailsPanel.Visible = false;
            // 
            // DeleteClientAttachmentButton
            // 
            this.DeleteClientAttachmentButton.Location = new System.Drawing.Point(3, 76);
            this.DeleteClientAttachmentButton.Name = "DeleteClientAttachmentButton";
            this.DeleteClientAttachmentButton.Size = new System.Drawing.Size(75, 23);
            this.DeleteClientAttachmentButton.TabIndex = 16;
            this.DeleteClientAttachmentButton.Text = "Delete";
            this.DeleteClientAttachmentButton.UseVisualStyleBackColor = true;
            this.DeleteClientAttachmentButton.Click += new System.EventHandler(this.DeleteClientAttachmentButton_Click);
            // 
            // AttachmentFileNameTextbox
            // 
            this.AttachmentFileNameTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AttachmentFileNameTextbox.Location = new System.Drawing.Point(88, 58);
            this.AttachmentFileNameTextbox.Name = "AttachmentFileNameTextbox";
            this.AttachmentFileNameTextbox.ReadOnly = true;
            this.AttachmentFileNameTextbox.Size = new System.Drawing.Size(311, 13);
            this.AttachmentFileNameTextbox.TabIndex = 15;
            this.AttachmentFileNameTextbox.WordWrap = false;
            // 
            // AttachmentExtentionLabel
            // 
            this.AttachmentExtentionLabel.AutoSize = true;
            this.AttachmentExtentionLabel.Location = new System.Drawing.Point(85, 40);
            this.AttachmentExtentionLabel.Name = "AttachmentExtentionLabel";
            this.AttachmentExtentionLabel.Size = new System.Drawing.Size(10, 13);
            this.AttachmentExtentionLabel.TabIndex = 14;
            this.AttachmentExtentionLabel.Text = "-";
            // 
            // AttachmentCreationDateLabel
            // 
            this.AttachmentCreationDateLabel.AutoSize = true;
            this.AttachmentCreationDateLabel.Location = new System.Drawing.Point(85, 22);
            this.AttachmentCreationDateLabel.Name = "AttachmentCreationDateLabel";
            this.AttachmentCreationDateLabel.Size = new System.Drawing.Size(10, 13);
            this.AttachmentCreationDateLabel.TabIndex = 13;
            this.AttachmentCreationDateLabel.Text = "-";
            // 
            // AttachmentTypeLabel
            // 
            this.AttachmentTypeLabel.AutoSize = true;
            this.AttachmentTypeLabel.Location = new System.Drawing.Point(85, 4);
            this.AttachmentTypeLabel.Name = "AttachmentTypeLabel";
            this.AttachmentTypeLabel.Size = new System.Drawing.Size(10, 13);
            this.AttachmentTypeLabel.TabIndex = 12;
            this.AttachmentTypeLabel.Text = "-";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label8.Location = new System.Drawing.Point(3, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "File Name:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label7.Location = new System.Drawing.Point(3, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Extention:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label6.Location = new System.Drawing.Point(3, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Creation Date:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label5.Location = new System.Drawing.Point(3, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Type:";
            // 
            // SaveClientAttachmentButton
            // 
            this.SaveClientAttachmentButton.Location = new System.Drawing.Point(324, 76);
            this.SaveClientAttachmentButton.Name = "SaveClientAttachmentButton";
            this.SaveClientAttachmentButton.Size = new System.Drawing.Size(75, 23);
            this.SaveClientAttachmentButton.TabIndex = 10;
            this.SaveClientAttachmentButton.Text = "Download";
            this.SaveClientAttachmentButton.UseVisualStyleBackColor = true;
            this.SaveClientAttachmentButton.Click += new System.EventHandler(this.SaveClientAttachmentButton_Click);
            // 
            // PDFFormsTabpage
            // 
            this.PDFFormsTabpage.Controls.Add(this.ContractAppendixGroupbox);
            this.PDFFormsTabpage.Controls.Add(this.PDFFormsGroupbox);
            this.PDFFormsTabpage.Location = new System.Drawing.Point(4, 22);
            this.PDFFormsTabpage.Name = "PDFFormsTabpage";
            this.PDFFormsTabpage.Size = new System.Drawing.Size(1064, 442);
            this.PDFFormsTabpage.TabIndex = 1;
            this.PDFFormsTabpage.Text = "PDF Forms";
            this.PDFFormsTabpage.UseVisualStyleBackColor = true;
            // 
            // ContractAppendixGroupbox
            // 
            this.ContractAppendixGroupbox.Controls.Add(this.ContractAppendixExistsButton);
            this.ContractAppendixGroupbox.Controls.Add(this.ContractAppendixRemove);
            this.ContractAppendixGroupbox.Controls.Add(this.ContractAppendixUpload);
            this.ContractAppendixGroupbox.Controls.Add(this.ContractAppendixDownload);
            this.ContractAppendixGroupbox.Location = new System.Drawing.Point(9, 66);
            this.ContractAppendixGroupbox.Name = "ContractAppendixGroupbox";
            this.ContractAppendixGroupbox.Size = new System.Drawing.Size(1047, 56);
            this.ContractAppendixGroupbox.TabIndex = 5;
            this.ContractAppendixGroupbox.TabStop = false;
            this.ContractAppendixGroupbox.Text = "Contract Appendix PDF";
            // 
            // ContractAppendixExistsButton
            // 
            this.ContractAppendixExistsButton.Location = new System.Drawing.Point(249, 19);
            this.ContractAppendixExistsButton.Name = "ContractAppendixExistsButton";
            this.ContractAppendixExistsButton.Size = new System.Drawing.Size(75, 23);
            this.ContractAppendixExistsButton.TabIndex = 1;
            this.ContractAppendixExistsButton.Text = "Exists";
            this.ContractAppendixExistsButton.UseVisualStyleBackColor = true;
            this.ContractAppendixExistsButton.Click += new System.EventHandler(this.ContractAppendixExistsButton_Click);
            // 
            // ContractAppendixRemove
            // 
            this.ContractAppendixRemove.Location = new System.Drawing.Point(168, 19);
            this.ContractAppendixRemove.Name = "ContractAppendixRemove";
            this.ContractAppendixRemove.Size = new System.Drawing.Size(75, 23);
            this.ContractAppendixRemove.TabIndex = 0;
            this.ContractAppendixRemove.Text = "Remove";
            this.ContractAppendixRemove.UseVisualStyleBackColor = true;
            this.ContractAppendixRemove.Click += new System.EventHandler(this.ContractAppendixRemove_Click);
            // 
            // ContractAppendixUpload
            // 
            this.ContractAppendixUpload.Location = new System.Drawing.Point(87, 19);
            this.ContractAppendixUpload.Name = "ContractAppendixUpload";
            this.ContractAppendixUpload.Size = new System.Drawing.Size(75, 23);
            this.ContractAppendixUpload.TabIndex = 0;
            this.ContractAppendixUpload.Text = "Upload";
            this.ContractAppendixUpload.UseVisualStyleBackColor = true;
            this.ContractAppendixUpload.Click += new System.EventHandler(this.ContractAppendixUpload_Click);
            // 
            // ContractAppendixDownload
            // 
            this.ContractAppendixDownload.Location = new System.Drawing.Point(6, 19);
            this.ContractAppendixDownload.Name = "ContractAppendixDownload";
            this.ContractAppendixDownload.Size = new System.Drawing.Size(75, 23);
            this.ContractAppendixDownload.TabIndex = 0;
            this.ContractAppendixDownload.Text = "Download";
            this.ContractAppendixDownload.UseVisualStyleBackColor = true;
            this.ContractAppendixDownload.Click += new System.EventHandler(this.ContractAppendixDownload_Click);
            // 
            // PDFFormsGroupbox
            // 
            this.PDFFormsGroupbox.Controls.Add(this.PDFFormExistsButton);
            this.PDFFormsGroupbox.Controls.Add(this.label9);
            this.PDFFormsGroupbox.Controls.Add(this.RemovePDFFormButton);
            this.PDFFormsGroupbox.Controls.Add(this.PDFFormTypeCombobox);
            this.PDFFormsGroupbox.Controls.Add(this.DownloadPDFFormButton);
            this.PDFFormsGroupbox.Controls.Add(this.UploadPDFFormButton);
            this.PDFFormsGroupbox.Location = new System.Drawing.Point(8, 3);
            this.PDFFormsGroupbox.Name = "PDFFormsGroupbox";
            this.PDFFormsGroupbox.Size = new System.Drawing.Size(1048, 56);
            this.PDFFormsGroupbox.TabIndex = 4;
            this.PDFFormsGroupbox.TabStop = false;
            this.PDFFormsGroupbox.Text = "PDF Forms";
            // 
            // PDFFormExistsButton
            // 
            this.PDFFormExistsButton.Location = new System.Drawing.Point(489, 17);
            this.PDFFormExistsButton.Name = "PDFFormExistsButton";
            this.PDFFormExistsButton.Size = new System.Drawing.Size(75, 23);
            this.PDFFormExistsButton.TabIndex = 4;
            this.PDFFormExistsButton.Text = "Exists";
            this.PDFFormExistsButton.UseVisualStyleBackColor = true;
            this.PDFFormExistsButton.Click += new System.EventHandler(this.PDFFormExistsButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Form Type:";
            // 
            // RemovePDFFormButton
            // 
            this.RemovePDFFormButton.Location = new System.Drawing.Point(408, 17);
            this.RemovePDFFormButton.Name = "RemovePDFFormButton";
            this.RemovePDFFormButton.Size = new System.Drawing.Size(75, 23);
            this.RemovePDFFormButton.TabIndex = 3;
            this.RemovePDFFormButton.Text = "Remove";
            this.RemovePDFFormButton.UseVisualStyleBackColor = true;
            this.RemovePDFFormButton.Click += new System.EventHandler(this.RemovePDFFormButton_Click);
            // 
            // PDFFormTypeCombobox
            // 
            this.PDFFormTypeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PDFFormTypeCombobox.FormattingEnabled = true;
            this.PDFFormTypeCombobox.Location = new System.Drawing.Point(72, 19);
            this.PDFFormTypeCombobox.Name = "PDFFormTypeCombobox";
            this.PDFFormTypeCombobox.Size = new System.Drawing.Size(168, 21);
            this.PDFFormTypeCombobox.TabIndex = 1;
            // 
            // DownloadPDFFormButton
            // 
            this.DownloadPDFFormButton.Location = new System.Drawing.Point(327, 17);
            this.DownloadPDFFormButton.Name = "DownloadPDFFormButton";
            this.DownloadPDFFormButton.Size = new System.Drawing.Size(75, 23);
            this.DownloadPDFFormButton.TabIndex = 2;
            this.DownloadPDFFormButton.Text = "Download";
            this.DownloadPDFFormButton.UseVisualStyleBackColor = true;
            this.DownloadPDFFormButton.Click += new System.EventHandler(this.DownloadPDFFormButton_Click);
            // 
            // UploadPDFFormButton
            // 
            this.UploadPDFFormButton.Location = new System.Drawing.Point(246, 17);
            this.UploadPDFFormButton.Name = "UploadPDFFormButton";
            this.UploadPDFFormButton.Size = new System.Drawing.Size(75, 23);
            this.UploadPDFFormButton.TabIndex = 2;
            this.UploadPDFFormButton.Text = "Upload";
            this.UploadPDFFormButton.UseVisualStyleBackColor = true;
            this.UploadPDFFormButton.Click += new System.EventHandler(this.UploadPDFFormButton_Click);
            // 
            // MailFilesTabpage
            // 
            this.MailFilesTabpage.Controls.Add(this.ContractMailBodyGroupbox);
            this.MailFilesTabpage.Location = new System.Drawing.Point(4, 22);
            this.MailFilesTabpage.Name = "MailFilesTabpage";
            this.MailFilesTabpage.Size = new System.Drawing.Size(1064, 442);
            this.MailFilesTabpage.TabIndex = 2;
            this.MailFilesTabpage.Text = "Mail Files";
            this.MailFilesTabpage.UseVisualStyleBackColor = true;
            // 
            // ContractMailBodyGroupbox
            // 
            this.ContractMailBodyGroupbox.Controls.Add(this.ContractMailBodiesListbox);
            this.ContractMailBodyGroupbox.Controls.Add(this.ListContractMailBodiesButton);
            this.ContractMailBodyGroupbox.Controls.Add(this.ContractMailBodyCultureCombobox);
            this.ContractMailBodyGroupbox.Controls.Add(this.label12);
            this.ContractMailBodyGroupbox.Controls.Add(this.ContractMailBodyRemoveButton);
            this.ContractMailBodyGroupbox.Controls.Add(this.ContractMailBodyUploadButton);
            this.ContractMailBodyGroupbox.Controls.Add(this.ContractMailBodyDownloadButton);
            this.ContractMailBodyGroupbox.Location = new System.Drawing.Point(8, 3);
            this.ContractMailBodyGroupbox.Name = "ContractMailBodyGroupbox";
            this.ContractMailBodyGroupbox.Size = new System.Drawing.Size(1048, 262);
            this.ContractMailBodyGroupbox.TabIndex = 0;
            this.ContractMailBodyGroupbox.TabStop = false;
            this.ContractMailBodyGroupbox.Text = "Contract Mail Body";
            // 
            // ContractMailBodiesListbox
            // 
            this.ContractMailBodiesListbox.FormattingEnabled = true;
            this.ContractMailBodiesListbox.Location = new System.Drawing.Point(9, 78);
            this.ContractMailBodiesListbox.Name = "ContractMailBodiesListbox";
            this.ContractMailBodiesListbox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.ContractMailBodiesListbox.Size = new System.Drawing.Size(410, 173);
            this.ContractMailBodiesListbox.TabIndex = 4;
            // 
            // ListContractMailBodiesButton
            // 
            this.ListContractMailBodiesButton.Location = new System.Drawing.Point(55, 49);
            this.ListContractMailBodiesButton.Name = "ListContractMailBodiesButton";
            this.ListContractMailBodiesButton.Size = new System.Drawing.Size(121, 23);
            this.ListContractMailBodiesButton.TabIndex = 3;
            this.ListContractMailBodiesButton.Text = "List All Files";
            this.ListContractMailBodiesButton.UseVisualStyleBackColor = true;
            this.ListContractMailBodiesButton.Click += new System.EventHandler(this.ListContractMailBodiesButton_Click);
            // 
            // ContractMailBodyCultureCombobox
            // 
            this.ContractMailBodyCultureCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ContractMailBodyCultureCombobox.FormattingEnabled = true;
            this.ContractMailBodyCultureCombobox.Location = new System.Drawing.Point(55, 21);
            this.ContractMailBodyCultureCombobox.Name = "ContractMailBodyCultureCombobox";
            this.ContractMailBodyCultureCombobox.Size = new System.Drawing.Size(121, 21);
            this.ContractMailBodyCultureCombobox.TabIndex = 2;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Culture:";
            // 
            // ContractMailBodyRemoveButton
            // 
            this.ContractMailBodyRemoveButton.Location = new System.Drawing.Point(344, 19);
            this.ContractMailBodyRemoveButton.Name = "ContractMailBodyRemoveButton";
            this.ContractMailBodyRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.ContractMailBodyRemoveButton.TabIndex = 0;
            this.ContractMailBodyRemoveButton.Text = "Remove";
            this.ContractMailBodyRemoveButton.UseVisualStyleBackColor = true;
            this.ContractMailBodyRemoveButton.Click += new System.EventHandler(this.ContractMailBodyRemoveButton_Click);
            // 
            // ContractMailBodyUploadButton
            // 
            this.ContractMailBodyUploadButton.Location = new System.Drawing.Point(263, 19);
            this.ContractMailBodyUploadButton.Name = "ContractMailBodyUploadButton";
            this.ContractMailBodyUploadButton.Size = new System.Drawing.Size(75, 23);
            this.ContractMailBodyUploadButton.TabIndex = 0;
            this.ContractMailBodyUploadButton.Text = "Upload";
            this.ContractMailBodyUploadButton.UseVisualStyleBackColor = true;
            this.ContractMailBodyUploadButton.Click += new System.EventHandler(this.ContractMailBodyUploadButton_Click);
            // 
            // ContractMailBodyDownloadButton
            // 
            this.ContractMailBodyDownloadButton.Location = new System.Drawing.Point(182, 19);
            this.ContractMailBodyDownloadButton.Name = "ContractMailBodyDownloadButton";
            this.ContractMailBodyDownloadButton.Size = new System.Drawing.Size(75, 23);
            this.ContractMailBodyDownloadButton.TabIndex = 0;
            this.ContractMailBodyDownloadButton.Text = "Download";
            this.ContractMailBodyDownloadButton.UseVisualStyleBackColor = true;
            this.ContractMailBodyDownloadButton.Click += new System.EventHandler(this.ContractMailBodyDownloadButton_Click);
            // 
            // BTKLogsTabpage
            // 
            this.BTKLogsTabpage.Controls.Add(this.BTKLogsDownloadButton);
            this.BTKLogsTabpage.Controls.Add(this.BTKLogsListbox);
            this.BTKLogsTabpage.Controls.Add(this.BTKLogsListButton);
            this.BTKLogsTabpage.Controls.Add(this.BTKLogUploadButton);
            this.BTKLogsTabpage.Controls.Add(this.BTKLogDatetimepicker);
            this.BTKLogsTabpage.Controls.Add(this.label11);
            this.BTKLogsTabpage.Controls.Add(this.BTKLogTypeCombobox);
            this.BTKLogsTabpage.Controls.Add(this.label10);
            this.BTKLogsTabpage.Location = new System.Drawing.Point(4, 22);
            this.BTKLogsTabpage.Name = "BTKLogsTabpage";
            this.BTKLogsTabpage.Size = new System.Drawing.Size(1064, 442);
            this.BTKLogsTabpage.TabIndex = 3;
            this.BTKLogsTabpage.Text = "BTK Logs";
            this.BTKLogsTabpage.UseVisualStyleBackColor = true;
            // 
            // BTKLogsDownloadButton
            // 
            this.BTKLogsDownloadButton.Location = new System.Drawing.Point(566, 405);
            this.BTKLogsDownloadButton.Name = "BTKLogsDownloadButton";
            this.BTKLogsDownloadButton.Size = new System.Drawing.Size(75, 23);
            this.BTKLogsDownloadButton.TabIndex = 7;
            this.BTKLogsDownloadButton.Text = "Download";
            this.BTKLogsDownloadButton.UseVisualStyleBackColor = true;
            this.BTKLogsDownloadButton.Click += new System.EventHandler(this.BTKLogsDownloadButton_Click);
            // 
            // BTKLogsListbox
            // 
            this.BTKLogsListbox.FormattingEnabled = true;
            this.BTKLogsListbox.Location = new System.Drawing.Point(647, 8);
            this.BTKLogsListbox.Name = "BTKLogsListbox";
            this.BTKLogsListbox.Size = new System.Drawing.Size(409, 420);
            this.BTKLogsListbox.TabIndex = 6;
            // 
            // BTKLogsListButton
            // 
            this.BTKLogsListButton.Location = new System.Drawing.Point(566, 8);
            this.BTKLogsListButton.Name = "BTKLogsListButton";
            this.BTKLogsListButton.Size = new System.Drawing.Size(75, 23);
            this.BTKLogsListButton.TabIndex = 5;
            this.BTKLogsListButton.Text = "List";
            this.BTKLogsListButton.UseVisualStyleBackColor = true;
            this.BTKLogsListButton.Click += new System.EventHandler(this.BTKLogsListButton_Click);
            // 
            // BTKLogUploadButton
            // 
            this.BTKLogUploadButton.Location = new System.Drawing.Point(485, 8);
            this.BTKLogUploadButton.Name = "BTKLogUploadButton";
            this.BTKLogUploadButton.Size = new System.Drawing.Size(75, 23);
            this.BTKLogUploadButton.TabIndex = 4;
            this.BTKLogUploadButton.Text = "Upload";
            this.BTKLogUploadButton.UseVisualStyleBackColor = true;
            this.BTKLogUploadButton.Click += new System.EventHandler(this.BTKLogUploadButton_Click);
            // 
            // BTKLogDatetimepicker
            // 
            this.BTKLogDatetimepicker.CustomFormat = "yyyy-MM-dd HH:mm";
            this.BTKLogDatetimepicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.BTKLogDatetimepicker.Location = new System.Drawing.Point(341, 10);
            this.BTKLogDatetimepicker.Name = "BTKLogDatetimepicker";
            this.BTKLogDatetimepicker.Size = new System.Drawing.Size(138, 20);
            this.BTKLogDatetimepicker.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(281, 13);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(54, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Log Date:";
            // 
            // BTKLogTypeCombobox
            // 
            this.BTKLogTypeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BTKLogTypeCombobox.FormattingEnabled = true;
            this.BTKLogTypeCombobox.Location = new System.Drawing.Point(69, 10);
            this.BTKLogTypeCombobox.Name = "BTKLogTypeCombobox";
            this.BTKLogTypeCombobox.Size = new System.Drawing.Size(206, 21);
            this.BTKLogTypeCombobox.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Log Type:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 468);
            this.Controls.Add(this.MainTabcontrol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RadiusR File Management Test Unit";
            ((System.ComponentModel.ISupportInitialize)(this.ArchiveNoNumeric)).EndInit();
            this.MainTabcontrol.ResumeLayout(false);
            this.ClientAttachmentsTabpage.ResumeLayout(false);
            this.ClientAttachmentsTabpage.PerformLayout();
            this.AttachmentDetailsPanel.ResumeLayout(false);
            this.AttachmentDetailsPanel.PerformLayout();
            this.PDFFormsTabpage.ResumeLayout(false);
            this.ContractAppendixGroupbox.ResumeLayout(false);
            this.PDFFormsGroupbox.ResumeLayout(false);
            this.PDFFormsGroupbox.PerformLayout();
            this.MailFilesTabpage.ResumeLayout(false);
            this.ContractMailBodyGroupbox.ResumeLayout(false);
            this.ContractMailBodyGroupbox.PerformLayout();
            this.BTKLogsTabpage.ResumeLayout(false);
            this.BTKLogsTabpage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown ArchiveNoNumeric;
        private System.Windows.Forms.Button GetAttachmentsButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox ClientAttachmentsListbox;
        private System.Windows.Forms.ListBox LocalAttachmentsListbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button AddLocalAttachment;
        private System.Windows.Forms.Button UploadAttachmentsButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox UploadAttachmentTypeCombobox;
        private System.Windows.Forms.TabControl MainTabcontrol;
        private System.Windows.Forms.TabPage ClientAttachmentsTabpage;
        private System.Windows.Forms.Button SaveClientAttachmentButton;
        private System.Windows.Forms.Panel AttachmentDetailsPanel;
        private System.Windows.Forms.Label AttachmentTypeLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label AttachmentCreationDateLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label AttachmentExtentionLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox AttachmentFileNameTextbox;
        private System.Windows.Forms.Button DeleteClientAttachmentButton;
        private System.Windows.Forms.TabPage PDFFormsTabpage;
        private System.Windows.Forms.ComboBox PDFFormTypeCombobox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button UploadPDFFormButton;
        private System.Windows.Forms.Button DownloadPDFFormButton;
        private System.Windows.Forms.Button RemovePDFFormButton;
        private System.Windows.Forms.GroupBox PDFFormsGroupbox;
        private System.Windows.Forms.GroupBox ContractAppendixGroupbox;
        private System.Windows.Forms.Button ContractAppendixRemove;
        private System.Windows.Forms.Button ContractAppendixUpload;
        private System.Windows.Forms.Button ContractAppendixDownload;
        private System.Windows.Forms.TabPage MailFilesTabpage;
        private System.Windows.Forms.GroupBox ContractMailBodyGroupbox;
        private System.Windows.Forms.Button ContractMailBodyRemoveButton;
        private System.Windows.Forms.Button ContractMailBodyUploadButton;
        private System.Windows.Forms.Button ContractMailBodyDownloadButton;
        private System.Windows.Forms.TabPage BTKLogsTabpage;
        private System.Windows.Forms.ComboBox BTKLogTypeCombobox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker BTKLogDatetimepicker;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button BTKLogUploadButton;
        private System.Windows.Forms.Button BTKLogsListButton;
        private System.Windows.Forms.ListBox BTKLogsListbox;
        private System.Windows.Forms.Button BTKLogsDownloadButton;
        private System.Windows.Forms.ComboBox ContractMailBodyCultureCombobox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button PDFFormExistsButton;
        private System.Windows.Forms.Button ListContractMailBodiesButton;
        private System.Windows.Forms.ListBox ContractMailBodiesListbox;
        private System.Windows.Forms.Button ContractAppendixExistsButton;
    }
}

