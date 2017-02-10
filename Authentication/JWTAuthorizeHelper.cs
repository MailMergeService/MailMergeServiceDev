using JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace XServices.Common.Authentication
{
    //https://tools.ietf.org/html/draft-ietf-oauth-json-web-token-32
    public class JWTAuthorizeHelper
    {
        protected const char CustomTokenIdSeparator = ':';

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

        public string GenerateTokenString(string tokenKey, string tokenId, string claim, DateTime? expirationDateTime = null, string issuer = "https://admin.contactsamie.com", string audience = "https://api.contactsamie.com")
        {
            var payload = new Dictionary<string, object>() { { "aud", audience }, { "iss", issuer }, { tokenId, claim } };
            if (expirationDateTime != null)
            {
                //https://tools.ietf.org/html/draft-ietf-oauth-json-web-token-32#section-4.1.4
                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var now = Math.Round((expirationDateTime.Value - unixEpoch).TotalSeconds);
                payload.Add("exp", now);
            }

            return Base64Encode(tokenId + CustomTokenIdSeparator + JsonWebToken.Encode(payload, tokenKey, JWT.JwtHashAlgorithm.HS256));
        }

        // using knowledge of the separator
        public Tuple<string, string> ExtractTokenAndIdFromTokenString(string tokenStringBase64)
        {
            var tokenString = Base64Decode(tokenStringBase64);

            if (string.IsNullOrEmpty(tokenString) || !tokenString.Contains(CustomTokenIdSeparator) || tokenString.Split(CustomTokenIdSeparator).Count() != 2)
            {
                return null;
            }
            var tokenStringParts = tokenString.Split(CustomTokenIdSeparator);
            var tokenId = tokenStringParts[0];
            var token = tokenStringParts[1];
            return new Tuple<string, string>(tokenId, token);
        }

        public bool IsValidClaim(string tokenString, string requestedClaim, Func<string, JWTUser> getTokenKeyByTokenId, Action<Exception> onException = null)
        {
            if (getTokenKeyByTokenId == null) throw new ArgumentNullException(nameof(getTokenKeyByTokenId));

            var tokenData = ExtractTokenAndIdFromTokenString(tokenString);
            if (tokenData == null) return false;
            var tokenId = tokenData.Item1;
            var token = tokenData.Item2;
            var user = getTokenKeyByTokenId(tokenId);
            return IsValidClaim(tokenId, token, requestedClaim, user, onException);
        }

        public bool IsValidClaim(string tokenId, string token, string requestedClaim, JWTUser user, Action<Exception> onException = null)
        {
            if (string.IsNullOrEmpty(requestedClaim))
            {
                return true;
            }
            try
            {
                var tokenKey = user.TokenKey;

                var claims = JWT.JsonWebToken.DecodeToObject(token, tokenKey) as IDictionary<string, object>;
                if (claims == null)
                {
                    return false;
                }

                var requestTokenIdMatchesItsClaims = claims.ContainsKey(tokenId) && claims[tokenId] != null;
                var requestTokenClaimMatchesMethodToBeExecuted = (string)claims[tokenId] == requestedClaim;
                var requestTokenClaimExistsInThesystem = user.Claims.Contains(requestedClaim);

                var isValidClaim = requestTokenIdMatchesItsClaims && requestTokenClaimMatchesMethodToBeExecuted && requestTokenClaimExistsInThesystem;

                return isValidClaim;
            }
            catch (JWT.SignatureVerificationException e)
            {
                if (onException != null)
                {
                    onException(new Exception("SignatureVerificationException", e));
                }
                return false;
            }
            catch (Exception e)
            {
                if (onException != null)
                {
                    onException(new Exception("Exception Xured in jwt implementation", e));
                }
                return false;
            }
        }

        public bool AuthorizationDisabled(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() ||
                   actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        //todo for when requires username and password
        public static string GetTokenKeyWithPassword(string password, string salt)
        {
            const string alg = "HmacSHA256";

            var key = string.Join(":", new string[] { password, salt });

            using (var hmac = HMAC.Create(alg))
            {
                hmac.Key = Encoding.UTF8.GetBytes(salt);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(hmac.Hash);
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string str)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(str);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}