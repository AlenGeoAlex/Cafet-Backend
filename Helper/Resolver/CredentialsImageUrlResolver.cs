using AutoMapper;
using Cafet_Backend.Dto;
using Cafet_Backend.Models;

namespace Cafet_Backend.Helper;

public class CredentialsImageUrlResolver : IValueResolver<User, CredentialsDto, string>
{
    
    private readonly IConfiguration Config;
    
    public CredentialsImageUrlResolver(IConfiguration configuration)
    {
        this.Config = configuration;
    }
    
    public string Resolve(User source, CredentialsDto destination, string destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.ProfileImage))
            return null;
        
        return $"{Config["apiUrl"]}_images/_user/{source.ProfileImage}";

    }
}