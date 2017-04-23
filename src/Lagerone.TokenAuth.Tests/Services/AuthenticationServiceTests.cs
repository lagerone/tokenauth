using System;
using System.Threading.Tasks;
using System.Web;
using FakeItEasy;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Settings;
using Lagerone.TokenAuth.Factories;
using Lagerone.TokenAuth.Providers;
using Lagerone.TokenAuth.Repositories;
using Lagerone.TokenAuth.Utils;
using Lagerone.TokenAuth.Services;
using NUnit.Framework;

namespace Lagerone.TokenAuth.Tests.Services
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
#pragma warning disable 649
        [UnderTest] private readonly AuthenticationService _authenticationService;
        [Fake] private readonly ICookieHelper _cookieHelper;
        [Fake] private readonly ICookieSettings _cookieSettings;
        [Fake] private readonly IAuthenticationRequestRepository _authenticationRequestRepository;
        [Fake] private readonly IDateProvider _dateProvider;
        [Fake] private readonly IAuthenticatedUserRepository _authenticatedUserRepository;
        [Fake] private readonly IAuthenticatedUserFactory _authenticatedUserFactory;
#pragma warning restore 649

        private readonly HttpRequestBase _httpRequest = A.Dummy<HttpRequestBase>();
        private readonly HttpResponseBase _httpResponse = A.Dummy<HttpResponseBase>();
        private readonly string _emailtoken = "emailtoken";
        private readonly string _cookietoken = "cookietoken";
        private readonly string _requestCookieName = "requestcookie";
        private readonly string _authCookieName = "authcookie";
        private readonly int _authCookieExpirationMinues = 100;
        private readonly DateTime _utcNow = DateTime.UtcNow;

        [SetUp]
        public void Setup()
        {
            Fake.InitializeFixture(this);
            SetupCookieSettings();
            A.CallTo(() => _dateProvider.UtcNow).Returns(_utcNow);
        }

        [Test]
        public async Task Should_fail_when_no_request_cookie_is_found()
        {
            //Act
            var result = await _authenticationService.AuthenticateUser(_httpRequest, _httpResponse, _emailtoken);

            //Assert
            Assert.That(result.AuthenticationStatus, Is.EqualTo(AuthenticationStatus.Fail));
        }

        [Test]
        public async Task Should_fail_when_no_request_cookie_value_is_found()
        {
            //Arrange
            A.CallTo(() => _cookieHelper.GetCookieFromRequest(_httpRequest, _requestCookieName)).Returns(null);

            //Act
            var result = await _authenticationService.AuthenticateUser(_httpRequest, _httpResponse, _emailtoken);

            //Assert
            Assert.That(result.AuthenticationStatus, Is.EqualTo(AuthenticationStatus.Fail));
        }

        [Test]
        public async Task Should_delete_request_cookie_from_response()
        {
            //Arrange
            SetupValidRequestCookie();

            //Act
            await _authenticationService.AuthenticateUser(_httpRequest, _httpResponse, _emailtoken);

            //Assert
            A.CallTo(() => _cookieHelper.DeleteCookie(_httpResponse, _requestCookieName))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_get_request_from_database()
        {
            //Arrange
            SetupValidRequestCookie();

            //Act
            await _authenticationService.AuthenticateUser(_httpRequest, _httpResponse, _emailtoken);

            //Assert
            A.CallTo(() => _authenticationRequestRepository.GetRequestByTokens(_cookietoken, _emailtoken))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_fail_if_no_request_in_database_is_found()
        {
            //Arrange
            SetupValidRequestCookie();
            A.CallTo(() => _authenticationRequestRepository.GetRequestByTokens(_cookietoken, _emailtoken))
                .Returns(Task.FromResult<AuthenticationRequest>(null));

            //Act
            var result = await _authenticationService.AuthenticateUser(_httpRequest, _httpResponse, _emailtoken);

            //Assert
            Assert.That(result.AuthenticationStatus, Is.EqualTo(AuthenticationStatus.Fail));
        }

        [Test]
        public async Task Should_fail_if_request_from_database_has_expired()
        {
            //Arrange
            SetupValidRequestCookie();
            SetupAuthRequestRepository(string.Empty, _utcNow.AddDays(-1));

            //Act
            var result = await _authenticationService.AuthenticateUser(_httpRequest, _httpResponse, _emailtoken);

            //Assert
            Assert.That(result.AuthenticationStatus, Is.EqualTo(AuthenticationStatus.Fail));
        }

        [Test]
        public async Task Should_create_authenticated_user()
        {
            //Arrange
            SetupValidRequestCookie();
            const string email = "some@email.com";
            SetupAuthRequestRepository(email, _utcNow.AddDays(1));

            //Act
            await _authenticationService.AuthenticateUser(_httpRequest, _httpResponse, _emailtoken);

            //Assert
            A.CallTo(() => _authenticatedUserFactory.Create(email, _utcNow.AddMinutes(_authCookieExpirationMinues)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_save_authenticaded_user_to_database()
        {
            //Arrange
            SetupValidRequestCookie();
            const string email = "some@email.com";
            SetupAuthRequestRepository(email, _utcNow.AddDays(1));
            var authenticatedUser = SetupAuthenticatedUserFactory(null, email);

            //Act
            await _authenticationService.AuthenticateUser(_httpRequest, _httpResponse, _emailtoken);

            //Assert
            A.CallTo(() => _authenticatedUserRepository.Add(authenticatedUser))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_set_authenticated_user_cookie()
        {
            //Arrange
            SetupValidRequestCookie();

            const string email = "some@email.com";
            SetupAuthRequestRepository(email, _utcNow.AddDays(1));

            const string authId = "authenticatedserId";
            SetupAuthenticatedUserFactory(authId, email);
            var expectedExpirationDate = _utcNow.AddMinutes(_authCookieExpirationMinues);

            //Act
            await _authenticationService.AuthenticateUser(_httpRequest, _httpResponse, _emailtoken);

            //Assert
            A.CallTo(() => _cookieHelper.SetCookie(_httpResponse,
                    A<HttpCookie>.That.Matches(c => c.Name.Equals(_authCookieName)
                                                    && c.Value.Equals(authId)
                                                    && c.Expires.Equals(expectedExpirationDate))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private AuthenticatedUser SetupAuthenticatedUserFactory(string id, string email)
        {
            var expirationDate = _utcNow.AddMinutes(_authCookieExpirationMinues);
            var authenticatedUser = new AuthenticatedUser
            {
                Id = id,
                Email = email,
                Expires = expirationDate,
            };
            A.CallTo(() => _authenticatedUserFactory.Create(email, expirationDate))
                .Returns(authenticatedUser);

            return authenticatedUser;
        }

        private void SetupAuthRequestRepository(string email, DateTime expirationDate)
        {
            var authenticationRequest = new AuthenticationRequest
            {
                Email = email,
                ExpirationDate = expirationDate,
            };
            A.CallTo(() => _authenticationRequestRepository.GetRequestByTokens(_cookietoken, _emailtoken))
                .Returns(Task.FromResult(authenticationRequest));
        }

        private void SetupValidRequestCookie()
        {
            A.CallTo(() => _cookieHelper.GetCookieFromRequest(_httpRequest, _requestCookieName))
                .Returns(new HttpCookie(_requestCookieName, _cookietoken));
        }

        private void SetupCookieSettings()
        {
            A.CallTo(() => _cookieSettings.TokenCookieName).Returns(_requestCookieName);
            A.CallTo(() => _cookieSettings.AuthenticatedUserCookieName).Returns(_authCookieName);
            A.CallTo(() => _cookieSettings.AuthenticatedUserCookieExpirationMinutes)
                .Returns(_authCookieExpirationMinues);
        }
    }
}