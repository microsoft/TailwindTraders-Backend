using System.ServiceModel;

namespace Tailwind.Traders.Rewards.Registration.Api
{
    [ServiceContract]
    public interface IUserService
    {

        [OperationContract]
        bool Registration(string email);
    }
}
