using JogsifoglaloApi.Dto;

namespace JogsifoglaloApi.Interfaces
{
    public interface IAdminRepository
    {
        Task<IEnumerable<UserAdminResponseDto>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(int id, UpdateUserAdminDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ResetPasswordAsync(int id, string newPassword);
        Task<IEnumerable<AppointmentAdminResponseDto>> GetAllAppointmentsAsync();
        Task<bool> UpdateAppointmentAsync(int id, UpdateAppointmentAdminDto dto);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<AdminDashboardDto> GetDashboardStatsAsync();
    }
}
