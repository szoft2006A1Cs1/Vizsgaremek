using JogsifoglaloApi.Model;

namespace JogsifoglaloApi.Interfaces
{
    public interface IInstructRepository
    {
        Task<IEnumerable<Instruct>> GetByInstructorIdAsync(int oktatId);
        Task AddAsync(Instruct instruct);
        void Delete(Instruct instruct);
        Task<bool> SaveChangesAsync();
    }
}
