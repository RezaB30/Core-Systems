using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SavedFileViewModel
    {
        public string FileName { get; set; }

        [UIHint("ExactTime")]
        public DateTime CreationDate { get; set; }

        public string FilePath { get; set; }

        public string FileExtention { get; set; }
    }
}
