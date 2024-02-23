using DZ.SAMP.Client.Business;
using DZ.SAMP.Client.Models;
using DZ.SAMP.Client.MVVM;
using DZ.SAMP.Client.Resources.Language;
using DZ.SAMP.Client.UI.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace DZ.SAMP.Client.UI.ViewModels
{
    public class ServerListViewModelBase : ViewModelBase
    {
        #region Properties
        public bool ExcludeFullServers
        {
            get => _excludeFullServers;
            set
            {
                _excludeFullServers = value;

                this.UpdateFilter();
            }
        }
        public bool ExcludeEmptyServers
        {
            get => _excludeEmptyServers;
            set
            {
                _excludeEmptyServers = value;

                this.UpdateFilter();
            }
        }
        public bool ExcludeLockedServers
        {
            get => _excludeLockedServers;
            set
            {
                _excludeLockedServers = value;

                this.UpdateFilter();
            }
        }
        public bool IsLoading { get; set; }
        public ObservableCollection<Server> Servers
        {
            get { return _servers; }
            set
            {
                if (_servers != value)
                {
                    _servers = value;
                    OnPropertyChanged(nameof(Servers));
                }
            }
        }

        public ICollectionView FilteredServerList { get; set; }

        public string SearchFilter
        {
            get => _searchFilter;
            set
            {
                _searchFilter = value;

                this.UpdateFilter();
            }
        }

        public MasterlistConnector MasterlistConnector { get; }
        public ServerStatsConnector ServerStatsConnector { get; set; }
        public MainViewModel VM { get; set; }
        public bool HasNoFavorites { get; set; }
        public bool HasNoHistory { get; set; }
        public Server DisplayedServer
        {
            get => this._displayedServer;
            set
            {
                if (!this._setByDisplayer)
                {
                    if (value != null)
                        this.Server = value;

                    this._displayedServer = value;

                    this.OnPropertyChanged();
                }
            }
        }

        //public Server Server
        //{
        //    get => this._server;
        //    set
        //    {
        //        int index = this.Servers.IndexOf(this.Server);

        //        if (value != null)
        //        {
        //            this._setByDisplayer = true;
        //            Application.Current.Dispatcher.Invoke(() =>
        //            {
        //                if (index != -1)
        //                {
        //                    this._setByDisplayer = true;
        //                    Servers.RemoveAt(index);
        //                    Servers.Insert(index, value);
        //                }
        //            });
        //            this._setByDisplayer = false;
        //        }

        //        this._server = value;

        //        if (this._displayedServer != null && value != null)
        //        {
        //            if (this._displayedServer.IPAddress == value.IPAddress)
        //            {
        //                this._setByDisplayer = true;
        //                this._displayedServer = value;
        //                this._setByDisplayer = false;
        //            }
        //        }
        //        this.OnPropertyChanged();
        //    }
        //}
        public Server Server
        {
            get => this._server;
            set
            {             
                this._server = value;
                
                this.OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand JoinServerCommand => new RelayCommand(this.JoinServer);
        public ICommand CopyServerPropertiesCommand => new RelayCommand(this.CopyServerProperties);
        public ICommand AddAsFavoriteCommand => new RelayCommand(this.AddAsFavorite);
        public ICommand AddFavoriteCommand => new RelayCommand(this.AddFavorite);
        public ICommand DeleteFromFavoritesCommand => new RelayCommand(this.DeleteFromFavorites);
        #endregion

        #region Fields
        private Server _server;
        private string _searchFilter;
        private bool _excludeFullServers;
        private bool _excludeEmptyServers;
        private bool _excludeLockedServers;
        private static object _timerLock = new object();
        private ObservableCollection<Server> _servers;
        private Server _displayedServer;
        private bool _setByDisplayer;
        #endregion

        public ServerListViewModelBase(MainViewModel vm)
        {
            this.Servers = new ObservableCollection<Server>();

            var source = new CollectionViewSource { Source = this.Servers };
            this.FilteredServerList = source.View;

            this.SearchFilter = string.Empty;

            this.MasterlistConnector = new MasterlistConnector();
            this.ServerStatsConnector = new ServerStatsConnector();

            this.VM = vm;

            var timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Enabled = true;

            timer.Elapsed += (_, __) =>
            {
                lock (_timerLock)
                {
                    if (this.Server != null && this.VM.IsShow == false)
                    {
                        if (!string.IsNullOrEmpty(this.Server.IPAddress))
                        {
                            //this.Server = this.ServerStatsConnector.GetServerProperties(this.Server.FormattedIP).Result;

                            var server = this.ServerStatsConnector.GetServerProperties(this.Server.FormattedIP).Result;

                            if (this.Server != null)
                            {
                                if (server.IPAddress == this.Server.IPAddress)
                                {
                                    this.Server.Name = server.Name;

                                    this.OnPropertyChanged();
                                }
                            }
                        }
                    }
                }
            };
        }

        public void Get(string url)
        {
            try
            {
                this.IsLoading = true;

                foreach (var ip in this.MasterlistConnector.GetServers(url))
                {
                    var server = this.ServerStatsConnector.GetServerProperties(ip);

                    if (server.Result.Ping == 0)
                        continue;

                    if(Application.Current != null)
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ParameterizedThreadStart(AddItem), server.Result);

                    if (this.Server == null)
                        this.Server = this.Servers?.FirstOrDefault();
                }

                this.IsLoading = false;

                this.OnPropertyChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                this.IsLoading = false;
                this.OnPropertyChanged();
            }
        }

        private void JoinServer()
        {
            if (this.Server == null)
                return;

            this.VM.DialogObject = new ConnectorView(this);
            this.VM.IsShow = true;
        }

        private void AddItem(object server)
            => this.Servers.Add((Server)server);

        private void UpdateFilter()
        {
            try
            {
                this.FilteredServerList.Filter = new Predicate<object>(item =>
                {
                    var server = (Server)item;
                    var searchFilter = this.SearchFilter.ToLower();
                    var isNameValid = server.Name != null && server.Name.ToLower().Contains(searchFilter);
                    var isModeValid = server.Mode != null && server.Mode.ToLower().Contains(searchFilter);
                    var isLanguageValid = server.Language != null && server.Language.ToLower().Contains(searchFilter);
                    var isMapValid = server.Map != null && server.Map.ToLower().Contains(searchFilter);

                    if (this.ExcludeEmptyServers && !this.ExcludeFullServers && !this.ExcludeLockedServers)
                    {
                        return isNameValid || isModeValid || isLanguageValid || isMapValid;
                    }
                    else if (!this.ExcludeEmptyServers && this.ExcludeFullServers && !this.ExcludeLockedServers)
                    {
                        return isNameValid || isModeValid || isLanguageValid || isMapValid && (server.Players != server.MaxPlayers);
                    }
                    else if (!this.ExcludeEmptyServers && !this.ExcludeFullServers && this.ExcludeLockedServers)
                    {
                        return isNameValid || isModeValid || isLanguageValid || isMapValid && !server.IsPassword;
                    }
                    else if (this.ExcludeEmptyServers && this.ExcludeFullServers && !this.ExcludeLockedServers)
                    {
                        return isNameValid || isModeValid || isLanguageValid || isMapValid && (server.Players != 0) && (server.Players != server.MaxPlayers);
                    }
                    else if (!this.ExcludeEmptyServers && this.ExcludeFullServers && this.ExcludeLockedServers)
                    {
                        return isNameValid || isModeValid || isLanguageValid || isMapValid && (server.Players != server.MaxPlayers) && !server.IsPassword;
                    }
                    else if (this.ExcludeEmptyServers && !this.ExcludeFullServers && this.ExcludeLockedServers)
                    {
                        return isNameValid || isModeValid || isLanguageValid || isMapValid && !server.IsPassword && (server.Players != 0);
                    }
                    else if (this.ExcludeEmptyServers && this.ExcludeFullServers && this.ExcludeLockedServers)
                    {
                        return isNameValid || isModeValid || isLanguageValid || isMapValid && !server.IsPassword && (server.Players != 0) && (server.Players != server.MaxPlayers);
                    }
                    else
                    {
                        return isNameValid || isModeValid || isLanguageValid || isMapValid;
                    }
                });
                this.OnPropertyChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CopyServerProperties()
        {
            if (this.Server == null)
            {
                MessageBox.Show(LauncherStrings.NoServerSelected, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("Hostname: " + this.Server.Name);
            builder.AppendLine("Address:  " + this.Server.FormattedIP);
            builder.AppendLine("Players:  " + this.Server.FormattedPlayers);
            builder.AppendLine("Ping:     " + this.Server.Ping);
            builder.AppendLine("Mode:     " + this.Server.Mode);
            builder.AppendLine("Language: " + this.Server.Language);

            Clipboard.SetText(builder.ToString());
        }

        private void AddAsFavorite()
        {
            if (this.Server == null)
            {
                MessageBox.Show(LauncherStrings.NoServerSelected, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.VM.CurrentTab = 0;
            this.VM.Settings.Favorites.Add(this.Server);

            this.VM.Settings.Save();
        }

        private void AddFavorite()
        {
            this.VM.DialogObject = new AddFavoriteView(this);
            this.VM.IsShow = true;
        }

        private void DeleteFromFavorites()
        {
            if (this.Server == null)
            {
                MessageBox.Show(LauncherStrings.NoServerSelected, LauncherStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var item in this.VM.Settings.Favorites.Where(x => x.IPAddress == this.Server.IPAddress).ToList())
                this.VM.Settings.Favorites.Remove(item);

            this.VM.Settings.Save();
        }
    }
}
