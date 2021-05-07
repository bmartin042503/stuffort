using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Timers;
using System.ComponentModel;
using Stuffort.Resources;
using Stuffort.Model;
using System.Collections.ObjectModel;

namespace Stuffort.ViewModel
{
    public class StudyTimerViewModel : INotifyPropertyChanged
    {
        //"\uec74" start
        //"\uec72" stop

        private Picker TaskPicker;
        private Switch TaskSwitch;
        private bool Running;
        private ObservableCollection<STask> tasklist;
        public ObservableCollection<STask> TaskList
        {
            get { return tasklist; }
            set
            {
                tasklist = value;
                OnPropertyChanged(nameof(TaskList));
            }
        }
        private string taskname;
        public string TaskName
        {
            get { return taskname; }
            set
            {
                taskname = value;
                OnPropertyChanged(nameof(TaskName));
            }
        }

        private string subjectname;
        public string SubjectName
        {
            get { return subjectname; }
            set
            {
                subjectname = value;
                OnPropertyChanged(nameof(SubjectName));
            }
        }

        private bool tasknamevisible;
        public bool TaskNameVisible
        {
            get { return tasknamevisible; }
            set
            {
                tasknamevisible = value;
                OnPropertyChanged(nameof(TaskNameVisible));
            }
        }

        private string iconstatus;
        public string IconStatus
        {
            get { return iconstatus; }
            set
            {
                iconstatus = value;
                OnPropertyChanged(nameof(IconStatus));
            }
        }
        public Command TimerHandler { get; set; }
        public Command TimerSave { get; set; }
        public Command TimerClear { get; set; }

        private Statistics currentstats;
        public Statistics CurrentStats
        {
            get { return currentstats; }
            set { currentstats = value; }
        }

        public TimeSpan StudyTime
        {
            get { return currentstats.Time; }
            set
            {
                currentstats.Time = value;
                OnPropertyChanged(nameof(CurrentStats));
            }
        }

        private bool isfreetimer;
        public bool IsFreeTimer
        {
            get { return isfreetimer; }
            set {
                isfreetimer = value;
                CurrentStats.TaskDisconnection = value;
                if (value == true)
                    TaskPicker.IsEnabled = false;
                else
                    TaskPicker.IsEnabled = true;
                OnPropertyChanged(nameof(IsFreeTimer));
            }
        }
        public StudyTimerViewModel(bool run, Switch sw, Picker pc)
        {
            TaskName = "";
            SubjectName = "";
            TaskList = new ObservableCollection<STask>();
            IconStatus = "\uec74";
            Running = run;
            TaskPicker = pc;
            TaskSwitch = sw;
            TimerHandler = new Command(TimerSwitch);
            TimerSave = new Command(SaveData);
            TimerClear = new Command(ResetData);
        }

        public async void ResetData()
        {
            var stats = await StatisticsServices.GetStatistics();
            foreach(var stat in stats)
            {
                await App.Current.MainPage.DisplayAlert("Statisztika-Adatok",
                    $"ID: {stat.ID}\nElindítva: {stat.Started:g}\nBefejezve: {stat.Finished:g}\nTask-ID: {stat.TaskID}\nSubject-ID: {stat.SubjectID}\nTask-Connection: {stat.TaskDisconnection}\nIsDone értéke: {stat.IsDone}\nIdő: {stat.Time:t}", "Ok");
            }
        }

        public async void TimerSwitch()
        {
            if(Running == false)
            {
                if(StudyTime.TotalSeconds == 0)
                {
                    if(CurrentStats.TaskDisconnection == false)
                    {
                        STask selectedTask = TaskPicker.SelectedItem as STask;
                        CurrentStats.TaskID = selectedTask.ID;
                        CurrentStats.SubjectID = selectedTask.SubjectID;
                        TaskName = selectedTask.Name;
                        SubjectName = selectedTask.SubjectName;
                        TaskNameVisible = true;
                    }
                    else
                    {
                        CurrentStats.TaskID = -1;
                        CurrentStats.SubjectID = -1;
                        TaskNameVisible = false;
                    }
                    CurrentStats.Started = DateTime.Now;
                    await StatisticsServices.AddStatistics(CurrentStats);
                }
                else
                {
                    if (CurrentStats.TaskDisconnection == false)
                    {
                        STask selectedTask = TaskPicker.SelectedItem as STask;
                        TaskName = selectedTask.Name;
                        SubjectName = selectedTask.SubjectName;
                        TaskNameVisible = true;
                    }
                    else TaskNameVisible = false;
                }
                TaskPicker.IsEnabled = false;
                TaskSwitch.IsEnabled = false;
                Running = true;
                IconStatus = "\uec72";
                int count = 0;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (Running)
                    {
                        Device.BeginInvokeOnMainThread(async() => { 
                            if(count == 45)
                            {
                                await StatisticsServices.UpdateStatistics(CurrentStats);
                                count = 0;
                            }
                        });
                        StudyTime = StudyTime.Add(TimeSpan.FromSeconds(1));
                        count++;
                    }
                    return Running;
                });
            }
            else
            {
                IconStatus = "\uec74";
                Running = false;
                await StatisticsServices.UpdateStatistics(CurrentStats);
            }
        }

        public async void SaveData()
        {
            if(StudyTime.TotalSeconds < 60)
            {
                IconStatus = "\uec74";
                Running = false;
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                    AppResources.ResourceManager.GetString("TimerAtLeast1Min"), "Ok");
                return;
            }
            string taskName = string.Empty;
            if(CurrentStats.TaskDisconnection == false)
            {
                bool completed = await App.Current.MainPage.DisplayAlert("", AppResources.ResourceManager.GetString("AreYouSureDoneTask"),
                    AppResources.ResourceManager.GetString("Cancel"), AppResources.ResourceManager.GetString("Yes"));
                if (!completed)
                {
                    STask selectedTask = TaskPicker.SelectedItem as STask;
                    selectedTask.IsDone = true;
                    taskName = selectedTask.Name;
                    await STaskServices.UpdateTask(selectedTask);
                    CurrentStats.IsDone = true;
                }
                else CurrentStats.IsDone = true;
            }
            else
            {
                taskName = "-";
                CurrentStats.IsDone = true;
            }
            CurrentStats.Finished = DateTime.Now;
            IconStatus = "\uec74";
            Running = false;
            await StatisticsServices.UpdateStatistics(CurrentStats);
            await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Congratulations"),
                    $"{AppResources.ResourceManager.GetString("TimerSuccessfullySaved")}\n{AppResources.ResourceManager.GetString("TimeSpent")} {CurrentStats.Time:t}\n{AppResources.ResourceManager.GetString("TimerTask")} {taskName}", "Ok");
            CurrentStats = new Statistics();
            StudyTime = new TimeSpan();
            TaskNameVisible = false;
            TaskSwitch.IsToggled = false;
            await ImportTasks();
        }

        public async Task ImportTasks()
        {
            TaskList.Clear();
            var tasks = await STaskServices.GetTasks();
            foreach(var task in tasks)
            {
                if (task.IsDone == false)
                    TaskList.Add(task);
            }
            if (TaskList.Count == 0)
            {
                TaskPicker.IsEnabled = false;
                TaskSwitch.IsEnabled = false;
            }
            else
            {
                TaskPicker.IsEnabled = true;
                TaskSwitch.IsEnabled = true;
                TaskPicker.SelectedItem = TaskList[0];
            }
        }

        public async Task InitializeStats()
        {
            var stats = await StatisticsServices.GetStatistics();
            if(stats != null && stats.Count() != 0)
            {
                var orderedstats = from stat in stats
                                   orderby stat.ID descending
                                   select stat;
                foreach(var stat in orderedstats)
                {
                    if (stat.IsDone == false)
                    {
                        CurrentStats = stat;
                        StudyTime = stat.Time;
                        if (stat.SubjectID != -1 && stat.TaskID != -1)
                        {
                            TaskPicker.SelectedItem = TaskList.Where(x => x.ID == CurrentStats.TaskID);
                            STask selectedItem = TaskPicker.SelectedItem as STask;
                            TaskName = selectedItem.Name;
                            SubjectName = selectedItem.SubjectName;
                            TaskNameVisible = true;
                        }
                        TaskPicker.IsEnabled = false;
                        TaskSwitch.IsEnabled = false;
                        return;
                    }
                }
            }
            CurrentStats = new Statistics();
            IsFreeTimer = false;
            StudyTime = new TimeSpan();
            TaskPicker.IsEnabled = true;
            TaskSwitch.IsEnabled = true;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
