using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RadiusR.FileManagement.TestUnit
{
    public partial class MainForm : Form
    {
        private MasterISSFileManager FileManager { get; set; }

        public MainForm()
        {
            InitializeComponent();
            // attachment types
            UploadAttachmentTypeCombobox.Items.AddRange(Enum.GetNames(typeof(ClientAttachmentTypes)));
            UploadAttachmentTypeCombobox.SelectedIndex = 0;
            // pdf forms
            PDFFormTypeCombobox.Items.AddRange(Enum.GetNames(typeof(RadiusR.DB.Enums.PDFFormType)));
            PDFFormTypeCombobox.SelectedIndex = 0;
            // btk logs
            BTKLogTypeCombobox.Items.AddRange(Enum.GetNames(typeof(RadiusR.DB.Enums.BTKLogTypes)));
            BTKLogTypeCombobox.SelectedIndex = 0;
            // file manager
            try
            {
                FileManager = new MasterISSFileManager();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void GetClientAttachments()
        {
            ClientAttachmentsListbox.Items.Clear();
            var result = FileManager.GetClientAttachmentsList((long)ArchiveNoNumeric.Value);
            if (result.InternalException != null)
            {
                ShowError(result.InternalException);
            }
            else if (result.Result != null)
            {
                ClientAttachmentsListbox.Items.AddRange(result.Result.ToArray());
            }
        }

        private void GetAttachmentsButton_Click(object sender, EventArgs e)
        {
            GetClientAttachments();
        }

        private void AddLocalAttachment_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = dialog.CheckFileExists = dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LocalAttachmentsListbox.Items.AddRange(dialog.FileNames);
            }
        }

        private void LocalAttachmentsListbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var toRemove = LocalAttachmentsListbox.SelectedItems.Cast<string>().ToArray();
                foreach (var item in toRemove)
                {
                    LocalAttachmentsListbox.Items.Remove(item);
                }
            }
        }

        private void UploadAttachmentsButton_Click(object sender, EventArgs e)
        {
            var attachmentType = (ClientAttachmentTypes)Enum.Parse(typeof(ClientAttachmentTypes), UploadAttachmentTypeCombobox.SelectedItem as string);
            foreach (string item in LocalAttachmentsListbox.SelectedItems)
            {
                using (var fileStream = File.OpenRead(item))
                {
                    var attachment = new FileManagerClientAttachmentWithContent(fileStream, attachmentType, item.Substring(item.LastIndexOf('.') + 1));
                    var result = FileManager.SaveClientAttachment((long)ArchiveNoNumeric.Value, attachment);
                    if (result.InternalException != null)
                    {
                        ShowError(result.InternalException);
                        break;
                    }
                }
            }

            GetClientAttachments();
        }

        private void ClientAttachmentsListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ClientAttachmentsListbox.SelectedItem != null)
            {
                var currentAttachment = ClientAttachmentsListbox.SelectedItem as FileManagerClientAttachment;
                if (currentAttachment == null)
                {
                    AttachmentDetailsPanel.Visible = false;
                    return;
                }
                AttachmentTypeLabel.Text = currentAttachment.AttachmentType.ToString();
                AttachmentCreationDateLabel.Text = currentAttachment.CreationDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                AttachmentExtentionLabel.Text = currentAttachment.FileExtention;
                AttachmentFileNameTextbox.Text = currentAttachment.ServerSideName;
                AttachmentDetailsPanel.Visible = true;
            }
            else
            {
                AttachmentDetailsPanel.Visible = false;
            }
        }

        private void SaveClientAttachmentButton_Click(object sender, EventArgs e)
        {
            if (ClientAttachmentsListbox.SelectedItem != null)
            {
                var currentAttachment = ClientAttachmentsListbox.SelectedItem as FileManagerClientAttachment;
                if (currentAttachment == null)
                {
                    AttachmentDetailsPanel.Visible = false;
                    return;
                }
                var dialog = new SaveFileDialog();
                dialog.Filter = $"Server File|*.{currentAttachment.FileExtention}";
                dialog.DefaultExt = currentAttachment.FileExtention;
                dialog.AddExtension = true;
                dialog.FileName = currentAttachment.ServerSideName;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (var result = FileManager.GetClientAttachment((long)ArchiveNoNumeric.Value, currentAttachment.ServerSideName))
                    {
                        if (result.InternalException != null)
                        {
                            ShowError(result.InternalException);
                        }
                        else
                        {
                            try
                            {
                                using (var fileStream = File.Create(dialog.FileName))
                                {
                                    result.Result.Content.CopyTo(fileStream);
                                    fileStream.Flush();
                                    fileStream.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                ShowError(ex);
                            }
                        }
                    }
                }
            }
        }

        private void DeleteClientAttachmentButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete Client Attachment", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var result = FileManager.RemoveClientAttachment((long)ArchiveNoNumeric.Value, ClientAttachmentsListbox.SelectedItem.ToString());
                if (result.InternalException != null)
                {
                    ShowError(result.InternalException);
                    return;
                }

                GetClientAttachments();
            }
        }

        private void DownloadPDFFormButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (var result = FileManager.GetPDFForm((RadiusR.DB.Enums.PDFFormType)Enum.Parse(typeof(RadiusR.DB.Enums.PDFFormType), PDFFormTypeCombobox.SelectedItem as string)))
                {
                    if (result.InternalException != null)
                    {
                        ShowError(result.InternalException);
                    }
                    else
                    {
                        using (var fileStream = File.Create($"{dialog.SelectedPath}\\{result.Result.FileName}"))
                        {
                            result.Result.Content.CopyTo(fileStream);
                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }
                }
            }
        }

        private void UploadPDFFormButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (var fileStream = File.OpenRead(dialog.FileName))
                {
                    var result = FileManager.SavePDFForm((DB.Enums.PDFFormType)Enum.Parse(typeof(RadiusR.DB.Enums.PDFFormType), PDFFormTypeCombobox.SelectedItem as string), new FileManagerBasicFile(dialog.SafeFileName, fileStream));
                    if (result.InternalException != null)
                    {
                        ShowError(result.InternalException);
                    }
                }
            }
        }

        private void RemovePDFFormButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete PDF Form", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var result = FileManager.RemovePDFForm((DB.Enums.PDFFormType)Enum.Parse(typeof(RadiusR.DB.Enums.PDFFormType), PDFFormTypeCombobox.SelectedItem as string));
                if (result.InternalException != null)
                {
                    ShowError(result.InternalException);
                }
            }
        }

        private void ContractAppendixDownload_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (var result = FileManager.GetContractAppendix())
                {
                    if (result.InternalException != null)
                    {
                        ShowError(result.InternalException);
                        return;
                    }

                    try
                    {
                        using (var fileStream = File.Create($"{dialog.SelectedPath}\\{result.Result.FileName}"))
                        {
                            result.Result.Content.CopyTo(fileStream);
                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex);
                    }
                }
            }
        }

        private void ContractAppendixUpload_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            dialog.Filter = "pdf files (*.pdf)|*.pdf";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (var fileStream = File.OpenRead(dialog.FileName))
                {
                    var file = new FileManagerBasicFile(dialog.SafeFileName, fileStream);
                    var result = FileManager.SaveContractAppendix(file);
                    if (result.InternalException != null)
                    {
                        ShowError(result.InternalException);
                    }
                }
            }
        }

        private void ContractAppendixRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete Contract Appendix", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var result = FileManager.RemoveContractAppendix();
                if (result.InternalException != null)
                {
                    ShowError(result.InternalException);
                }
            }
        }

        private void ContractMailBodyDownloadButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var result = FileManager.GetContractMailBody();
                if (result.InternalException != null)
                {
                    ShowError(result.InternalException);
                }
                else
                {
                    try
                    {
                        using (var fileStream = File.Create($"{dialog.SelectedPath}\\{result.Result.FileName}"))
                        {
                            result.Result.Content.CopyTo(fileStream);
                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex);
                    }
                }
            }
        }

        private void ContractMailBodyUploadButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            dialog.Filter = "html files (*.html)|*.html";
            dialog.DefaultExt = "html";
            dialog.AddExtension = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (var fileStream = File.OpenRead(dialog.FileName))
                {
                    var file = new FileManagerBasicFile(dialog.SafeFileName, fileStream);
                    var result = FileManager.SaveContractMailBody(file);
                    if (result.InternalException != null)
                    {
                        ShowError(result.InternalException);
                    }
                }
            }
        }

        private void ContractMailBodyRemoveButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete Contract Mail Body", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var result = FileManager.RemoveContractMailBody();
                if (result.InternalException != null)
                {
                    ShowError(result.InternalException);
                }
            }
        }

        private void BTKLogUploadButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (var fileStream = File.OpenRead(dialog.FileName))
                {
                    var result = FileManager.SaveBTKLogFile((RadiusR.DB.Enums.BTKLogTypes)Enum.Parse(typeof(RadiusR.DB.Enums.BTKLogTypes), BTKLogTypeCombobox.SelectedItem as string), BTKLogDatetimepicker.Value, new FileManagerBasicFile(dialog.SafeFileName, fileStream));
                    if (result.InternalException != null)
                    {
                        ShowError(result.InternalException);
                    }
                }
            }
        }

        private void BTKLogsListButton_Click(object sender, EventArgs e)
        {
            var results = FileManager.ListBTKLogs((RadiusR.DB.Enums.BTKLogTypes)Enum.Parse(typeof(RadiusR.DB.Enums.BTKLogTypes), BTKLogTypeCombobox.SelectedItem as string), BTKLogDatetimepicker.Value);
            if (results.InternalException != null)
            {
                ShowError(results.InternalException);
            }
            else if(results.Result != null)
            {
                BTKLogsListbox.Items.Clear();
                BTKLogsListbox.Items.AddRange(results.Result.ToArray());
            }
        }

        private void BTKLogsDownloadButton_Click(object sender, EventArgs e)
        {
            if (BTKLogsListbox.SelectedItem != null)
            {
                var dialog = new FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var result = FileManager.GetBTKLog((RadiusR.DB.Enums.BTKLogTypes)Enum.Parse(typeof(RadiusR.DB.Enums.BTKLogTypes), BTKLogTypeCombobox.SelectedItem as string), BTKLogDatetimepicker.Value, BTKLogsListbox.SelectedItem as string);
                    if(result.InternalException != null)
                    {
                        ShowError(result.InternalException);
                    }
                    else
                    {
                        try
                        {
                            using (var fileStream = File.Create($"{dialog.SelectedPath}\\{result.Result.FileName}"))
                            {
                                result.Result.Content.CopyTo(fileStream);
                                fileStream.Flush();
                                fileStream.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex);
                        }
                    }
                }
            }
        }
    }
}
