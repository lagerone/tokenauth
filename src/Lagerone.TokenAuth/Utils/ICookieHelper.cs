using System.Net.Http.Headers;
using System.Web;

namespace Lagerone.TokenAuth.Utils
{
    internal interface ICookieHelper
    {
        CookieState GetCookieFromRequestHeaders(HttpRequestHeaders httpRequestHeaders, string cookieName);
        HttpCookie GetCookieFromRequest(HttpRequestBase httpRequest, string cookieName);
        void DeleteCookie(HttpResponseBase httpResponse, string cookieName);
        void SetCookie(HttpResponseBase httpResponse, HttpCookie cookie);
    }
}