using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuoteAPI.Models;
using System.Diagnostics;

namespace QuoteAPI.Services
{
    public class QuoteService
    {
        private readonly IMongoCollection<Quote> _quoteCollection;
        private string? HerokuConnectionString { get; set; }
        private string? HerokuDatabaseName { get; set; }
        private string? HerokuCollectionName { get; set; }
        private QuoteModelFactory _quoteModelFactory { get; set; }

        public QuoteService(
            IOptions<QuoteDBSettings> quoteDbSettings, 
            IOptions<QuoteDBCredentials> quoteDbCredentials,
            QuoteModelFactory quoteModelFactory)
        {
            SetHerokuEnvVars();

            MongoClient mongoClient = new(HerokuConnectionString ?? 
                ConfigureDbConnection(quoteDbSettings.Value.ConnectionString, quoteDbCredentials));

            IMongoDatabase mongoDb = mongoClient.GetDatabase(HerokuDatabaseName ?? quoteDbSettings.Value.DatabaseName);

            _quoteCollection = mongoDb.GetCollection<Quote>(HerokuCollectionName ?? quoteDbSettings.Value.QuoteCollectionName);

            _quoteModelFactory = quoteModelFactory;
        }

        public async Task<List<Quote>> GetQuotesAsync() => 
            await _quoteCollection.Find(_ => true).ToListAsync();

        public async Task<Quote> GetQuoteByIdAsync(string id) =>
            await _quoteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Quote> CreateQuoteAsync(QuoteCreationModel creationModel)
        {
            Quote quote = _quoteModelFactory.ConvertToQuote(creationModel);

            await _quoteCollection.InsertOneAsync(quote);

            return quote;
        }

        public async Task<Quote?> UpdateQuoteAsync(string id, QuoteUpdateModel updatedQuote)
        {
            Quote quote = await _quoteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (quote == null)
            {
                return null;
            }

            quote = _quoteModelFactory.ConvertToQuote(updatedQuote, quote);
            await _quoteCollection.ReplaceOneAsync(x => x.Id == id, quote);

            return quote;
        }

        public async Task<Quote?> RemoveQuoteAsync(string id)
        {
            Quote quote = await _quoteCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (quote == null)
                return null;
            
            await _quoteCollection.DeleteOneAsync(x => x.Id == id);
            return quote;
        }

        private static string ConfigureDbConnection(string baseConnectionString, IOptions<QuoteDBCredentials> dbCredentials)
        {
            return baseConnectionString
                .Replace("<username>", dbCredentials.Value.DBUsername)
                .Replace("<password>", dbCredentials.Value.DBPassword)
                .Replace("@", $"@{dbCredentials.Value.DBEndString}");
        }

        private void SetHerokuEnvVars()
        {
            HerokuConnectionString = Environment.GetEnvironmentVariable("connectionString");
            HerokuDatabaseName = Environment.GetEnvironmentVariable("databaseName");
            HerokuCollectionName = Environment.GetEnvironmentVariable("quoteCollectionName");
        }
    }
}
