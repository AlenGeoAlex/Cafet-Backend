using Cafet_Backend.Abstracts;
using Cafet_Backend.Interfaces;
using Cafet_Backend.Models;
using Cafet_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cafet_Backend.Controllers;    

public class RoleController : AbstractController
{

    private readonly IRoleRepository RoleRepository;

    public RoleController(IRoleRepository roleRepository)
    {
        RoleRepository = roleRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IReadOnlyList<Role> readOnlyList = await RoleRepository.GetRolesAsync();
        return Ok(readOnlyList);
    }
    
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Role), 200)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
    public async Task<IActionResult> GetAll(int id)
    {
        Role? roleByIdAsync = await RoleRepository.GetRoleByIdAsync(id);
        Console.WriteLine(JsonConvert.SerializeObject(roleByIdAsync));
        if (roleByIdAsync == null)
            NotFound();
        
        
        return Ok();
    }
    
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(Role), 200)]
    [ProducesResponseType(typeof(NotFoundObjectResult), 404)]
    public async Task<IActionResult> GetAll(string name)
    {
        Role? roleByIdAsync = await RoleRepository.GetRoleByNameAsync(name);
        if (roleByIdAsync == null)
            NotFound();
        
        
        return Ok();
    }
}