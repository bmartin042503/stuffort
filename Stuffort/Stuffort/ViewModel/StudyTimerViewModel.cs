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

        private Statistics currentstats;
        public Statistics CurrentStats
        {
            get { return currentstats; }
            set
            {
                currentstats = value;
                OnPropertyChanged(nameof(CurrentStats));
            }
        }

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
                SwitchTimer.IsEnabled = false;
            }
            else
            {
                TaskPicker.IsEnabled = true;
                SwitchTimer.IsEnabled = true;
            }
        }

        public void StudyTimeClear()
        {
            var stats = StatisticsServices.GetStatistics().Result;
            foreach(var stat in stats)
            {
                App.Current.MainPage.DisplayAlert("Statisztika-Adat", $"ID: {stat.ID} - Kezdés: {stat.Started:d}\nIdő: {stat.Time:t}\nTask és SID: {stat.TaskID} - {stat.SubjectID}\nTaskConn és TaskDone: {stat.TaskConnection} {stat.IsDone}", "Ok");
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
            if(CurrentStats.TaskConnection == true)
            {
                bool complete = await App.Current.MainPage.DisplayAlert("", AppResources.ResourceManager.GetString("AreYouSureDoneTask"),
                    AppResources.ResourceManager.GetString("Cancel"), AppResources.ResourceManager.GetString("Yes"));
                if(complete)
                {
                    var tasks = STaskServices.GetTasks().Result;
                    STask selectedTask = tasks.Where(x => x.ID == CurrentStats.TaskID) as STask;
                    selectedTask.IsDone = true;
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
            CurrentStats.TaskConnection = IsFreeTimer;
            CurrentStats.Time = StudyTime;
            await StatisticsServices.UpdateStatistics(CurrentStats);
            StudyTime = new TimeSpan();
            ImportTasks();
            IconChanging = "\uec74";
        }

        public async void SaveStat(Statistics s)
        {
            
        }     

        public async void TimerSwitch()
        {
            try
            {
                if (Running == false)
                {
                    Running = true;
                    if (StudyTime.TotalSeconds == 0)
                    {
                        if (IsFreeTimer == false && TaskList.Count != 0)
                        {
                            CurrentStats.TaskConnection = true;
                            CurrentStats.TaskID = TaskList[TaskPicker.SelectedIndex].ID;
                            var subjects = SubjectServices.GetSubjects().Result;
                            Subject selectedSubject = subjects.Where(x => x.ID == TaskList[TaskPicker.SelectedIndex].SubjectID) as Subject;
                            CurrentStats.SubjectID = selectedSubject.ID;
                        }
                        else
                        {
                            CurrentStats.TaskID = -1;
                            CurrentStats.SubjectID = -1;
                        }
                        CurrentStats.Started = DateTime.Now;
                        SwitchTimer.IsEnabled = false;
                        TaskPicker.IsEnabled = false;
                        await StatisticsServices.AddStatistics(CurrentStats);
                    }
                    IconChanging = "\uec72";
                    int count = 0;
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        Device.BeginInvokeOnMainThread(async () => {
                            if (count == 45)
                            {
                                await StatisticsServices.UpdateStatistics(CurrentStats);
                                count = 0;
                            }
                            count++;
                        });
                        StudyTime = StudyTime.Add(new TimeSpan(0, 0, 1));
                        return Running;
                    });
                }
                else
                {
                    Running = false;
                    IconChanging = "\uec74";
                    await StatisticsServices.UpdateStatistics(CurrentStats);
                }
            }
            catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"), 
                    $"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }
        public async Task InitializeStats()
        {
            var stats = await StatisticsServices.GetStatistics();
            CurrentStats = new Statistics();
            if (stats != null)
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
                        break;
                    }
                }
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
