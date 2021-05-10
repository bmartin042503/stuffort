using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Stuffort.Resources;

namespace Stuffort.Model
{
    public static class STaskServices
    {
        private static SQLiteAsyncConnection db;

        static public async Task Init()
        {
            try
            {
                if (db != null)
                    return;

                db = new SQLiteAsyncConnection(App.DatabaseLocation);
                await db.CreateTableAsync<Subject>();
                await db.CreateTableAsync<STask>();
                await db.CreateTableAsync<Statistics>();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        static async public Task DeleteAll()
        {
            try
            {
                await Init();
                await db.DeleteAllAsync<STask>();
                await db.CloseAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                    $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        static public async Task<int> AddTask(STask s)
        {
            int rows = 0;
            try
            {
                await Init();
                rows = await db.InsertAsync(s);
                await db.CloseAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
            return rows;
        }

        static public async Task<int> RemoveTask(STask s)
        {
            int rows = 0;
            try
            {
                await Init();
                rows += await db.ExecuteAsync($"DELETE FROM [STask] WHERE [ID] = {s.ID}");
                rows += await db.ExecuteAsync($"UPDATE [Statistics] SET [TaskName] = 'UNDEFINED' WHERE [TaskID] = {s.ID}");
                await db.CloseAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
            return rows;
        }

        static public async Task<int> UpdateTask(STask s)
        {
            int rows = 0;
            try
            {
                await Init();
                rows = await db.UpdateAsync(s);
                await db.CloseAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
            return rows;
        }

        static public async Task<IEnumerable<STask>> GetTasks()
        {
            try
            {
                await Init();
                var taskList = await db.Table<STask>().ToListAsync();
                await db.CloseAsync();
                return taskList;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
            return null;
        }
    }
}
