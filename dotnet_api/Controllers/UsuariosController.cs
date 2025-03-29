using dotnet_api.Models;
using dotnet_api.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using dotnet_api.Shared.Helpers;
using dotnet_api.Shared.DTOs;

namespace dotnet_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController(IJWTService jwtService, UserManager<Usuario> userManager, RoleManager<GrupoUsuarios> roleManager, IConfiguration config) : ControllerBase
{
    private readonly IJWTService _jwtService = jwtService;
    private readonly UserManager<Usuario> _userManager = userManager;
    private readonly RoleManager<GrupoUsuarios> _roleManager = roleManager;
    private readonly IConfiguration _config = config;

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
        if (usuario is null || !await _userManager.CheckPasswordAsync(usuario, loginDTO.Password))
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
            new (ClaimTypes.Name, usuario.UserName!),
            new (ClaimTypes.NameIdentifier, usuario.Id)
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
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] RegisterDTO register)
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

        var resultUser = await _userManager.CreateAsync(usuario, register.Password);
        if (!resultUser.Succeeded)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Falha ao registrar usuário na plataforma",
                Errors = ErrorFormatters.FormatarErrosIdentity(resultUser),
                StackTrace = "API Authentication"
            });
        }

        var grupoExiste = await _roleManager.FindByIdAsync(register.GrupoUsuariosID);
        if (grupoExiste is null || grupoExiste.Name is null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Grupo de Usuários não encontrado ou inexistente na base de dados",
                Errors = ["Grupo de Usuários não encontrado ou inexistente na base de dados"],
                StackTrace = "API Authentication"
            });
        }

        var resultGrupo = await _userManager.AddToRoleAsync(usuario, grupoExiste.Name);
        
        if (!resultGrupo.Succeeded)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Grupo de Usuários não encontrado ou inexistente na base de dados",
                Errors = ["Grupo de Usuários não encontrado ou inexistente na base de dados"],
                StackTrace = "API Authentication"
            });
        }

        return Ok();
    }

    [HttpPost]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] RegisterDTO register)
    {
        if(register.Id is null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "ID do usuário não informado",
                Errors = ["ID do usuário não informado"],
                StackTrace = "API Authentication"
            });
        }

        var usuario = await _userManager.FindByIdAsync(register.Id);

        if (usuario == null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Usuário não encontrado na base de dados",
                Errors = ["Usuário não encontrado na base de dados"],
                StackTrace = "API Authentication"
            });
        }

        usuario.Email = register.Email;
        usuario.UserName = register.Username;
        usuario.PasswordHash = register.Password == null ? usuario.PasswordHash : _userManager.PasswordHasher.HashPassword(usuario, register.Password);

        var resultUser = await _userManager.UpdateAsync(usuario);
        if (!resultUser.Succeeded)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Falha ao atualizar usuário na plataforma",
                Errors = ErrorFormatters.FormatarErrosIdentity(resultUser),
                StackTrace = "API Authentication"
            });
        }

        var newGrupoExiste = await _roleManager.FindByIdAsync(register.GrupoUsuariosID);
        if (newGrupoExiste is null || newGrupoExiste.Name is null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Grupo de Usuários não encontrado ou inexistente na base de dados",
                Errors = ["Grupo de Usuários não encontrado ou inexistente na base de dados"],
                StackTrace = "API Authentication"
            });
        }

        var gruposVinculado = await _userManager.GetRolesAsync(usuario);
        if (gruposVinculado.FirstOrDefault() != newGrupoExiste.Id)
        {
            await _userManager.RemoveFromRolesAsync(usuario, gruposVinculado);
            await _userManager.AddToRoleAsync(usuario, newGrupoExiste.Name);
        }

        return Ok();
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
                Errors = ["Refresh Token inválido, Realize Login Novamente"],
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
                Errors = ["Usuário não encontrado"],
                StackTrace = "API Authentication"
            });
        }
        usuario.RefreshToken = null;
        usuario.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(usuario);

        return Ok(new { message = "Usuário Revogado com Sucesso" });
    }
}
