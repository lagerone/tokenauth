using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lagerone.TokenAuth.Repositories;
using Lagerone.TokenAuth.Settings;
using Lagerone.TokenAuth.Utils;

namespace Lagerone.TokenAuth.Providers
{
    internal class CurrentUserIdProvider : ICurrentUserIdProvider
    {
        private readonly ICookieSettings _cookieSettings;
        private readonly ICookieHelper _cookieHelper;
        private readonly IAuthenticatedUserRepository _authenticatedUserRepository;

        public CurrentUserIdProvider(ICookieSettings cookieSettings,
            ICookieHelper cookieHelper,
            IAuthenticatedUserRepository authenticatedUserRepository)
        {
            _cookieSettings = cookieSettings;
            _cookieHelper = cookieHelper;
            _authenticatedUserRepository = authenticatedUserRepository;
        }

        public async Task<string> Get(HttpRequestHeaders httpRequestHeaders)
        {
            var authCookie = _cookieHelper.GetCookieFromRequestHeaders(httpRequestHeaders,
                _cookieSettings.AuthenticatedUserCookieName);
            if (authCookie == null)
            {
                return null;
            }
            var authenticatedUser = await _authenticatedUserRepository.GetById(authCookie.Value);
            return authenticatedUser?.UserId;
        }
    }
}