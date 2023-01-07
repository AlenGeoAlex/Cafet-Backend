using Cafet_Backend.Abstracts;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Cafet_Backend.Controllers;

public class RoleController : AbstractController
{

    private readonly IRoleRepository RoleRepository;

    public RoleController(IRoleRepository roleRepository)
    {
        RoleRepository = roleRepository;
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAll()
    {
        IReadOnlyList<Role> readOnlyList = await RoleRepository.GetRolesAsync();
        return Ok(readOnlyList);
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAll(int id)
    {
        Role? roleByIdAsync = await RoleRepository.GetRoleByIdAsync(id);
        if (roleByIdAsync != null)
            Ok(roleByIdAsync);
        
        
        return NotFound(roleByIdAsync);
    }
    
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetAll(string name)
    {
        Role? roleByIdAsync = await RoleRepository.GetRoleByNameAsync(name);
        if (roleByIdAsync != null)
            Ok(roleByIdAsync);
        
        
        return NotFound(roleByIdAsync);
    }
}