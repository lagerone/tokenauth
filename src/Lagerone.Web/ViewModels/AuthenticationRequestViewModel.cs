using System.ComponentModel.DataAnnotations;

namespace Lagerone.Web.ViewModels
{
    public class AuthenticationRequestViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}