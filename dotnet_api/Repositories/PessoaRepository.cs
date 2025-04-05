using dotnet_api.Database;
using dotnet_api.Models;
using dotnet_api.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api.Repositories;

public class PessoaRepository(BDContext db) : IPessoaRepository
{
    private readonly BDContext _db = db;

    public async Task<Pessoa?> Get(int id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _db.Pessoas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<IEnumerable<Pessoa>>? Get(Pagination paginacao)
    {
        throw new NotImplementedException();
    }

    public void Delete(Pessoa pessoa)
    {
        ArgumentNullException.ThrowIfNull(pessoa);

        _db.Pessoas.Remove(pessoa);
    }

    public Pessoa Create(Pessoa pessoa)
    {
        ArgumentNullException.ThrowIfNull(pessoa);

        _db.Pessoas.Add(pessoa);
        return pessoa;
    }

    public Pessoa Update(Pessoa pessoa)
    {
        ArgumentNullException.ThrowIfNull(pessoa);

        _db.Entry(pessoa).State = EntityState.Modified;
        return pessoa;
    }
}

public interface IPessoaRepository
{
    Task<Pessoa?> Get(int id);
    Task<IEnumerable<Pessoa>>? Get(Pagination paginacao);
    Pessoa Create(Pessoa pessoa);
    Pessoa Update(Pessoa pessoa);
    void Delete(Pessoa pessoa);
}
