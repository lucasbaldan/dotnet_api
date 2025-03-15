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
        public void Commit()
        {
            _db.SaveChanges();
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                return await _db.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erro ao salvar alterações no banco: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return false;
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
        void Commit();
        void Dispose();
        Task<bool> CommitAsync();
    }
}
