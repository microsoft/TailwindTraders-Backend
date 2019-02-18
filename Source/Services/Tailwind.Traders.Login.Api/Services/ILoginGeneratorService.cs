using Tailwind.Traders.Login.Api.Models;

namespace Tailwind.Traders.Login.Api.Services
{
    public interface ILoginGeneratorService
    {
        TokenResponseModel GenerateToken(string username);
    }
}
