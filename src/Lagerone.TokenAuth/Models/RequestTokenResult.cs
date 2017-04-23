namespace Lagerone.TokenAuth.Models
{
    public class RequestTokenResult
    {
        public AuthenticationStatus AuthenticationStatus { get; set; }
        public string Email { get; set; }
        public string EmailToken { get; set; }
    }
}