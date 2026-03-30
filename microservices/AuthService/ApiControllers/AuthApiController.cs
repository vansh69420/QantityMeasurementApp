using System;
using System.Threading.Tasks;
using AuthService.ApiControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Dtos;
using ServiceLayer.Interfaces;
using AuthService.Contracts;

namespace AuthService.ApiControllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthApiController : ControllerBase
    {
        private const string RefreshCookieName = "qm_refresh_token";

        private readonly IAuthService authService;

        public AuthApiController(IAuthService authService)
        {
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
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

        private void AppendRefreshCookie(string refreshTokenPlainText)
        {
            CookieOptions options = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/"
            };

            Response.Cookies.Append(RefreshCookieName, refreshTokenPlainText, options);
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