namespace CRM.Application.Auth;

public record IssuedRefreshToken(string Value, DateTime ExpiresAtUtc);
