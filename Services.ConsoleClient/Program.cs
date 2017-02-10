using System;
using System.Collections.Generic;
using System.Linq;
using XServices.Common;
using XServices.Common.Models;

namespace Services.ConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var templateService = new RESTEmailTemplateService();
            var _templateName = Guid.NewGuid().ToString();
            var requestData = GetRequestMergeData();
            var t1 = templateService.CreateTemplateAsync(_templateName).Result;
            var t2 = templateService.UpdateTemplateAsync(_templateName, Template).Result;
            var t3 = templateService.PublishTemplateAsync(_templateName).Result;
            var finalResult = templateService.SendEmailAsync(_templateName, requestData, senderParams);

            var templates = templateService.GetAllTemplatesAsync().Result;
            var firstTemplate = templates.Response?.First(x => x.Name == _templateName);
            var loadedTemplate = templateService.GetTemplateAsync(_templateName);

            //Assert.IsTrue(finalResult.Response.IsSuccessfull);
            //Assert.AreEqual(Template, loadedTemplate.Response.Content);
            //Assert.AreEqual(1, templates.Response.Where(x => x.Name == _templateName).ToList().Count);
            //Assert.AreEqual(_templateName, firstTemplate.Name);
            //Assert.AreEqual(Template, firstTemplate.Content);
            Console.ReadLine();
        }

        public static List<EmailMergeModelData> GetRequestMergeData()
        {
            var emails = new List<dynamic>()
            {
                new
                {
                    Codes = new List<dynamic>()
                    {
                        new{Code = "DSUSDBVRF777"},
                        new{Code = "FDHFDHJHGY65HRTHRTH"}
                    },
                    Email = "sam.bamgboye.oc@gmail.com",
                    FirstName = "2dfjdsf samuel"
                }
            };

            var requestData = emails.Select(x => new EmailMergeModelData() { Email = x.Email, ToName = x.FirstName, Model = x }).ToList();

            return requestData;
        }

        private static TemplateEmailSenderInformation senderParams = new TemplateEmailSenderInformation()
        {
            FromEmail = "sam.bamgboye.oc@gmail.com",
            FromName = "Webmaster",
            Subject = "Test Subject",
        };

        private const string Template = "<span>Dear {{FirstName}}, here are your codes {{#each Codes}}<br/> {{Code}} {{/each}} <br/> Thank you.<br/> {{#each Model}}<br/> {{Code}} {{/each}}  </span>";

      
    }
}