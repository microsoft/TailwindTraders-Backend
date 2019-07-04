using System.Threading.Tasks;

namespace Tailwind.Traders.WebBff.Services
{
    public interface IRegisterService
    {
        Task<bool> RegisterUserIfNotExists(string email);
    }
}
