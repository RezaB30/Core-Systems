
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
            this.SaveClientAttachmentButton = new System.Windows.Forms.Button();
            this.AttachmentDetailsPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.AttachmentTypeLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.AttachmentCreationDateLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.AttachmentExtentionLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.AttachmentFileNameTextbox = new System.Windows.Forms.TextBox();
            this.DeleteClientAttachmentButton = new System.Windows.Forms.Button();
            this.PDFFormsTabpage = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.PDFFormTypeCombobox = new System.Windows.Forms.ComboBox();
            this.UploadPDFFormButton = new System.Windows.Forms.Button();
            this.DownloadPDFFormButton = new System.Windows.Forms.Button();
            this.RemovePDFFormButton = new System.Windows.Forms.Button();
            this.PDFFormsGroupbox = new System.Windows.Forms.GroupBox();
            this.ContractAppendixGroupbox = new System.Windows.Forms.GroupBox();
            this.ContractAppendixDownload = new System.Windows.Forms.Button();
            this.ContractAppendixUpload = new System.Windows.Forms.Button();
            this.ContractAppendixRemove = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ArchiveNoNumeric)).BeginInit();
            this.MainTabcontrol.SuspendLayout();
            this.ClientAttachmentsTabpage.SuspendLayout();
            this.AttachmentDetailsPanel.SuspendLayout();
            this.PDFFormsTabpage.SuspendLayout();
            this.PDFFormsGroupbox.SuspendLayout();
            this.ContractAppendixGroupbox.SuspendLayout();
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
            // AttachmentTypeLabel
            // 
            this.AttachmentTypeLabel.AutoSize = true;
            this.AttachmentTypeLabel.Location = new System.Drawing.Point(85, 4);
            this.AttachmentTypeLabel.Name = "AttachmentTypeLabel";
            this.AttachmentTypeLabel.Size = new System.Drawing.Size(10, 13);
            this.AttachmentTypeLabel.TabIndex = 12;
            this.AttachmentTypeLabel.Text = "-";
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
            // AttachmentCreationDateLabel
            // 
            this.AttachmentCreationDateLabel.AutoSize = true;
            this.AttachmentCreationDateLabel.Location = new System.Drawing.Point(85, 22);
            this.AttachmentCreationDateLabel.Name = "AttachmentCreationDateLabel";
            this.AttachmentCreationDateLabel.Size = new System.Drawing.Size(10, 13);
            this.AttachmentCreationDateLabel.TabIndex = 13;
            this.AttachmentCreationDateLabel.Text = "-";
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
            // AttachmentExtentionLabel
            // 
            this.AttachmentExtentionLabel.AutoSize = true;
            this.AttachmentExtentionLabel.Location = new System.Drawing.Point(85, 40);
            this.AttachmentExtentionLabel.Name = "AttachmentExtentionLabel";
            this.AttachmentExtentionLabel.Size = new System.Drawing.Size(10, 13);
            this.AttachmentExtentionLabel.TabIndex = 14;
            this.AttachmentExtentionLabel.Text = "-";
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
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Form Type:";
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
            // PDFFormsGroupbox
            // 
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
            // ContractAppendixGroupbox
            // 
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
            // ContractAppendixUpload
            // 
            this.ContractAppendixUpload.Location = new System.Drawing.Point(87, 19);
            this.ContractAppendixUpload.Name = "ContractAppendixUpload";
            this.ContractAppendixUpload.Size = new System.Drawing.Size(75, 23);
            this.ContractAppendixUpload.TabIndex = 0;
            this.ContractAppendixUpload.Text = "Upload";
            this.ContractAppendixUpload.UseVisualStyleBackColor = true;
            // 
            // ContractAppendixRemove
            // 
            this.ContractAppendixRemove.Location = new System.Drawing.Point(168, 19);
            this.ContractAppendixRemove.Name = "ContractAppendixRemove";
            this.ContractAppendixRemove.Size = new System.Drawing.Size(75, 23);
            this.ContractAppendixRemove.TabIndex = 0;
            this.ContractAppendixRemove.Text = "Remove";
            this.ContractAppendixRemove.UseVisualStyleBackColor = true;
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
            this.PDFFormsGroupbox.ResumeLayout(false);
            this.PDFFormsGroupbox.PerformLayout();
            this.ContractAppendixGroupbox.ResumeLayout(false);
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
    }
}

