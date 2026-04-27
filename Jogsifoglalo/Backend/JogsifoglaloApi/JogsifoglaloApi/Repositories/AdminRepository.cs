using JogsifoglaloApi.Data;
using JogsifoglaloApi.Dto;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model.Enum;
using Microsoft.EntityFrameworkCore;

namespace JogsifoglaloApi.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly JogsifoglaloContext _context;

        public AdminRepository(JogsifoglaloContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserAdminResponseDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserAdminResponseDto
                {
                    Felhasznalo_Id = u.Felhasznalo_Id,
                    Nev = u.Nev,
                    Email = u.Email,
                    Cim = u.Cim,
                    Telefonszam = u.Telefonszam,
                    Szerepkor_Nev = u.Szerepkor_Nev.ToString()
                }).ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserAdminDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Nev = dto.Nev;
            user.Email = dto.Email;
            user.Cim = dto.Cim;
            user.Telefonszam = dto.Telefonszam;

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ResetPasswordAsync(int id, string hashedNewPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Jelszo = hashedNewPassword;
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<AppointmentAdminResponseDto>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Oktat!).ThenInclude(o => o.Oktato!).ThenInclude(u => u.Felhasznalo)
                .Include(a => a.Tanulo)
                .Select(a => new AppointmentAdminResponseDto
                {
                    Idopont_Id = a.Idopont_Id,
                    OktatoNeve = a.Oktat!.Oktato!.Felhasznalo!.Nev,
                    TanuloNeve = a.Tanulo != null ? a.Tanulo.Nev : "Nincs foglalva",
                    Kezdes_Dt = a.Kezdes_Dt,
                    Idotartam = a.Idotartam,
                    Ar = a.Ar,
                    Allapot = a.Allapot.ToString(),
                    Megjegyzes = a.Megjegyzes
                })
                .OrderBy(a => a.Idopont_Id)
                .ToListAsync();
        }

        public async Task<bool> UpdateAppointmentAsync(int id, UpdateAppointmentAdminDto dto)
        {
            var app = await _context.Appointments.FindAsync(id);
            if (app == null) return false;

            app.Kezdes_Dt = dto.Kezdes_Dt;
            app.Idotartam = (ushort)dto.Idotartam;
            app.Ar = dto.Ar;
            app.Megjegyzes = dto.Megjegyzes;

            if (Enum.TryParse<Status>(dto.Allapot, out var status))
            {
                app.Allapot = status;
            }

            _context.Appointments.Update(app);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            _context.Appointments.Remove(appointment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<AdminDashboardDto> GetDashboardStatsAsync()
        {
            return new AdminDashboardDto
            {
                OsszesFelhasznalo = await _context.Users.CountAsync(),
                AktivOktatokSzama = await _context.Users.CountAsync(u => u.Szerepkor_Nev == Role.oktato),
                OsszesBevetel = await _context.Pays.SumAsync(f => f.Osszeg),
                MaiIdopontokSzama = await _context.Appointments.CountAsync(a => a.Kezdes_Dt.Date == DateTime.Today)
            };
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
