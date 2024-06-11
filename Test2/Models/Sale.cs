using Microsoft.EntityFrameworkCore.Query;

namespace Test2.Models
{
    public class Sale
    {
        public int IdSale { get; set; }
        public int IdClient { get; set; }
        public int IdSubscription { get; set; }
        public DateTime CreatedAt { get; set; }
        public Client Client { get; set; } = null!;
        public Subscription Subscription { get; set; } = null!;
    }
}
