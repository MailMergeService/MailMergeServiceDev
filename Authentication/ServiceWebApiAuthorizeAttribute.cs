using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace XServices.Common.Authentication
{
    public class ServiceWebApiAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly JWTAuthorizeHelper _occjwtAuthorizeHelper;

        public ServiceWebApiAuthorizeAttribute()
        {
            _occjwtAuthorizeHelper = new JWTAuthorizeHelper();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //  throw new Exception();
            //https://en.wikipedia.org/wiki/List_of_HTTP_header_fields#Request_fields
            if (actionContext.Request.Headers.GetValues("Authorization") != null)
            {
                var tokenString = Convert.ToString(actionContext.Request.Headers.GetValues("Authorization").FirstOrDefault());

                if (_occjwtAuthorizeHelper.IsValidClaim(tokenString, Roles, TokenFactory.GetUserDetailsByTokenId))
                {
                    return;
                }
            }
            HandleUnauthorizedRequest(actionContext);
        }

        protected new void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            _occjwtAuthorizeHelper.HandleUnauthorizedRequest(actionContext);
        }
    }
}