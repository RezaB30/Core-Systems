using RezaB.Files;
using RadiusR.FileManagement;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using RadiusR.DB;
using RadiusR.DB.Enums;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB.Localization.Bills;

namespace RadiusR.PDFForms
{
    public static class PDFWriter
    {
        private const float baseFontSize = 20f;

        private static PdfFont GetFont()
        {
            if (!PdfFontFactory.IsRegistered("Arial"))
            {
                var fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\Arial.ttf";
                PdfFontFactory.Register(fontPath);
            }

            return PdfFontFactory.CreateRegisteredFont("Arial", PdfEncodings.IDENTITY_H, true);
        }

        public static Stream CreatePDF(Stream BackgroundFile, Stream AppendingPDF, IEnumerable<PDFElement> elements)
        {
            Document resultsDoc;
            SizeF resultSize;
            var fontSize = baseFontSize;
            var resultsStream = new MemoryStream();
            using (var writer = new PdfWriter(resultsStream))
            {
                writer.SetCloseStream(false);

                var pdfDoc = new PdfDocument(writer);
                using (var imageReader = new BinaryReader(BackgroundFile))
                {
                    var imageData = ImageDataFactory.Create(imageReader.ReadBytes((int)imageReader.BaseStream.Length));
                    resultSize = new SizeF(imageData.GetWidth(), imageData.GetHeight());
                    fontSize = baseFontSize * (imageData.GetWidth() / 1024f);
                    resultsDoc = new Document(pdfDoc, new iText.Kernel.Geom.PageSize(resultSize.Width, resultSize.Height));
                    resultsDoc.SetMargins(0f, 0f, 0f, 0f);
                    resultsDoc.SetFont(GetFont());
                    resultsDoc.SetFontSize(fontSize);

                    var image = new iText.Layout.Element.Image(imageData);
                    image.SetFixedPosition(0, 0);
                    resultsDoc.Add(image);

                    foreach (var item in elements)
                    {
                        var paragraph = new Paragraph(item.Text ?? string.Empty);

                        var location = new PointF(resultSize.Width * item.Coords.X, resultSize.Height * (1f - item.Coords.Y) - fontSize * 1.5f);
                        paragraph.SetFixedPosition(location.X, location.Y, 1000f);
                        resultsDoc.Add(paragraph);
                    }

                    if (AppendingPDF != null)
                    {
                        // merge
                        using (var reader = new PdfReader(AppendingPDF))
                        {
                            var loadedDoc = new PdfDocument(reader);
                            loadedDoc.CopyPagesTo(1, loadedDoc.GetNumberOfPages(), pdfDoc);
                        }
                    }

                    //pdfDoc.SetCloseWriter(false);
                    resultsDoc.Close();
                    //writer.Close();
                    resultsStream.Position = 0;
                    return resultsStream;
                }
            }
        }

        public static FileManagerResult<Stream> GetContractPDF(RadiusREntities db, long SubscriptionID, CultureInfo culture = null)
        {
            var subscription = db.Subscriptions.Find(SubscriptionID);
            var formType = subscription.Customer.CustomerType == (short)CustomerType.Individual ? PDFFormType.IndividualContract : PDFFormType.CorporateContract;
            var elementList = LoadFormItems(db, formType, SubscriptionID, null, null, null, culture);
            var fileManager = new MasterISSFileManager();
            using (var pdfFormResult = fileManager.GetPDFForm(formType))
            using (var formAppendixResult = fileManager.GetContractAppendix())
            {
                if (pdfFormResult.InternalException != null || formAppendixResult.InternalException != null)
                {
                    return new FileManagerResult<Stream>(pdfFormResult.InternalException ?? formAppendixResult.InternalException);
                }
                return new FileManagerResult<Stream>(CreatePDF(pdfFormResult.Result.Content, formAppendixResult.Result.Content, elementList));
            }
        }

        public static FileManagerResult<Stream> GetTransitionPDF(RadiusREntities db, long subscriptionID, CultureInfo culture = null)
        {
            var subscription = db.Subscriptions.Find(subscriptionID);
            var formType = subscription.Customer.CustomerType == (short)CustomerType.Individual ? PDFFormType.IndividualTransition : PDFFormType.CorporateTransition;
            var elementList = LoadFormItems(db, formType, subscriptionID, null, null, null, culture);
            var fileManager = new MasterISSFileManager();
            using (var pdfFormResult = fileManager.GetPDFForm(formType))
            {
                if (pdfFormResult.InternalException != null)
                {
                    return new FileManagerResult<Stream>(pdfFormResult.InternalException);
                }
                return new FileManagerResult<Stream>(CreatePDF(pdfFormResult.Result.Content, null, elementList));
            }
        }

        public static FileManagerResult<Stream> GetPSTNtoNakedPDF(RadiusREntities db, long subscriptionID, CultureInfo culture = null)
        {
            var elementList = LoadFormItems(db, PDFFormType.PSTNtoNaked, subscriptionID, null, null, null, culture);
            var fileManager = new MasterISSFileManager();
            using (var pdfFormResult = fileManager.GetPDFForm(PDFFormType.PSTNtoNaked))
            {
                if (pdfFormResult.InternalException != null)
                {
                    return new FileManagerResult<Stream>(pdfFormResult.InternalException);
                }
                return new FileManagerResult<Stream>(CreatePDF(pdfFormResult.Result.Content, null, elementList));
            }
        }

        public static FileManagerResult<Stream> GetTransferPDF(RadiusREntities db, long transferringSubscriptionID, long transferredSubscriptionID, CultureInfo culture = null)
        {
            var elementList = LoadFormItems(db, PDFFormType.Transfer, null, transferringSubscriptionID, transferredSubscriptionID, null, culture);
            var fileManager = new MasterISSFileManager();
            using (var pdfFormResult = fileManager.GetPDFForm(PDFFormType.Transfer))
            {
                if (pdfFormResult.InternalException != null)
                {
                    return new FileManagerResult<Stream>(pdfFormResult.InternalException);
                }
                return new FileManagerResult<Stream>(CreatePDF(pdfFormResult.Result.Content, null, elementList));
            }
        }

        public static FileManagerResult<Stream> GetTransportPDF(RadiusREntities db, long subscriptionID, CultureInfo culture = null)
        {
            var elementList = LoadFormItems(db, PDFFormType.Transport, subscriptionID, null, null, null, culture);
            var fileManager = new MasterISSFileManager();
            using (var pdfFormResult = fileManager.GetPDFForm(PDFFormType.Transport))
            {
                if (pdfFormResult.InternalException != null)
                {
                    return new FileManagerResult<Stream>(pdfFormResult.InternalException);
                }
                return new FileManagerResult<Stream>(CreatePDF(pdfFormResult.Result.Content, null, elementList));
            }
        }

        public static FileManagerResult<Stream> GetBillReceiptPDF(RadiusREntities db, long subscriptionID, long billId, CultureInfo culture = null)
        {
            var elementList = LoadFormItems(db, PDFFormType.BillReceipt, subscriptionID, null, null, billId, culture);
            var fileManager = new MasterISSFileManager();
            using (var pdfFormResult = fileManager.GetPDFForm(PDFFormType.BillReceipt))
            {
                if (pdfFormResult.InternalException != null)
                {
                    return new FileManagerResult<Stream>(pdfFormResult.InternalException);
                }
                return new FileManagerResult<Stream>(CreatePDF(pdfFormResult.Result.Content, null, elementList));
            }
        }

        private static IEnumerable<PDFElement> LoadFormItems(RadiusREntities db, PDFFormType formType, long? subscriptionId, long? transferringSubscriptionId, long? transferredSubscriptionId, long? billId, CultureInfo culture = null)
        {
            var subscription = subscriptionId.HasValue ? db.Subscriptions.Find(subscriptionId) : null;
            var bill = billId.HasValue ? db.Bills.Find(billId) : null;
            var transferringSubscription = transferringSubscriptionId.HasValue ? db.Subscriptions.Find(transferringSubscriptionId) : null;
            var transferredSubscription = transferredSubscriptionId.HasValue ? db.Subscriptions.Find(transferredSubscriptionId) : null;
            var placeList = db.PDFFormItemPlacements.Where(item => item.FormType == (int)formType).ToList();

            List<PDFElement> elementList = new List<PDFElement>();
            foreach (var item in placeList)
            {
                if (subscription != null)
                {
                    switch ((PDFItemIDs)item.ItemID)
                    {
                        case PDFItemIDs.SubscriberNo:
                            elementList.Add(new PDFElement()
                            {
                                Text = subscription.SubscriberNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.FirstName:
                            elementList.Add(new PDFElement
                            {
                                Text = subscription.Customer.FirstName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.LastName:
                            elementList.Add(new PDFElement
                            {
                                Text = subscription.Customer.LastName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.BillingAddress:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.AddressText, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.BirthDate:
                            elementList.Add(new PDFElement
                            {
                                Text = subscription.Customer.BirthDate.ToString("dd MMMM yyyy", culture),
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.BirthPlace:
                            elementList.Add(new PDFElement
                            {
                                Text = subscription.Customer.BirthPlace,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.CentralSystemNo:
                            elementList.Add(new PDFElement { Text = subscription.Customer.CorporateCustomerInfo.CentralSystemNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.CompanyTitle:
                            elementList.Add(new PDFElement { Text = subscription.Customer.CorporateCustomerInfo.Title, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ContactPhoneNo:
                            elementList.Add(new PDFElement { Text = subscription.Customer.ContactPhoneNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.Email:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Email, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.FathersName:
                            elementList.Add(new PDFElement
                            {
                                Text = subscription.Customer.FathersName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.PSTN:
                            elementList.Add(new PDFElement { Text = subscription.SubscriptionTelekomInfo != null ? subscription.SubscriptionTelekomInfo.PSTN : "", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.LandPhone_No:
                            elementList.Add(new PDFElement { Text = subscription.SubscriptionTelekomInfo == null || string.IsNullOrEmpty(subscription.SubscriptionTelekomInfo.PSTN) ? "X" : "", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.LandPhone_Yes:
                            elementList.Add(new PDFElement { Text = subscription.SubscriptionTelekomInfo != null && !string.IsNullOrEmpty(subscription.SubscriptionTelekomInfo.PSTN) ? "X" : "", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.MothersMaidenName:
                            elementList.Add(new PDFElement
                            {
                                Text = subscription.Customer.MothersMaidenName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.MothersName:
                            elementList.Add(new PDFElement
                            {
                                Text = subscription.Customer.MothersName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Nationality:
                            elementList.Add(new PDFElement
                            {
                                Text = new LocalizedList<CountryCodes, RadiusR.Localization.Lists.CountryCodes>().GetDisplayText(subscription.Customer.Nationality, culture),
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Profession:
                            elementList.Add(new PDFElement
                            {
                                Text = new LocalizedList<Profession, RadiusR.Localization.Lists.Profession>().GetDisplayText(subscription.Customer.Profession, culture),
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.ResidenceAddress_BuildingName:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.BuildingName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ResidenceAddress_BuildingNo:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.ApartmentNo.Replace("Ic Kapi(Daire) ", ""), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ResidenceAddress_DistrictName:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.DistrictName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ResidenceAddress_DoorNo:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.DoorNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ResidenceAddress_Floor:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.Floor, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ResidenceAddress_NeighborhoodName:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.NeighborhoodName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ResidenceAddress_ProvinceName:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.ProvinceName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ResidenceAddress_StreetName:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.StreetName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ResidenceAddress_PostalCode:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.PostalCode.ToString(), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.ResidenceAddress_BBK:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Address.ApartmentID.ToString(), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.Sex_Female:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Sex == 1 ? " " : "X", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.Sex_Male:
                            elementList.Add(new PDFElement { Text = subscription.Customer.Sex == 2 ? " " : "X", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_BuildingName:
                            elementList.Add(new PDFElement { Text = subscription.Address.BuildingName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_BuildingNo:
                            elementList.Add(new PDFElement { Text = subscription.Address.ApartmentNo.Replace("Ic Kapi(Daire) ", ""), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_DistrictName:
                            elementList.Add(new PDFElement { Text = subscription.Address.DistrictName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_DoorNo:
                            elementList.Add(new PDFElement { Text = subscription.Address.DoorNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_Floor:
                            elementList.Add(new PDFElement { Text = subscription.Address.Floor, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_NeighborhoodName:
                            elementList.Add(new PDFElement { Text = subscription.Address.NeighborhoodName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_ProvinceName:
                            elementList.Add(new PDFElement { Text = subscription.Address.ProvinceName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_StreetName:
                            elementList.Add(new PDFElement { Text = subscription.Address.StreetName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_PostalCode:
                            elementList.Add(new PDFElement { Text = subscription.Address.PostalCode.ToString(), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.SubscriptionAddress_BBK:
                            elementList.Add(new PDFElement { Text = subscription.Address.ApartmentID.ToString(), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.TariffName:
                            elementList.Add(new PDFElement { Text = subscription.Service.Name, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.TaxNo:
                            elementList.Add(new PDFElement { Text = subscription.Customer.CorporateCustomerInfo.TaxNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.TCKAndTaxNo:
                            elementList.Add(new PDFElement()
                            {
                                Text = subscription.Customer.CustomerType == (short)CustomerType.Individual ? subscription.Customer.CustomerIDCard.TCKNo : subscription.Customer.CorporateCustomerInfo.TaxNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.FirstAndLastName:
                            elementList.Add(new PDFElement()
                            {
                                Text = subscription.Customer.FirstName + " " + subscription.Customer.LastName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.DisplayName:
                            elementList.Add(new PDFElement()
                            {
                                Text = subscription.ValidDisplayName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.TaxOffice:
                            elementList.Add(new PDFElement { Text = subscription.Customer.CorporateCustomerInfo.TaxOffice, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.TCKNo:
                            elementList.Add(new PDFElement
                            {
                                Text = subscription.Customer.CustomerIDCard.TCKNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.TradeRegistrationNo:
                            elementList.Add(new PDFElement { Text = subscription.Customer.CorporateCustomerInfo.TradeRegistrationNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                            break;
                        case PDFItemIDs.XDSLNo:
                            elementList.Add(new PDFElement
                            {
                                Text = subscription.SubscriptionTelekomInfo?.SubscriptionNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.TransitionSourceOperator:
                            elementList.Add(new PDFElement()
                            {
                                Text = subscription.SubscriptionTelekomInfo?.TransitionOperator?.DisplayName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        default:
                            break;
                    }
                }
                if (transferredSubscription != null)
                {
                    switch ((PDFItemIDs)item.ItemID)
                    {
                        case PDFItemIDs.Transferred_SubscriberNo:
                            elementList.Add(new PDFElement
                            {
                                Text = transferredSubscription.SubscriberNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferred_FirstName:
                            elementList.Add(new PDFElement
                            {
                                Text = transferredSubscription.Customer.FirstName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferred_LastName:
                            elementList.Add(new PDFElement
                            {
                                Text = transferredSubscription.Customer.LastName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferred_FirstAndLastName:
                            elementList.Add(new PDFElement
                            {
                                Text = transferredSubscription.Customer.FirstName + " " + transferredSubscription.Customer.LastName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferred_CompanyTitle:
                            elementList.Add(new PDFElement
                            {
                                Text = transferredSubscription.Customer.CorporateCustomerInfo?.Title,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferred_DisplayName:
                            elementList.Add(new PDFElement
                            {
                                Text = transferredSubscription.ValidDisplayName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferred_TCKNo:
                            elementList.Add(new PDFElement
                            {
                                Text = transferredSubscription.Customer.CustomerIDCard.TCKNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferred_TaxNo:
                            elementList.Add(new PDFElement
                            {
                                Text = transferredSubscription.Customer.CorporateCustomerInfo?.TaxNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferred_TCKAndTaxNo:
                            elementList.Add(new PDFElement
                            {
                                Text = transferredSubscription.Customer.CustomerType == (short)CustomerType.Individual ? transferredSubscription.Customer.CustomerIDCard.TCKNo : transferredSubscription.Customer.CorporateCustomerInfo?.TaxNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        default:
                            break;
                    }
                }
                if (transferringSubscription != null)
                {
                    switch ((PDFItemIDs)item.ItemID)
                    {
                        case PDFItemIDs.Transferring_SubscriberNo:
                            elementList.Add(new PDFElement
                            {
                                Text = transferringSubscription.SubscriberNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferring_FirstName:
                            elementList.Add(new PDFElement
                            {
                                Text = transferringSubscription.Customer.FirstName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferring_LastName:
                            elementList.Add(new PDFElement
                            {
                                Text = transferringSubscription.Customer.LastName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferring_FirstAndLastName:
                            elementList.Add(new PDFElement
                            {
                                Text = transferringSubscription.Customer.FirstName + " " + transferringSubscription.Customer.LastName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferring_CompanyTitle:
                            elementList.Add(new PDFElement
                            {
                                Text = transferringSubscription.Customer.CorporateCustomerInfo?.Title,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferring_DisplayName:
                            elementList.Add(new PDFElement
                            {
                                Text = transferringSubscription.ValidDisplayName,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferring_TCKNo:
                            elementList.Add(new PDFElement
                            {
                                Text = transferringSubscription.Customer.CustomerIDCard.TCKNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferring_TaxNo:
                            elementList.Add(new PDFElement
                            {
                                Text = transferringSubscription.Customer.CorporateCustomerInfo?.TaxNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Transferring_TCKAndTaxNo:
                            elementList.Add(new PDFElement
                            {
                                Text = transferringSubscription.Customer.CustomerType == (short)CustomerType.Individual ? transferringSubscription.Customer.CustomerIDCard.TCKNo : transferringSubscription.Customer.CorporateCustomerInfo?.TaxNo,
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        default:
                            break;
                    }
                }
                if (bill != null)
                {
                    var billFees = bill.BillFees.Select(bf => new { Name = bf.GetDisplayName(enforcedCulture: culture), Cost = bf.CurrentCost, Discount = bf.Discount?.Amount ?? 0m }).ToArray();
                    var tlSign = CultureInfo.CreateSpecificCulture("tr-tr").NumberFormat.CurrencySymbol;
                    switch ((PDFItemIDs)item.ItemID)
                    {
                        case PDFItemIDs.Bill_IssueDate:
                            elementList.Add(new PDFElement()
                            {
                                Text = bill.IssueDate.ToString("dd MMMM yyyy", culture),
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        case PDFItemIDs.Bill_PaymentDate:
                            if (bill.PayDate.HasValue)
                                elementList.Add(new PDFElement()
                                {
                                    Text = bill.PayDate.Value.ToString("dd MMMM yyyy", culture),
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_1:
                            if (billFees.Length > 0)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[0].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_2:
                            if (billFees.Length > 1)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[1].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_3:
                            if (billFees.Length > 2)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[2].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_4:
                            if (billFees.Length > 3)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[3].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_5:
                            if (billFees.Length > 4)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[4].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_6:
                            if (billFees.Length > 5)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[5].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_7:
                            if (billFees.Length > 6)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[6].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_8:
                            if (billFees.Length > 7)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[7].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_9:
                            if (billFees.Length > 8)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[8].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_10:
                            if (billFees.Length > 9)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[9].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_11:
                            if (billFees.Length > 10)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[10].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeItems_12:
                            if (billFees.Length > 11)
                                elementList.Add(new PDFElement()
                                {
                                    Text = billFees[11].Name,
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            break;
                        case PDFItemIDs.Bill_FeeAmount_1:
                            if (billFees.Length > 0)
                            {
                                var text = $"{billFees[0].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[0].Discount > 0m ? $" - {billFees[0].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_2:
                            if (billFees.Length > 1)
                            {
                                var text = $"{billFees[1].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[1].Discount > 0m ? $" - {billFees[1].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_3:
                            if (billFees.Length > 2)
                            {
                                var text = $"{billFees[2].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[2].Discount > 0m ? $" - {billFees[2].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_4:
                            if (billFees.Length > 3)
                            {
                                var text = $"{billFees[3].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[3].Discount > 0m ? $" - {billFees[3].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_5:
                            if (billFees.Length > 4)
                            {
                                var text = $"{billFees[4].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[4].Discount > 0m ? $" - {billFees[4].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_6:
                            if (billFees.Length > 5)
                            {
                                var text = $"{billFees[5].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[5].Discount > 0m ? $" - {billFees[5].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_7:
                            if (billFees.Length > 6)
                            {
                                var text = $"{billFees[6].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[6].Discount > 0m ? $" - {billFees[6].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_8:
                            if (billFees.Length > 7)
                            {
                                var text = $"{billFees[7].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[7].Discount > 0m ? $" - {billFees[7].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_9:
                            if (billFees.Length > 8)
                            {
                                var text = $"{billFees[8].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[8].Discount > 0m ? $" - {billFees[8].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_10:
                            if (billFees.Length > 9)
                            {
                                var text = $"{billFees[9].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[9].Discount > 0m ? $" - {billFees[9].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_11:
                            if (billFees.Length > 10)
                            {
                                var text = $"{billFees[10].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[10].Discount > 0m ? $" - {billFees[10].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_FeeAmount_12:
                            if (billFees.Length > 11)
                            {
                                var text = $"{billFees[11].Cost.ToString("###,##0,00", culture)}{tlSign}";
                                var discountText = billFees[11].Discount > 0m ? $" - {billFees[11].Discount.ToString("###,##0,00", culture)}{tlSign}" : null;
                                elementList.Add(new PDFElement()
                                {
                                    Text = $"{text}{discountText}",
                                    Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                                });
                            }
                            break;
                        case PDFItemIDs.Bill_TotalFee:
                            elementList.Add(new PDFElement()
                            {
                                Text = $"{billFees.Select(bf => bf.Cost - bf.Discount).DefaultIfEmpty(0m).Sum().ToString("###,###,##0.00", culture)}{tlSign}",
                                Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                            });
                            break;
                        default:
                            break;
                    }
                }
            }

            return elementList;
        }
    }
}
