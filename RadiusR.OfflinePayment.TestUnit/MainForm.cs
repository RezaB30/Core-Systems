using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.IO;

namespace RadiusR.OfflinePayment.TestUnit
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            var formatData = new List<KeyValuePair<int, string>>();
            var formatTypes = Enum.GetValues(typeof(FormatTypes));
            foreach (var item in formatTypes)
            {
                formatData.Add(new KeyValuePair<int, string>((int)item, Enum.GetName(typeof(FormatTypes), item)));
            }
            UploadFormatCombobox.DataSource = formatData;
            UploadFormatCombobox.DisplayMember = "Value";
            UploadFormatCombobox.ValueMember = "Key";

            ProcessFormatCombobox.DataSource = formatData;
            ProcessFormatCombobox.DisplayMember = "Value";
            ProcessFormatCombobox.ValueMember = "Key";
        }

        private void CreateUploadFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = "txt files (*.txt)|*.txt";
            dialog.CheckPathExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StatusLabel.Text = "Loading from data base...";
                try
                {
                    using (DB.RadiusREntities db = new DB.RadiusREntities())
                    {
                        var startTime = DateTime.Now;
                        long startingID = 0;
                        using (var finalStream = new MemoryStream())
                        {
                            while (true)
                            {
                                var baseQuery = db.Bills.OrderBy(b => b.ID).Where(b => b.BillStatusID == (short)DB.Enums.BillState.Unpaid || (b.BillStatusID == (short)DB.Enums.BillState.Paid && b.PayDate > startTime)).Select(b => new
                                {
                                    BillNo = b.ID,
                                    Amount = b.BillFees.Select(bf => bf.CurrentCost - (bf.DiscountID.HasValue ? bf.Discount.Amount : 0m)).DefaultIfEmpty(0m).Sum(),
                                    DueDate = b.DueDate,
                                    FullName = b.Subscription.Customer.CorporateCustomerInfo != null ? b.Subscription.Customer.CorporateCustomerInfo.Title : b.Subscription.Customer.FirstName + " " + b.Subscription.Customer.LastName,
                                    SubscriberNo = b.Subscription.SubscriberNo
                                });
                                var query = baseQuery.Where(b => b.BillNo > startingID);
                                var results = query.Take(1000).ToArray();
                                if (results.Count() == 0)
                                {
                                    var totalAmount = baseQuery.Select(b => b.Amount).DefaultIfEmpty(0m).Sum();
                                    var totalCount = baseQuery.Count();
                                    BatchProcessor.CopyToStream(finalStream, (FormatTypes)UploadFormatCombobox.SelectedValue, Enumerable.Empty<Sending.BatchReadyBill>(), new Sending.FinishLine()
                                    {
                                        TotalAmount = totalAmount,
                                        TotalCount = totalCount
                                    });
                                    break;
                                }

                                BatchProcessor.CopyToStream(finalStream, (FormatTypes)UploadFormatCombobox.SelectedValue, results.Select(r => new Sending.BatchReadyBill()
                                {
                                    Amount = r.Amount,
                                    BillNo = r.BillNo.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                    DueDate = r.DueDate,
                                    FullName = r.FullName,
                                    SubscriberNo = r.SubscriberNo
                                }), WriteHeaderLine: startingID == 0);

                                startingID = results.Max(r => r.BillNo);
                            }

                            finalStream.Seek(0, SeekOrigin.Begin);
                            using (var stream = File.OpenWrite(dialog.FileName))
                            {
                                finalStream.CopyTo(stream);
                                stream.Flush();
                                stream.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    StatusLabel.Text = "Error";
                }

                StatusLabel.Text = "File saved";
            }
        }

        private void ProcessButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = dialog.CheckPathExists = true;
            dialog.Filter = "txt files (*.txt)|*.txt";

            if (dialog.ShowDialog()== DialogResult.OK)
            {
                StatusLabel.Text = "Processing downloaded file...";
                try
                {
                    using (var source = File.Open(dialog.FileName, FileMode.Open))
                    {
                        var results = BatchProcessor.ProcessStream(source, (FormatTypes)ProcessFormatCombobox.SelectedValue);
                        ProcessedBatchGridview.DataSource = results.ToArray();
                        foreach (DataGridViewRow item in ProcessedBatchGridview.Rows)
                        {
                            item.HeaderCell.Value = Convert.ToString(item.Index + 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    StatusLabel.Text = "Error";
                }
            }
        }
    }
}
