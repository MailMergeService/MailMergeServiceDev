namespace XServices.Common.Models
{
    public class WebApiEmailTemplateServiceEndPoints
    {
        public string SendEmail { set; get; }

        public string TemplateExists { set; get; }

        public string CreateTemplate { set; get; }

        public string DeleteTemplate { set; get; }

        public string UpdateTemplate { set; get; }

        public string PublishTemplate { set; get; }

        public string UnPublishTemplate { set; get; }

        public string GetTemplate { set; get; }

        public string GetAllTemplates { set; get; }

    }
}