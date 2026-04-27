using JogsifoglaloApi.Data;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using JogsifoglaloApi.Model.Enum;
using Microsoft.EntityFrameworkCore;

namespace JogsifoglaloApi.Repositories
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly JogsifoglaloContext _context;

        public InstructorRepository(JogsifoglaloContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Instructor>> GetAllAsync()
        {
            return await _context.Instructors.Include(i => i.Felhasznalo).Include(i => i.Instructs).ThenInclude(ins => ins.Kategoria).ToListAsync();
        }

        public async Task<Instructor?> GetByIdAsync(int id)
        {
            return await _context.Instructors.Include(i => i.Felhasznalo).Include(i => i.Instructs).ThenInclude(ins => ins.Kategoria).FirstOrDefaultAsync(i => i.Oktato_Id == id);
        }

        public async Task<bool> HasActiveAppointmentsAsync(int instructorId)
        {
            return await _context.Appointments.AnyAsync(a => a.Oktat!.Oktato_Id == instructorId && (a.Allapot == Status.foglalt || a.Kezdes_Dt > DateTime.Now));
        }

        public void Update(Instructor instructor)
        {
            _context.Instructors.Update(instructor);
        }

        public async Task<IEnumerable<Instructor?>> GetInstructorsByCategoryAsync(string categoryCode)
        {
            return await _context.Instructors.Include(o => o.Felhasznalo).Include(o => o.Instructs).ThenInclude(ins => ins.Kategoria)
                .Where(o => o.Instructs.Any(ins => ins.Kategoria != null && ins.Kategoria.Kategoria_Kod.ToLower() == categoryCode.ToLower())).ToListAsync();
        }

        public async Task<Category?> GetCategoryByNameOrCodeAsync(string search)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(k => k.Kategoria_Nev == search || k.Kategoria_Kod == search);
        }

        public void RemoveCategories(Instructor instructor, List<string> newCategoryCodes)
        {
            var categoriesToRemove = instructor.Instructs
                .Where(ins => !newCategoryCodes.Contains(ins.Kategoria?.Kategoria_Kod ?? ""))
                .ToList();

            foreach (var remove in categoriesToRemove)
            {
                _context.Instructs.Remove(remove);
            }
        }

        public async Task<bool> DeleteInstructorAsync(int id)
        {
            var instructor = await _context.Instructors
                    .Include(i => i.Instructs) 
                    .Include(i => i.Felhasznalo) 
                    .FirstOrDefaultAsync(i => i.Oktato_Id == id);

            if (instructor == null) return false;

            _context.Instructs.RemoveRange(instructor.Instructs);

            _context.Instructors.Remove(instructor);

            if (instructor.Felhasznalo != null)
            {
                _context.Users.Remove(instructor.Felhasznalo);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        } 
    }
}
