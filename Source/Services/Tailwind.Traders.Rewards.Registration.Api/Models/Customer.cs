namespace Tailwind.Traders.Rewards.Registration.Api.Models
{
    public class Customer
    {
        public byte[] RowVersion { get; set; }
        public int CustomerId { get; set; }
        public string AccountCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FirstAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Website { get; set; }
        public bool Active { get; set; }
        public EnrollmentStatusEnum Enrrolled { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string FaxNumber { get; set; }
    }
}