using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.FileManagement
{
    static class PathRepository
    {
        public static string[] ClientAttachments
        {
            get
            {
                return new string[]
                {
                    "MasterISS",
                    "Client Attachments"
                };
            }
        }

        public static string[] PDFForms
        {
            get
            {
                return new string[]
                {
                    "MasterISS",
                    "PDF Templates"
                };
            }
        }

        public static string ContractAppendixFileName
        {
            get
            {
                return "ContractAppendix.pdf";
            }
        }
    }
}
