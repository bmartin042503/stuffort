using Stuffort.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Stuffort.ViewModel.Converters
{
    public class DeadlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTimeOffset dTime = (DateTimeOffset)value;
            if (dTime.Year == 1900 && dTime.Month == 1 && dTime.Day == 1)
                return AppResources.ResourceManager.GetString("NoDeadline");
            else
            {
                return $"{AppResources.ResourceManager.GetString("Deadline")} {dTime:d}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
