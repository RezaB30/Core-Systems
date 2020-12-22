using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Files
{
    public class RadiusRFolders
    {
        public const string ClientAttachments = @"Manager\Client Attachments\";

        public const string PDFFormTemplates = @"Manager\PDF Templates\";

        public const string ContractAppendixFileName = @"Contract.pdf";

        public const string BTKTempLogFiles = @"BTK Logs\Temp\";

        public const string BTKTempLogCatalog = BTKTempLogFiles + @"Catalog\";

        public const string BTKTempLogChanges = BTKTempLogFiles + @"Changes\";

        public const string BTKTempLogIPDR = BTKTempLogFiles + @"IPDR\";

        public const string BTKTempLogIPBlock = BTKTempLogFiles + @"IPBlock\";

        public const string BTKTempLogSessions = BTKTempLogFiles + @"Sessions\";

        public const string BTKTempLogClientOld = BTKTempLogFiles + @"Client Old\";

        public const string MailFiles = @"Manager\Mail Files\";

        public const string MailContarctFiles = MailFiles + @"Contract\";

        public const string MailContractFileName = @"ContractMailBody";
    }

}
