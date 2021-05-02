﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Threading.Tasks;

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
        }

        static public async void DeleteAll()
        {
            await Init();
            await db.DeleteAllAsync<Subject>();
            await db.DeleteAllAsync<STask>();
            await db.CloseAsync();
        }

        static public async Task<int> AddSubject(Subject s)
        {
            int rows = 0;
            await Init();
            rows = await db.InsertAsync(s);
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
