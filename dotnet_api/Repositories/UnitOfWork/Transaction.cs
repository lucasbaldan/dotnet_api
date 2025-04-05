using dotnet_api.Database;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api.Repositories.UnitOfWork
{

    public class Transaction(BDContext db) : ITransaction
    {
        private readonly BDContext _db = db;
        private IProdutoRepository? _produtoRepository;
        private IPessoaRepository? _pessoaRepository;

        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepository ??= new ProdutoRepository(_db);
            }
        }

        public IPessoaRepository PessoaRepository
        {
            get
            {
                return _pessoaRepository ??= new PessoaRepository(_db);
            }
        }

        public async Task<bool> Commit()
        {
            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                return false;
                throw new Exception($"Erro no banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception($"Erro no banco de dados: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }


    public interface ITransaction
    {
        IProdutoRepository ProdutoRepository { get; }
        IPessoaRepository PessoaRepository { get; }
        void Dispose();
        Task<bool> Commit();
    }
}
