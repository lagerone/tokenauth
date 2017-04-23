using System;
using System.Threading.Tasks;
using System.Web;
using Lagerone.TokenAuth.Factories;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Providers;
using Lagerone.TokenAuth.Repositories;
using Lagerone.TokenAuth.Settings;
using Lagerone.TokenAuth.Utils;

namespace Lagerone.TokenAuth.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRequestRepository _authenticationRequestRepository;
        private readonly IAuthenticatedUserRepository _authenticatedUserRepository;
        private readonly ICookieHelper _cookieHelper;
        private readonly ICookieSettings _cookieSettings;
        private readonly IDateProvider _dateProvider;
        private readonly IAuthenticatedUserFactory _authenticatedUserFactory;

        public AuthenticationService(IAuthenticationRequestRepository authenticationRequestRepository,
            IAuthenticatedUserRepository authenticatedUserRepository, 
            ICookieHelper cookieHelper,
            ICookieSettings cookieSettings,
            IDateProvider dateProvider, 
            IAuthenticatedUserFactory authenticatedUserFactory)
        {
            _authenticationRequestRepository = authenticationRequestRepository;
            _authenticatedUserRepository = authenticatedUserRepository;
            _cookieHelper = cookieHelper;
            _cookieSettings = cookieSettings;
            _dateProvider = dateProvider;
            _authenticatedUserFactory = authenticatedUserFactory;
        }
        
        public async Task<AuthenticationResult> AuthenticateUser(HttpRequestBase httpRequest,
            HttpResponseBase httpResponse, string emailToken)
        {
            if (emailToken == null)
            {
                throw new ArgumentNullException(nameof(emailToken));
            }
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }
            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }

            var authenticationRequestCookie = _cookieHelper.GetCookieFromRequest(httpRequest,
                _cookieSettings.TokenCookieName);
            if (authenticationRequestCookie == null)
            {
                return GetInvalidResponse(AuthenticationStatus.Fail);
            }

            var cookieToken = authenticationRequestCookie.Value;
            if (string.IsNullOrWhiteSpace(cookieToken))
            {
                return GetInvalidResponse(AuthenticationStatus.Fail);
            }
            _cookieHelper.DeleteCookie(httpResponse, _cookieSettings.TokenCookieName);

            var authenticationRequest =
                await _authenticationRequestRepository.GetRequestByTokens(cookieToken, emailToken);
            if (authenticationRequest == null)
            {
                return GetInvalidResponse(AuthenticationStatus.Fail);
            }

            if (authenticationRequest.ExpirationDate < _dateProvider.UtcNow)
            {
                return GetInvalidResponse(AuthenticationStatus.Fail);
            }

            var authenticatedUser = _authenticatedUserFactory.Create(authenticationRequest.Email, 
                _dateProvider.UtcNow.AddMinutes(_cookieSettings.AuthenticatedUserCookieExpirationMinutes));

            await _authenticatedUserRepository.Add(authenticatedUser);

            var authenticadedUserCookie = new HttpCookie(_cookieSettings.AuthenticatedUserCookieName, authenticatedUser.Id)
            {
                Expires = authenticatedUser.Expires
            };

            _cookieHelper.SetCookie(httpResponse, authenticadedUserCookie);

            return new AuthenticationResult
            {
                AuthenticationStatus = AuthenticationStatus.Success,
                Email = authenticatedUser.Email
            };
        }

        public async Task DeleteAuthenticatedUser(HttpRequestBase httpRequest,
            HttpResponseBase httpResponse)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }
            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }
            var cookie = _cookieHelper.GetCookieFromRequest(httpRequest, _cookieSettings.AuthenticatedUserCookieName);
            if (cookie != null)
            {
                await _authenticatedUserRepository.Delete(cookie.Value);
                _cookieHelper.DeleteCookie(httpResponse, _cookieSettings.AuthenticatedUserCookieName);
            }
        }

        public async Task SetAuthenticatedUserId(HttpRequestBase httpRequest, string userId)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            var authCookie = _cookieHelper.GetCookieFromRequest(httpRequest,
                _cookieSettings.AuthenticatedUserCookieName);
            var authenticationId = authCookie?.Value;
            if (string.IsNullOrEmpty(authenticationId))
            {
                throw new Exception("Invalid authentication request");
            }
            await _authenticatedUserRepository.UpdateUserId(authenticationId, userId);
        }
        
        private static AuthenticationResult GetInvalidResponse(AuthenticationStatus status)
        {
            return new AuthenticationResult {AuthenticationStatus = status};
        }
    }
}