namespace Lagerone.TokenAuth.Settings
{
    public interface ICookieSettings
    {
        string TokenCookieName { get; }
        string AuthenticatedUserCookieName { get; }
        int AuthenticatedUserCookieExpirationMinutes { get; }
        int TokenCookieExpirationMinutes { get; }
    }
}