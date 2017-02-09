using XServices.Common.Contracts.Services;
using XServices.Common.Factories;
using XServices.Common.Models;
using XServices.Common.ServiceConsumer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XServices.Common.Services
{
    public class WebApiEmailTemplateService : IEmailTemplateService
    {
        private WebApiEmailTemplateServiceEndPointsFactory EndPointsFactory { set; get; }

        private WebApiEmailTemplateServiceEndPoints EndPoints { set; get; }

        public WebApiEmailTemplateService()
        {
            EndPointsFactory = new WebApiEmailTemplateServiceEndPointsFactory();
            EndPoints = EndPointsFactory.GetEndPoints();
            ServiceCaller = new ExternalServiceCaller();
        }

        public async Task<ServiceStandardResponse<EmailTemplateServiceProcessResult>> SendEmailAsync(string templateName, List<EmailMergeModelData> emailMergeModelData, TemplateEmailSenderInformation request)
        {
            return  await ServiceCaller.PostAsync<EmailTemplateServiceProcessResult>(EndPoints.SendEmail, new HttpSendEmailCommand
            {
                EmailMergeModelData = emailMergeModelData,
                Request = request,
                TemplateName = templateName
            }).ContinueWith(x=>new ServiceStandardResponse<EmailTemplateServiceProcessResult>(x.Result));
        }

        public Task<ServiceStandardResponse<bool>> TemplateExists(string templateName)
        {
            return ServiceCaller.GetAsync<bool>(EndPoints.TemplateExists + getAppendtemplate(templateName)).ContinueWith(x => new ServiceStandardResponse<bool>(x.Result));
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

        public async Task<ServiceStandardResponse<bool>> UpdateTemplateAsync(string templateName, string templateBody)
        {
            return await ServiceCaller.PostAsync<bool>(EndPoints.UpdateTemplate, new HttpTemplateUpdateCommand()
            {
                TemplateBody = templateBody,
                TemplateName = templateName
            }).ContinueWith(x=>new ServiceStandardResponse<bool>(x.Result));
        }

        public async Task<ServiceStandardResponse<bool>> PublishTemplateAsync(string templateName)
        {
            return await ServiceCaller.PostAsync<bool>(EndPoints.PublishTemplate + getAppendtemplate(templateName), new { }).ContinueWith(x => new ServiceStandardResponse<bool>(true));
        }

        public async Task<ServiceStandardResponse<bool>> UnPublishTemplateAsync(string templateName)
        {
            return await ServiceCaller.PostAsync<bool>(EndPoints.UnPublishTemplate + getAppendtemplate(templateName), new { }).ContinueWith(x => new ServiceStandardResponse<bool>(true));
        }

        public async Task<ServiceStandardResponse<EmailTemplateInformation>> GetTemplateAsync(string templateName)
        {
            return await ServiceCaller.GetAsync<EmailTemplateInformation>(EndPoints.GetTemplate + getAppendtemplate(templateName)).ContinueWith(x=>new ServiceStandardResponse<EmailTemplateInformation>(x.Result));
        }

        public async Task<ServiceStandardResponse<List<EmailTemplateInformation>>> GetAllTemplatesAsync()
        {
            return await ServiceCaller.GetAsync<List<EmailTemplateInformation>>(EndPoints.GetAllTemplates).ContinueWith(x=>new ServiceStandardResponse<List<EmailTemplateInformation>>(x.Result));
        }

        private ExternalServiceCaller ServiceCaller { set; get; }

        private static string getAppendtemplate(string templateName)
        {
            Console.WriteLine(templateName + " ...");
            return templateName != null ? "/" + templateName : "";
        }
    }
}