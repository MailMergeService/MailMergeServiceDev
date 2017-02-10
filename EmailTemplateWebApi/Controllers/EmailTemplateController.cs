using NFluent;

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using XServices.Common;
using XServices.Common.Authentication;
using XServices.Common.Models;

namespace XServices.EmailTemplateWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [ServiceWebApiAuthorize(Roles = "Administrator")]
    public class EmailTemplateController : ApiController
    {
        private IEmailTemplateService Service { set; get; }

        public EmailTemplateController(IEmailTemplateService service)
        {
            Check.That(service).IsNotNull();
            Service = service;
        }

        [HttpPut]
        public async Task<HttpResponseMessage> CreateTemplate(string id)
        {
            var data = await Service.CreateTemplateAsync(id).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteTemplate(string id)
        {
            var data = await Service.DeleteTemplateAsync(id).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK, data.Response);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UpdateTemplate(HttpTemplateUpdateCommand command)
        {
            Check.That(command).IsNotNull();
            //Check.That(command.TemplateName).IsNotNullOrWhitespace();

            var data = await Service.UpdateTemplateAsync(command.TemplateName, command.TemplateBody).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK, data.Response);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PublishTemplate(string id)
        {
            var data = await Service.PublishTemplateAsync(id).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK, data.Response);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetTemplate(string id)
        {
            var data = await Service.GetTemplateAsync(id).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK, data.Response);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetAllTemplates()
        {
            var data = await Service.GetAllTemplatesAsync().ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK, data.Response);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> TemplateExists(string id)
        {
            var data = await Service.TemplateExists(id).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK, data.Response);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SendEMail(HttpSendEmailCommand command)
        {
            Check.That(command).IsNotNull();
            //Check.That(command.TemplateName).IsNotNullOrWhitespace();
            Check.That(command.EmailMergeModelData).IsNotNull();
            Check.That(command.Request).IsNotNull();
            //Check.That(command.Request.FromEmail).IsNotNullOrWhitespace();
            //Check.That(command.Request.Subject).IsNotNullOrWhitespace();
            Check.That(command.Request.PreserveRecipients).IsFalse();

            var data = await Service.SendEmailAsync(command.TemplateName, command.EmailMergeModelData, command.Request).ConfigureAwait(false);

            return Request.CreateResponse(HttpStatusCode.OK, data.Response);
        }
    }
}