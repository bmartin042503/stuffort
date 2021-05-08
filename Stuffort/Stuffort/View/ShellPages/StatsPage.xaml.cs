using Stuffort.Resources;
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
        public StatsPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var stats = await StatisticsServices.GetStatistics();
            var data = from stat in stats
                       group stat by stat.SubjectName
                       into newlist
                       select newlist;
            List<ChartEntry> EntryList1 = new List<ChartEntry>();
            foreach(var item in data)
            {
                var timeSum = item.Sum(x => x.Time.TotalSeconds);
                string itemName = item.Key == "UNDEFINED" ? AppResources.ResourceManager.GetString("UndefinedSubject") : item.Key;
                itemName = LocalLongnameConverter(itemName); 
                SKColor color = item.Key == "UNDEFINED" ? SKColor.Parse("#2e4f78") : SKColor.Parse("#528fdb");
                string converted = ConvertToHourMinSec((long)timeSum);
                EntryList1.Add(new ChartEntry((float)timeSum / 100) { Label=itemName, ValueLabel=converted, Color = color });
            }
            statsViewSubjects.Chart = new PointChart{ Entries = EntryList1, LabelTextSize = 40, IsAnimated=true, AnimationProgress = 1.5f,
               PointMode=PointMode.Circle, ValueLabelOrientation = Orientation.Horizontal};
            List<ChartEntry> EntryList2 = new List<ChartEntry>();
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
                    case 1: monthName = AppResources.ResourceManager.GetString("January"); break;
                    case 2: monthName = AppResources.ResourceManager.GetString("February"); break;
                    case 3: monthName = AppResources.ResourceManager.GetString("March"); break;
                    case 4: monthName = AppResources.ResourceManager.GetString("April"); break;
                    case 5: monthName = AppResources.ResourceManager.GetString("May"); break;
                    case 6: monthName = AppResources.ResourceManager.GetString("June"); break;
                    case 7: monthName = AppResources.ResourceManager.GetString("July"); break;
                    case 8: monthName = AppResources.ResourceManager.GetString("August"); break;
                    case 9: monthName = AppResources.ResourceManager.GetString("September"); break;
                    case 10: monthName = AppResources.ResourceManager.GetString("October"); break;
                    case 11: monthName = AppResources.ResourceManager.GetString("November"); break;
                    case 12: monthName = AppResources.ResourceManager.GetString("December"); break;
                }
                var timeSum = item.Sum(x => x.Time.TotalSeconds);
                string converted = ConvertToHourMinSec((long)timeSum);
                EntryList2.Add(new ChartEntry((float)timeSum / 100) { Label=monthName, ValueLabel=converted, Color = SKColor.Parse("#528fdb") });
            }
            statsViewYear.Chart = new BarChart { Entries = EntryList2, LabelTextSize = 40, IsAnimated = true, AnimationProgress=1.5f, ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal};
            this.Title = AppResources.ResourceManager.GetString("StatsPage");
        }

        public string LocalLongnameConverter(string val)
        {
            int uppers = 0;
            for (int i = 0; i < val.Length; ++i)
                if (Char.IsUpper(val[i])) uppers++;
            if (val.Length > 11 && uppers > 3)
                return $"{val.Substring(0, 11)}..";
            if (val.Length > 14)
                return $"{val.Substring(0, 14)}..";
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
            if (hour >= 1 && hour < 24) return $"{hour}{AppResources.ResourceManager.GetString("Hours")} {min}{AppResources.ResourceManager.GetString("Minutes")}";
            else if(hour >= 24) return $"{hour}{AppResources.ResourceManager.GetString("Hours")}";
            else return $"{min}{AppResources.ResourceManager.GetString("Minutes")}";
        }
    }
}