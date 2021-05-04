using Stuffort.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Stuffort.ViewModel.Converters
{
    public class IsDoneToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var task = value as STask;
            DateTime savedDate = task.DeadLine.DateTime;
            if (task.IsDone) return "\ueed7";
            else if (task.IsDone == false && (savedDate-DateTime.Now).TotalDays <= 1 && task.IsDeadline == true) return "\uef19";
            else return "\ueff7";
            //&#xeed7; pipa
            //&#xef19; surgos
            //&#xeff7; ures

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
