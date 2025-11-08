using Api.Domain;
using Api.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Api.Repositories;

public class MongoRepository<T> : IRepository<T> where T : User
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoClient client, MongoDbSettings settings)
    {
        var db = client.GetDatabase(settings.DatabaseName);
        string tableName = typeof(T).Name.ToLower();
        _collection = db.GetCollection<T>(tableName);
    }
    public async Task<List<T>> GetAllAsync()
    {
        return await _collection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _collection.Find(doc => doc.Id == id).FirstOrDefaultAsync();
    }

    public async Task<T> AddAsync(T item)
    {
        await _collection.InsertOneAsync(item);
        return item;
    }
    
    public async Task<T> UpdateAsync(T item)
    {
        item.LastChangedAt = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(doc => doc.Id == item.Id, item, 
            new ReplaceOptions(){IsUpsert = true});
        return item;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(doc => doc.Id == id);
    }
}