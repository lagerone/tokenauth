using System;
using Lagerone.TokenAuth.Models;

namespace Lagerone.TokenAuth.Factories
{
    internal class AuthenticationRequestFactory : IAuthenticationRequestFactory
    {
        private readonly ICookieTokenFactory _cookieTokenFactory;

        public AuthenticationRequestFactory(ICookieTokenFactory cookieTokenFactory)
        {
            _cookieTokenFactory = cookieTokenFactory;
        }
        public AuthenticationRequest Create(string email, DateTime expirationDate)
        {
            return new AuthenticationRequest
            {
                CookieToken = _cookieTokenFactory.Create(),
                EmailToken = _cookieTokenFactory.Create(),
                Email = email,
                ExpirationDate = expirationDate,
            };
        }
    }
}