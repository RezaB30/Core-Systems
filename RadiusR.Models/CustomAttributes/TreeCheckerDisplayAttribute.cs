using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RadiusR_Manager.Models.CustomAttributes
{
    public class TreeCheckerDisplayAttribute : Attribute, IMetadataAware
    {
        private readonly Type _resourceType;

        public TreeCheckerDisplayAttribute(Type resourceType)
        {
            _resourceType = resourceType;
        }
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues["EnumResourceType"] = _resourceType;
        }

        public Type ResourceType
        {
            get
            {
                return _resourceType;
            }
        }
    }
}
