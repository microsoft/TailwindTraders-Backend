using System.Data;
using Tailwind.Traders.Rewards.Registration.Api.Models;

namespace Tailwind.Traders.Rewards.Registration.Api.Mappers
{
    public class CustomerMapper
    {
        public Customer Map(DataRow customerRow)
        {
            if (customerRow == null)
            {
                return null;
            }

            return new Customer
            {
                AccountCode = customerRow["AccountCode"].ToString(),
                Active = (bool)customerRow["Active"],
                City = customerRow["City"].ToString(),
                Country = customerRow["Country"].ToString(),
                CustomerId = (int)customerRow["CustomerId"],
                Email = customerRow["Email"].ToString(),
                Enrrolled = (EnrollmentStatusEnum)customerRow["Enrrolled"],
                FaxNumber = customerRow["FaxNumber"].ToString(),
                FirstAddress = customerRow["FirstAddress"].ToString(),
                FirstName = customerRow["FirstName"].ToString(),
                LastName = customerRow["LastName"].ToString(),
                MobileNumber = customerRow["MobileNumber"].ToString(),
                PhoneNumber = customerRow["PhoneNumber"].ToString(),
                RowVersion = (byte[])customerRow["RowVersion"],
                Website = customerRow["Website"].ToString(),
                ZipCode = customerRow["ZipCode"].ToString()
            };
        }
    }
}