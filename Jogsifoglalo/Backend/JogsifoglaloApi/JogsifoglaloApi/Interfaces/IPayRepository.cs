using JogsifoglaloApi.Model;

namespace JogsifoglaloApi.Interfaces
{
    public interface IPayRepository
    {
        Task AddAsync(Pay pay);
        Task<IEnumerable<Pay>> GetByUserIdAsync(int felhasznaloId);
        Task<Pay?> GetByIdAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
