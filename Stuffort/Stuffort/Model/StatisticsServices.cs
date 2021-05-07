using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Stuffort.Resources;

namespace Stuffort.Model
{
    public static class StatisticsServices
    {
        private static SQLiteAsyncConnection db;

        static public async Task Init()
        {
            try
            {
                if (db != null)
                    return;

                db = new SQLiteAsyncConnection(App.DatabaseLocation);
                await db.CreateTableAsync<Statistics>();
            }
            catch(Exception ex) { await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok"); }
        }

        static public async Task<int> AddStatistics(Statistics s)
        {
            try 
            {
                await Init();
                int rows = 0;
                rows += await db.InsertAsync(s);
                await db.CloseAsync();
                return rows;
            }
            catch(Exception ex) { await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok"); }
            return 0;
        }

        static public async Task<int> UpdateStatistics(Statistics s)
        {
            try 
            {
                await Init();
                int rows = 0;
                rows += await db.UpdateAsync(s);
                await db.CloseAsync();
                return rows;
            }
            catch(Exception ex) { await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok"); }
            return 0;
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
            try {
                await Init();
                int rows = 0;
                rows += await db.ExecuteAsync($"DELETE From [Statistics] WHERE [ID] = {s.ID}");
                await db.CloseAsync();
                return rows;
            }
            catch(Exception ex) { await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok"); }
            return 0;
        }

        static public async Task<IEnumerable<Statistics>> GetStatistics()
        {
            try
            {
                await Init();
                var statList = await db.Table<Statistics>().ToListAsync();
                await db.CloseAsync();
                return statList;
            }
            catch(Exception ex) { await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok"); }
            return null;
        }
    }
}
