using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Stuffort.ViewModel.Converters
{
    public class LongNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value as string;
            int uppers = 0;
            for(int i = 0; i < val.Length; ++i)
                if (Char.IsUpper(val[i])) uppers++;
            if (val.Length > 14 && uppers > 3)
                return $"{val.Substring(0,14)}..";
            if (val.Length > 21)
                return $"{val.Substring(0,21)}..";
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
