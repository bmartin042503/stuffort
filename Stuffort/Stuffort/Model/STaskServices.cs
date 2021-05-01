using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Stuffort.Model
{
    public static class STaskServices
    {
        private static SQLiteAsyncConnection db;

        static public async Task Init()
        {
            if (db != null)
                return;

            db = new SQLiteAsyncConnection(App.DatabaseLocation);
            await db.CreateTableAsync<STask>();
        }

        static public async void AddTask(STask s)
        {
            await Init();
            await db.InsertAsync(s);
        }

        static public async void RemoveTask(Task s)
        {
            await Init();
            await db.DeleteAsync(s);
        }

        static public async Task<IEnumerable<STask>> GetTasks()
        {
            await Init();
            var taskList = db.Table<STask>().ToListAsync();
            return (IEnumerable<STask>)taskList;
        }
    }
}
