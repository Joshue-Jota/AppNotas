using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Notas.Models;

namespace Notas.Database
{
    public class DatabaseNotas
    {
        readonly SQLiteAsyncConnection database;

        public DatabaseNotas(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Nota>().Wait();
        }

        // Ordenado: pinned primero, luego más reciente
        public Task<List<Nota>> GetNotasAsync()
        {
            return database.QueryAsync<Nota>(
                "SELECT * FROM Nota ORDER BY IsPinned DESC, UpdatedAt DESC");
        }

        public Task<Nota> GetNotaByIdAsync(int id)
        {
            return database.FindAsync<Nota>(id);
        }

        public Task<int> SaveNotaAsync(Nota nota)
        {
            return database.InsertAsync(nota);
        }

        public Task<int> DeleteNotaAsync(Nota nota)
        {
            return database.DeleteAsync(nota);
        }

        public Task<int> UpdateNotaAsync(Nota nota)
        {
            return database.UpdateAsync(nota);
        }
    }
}