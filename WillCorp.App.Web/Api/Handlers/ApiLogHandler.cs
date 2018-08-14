using WillCorp.Logging;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace WillCorp.App.Web.Api.Handlers
{
    public class ApiLogHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public ApiLogHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apiLogEntry = CreateApiLogEntryWithRequestData(request);
            var sw = new Stopwatch();

            if (request.Content != null)
            {
                await request.Content.ReadAsStringAsync()
                    .ContinueWith(task =>
                    {
                        apiLogEntry.RequestContentBody = task.Result;
                    }, cancellationToken);
            }

            sw.Start();
            return await base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var response = task.Result;

                    sw.Stop();
                    var formattedTimeElapsed = sw.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture);
                    // Update the API log entry with response info
                    var statusCode = (int)response.StatusCode;
                    apiLogEntry.ResponseStatusCode = statusCode;
                    apiLogEntry.ResponseTimestamp = DateTime.Now;

                    if (response.Content != null)
                    {
                        apiLogEntry.ResponseContentBody = response.Content.ReadAsStringAsync().Result;
                        apiLogEntry.ResponseContentType = response.Content.Headers.ContentType.MediaType;
                        apiLogEntry.ResponseHeaders = SerializeHeaders(response.Content.Headers);
                    }

                    using (_logger.AddContext("CorrelationId", request.GetCorrelationId()))
                    {
                        /* * Logging levels
                        All 400-499 http status codes are logged as Warnings
                        All 500 > http status codes are logged as Errors
                        All other requests are logged as Info
                        * */

                        if (statusCode >= 400 && statusCode <= 499)
                        {
                            _logger.Warn("HTTP {HttpMethod} {AbsoluteUrl} responded {StatusCode} in {EllapseTime} ms", apiLogEntry.RequestMethod.ToUpper(),
                                apiLogEntry.RequestUri, statusCode, formattedTimeElapsed);
                        }
                        else if (statusCode >= 500)
                            _logger.Error("HTTP {HttpMethod} {AbsoluteUrl} responded {StatusCode} in {EllapseTime} ms", apiLogEntry.RequestMethod.ToUpper(),
                                apiLogEntry.RequestUri, statusCode, formattedTimeElapsed);
                        else
                            _logger.Information("HTTP {HttpMethod} {AbsoluteUrl} responded {StatusCode} in {EllapseTime} ms", apiLogEntry.RequestMethod.ToUpper(),
                                apiLogEntry.RequestUri, statusCode, formattedTimeElapsed);
                    }

                    return response;
                }, cancellationToken);
        }

        private ApiLogEntry CreateApiLogEntryWithRequestData(HttpRequestMessage request)
        {
            var context = ((OwinContext)request.Properties["MS_OwinContext"]);
            var routeData = request.GetRouteData();

            return new ApiLogEntry
            {
                ApiLogEntryId = Guid.NewGuid().ToString(),
                RequestContentType = context?.Request?.ContentType,
                RequestRouteTemplate = routeData?.Route?.RouteTemplate,
                RequestRouteData = SerializeRouteData(routeData),
                RequestIpAddress = context?.Request?.RemoteIpAddress,
                RequestMethod = request.Method.Method,
                RequestHeaders = SerializeHeaders(request.Headers),
                RequestTimestamp = DateTime.Now,
                RequestUri = request?.RequestUri?.ToString()
            };
        }

        private string SerializeRouteData(IHttpRouteData routeData)
        {
            return JsonConvert.SerializeObject(routeData, Formatting.Indented);
        }

        private string SerializeHeaders(HttpHeaders headers)
        {
            var dict = new Dictionary<string, string>();

            foreach (var item in headers.ToList())
            {
                if (item.Value != null)
                {
                    var header = String.Empty;
                    foreach (var value in item.Value)
                    {
                        header += value + " ";
                    }

                    // Trim the trailing space and add item to the dictionary
                    header = header.TrimEnd(" ".ToCharArray());
                    dict.Add(item.Key, header);
                }
            }

            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }
    }

    internal sealed class ApiLogEntry
    {
        public string ApiLogEntryId { get; set; }           // The (database) ID for the API log entry.
        public string RequestId { get; set; }               // The request id.
        public string RequestIpAddress { get; set; }        // The IP address that made the request.
        public string RequestContentType { get; set; }      // The request content type.
        public string RequestContentBody { get; set; }      // The request content body.
        public string RequestUri { get; set; }              // The request URI.
        public string RequestMethod { get; set; }           // The request method (GET, POST, etc).
        public string RequestRouteTemplate { get; set; }    // The request route template.
        public string RequestRouteData { get; set; }        // The request route data.
        public string RequestHeaders { get; set; }          // The request headers.
        public DateTime? RequestTimestamp { get; set; }     // The request timestamp.
        public string ResponseContentType { get; set; }     // The response content type.
        public string ResponseContentBody { get; set; }     // The response content body.
        public int? ResponseStatusCode { get; set; }        // The response status code.
        public string ResponseHeaders { get; set; }         // The response headers.
        public DateTime? ResponseTimestamp { get; set; }    // The response timestamp.
    }
}
