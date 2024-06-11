using System.Data.SqlTypes;

namespace Test2.Models
{
    public class Payment
    {
        public int IdPayment { get; set; }
        public DateTime Date { get; set; }
        public int IdClient { get; set; }
        public int IdSubscription { get; set; }
        public int Value { get; set; }
        public Client Client { get; set; } = null!;
        public Subscription Subscription { get; set; } = null!;
    }
}
