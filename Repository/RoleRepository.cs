using Cafet_Backend.Context;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafet_Backend.Repository;

public class RoleRepository : IRoleRepository
{

    private readonly CafeContext CafeContext;

    public RoleRepository(CafeContext cafeContext)
    {
        CafeContext = cafeContext;
    }

    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        return await CafeContext.Roles.FindAsync(id);
    }

    public async Task<Role?> GetRoleByNameAsync(string name)
    {
        return await CafeContext.Roles
            .FirstOrDefaultAsync(role => role.RoleName == name);
    }

    public async Task<IReadOnlyList<Role>> GetRolesAsync()
    {
        return await CafeContext.Roles.ToListAsync();
    }
}