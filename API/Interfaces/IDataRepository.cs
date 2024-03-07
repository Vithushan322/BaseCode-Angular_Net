using System.Linq.Expressions;

namespace API.Interfaces
{
    public interface IDataRepository<T>
    {
        void Add(params T[] items);

        void Update(params T[] items);

        void Remove(params T[] items);

        Task<IEnumerable<T>> GetAll();

        Task<T> GetItemById(int id);

        Task<bool> SaveAllAsync();
    }
}