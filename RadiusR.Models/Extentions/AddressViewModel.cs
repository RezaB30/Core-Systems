using RadiusR.DB;
using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.Extentions
{
    public static class AddressViewModelExtention
    {
        public static void CopyToDBObject(this AddressViewModel address, Address dbAddress)
        {
            dbAddress.AddressNo = address.AddressNo;
            dbAddress.AddressText = address.AddressText;
            dbAddress.ApartmentID = address.ApartmentID;
            dbAddress.ApartmentNo = address.ApartmentNo;
            dbAddress.DistrictID = address.DistrictID;
            dbAddress.DistrictName = address.DistrictName;
            dbAddress.DoorID = address.DoorID;
            dbAddress.DoorNo = address.DoorNo;
            dbAddress.NeighborhoodID = address.NeighbourhoodID;
            dbAddress.NeighborhoodName = address.NeighborhoodName;
            dbAddress.PostalCode = address.PostalCode.Value;
            dbAddress.Floor = address.Floor;
            dbAddress.ProvinceID = address.ProvinceID;
            dbAddress.ProvinceName = address.ProvinceName;
            dbAddress.RuralCode = address.RuralCode;
            dbAddress.StreetID = address.StreetID;
            dbAddress.StreetName = address.StreetName;
        }
    }
}
