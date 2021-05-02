using Stuffort.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Stuffort.ViewModel.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
            DateTimeOffset dateTimeNow = DateTimeOffset.Now;
            var diff = dateTimeNow - dateTimeOffset;
            if (diff.TotalDays > 1)
                return $"{dateTimeOffset:d}";
            else
            {
                if(diff.TotalSeconds < 60)
                    return $"{diff.TotalSeconds:0} {AppResources.ResourceManager.GetString("SecondsAgo")}";
                if (diff.TotalMinutes < 60)
                    return $"{diff.TotalMinutes:0} {AppResources.ResourceManager.GetString("MinutesAgo")}";
                if (diff.TotalHours < 24)
                    return $"{diff.TotalHours:0} {AppResources.ResourceManager.GetString("HoursAgo")}";

                return AppResources.ResourceManager.GetString("Yesterday");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTimeOffset.Now;
        }
    }
}
