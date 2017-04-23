using System;
using Lagerone.TokenAuth.Models;

namespace Lagerone.TokenAuth.Factories
{
    internal class AuthenticatedUserFactory : IAuthenticatedUserFactory
    {
        public AuthenticatedUser Create(string email, DateTime expirationDate)
        {
            return new AuthenticatedUser
            {
                Email = email,
                Expires = expirationDate,
            };
        }
    }
}