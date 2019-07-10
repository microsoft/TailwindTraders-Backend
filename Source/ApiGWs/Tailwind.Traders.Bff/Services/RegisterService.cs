using RegistrationUserService;
using System.Threading.Tasks;

namespace Tailwind.Traders.MobileBff.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IUserService _client;

        public RegisterService(IUserService client)
        {
            _client = client;
        }

        public async Task<bool> RegisterUserIfNotExists(string email)
        {
            return await _client.RegistrationAsync(email);
        }
    }
}
