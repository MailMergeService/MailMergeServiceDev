using System.Collections.Generic;
using System.Threading.Tasks;
using XServices.Common.Contracts.Services;
using XServices.Common.Models;

namespace XServices.EmailTemplateWebApi
{
    internal class EmailTemplateService: IEmailTemplateService
    {
        public Task<ServiceStandardResponse<EmailTemplateServiceProcessResult>> SendEmailAsync(string templateName, List<EmailMergeModelData> emailMergeModelData, TemplateEmailSenderInformation request)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceStandardResponse<bool>> TemplateExists(string templateName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceStandardResponse<bool>> CreateTemplateAsync(string templateName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceStandardResponse<bool>> DeleteTemplateAsync(string templateName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceStandardResponse<bool>> UpdateTemplateAsync(string templateName, string templateBody)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceStandardResponse<bool>> PublishTemplateAsync(string templateName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceStandardResponse<bool>> UnPublishTemplateAsync(string templateName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceStandardResponse<EmailTemplateInformation>> GetTemplateAsync(string templateName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceStandardResponse<List<EmailTemplateInformation>>> GetAllTemplatesAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}