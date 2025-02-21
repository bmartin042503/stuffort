﻿using Stuffort.Resources;
using Stuffort.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcharts;
using Microcharts.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Stuffort.Model;
using System.Diagnostics;
using SkiaSharp;

namespace Stuffort.View.ShellPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatsPage : ContentPage
    {
        public StatsViewModel StatsViewModel;
        public StatsPage()
        {
            InitializeComponent();
            this.StatsViewModel = new StatsViewModel();
            BindingContext = this.StatsViewModel;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            this.Title = AppResources.StatsPage;
            longestSessionLbl.Text = AppResources.LongestSession;
            timerSpentLbl.Text = AppResources.TimerSpent;
            var stats = await StatisticsServices.GetStatistics();
            if(stats == null || stats.Count() == 0)
            {
                await DisplayAlert(AppResources.Error, AppResources.NoStats, "Ok");
                await Shell.Current.GoToAsync($"//{nameof(SubjectsPage)}");
                return;
            }
            this.StatsViewModel.NumberOf_Subjects();
            this.StatsViewModel.Completed_Tasks();
            this.StatsViewModel.CountOf_Sessions();
            this.StatsViewModel.Longest_Session();
            this.StatsViewModel.AllSessions_Time();
            var data = from stat in stats
                       group stat by stat.SubjectName
                       into newlist
                       select newlist;
            List<ChartEntry> EntryList1 = new List<ChartEntry>();
            foreach(var item in data)
            {
                var timeSum = item.Sum(x => x.Time.TotalSeconds);
                string itemName = item.Key == "UNDEFINED" ? AppResources.UndefinedSubject : item.Key;
                itemName = LocalLongnameConverter(itemName); 
                SKColor color = item.Key == "UNDEFINED" ? SKColor.Parse("#000000") : SKColor.Parse("#528fdb");
                string converted = ConvertToHourMinSec((long)timeSum);
                EntryList1.Add(new ChartEntry((float)timeSum / 100) { Label=itemName, ValueLabel=converted, Color = color });
            }
            statsViewSubjects.Chart = new PointChart{ Entries = EntryList1, LabelTextSize = 30, IsAnimated=true, AnimationProgress = 1.5f,
               PointMode=PointMode.Circle, ValueLabelOrientation = Orientation.Horizontal, BackgroundColor=SKColor.Empty};
            List <ChartEntry> EntryList2 = new List<ChartEntry>();
            var ydata = from stat in stats
                        where stat.Finished.Year == DateTime.Now.Year
                        group stat by stat.Finished.Month
                        into newlist
                        select newlist;
            string monthName = string.Empty;
            foreach(var item in ydata)
            {
                switch(item.Key)
                {
                    case 1: monthName = AppResources.January; break;
                    case 2: monthName = AppResources.February; break;
                    case 3: monthName = AppResources.March; break;
                    case 4: monthName = AppResources.April; break;
                    case 5: monthName = AppResources.May; break;
                    case 6: monthName = AppResources.June; break;
                    case 7: monthName = AppResources.July; break;
                    case 8: monthName = AppResources.August; break;
                    case 9: monthName = AppResources.September; break;
                    case 10: monthName = AppResources.October; break;
                    case 11: monthName = AppResources.November; break;
                    case 12: monthName = AppResources.December; break;
                }
                var timeSum = item.Sum(x => x.Time.TotalSeconds);
                string converted = ConvertToHourMinSec((long)timeSum);
                EntryList2.Add(new ChartEntry((float)timeSum / 100) { Label=monthName, ValueLabel=converted, Color = SKColor.Parse("#528fdb") });
            }
            statsViewYear.Chart = new BarChart { Entries = EntryList2, LabelTextSize = 30, IsAnimated = true, AnimationProgress=1.5f, ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal, BackgroundColor=SKColor.Empty};
        }

        public string LocalLongnameConverter(string val)
        {
            int uppers = 0;
            for (int i = 0; i < val.Length; ++i)
                if (Char.IsUpper(val[i])) uppers++;
            if (val.Length > 6 && uppers > 3)
                return $"{val.Substring(0, 6)}..";
            if (val.Length > 10)
                return $"{val.Substring(0, 10)}..";
            return val;
        }

        public string ConvertToHourMinSec(long seconds)
        {
            int hour = 0, min = 0, sec = 0;
            if(seconds > 60)
            {
                min = (int)seconds / 60;
                sec = (int)seconds % 60;
            }
            if(min > 60)
            {
                hour = (int)min / 60;
                min = (int)min % 60;
            }
            if (hour >= 1 && hour < 24) return $"{hour}{AppResources.Hours} {min}{AppResources.Minutes}";
            else if(hour >= 24) return $"{hour}{AppResources.Hours}";
            else return $"{min}{AppResources.Minutes}";
        }
    }
}