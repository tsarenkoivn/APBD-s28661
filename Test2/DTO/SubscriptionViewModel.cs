namespace Test2.DTO
{
    public class SubscriptionViewModel
    {
        public int IdSubscription { get; set; }
        public string Name { get; set; } = null!;
        public DateTime EndTime { get; set; }
        public int AmountPaid { get; set; }
    }
}
