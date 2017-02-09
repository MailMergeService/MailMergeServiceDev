using System;
using System.Linq;
using System.Net;

namespace XServices.Common.Models
{
    [Serializable]
    public class ServiceStandardResponse<T>
    {
        public ServiceStandardResponse(T response,HttpStatusCode statusCode= HttpStatusCode.OK)
        {
            Response = response;
            HttpStatusCode = statusCode;
        }
        public ServiceStandardResponse()
        {

        }

        public string Url { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string HttpReasonPhrase { get; set; }

        public ServiceStandardError StandardError { get; set; }

        public T Response { get; set; }

        public bool HasError
        {
            get
            {
                return (StandardError != null
                        && (StandardError.Exception != null
                            || (StandardError.Errors != null && StandardError.Errors.Any())
                            || StandardError.HttpStatusCode != HttpStatusCode.OK));
            }
        }
    }
}