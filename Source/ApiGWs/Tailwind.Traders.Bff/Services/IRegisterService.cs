using System.Threading.Tasks;

namespace Tailwind.Traders.MobileBff.Services
{
    public interface IRegisterService
    {
        Task<bool> RegisterUserIfNotExists(string email);
    }
}
