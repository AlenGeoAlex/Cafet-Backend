using Cafet_Backend.Context;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;

namespace Cafet_Backend;

public class AuthenticationRepository : IAuthenticationRepository
{

    private readonly CafeContext CafeContext;

    public AuthenticationRepository(CafeContext cafeContext)
    {
        CafeContext = cafeContext;
    }

    public async Task<User?> GetUserOfId(int id)
    {
        User? user = await CafeContext.Users.FindAsync(id);
        return user;
    }

    public async Task<int> Register(User user)
    {
        CafeContext.Users.Add(user);
        return await CafeContext.SaveChangesAsync();
    }

    public async Task<int> Update(User user)
    {
        CafeContext.Users.Update(user);
        return await CafeContext.SaveChangesAsync();
    }
}