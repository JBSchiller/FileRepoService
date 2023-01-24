using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRepoServiceApi.Models;

namespace FileRepoApiTestProject
{
    public abstract class SqliteTestBase : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;

        protected readonly FileRepoDC DbContext;

        protected SqliteTestBase()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            var options = new DbContextOptionsBuilder<FileRepoDC>()
                    .UseSqlite(_connection)
                    .Options;
            DbContext = new FileRepoDC(options);
            DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
