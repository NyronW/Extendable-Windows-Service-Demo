using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WillCorp.App.Web.Api.ActionResults
{
    internal class HttpActionResult<T> : IHttpActionResult
    {
        private readonly T _result;
        private readonly HttpStatusCode _statusCode;

        public HttpActionResult(HttpStatusCode statusCode, T result)
        {
            _statusCode = statusCode;
            _result = result;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent
                (
                    JsonConvert.SerializeObject(_result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                    Encoding.UTF8,
                    "application/json"
                )
            };
            return Task.FromResult(response);
        }
    }
}
