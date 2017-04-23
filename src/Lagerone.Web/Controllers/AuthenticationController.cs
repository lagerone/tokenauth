using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lagerone.TokenAuth.Models;
using Lagerone.TokenAuth.Services;
using Lagerone.Web.Settings;
using Lagerone.Web.ViewModels;

namespace Lagerone.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IEmailSettings _emailSettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITokenService _tokenService;

        private const string TempDataErrorKey = "TempDataError";

        public AuthenticationController(IEmailSettings emailSettings,
            IAuthenticationService authenticationService,
            ITokenService tokenService)
        {
            _emailSettings = emailSettings;
            _authenticationService = authenticationService;
            _tokenService = tokenService;
        }

        public ActionResult Index()
        {
            return View(model: TempData[TempDataErrorKey]);
        }

        public ActionResult AccessDenied()
        {
            return View(model: TempData[TempDataErrorKey]);
        }

        public async Task<RedirectToRouteResult> LogIn(string token)
        {
            var authResult = await _authenticationService.AuthenticateUser(Request, Response, token);
            if (!authResult.AuthenticationStatus.Equals(AuthenticationStatus.Success))
            {
                TempData[TempDataErrorKey] = "Access denied";
                return RedirectToAction("AccessDenied");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<RedirectToRouteResult> LogOff()
        {
            await _authenticationService.DeleteAuthenticatedUser(Request, Response);
            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<RedirectToRouteResult> Post(AuthenticationRequestViewModel vm)
        {
            if (string.IsNullOrEmpty(vm?.Email))
            {
                TempData[TempDataErrorKey] = "You must enter an email address";
                return RedirectToAction("Index");
            }
            var authResult = await _tokenService.CreateRequestToken(vm.Email, Response);
            if (!authResult.AuthenticationStatus.Equals(AuthenticationStatus.Success))
            {
                TempData[TempDataErrorKey] = "Request failed";
                return RedirectToAction("Index");
            }

            SendMail(authResult.Email, authResult.EmailToken);

            return RedirectToAction("LoginMailSent", new { email = authResult.Email });
        }

        public ActionResult LoginMailSent(string email)
        {
            return View(model: email);
        }

        private void SendMail(string email, string emailToken)
        {
            var encodedToken = HttpUtility.UrlEncode(emailToken);
            var loginUrl = $"{_emailSettings.LoginBaseUrl}/authentication/login?token={encodedToken}";
            var body = $"<p><a href=\"{loginUrl}\">Log in</a>.</p>";
            
            using (var smtp = new SmtpClient())
            using (var message = new MailMessage())
            {
                message.To.Add(new MailAddress(email));
                message.From = new MailAddress(_emailSettings.FromAddress);
                message.Subject = "Log in mail";
                message.Body = body;
                message.IsBodyHtml = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential
                {
                    UserName = _emailSettings.Username,
                    Password = _emailSettings.Password,
                };
                smtp.Host = _emailSettings.SmtpHost;
                smtp.Port = _emailSettings.SmtpPort;
                smtp.EnableSsl = _emailSettings.EnableSsl;

                smtp.Send(message);
            }
        }
    }
}