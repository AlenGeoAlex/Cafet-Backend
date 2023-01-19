using Cafet_Backend.Dto;
using Cafet_Backend.Dto.InputDtos;
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

    Task<User?> ResetPassword(User userOfId);

    Task<User?> TryRegister(RegistrationParam param);

    Task<List<EmailQueryDto>> GetEmailAddress(string emailSearch);

    Task<EmailQueryDto?> GetUserOfEmailAddress(string emailSearch);

    Task SaveAsync();

    Task<User?> UpdateUser(ProfileUpdate profileUpdate);

}