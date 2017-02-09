using System.Collections.Generic;

namespace XServices.Common.Authentication.Attributes
{
    public class JWTUser
    {
        public JWTUser()
        {
            Claims = new List<string>();
        }

        public string TokenId { set; get; }

        public string TokenKey { set; get; }

        public List<string> Claims { set; get; }
    }
}