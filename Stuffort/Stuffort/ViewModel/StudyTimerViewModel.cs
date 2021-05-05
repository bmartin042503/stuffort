using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;
using System.ComponentModel;
using Stuffort.Resources;

namespace Stuffort.ViewModel
{
    public class StudyTimerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool HasStarted { get; set; }

        private string timerstatus;
        public string TimerStatus
        {
            get { return timerstatus; }
            set
            {
                timerstatus = value;
                OnPropertyChanged(nameof(TimerStatus));
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
        public StudyTimerViewModel()
        {
            StudyTime = new TimeSpan();
            HasStarted = false;
            TimerStatus = string.Empty;
            TimerHandler = new Command(TimeSwitch);
        }

        public void TimeSwitch()
        {
            if (HasStarted == false)
            {
                TimerStatus = "Stop";
                HasStarted = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    StudyTime.Add(new TimeSpan(0,0,1));
                    return HasStarted;
                });
            }
            else
            {
                HasStarted = false;
                TimerStatus = "Start";
            }
        }


    }
}

public class StudyTimerViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged == null)
            return;

        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool HasStarted { get; set; }

    private string timerstatus;
    public string TimerStatus
    {
        get { return timerstatus; }
        set
        {
            timerstatus = value;
            OnPropertyChanged(nameof(TimerStatus));
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
    public StudyTimerViewModel()
    {
        StudyTime = new TimeSpan();
        HasStarted = false;
        TimerStatus = AppResources.ResourceManager.GetString("Start");
        TimerHandler = new Command(TimeSwitch);
    }

    public void TimeSwitch()
    {
        if (HasStarted == false)
        {
            TimerStatus = AppResources.ResourceManager.GetString("Stop");
            HasStarted = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                StudyTime = StudyTime.Add(new TimeSpan(0,0,1));
                return HasStarted;
            });
        }
        else
        {
            HasStarted = false;
            TimerStatus = AppResources.ResourceManager.GetString("Start");
        }
    }


}
