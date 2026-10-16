namespace CRM.Application.Auth;

public class JwtSettings
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string SigningKey { get; set; }
    public int AccessTokenExpirationMinutes { get; set; } = 60;
}
