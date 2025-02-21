using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AshishGeneralStore.DTOs;
using AshishGeneralStore.Services;
using AshishGeneralStore.Common;
using AshishGeneralStore.DTOs.Authentication;
using AshishGeneralStore.Services.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace AshishGeneralStore.Controllers.Authentication;

[ApiController]
[Route(Constants.ApiRoutes.AuthBase)]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var tokens = await _authService.LoginAsync(loginDto);
            Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });
            return Ok(new { AccessToken = tokens.AccessToken });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid credentials.");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message, Action = "Use /api/auth/force-login to log out from all devices and log in." });
        }
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto userDto)
    {
        try
        {
            var user = await _authService.RegisterAsync(userDto);
            return CreatedAtAction(nameof(Login), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest("Refresh token not found.");
        }

        try
        {
            var tokens = await _authService.RefreshTokenAsync(refreshToken);
            Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });
            return Ok(new { AccessToken = tokens.AccessToken });
        }
        catch (SecurityTokenException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var user = User.Identity;
        if (user == null || !user.IsAuthenticated)
        {
            return Unauthorized("User is not authenticated.");
        }

        var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (userIdClaim == null)
        {
            return BadRequest("User ID claim is missing.");
        }

        if (int.TryParse(userIdClaim, out int userId))
        {
            await _authService.RevokeAllTokensAsync(userId); // Delete all tokens for the user
        }

        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }


    [HttpPost("force-login")]
    public async Task<IActionResult> ForceLogin([FromBody] LoginDto loginDto)
    {
        try
        {
            var tokens = await _authService.LoginAsync(loginDto, forceLogin: true);
            Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });
            return Ok(new { AccessToken = tokens.AccessToken });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid credentials.");
        }
    }
}