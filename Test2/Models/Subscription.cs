using System.Data.SqlTypes;

namespace Test2.Models
{
    public class Subscription
    {
        public int IdSubscription { get; set; }
        public string Name { get; set; } = null!;
        public int RenewalPeriod { get; set; }
        public DateTime EndTime { get; set; }
        public int Money { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
