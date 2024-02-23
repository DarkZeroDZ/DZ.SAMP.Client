using DZ.SAMP.Client.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DZ.SAMP.Client.UI.Views
{
    /// <summary>
    /// Interaktionslogik für InfoView.xaml
    /// </summary>
    public partial class InfoView : UserControl
    {
        #region Properties
        public MainViewModel VM { get; set; }
        #endregion

        public InfoView(MainViewModel vm)
        {
            InitializeComponent();

            this.DataContext = this;

            this.VM = vm;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.VM.DialogObject = null;
            this.VM.IsShow = false;
        }
    }
}
