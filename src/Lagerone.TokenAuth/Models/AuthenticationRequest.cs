using System;

namespace Lagerone.TokenAuth.Models
{
    internal class AuthenticationRequest : MongoEntity
    {
        public string CookieToken { get; set; }
        public string EmailToken { get; set; }
        public string Email { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}