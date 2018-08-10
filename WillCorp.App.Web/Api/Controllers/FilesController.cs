using System;
using System.IO;
using System.Web.Http;
using WillCorp.Configuration;
using WillCorp.Core.FileSystem;
using WillCorp.Logging;

namespace WillCorp.App.Web.Api.Controllers
{
    [RoutePrefix("files")]
    public class FilesController: BaseController
    {
        private readonly IConfigurationRepository _configuration;
        private readonly ILogger _logger;

        public FilesController(IConfigurationRepository configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [Route("")]
        public IHttpActionResult Post(string name, string content)
        {
            if (string.IsNullOrEmpty(name)) return Error("Invalid file name");
            if (string.IsNullOrEmpty(content)) return Error("Invalid file content");

            try
            {
                var directory = _configuration.GetConfigurationValue(ConfigurationKeys.ImportDirectory, "");
                if(string.IsNullOrEmpty(directory)) return InternalServerError("Import directory not set");

                var file = ImportFile.Create(Path.Combine(directory,name), content);
                file.Save();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return InternalServerError("File not create");
            }

            return Ok();
        }
    }
}
