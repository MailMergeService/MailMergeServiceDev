using System.Collections.Generic;

namespace XServices.Common.Models
{
    public class HttpSendEmailCommand
    {
        public string TemplateName { set; get; }

        public List<EmailMergeModelData> EmailMergeModelData { set; get; }

        public TemplateEmailSenderInformation Request { set; get; }
    }
}