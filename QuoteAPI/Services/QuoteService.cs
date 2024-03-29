﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuoteAPI.Helpers;
using QuoteAPI.Models;

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

        public List<Quote> GetQuotes(string userId, string? word = null, string? source = null, string? tag = null)
        {
            IQueryable<Quote> quotes = _quoteCollection.AsQueryable();
            quotes = quotes.Where(x => x.UserId == userId);

            if (word != null)
            {
                word = word.Trim();
                quotes = quotes.Where(x => x.QuoteText!.Contains(word));
            }

            if (source != null)
            {
                source = source.Trim();
                quotes = quotes.Where(x => x.QuoteSource!.Equals(source));
            }

            if (tag != null)
            {
                tag = tag.Trim();
                quotes = quotes.Where(x => x.Tags!.Contains(tag));
            }

            return quotes.ToList();
        }

        public async Task<Quote> GetQuoteByIdAsync(string userId, string id) =>
            await _quoteCollection.Find(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync();

        public async Task<Quote> CreateQuoteAsync(QuoteCreationModel creationModel, string userId)
        {
            creationModel.UserId = userId;
            Quote quote = _quoteModelFactory.ConvertToQuote(creationModel);

            await _quoteCollection.InsertOneAsync(quote);

            return quote;
        }

        public async Task<Quote?> UpdateQuoteAsync(string userId, string id, QuoteUpdateModel updatedQuote)
        {
            Quote quote = await _quoteCollection.Find(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync();

            if (quote == null)
            {
                return null;
            }

            quote = _quoteModelFactory.ConvertToQuote(updatedQuote, quote);
            await _quoteCollection.ReplaceOneAsync(x => x.Id == id, quote);

            return quote;
        }

        public async Task<Quote?> RemoveQuoteAsync(string userId, string id)
        {
            Quote quote = await _quoteCollection.Find(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync();
            if (quote == null)
                return null;
            
            await _quoteCollection.DeleteOneAsync(x => x.Id == id);
            return quote;
        }

        public List<Quote> AdminGetAllQuotes(string? word = null, string? source = null, string? tag = null, string? userId = null)
        {
            IQueryable<Quote> quotes = _quoteCollection.AsQueryable();

            if (word != null)
            {
                word = word.Trim();
                quotes = quotes.Where(x => x.QuoteText!.Contains(word));
            }

            if (source != null)
            {
                source = source.Trim();
                quotes = quotes.Where(x => x.QuoteSource!.Equals(source));
            }

            if (tag != null)
            {
                tag = tag.Trim();
                quotes = quotes.Where(x => x.Tags!.Contains(tag));
            }

            if (userId != null)
            {
                userId = userId.Trim();
                quotes = quotes.Where(x => x.UserId == userId);
            }

            return quotes.ToList();
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
