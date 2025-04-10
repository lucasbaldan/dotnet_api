using dotnet_api.Database;
using dotnet_api.Models;
using dotnet_api.Shared.Utilities;
using dotnet_api.Shared.Utilities.FilterClasses;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace dotnet_api.Repositories;

public class ProdutoRepository(BDContext db) : IProdutoRepository
{
    private readonly BDContext _db = db;

    public async Task<IEnumerable<Produto>> Get()
    {
        return await _db.Produtos.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Produto>> Get(Pagination paginacao, ProdutoFilter? filtro)
    {
        ArgumentNullException.ThrowIfNull(paginacao);

        var query = _db.Produtos.AsQueryable();

        if (filtro != null)
        {

            if (filtro.Id != null)
            {
                var filtroID = filtro.Id?.ToString();
                if (filtroID != null) query = query.Where(p => p.Id.ToString().Contains(filtroID));
            }
            if (filtro.Nome != null) query = query.Where(p => p.Nome != null && p.Nome.Contains(filtro.Nome));
            if (filtro.Descricao != null) query = query.Where(p => p.Descricao != null && p.Descricao.Contains(filtro.Descricao));
            if (filtro.MinPreco != null) query = query.Where(p => p.Preco >= filtro.MinPreco);
            if (filtro.MaxPreco != null) query = query.Where(p => p.Preco <= filtro.MaxPreco);
            if (filtro.MinEstoque != null) query = query.Where(p => p.Estoque >= filtro.MinEstoque);
            if (filtro.MaxEstoque != null) query = query.Where(p => p.Estoque <= filtro.MaxEstoque);
            if (filtro.CategoriaId != null) query = query.Where(p => p.CategoriaId == filtro.CategoriaId);
        }

        return await query.
            Include(p => p.Categoria).
            OrderByDescending(p => p.Id)
            .Skip((paginacao.PageNumber - 1) * paginacao.PageSize)
            .Take(paginacao.PageSize)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Produto?> Get(int id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _db.Produtos.FirstOrDefaultAsync(p => p.Id == id);
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
    Task<IEnumerable<Produto>> Get(Pagination paginacao, ProdutoFilter? filtro);
    Produto Create(Produto produto);
    Produto Update(Produto produto);
    void Delete(Produto produto);
}
