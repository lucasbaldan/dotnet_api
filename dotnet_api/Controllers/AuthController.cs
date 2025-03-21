using dotnet_api.Models;
using dotnet_api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AuthController(JWTService jwtService, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }
    }
}
