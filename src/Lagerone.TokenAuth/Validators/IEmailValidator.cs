namespace Lagerone.TokenAuth.Validators
{
    public interface IEmailValidator
    {
        bool IsValid(string email);
    }
}