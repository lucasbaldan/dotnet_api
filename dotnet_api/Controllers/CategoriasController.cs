using AutoMapper;
using Asp.Versioning;
using dotnet_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using dotnet_api.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using dotnet_api.Shared.Utilities;
using dotnet_api.Shared.Utilities.FilterClasses;
using dotnet_api.Shared.DTOs;
using dotnet_api.Shared.Filters;
using dotnet_api.Shared.Enums;

namespace dotnet_api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    //[Authorize]
    [EnableRateLimiting("fixed")]
    public class CategoriasController(ITransaction transaction, IMapper mapper) : ControllerBase
    {
        private readonly ITransaction _transaction = transaction;
        private readonly IMapper _mapper = mapper;

        //[PermissionAuthorize((int)PermissoesEnum.produtosRead)]
        [HttpPost("getAll")]

        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] Pagination paginacao, [FromBody] CategoriaFilter? filtro)
        {
            var categorias = await _transaction.CategoriaRepository.Get(paginacao, filtro);
  
            return Ok(categorias == null ? [] : _mapper.Map<IEnumerable<CategoriaDTO>>(categorias));
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            var categoriaConsulta = await _transaction.CategoriaRepository.Get(id);

            return Ok(categoriaConsulta == null ? new List<CategoriaDTO>() : _mapper.Map<CategoriaDTO>(categoriaConsulta));
        }

        [HttpGet("Produtos/{id:int:min(1)}")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetProdutosByCategoria([FromQuery] Pagination paginacao, int id)
        {
            var registros = await _transaction.CategoriaRepository.GetProdutosByCategoria(paginacao, id);

            return Ok(registros == null ? new List<CategoriaDTO>() : _mapper.Map<IEnumerable<CategoriaDTO>>(registros));
        }

        [HttpPost]
        public async Task<ActionResult> Create(CategoriaDTO categoria)
        {
            Categoria categoriaCriado = await _transaction.CategoriaRepository.Create(_mapper.Map<Categoria>(categoria));

            if (categoriaCriado == null) return BadRequest();

            await _transaction.Commit();

            return CreatedAtAction(nameof(Get), new { id = categoriaCriado.Id }, new CreatedResponseDTO() { Id = categoriaCriado.Id.ToString() });
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult> Update(int id, CategoriaDTO categoria)
        {
            if (id != categoria.Id) throw new FormatException("A requisição foi realizada de maneira incorreta!! IDs INCONSISTENTES!");

            Categoria categoriaAtualizado = _transaction.CategoriaRepository.Update(_mapper.Map<Categoria>(categoria));

            await _transaction.Commit();
            return Ok(categoriaAtualizado);
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<ActionResult> Delete(int id)
        {
            Categoria? categoria = await _transaction.CategoriaRepository.Get(id);
           
            if (categoria == null) return BadRequest(new ErrorResponse()
            {
                Errors = ["Registro não encontrado na base de dados para exclusão"],
                Message = "Registro não encontrado na base de dados para exclusão",
                StatusCode = 400
            }
            );

            _transaction.CategoriaRepository.Delete(categoria);

            await _transaction.Commit();

            return Ok();
        }

    }
}
