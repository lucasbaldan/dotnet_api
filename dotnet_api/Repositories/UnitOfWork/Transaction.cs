using dotnet_api.Database;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api.Repositories.UnitOfWork
{

    public class Transaction : ITransaction
    {
        private readonly BDContext _db;
        private IProdutoRepository? _produtoRepository;

        public Transaction(BDContext db)
        {
            _db = db;
        }
        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepository ??= new ProdutoRepository(_db);
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
        void Dispose();
        Task<bool> Commit();
    }
}
