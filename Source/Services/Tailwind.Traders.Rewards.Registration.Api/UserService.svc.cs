using System;
using Tailwind.Traders.Rewards.Registration.Api.Repositories;

namespace Tailwind.Traders.Rewards.Registration.Api
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class UserService : IUserService
    {
        private readonly CustomerRepository _customerRepository;

        public UserService()
        {
            _customerRepository = new CustomerRepository();
        }

        public bool Registration(string email)
        {
            try
            {
                var user = _customerRepository.GetCustomerByEmailOrName(email);
                if (user != null)
                {
                    return false;
                }

                _customerRepository.InsertCustomer(email);

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
