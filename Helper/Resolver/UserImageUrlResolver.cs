using AutoMapper;
using Cafet_Backend.Dto;
using Cafet_Backend.Models;

namespace Cafet_Backend.Helper;

public class UserImageUrlResolver : IValueResolver<User, UserDto, string>
{
    
    private readonly IConfiguration Config;
    
    public UserImageUrlResolver(IConfiguration configuration)
    {
        this.Config = configuration;
    }
    
    public string Resolve(User source, UserDto destination, string destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.ProfileImage))
            return null;
        
        return $"{Config["apiUrl"]}_images/_user/{source.ProfileImage}";

    }
}