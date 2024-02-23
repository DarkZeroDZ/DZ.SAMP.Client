using DZ.SAMP.Client.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DZ.SAMP.Client.UI.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        #region Properties
        public SettingsViewModel VM { get; set; }
        #endregion

        public SettingsView(SettingsViewModel vm)
        {
            InitializeComponent();

            this.DataContext = this;

            this.VM = vm;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.VM.VM.DialogObject = null;
            this.VM.VM.IsShow = false;
        }
    }
}
