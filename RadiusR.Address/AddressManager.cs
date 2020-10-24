using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.Address.QueryInterface;
using RadiusR.DB.Enums;
using RadiusR.DB.Settings;
using RadiusR.API.AddressQueryAdapter;
using RezaB.TurkTelekom.WebServices.Address;

namespace RadiusR.Address
{
    public class AddressManager : IAddressQuery
    {
        public RadiusAddress<AddressDetails> GetApartmentAddress(long apartmentId)
        {
            switch (AddressAPISettings.AddressAPIType)
            {
                case (short)AddressAPIType.AddressQueryService:
                    {
                        var adapter = new AddressServiceAdapter();
                        return adapter.GetApartmentAddress(apartmentId);
                    }
                case (short)AddressAPIType.DirectAccess:
                    {
                        var serviceClient = new AddressServiceClient(AddressAPISettings.AddressAPIDirectUserId, AddressAPISettings.AddressAPIDirectPassword);
                        var addressTextResults = serviceClient.GetAddressFromCode(apartmentId);
                        var addressNoResults = serviceClient.GetAddressNo(apartmentId);
                        if (addressTextResults.InternalException != null || addressNoResults.InternalException != null)
                        {
                            return new RadiusAddress<AddressDetails>()
                            {
                                ErrorOccured = true,
                                ErrorMessage = addressTextResults.InternalException != null ? addressTextResults.InternalException.GetShortMessage() : addressNoResults.InternalException.GetShortMessage()
                            };
                        }
                        return new RadiusAddress<AddressDetails>()
                        {
                            ErrorOccured = false,
                            Data = new AddressDetails()
                            {
                                AddressNo = addressNoResults.Data ?? 0,
                                AddressText = addressTextResults.Data.AddressText,
                                ApartmentID = addressTextResults.Data.ApartmentID,
                                ApartmentNo = addressTextResults.Data.ApartmentNo,
                                DistrictID = addressTextResults.Data.DistrictID,
                                DistrictName = addressTextResults.Data.DistrictName,
                                DoorID = addressTextResults.Data.DoorID,
                                DoorNo = addressTextResults.Data.DoorNo,
                                NeighbourhoodID = addressTextResults.Data.NeighbourhoodID,
                                NeighbourhoodName = addressTextResults.Data.NeighbourhoodName,
                                ProvinceID = addressTextResults.Data.ProvinceID,
                                ProvinceName = addressTextResults.Data.ProvinceName,
                                RuralCode = addressTextResults.Data.RuralCode,
                                StreetID = addressTextResults.Data.StreetID,
                                StreetName = addressTextResults.Data.StreetName
                            }
                        };
                    }
                default:
                    return new RadiusAddress<AddressDetails>()
                    {
                        ErrorOccured = true,
                        ErrorMessage = "Invalid Address API Type."
                    };
            }
        }

        public RadiusAddress<IEnumerable<ValueNamePair>> GetBuildingApartments(long buildingId)
        {
            switch (AddressAPISettings.AddressAPIType)
            {
                case (short)AddressAPIType.AddressQueryService:
                    {
                        var adapter = new AddressServiceAdapter();
                        return adapter.GetBuildingApartments(buildingId);
                    }
                case (short)AddressAPIType.DirectAccess:
                    {
                        var serviceClient = new AddressServiceClient(AddressAPISettings.AddressAPIDirectUserId, AddressAPISettings.AddressAPIDirectPassword);
                        var results = serviceClient.GetBuildingApartmetns(buildingId);
                        if (results.InternalException != null)
                        {
                            return new RadiusAddress<IEnumerable<ValueNamePair>>()
                            {
                                ErrorOccured = true,
                                ErrorMessage = results.InternalException.GetShortMessage()
                            };
                        }
                        return new RadiusAddress<IEnumerable<ValueNamePair>>()
                        {
                            ErrorOccured = false,
                            Data = results.Data.Select(d => new ValueNamePair()
                            {
                                Code = d.Code,
                                Name = d.Name
                            }).ToArray()
                        };
                    }
                default:
                    return new RadiusAddress<IEnumerable<ValueNamePair>>()
                    {
                        ErrorOccured = true,
                        ErrorMessage = "Invalid Address API Type."
                    };
            }
        }

        public RadiusAddress<IEnumerable<ValueNamePair>> GetDistrictRuralRegions(long districtId)
        {
            switch (AddressAPISettings.AddressAPIType)
            {
                case (short)AddressAPIType.AddressQueryService:
                    {
                        var adapter = new AddressServiceAdapter();
                        return adapter.GetDistrictRuralRegions(districtId);
                    }
                case (short)AddressAPIType.DirectAccess:
                    {
                        var serviceClient = new AddressServiceClient(AddressAPISettings.AddressAPIDirectUserId, AddressAPISettings.AddressAPIDirectPassword);
                        var results = serviceClient.GetCityRegions(districtId);
                        if (results.InternalException != null)
                        {
                            return new RadiusAddress<IEnumerable<ValueNamePair>>()
                            {
                                ErrorOccured = true,
                                ErrorMessage = results.InternalException.GetShortMessage()
                            };
                        }
                        return new RadiusAddress<IEnumerable<ValueNamePair>>()
                        {
                            ErrorOccured = false,
                            Data = results.Data.Select(d => new ValueNamePair()
                            {
                                Code = d.Code,
                                Name = d.Name
                            }).ToArray()
                        };
                    }
                default:
                    return new RadiusAddress<IEnumerable<ValueNamePair>>()
                    {
                        ErrorOccured = true,
                        ErrorMessage = "Invalid Address API Type."
                    };
            }
        }

        public RadiusAddress<IEnumerable<ValueNamePair>> GetNeighbourhoodStreets(long neighbourhoodId)
        {
            switch (AddressAPISettings.AddressAPIType)
            {
                case (short)AddressAPIType.AddressQueryService:
                    {
                        var adapter = new AddressServiceAdapter();
                        return adapter.GetNeighbourhoodStreets(neighbourhoodId);
                    }
                case (short)AddressAPIType.DirectAccess:
                    {
                        var serviceClient = new AddressServiceClient(AddressAPISettings.AddressAPIDirectUserId, AddressAPISettings.AddressAPIDirectPassword);
                        var results = serviceClient.GetDistrictStreets(neighbourhoodId);
                        if (results.InternalException != null)
                        {
                            return new RadiusAddress<IEnumerable<ValueNamePair>>()
                            {
                                ErrorOccured = true,
                                ErrorMessage = results.InternalException.GetShortMessage()
                            };
                        }
                        return new RadiusAddress<IEnumerable<ValueNamePair>>()
                        {
                            ErrorOccured = false,
                            Data = results.Data.Select(d => new ValueNamePair()
                            {
                                Code = d.Code,
                                Name = d.Name
                            }).ToArray()
                        };
                    }
                default:
                    return new RadiusAddress<IEnumerable<ValueNamePair>>()
                    {
                        ErrorOccured = true,
                        ErrorMessage = "Invalid Address API Type."
                    };
            }
        }

        public RadiusAddress<IEnumerable<ValueNamePair>> GetProvinceDistricts(long provinceId)
        {
            switch (AddressAPISettings.AddressAPIType)
            {
                case (short)AddressAPIType.AddressQueryService:
                    {
                        var adapter = new AddressServiceAdapter();
                        return adapter.GetProvinceDistricts(provinceId);
                    }
                case (short)AddressAPIType.DirectAccess:
                    {
                        var serviceClient = new AddressServiceClient(AddressAPISettings.AddressAPIDirectUserId, AddressAPISettings.AddressAPIDirectPassword);
                        var results = serviceClient.GetProvinceCities(provinceId);
                        if (results.InternalException != null)
                        {
                            return new RadiusAddress<IEnumerable<ValueNamePair>>()
                            {
                                ErrorOccured = true,
                                ErrorMessage = results.InternalException.GetShortMessage()
                            };
                        }
                        return new RadiusAddress<IEnumerable<ValueNamePair>>()
                        {
                            ErrorOccured = false,
                            Data = results.Data.Select(d => new ValueNamePair()
                            {
                                Code = d.Code,
                                Name = d.Name
                            }).ToArray()
                        };
                    }
                default:
                    return new RadiusAddress<IEnumerable<ValueNamePair>>()
                    {
                        ErrorOccured = true,
                        ErrorMessage = "Invalid Address API Type."
                    };
            }
        }

        public RadiusAddress<IEnumerable<ValueNamePair>> GetProvinces()
        {
            switch (AddressAPISettings.AddressAPIType)
            {
                case (short)AddressAPIType.AddressQueryService:
                    {
                        var adapter = new AddressServiceAdapter();
                        return adapter.GetProvinces();
                    }
                case (short)AddressAPIType.DirectAccess:
                    {
                        var serviceClient = new AddressServiceClient(AddressAPISettings.AddressAPIDirectUserId, AddressAPISettings.AddressAPIDirectPassword);
                        var results = serviceClient.GetProvincesList();
                        if (results.InternalException != null)
                        {
                            return new RadiusAddress<IEnumerable<ValueNamePair>>()
                            {
                                ErrorOccured = true,
                                ErrorMessage = results.InternalException.GetShortMessage()
                            };
                        }
                        return new RadiusAddress<IEnumerable<ValueNamePair>>()
                        {
                            ErrorOccured = false,
                            Data = results.Data.Select(d => new ValueNamePair()
                            {
                                Code = d.Code,
                                Name = d.Name
                            }).ToArray()
                        };
                    }
                default:
                    return new RadiusAddress<IEnumerable<ValueNamePair>>()
                    {
                        ErrorOccured = true,
                        ErrorMessage = "Invalid Address API Type."
                    };
            }
        }

        public RadiusAddress<IEnumerable<ValueNamePair>> GetRuralRegionNeighbourhoods(long ruralRegionId)
        {
            switch (AddressAPISettings.AddressAPIType)
            {
                case (short)AddressAPIType.AddressQueryService:
                    {
                        var adapter = new AddressServiceAdapter();
                        return adapter.GetRuralRegionNeighbourhoods(ruralRegionId);
                    }
                case (short)AddressAPIType.DirectAccess:
                    {
                        var serviceClient = new AddressServiceClient(AddressAPISettings.AddressAPIDirectUserId, AddressAPISettings.AddressAPIDirectPassword);
                        var results = serviceClient.GetRegionDistrict(ruralRegionId);
                        if (results.InternalException != null)
                        {
                            return new RadiusAddress<IEnumerable<ValueNamePair>>()
                            {
                                ErrorOccured = true,
                                ErrorMessage = results.InternalException.GetShortMessage()
                            };
                        }
                        return new RadiusAddress<IEnumerable<ValueNamePair>>()
                        {
                            ErrorOccured = false,
                            Data = results.Data.Select(d => new ValueNamePair()
                            {
                                Code = d.Code,
                                Name = d.Name
                            }).ToArray()
                        };
                    }
                default:
                    return new RadiusAddress<IEnumerable<ValueNamePair>>()
                    {
                        ErrorOccured = true,
                        ErrorMessage = "Invalid Address API Type."
                    };
            }
        }

        public RadiusAddress<IEnumerable<ValueNamePair>> GetStreetBuildings(long streetId)
        {
            switch (AddressAPISettings.AddressAPIType)
            {
                case (short)AddressAPIType.AddressQueryService:
                    {
                        var adapter = new AddressServiceAdapter();
                        return adapter.GetStreetBuildings(streetId);
                    }
                case (short)AddressAPIType.DirectAccess:
                    {
                        var serviceClient = new AddressServiceClient(AddressAPISettings.AddressAPIDirectUserId, AddressAPISettings.AddressAPIDirectPassword);
                        var results = serviceClient.GetStreetBuildingsCodes(streetId);
                        if (results.InternalException != null)
                        {
                            return new RadiusAddress<IEnumerable<ValueNamePair>>()
                            {
                                ErrorOccured = true,
                                ErrorMessage = results.InternalException.GetShortMessage()
                            };
                        }
                        return new RadiusAddress<IEnumerable<ValueNamePair>>()
                        {
                            ErrorOccured = false,
                            Data = results.Data.Select(d => new ValueNamePair()
                            {
                                Code = d.Code,
                                Name = d.Name
                            }).ToArray()
                        };
                    }
                default:
                    return new RadiusAddress<IEnumerable<ValueNamePair>>()
                    {
                        ErrorOccured = true,
                        ErrorMessage = "Invalid Address API Type."
                    };
            }
        }
    }
}
