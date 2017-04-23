using System;

namespace Lagerone.TokenAuth.Models
{
    internal class AuthenticatedUser : MongoEntity
    {
        public string Email { get; set; }
        public DateTime Expires { get; set; }
        public string UserId { get; set; }
    }
}