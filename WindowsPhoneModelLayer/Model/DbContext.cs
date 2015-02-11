using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using SQLite;

namespace GameOfTasks.Model
{
    public class DbContext
    {
        private readonly SQLiteConnection _connection;

        private const string ConnectionString = "Jobs.db";

        public DbContext()
        {
            _connection = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, ConnectionString));
        }

        public SQLiteConnection Connection
        {
            get { return _connection; }
        }

        public List<ToDoJob> ToDoJobs
        {
            get { return _connection.Table<ToDoJob>().ToList(); }
        }
    }
}
