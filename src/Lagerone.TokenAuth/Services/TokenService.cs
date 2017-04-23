using System;
using System.Threading.Tasks;
using System.Web;
using Lagerone.TokenAuth.Factories;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Providers;
using Lagerone.TokenAuth.Repositories;
using Lagerone.TokenAuth.Settings;
using Lagerone.TokenAuth.Utils;
using Lagerone.TokenAuth.Validators;

namespace Lagerone.TokenAuth.Services
{
    internal class TokenService : ITokenService
    {
        private readonly IAuthenticationRequestRepository _authenticationRequestRepository;
        private readonly IEmailValidator _emailValidator;
        private readonly ICookieHelper _cookieHelper;
        private readonly ICookieSettings _cookieSettings;
        private readonly IDateProvider _dateProvider;
        private readonly IAuthenticationRequestFactory _authenticationRequestFactory;

        public TokenService(IAuthenticationRequestRepository authenticationRequestRepository,
            IEmailValidator emailValidator,
            ICookieHelper cookieHelper,
            ICookieSettings cookieSettings,
            IDateProvider dateProvider,
            IAuthenticationRequestFactory authenticationRequestFactory)
        {
            _authenticationRequestRepository = authenticationRequestRepository;
            _emailValidator = emailValidator;
            _cookieHelper = cookieHelper;
            _cookieSettings = cookieSettings;
            _dateProvider = dateProvider;
            _authenticationRequestFactory = authenticationRequestFactory;
        }

        public async Task<RequestTokenResult> CreateRequestToken(string email, HttpResponseBase httpResponse)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }
            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }
            if (!_emailValidator.IsValid(email))
            {
                return CreateInvalidResponse(AuthenticationStatus.Fail);
            }

            var emailNormalized = email.Trim().ToLowerInvariant();
            await _authenticationRequestRepository.DeleteRequestsByEmail(emailNormalized);

            var authRequest = _authenticationRequestFactory.Create(email, 
                _dateProvider.UtcNow.AddMinutes(_cookieSettings.TokenCookieExpirationMinutes));
            await _authenticationRequestRepository.AddRequest(authRequest);

            SetRequestTokenCookie(httpResponse, authRequest);

            return new RequestTokenResult
            {
                AuthenticationStatus = AuthenticationStatus.Success,
                Email = authRequest.Email,
                EmailToken = authRequest.EmailToken,
            };
        }

        private void SetRequestTokenCookie(HttpResponseBase httpResponse,
            AuthenticationRequest authenticationRequest)
        {
            var requestTokenCookie = new HttpCookie(_cookieSettings.TokenCookieName)
            {
                Expires = authenticationRequest.ExpirationDate,
                Value = authenticationRequest.CookieToken,
            };
            _cookieHelper.SetCookie(httpResponse, requestTokenCookie);
        }

        private static RequestTokenResult CreateInvalidResponse(AuthenticationStatus status)
        {
            return new RequestTokenResult { AuthenticationStatus = status };
        }
    }
}