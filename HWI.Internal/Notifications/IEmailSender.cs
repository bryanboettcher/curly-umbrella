namespace HWI.Internal.Notifications
{
    public interface IEmailSender
    {
        string ServerName { get; set; }
        int ServerPort { get; set; }
        SmtpTransportSecurityType SecurityType { get; set; }
        SmtpAuthenticationType AuthenticationType { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        void Send(EmailMessage message);
    }
}