using System.Collections.Generic;
using System.Threading.Tasks;
using XServices.Common.Models;

namespace XServices.Common
{
    public interface IEmailTemplateService
    {
        Task<ServiceStandardResponse<EmailTemplateServiceProcessResult>> SendEmailAsync(string templateName, List<EmailMergeModelData> emailMergeModelData, TemplateEmailSenderInformation request);

        Task<ServiceStandardResponse<bool>> TemplateExists(string templateName);

        Task<ServiceStandardResponse<bool>> CreateTemplateAsync(string templateName);

        Task<ServiceStandardResponse<bool>> DeleteTemplateAsync(string templateName);

        Task<ServiceStandardResponse<bool>> UpdateTemplateAsync(string templateName, string templateBody);

        Task<ServiceStandardResponse<bool>> PublishTemplateAsync(string templateName);

        Task<ServiceStandardResponse<bool>> UnPublishTemplateAsync(string templateName);

        Task<ServiceStandardResponse<EmailTemplateInformation>> GetTemplateAsync(string templateName);

        Task<ServiceStandardResponse<List<EmailTemplateInformation>>> GetAllTemplatesAsync();
    }
}