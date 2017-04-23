using System;
using System.Net.Http.Headers;
using System.Web;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Repositories;
using Lagerone.TokenAuth.Settings;
using Lagerone.TokenAuth.Utils;

namespace Lagerone.TokenAuth.Services
{
    internal class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthenticatedUserRepository _authenticatedUserRepository;
        private readonly ICookieHelper _cookieHelper;
        private readonly ICookieSettings _cookieSettings;

        public AuthorizationService(IAuthenticatedUserRepository authenticatedUserRepository,
            ICookieHelper cookieHelper,
            ICookieSettings cookieSettings)
        {
            _authenticatedUserRepository = authenticatedUserRepository;
            _cookieHelper = cookieHelper;
            _cookieSettings = cookieSettings;
        }

        public AuthorizationResult UserIsAutorizedSync(HttpRequestHeaders httpRequestHeaders)
        {
            if (httpRequestHeaders == null)
            {
                throw new ArgumentNullException(nameof(httpRequestHeaders));
            }
            var authCookie = _cookieHelper.GetCookieFromRequestHeaders(httpRequestHeaders,
                _cookieSettings.AuthenticatedUserCookieName);
            var authenticationId = authCookie?.Value;
            return UserIsAuthorizedSync(authenticationId);
        }

        public AuthorizationResult UserIsAutorizedSync(HttpRequestBase httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }
            var authCookie = _cookieHelper.GetCookieFromRequest(httpRequest,
                _cookieSettings.AuthenticatedUserCookieName);
            var authenticationId = authCookie?.Value;
            return UserIsAuthorizedSync(authenticationId);
        }

        private AuthorizationResult UserIsAuthorizedSync(string authenticationId)
        {
            if (string.IsNullOrWhiteSpace(authenticationId))
            {
                return GetInvalidResponse(AuthorizationStatus.Fail);
            }

            var authenticatedUser = _authenticatedUserRepository.GetByIdSync(authenticationId);
            if (authenticatedUser == null)
            {
                return GetInvalidResponse(AuthorizationStatus.Fail);
            }

            if (authenticatedUser.Expires < DateTime.UtcNow)
            {
                _authenticatedUserRepository.DeleteSync(authenticationId);
                return GetInvalidResponse(AuthorizationStatus.Fail);
            }

            return new AuthorizationResult
            {
                AuthorizationStatus = AuthorizationStatus.Success
            };
        }

        private static AuthorizationResult GetInvalidResponse(AuthorizationStatus status)
        {
            return new AuthorizationResult { AuthorizationStatus= status};
        }
    }
}