﻿using Stuffort.Model;
using Stuffort.View.ShellPages;
using Stuffort.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stuffort.ViewModel
{
    public class TasksViewModel 
    {
        public TaskCommand TaskCommand { get; set; }
        public ObservableCollection<Tuple<string,DateTime,DateTime,string,int,bool>> TaskList { get; set; }

        public TasksViewModel()
        {
            this.TaskCommand = new TaskCommand(this);
            TaskList = new ObservableCollection<Tuple<string, DateTime, DateTime, string, int, bool>>();
        }

        public async Task UpdateTasks()
        {
            TaskList.Clear();
            var tasks = await STaskServices.GetTasks();
            var subjects = await SubjectServices.GetSubjects();
            var newlist = from task in tasks
                          join subject in subjects
                          on task.SubjectID equals subject.ID
                          select new
                          {
                              Name = task.Name,
                              AddedTime = task.AddedTime,
                              DeadLine = task.DeadLine,
                              SubjectName = subject.Name,
                              ID = task.ID,
                              IsDone = task.IsDone
                          };
            foreach(var task in newlist)
            {
                TaskList.Add(new Tuple<string, DateTime, DateTime, string, int, bool>(task.Name, task.AddedTime, task.DeadLine, task.SubjectName, task.ID, task.IsDone));
            }
        }

        public async Task NavigateToNewTask()
        {
            await AppShell.Current.GoToAsync($"{nameof(NewTaskPage)}");
        }

    }
}
