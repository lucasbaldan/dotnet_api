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
    [Authorize]
    [EnableRateLimiting("fixed")]
    public class ProdutosController(ITransaction transaction, IMapper mapper) : ControllerBase
    {
        private readonly ITransaction _transaction = transaction;
        private readonly IMapper _mapper = mapper;

        [PermissionAuthorize((int)PermissoesEnum.produtosRead)]
        [HttpPost("getAll")]

        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] Pagination paginacao, [FromBody] ProdutoFilter? filtro)
        {
            var produtos = await _transaction.ProdutoRepository.Get(paginacao, filtro);
            if (produtos == null) throw new Exception("Ocorreu um erro inesperado ao consultar os registro na base de dados");
            else if (!produtos.Any()) return NotFound();
            return Ok(_mapper.Map<IEnumerable<ProdutoDTO>>(produtos));
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var produtoConsulta = await _transaction.ProdutoRepository.Get(id);
            if (produtoConsulta == null) return NotFound();

            return Ok(_mapper.Map<ProdutoDTO>(produtoConsulta));
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProdutoDTO produto)
        {
            Produto produtoCriado = _transaction.ProdutoRepository.Create(_mapper.Map<Produto>(produto));

            if (produtoCriado == null) return BadRequest();

            await _transaction.Commit();
            return CreatedAtAction(nameof(Get), new { id = produtoCriado.Id }, new CreatedResponseDTO() { Id = produtoCriado.Id.ToString() });
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult> Update(int id, ProdutoDTO produto)
        {
            if (id != produto.Id) throw new FormatException("A requisição foi enviada de maneira incorreta");

            Produto produtoAtualizado = _transaction.ProdutoRepository.Update(_mapper.Map<Produto>(produto));

            await _transaction.Commit();
            return Ok(produtoAtualizado);
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<ActionResult> Delete(int id)
        {
            Produto? produto = await _transaction.ProdutoRepository.Get(id);
            if (produto == null) return NotFound();

            _transaction.ProdutoRepository.Delete(produto);

            await _transaction.Commit();

            return Ok();
        }

    }
}
