using System;

namespace AuthService.Contracts
{
    public sealed class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public sealed class LoginRequest
    {
        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public sealed class AuthSessionResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiresUtc { get; set; }

        public Guid UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}