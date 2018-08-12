using WillCorp.App.Web.Middleware;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using System.Web.Http.ExceptionHandling;
using WillCorp.App.Web.Api.Handlers;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using WillCorp.App.Web.StructureMap;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using System.Configuration;
using System.IO;
using WillCorp.App.Web.SignalR;

namespace WillCorp.App.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Setup the cors middleware to run before other pipeline entries.
            // By default this will allow all origins. You can 
            // configure the set of origins and/or http verbs by
            // providing a cors options with a different policy.
            appBuilder.UseCors(CorsOptions.AllowAll);

            appBuilder.Use<GlobalExceptionMiddleware>();

            appBuilder.Map("/api", api =>
            {
                // Create our config object we'll use to configure the API
                //
                var config = new HttpConfiguration();

                config.UseStructureMap(WebModule.Container);

                // Use attribute routing
                //
                config.MapHttpAttributeRoutes();

                config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

                // clear the supported mediatypes of the xml formatter
                config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

                //Remove the XM Formatter from the web api
                config.Formatters.Remove(config.Formatters.XmlFormatter);

                var jsonFormatter = config.Formatters.JsonFormatter;
                jsonFormatter.UseDataContractJsonSerializer = false;

                var settings = jsonFormatter.SerializerSettings;
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

#if DEBUG
                settings.Formatting = Formatting.Indented; // Pretty json for developers.
#else
            settings.Formatting = Formatting.None;
#endif

                // Now add in the WebAPI middleware
                //
                api.UseWebApi(config);
            });

            appBuilder.Map("/signalr", map =>
            {
                var config = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                    EnableDetailedErrors = true,
                    EnableJavaScriptProxies = true,
                    EnableJSONP = true
                };

                config.UseStructureMap(WebModule.Container);

                // camelcase contract resolver
                var serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    ContractResolver = new SignalRContractResolver()
                });

                config.Resolver.Register(typeof(JsonSerializer), () => serializer);

                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch is already runs under the "/signalr"
                // path.
                map.RunSignalR(config);
            });

            var webRoot = @"./wwwroot";
            var cfg = ConfigurationManager.AppSettings["web:wwwroot"];
            if (!string.IsNullOrEmpty(cfg))
            {
                if (Directory.Exists(cfg))
                {
                    if (File.Exists(Path.Combine(cfg, "index.html")))
                    {
                        webRoot = cfg;
                    }
                }
            }

            var physicalFileSystem = new PhysicalFileSystem(webRoot);
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem,
                EnableDirectoryBrowsing = false
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[]
            {
                "index.html"
            };

            appBuilder.UseFileServer(options);
        }
    }
}
