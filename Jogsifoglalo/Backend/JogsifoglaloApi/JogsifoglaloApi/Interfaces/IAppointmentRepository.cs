using JogsifoglaloApi.Model;

namespace JogsifoglaloApi.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Appointment?>> GetAvailableByIdAsync(int oktatoId);
        Task AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(int id);
        void Update(Appointment appointment);
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<Appointment>> GetAvailableAsync();
        Task<Instruct?> GetInstructByIdAsync(int oktatId);
        Task<IEnumerable<Appointment>> GetByInstructorIdAsync(int oktatoId);
        Task<IEnumerable<Appointment>> GetByInstructorUserIdAsync(int felhasznloId);
        Task<IEnumerable<Appointment>> GetByStudentIdAsync(int tanuloId);
    }
}
