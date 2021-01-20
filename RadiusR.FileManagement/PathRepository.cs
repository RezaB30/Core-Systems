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

        public static string[] MailFiles
        {
            get
            {
                return new string[]
                {
                    "MasterISS",
                    "Mail Files"
                };
            }
        }

        public static string[] MailContractFiles
        {
            get
            {
                return new string[]
                {
                    "Contract"
                };
            }
        }

        public static string MailContractBodyFile
        {
            get
            {
                return "ContractMailBody";
            }
        }

        public static string[] BTKLogs
        {
            get
            {
                return new string[]
                {
                    "BTK Logs"
                };
            }
        }

        public static string[] SupportRequestAttachments
        {
            get
            {
                return new string[]
                {
                    "MasterISS",
                    "Support Request Attachments"
                };
            }
        }
    }
}
