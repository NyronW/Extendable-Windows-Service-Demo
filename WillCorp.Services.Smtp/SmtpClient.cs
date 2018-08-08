using WillCorp.Smtp;
using System;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using WillCorp.Logging;
using WillCorp.Configuration;

namespace WillCorp.Services.Smtp
{
    public class SmtpClient : ISmtpClient, IServicePlugin
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRepository _configuration;
        private readonly Regex _expression = new Regex(@"\w+@[0-9a-zA-Z_]+?\.[a-zA-Z]{2,10}");
        private readonly bool _validateEmail;

        public SmtpClient(ILogger logger, IConfigurationRepository configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _validateEmail = _configuration.GetConfigurationValue("smtp:validate.email", false);
        }

        public bool Send(SmtpMessage message)
        {
            if (message.Recipients.Length == 0) return false;

            bool sent = true;

            try
            {
                _logger.Debug($"Sending email to {string.Join(", ", message.Recipients)}");

                var mail = new MailMessage();
                foreach (string email in message.Recipients)
                {
                    if (!ValidateEmailAddress(email)) continue;
                    mail.To.Add(new MailAddress(email));
                }

                mail.From = new MailAddress(message.Sender);
                mail.Subject = message.Subject;
                mail.Body = message.Message;
                mail.IsBodyHtml = message.SendAsHtml;
                mail.BodyEncoding = Encoding.UTF8;

                if (message.Cc != null && message.Cc.Length != 0)
                {
                    foreach (string email in message.Cc)
                    {
                        if (!ValidateEmailAddress(email)) continue;
                        mail.CC.Add(new MailAddress(email));
                    }
                }

                if (message.Bcc != null && message.Bcc.Length != 0)
                {
                    foreach (string email in message.Bcc)
                    {
                        if (!ValidateEmailAddress(email)) continue;
                        mail.Bcc.Add(new MailAddress(email));
                    }
                }

                var client = new System.Net.Mail.SmtpClient
                {
                    UseDefaultCredentials = false
                };

                if (message.Attachments != null)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        try
                        {
                            mail.Attachments.Add(new Attachment(attachment));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Attachment failed",ex);
                        }
                    }

                    _logger.Verbose($"{message.Attachments.Length} file(s) attached to email");
                }

                client.Send(mail);

                _logger.Debug($"Email sent via smtp host {client.Host}");
            }
            catch (Exception ex)
            {
                sent = false;
                _logger.Error("Error occured while sending email", ex);
            }

            return sent;
        }

        private bool ValidateEmailAddress(string emailAddress)
        {
            if (!_validateEmail) return true;

            try
            {
                return _expression.IsMatch(emailAddress);
            }
            catch
            {
                _logger.Verbose("{email} failed email validation", emailAddress);
                return false;
            }
        }
    }
}
