using Cafet_Backend.Models;

namespace Cafet_Backend.Interfaces;

public interface IRoleRepository
{

    Task<Role?> GetRoleByIdAsync(int id);

    Task<Role?> GetRoleByNameAsync(string name);

    Task<IReadOnlyList<Role>> GetRolesAsync();

}