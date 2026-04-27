using JogsifoglaloApi.Model;

namespace JogsifoglaloApi.Interfaces
{
    public interface IInstructorRepository
    {
        Task<IEnumerable<Instructor>> GetAllAsync();
        Task<Instructor?> GetByIdAsync(int id);
        Task<bool> HasActiveAppointmentsAsync(int instructorId);
        void Update(Instructor instructor);
        Task<Category?> GetCategoryByNameOrCodeAsync(string search);
        Task<IEnumerable<Instructor?>> GetInstructorsByCategoryAsync(string categoryCode);
        void RemoveCategories(Instructor instructor, List<string> newCategoryCodes);
        Task<bool> DeleteInstructorAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
