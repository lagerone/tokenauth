namespace Lagerone.Web.Settings
{
    public interface IEmailSettings
    {
        string LoginBaseUrl { get; }
        string Username { get; }
        string Password { get; }
        string FromAddress { get; }
        string SmtpHost { get; }
        int SmtpPort { get; }
        bool EnableSsl { get; }
    }
}