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
        private bool Running;

        private string iconchanging;
        public string IconChanging
        {
            get { return iconchanging; }
            set
            {
                iconchanging = value;
                OnPropertyChanged(nameof(IconChanging));
            }
        }

        private Statistics CurrentStats;

        private TimeSpan studytime;
        public TimeSpan StudyTime
        {
            get { return studytime; }
            set
            {
                studytime = value;
                OnPropertyChanged(nameof(StudyTime));
            }
        }
        public Command TimerHandler { get; set; }
        public Command TimerSave { get; set; }

        public Command TimerClear { get; set; }

        public ObservableCollection<STask> TaskList { get; set; }

        public Picker TaskPicker { get; set; }
        public Switch SwitchTimer { get; set; }

        private bool isfreetimer;
        public bool IsFreeTimer
        {
            get { return isfreetimer; }
            set
            {
                isfreetimer = value;
                if (value == true)
                    TaskPicker.IsEnabled = false;
                else
                    TaskPicker.IsEnabled = true;
                OnPropertyChanged(nameof(IsFreeTimer));
            }
        }

        public StudyTimerViewModel(bool run, Switch sw, Picker pc)
        {
            TaskList = new ObservableCollection<STask>();
            ImportTasks();
            Running = run;
            IconChanging = "\uec74";
            TimerHandler = new Command(TimerSwitch);
            TimerSave = new Command(StudyTimeSave);
            TimerClear = new Command(StudyTimeClear);
            SwitchTimer = sw;
            TaskPicker = pc;
            pc.SelectedIndex = 0;
        }

        public async void ImportTasks()
        {
            TaskList.Clear();
            var tasks = await STaskServices.GetTasks();
            foreach(var task in tasks)
            {
                if (task.IsDone == false)
                    TaskList.Add(task);
            }
            if(TaskList.Count == 0)
            {
                TaskPicker.IsEnabled = false;
                IsFreeTimer = true;
                SwitchTimer.IsEnabled = false;
            }
            else
            {
                TaskPicker.IsEnabled = true;
                SwitchTimer.IsEnabled = true;
            }
        }

        public async void StudyTimeClear()
        {
            var stats = await StatisticsServices.GetStatistics();
            foreach(var stat in stats)
            {
                await App.Current.MainPage.DisplayAlert("Statisztika-Adat", $"ID: {stat.ID} - Kezdés: {stat.Started:d}\nIdő: {stat.Time:t}\nTask és SID: {stat.TaskID} - {stat.SubjectID}\nTaskConn és TaskDone: {stat.TaskConnection} {stat.IsDone}", "Ok");
            }    
        }

        public async void StudyTimeSave()
        {
            /*if(StudyTime.TotalSeconds < 60)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"), 
                    AppResources.ResourceManager.GetString("TimerAtLeast1Min"),"Ok");
            }*/
            Running = false;
            if(IsFreeTimer == false)
            {
                bool complete = await App.Current.MainPage.DisplayAlert("", AppResources.ResourceManager.GetString("AreYouSureDoneTask"),
                    AppResources.ResourceManager.GetString("Cancel"), AppResources.ResourceManager.GetString("Yes"));
                if(!complete)
                {
                    STask selectedTask = TaskPicker.SelectedItem as STask;
                    selectedTask.IsDone = true;
                    await STaskServices.UpdateTask(selectedTask);
                    await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Congratulations"),
                        $"{AppResources.ResourceManager.GetString("TimerSuccessfullySaved")}\n{AppResources.ResourceManager.GetString("TimerSpent")} {StudyTime:t} {AppResources.ResourceManager.GetString("TimerTask")} {selectedTask.Name}", "Ok");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Congratulations"),
                    $"{AppResources.ResourceManager.GetString("TimerSuccessfullySaved")}\n{AppResources.ResourceManager.GetString("TimerSpent")} {StudyTime:t}", "Ok");
            }
            CurrentStats.IsDone = true;
            CurrentStats.TaskConnection = !IsFreeTimer;
            CurrentStats.Time = StudyTime;
            await StatisticsServices.UpdateStatistics(CurrentStats);
            StudyTime = new TimeSpan();
            ImportTasks();
            IconChanging = "\uec74";
        }     

        public async void TimerSwitch()
        {
            try
            {
                if (Running == false)
                {
                    if (StudyTime.TotalSeconds == 0)
                    {
                        if (IsFreeTimer == false && TaskList.Count != 0)
                        {
                            STask selectedTask = TaskPicker.SelectedItem as STask;
                            CurrentStats.TaskConnection = true;
                            CurrentStats.TaskID = selectedTask.ID;
                            CurrentStats.SubjectID = selectedTask.SubjectID;
                        }
                        else
                        {
                            CurrentStats.TaskID = -1;
                            CurrentStats.SubjectID = -1;
                        }
                        CurrentStats.Started = DateTime.Now;
                        await StatisticsServices.AddStatistics(CurrentStats);
                    }
                    else
                    {
                        TaskPicker.SelectedItem = TaskList.Where(x => x.ID == CurrentStats.TaskID);
                    }
                    SwitchTimer.IsEnabled = false;
                    TaskPicker.IsEnabled = false;
                    IconChanging = "\uec72";
                    Running = true;
                    int count = 0;
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        if (Running)
                        {

                            StudyTime = StudyTime.Add(new TimeSpan(0, 0, 1));
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                if (count == 45)
                                {
                                    count = 0;
                                    CurrentStats.Time = StudyTime;
                                    await StatisticsServices.UpdateStatistics(CurrentStats);
                                    await App.Current.MainPage.DisplayAlert("Success", "Stat is updated!", "Ok");
                                }
                            });
                            count++;
                        }
                        return Running;
                    });
                }
                else
                {
                    CurrentStats.Time = StudyTime;
                    await StatisticsServices.UpdateStatistics(CurrentStats);
                    Running = false;
                    IconChanging = "\uec74";
                }
            }
            catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"), 
                    $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }
        public async Task InitializeStats()
        {
            try
            {
                var stats = await StatisticsServices.GetStatistics();
                CurrentStats = new Statistics();
                if (stats != null && stats.Count() != 0)
                {
                    var orderedStats = from stat in stats
                                       orderby stat.ID descending
                                       select stat;
                    foreach (var stat in orderedStats)
                    {
                        if (stat.IsDone == false)
                        {
                            CurrentStats = stat;
                            SwitchTimer.IsEnabled = false;
                            TaskPicker.IsEnabled = false;
                            StudyTime = CurrentStats.Time;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
                    $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
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
