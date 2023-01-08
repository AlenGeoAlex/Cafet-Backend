using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface IAuthenticationRepository
{
    Task<User?> GetUserOfId(int id);

    Task<int> Register(User user);

    Task<int> Update(User user);
}