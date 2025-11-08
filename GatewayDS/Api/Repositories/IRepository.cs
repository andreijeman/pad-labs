using Api.Domain;

namespace Api.Repositories;

public interface IRepository<T> 
    where T : MongoDocument
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T> AddAsync(T item);
    Task<T> UpdateAsync(T item);
    Task DeleteAsync(Guid id);
}