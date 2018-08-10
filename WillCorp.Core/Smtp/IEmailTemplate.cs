namespace WillCorp.Smtp
{
    /// <summary>
    /// Implementing classes will be responsible for
    /// generating tamplataes used when sending email.
    /// It accepts the data to be sent and performs some
    /// form of interpulation to create the email body;
    /// this is very useful when sending html based emails
    /// </summary>
    public interface IEmailTemplate
    {
        string Generate<TModel>(string templateName, TModel model);
    }
}
