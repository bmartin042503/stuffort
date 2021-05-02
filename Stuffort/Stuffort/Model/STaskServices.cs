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

        static public async Task<int> AddTask(STask s)
        {
            int rows = 0;
            await Init();
            rows = await db.InsertAsync(s);
            await db.CloseAsync();
            return rows;
        }

        static public async Task<int> RemoveTask(STask s)
        {
            int rows = 0;
            await Init();
            rows = await db.ExecuteAsync($"DELETE FROM [STask] WHERE [ID] = {s.ID}");
            await db.CloseAsync();
            return rows;
        }

        static public async Task<IEnumerable<STask>> GetTasks()
        {
            await Init();
            var taskList = await db.Table<STask>().ToListAsync();
            await db.CloseAsync();
            return taskList;
        }
    }
}
