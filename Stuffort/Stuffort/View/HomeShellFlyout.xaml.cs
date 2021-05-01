using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stuffort.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeShellFlyout : ContentPage
    {
        public ListView ListView;

        public HomeShellFlyout()
        {
            InitializeComponent();

            BindingContext = new HomeShellFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        class HomeShellFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<HomeShellFlyoutMenuItem> MenuItems { get; set; }

            public HomeShellFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<HomeShellFlyoutMenuItem>(new[]
                {
                    new HomeShellFlyoutMenuItem { Id = 0, Title = "Page 1" },
                    new HomeShellFlyoutMenuItem { Id = 1, Title = "Page 2" },
                    new HomeShellFlyoutMenuItem { Id = 2, Title = "Page 3" },
                    new HomeShellFlyoutMenuItem { Id = 3, Title = "Page 4" },
                    new HomeShellFlyoutMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}