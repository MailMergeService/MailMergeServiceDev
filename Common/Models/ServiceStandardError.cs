using System;
using System.Collections.Generic;
using System.Net;

namespace XServices.Common.Models
{
    [Serializable]
    public class ServiceStandardError
    {
        public string Url { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }
        public string HttpReasonPhrase { get; set; }

        public Exception Exception { get; set; }
        public List<string> Errors { get; private set; }

        public ServiceStandardError()
        {
            HttpStatusCode = HttpStatusCode.OK;
            Errors = new List<string>();
        }

        public ServiceStandardError(string error)
        {
            HttpStatusCode = HttpStatusCode.OK;
            Errors = new List<string> { error };
        }

        public void Error(string error)
        {
            Errors.Add(error);
        }

        public void Error(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
        }
    }
}