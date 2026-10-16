namespace CRM.Application.Auth;

public interface ITokenService
{
    AccessToken GenerateAccessToken(UserCredentials user);
}
