using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserOfId(int id);

    Task<int> Register(User user);

    Task<User?> GetUserOfEmail(string emailAddress);

    Task<int> Update(User user);

    Task<bool> Exists(string email);
    
}