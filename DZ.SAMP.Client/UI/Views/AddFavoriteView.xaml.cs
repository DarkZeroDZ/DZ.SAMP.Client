using DZ.SAMP.Client.Models;
using DZ.SAMP.Client.Resources.Language;
using DZ.SAMP.Client.UI.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DZ.SAMP.Client.UI.Views
{
    /// <summary>
    /// Interaktionslogik für AddFavoriteView.xaml
    /// </summary>
    public partial class AddFavoriteView : UserControl
    {
        #region Properties
        public ServerListViewModelBase VM { get; set; }
        public string IPAddress { get; set; }
        #endregion

        public AddFavoriteView(ServerListViewModelBase vm)
        {
            this.InitializeComponent();

            this.DataContext = this;

            this.VM = vm;
        }
        private async void ConnectClick(object sender, RoutedEventArgs e)
        {
            Server server;

            if(this.IPAddress.Contains(':'))
            {
                if (this.VM.Servers.Any(x => x.IPAddress == this.IPAddress.Split(':')[0] && x.Port == this.IPAddress.Split(':')[1]))
                {
                    MessageBox.Show(LauncherStrings.ServerAlreadyInFavorites, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                server = this.VM.ServerStatsConnector.GetServerProperties(this.IPAddress).Result;
            }
            else
            {
                if (this.VM.Servers.Any(x => x.IPAddress == this.IPAddress && x.Port == "7777"))
                {
                    MessageBox.Show(LauncherStrings.ServerAlreadyInFavorites, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var formattedIp = this.IPAddress + ":7777";
                server = this.VM.ServerStatsConnector.GetServerProperties(formattedIp).Result;
            }

            if (server.Name != null)
                this.VM.VM.Settings.Favorites.Add(server);
            else
                MessageBox.Show(LauncherStrings.InvalidIP, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);

            this.VM.VM.DialogObject = null;
            this.VM.VM.IsShow = false;

            this.VM.VM.Settings.Save();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.VM.VM.DialogObject = null;
            this.VM.VM.IsShow = false;
        }
    }
}
