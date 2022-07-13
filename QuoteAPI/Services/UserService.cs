using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuoteAPI.Helpers;
using QuoteAPI.Models;

namespace QuoteAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<UserDTO> _users;
        private string? HerokuConnectionString { get; set; }
        private string? HerokuDatabaseName { get; set; }
        private string? HerokuUserCollectionName { get; set; }

        public UserService(
            IOptions<QuoteDBSettings> quoteDbSettings,
            IOptions<QuoteDBCredentials> quoteDbCredentials)
        {
            SetHerokuEnvVars();

            MongoClient mongoClient = new(HerokuConnectionString ??
                ConfigureDbConnection(quoteDbSettings.Value.ConnectionString, quoteDbCredentials));

            IMongoDatabase mongoDb = mongoClient.GetDatabase(HerokuDatabaseName ?? quoteDbSettings.Value.DatabaseName);

            _users = mongoDb.GetCollection<UserDTO>(HerokuUserCollectionName ?? quoteDbSettings.Value.UserCollectionName);
        }

        public async Task<UserDTO>? GetUser(UserModel userModel) =>
            await _users.Find(x => x.UserName! == userModel.UserName! && x.Password! == userModel.Password!).FirstOrDefaultAsync();

        public async Task<bool> AddUser(UserModel userModel)
        {
            if (_users.Find(x => x.UserName! == userModel.UserName!).Any())
            {
                return false;
            }

            await _users.InsertOneAsync(new UserDTO
            {
                UserName = userModel.UserName,
                Password = userModel.Password,
                PhoneNumber = userModel.PhoneNumber,
                Role = "User"
            });

            return true;
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
            HerokuUserCollectionName = Environment.GetEnvironmentVariable("userCollectionName");
        }
    }
}
