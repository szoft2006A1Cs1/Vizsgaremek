using JogsifoglaloApi.Data;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using JogsifoglaloApi.Model.Enum;
using Microsoft.EntityFrameworkCore;

namespace JogsifoglaloApi.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly JogsifoglaloContext _context;

        public AppointmentRepository(JogsifoglaloContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                    .Include(a => a.Oktat!).ThenInclude(o => o.Oktato!).ThenInclude(u => u.Felhasznalo).Include(a => a.Oktat!)
                    .ThenInclude(o => o.Kategoria).Include(a => a.Tanulo).OrderBy(a => a.Idopont_Id).ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments.Include(a => a.Oktat!).ThenInclude(o => o.Oktato).FirstOrDefaultAsync(a => a.Idopont_Id == id);
        }

        public async Task<IEnumerable<Appointment?>> GetAvailableByIdAsync(int oktatoId)
        {
            return await _context.Appointments
                .Where(a => a.Oktat!.Oktato_Id == oktatoId && a.Allapot == Status.szabad).ToListAsync();
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }
        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }
        }
        public void Update(Appointment appointment)
        {
             _context.Appointments.Update(appointment);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Appointment>> GetAvailableAsync()
        {
            return await _context.Appointments.Include(a => a.Oktat!).ThenInclude(o => o.Oktato!).ThenInclude(f => f.Felhasznalo!).Include(a => a.Oktat!).ThenInclude(k => k.Kategoria)
                                               .Where(a => a.Allapot == Status.szabad).ToListAsync();
        }
        public async Task<Instruct?> GetInstructByIdAsync(int oktatId)
        {
            return await _context.Instructs.Include(i => i.Oktato).FirstOrDefaultAsync(i => i.Oktat_Id == oktatId);
        }

        public async Task<IEnumerable<Appointment>> GetByInstructorIdAsync(int oktatoId)
        {
            return await _context.Appointments
                .Include(o => o.Oktat).Where(o => o.Oktat!.Oktato_Id == oktatoId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByInstructorUserIdAsync(int felhasznaloId)
        {
            return await _context.Appointments
                .Include(a => a.Oktat!).ThenInclude(o => o.Oktato!).ThenInclude(f => f.Felhasznalo).Include(a => a.Oktat!).ThenInclude(o => o.Kategoria).Include(a => a.Tanulo!).Where(a => a.Oktat!.Oktato!.Felhasznalo_Id == felhasznaloId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByStudentIdAsync(int tanuloId)
        {
            return await _context.Appointments
                .Where(a => a.Tanulo_Id == tanuloId).Include(a => a.Oktat!).ThenInclude(o => o.Oktato!).ThenInclude(u => u.Felhasznalo).Include(a => a.Oktat!).ThenInclude(o => o.Kategoria).ToListAsync();
        }
    }
}
