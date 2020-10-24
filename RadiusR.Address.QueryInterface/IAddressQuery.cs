using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Address.QueryInterface
{
    public interface IAddressQuery
    {
        RadiusAddress<IEnumerable<ValueNamePair>> GetProvinces();

        RadiusAddress<IEnumerable<ValueNamePair>> GetProvinceDistricts(long provinceId);

        RadiusAddress<IEnumerable<ValueNamePair>> GetDistrictRuralRegions(long districtId);

        RadiusAddress<IEnumerable<ValueNamePair>> GetRuralRegionNeighbourhoods(long ruralRegionId);

        RadiusAddress<IEnumerable<ValueNamePair>> GetNeighbourhoodStreets(long neighbourhoodId);

        RadiusAddress<IEnumerable<ValueNamePair>> GetStreetBuildings(long streetId);

        RadiusAddress<IEnumerable<ValueNamePair>> GetBuildingApartments(long buildingId);

        RadiusAddress<AddressDetails> GetApartmentAddress(long apartmentId);
    }
}
