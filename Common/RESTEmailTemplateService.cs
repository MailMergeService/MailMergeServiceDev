using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XServices.Common.Authentication;
using XServices.Common.Models;

namespace XServices.Common
{
    public class RESTEmailTemplateService : IEmailTemplateService
    {
        private RESTEmailTemplateServiceEndPointsFactory EndPointsFactory { set; get; }

        private WebApiEmailTemplateServiceEndPoints EndPoints { set; get; }

        public RESTEmailTemplateService()
        {
            EndPointsFactory = new RESTEmailTemplateServiceEndPointsFactory();
            EndPoints = EndPointsFactory.GetEndPoints();
            ServiceCaller = new ExternalServiceCaller();
        }

        public Task<ServiceStandardResponse<EmailTemplateServiceProcessResult>> SendEmailAsync(string templateName, List<EmailMergeModelData> emailMergeModelData, TemplateEmailSenderInformation request)
        {
            return ServiceCaller.PostAsync<EmailTemplateServiceProcessResult>(EndPoints.SendEmail, new HttpSendEmailCommand
            {
                EmailMergeModelData = emailMergeModelData,
                Request = request,
                TemplateName = templateName
            }).ContinueWith(x => new ServiceStandardResponse<EmailTemplateServiceProcessResult>(x.Result));
        }

        public async Task<ServiceStandardResponse<bool>> TemplateExists(string templateName)
        {
            var result = await ServiceCaller.GetAsync<bool>(EndPoints.TemplateExists + getAppendtemplate(templateName));
            return new ServiceStandardResponse<bool>(result);
        }

        public async Task<ServiceStandardResponse<bool>> CreateTemplateAsync(string templateName)
        {
            await ServiceCaller.PutAsync(EndPoints.CreateTemplate + getAppendtemplate(templateName), new { });
            return new ServiceStandardResponse<bool>(true);
        }

        public async Task<ServiceStandardResponse<bool>> DeleteTemplateAsync(string templateName)
        {
            await ServiceCaller.DeleteAsync(EndPoints.DeleteTemplate + getAppendtemplate(templateName));
            return new ServiceStandardResponse<bool>(true);
        }

        public Task<ServiceStandardResponse<bool>> UpdateTemplateAsync(string templateName, string templateBody)
        {
            return ServiceCaller.PostAsync<bool>(EndPoints.UpdateTemplate, new HttpTemplateUpdateCommand()
            {
                TemplateBody = templateBody,
                TemplateName = templateName
            }).ContinueWith(x => new ServiceStandardResponse<bool>(x.Result));
        }

        public Task<ServiceStandardResponse<bool>> PublishTemplateAsync(string templateName)
        {
            return ServiceCaller.PostAsync<bool>(EndPoints.PublishTemplate + getAppendtemplate(templateName), new { }).ContinueWith(x => new ServiceStandardResponse<bool>(true));
        }

        public Task<ServiceStandardResponse<bool>> UnPublishTemplateAsync(string templateName)
        {
            return ServiceCaller.PostAsync<bool>(EndPoints.UnPublishTemplate + getAppendtemplate(templateName), new { }).ContinueWith(x => new ServiceStandardResponse<bool>(true));
        }

        public Task<ServiceStandardResponse<EmailTemplateInformation>> GetTemplateAsync(string templateName)
        {
            return ServiceCaller.GetAsync<EmailTemplateInformation>(EndPoints.GetTemplate + getAppendtemplate(templateName)).ContinueWith(x => new ServiceStandardResponse<EmailTemplateInformation>(x.Result));
        }

        public Task<ServiceStandardResponse<List<EmailTemplateInformation>>> GetAllTemplatesAsync()
        {
            return ServiceCaller.GetAsync<List<EmailTemplateInformation>>(EndPoints.GetAllTemplates).ContinueWith(x => new ServiceStandardResponse<List<EmailTemplateInformation>>(x.Result));
        }

        private ExternalServiceCaller ServiceCaller { set; get; }

        private static string getAppendtemplate(string templateName)
        {
            Console.WriteLine(templateName + " ...");
            return templateName != null ? "/" + templateName : "";
        }
    }
}