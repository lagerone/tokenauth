using System;
using System.Threading.Tasks;
using System.Web;
using FakeItEasy;
using Lagerone.TokenAuth.Factories;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Providers;
using Lagerone.TokenAuth.Repositories;
using Lagerone.TokenAuth.Services;
using Lagerone.TokenAuth.Settings;
using Lagerone.TokenAuth.Utils;
using Lagerone.TokenAuth.Validators;
using NUnit.Framework;

namespace Lagerone.TokenAuth.Tests.Services
{
    [TestFixture]
    public class TokenServiceTests
    {
#pragma warning disable 649
        [UnderTest] private readonly TokenService _tokenService;
        [Fake] private readonly IEmailValidator _emailValidator;
        [Fake] private readonly IAuthenticationRequestRepository _authenticationRequestRepository;
        [Fake] private readonly IDateProvider _dateProvider;
        [Fake] private readonly ICookieSettings _cookieSettings;
        [Fake] private readonly IAuthenticationRequestFactory _authenticationRequestFactory;
        [Fake] private readonly ICookieHelper _cookieHelper;
#pragma warning restore 649

        private readonly HttpResponseBase _httpResponse = A.Dummy<HttpResponseBase>();
        private readonly string _email = "a@fake.email";
        private readonly int _expirationMinues = 180;
        private readonly DateTime _utcNow = DateTime.UtcNow;
        private readonly string _cookietoken = "cookietoken";
        private readonly string _requestCookieName = "requestcookie";

        [SetUp]
        public void SetUp()
        {
            Fake.InitializeFixture(this);
            SetupCookieSettings();
            A.CallTo(() => _dateProvider.UtcNow).Returns(_utcNow);
        }

        [Test]
        public async Task Should_return_invalid_if_email_is_invalid()
        {
            //Arrange
            const string invalidEmail = "invalidEmail";
            A.CallTo(() => _emailValidator.IsValid(invalidEmail)).Returns(false);

            //Act
            var result = await _tokenService.CreateRequestToken(invalidEmail, _httpResponse);

            //Assert
            Assert.That(result.AuthenticationStatus, Is.EqualTo(AuthenticationStatus.Fail));
        }

        [TestCase("Some.User@gmail.com", "some.user@gmail.com")]
        [TestCase("    Some.User@gmail.com ", "some.user@gmail.com")]
        public async Task Should_delete_existing_requests_by_normalized_email(string inputEmail, string normalizedEmail)
        {
            //Arrange
            A.CallTo(() => _emailValidator.IsValid(inputEmail)).Returns(true);

            //Act
            await _tokenService.CreateRequestToken(inputEmail, _httpResponse);

            //Assert
            A.CallTo(() => _authenticationRequestRepository.DeleteRequestsByEmail(normalizedEmail))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_create_AuthenticationRequest()
        {
            //Arrange
            SetupValidEmail();

            //Act
            await _tokenService.CreateRequestToken(_email, _httpResponse);

            //Asssert
            A.CallTo(() => _authenticationRequestFactory.Create(_email,
                    _utcNow.AddMinutes(_cookieSettings.TokenCookieExpirationMinutes)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_save_AuthenticationRequest()
        {
            //Arrange
            SetupValidEmail();
            var authRequest = new AuthenticationRequest {Email = _email};
            A.CallTo(() => _authenticationRequestFactory.Create(_email, A<DateTime>._)).Returns(authRequest);

            //Act
            await _tokenService.CreateRequestToken(_email, _httpResponse);

            //Assert
            A.CallTo(() => _authenticationRequestRepository.AddRequest(authRequest))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_set_request_cookie()
        {
            //Arrange
            SetupValidEmail();
            var authRequest = SetupAuthenticationRequestFactory();

            //Act
            await _tokenService.CreateRequestToken(_email, _httpResponse);

            //Assert
            A.CallTo(() => _cookieHelper.SetCookie(A<HttpResponseBase>._,
                    A<HttpCookie>.That.Matches(c => c.Name.Equals(_requestCookieName)
                                                    && c.Value.Equals(authRequest.CookieToken)
                                                    && c.Expires.Equals(authRequest.ExpirationDate))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_return_saved_emailToken_on_success()
        {
            //Arrange
            SetupValidEmail();
            var authRequest = SetupAuthenticationRequestFactory();

            //Act
            var result = await _tokenService.CreateRequestToken(_email, _httpResponse);

            //Assert
            Assert.That(result.EmailToken, Is.EqualTo(authRequest.EmailToken));
        }

        [Test]
        public async Task Should_return_saved_email_on_success()
        {
            //Arrange
            SetupValidEmail();
            var authRequest = SetupAuthenticationRequestFactory();

            //Act
            var result = await _tokenService.CreateRequestToken(_email, _httpResponse);

            //Assert
            Assert.That(result.Email, Is.EqualTo(authRequest.Email));
        }

        [Test]
        public async Task Should_return_success_status_on_success()
        {
            //Arrange
            SetupValidEmail();

            //Act
            var result = await _tokenService.CreateRequestToken(_email, _httpResponse);

            //Assert
            Assert.That(result.AuthenticationStatus, Is.EqualTo(AuthenticationStatus.Success));
        }

        private AuthenticationRequest SetupAuthenticationRequestFactory()
        {
            var authRequest = new AuthenticationRequest
            {
                Email = _email,
                CookieToken = _cookietoken,
                ExpirationDate = _utcNow.AddMinutes(_cookieSettings.TokenCookieExpirationMinutes),
            };
            A.CallTo(() => _authenticationRequestFactory.Create(_email, A<DateTime>._)).Returns(authRequest);
            return authRequest;
        }

        private void SetupCookieSettings()
        {
            A.CallTo(() => _cookieSettings.TokenCookieExpirationMinutes).Returns(_expirationMinues);
            A.CallTo(() => _cookieSettings.TokenCookieName).Returns(_requestCookieName);
        }

        private void SetupValidEmail()
        {
            A.CallTo(() => _emailValidator.IsValid(_email)).Returns(true);
        }
    }
}