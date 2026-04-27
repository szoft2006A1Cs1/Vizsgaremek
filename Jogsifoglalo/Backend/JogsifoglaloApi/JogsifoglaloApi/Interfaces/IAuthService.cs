using JogsifoglaloApi.Model;

namespace JogsifoglaloApi.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(User user, string password);
        Task<string?> LoginAsync(string email, string password);
        string CreateToken(User user);
    }
}
