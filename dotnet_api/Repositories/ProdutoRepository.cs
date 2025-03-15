using dotnet_api.Database;
using dotnet_api.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly BDContext _db;
    public ProdutoRepository(BDContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Produto>> Get()
    {
        return await _db.Produtos.AsNoTracking().ToListAsync();
    }
    public async Task<Produto?> Get(int id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _db.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }
    public Produto Create(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);

        _db.Produtos.Add(produto);
        return produto;
    }
    public Produto Update(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);

        _db.Entry(produto).State = EntityState.Modified;
        return produto;
    }
    public void Delete(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);

        _db.Produtos.Remove(produto);
    }
}

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> Get();
    Task<Produto?> Get(int id);
    Produto Create(Produto produto);
    Produto Update(Produto produto);
    void Delete(Produto produto);
}
