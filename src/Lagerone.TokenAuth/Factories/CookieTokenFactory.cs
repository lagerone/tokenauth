using System;

namespace Lagerone.TokenAuth.Factories
{
    internal class CookieTokenFactory : ICookieTokenFactory
    {
        public string Create()
        {
            return Guid.NewGuid().ToString();
        }
    }
}