using dotnet_api.DTOs;
using dotnet_api.Models;
using dotnet_api.Helpers;
using dotnet_api.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly IJWTService _jwtService;
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;

    public UsuariosController(IJWTService jwtService, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
    {

        if(!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Erro ao efetuar login",
                Errors = ErrorFormatters.FormatarErrosModel(ModelState),
                StackTrace = "API Authentication"
            });
        }

        var usuario = await _userManager.FindByNameAsync(loginDTO.Username);
        if (usuario is null || await _userManager.CheckPasswordAsync(usuario, loginDTO.Password))
        {
            return Unauthorized(new ErrorResponse()
            {
                StatusCode = 401,
                Message = "Usuário ou senha inválidos",
                Errors = ["Usuário ou senha inválidos"],
                StackTrace = "API Authentication"
            });
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.UserName!),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id)
        };

        var token = _jwtService.GerarToken(claims, _config);

        var refreshToken = _jwtService.GenerateRefreshToken();

        _ = int.TryParse(_config.GetSection("JWT").GetValue<string>("RefreshExpiration"), out int refreshExpiration);
        _ = int.TryParse(_config.GetSection("JWT").GetValue<string>("AccessExpiration"), out int tokenExpiration);

        usuario.RefreshToken = refreshToken;
        usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshExpiration);

        await _userManager.UpdateAsync(usuario);

        return Ok(new
        {
            acessToken = new JwtSecurityTokenHandler().WriteToken(token),
            expirationAt = DateTime.UtcNow.AddMinutes(tokenExpiration),
            refreshToken = usuario.RefreshToken
        });
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDTO register)
    {
        var userExists = await _userManager.FindByEmailAsync(register.Email!);

        if (userExists != null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Email já cadastrado para outro usuário na plataforma",
                Errors = ["Email já cadastrado para outro usuário na plataforma"],
                StackTrace = "API Authentication"
            });
        }

        Usuario usuario = new()
        {
            Email = register.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = register.Username
        };

        var result = await _userManager.CreateAsync(usuario, register.Password);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Falha ao registrar usuário na plataforma",
                Errors = ErrorFormatters.FormatarErrosIdentity(result),
                StackTrace = "API Authentication"
            });
        }
        return Ok(new { message = "Usuário criado com sucesso" });
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenDTO refresh)
    {
        if (refresh is null || refresh.AcessToken is null || refresh.RefreshToken is null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Token de atualização inválido!",
                Errors = ["Token de atualização inválido"],
                StackTrace = "API Authentication"
            });
        }

        var data = _jwtService.GetDataFromExpiredToken(refresh.AcessToken, _config);
        string username = data.Identity!.Name!;

        if (username is null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Token de atualização inválido!",
                Errors= ["Bad Request"],
                StackTrace = "API Authentication"
            });
        }

        var usuario = await _userManager.FindByNameAsync(username);

        if (usuario == null || usuario.RefreshToken != refresh.RefreshToken || usuario.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Token de atualização inválido",
                Errors = ["JWT inválido"],
                StackTrace = "API Authentication"
            });
        }

        var novoAcessToken = _jwtService.GerarToken(data.Claims.ToList(), _config);
        var novoRefreshToken = _jwtService.GenerateRefreshToken();

        await _userManager.UpdateAsync(usuario);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(novoAcessToken), refreshToken = novoRefreshToken });

    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{username:alpha}")]
    public async Task<ActionResult> Revoke(string username)
    {
        var usuario = await _userManager.FindByNameAsync(username);
        if (usuario is null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Usuário não encontrado",
                Errors = ["Bad Request"],
                StackTrace = "API Authentication"
            });
        }
        usuario.RefreshToken = null;
        usuario.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(usuario);

        return Ok(new { message = "Usuário Revogado com Sucesso" });
    }
}
