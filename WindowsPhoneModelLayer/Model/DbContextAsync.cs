using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;

namespace GameOfTasks.Model
{
    public class DbContextAsync
    {
        private SQLiteAsyncConnection _connection;

        private const string ConnectionString = "Jobs.db";

        public async Task Init()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, ConnectionString));
#if DEBUG
            //await _connection.DropTableAsync<ToDoJob>();
#endif
            await _connection.CreateTableAsync<ToDoJob>();
        }

        public AsyncTableQuery<ToDoJob> ToDoJobs
        {
            get { return _connection.Table<ToDoJob>(); }
        }

        public SQLiteAsyncConnection Connection
        {
            get { return _connection; }
        }
    }
}
