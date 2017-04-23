using System;
using Lagerone.TokenAuth.Models;

namespace Lagerone.TokenAuth.Factories
{
    internal interface IAuthenticatedUserFactory
    {
        AuthenticatedUser Create(string email, DateTime expirationDate);
    }
}
