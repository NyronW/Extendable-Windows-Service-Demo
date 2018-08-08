using System;

namespace WillCorp.Smtp
{
    public struct SmtpMessage
    {
        public SmtpMessage(string sender, string subject, string message, string[] recipients, string[] cc, string[] bcc, string[] attachments, bool sendAsHtml = false)
        {
            if(string.IsNullOrEmpty(sender)) throw new ArgumentNullException(nameof(sender));
            if(string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
            if(string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
            if(recipients.Length == 0) throw new ArgumentNullException(nameof(recipients));

            Sender = sender;
            Subject = subject;
            Message = message;
            Recipients = recipients;
            Cc = cc;
            Bcc = bcc;
            Attachments = attachments;
            SendAsHtml = sendAsHtml;
        }

        public SmtpMessage(string sender, string subject, string message, string[] recipients, bool sendAsHtml = false)
            :this(sender, subject, message, recipients,null,null,null, sendAsHtml)
        {

        }

        public string Sender { get; }
        public string Subject { get; }
        public string Message { get; }
        public string[] Recipients { get; }
        public string[] Cc { get; }
        public string[] Bcc { get; }
        public string[] Attachments { get; }
        public bool SendAsHtml { get; }
    }
}
