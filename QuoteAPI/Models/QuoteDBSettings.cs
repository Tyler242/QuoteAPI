namespace QuoteAPI.Models
{
    public class QuoteDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string QuoteCollectionName { get; set; } = null!;
    }
}
