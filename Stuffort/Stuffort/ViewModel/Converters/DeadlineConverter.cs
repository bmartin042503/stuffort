using Stuffort.Model;
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
            STask task = value as STask;
            if (task.IsDeadline)
            {
                DateTimeOffset dTime = task.DeadLine;
                return $"{AppResources.ResourceManager.GetString("Deadline")}\n{dTime:g}";
            }
            else return AppResources.ResourceManager.GetString("NoDeadline");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
