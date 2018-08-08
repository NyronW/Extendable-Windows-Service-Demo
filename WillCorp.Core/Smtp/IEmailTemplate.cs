namespace WillCorp.Smtp
{
    public interface IEmailTemplate
    {
        string Generate<TModel>(string templateName, TModel model);
    }
}
