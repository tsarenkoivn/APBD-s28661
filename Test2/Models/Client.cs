namespace Test2.Models
{
    public class Client
    {
        public int IdClient { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
