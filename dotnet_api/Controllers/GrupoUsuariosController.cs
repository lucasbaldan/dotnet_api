using System.Text.Json;
using dotnet_api.Models;
using dotnet_api.Shared.DTOs;
using dotnet_api.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using dotnet_api.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_api.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class GrupoUsuariosController : ControllerBase
{
    private readonly RoleManager<GrupoUsuarios> _roleManager;

    public GrupoUsuariosController(RoleManager<GrupoUsuarios> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] GrupoUsuariosDTO gu)
    {
        string? permissoes = null;

        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Erro ao cadastrar Grupo de Usuários na plataforma",
                Errors = ErrorFormatters.FormatarErrosModel(ModelState),
                StackTrace = "API - Grupo de Usuários"
            });
        }

        if (gu.Permissoes != null)
        {
            var permissoesValidas = gu.Permissoes
            .Where(p => Enum.IsDefined(typeof(PermissoesEnum), p))
            .Select(p => (PermissoesEnum)p)
            .ToList();

            if (permissoesValidas.Count != gu.Permissoes.Count)
            {
                return BadRequest(new ErrorResponse()
                {
                    StatusCode = 400,
                    Message = "Erro ao cadastrar Grupo de Usuários na plataforma",
                    Errors = ["Uma ou mais permissões informadas são inválidas na plataforma."],
                    StackTrace = "API - Grupo de Usuários"
                });
            }
            permissoes = JsonSerializer.Serialize(permissoesValidas);
        }

            var result = await _roleManager.CreateAsync(new GrupoUsuarios()
            {
                Ativo = gu.Ativo,
                Name = gu.Nome,
                Descricao = gu.Descricao,
                PermissoesJson = permissoes
            });

        if (!result.Succeeded)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Erro ao cadastrar Grupo de Usuários na plataforma",
                Errors = ErrorFormatters.FormatarErrosIdentity(result),
                StackTrace = "API"
            });
        }

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, [FromBody] GrupoUsuariosDTO gu)
    {
        string? permissoes = null;

        if (gu == null || gu.Id == null || !ModelState.IsValid || gu.Id != id)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Erro ao atualizar Grupo de Usuários na plataforma",
                Errors = ["Erro ao recuperar informações enviadas para a API. Contate o Administrador"],
                StackTrace = "API"
            });
        }

        if (gu.Permissoes != null)
        {
            var permissoesValidas = gu.Permissoes
            .Where(p => Enum.IsDefined(typeof(PermissoesEnum), p))
            .Select(p => (PermissoesEnum)p)
            .ToList();

            if (permissoesValidas.Count != gu.Permissoes.Count)
            {
                return BadRequest(new ErrorResponse()
                {
                    StatusCode = 400,
                    Message = "Erro ao cadastrar Grupo de Usuários na plataforma",
                    Errors = ["Uma ou mais permissões informadas são inválidas na plataforma."],
                    StackTrace = "API - Grupo de Usuários"
                });
            }
            permissoes = JsonSerializer.Serialize(permissoesValidas);
        }

        var grupoExists = await _roleManager.FindByIdAsync(gu.Id);

        if (grupoExists == null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "O grupo de usuário não foi encontrado na base de dados",
                Errors = ["O grupo de usuário não foi encontrado na base de dados"],
                StackTrace = "API Authentication"
            });
        }

        grupoExists.Ativo = gu.Ativo;
        grupoExists.Name = gu.Nome;
        grupoExists.PermissoesJson = permissoes;
        grupoExists.Descricao = gu.Descricao;

        var result = await _roleManager.UpdateAsync(grupoExists);

        if (!result.Succeeded)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Erro ao atualizar Grupo de Usuários na plataforma",
                Errors = ErrorFormatters.FormatarErrosIdentity(result),
                StackTrace = "API"
            });
        }
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var grupoExists = await _roleManager.FindByIdAsync(id.ToString());

        if (grupoExists == null)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "O grupo de usuário não foi encontrado na base de dados",
                Errors = ["O grupo de usuário não foi encontrado na base de dados"],
                StackTrace = "API Authentication"
            });
        }

        var result = await _roleManager.DeleteAsync(grupoExists);

        if (!result.Succeeded)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 400,
                Message = "Erro ao deletar Grupo de Usuários na plataforma",
                Errors = ErrorFormatters.FormatarErrosIdentity(result),
                StackTrace = "API"
            });
        }

        return Ok();
    }
}
