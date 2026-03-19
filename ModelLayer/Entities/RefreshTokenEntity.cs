using System;

namespace QuantityMeasurementApp.Entities
{
    public sealed class RefreshTokenEntity
    {
        public RefreshTokenEntity(
            Guid refreshTokenId,
            Guid userId,
            byte[] tokenHash,
            DateTime createdUtc,
            DateTime expiresUtc,
            DateTime? revokedUtc,
            Guid? replacedByRefreshTokenId)
        {
            RefreshTokenId = refreshTokenId;
            UserId = userId;
            TokenHash = tokenHash ?? throw new ArgumentNullException(nameof(tokenHash));
            CreatedUtc = createdUtc;
            ExpiresUtc = expiresUtc;
            RevokedUtc = revokedUtc;
            ReplacedByRefreshTokenId = replacedByRefreshTokenId;
        }

        public Guid RefreshTokenId { get; }
        public Guid UserId { get; }
        public byte[] TokenHash { get; }
        public DateTime CreatedUtc { get; }
        public DateTime ExpiresUtc { get; }
        public DateTime? RevokedUtc { get; }
        public Guid? ReplacedByRefreshTokenId { get; }
    }
}