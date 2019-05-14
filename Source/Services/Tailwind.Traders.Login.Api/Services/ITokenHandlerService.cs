using Tailwind.Traders.Login.Api.Models;

namespace Tailwind.Traders.Login.Api.Services
{
    public interface ITokenHandlerService
    {
        TokenResponseModel SignIn(string username);

        TokenResponseModel RefreshAccessToken(string token);
    }
}
