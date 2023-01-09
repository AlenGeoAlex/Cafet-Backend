using Cafet_Backend.Context;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend;

public class UserRepository : IUserRepository
{

    private readonly CafeContext CafeContext;

    public UserRepository(CafeContext cafeContext)
    {
        CafeContext = cafeContext;
    }

    public async Task<User?> GetUserOfId(int id)
    {
        User? user = await CafeContext.Users
            .Include(u => u.Cart )
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }

    public async Task<int> Register(User user)
    {
        CafeContext.Users.Add(user);
        return await CafeContext.SaveChangesAsync();
    }

    public Task<User?> GetUserOfEmail(string emailAddress)
    {
        return CafeContext.Users
            .Include(u => u.Cart )
            .Include(u => u.Role)
            .FirstOrDefaultAsync(user => user.EmailAddress.ToLower() == emailAddress);
    }

    public async Task<int> Update(User user)
    {
        CafeContext.Users.Update(user);
        return await CafeContext.SaveChangesAsync();
    }

    public async Task<bool> Exists(string email)
    {
        return await CafeContext.Users.AnyAsync(user => user.EmailAddress.ToLower() == email.ToLower());
    }
    
}