using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Stuffort.Model
{
    public static class StatisticsServices
    {
        private static SQLiteAsyncConnection db;

        static public async Task Init()
        {
            if (db != null)
                return;

            db = new SQLiteAsyncConnection(App.DatabaseLocation);
            await db.CreateTableAsync<Statistics>();
        }

        static public async Task<int> AddStatistics(Statistics s)
        {
            await Init();
            int rows = 0;
            rows += await db.InsertAsync(s);
            await db.CloseAsync();
            return rows;
        }

        static public async Task<int> UpdateStatistics(Statistics s)
        {
            await Init();
            int rows = 0;
            rows += await db.UpdateAsync(s);
            await db.CloseAsync();
            return rows;
        }

        static public void DeleteAll()
        {
            db = new SQLiteAsyncConnection(App.DatabaseLocation);
            db.CreateTableAsync<Statistics>();
            db.DeleteAllAsync<Statistics>();
            db.CloseAsync();
        }

        static public async Task<int> DeleteStatistics(Statistics s)
        {
            await Init();
            int rows = 0;
            rows += await db.ExecuteAsync($"DELETE From [Statistics] WHERE [ID] = {s.ID}");
            await db.CloseAsync();
            return rows;
        }

        static public async Task<IEnumerable<Statistics>> GetStatistics()
        {
            await Init();
            var statList = await db.Table<Statistics>().ToListAsync();
            await db.CloseAsync();
            return statList;
        }
    }
}
