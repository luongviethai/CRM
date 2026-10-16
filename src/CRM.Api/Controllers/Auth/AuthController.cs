using CRM.Application.Auth;
using CRM.Application.Auth.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        IIdentityService identityService,
        ITokenService tokenService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _identityService.RegisterAsync(request.Email, request.Password, request.FullName);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(StatusCodes.Status201Created, new { userId = result.UserId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var credentials = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);
        if (credentials is null)
        {
            return Unauthorized();
        }

        var token = _tokenService.GenerateAccessToken(credentials);

        return Ok(new AuthResponse(token.Value, token.ExpiresAtUtc, credentials.Email, credentials.FullName));
    }

    // Endpoint tạm để xác nhận JWT hoạt động đúng — xem được claims thật sự nằm trong token
    // sau khi roundtrip qua AddJwtBearer validate.
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
