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
                if(dateTimeOffset.Day == dateTimeNow.Day-1)
                    return $"{AppResources.ResourceManager.GetString("Added")} {AppResources.ResourceManager.GetString("Yesterday")}";
                if (diff.TotalSeconds < 60)
                    return $"{AppResources.ResourceManager.GetString("Added")} {diff.TotalSeconds:0} {AppResources.ResourceManager.GetString("SecondsAgo")}";
                if (diff.TotalMinutes < 60)
                    return $"{AppResources.ResourceManager.GetString("Added")} {diff.TotalMinutes:0} {AppResources.ResourceManager.GetString("MinutesAgo")}";
                if (diff.TotalHours < 24)
                    return $"{AppResources.ResourceManager.GetString("Added")} {diff.TotalHours:0} {AppResources.ResourceManager.GetString("HoursAgo")}";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTimeOffset.Now;
        }
    }
}
