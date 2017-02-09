using XServices.Common.Models;
using System;

namespace XServices.Common.Factories
{
    public class WebApiEmailTemplateServiceEndPointsFactory
    {
       

        public WebApiEmailTemplateServiceEndPoints GetEndPoints()
        {
            return new WebApiEmailTemplateServiceEndPoints()
            {
                SendEmail = "api/EmailTemplate/SendEMail",
                TemplateExists = "api/EmailTemplate/TemplateExists",
                CreateTemplate = "api/EmailTemplate/CreateTemplate",
                DeleteTemplate = "api/EmailTemplate/DeleteTemplate",
                UpdateTemplate = "api/EmailTemplate/UpdateTemplate",
                PublishTemplate = "api/EmailTemplate/PublishTemplate",
                UnPublishTemplate = "api/EmailTemplate/UnPublishTemplate",
                GetTemplate = "api/EmailTemplate/GetTemplate",
                GetAllTemplates = "api/EmailTemplate/GetAllTemplates",
            };
        }
    }
}