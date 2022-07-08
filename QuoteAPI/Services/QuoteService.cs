using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuoteAPI.Models;
using System.Diagnostics;

namespace QuoteAPI.Services
{
    public class QuoteService
    {
        private readonly IMongoCollection<Quote> _quoteCollection;

        public QuoteService(
            IOptions<QuoteDBSettings> quoteDbSettings, 
            IOptions<QuoteDBCredentials> quoteDbCredentials)
        {
            MongoClient mongoClient = new(ConfigureDbConnection(quoteDbSettings.Value.ConnectionString, quoteDbCredentials));

            IMongoDatabase mongoDb = mongoClient.GetDatabase(quoteDbSettings.Value.DatabaseName);

            _quoteCollection = mongoDb.GetCollection<Quote>(quoteDbSettings.Value.QuoteCollectionName);
        }

        public async Task<List<Quote>> GetQuotesAsync() => 
            await _quoteCollection.Find(_ => true).ToListAsync();

        public async Task<Quote> GetQuoteByIdAsync(string id) =>
            await _quoteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateQuoteAsync(Quote newQuote) =>
            await _quoteCollection.InsertOneAsync(newQuote);

        public async Task UpdateQuoteAsync(string id, Quote updatedQuote) =>
            await _quoteCollection.ReplaceOneAsync(x => x.Id == id, updatedQuote);

        public async Task RemoveQuoteAsync(string id) =>
            await _quoteCollection.DeleteOneAsync(x => x.Id == id);

        private static string ConfigureDbConnection(string baseConnectionString, IOptions<QuoteDBCredentials> dbCredentials)
        {
            return baseConnectionString
                .Replace("<username>", dbCredentials.Value.DBUsername)
                .Replace("<password>", dbCredentials.Value.DBPassword)
                .Replace("@", $"@{dbCredentials.Value.DBEndString}");
        }
    }
}
