namespace WillCorp.Smtp
{
    public interface ISmtpClient
    {
        bool Send(SmtpMessage message);
    }
}
