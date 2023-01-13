using Cafet_Backend.Dto;
using Cafet_Backend.Models;
using Cafet_Backend.QueryParams;

namespace Cafet_Backend.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserOfId(int id);

    Task<int> Register(User user);

    Task<User?> GetUserOfEmail(string emailAddress);

    Task<int> Update(User user);

    Task<bool> Exists(string email);

    Task<List<User>> GetAllUser();
    
    Task<List<User>> GetActiveUsers();

    Task<bool> DeleteUser(int id);

    Task<User?> ResetPassword(int id);

    Task<User?> TryRegister(RegistrationParam param);

    Task SaveAsync();

}