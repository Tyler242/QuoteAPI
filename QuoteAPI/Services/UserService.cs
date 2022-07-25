using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuoteAPI.Helpers;
using QuoteAPI.Models;
using System.Security.Cryptography;
using System.Text;

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

        public async Task<UserDTO?> ValidateUser(UserLogin userLogin)
        {
            UserDTO? user = await _users.Find(x => x.UserName! == userLogin.UserName).FirstOrDefaultAsync();
            if (user == null)
                return null;

            if (VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
                return user;
            else
                return null;
        }

        public async Task<bool> AddUser(UserModel userModel)
        {
            if (_users.Find(x => x.UserName! == userModel.UserName!).Any())
                return false;

            CreatePasswordHash(userModel.Password, out byte[] passwordHash, out byte[] passwordSalt);

            await _users.InsertOneAsync(new UserDTO
            {
                UserName = userModel.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                PhoneNumber = userModel.PhoneNumber,
                Role = "User"
            });

            return true;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
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
