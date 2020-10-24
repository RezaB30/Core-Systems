namespace RadiusR.OfflinePayment.TestUnit
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
            this.UploadDownloadTabcontrol = new System.Windows.Forms.TabControl();
            this.UploadTabpage = new System.Windows.Forms.TabPage();
            this.UploadFormatCombobox = new System.Windows.Forms.ComboBox();
            this.CreateUploadFileButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.DownloadTabpage = new System.Windows.Forms.TabPage();
            this.MainStatusstrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.ProcessFormatCombobox = new System.Windows.Forms.ComboBox();
            this.ProcessButton = new System.Windows.Forms.Button();
            this.ProcessedBatchGridview = new System.Windows.Forms.DataGridView();
            this.UploadDownloadTabcontrol.SuspendLayout();
            this.UploadTabpage.SuspendLayout();
            this.DownloadTabpage.SuspendLayout();
            this.MainStatusstrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProcessedBatchGridview)).BeginInit();
            this.SuspendLayout();
            // 
            // UploadDownloadTabcontrol
            // 
            this.UploadDownloadTabcontrol.Controls.Add(this.UploadTabpage);
            this.UploadDownloadTabcontrol.Controls.Add(this.DownloadTabpage);
            this.UploadDownloadTabcontrol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UploadDownloadTabcontrol.Location = new System.Drawing.Point(0, 0);
            this.UploadDownloadTabcontrol.Name = "UploadDownloadTabcontrol";
            this.UploadDownloadTabcontrol.SelectedIndex = 0;
            this.UploadDownloadTabcontrol.Size = new System.Drawing.Size(790, 326);
            this.UploadDownloadTabcontrol.TabIndex = 0;
            // 
            // UploadTabpage
            // 
            this.UploadTabpage.Controls.Add(this.UploadFormatCombobox);
            this.UploadTabpage.Controls.Add(this.CreateUploadFileButton);
            this.UploadTabpage.Controls.Add(this.label1);
            this.UploadTabpage.Location = new System.Drawing.Point(4, 22);
            this.UploadTabpage.Name = "UploadTabpage";
            this.UploadTabpage.Padding = new System.Windows.Forms.Padding(3);
            this.UploadTabpage.Size = new System.Drawing.Size(782, 300);
            this.UploadTabpage.TabIndex = 0;
            this.UploadTabpage.Text = "Upload";
            this.UploadTabpage.UseVisualStyleBackColor = true;
            // 
            // UploadFormatCombobox
            // 
            this.UploadFormatCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.UploadFormatCombobox.FormattingEnabled = true;
            this.UploadFormatCombobox.Location = new System.Drawing.Point(540, 8);
            this.UploadFormatCombobox.Name = "UploadFormatCombobox";
            this.UploadFormatCombobox.Size = new System.Drawing.Size(121, 21);
            this.UploadFormatCombobox.TabIndex = 2;
            // 
            // CreateUploadFileButton
            // 
            this.CreateUploadFileButton.Location = new System.Drawing.Point(667, 6);
            this.CreateUploadFileButton.Name = "CreateUploadFileButton";
            this.CreateUploadFileButton.Size = new System.Drawing.Size(107, 23);
            this.CreateUploadFileButton.TabIndex = 1;
            this.CreateUploadFileButton.Text = "Create Upload File";
            this.CreateUploadFileButton.UseVisualStyleBackColor = true;
            this.CreateUploadFileButton.Click += new System.EventHandler(this.CreateUploadFileButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(372, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This will load all unpaid bills from data base and save it as a formatted text fi" +
    "le.";
            // 
            // DownloadTabpage
            // 
            this.DownloadTabpage.Controls.Add(this.ProcessedBatchGridview);
            this.DownloadTabpage.Controls.Add(this.ProcessButton);
            this.DownloadTabpage.Controls.Add(this.ProcessFormatCombobox);
            this.DownloadTabpage.Controls.Add(this.label2);
            this.DownloadTabpage.Location = new System.Drawing.Point(4, 22);
            this.DownloadTabpage.Name = "DownloadTabpage";
            this.DownloadTabpage.Padding = new System.Windows.Forms.Padding(3);
            this.DownloadTabpage.Size = new System.Drawing.Size(782, 300);
            this.DownloadTabpage.TabIndex = 1;
            this.DownloadTabpage.Text = "Download";
            this.DownloadTabpage.UseVisualStyleBackColor = true;
            // 
            // MainStatusstrip
            // 
            this.MainStatusstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.MainStatusstrip.Location = new System.Drawing.Point(0, 304);
            this.MainStatusstrip.Name = "MainStatusstrip";
            this.MainStatusstrip.Size = new System.Drawing.Size(790, 22);
            this.MainStatusstrip.TabIndex = 1;
            this.MainStatusstrip.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(39, 17);
            this.StatusLabel.Text = "Ready";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Will process a paid batch file into data to display.";
            // 
            // ProcessFormatCombobox
            // 
            this.ProcessFormatCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProcessFormatCombobox.FormattingEnabled = true;
            this.ProcessFormatCombobox.Location = new System.Drawing.Point(503, 8);
            this.ProcessFormatCombobox.Name = "ProcessFormatCombobox";
            this.ProcessFormatCombobox.Size = new System.Drawing.Size(121, 21);
            this.ProcessFormatCombobox.TabIndex = 1;
            // 
            // ProcessButton
            // 
            this.ProcessButton.Location = new System.Drawing.Point(630, 6);
            this.ProcessButton.Name = "ProcessButton";
            this.ProcessButton.Size = new System.Drawing.Size(144, 23);
            this.ProcessButton.TabIndex = 2;
            this.ProcessButton.Text = "Process Downloaded File";
            this.ProcessButton.UseVisualStyleBackColor = true;
            this.ProcessButton.Click += new System.EventHandler(this.ProcessButton_Click);
            // 
            // ProcessedBatchGridview
            // 
            this.ProcessedBatchGridview.AllowUserToAddRows = false;
            this.ProcessedBatchGridview.AllowUserToDeleteRows = false;
            this.ProcessedBatchGridview.AllowUserToResizeRows = false;
            this.ProcessedBatchGridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ProcessedBatchGridview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ProcessedBatchGridview.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ProcessedBatchGridview.Location = new System.Drawing.Point(3, 35);
            this.ProcessedBatchGridview.Name = "ProcessedBatchGridview";
            this.ProcessedBatchGridview.ReadOnly = true;
            this.ProcessedBatchGridview.Size = new System.Drawing.Size(776, 262);
            this.ProcessedBatchGridview.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 326);
            this.Controls.Add(this.MainStatusstrip);
            this.Controls.Add(this.UploadDownloadTabcontrol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RadiusR Offline Payment";
            this.UploadDownloadTabcontrol.ResumeLayout(false);
            this.UploadTabpage.ResumeLayout(false);
            this.UploadTabpage.PerformLayout();
            this.DownloadTabpage.ResumeLayout(false);
            this.DownloadTabpage.PerformLayout();
            this.MainStatusstrip.ResumeLayout(false);
            this.MainStatusstrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ProcessedBatchGridview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl UploadDownloadTabcontrol;
        private System.Windows.Forms.TabPage UploadTabpage;
        private System.Windows.Forms.TabPage DownloadTabpage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CreateUploadFileButton;
        private System.Windows.Forms.StatusStrip MainStatusstrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ComboBox UploadFormatCombobox;
        private System.Windows.Forms.Button ProcessButton;
        private System.Windows.Forms.ComboBox ProcessFormatCombobox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView ProcessedBatchGridview;
    }
}

