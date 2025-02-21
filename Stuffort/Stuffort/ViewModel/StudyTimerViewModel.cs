﻿using System;
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
using System.Threading;
using Acr.UserDialogs;
using Stuffort.Configuration;
using System.Windows.Input;

namespace Stuffort.ViewModel
{
    public class StudyTimerViewModel : INotifyPropertyChanged
    {
        //"\uec74" start
        //"\uec72" stop
        private INotificationManager notificationManager;
        private Picker TaskPicker;
        private Switch TaskSwitch;
        private Button TimerHandlerButton;
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

        private ObservableCollection<Statistics> statslist;
        public ObservableCollection<Statistics> StatsList
        {
            get { return statslist; }
            set
            {
                statslist = value;
                OnPropertyChanged(nameof(StatsList));
            }
        }

        private bool isrefreshing;
        public bool IsRefreshing
        {
            get { return isrefreshing; }
            set 
            { 
                isrefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
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

        public ICommand TimerHandler { get; set; }
        public ICommand TimerSave { get; set; }
        public ICommand TimerClear { get; set; }
        public ICommand RefreshStatsCommand { get; set; }
        public ICommand StatsRemoveCommand { get; set; }

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

        public StudyTimerViewModel(bool run, Switch sw, Picker pc, Button btn)
        { 
            StatsList = new ObservableCollection<Statistics>();
            notificationManager = DependencyService.Get<INotificationManager>();
            TimerHandlerButton = btn;
            TaskName = "";
            SubjectName = "";
            TaskList = new ObservableCollection<STask>();
            TimerHandlerButton.Text = "\uec74";
            Running = run;
            TaskPicker = pc;
            TaskSwitch = sw;
            TimerHandler = new Command(TimerSwitch);
            TimerSave = new Command(SaveData);
            TimerClear = new Command(ResetData);
            RefreshStatsCommand = new Command(Refresh);
            StatsRemoveCommand = new Command(RemoveStat);
        }

        public async void RemoveStat(object value)
        {
            try
            {
                if (value == null) return;
                Statistics stat = value as Statistics;
                bool delete = await App.Current.MainPage.DisplayAlert("", AppResources.AreYouSureDeleteStat,
                    AppResources.Cancel, AppResources.Yes);
                if (!delete)
                {
                    if (stat.ID == CurrentStats.ID) ResetData("true");
                    else await StatisticsServices.DeleteStatistics(stat);
                    await ImportStats();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
$"{AppResources.ErrorMessage} {ex.Message}", "Ok");
            }
        }

        public async void ResetData(object value)
        {
            string val = value as string;
            bool skipit = bool.Parse(val);
            bool delete = true;
            if (skipit == false)
            {
                delete = await App.Current.MainPage.DisplayAlert("", AppResources.ResetTimer,
                    AppResources.Cancel, AppResources.Yes);
            }
            if (!delete || skipit == true)
            {
                Running = false;
                await StatisticsServices.DeleteStatistics(CurrentStats);
                await ImportStats();
                StudyTime = new TimeSpan();
                CurrentStats = new Statistics();
                TaskName = string.Empty;
                SubjectName = string.Empty;
                TaskNameVisible = false;
                if (TaskList.Count != 0)
                {
                    TaskPicker.IsEnabled = true;
                    TaskSwitch.IsEnabled = true;
                }
                else
                {
                    TaskPicker.IsEnabled = false;
                    TaskSwitch.IsEnabled = false;
                    CurrentStats.TaskDisconnection = true;
                }
                TimerHandlerButton.Text = "\uec74";
            }

        }

        public void SwitchRefresh()
        {
            TimerHandlerButton.IsEnabled = false;
            UserDialogs.Instance.ShowLoading(AppResources.Loading, new MaskType());
            Task.Run(async () =>
            {
                await Task.Delay(1250);
                UserDialogs.Instance.HideLoading();
            });
            TimerHandlerButton.IsEnabled = true;
        }

        public async void TimerSwitch()
        {
            try
            {
                SwitchRefresh();
                if (Running == false)
                {
                    if (StudyTime.TotalSeconds == 0)
                    {
                        if (CurrentStats.TaskDisconnection == false)
                        {
                            STask selectedTask = TaskPicker.SelectedItem as STask;
                            CurrentStats.TaskID = selectedTask.ID;
                            CurrentStats.SubjectID = selectedTask.SubjectID;
                            TaskName = selectedTask.Name;
                            SubjectName = selectedTask.SubjectName;
                            CurrentStats.SubjectName = selectedTask.SubjectName;
                            CurrentStats.TaskName = selectedTask.Name;
                            TaskNameVisible = true;
                        }
                        else
                        {
                            CurrentStats.TaskID = -1;
                            CurrentStats.SubjectID = -1;
                            CurrentStats.SubjectName = "UNDEFINED";
                            CurrentStats.TaskName = "UNDEFINED";
                            TaskNameVisible = false;
                        }
                        CurrentStats.Started = DateTime.Now;
                        await StatisticsServices.AddStatistics(CurrentStats);
                        await ImportStats();
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
                    ConfigurationType ct = ConfigurationServices.GetConfigurationData();
                    if (ct.NotificationEnabled == true)
                    {
                        notificationManager.SendNotification(AppResources.StudyTimerPage,
                            AppResources.TimerHasStartedNotification, DateTime.Now);
                    }
                    Running = true;
                    TaskPicker.IsEnabled = false;
                    TaskSwitch.IsEnabled = false;
                    TimerHandlerButton.Text = "\uec72";
                    int count = 0;
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        if (Running)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                if (count == 45)
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
                    TimerHandlerButton.Text = "\uec74";
                    Running = false;
                    await StatisticsServices.UpdateStatistics(CurrentStats);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
$"{AppResources.ErrorMessage} {ex.Message}", "Ok");
            }
        }

        public async void SaveData()
        {
            try
            {
                if (StudyTime.TotalSeconds < 60)
                {
                    TimerHandlerButton.Text = "\uec74";
                    Running = false;
                    await App.Current.MainPage.DisplayAlert(AppResources.Error,
                        AppResources.TimerAtLeast1Min, "Ok");
                    await StatisticsServices.UpdateStatistics(CurrentStats);
                    return;
                }
                Running = false;
                string taskName = string.Empty;
                if (CurrentStats.TaskDisconnection == false)
                {
                    bool completed = await App.Current.MainPage.DisplayAlert("", AppResources.AreYouSureDoneTask,
                        AppResources.Cancel, AppResources.Yes);
                    if (!completed)
                    {
                        STask selectedTask = TaskPicker.SelectedItem as STask;
                        selectedTask.IsDone = true;
                        selectedTask.Finished = DateTime.Now;
                        taskName = selectedTask.Name;
                        await STaskServices.UpdateTask(selectedTask);
                    }
                    CurrentStats.IsDone = true;
                }
                else
                {
                    taskName = "-";
                    CurrentStats.IsDone = true;
                }
                CurrentStats.Finished = DateTime.Now;
                TimerHandlerButton.Text = "\uec74";
                await StatisticsServices.UpdateStatistics(CurrentStats);
                await App.Current.MainPage.DisplayAlert(AppResources.Congratulations,
                        $"{AppResources.TimerSuccessfullySaved}\n{AppResources.TimerSpent} {CurrentStats.Time:t}\n{AppResources.TimerTask} {taskName}", "Ok");
                CurrentStats = new Statistics();
                StudyTime = new TimeSpan();
                CurrentStats.TaskDisconnection = true;
                TaskNameVisible = false;
                TaskSwitch.IsToggled = true;
                await ImportTasks();
                await ImportStats();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
$"{AppResources.ErrorMessage} {ex.Message}", "Ok");
            }
        }

        public async Task ImportTasks()
        {
            try
            {
                if (Running == false)
                {
                    TaskList.Clear();
                    var tasks = await STaskServices.GetTasks();
                    foreach (var task in tasks)
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
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
$"{AppResources.ErrorMessage} {ex.Message}", "Ok");
            }
        }

        public async void Refresh()
        {
            IsRefreshing = true;
            await Task.Delay(500);
            await ImportStats();
            IsRefreshing = false;
        }

        public async Task ImportStats()
        {
            try
            {
                StatsList.Clear();
                var stats = await StatisticsServices.GetStatistics();
                if (stats != null && stats.Count() != 0)
                {
                    var orderedstats = from stat in stats
                                       orderby stat.ID descending
                                       select stat;
                    foreach (var stat in orderedstats)
                    {
                        if (stat.SubjectID == -1 || stat.TaskID == -1) stat.TemporaryName = AppResources.FreeTimerTitle;
                        else stat.TemporaryName = stat.TaskName;
                        StatsList.Add(stat);
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.Error,
$"{AppResources.ErrorMessage} {ex.Message}", "Ok");
            }
        }

        public async Task InitializeStats()
        {
            if (Running == false)
            {
                var stats = await StatisticsServices.GetStatistics();
                if (stats != null && stats.Count() != 0)
                {
                    var orderedstats = from stat in stats
                                       orderby stat.ID descending
                                       select stat;
                    foreach (var stat in orderedstats)
                    {
                        if (stat.IsDone == false)
                        {
                            CurrentStats = stat;
                            StudyTime = stat.Time;
                            if (stat.SubjectID != -1 && stat.TaskID != -1)
                            {
                                STask selectedItem = null;
                                for(int i = 0; i < TaskList.Count; ++i)
                                {
                                    if(TaskList[i].ID == CurrentStats.TaskID)
                                    {
                                        selectedItem = TaskList[i];
                                        break;
                                    }
                                }
                                if (selectedItem == null) return;
                                TaskPicker.SelectedItem = selectedItem;
                                TaskName = selectedItem.Name;
                                SubjectName = selectedItem.SubjectName;
                                CurrentStats.TaskDisconnection = false;
                                TaskPicker.IsEnabled = false;
                                TaskSwitch.IsEnabled = false;
                                TaskNameVisible = true;
                            }
                            return;
                        }
                    }
                }
                CurrentStats = new Statistics();
                TaskPicker.IsEnabled = false;
                CurrentStats.TaskDisconnection = true;
                StudyTime = new TimeSpan();
            }
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
