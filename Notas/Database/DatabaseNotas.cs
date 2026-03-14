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

        public Task<List<Nota>> GetNotasAsync()
        {
            return database.Table<Nota>().ToListAsync();
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

