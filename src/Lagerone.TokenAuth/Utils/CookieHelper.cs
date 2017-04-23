using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Lagerone.TokenAuth.Utils
{
    internal class CookieHelper : ICookieHelper
    {
        public CookieState GetCookieFromRequestHeaders(HttpRequestHeaders httpRequestHeaders, string cookieName)
        {
            var cookieHeaderValue = httpRequestHeaders.GetCookies(cookieName).FirstOrDefault();
            // WTF?!!?!
            return
                cookieHeaderValue?.Cookies.FirstOrDefault(
                    c => string.Equals(c.Name, cookieName, StringComparison.OrdinalIgnoreCase));
        }

        public HttpCookie GetCookieFromRequest(HttpRequestBase httpRequest, string cookieName)
        {
            return httpRequest.Cookies.Get(cookieName);
        }

        public void DeleteCookie(HttpResponseBase httpResponse, string cookieName)
        {
            httpResponse.Cookies.Remove(cookieName);
            var expiredCookie = new HttpCookie(cookieName)
            {
                Expires = DateTime.Now.AddDays(-10),
                Value = null,
            };
            SetCookie(httpResponse, expiredCookie);
        }

        public void SetCookie(HttpResponseBase httpResponse, HttpCookie cookie)
        {
            if (httpResponse.Cookies.AllKeys.Contains(cookie.Name))
            {
                httpResponse.Cookies.Set(cookie);
            }
            else
            {
                httpResponse.Cookies.Add(cookie);
            }
        }
    }
}