using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Konscious.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementApp.Dtos;
using QuantityMeasurementApp.Entities;
using QuantityMeasurementApp.Enums;
using RepositoryLayer.Repositories;
using ServiceLayer.Interfaces;
using ServiceLayer.Options;

namespace ServiceLayer.Services
{
    public sealed class AuthServiceImpl : IAuthService
    {
        private const int SaltLengthBytes = 16;
        private const int PasswordHashLengthBytes = 32;

        private const int Argon2MemoryMb = 64;
        private const int Argon2Iterations = 4;
        private const int Argon2Parallelism = 2;

        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        private readonly IAuthRepository authRepository;
        private readonly JwtTokenOptions jwtOptions;

        public AuthServiceImpl(IAuthRepository authRepository, JwtTokenOptions jwtOptions)
        {
            this.authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            this.jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        public async Task<AuthRegisterResultDto> RegisterAsync(string username, string email, string password)
        {
            AuthRegisterResultDto result = new AuthRegisterResultDto();

            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return BuildRegisterError(400, "Username is required.");
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    return BuildRegisterError(400, "Email is required.");
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return BuildRegisterError(400, "Password is required.");
                }

                bool usernameExists = await authRepository.UserExistsByUsernameAsync(username);
                if (usernameExists)
                {
                    return BuildRegisterError(409, "Username already exists.");
                }

                bool emailExists = await authRepository.UserExistsByEmailAsync(email);
                if (emailExists)
                {
                    return BuildRegisterError(409, "Email already exists.");
                }

                byte[] salt = RandomNumberGenerator.GetBytes(SaltLengthBytes);
                byte[] hash = ComputeArgon2idHash(password, salt);

                Guid userId = Guid.NewGuid();
                DateTime nowUtc = DateTime.UtcNow;

                UserEntity user = new UserEntity(
                    userId,
                    username,
                    email,
                    hash,
                    salt,
                    UserRole.User.ToString(),
                    nowUtc,
                    updatedUtc: null);

                await authRepository.CreateUserAsync(user);

                result.HasError = false;
                result.StatusCode = 201;
                result.Message = "User Created";
                result.ErrorMessage = null;

                return result;
            }
            catch (Exception ex)
            {
                return BuildRegisterError(500, ex.Message);
            }
        }

        public async Task<AuthSessionResultDto> LoginAsync(string login, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login))
                {
                    return BuildSessionError(400, "Login is required.");
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return BuildSessionError(400, "Password is required.");
                }

                UserEntity? user = await authRepository.GetUserByLoginAsync(login);
                if (user is null)
                {
                    return BuildSessionError(401, "Invalid credentials.");
                }

                bool passwordValid = VerifyPassword(password, user.PasswordSalt, user.PasswordHash);
                if (!passwordValid)
                {
                    return BuildSessionError(401, "Invalid credentials.");
                }

                (string accessToken, DateTime expiresUtc) = CreateAccessToken(user);

                string refreshTokenPlainText = CreateRefreshTokenPlainText();
                byte[] refreshTokenHash = ComputeSha256(refreshTokenPlainText);

                DateTime nowUtc = DateTime.UtcNow;

                RefreshTokenEntity refreshToken = new RefreshTokenEntity(
                    refreshTokenId: Guid.NewGuid(),
                    userId: user.UserId,
                    tokenHash: refreshTokenHash,
                    createdUtc: nowUtc,
                    expiresUtc: nowUtc.Add(RefreshTokenLifetime),
                    revokedUtc: null,
                    replacedByRefreshTokenId: null);

                await authRepository.SaveRefreshTokenAsync(refreshToken);

                return BuildSessionSuccess(user, accessToken, expiresUtc, refreshTokenPlainText);
            }
            catch (Exception ex)
            {
                return BuildSessionError(500, ex.Message);
            }
        }

        public async Task<AuthSessionResultDto> RefreshAsync(string refreshTokenPlainText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshTokenPlainText))
                {
                    return BuildSessionError(401, "Missing refresh token.");
                }

                byte[] tokenHash = ComputeSha256(refreshTokenPlainText);

                RefreshTokenEntity? existing = await authRepository.GetRefreshTokenByHashAsync(tokenHash);
                if (existing is null)
                {
                    return BuildSessionError(401, "Invalid refresh token.");
                }

                if (existing.RevokedUtc.HasValue)
                {
                    return BuildSessionError(401, "Refresh token revoked.");
                }

                if (DateTime.UtcNow >= existing.ExpiresUtc)
                {
                    return BuildSessionError(401, "Refresh token expired.");
                }

                UserEntity? user = await authRepository.GetUserByUserIdAsync(existing.UserId);
                if (user is null)
                {
                    return BuildSessionError(401, "Invalid refresh token.");
                }

                string newRefreshTokenPlainText = CreateRefreshTokenPlainText();
                byte[] newTokenHash = ComputeSha256(newRefreshTokenPlainText);

                Guid newRefreshTokenId = Guid.NewGuid();
                DateTime nowUtc = DateTime.UtcNow;

                RefreshTokenEntity newRefreshToken = new RefreshTokenEntity(
                    refreshTokenId: newRefreshTokenId,
                    userId: user.UserId,
                    tokenHash: newTokenHash,
                    createdUtc: nowUtc,
                    expiresUtc: nowUtc.Add(RefreshTokenLifetime),
                    revokedUtc: null,
                    replacedByRefreshTokenId: null);

                await authRepository.SaveRefreshTokenAsync(newRefreshToken);
                await authRepository.RevokeRefreshTokenAsync(existing.RefreshTokenId, nowUtc, newRefreshTokenId);

                (string accessToken, DateTime expiresUtc) = CreateAccessToken(user);

                return BuildSessionSuccess(user, accessToken, expiresUtc, newRefreshTokenPlainText);
            }
            catch (Exception ex)
            {
                return BuildSessionError(500, ex.Message);
            }
        }

        public async Task LogoutAsync(string? refreshTokenPlainText)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenPlainText))
            {
                return;
            }

            byte[] tokenHash = ComputeSha256(refreshTokenPlainText);
            await authRepository.RevokeRefreshTokenByHashAsync(tokenHash, DateTime.UtcNow);
        }

        private static AuthRegisterResultDto BuildRegisterError(int statusCode, string message)
        {
            return new AuthRegisterResultDto
            {
                HasError = true,
                StatusCode = statusCode,
                ErrorMessage = message,
                Message = null
            };
        }

        private static AuthSessionResultDto BuildSessionError(int statusCode, string message)
        {
            return new AuthSessionResultDto
            {
                HasError = true,
                StatusCode = statusCode,
                ErrorMessage = message,
                AccessToken = null,
                AccessTokenExpiresUtc = null,
                UserId = null,
                Username = null,
                Email = null,
                Role = null,
                RefreshTokenPlainText = null
            };
        }

        private static AuthSessionResultDto BuildSessionSuccess(UserEntity user, string accessToken, DateTime expiresUtc, string refreshTokenPlainText)
        {
            UserRole roleEnum = ParseRole(user.Role);

            return new AuthSessionResultDto
            {
                HasError = false,
                StatusCode = 200,
                ErrorMessage = null,
                AccessToken = accessToken,
                AccessTokenExpiresUtc = expiresUtc,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = roleEnum,
                RefreshTokenPlainText = refreshTokenPlainText
            };
        }

        private (string Token, DateTime ExpiresUtc) CreateAccessToken(UserEntity user)
        {
            DateTime nowUtc = DateTime.UtcNow;
            DateTime expiresUtc = nowUtc.Add(jwtOptions.AccessTokenLifetime);

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims =
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                notBefore: nowUtc,
                expires: expiresUtc,
                signingCredentials: creds);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expiresUtc);
        }

        private static byte[] ComputeArgon2idHash(string password, byte[] salt)
        {
            if (password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            salt = salt ?? throw new ArgumentNullException(nameof(salt));

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using Argon2id argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = Argon2Parallelism,
                Iterations = Argon2Iterations,
                MemorySize = Argon2MemoryMb * 1024
            };

            return argon2.GetBytes(PasswordHashLengthBytes);
        }

        private static bool VerifyPassword(string password, byte[] salt, byte[] expectedHash)
        {
            byte[] actual = ComputeArgon2idHash(password, salt);
            return CryptographicOperations.FixedTimeEquals(actual, expectedHash);
        }

        private static string CreateRefreshTokenPlainText()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            return Base64UrlEncode(bytes);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            string base64 = Convert.ToBase64String(bytes);
            return base64
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        private static byte[] ComputeSha256(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return SHA256.HashData(bytes);
        }

        private static UserRole ParseRole(string roleText)
        {
            if (Enum.TryParse(roleText, ignoreCase: true, out UserRole role))
            {
                return role;
            }

            return UserRole.User;
        }
    }
}