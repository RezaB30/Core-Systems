using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.FileManagement;
using RezaB.Data.Localization;
using RezaB.Files;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<PDFElement> ElementList = new List<PDFElement>();
            var Subscription = db.Subscriptions.Find(SubscriptionID);
            var PlaceList = new List<PDFFormItemPlacement>();
            var formType = PDFFormType.IndividualContract;
            if (Subscription.Customer.CustomerType == (short)CustomerType.Individual)
            {
                PlaceList = db.PDFFormItemPlacements.Where(item => item.FormType == (int)PDFFormType.IndividualContract).ToList();
                formType = PDFFormType.IndividualContract;
            }
            else if (Subscription.Customer.CorporateCustomerInfo != null)
            {
                PlaceList = db.PDFFormItemPlacements.Where(item => item.FormType == (int)PDFFormType.CorporateContract).ToList();
                formType = PDFFormType.CorporateContract;
            }

            foreach (var item in PlaceList)
            {
                switch ((PDFItemIDs)item.ItemID)
                {
                    case PDFItemIDs.FirstName:
                        ElementList.Add(new PDFElement
                        {
                            Text = Subscription.Customer.FirstName,
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.LastName:
                        ElementList.Add(new PDFElement
                        {
                            Text = Subscription.Customer.LastName,
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.BillingAddress:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.AddressText, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.BirthDate:
                        ElementList.Add(new PDFElement
                        {
                            Text = Subscription.Customer.BirthDate.ToString("dd MMMM yyyy", culture),
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.BirthPlace:
                        ElementList.Add(new PDFElement
                        {
                            Text = Subscription.Customer.BirthPlace,
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.CentralSystemNo:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.CorporateCustomerInfo.CentralSystemNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.CompanyTitle:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.CorporateCustomerInfo.Title, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ContactPhoneNo:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.ContactPhoneNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.Email:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Email, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.FathersName:
                        ElementList.Add(new PDFElement
                        {
                            Text = Subscription.Customer.FathersName,
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.PSTN:
                        ElementList.Add(new PDFElement { Text = Subscription.SubscriptionTelekomInfo != null ? Subscription.SubscriptionTelekomInfo.PSTN : "", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.LandPhone_No:
                        ElementList.Add(new PDFElement { Text = Subscription.SubscriptionTelekomInfo == null || string.IsNullOrEmpty(Subscription.SubscriptionTelekomInfo.PSTN) ? "X" : "", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.LandPhone_Yes:
                        ElementList.Add(new PDFElement { Text = Subscription.SubscriptionTelekomInfo != null && !string.IsNullOrEmpty(Subscription.SubscriptionTelekomInfo.PSTN) ? "X" : "", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.MothersMaidenName:
                        ElementList.Add(new PDFElement
                        {
                            Text = Subscription.Customer.MothersMaidenName,
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.MothersName:
                        ElementList.Add(new PDFElement
                        {
                            Text = Subscription.Customer.MothersName,
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.Nationality:
                        ElementList.Add(new PDFElement
                        {
                            Text = new LocalizedList<CountryCodes, RadiusR.Localization.Lists.CountryCodes>().GetDisplayText(Subscription.Customer.Nationality, culture),
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.Profession:
                        ElementList.Add(new PDFElement
                        {
                            Text = new LocalizedList<Profession, RadiusR.Localization.Lists.Profession>().GetDisplayText(Subscription.Customer.Profession, culture),
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.ResidenceAddress_BuildingName:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.BuildingName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ResidenceAddress_BuildingNo:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.ApartmentNo.Replace("Ic Kapi(Daire) ", ""), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ResidenceAddress_DistrictName:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.DistrictName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ResidenceAddress_DoorNo:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.DoorNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ResidenceAddress_Floor:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.Floor, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ResidenceAddress_NeighborhoodName:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.NeighborhoodName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ResidenceAddress_ProvinceName:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.ProvinceName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ResidenceAddress_StreetName:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.StreetName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ResidenceAddress_PostalCode:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.PostalCode.ToString(), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.ResidenceAddress_BBK:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Address.ApartmentID.ToString(), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.Sex_Female:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Sex == 1 ? " " : "X", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.Sex_Male:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.Sex == 2 ? " " : "X", Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_BuildingName:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.BuildingName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_BuildingNo:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.ApartmentNo.Replace("Ic Kapi(Daire) ", ""), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_DistrictName:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.DistrictName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_DoorNo:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.DoorNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_Floor:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.Floor, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_NeighborhoodName:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.NeighborhoodName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_ProvinceName:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.ProvinceName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_StreetName:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.StreetName, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_PostalCode:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.PostalCode.ToString(), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.SubscriptionAddress_BBK:
                        ElementList.Add(new PDFElement { Text = Subscription.Address.ApartmentID.ToString(), Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.TariffName:
                        ElementList.Add(new PDFElement { Text = Subscription.Service.Name, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.TaxNo:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.CorporateCustomerInfo.TaxNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.TaxOffice:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.CorporateCustomerInfo.TaxOffice, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    case PDFItemIDs.TCKNo:
                        ElementList.Add(new PDFElement
                        {
                            Text = Subscription.Customer.CustomerIDCard.TCKNo,
                            Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY))
                        });
                        break;
                    case PDFItemIDs.TradeRegistrationNo:
                        ElementList.Add(new PDFElement { Text = Subscription.Customer.CorporateCustomerInfo.TradeRegistrationNo, Coords = new PointF(Convert.ToSingle(item.CoordsX), Convert.ToSingle(item.CoordsY)) });
                        break;
                    default:
                        break;
                }

            }

            var fileManager = new MasterISSFileManager();
            using (var pdfFormResult = fileManager.GetPDFForm(formType))
            using (var formAppendixResult = fileManager.GetContractAppendix())
            {
                if (pdfFormResult.InternalException != null || formAppendixResult.InternalException != null)
                {
                    return new FileManagerResult<Stream>(pdfFormResult.InternalException ?? formAppendixResult.InternalException);
                }
                return new FileManagerResult<Stream>(CreatePDF(pdfFormResult.Result.Content, formAppendixResult.Result.Content, ElementList));
            }
        }
    }
}
