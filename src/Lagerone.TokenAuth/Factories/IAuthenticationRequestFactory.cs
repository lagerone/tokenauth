using System;
using Lagerone.TokenAuth.Models;

namespace Lagerone.TokenAuth.Factories
{
    internal interface IAuthenticationRequestFactory
    {
        AuthenticationRequest Create(string email, DateTime expirationDate);
    }
}
