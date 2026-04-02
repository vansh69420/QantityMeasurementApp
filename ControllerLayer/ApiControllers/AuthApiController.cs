using System;
using ControllerLayer.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QuantityMeasurementApp.Dtos;
using ServiceLayer.Interfaces;

namespace ControllerLayer.ApiControllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthApiController : ControllerBase
    {
        private const string RefreshCookieName = "qm_refresh_token";

        private readonly IAuthService authService;
        private readonly AuthCookieOptions authCookieOptions;

        public AuthApiController(IAuthService authService, IOptions<AuthCookieOptions> authCookieOptions)
        {
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.authCookieOptions = authCookieOptions?.Value ?? throw new ArgumentNullException(nameof(authCookieOptions));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            AuthRegisterResultDto result = await authService.RegisterAsync(request.Username, request.Email, request.Password);

            if (result.HasError)
            {
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            return Created(string.Empty, result.Message);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            AuthSessionResultDto result = await authService.LoginAsync(request.Login, request.Password);

            if (result.HasError)
            {
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            if (string.IsNullOrWhiteSpace(result.RefreshTokenPlainText))
            {
                return StatusCode(500, "Refresh token generation failed.");
            }

            AppendRefreshCookie(result.RefreshTokenPlainText);

            return Ok(MapToResponse(result));
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue(RefreshCookieName, out string? refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized("Missing refresh token.");
            }

            AuthSessionResultDto result = await authService.RefreshAsync(refreshToken);

            if (result.HasError)
            {
                return StatusCode(result.StatusCode, result.ErrorMessage);
            }

            if (string.IsNullOrWhiteSpace(result.RefreshTokenPlainText))
            {
                return StatusCode(500, "Refresh token rotation failed.");
            }

            AppendRefreshCookie(result.RefreshTokenPlainText);

            return Ok(MapToResponse(result));
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Request.Cookies.TryGetValue(RefreshCookieName, out string? refreshToken);

            await authService.LogoutAsync(refreshToken);

            DeleteRefreshCookie();

            return NoContent();
        }

        private void AppendRefreshCookie(string refreshTokenPlainText)
        {
            SameSiteMode sameSiteMode = ParseSameSite(authCookieOptions.SameSite);

            CookieOptions options = new CookieOptions
            {
                HttpOnly = true,
                Secure = authCookieOptions.Secure,
                SameSite = sameSiteMode,
                Expires = DateTimeOffset.UtcNow.AddDays(authCookieOptions.ExpirationDays),
                Path = "/"
            };

            Response.Cookies.Append(RefreshCookieName, refreshTokenPlainText, options);
        }

        private void DeleteRefreshCookie()
        {
            SameSiteMode sameSiteMode = ParseSameSite(authCookieOptions.SameSite);

            CookieOptions options = new CookieOptions
            {
                HttpOnly = true,
                Secure = authCookieOptions.Secure,
                SameSite = sameSiteMode,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                Path = "/"
            };

            Response.Cookies.Delete(RefreshCookieName, options);
        }

        private static SameSiteMode ParseSameSite(string? sameSiteText)
        {
            if (string.IsNullOrWhiteSpace(sameSiteText))
            {
                return SameSiteMode.Lax;
            }

            return sameSiteText.Trim().ToLowerInvariant() switch
            {
                "none" => SameSiteMode.None,
                "strict" => SameSiteMode.Strict,
                _ => SameSiteMode.Lax
            };
        }

        private static AuthSessionResponse MapToResponse(AuthSessionResultDto dto)
        {
            return new AuthSessionResponse
            {
                AccessToken = dto.AccessToken ?? string.Empty,
                AccessTokenExpiresUtc = dto.AccessTokenExpiresUtc ?? DateTime.UtcNow,
                UserId = dto.UserId ?? Guid.Empty,
                Username = dto.Username ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                Role = dto.Role?.ToString() ?? string.Empty
            };
        }
    }
}