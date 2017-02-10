using System.Collections.Generic;
using System.Threading.Tasks;
using XServices.Common;
using XServices.Common.Models;

namespace XServices.EmailTemplateWebApi
{
    internal class EmailTemplateService : IEmailTemplateService
    {
        public Task<ServiceStandardResponse<EmailTemplateServiceProcessResult>> SendEmailAsync(string templateName, List<EmailMergeModelData> emailMergeModelData, TemplateEmailSenderInformation request)
        {
            return Task.FromResult(new ServiceStandardResponse<EmailTemplateServiceProcessResult>());
        }

        public Task<ServiceStandardResponse<bool>> TemplateExists(string templateName)
        {
            return Task.FromResult(new ServiceStandardResponse<bool>());
        }

        public Task<ServiceStandardResponse<bool>> CreateTemplateAsync(string templateName)
        {
            return Task.FromResult(new ServiceStandardResponse<bool>());
        }

        public Task<ServiceStandardResponse<bool>> DeleteTemplateAsync(string templateName)
        {
            return Task.FromResult(new ServiceStandardResponse<bool>());
        }

        public Task<ServiceStandardResponse<bool>> UpdateTemplateAsync(string templateName, string templateBody)
        {
            return Task.FromResult(new ServiceStandardResponse<bool>());
        }

        public Task<ServiceStandardResponse<bool>> PublishTemplateAsync(string templateName)
        {
            return Task.FromResult(new ServiceStandardResponse<bool>());
        }

        public Task<ServiceStandardResponse<bool>> UnPublishTemplateAsync(string templateName)
        {
            return Task.FromResult(new ServiceStandardResponse<bool>());
        }

        public Task<ServiceStandardResponse<EmailTemplateInformation>> GetTemplateAsync(string templateName)
        {
            return Task.FromResult(new ServiceStandardResponse<EmailTemplateInformation>());
        }

        public Task<ServiceStandardResponse<List<EmailTemplateInformation>>> GetAllTemplatesAsync()
        {
            return Task.FromResult(new ServiceStandardResponse<List<EmailTemplateInformation>>());
        }
    }
}