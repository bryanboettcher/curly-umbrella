using System;
using System.Net;
using System.Net.Mail;
using HWI.Internal.Extensions;

namespace HWI.Internal.Notifications
{
    public class SmtpMailer : IEmailSender
    {
        private SmtpClient _smtpClient;
        private string _serverName;
        public string ServerName
        {
            get => _serverName;
            set
            {
                if (_serverName == value)
                    return;
                _serverName = value;
                _smtpClient = null;
            }
        }

        public int ServerPort { get; set; }
        public SmtpTransportSecurityType SecurityType { get; set; }
        public SmtpAuthenticationType AuthenticationType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public SmtpMailer()
        {
            // Set defaults
            ServerPort = 25;
            SecurityType = SmtpTransportSecurityType.None;
            AuthenticationType = SmtpAuthenticationType.None;
        }

        public SmtpMailer(string hostName) : this()
        {
            ServerName = hostName;
        }

        public void Send(EmailMessage message)
        {
            if (_smtpClient == null)
            {
                _smtpClient = BuildSmtpClient();
            }

            _smtpClient.Send(GetMailMessage(message));
        }

        private static MailMessage GetMailMessage(EmailMessage message)
        {
            var newMsg = new MailMessage
            {
                From = new MailAddress(message.From),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = message.IsHtml,
                Priority = (MailPriority) message.Priority
            };

            message.Attachments.ForEach(a => newMsg.Attachments.Add(new Attachment(a)));
            message.To.ForEach(x => newMsg.To.Add(new MailAddress(x.Key, x.Value)));
            message.Cc.ForEach(x => newMsg.CC.Add(new MailAddress(x.Key, x.Value)));
            message.Bcc.ForEach(x => newMsg.Bcc.Add(new MailAddress(x.Key, x.Value)));

            return newMsg;
        }

        private SmtpClient BuildSmtpClient()
        {
            if (string.IsNullOrWhiteSpace(ServerName))
                throw new InvalidOperationException("Cannot build an SMTP client until the SMTP server name has been specified");

            var client = new SmtpClient(ServerName, ServerPort)
            {
                EnableSsl = SecurityType == SmtpTransportSecurityType.Encrypted,
                Credentials = SetSmtpCredentials()
            };
            return client;
        }

        private ICredentialsByHost SetSmtpCredentials()
        {
            if (AuthenticationType == SmtpAuthenticationType.None)
                return null;

            throw new NotImplementedException("Using SMTP credentials has not been implemented yet");
        }
    }
}
