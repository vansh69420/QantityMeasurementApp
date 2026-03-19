using System;

namespace QuantityMeasurementApp.Entities
{
    public sealed class UserEntity
    {
        public UserEntity(
            Guid userId,
            string username,
            string email,
            byte[] passwordHash,
            byte[] passwordSalt,
            string role,
            DateTime createdUtc,
            DateTime? updatedUtc)
        {
            UserId = userId;
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            PasswordSalt = passwordSalt ?? throw new ArgumentNullException(nameof(passwordSalt));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            CreatedUtc = createdUtc;
            UpdatedUtc = updatedUtc;
        }

        public Guid UserId { get; }
        public string Username { get; }
        public string Email { get; }
        public byte[] PasswordHash { get; }
        public byte[] PasswordSalt { get; }
        public string Role { get; }
        public DateTime CreatedUtc { get; }
        public DateTime? UpdatedUtc { get; }
    }
}