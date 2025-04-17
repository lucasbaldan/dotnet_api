using dotnet_api.Database;
using dotnet_api.Models;
using dotnet_api.Shared.Utilities;
using dotnet_api.Shared.Utilities.FilterClasses;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api.Repositories;

public class CategoriaRepository(BDContext db) : ICategoriaRepository
{
    private readonly BDContext _db = db;

    public async Task<Categoria?> Get(int id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _db.Categorias.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Categoria>> Get(Pagination paginacao, CategoriaFilter? filtro)
    {
        ArgumentNullException.ThrowIfNull(paginacao);

        var query = _db.Categorias.AsQueryable();

        if (filtro != null)
        {

            if (filtro.Id != null)
            {
                var filtroID = filtro.Id?.ToString();
                if (filtroID != null) query = query.Where(p => p.Id.ToString().Contains(filtroID));
            }
            if (filtro.Nome != null) query = query.Where(p => p.Nome != null && p.Nome.Contains(filtro.Nome));
            if (filtro.Descricao != null) query = query.Where(p => p.Descricao != null && p.Descricao.Contains(filtro.Descricao));
        }

        return await query.
            OrderByDescending(p => p.Id)
            .Skip((paginacao.PageNumber - 1) * paginacao.PageSize)
            .Take(paginacao.PageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Categoria> Create(Categoria categoria)
    {
        ArgumentNullException.ThrowIfNull(categoria);

        await _db.Categorias.AddAsync(categoria);

        return categoria;
    }

    public Categoria Update(Categoria categoria)
    {
        ArgumentNullException.ThrowIfNull(categoria);

        _db.Entry(categoria).State = EntityState.Modified;
        return categoria;
    }

    public Categoria Delete(Categoria categoria)
    {
        ArgumentNullException.ThrowIfNull(categoria);

        _db.Categorias.Remove(categoria);
        return categoria;
    }

    public async Task<IEnumerable<Categoria>> GetProdutosByCategoria(Pagination paginacao, int id)
    {
        ArgumentNullException.ThrowIfNull(paginacao);
        ArgumentNullException.ThrowIfNull(id);

        return await _db.Categorias.Where(p => p.Id == id)
            .Include(p => p.Produtos)
            .OrderByDescending(p => p.Id)
            .Skip((paginacao.PageNumber - 1) * paginacao.PageSize)
            .Take(paginacao.PageSize)
            .AsNoTracking()
            .ToListAsync();
    }
}

public interface ICategoriaRepository
{
    Task<Categoria?> Get(int id);
    Task<IEnumerable<Categoria>> Get(Pagination paginacao, CategoriaFilter? filtro);
    Task<IEnumerable<Categoria>> GetProdutosByCategoria(Pagination paginacao, int id);
    Task<Categoria> Create(Categoria categoria);
    Categoria Update(Categoria categoria);
    Categoria Delete(Categoria categoria);
}
