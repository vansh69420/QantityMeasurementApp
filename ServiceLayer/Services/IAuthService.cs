using System.Threading.Tasks;
using QuantityMeasurementApp.Dtos;

namespace ServiceLayer.Interfaces
{
    public interface IAuthService
    {
        Task<AuthRegisterResultDto> RegisterAsync(string username, string email, string password);

        Task<AuthSessionResultDto> LoginAsync(string login, string password);

        Task<AuthSessionResultDto> RefreshAsync(string refreshTokenPlainText);

        Task LogoutAsync(string? refreshTokenPlainText);
    }
}