using HandlebarsDotNet;
using WillCorp.Configuration;
using WillCorp.Logging;
using WillCorp.Smtp;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace WillCorp.Services.Smtp.Template
{
    public class EmailTemplate : IEmailTemplate
    {
        private readonly string _templatePath;
        private readonly ILogger _logger;
        public static ConcurrentDictionary<string, Func<object, string>> Templates = new ConcurrentDictionary<string, Func<object, string>>();

        public EmailTemplate(ILogger logger, IConfigurationRepository configuration)
        {
            _logger = logger;

            var cfg = configuration.GetConfigurationValue("smtp:template.folder","");
            if (string.IsNullOrEmpty(cfg)) cfg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
            _templatePath = cfg;
        }

        public string Generate<TModel>(string templateName, TModel model)
        {
            try
            {
                if (Templates.ContainsKey(templateName))
                {
                    var tmpl = Templates[templateName];
                    return tmpl(model);
                }

                var path = Path.Combine(_templatePath, templateName + ".html");
                if (!System.IO.File.Exists(path))
                {
                    _logger.Warn("Email template {template} not found. The following directories were checked: {templatePath}", templateName, _templatePath);
                    return string.Empty;
                }

                var source = System.IO.File.ReadAllText(path);
                if (string.IsNullOrEmpty(source))
                {
                    _logger.Warn("Email template {template} is empty", templateName);
                    return string.Empty;
                }

                var template = Handlebars.Compile(source);
                Templates.AddOrUpdate(templateName, template, (key, oldValue) => template);

                return template(model);
            }
            catch (Exception e)
            {
                _logger.Error("Error occured while generating email from template: {template}", e, templateName);
                return string.Empty;
            }
        }
    }
}
