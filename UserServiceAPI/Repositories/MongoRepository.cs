using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserService.Configurations;
using MongoDB.Bson;

namespace UserService.Repositories
{
    public class MongoRepository<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoClient client, IOptions<MongoDbSettings> options)
        {
            var database = client.GetDatabase(options.Value.DatabaseName);
            _collection = database.GetCollection<T>(options.Value.UsersCollection);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> GetByUsernameAsync(string username)
        {
            return await _collection
                .Find(Builders<T>.Filter.Eq("Username", username))
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(ObjectId id, T entity)
        {
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), entity);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
        }
    }
}
