namespace Test2.DTO
{
    public class ClientRequestModel
    {
        public int IdClient { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public List<SubscriptionViewModel> Subscriptions { get; set; } = new List<SubscriptionViewModel>();
        public int TotalPaidAmount { get; set; }
    }
}
