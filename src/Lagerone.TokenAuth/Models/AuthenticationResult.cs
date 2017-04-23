namespace Lagerone.TokenAuth.Models
{
    public class AuthenticationResult
    {
        public AuthenticationStatus AuthenticationStatus { get; set; }
        public string Email { get; set; }
    }
}