using AutoMapper;
using dotnet_api.DTOs;
using dotnet_api.Models;
using dotnet_api.Repositories.UnitOfWork;
using dotnet_api.Utilities;
using dotnet_api.Utilities.FilterClasses;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ITransaction _transaction;
        private readonly IMapper _mapper;
        public ProdutosController(ITransaction transaction, IMapper mapper)
        {
            _transaction = transaction;
            _mapper = mapper;
        }

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
            return CreatedAtAction(nameof(Get), new { id = produtoCriado.Id }, new { id = produtoCriado.Id });
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
