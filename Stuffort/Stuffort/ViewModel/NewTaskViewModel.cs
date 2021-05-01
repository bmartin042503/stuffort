using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Stuffort.ViewModel
{
    public class NewTaskViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
