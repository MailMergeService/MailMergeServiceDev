using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace XServices.Common.Authentication
{
    // todo convert methods to async
    public class ExternalServiceCaller
    {
        protected readonly string BaseAddress;

        public ExternalServiceCaller(string baseAddress = null)
        {
            baseAddress = baseAddress ?? TokenFactory.GetBaseAddressFromConfig();
            if (baseAddress == null) throw new ArgumentNullException("baseAddress");
            BaseAddress = baseAddress;
        }

        public Task<T> GetAsync<T>(string service)
        {
            Console.WriteLine(BaseAddress + service);
            return SendRequestAndGetResponse<T>((httpClient) => httpClient.GetAsync(CreateEndPoint(service)));
        }

        public Task<T> PostAsync<T>(string service, object data)
        {
            Console.WriteLine(BaseAddress + service);
            return SendRequestAndGetResponse<T>((httpClient) => httpClient.PostAsJsonAsync(CreateEndPoint(service), data));
        }

        public Task PutAsync(string service, object data)
        {
            Console.WriteLine(BaseAddress + service);
            return SendRequestAndGetResponse<dynamic>((httpClient) => httpClient.PutAsJsonAsync(CreateEndPoint(service), data));
        }

        public Task DeleteAsync(string service)
        {
            Console.WriteLine(BaseAddress + service);
            return SendRequestAndGetResponse<dynamic>((httpClient) => httpClient.DeleteAsync(CreateEndPoint(service)));
        }

        protected async Task<T> SendRequestAndGetResponse<T>(Func<HttpClient, Task<HttpResponseMessage>> requestMethod)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");

                SetAuthorizationHeader(httpClient);

                var response = await requestMethod(httpClient);
                return await response.Content.ReadAsStringAsync().ContinueWith(r =>
                {
                    var data = r.Result;
                    Console.WriteLine(data);
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = new List<JsonConverter> { new StringEnumConverter() },
                    };
                    settings.Error += delegate (object sender, ErrorEventArgs args)
                    {
                        Console.WriteLine(args.ErrorContext);
                        args.ErrorContext.Handled = true;
                        Console.WriteLine(args);
                    };
                    return JsonConvert.DeserializeObject<T>(data, settings);
                });
            }
        }

        public void HandleDeSerializationError(object sender, ErrorEventArgs errorArgs)
        {
            //  var currentError = errorArgs.ErrorContext.Error.Message;
            // errorArgs.ErrorContext.Handled = true;
        }

        private static void SetAuthorizationHeader(HttpClient httpClient)
        {
            var XjwtAuthorizeHelper = new JWTAuthorizeHelper();
            var user = TokenFactory.AccessToken;
            var token = XjwtAuthorizeHelper.GenerateTokenString(user.TokenKey, user.TokenId, user.Claims.FirstOrDefault() /*, DateTime.UtcNow.AddDays(1)*/);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
        }

        private string CreateEndPoint(string service)
        {
            return BaseAddress.TrimEnd('\\').TrimEnd('/') + "/" + service.TrimStart('\\').TrimStart('/');
        }
    }
}