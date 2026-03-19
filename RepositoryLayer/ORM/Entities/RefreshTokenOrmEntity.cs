using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Orm.Entities
{
    [Table("RefreshTokens", Schema = "dbo")]
    public sealed class RefreshTokenOrmEntity
    {
        public long Id { get; set; }

        public Guid RefreshTokenId { get; set; }

        public Guid UserId { get; set; }

        [Column(TypeName = "varbinary(32)")]
        public byte[] TokenHash { get; set; } = Array.Empty<byte>();

        public DateTime CreatedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }

        public DateTime? RevokedUtc { get; set; }

        public Guid? ReplacedByRefreshTokenId { get; set; }
    }
}