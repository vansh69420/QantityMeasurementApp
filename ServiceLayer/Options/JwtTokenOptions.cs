using System;

namespace ServiceLayer.Options
{
    public sealed class JwtTokenOptions
    {
        public JwtTokenOptions(string issuer, string audience, string signingKey, TimeSpan accessTokenLifetime)
        {
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            Audience = audience ?? throw new ArgumentNullException(nameof(audience));
            SigningKey = signingKey ?? throw new ArgumentNullException(nameof(signingKey));
            AccessTokenLifetime = accessTokenLifetime;
        }

        public string Issuer { get; }
        public string Audience { get; }
        public string SigningKey { get; }
        public TimeSpan AccessTokenLifetime { get; }
    }
}