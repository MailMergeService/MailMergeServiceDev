using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace XServices.Common.Authentication
{
    //https://tools.ietf.org/html/draft-ietf-oauth-json-web-token-32
    public class JWTAuthorizeHelper
    {
        public void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            if (AuthorizationDisabled(actionContext))
                return;

            actionContext.Response = CreateUnauthorizedResponse(actionContext.ControllerContext.Request);
        }

        public HttpResponseMessage CreateUnauthorizedResponse(HttpRequestMessage request)
        {
            var result = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                RequestMessage = request
            };

            return result;
        }

        public bool AuthorizationDisabled(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() ||
                   actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}