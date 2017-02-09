using XServices.Common.Authentication.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace XServices.Common.Authentication
{
    public class TokenFactory
    {
        private const string ConfigSectionName = "XServices.TokenClaims";
        private const string ConfigTokenIdName = "XServices.TokenId";
        private const string ConfigClaimsName = "XServices.Claims";
        private const string ConfigTokenKeyName = "XServices.TokenKey";

        private const string ConfigServerName = "XServices.TokenServer";

        public static JWTUser AccessToken
        {
            get
            {
                return ParseClientTokenClaimsConfigSettings();
            }
        }

        public static List<JWTUser> AllAccessTokens
        {
            get
            {
                try
                {
                    return ParseConfigClaimsSettings();
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

        private static List<JWTUser> ParseConfigClaimsSettings()
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
    }
}