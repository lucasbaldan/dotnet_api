using dotnet_api.Database;
using dotnet_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly BDContext _db;
        public ProdutosController(BDContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produtosConsulta = _db.Produtos.AsNoTracking().ToList();
            if(produtosConsulta == null || produtosConsulta.Count == 0) return NotFound();

            return produtosConsulta;

        }

        [HttpGet("{id:int}")]
        public ActionResult<Produto> Get(int id)
        {
            var produtoConsulta = _db.Produtos.AsNoTracking().FirstOrDefault(p => p.Id == id);
            if (produtoConsulta == null) return NotFound();

            return produtoConsulta;
        }

        [HttpPost]
        public ActionResult Create(Produto produto)
        {
            _db.Produtos.Add(produto);
            _db.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = produto.Id }, new { id = produto.Id});
        }

        [HttpPut("{id:int}")]
        public ActionResult Update(int id, Produto produto)
        {
            if(id != produto.Id) return BadRequest();

            _db.Entry(produto).State = EntityState.Modified;
            _db.SaveChanges();

            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            Produto? produto = _db.Produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null) return NotFound();

            _db.Produtos.Remove(produto);
            _db.SaveChanges();

            return Ok();
        }

    }
}
