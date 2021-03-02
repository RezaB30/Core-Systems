using RadiusR.DB.DomainsCache;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class TransitionOperatorViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Username")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string Username { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string DisplayName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RemoteFolders")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(300, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string _remoteFolder { get; private set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RemoteFolders")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public IEnumerable<string> RemoteFolders
        {
            get
            {
                return string.IsNullOrWhiteSpace(_remoteFolder) ? Enumerable.Empty<string>() : _remoteFolder.Split('\t');
            }
            set
            {
                _remoteFolder = string.Join("\t", value);
            }
        }

        public TransitionOperatorViewModel(CachedTransitionOperator cachedOperator)
        {
            RemoteFolders = cachedOperator.RemoteFolders;
            ID = cachedOperator.ID;
            Username = cachedOperator.Username;
            DisplayName = cachedOperator.DisplayName;
        }

        public TransitionOperatorViewModel() { }
    }
}
