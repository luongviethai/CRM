namespace CRM.Application.Auth;

public interface IIdentityService
{
    Task<RegisterResult> RegisterAsync(string email, string password, string fullName);
    Task<UserCredentials?> ValidateCredentialsAsync(string email, string password);
}
