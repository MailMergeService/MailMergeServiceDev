using System;

namespace XServices.Common.Models
{
    public class EmailTemplateInformation
    {
        public string Name { set; get; }

        public string Content { set; get; }

        public DateTime? PublishedDateTime { set; get; }
    }
}