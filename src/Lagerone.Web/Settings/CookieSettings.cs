using Lagerone.TokenAuth.Settings;

namespace Lagerone.Web.Settings
{
    internal class CookieSettings : ICookieSettings
    {
        public string TokenCookieName => "AuthenticationRequest";
        public int AuthenticatedUserCookieExpirationMinutes => 60 * 24 * 365;
        public string AuthenticatedUserCookieName => "AuthenticatedUser";
        public int TokenCookieExpirationMinutes => 180;
    }
}