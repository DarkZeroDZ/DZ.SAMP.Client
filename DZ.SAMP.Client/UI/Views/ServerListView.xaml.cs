using DZ.SAMP.Client.Models;
using DZ.SAMP.Client.Resources.Language;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

namespace DZ.SAMP.Client.UI.Views
{
    /// <summary>
    /// Interaktionslogik für InternetView.xaml
    /// </summary>
    public partial class ServerListView : UserControl
    {
        private ListSortDirection playersSortDirection = ListSortDirection.Ascending;

        public ServerListView()
        {
            this.InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.ToString()));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.Header.ToString() == LauncherStrings.Players)
            {
                e.Handled = true;

                if (playersSortDirection == ListSortDirection.Ascending)
                {
                    playersSortDirection = ListSortDirection.Descending;
                }
                else
                {
                    playersSortDirection = ListSortDirection.Ascending;
                }

                var dataGrid = sender as DataGrid;
                if (dataGrid != null)
                {
                    var collectionView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource) as ListCollectionView;
                    if (collectionView != null)
                    {
                        collectionView.CustomSort = new PlayersComparer(playersSortDirection);
                    }
                }
            }
        }
    }

    public class PlayersComparer : IComparer
    {
        private ListSortDirection sortDirection;

        public PlayersComparer(ListSortDirection direction)
        {
            sortDirection = direction;
        }

        public int Compare(object x, object y)
        {
            if (x is Server serverX && y is Server serverY)
            {
                int result = serverX.Players.CompareTo(serverY.Players);
                return sortDirection == ListSortDirection.Ascending ? result : -result;
            }
            throw new ArgumentException("Objects could not be compared.");
        }
    }
}
