using System;
using System.Collections.Generic;
using System.Linq;
using XServices.Common;
using XServices.Common.Authentication;
using XServices.Common.Models;

namespace Services.ConsoleClient
{
    //http://www.primaryobjects.com/2015/05/08/token-based-authentication-for-web-service-apis-in-c-mvc-net/
    /*
     A Note on Token Strength .. and Weakness

It’s important to keep in mind the strength and weakness of a token-based system. The most important rule is - never transmit the token key across the wire.
That is, you select a key that will be used to generate a token for each request. This key must never be sent to the server and must always remain on the client
(ie., held in javascript memory). With that in mind, here are some descriptions of potential weaknesses for token-based authentication.

There are several vectors of attack that need to be considered. The first, is a simple replay attack or man-in-the-middle attack. This involves an attacker
capturing a token API request and replaying the same exact request again. We can prevent this type of attack by validating client-specific data as part of the token
(IP address and user-agent string). In addition, adding a token expiration date helps to limit the duration that such an attack is viable. You could further prevent
this type of attack by keeping a server log (MemoryCache, etc) of recently used tokens and invalidate them once used. Depending on how short the token expiration time
is (5-10 minutes), invalidation may not be necessary.

Of course, SSL is another way to help protect tokens. SSL encrypts url query string parameters and post data. Since the token is included as a parameter within the url,
SSL can be an effective tool to boost token authentication security.

A second type of attack is physically obtaining the token key. Since the javascript that generates a token is publicly available, an attacker could attempt to generate his
own token, impersonating another user. To do this, he would need to provide another user’s username as part of the token body. However, this requires using the user’s key to hash
the token message. To generate the user’s key, an attacker would need either the user’s password or their actual key. We’ll have to assume that an attacker would not have access to
the user’s password (otherwise, the user could change his password to invalidate the token key). Since we never transmit the token key to the server (the token key never goes over the wire),
it’s possible instead that the user’s PC could be physically compromised or stolen. An attacker could access the localStorage, obtain the user’s key, and generate his own token on behalf of the user.
Really, this is no different than auto-sign-in, which many web applications already do for the physical computer. We have to consider this case a more remote possibility and leave it to other security
mechanisms to protect the physical PC.

         */

    internal class Program
    {
        private static void Main(string[] args)
        {
            var password = "password";
            var salt = "rz8LuOtFBXphj9WQfvFh"; // Generated at https://www.random.org/strings
            var tokenKey = TokenFactory.CreateTokenKeyWithPassword(password, salt);
            /*
             on client ui
             -salt is stored on clear text
             -password is entered and token key is created from that using the salt and the resulting key is stored on client
             -username is entered and that is the token id and its stored
             -claims is an extra thing***no need to discuss
             -when request is to be made TOKENID AND TOKENKEY IS READY. CLAIMS AND SERVER ARE EASY STUFF
             */

            var templateService = new RESTEmailTemplateService();
            var _templateName = Guid.NewGuid().ToString();
            var requestData = GetRequestMergeData();
            var t1 = templateService.CreateTemplateAsync(_templateName).Result;
            var t2 = templateService.UpdateTemplateAsync(_templateName, Template).Result;
            var t3 = templateService.PublishTemplateAsync(_templateName).Result;
            var finalResult = templateService.SendEmailAsync(_templateName, requestData, _senderParams);

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

        private static readonly TemplateEmailSenderInformation _senderParams = new TemplateEmailSenderInformation()
        {
            FromEmail = "sam.bamgboye.oc@gmail.com",
            FromName = "Webmaster",
            Subject = "Test Subject",
        };

        private const string Template = "<span>Dear {{FirstName}}, here are your codes {{#each Codes}}<br/> {{Code}} {{/each}} <br/> Thank you.<br/> {{#each Model}}<br/> {{Code}} {{/each}}  </span>";
    }
}