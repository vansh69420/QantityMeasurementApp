using System;
using QuantityMeasurementApp.Enums;

namespace QuantityMeasurementApp.Dtos
{
    public sealed class AuthRegisterResultDto
    {
        public bool HasError { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Message { get; set; }
    }

    public sealed class AuthSessionResultDto
    {
        public bool HasError { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        public string? AccessToken { get; set; }
        public DateTime? AccessTokenExpiresUtc { get; set; }

        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public UserRole? Role { get; set; }

        // Used ONLY by controller to set HttpOnly cookie; never returned directly.
        public string? RefreshTokenPlainText { get; set; }
    }
}