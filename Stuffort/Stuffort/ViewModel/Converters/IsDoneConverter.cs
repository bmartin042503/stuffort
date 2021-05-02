using Stuffort.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Stuffort.ViewModel.Converters
{
    public class IsDoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDone = (bool)value;
            if (isDone == false)
                return $"{AppResources.ResourceManager.GetString("Status")} {AppResources.ResourceManager.GetString("Uncompleted")}";
            else
                return $"{AppResources.ResourceManager.GetString("Status")} {AppResources.ResourceManager.GetString("Completed")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
