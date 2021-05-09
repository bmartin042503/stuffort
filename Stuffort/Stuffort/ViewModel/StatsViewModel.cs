using Stuffort.Model;
using Stuffort.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stuffort.ViewModel
{
    public class StatsViewModel : INotifyPropertyChanged
    {
        public int CurrentYear
        {
            get { return DateTime.Now.Year; }
        }

        private string numberofsubjects;
        public string NumberOfSubjects
        {
            get { return numberofsubjects; }
            set
            {
                numberofsubjects = value;
                OnPropertyChanged(nameof(NumberOfSubjects));
            }
        }

        private string completedtasks;
        public string CompletedTasks
        {
            get { return completedtasks; }
            set
            {
                completedtasks = value;
                OnPropertyChanged(nameof(CompletedTasks));
            }
        }

        private string countofsessions;
        public string CountOfSessions
        {
            get { return countofsessions; }
            set
            {
                countofsessions = value;
                OnPropertyChanged(nameof(CountOfSessions));
            }
        }

        private string longestsession;
        public string LongestSession
        {
            get { return longestsession; }
            set
            {
                longestsession = value;
                OnPropertyChanged(nameof(LongestSession));
            }
        }

        private string allsessionstime;
        public string AllSessionsTime
        {
            get { return allsessionstime; }
            set
            {
                allsessionstime = value;
                OnPropertyChanged(nameof(AllSessionsTime));
            }
        }
        public async void NumberOf_Subjects()
        {
            try
            {
                var subjects = await SubjectServices.GetSubjects();
                if (subjects == null)
                    NumberOfSubjects = $"{AppResources.ResourceManager.GetString("CountOfSubjects")} 0";
                else
                    NumberOfSubjects = $"{AppResources.ResourceManager.GetString("CountOfSubjects")} {subjects.Count()}";
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void Completed_Tasks()
        {
            try
            {
                var tasks = await STaskServices.GetTasks();
                if (tasks == null) CompletedTasks = $"{AppResources.ResourceManager.GetString("CompletedTasks")} 0";
                else
                {
                    var completed = from task in tasks
                                    where task.IsDone == true
                                    select task;
                    CompletedTasks = $"{AppResources.ResourceManager.GetString("CompletedTasks")} {completed.Count()}/{tasks.Count()}";
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void CountOf_Sessions()
        {
            try
            {
                var stats = await StatisticsServices.GetStatistics();
                if (stats == null)
                    CountOfSessions = $"{AppResources.ResourceManager.GetString("CountOfSessions")} 0";
                else
                    CountOfSessions = $"{AppResources.ResourceManager.GetString("CountOfSessions")} {stats.Count()}";
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void Longest_Session()
        {
            try
            {
                var stats = await StatisticsServices.GetStatistics();
                if (stats == null) LongestSession = "--";
                else
                {
                    var orderedstats = from stat in stats
                                       orderby stat.Time descending
                                       select stat;
                    var selected = orderedstats.First();
                    LongestSession = $"{selected.Time.Hours} {AppResources.ResourceManager.GetString("HoursL")} {selected.Time.Minutes} {AppResources.ResourceManager.GetString("MinutesL")} {selected.Time.Seconds} {AppResources.ResourceManager.GetString("SecondsL")}";
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(AppResources.ResourceManager.GetString("Error"),
$"{AppResources.ResourceManager.GetString("ErrorMessage")} {ex.Message}", "Ok");
            }
        }

        public async void AllSessions_Time()
        {
            try
            {
                var stats = await StatisticsServices.GetStatistics();
                if (stats == null) AllSessionsTime = "--";
                else
                {
                    var totaltime = stats.Sum(x => x.Time.TotalSeconds);
                    long seconds = (long)totaltime;
                    int hour = 0, min = 0, sec = 0;
                    if (seconds > 60)
                    {
                        min = (int)seconds / 60;
                        sec = (int)seconds % 60;
                    }
                    if (min > 60)
                    {
                        hour = (int)min / 60;
                        min = (int)min % 60;
                    }
                    AllSessionsTime = $"{hour} {AppResources.ResourceManager.GetString("HoursL")} {min} {AppResources.ResourceManager.GetString("MinutesL")} {sec} {AppResources.ResourceManager.GetString("SecondsL")}";
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
