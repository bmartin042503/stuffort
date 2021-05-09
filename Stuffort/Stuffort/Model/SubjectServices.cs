using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Threading.Tasks;
using Stuffort.Resources;

namespace Stuffort.Model
{
    public static class SubjectServices
    {
        private static SQLiteAsyncConnection db;
        static public async Task Init()
        {
            if (db != null)
                return;

            db = new SQLiteAsyncConnection(App.DatabaseLocation);
            await db.CreateTableAsync<Subject>();
            await db.CreateTableAsync<STask>();
            await db.CreateTableAsync<Statistics>();
        }

        static async public Task DeleteAll()
        {
            try
            {
                await Init();
                await db.DeleteAllAsync<Subject>();
                await db.CloseAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                    $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        static public async Task<int> AddSubject(Subject s)
        {
            int rows = 0;
            await Init();
            rows = await db.InsertAsync(s);
            await db.CloseAsync();
            return rows;
        }
        static public async Task<int> RenameSubject(Subject s, string newname)
        {
            int rows = 0;
            await Init();
            rows += await db.ExecuteAsync($"UPDATE [Subject] SET [Name] = '{newname}' WHERE [ID] = {s.ID}");
            rows += await db.ExecuteAsync($"UPDATE [STask] SET [SubjectName] = '{newname}' WHERE [SubjectID] = {s.ID}");
            await db.CloseAsync();
            return rows;
        }

        static public async Task<int> RemoveSubject(Subject s)
        {
            int rows = 0;
            await Init();
            int tasksToRemoveID = s.ID;
            rows += await db.ExecuteAsync($"DELETE FROM [Subject] WHERE [ID] = {s.ID}");
            rows += await db.ExecuteAsync($"DELETE FROM [STask] WHERE [SubjectID] = {s.ID}");
            rows += await db.ExecuteAsync($"UPDATE [Statistics] SET [TaskID] = -1 WHERE [SubjectID] = {s.ID}");
            rows += await db.ExecuteAsync($"UPDATE [Statistics] SET [SubjectID] = -1 WHERE [SubjectID] = {s.ID}");
            await db.CloseAsync();
            return rows;
        }

        static public async Task<int> RemoveSubject(int key)
        {
            int rows = 0;
            await Init();
            rows = await db.DeleteAsync(key);
            await db.CloseAsync();
            return rows;
        }

        static public async Task<IEnumerable<Subject>> GetSubjects()
        {
            await Init();
            var subjectList = await db.Table<Subject>().ToListAsync();
            await db.CloseAsync();
            return subjectList;
        }
    }
}
