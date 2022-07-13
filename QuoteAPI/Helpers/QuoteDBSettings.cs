namespace QuoteAPI.Helpers
{
    public class QuoteDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string QuoteCollectionName { get; set; } = null!;
        public string UserCollectionName { get; set; } = null!;
    }
}
