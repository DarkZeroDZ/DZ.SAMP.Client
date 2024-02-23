using DZ.SAMP.Client.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DZ.SAMP.Client.UI.Views
{
    /// <summary>
    /// Interaktionslogik für MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();

            this.DataContext = vm;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InternetViewGrid.Height = this.ActualHeight;
            this.HostedViewGrid.Height = this.ActualHeight;
            this.FavoritesViewGrid.Height = this.ActualHeight;
            this.WhitelistViewGrid.Height = this.ActualHeight;
            this.HistoryViewGrid.Height = this.ActualHeight;
        }
    }
}
