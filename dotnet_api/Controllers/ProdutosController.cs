using dotnet_api.Models;
using dotnet_api.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ITransaction _transaction;
        public ProdutosController(ITransaction transaction)
        {
            _transaction = transaction;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            var produtos = await _transaction.ProdutoRepository.Get();
            if(produtos == null) return NotFound();
            return Ok(produtos);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<Produto>> Get(int id)
        {
            var produtoConsulta = await _transaction.ProdutoRepository.Get(id);
            if (produtoConsulta == null) return NotFound();

            return Ok(produtoConsulta);
        }

        [HttpPost]
        public ActionResult Create(Produto produto)
        {
            Produto produtoCriado = _transaction.ProdutoRepository.Create(produto);

            if(produtoCriado == null) return BadRequest();

            _transaction.Commit();
            return CreatedAtAction(nameof(Get), new { id = produtoCriado.Id }, new { id = produtoCriado.Id });
        }

        [HttpPut("{id:int:min(1)}")]
        public ActionResult Update(int id, Produto produto)
        {
            if (id != produto.Id) throw new FormatException("A requisição foi enviada de maneira incorreta");

            Produto produtoAtualizado = _transaction.ProdutoRepository.Update(produto);

            _transaction.Commit();
            return Ok(produtoAtualizado);
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<ActionResult> Delete(int id)
        {
            Produto? produto = await _transaction.ProdutoRepository.Get(id);
            if (produto == null) return NotFound();

            _transaction.ProdutoRepository.Delete(produto);

            var result = await _transaction.CommitAsync();

            if(!result) 
                return BadRequest();

            return Ok();
        }

    }
}
