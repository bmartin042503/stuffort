using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;
using System.ComponentModel;
using Stuffort.Resources;
using Stuffort.Model;

namespace Stuffort.ViewModel
{
    public class StudyTimerViewModel : INotifyPropertyChanged
    {
        public bool IsFreeTimer { get; set; }
        public List<STask> TaskList { get; set; }

        private Switch FreeTimerSwitch;
        private Picker TaskPicker;
        private bool HasStarted;

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

        public Command TimerHandler { get; set; }
        public StudyTimerViewModel(Switch sw, Picker pi)
        {
            IconChanging = "\uec74;";
            FreeTimerSwitch = sw;
            TaskPicker = pi;
            StudyTime = new TimeSpan();
            HasStarted = false;
            IsFreeTimer = false;
            TimerHandler = new Command(TimeSwitch);
            TaskList = new List<STask>();
        }

        public async void UpdateTasks()
        {
            TaskList.Clear();
            var tasks = await STaskServices.GetTasks();
            foreach (var task in tasks)
                TaskList.Add(task);
        }

        public async void TimeSwitch()
        {
            if (HasStarted == false)
            {
                HasStarted = true;
                IconChanging = "\uec72;";
                TaskPicker.IsEnabled = false;
                FreeTimerSwitch.IsEnabled = false;
                Statistics newStat = new Statistics();
                newStat.Started = DateTime.Now;
                newStat.Time = new TimeSpan();
                if (!IsFreeTimer)
                {
                    newStat.SubjectID = (TaskPicker.SelectedItem as STask).SubjectID;
                    newStat.TaskID = (TaskPicker.SelectedItem as STask).ID;
                }
                else
                {
                    newStat.SubjectID = -1;
                    newStat.TaskID = -1;
                }
                await StatisticsServices.AddStatistics(newStat);
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    StudyTime = StudyTime.Add(new TimeSpan(0,0,1));
                    return HasStarted;
                });
            }
            else
            {
                HasStarted = false;
                IconChanging = "\uec74;";
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
