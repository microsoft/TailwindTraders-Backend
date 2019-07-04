using System.Configuration;
using System.Data.SqlClient;
using Tailwind.Traders.Rewards.Registration.Api.Mappers;
using Tailwind.Traders.Rewards.Registration.Api.Models;

namespace Tailwind.Traders.Rewards.Registration.Api.Repositories
{
    public class CustomerRepository : BaseRepository
    {
        private readonly CustomerMapper _mapper;
        public CustomerRepository() : base(ConfigurationManager.ConnectionStrings["dbContext"].ConnectionString)
        {
            _mapper = new CustomerMapper();
        }

        public Customer GetCustomerByEmailOrName(string emailOrName)
        {
            var query = "SELECT TOP 1 * FROM CUSTOMERS WHERE FirstName = @emailOrName OR Email = @emailOrName";
            var emailOrNameParam = new SqlParameter("@emailOrName", emailOrName);
            var table = ExecuteSelect(query, new SqlParameter[] { emailOrNameParam });
            if (table?.Rows != null && table.Rows.Count > 0)
            {
                return _mapper.Map(table.Rows[0]);
            }

            return null;
        }

        public void InsertCustomer(string email)
        {
            var query = @"INSERT INTO CUSTOMERS
                                    ([Email], 
                                    [Active],
                                    [Enrrolled]
                                    ) 
                                VALUES 
                                    (@Email
                                    ,@Active
                                    ,@Enrrolled)";


            var enrollmentStatus = (int)EnrollmentStatusEnum.Uninitialized;
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Email", email),
                new SqlParameter("@Active", true),
                new SqlParameter("@Enrrolled", enrollmentStatus)
        };

            ExecuteNonSelect(query, parameters);
        }
    }
}