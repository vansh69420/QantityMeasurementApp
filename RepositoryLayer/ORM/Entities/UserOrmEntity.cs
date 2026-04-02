using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Orm.Entities
{
    [Table("Users", Schema = "dbo")]
    public sealed class UserOrmEntity
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }

        [MaxLength(64)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        [MaxLength(32)]
        public string Role { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; }

        public DateTime? UpdatedUtc { get; set; }
    }
}