using Cafet_Backend.Abstracts;
using Cafet_Backend.Interfaces;

namespace Cafet_Backend.Controllers;

public class AuthController : AbstractController
{

    private readonly IAuthenticationRepository AuthenticationRepository;

    public AuthController(IAuthenticationRepository authenticationRepository)
    {
        AuthenticationRepository = authenticationRepository;
    }
}