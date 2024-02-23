using DZ.SAMP.Client.Business;
using DZ.SAMP.Client.UI.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DZ.SAMP.Client.UI.Views
{
    /// <summary>
    /// Interaktionslogik für ConnectorView.xaml
    /// </summary>
    public partial class ConnectorView : UserControl
    {
        #region Properties
        public ServerListViewModelBase VM { get; set; }
        public string Password { get; set; }
        public string RconPassword { get; set; }
        #endregion

        public ConnectorView(ServerListViewModelBase vm)
        {
            this.InitializeComponent();

            this.DataContext = this;

            this.VM = vm;
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            var connector = new GameConnector();
            connector.Connect(this.VM.Server, this.VM.VM.Settings.PlayerName, this.Password, this.RconPassword, this.VM.VM.Settings.SingleplayerLocation);

            this.VM.VM.DialogObject = null;
            this.VM.VM.IsShow = false;

            var server = this.VM.VM.Settings.History.FirstOrDefault(x => x.FormattedIP == this.VM.Server.FormattedIP);

            var index = this.VM.VM.Settings.History.IndexOf(server);

            if (index == -1)
            {
                this.VM.VM.Settings.History.Insert(0, this.VM.Server);
            }
            else
            {
                var serverCopy = this.VM.Server.Clone();
                this.VM.VM.Settings.History.RemoveAt(index);
                this.VM.VM.Settings.History.Insert(0, serverCopy);
                this.VM.Server = this.VM.VM.Settings.History[0];
            }

            this.VM.VM.Settings.Save();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.VM.VM.DialogObject = null;
            this.VM.VM.IsShow = false;
        }
    }
}
