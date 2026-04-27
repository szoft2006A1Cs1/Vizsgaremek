using JogsifoglaloApi.Model;

namespace JogsifoglaloApi.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
