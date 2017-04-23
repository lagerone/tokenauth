namespace Lagerone.TokenAuth.Validators
{
    internal class EmailValidator : IEmailValidator
    {
        public bool IsValid(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
        }
    }
}