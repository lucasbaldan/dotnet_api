using dotnet_api.Database;
using dotnet_api.Models;
using dotnet_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace dotnet_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _Repository;
        public ProdutosController(IProdutoRepository repository)
        {
            _Repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            var produtos = await _Repository.Get();
            if(produtos == null) return NotFound();
            return Ok(produtos);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<Produto>> Get(int id)
        {
            var produtoConsulta = await _Repository.Get(id);
            if (produtoConsulta == null) return NotFound();

            return Ok(produtoConsulta);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Produto produto)
        {
            Produto produtoCriado = await _Repository.Create(produto);

            if(produtoCriado == null) return BadRequest();

            return CreatedAtAction(nameof(Get), new { id = produtoCriado.Id }, new { id = produtoCriado.Id });
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult> Update(int id, Produto produto)
        {
            if (id != produto.Id) throw new FormatException("A requisição foi enviada de maneira incorreta");

            Produto produtoAtualizado = await _Repository.Update(produto);

            return Ok(produtoAtualizado);
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<ActionResult> Delete(int id)
        {
            Produto? produto = await _Repository.Get(id);
            if (produto == null) return NotFound();

            var result = await _Repository.Delete(produto);
            if(!result) return BadRequest();

            return Ok();
        }

    }
}
