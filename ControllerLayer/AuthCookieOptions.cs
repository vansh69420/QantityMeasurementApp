namespace ControllerLayer
{
    public sealed class AuthCookieOptions
    {
        public string SameSite { get; set; } = "Lax";

        public bool Secure { get; set; }

        public int ExpirationDays { get; set; } = 7;
    }
}