using System;
using System.Threading.Tasks;
using QuantityMeasurementApp.Entities;

namespace RepositoryLayer.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> UserExistsByUsernameAsync(string username);
        Task<bool> UserExistsByEmailAsync(string email);

        Task CreateUserAsync(UserEntity user);

        Task<UserEntity?> GetUserByLoginAsync(string login);
        Task<UserEntity?> GetUserByUserIdAsync(Guid userId);

        Task SaveRefreshTokenAsync(RefreshTokenEntity refreshToken);
        Task<RefreshTokenEntity?> GetRefreshTokenByHashAsync(byte[] tokenHash);

        Task RevokeRefreshTokenAsync(Guid refreshTokenId, DateTime revokedUtc, Guid replacedByRefreshTokenId);
        Task RevokeRefreshTokenByHashAsync(byte[] tokenHash, DateTime revokedUtc);
    }
}