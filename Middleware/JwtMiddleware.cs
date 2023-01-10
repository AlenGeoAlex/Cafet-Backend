using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Cafet_Backend.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        await _next(context);
    }
}