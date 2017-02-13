using JWT;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace XServices.Common.Authentication
{
    public class TokenFactory
    {
        protected const char CustomTokenIdSeparator = ':';
        private const string ConfigSectionName = "XServices.TokenClaims";
        private const string ConfigTokenIdName = "XServices.TokenId";
        private const string ConfigClaimsName = "XServices.Claims";
        private const string ConfigTokenKeyName = "XServices.TokenKey";

        private const string ConfigServerName = "XServices.TokenServer";

        public static JWTUser AccessToken => ParseClientTokenClaimsConfigSettings();

        public static List<JWTUser> AllAccessTokens
        {
            get
            {
                try
                {
                    return ParseServerConfigClaimsSettings();
                }
                catch (Exception e)
                {
                    throw new Exception("An error Xured while trying to read Section '" + ConfigSectionName + "' , required to setup access tokens , from config file", e);
                }
            }
        }

        public static JWTUser GetUserDetailsByTokenId(string tokenId)
        {
            return AllAccessTokens.Find(x => x.TokenId == tokenId);
        }

        public static string GetBaseAddressFromConfig()
        {
            var server = System.Configuration.ConfigurationManager.AppSettings[ConfigServerName];
            if (string.IsNullOrEmpty(server))
                throw new Exception("'" + ConfigServerName + "' entry not found in config file");

            return server.TrimEnd('/') + "/";
        }

        #region Parsing webconfig sections

        private static JWTUser ParseClientTokenClaimsConfigSettings()
        {
            var tokenId = ConfigurationManager.AppSettings[ConfigTokenIdName];
            var claims = ConfigurationManager.AppSettings[ConfigClaimsName];
            var tokenKey = ConfigurationManager.AppSettings[ConfigTokenKeyName];
            return new JWTUser()
            {
                TokenId = tokenId,
                // todo or GetTokenKeyWithPassword("password","somesalt")
                TokenKey = tokenKey,
                Claims = (string.IsNullOrEmpty(claims) ? "" : claims).Split(',').ToList()
            };
        }

        private static List<JWTUser> ParseServerConfigClaimsSettings()
        {
            var sectionList = (NameValueCollection)ConfigurationManager.GetSection(ConfigSectionName);
            return (from object section in sectionList
                    select section.ToString()
                        into settingsName
                    select sectionList[settingsName]
                            into settingsValue
                    select settingsValue.Split(';').Select(t => t.Split(new char[] { '=' }, 2)).ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase)
                                into connStringParts
                    select new JWTUser()
                    {
                        TokenId = connStringParts["TokenId"],
                        TokenKey = connStringParts["TokenKey"],
                        Claims = connStringParts["Claims"].ToString().Split(',').ToList(),
                    }).ToList();
        }

        #endregion Parsing webconfig sections

        public static string GenerateTokenString(string tokenKey, string tokenId, string claim, DateTime? expirationDateTime = null, string issuer = "https://admin.contactsamie.com", string audience = "https://api.contactsamie.com")
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
        public static Tuple<string, string> ExtractTokenAndIdFromTokenString(string tokenStringBase64)
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

        public static bool IsValidClaim(string tokenString, string requestedClaim, Func<string, JWTUser> getTokenKeyByTokenId, Action<Exception> onException = null)
        {
            if (getTokenKeyByTokenId == null) throw new ArgumentNullException(nameof(getTokenKeyByTokenId));

            var tokenData = ExtractTokenAndIdFromTokenString(tokenString);
            if (tokenData == null) return false;
            var tokenId = tokenData.Item1;
            var token = tokenData.Item2;
            var user = getTokenKeyByTokenId(tokenId);
            return IsValidClaim(tokenId, token, requestedClaim, user, onException);
        }

        public static bool IsValidClaim(string tokenId, string token, string requestedClaim, JWTUser user, Action<Exception> onException = null)
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

        //todo for when requires username and password
        /// <summary>
        /// its just a hashed password for use as token key
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string CreateTokenKeyWithPassword(string password, string salt)
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