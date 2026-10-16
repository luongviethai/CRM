namespace CRM.Application.Auth;

public record UserCredentials(int UserId, string Email, string FullName, IReadOnlyList<string> Roles);
