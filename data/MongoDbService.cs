using MongoDB.Driver;
using MyAssetsManagerBackend.Entities;

namespace MyAssetsManagerBackend.data;

public class MongoDbService
{
    private readonly IConfiguration _configuration;
    private readonly IMongoDatabase? _database;
    private readonly IMongoCollection<User> _users;
    
    public MongoDbService(IConfiguration configuration)
    {
        this._configuration = configuration;
        
        var mongoConnectionString = this._configuration.GetConnectionString("MongoDB");
        if (string.IsNullOrWhiteSpace(mongoConnectionString))
        {
            throw new InvalidOperationException("MongoDB connection string is missing.");
        }
        
        var mongoUrl = new MongoUrl(mongoConnectionString);
        var mongoClient = new MongoClient(mongoUrl);
        _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        _users = _database.GetCollection<User>("Users");
    }
    
    public IMongoDatabase? Database => _database;
    public IMongoCollection<User> Users => _users;
}