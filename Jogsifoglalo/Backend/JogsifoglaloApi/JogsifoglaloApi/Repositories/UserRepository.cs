using JogsifoglaloApi.Data;
using JogsifoglaloApi.Interfaces;
using JogsifoglaloApi.Model;
using Microsoft.EntityFrameworkCore;

namespace JogsifoglaloApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly JogsifoglaloContext _context;

        public UserRepository(JogsifoglaloContext context)
        {
            _context = context;
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync(); 
        }
        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }
        public void Update(User user)
        {
            _context.Users.Update(user);
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
