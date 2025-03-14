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
    public async Task<Produto> Create(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);

        _db.Produtos.Add(produto);
        await _db.SaveChangesAsync();
        return produto;
    }
    public async Task<Produto> Update(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);

        _db.Entry(produto).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return produto;
    }
    public async Task<bool> Delete(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);

        _db.Produtos.Remove(produto);
        var result = await _db.SaveChangesAsync();
        return true;
    }
}

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> Get();
    Task<Produto?> Get(int id);
    Task<Produto> Create(Produto produto);
    Task<Produto> Update(Produto produto);
    Task<bool> Delete(Produto produto);
}
