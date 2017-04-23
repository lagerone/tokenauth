# TokenAuth

TokenAuth is a token-based authentication library for [ASP.NET MVC](https://www.asp.net/mvc), inspired by [Passwordless](https://passwordless.net/) for Express & Node.js.

TokenAuth requires mongodb to store login credentials.

## Installation

For the project run correctly you need to add implementations of `Lagerone.TokenAuth.Settings.IDatabaseSettings` and `Lagerone.Web.Settings.IEmailSettings` and populate them with your credentials, e.g.

```
using Lagerone.TokenAuth.Settings;

namespace Lagerone.Web.Settings
{
    public class Settings : IDatabaseSettings, IEmailSettings
    {
        string IDatabaseSettings.ConnectionString => ""; // e.g. mongodb://localhost:27017/tokenauth

        string IEmailSettings.LoginBaseUrl => ""; // The base url for the login link in the email, e.g. http://localhost:50194
        string IEmailSettings.Username => "";
        string IEmailSettings.Password => "";
        string IEmailSettings.FromAddress => "";
        string IEmailSettings.SmtpHost => "";
        int IEmailSettings.SmtpPort => 587;
        bool IEmailSettings.EnableSsl => true;
    }
}
```
