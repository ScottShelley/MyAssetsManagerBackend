using MongoDB.Driver;
using MyAssetsManagerBackend.Entities;

namespace MyAssetsManagerBackend.data;

public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(MongoDbService mongoDbService)
    {
        _users = mongoDbService.Database?.GetCollection<User>("Users");
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task CreateUserAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }
}