using DZ.SAMP.Client.Business;
using DZ.SAMP.Client.MVVM;
using DZ.SAMP.Client.UI.Views;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace DZ.SAMP.Client.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public Page CurrentPage { get; set; }
        public int CurrentTab
        {
            get => currentTab;
            set
            {
                currentTab = value;

                this.OnPropertyChanged();
            }
        }
        public bool IsHostedTabActive
        {
            get => isHostedTabActive; 
            set
            {
                isHostedTabActive = value;

                this.OnPropertyChanged();
            }
        }
        public Models.Settings Settings { get; set; }
        public InternetViewModel InternetViewModel { get; set; }
        public HostedViewModel HostedViewModel { get; set; }
        public FavoritesViewModel FavoritesViewModel { get; set; }
        public SettingsViewModel SettingsViewModel { get; set; }
        public WhitelistViewModel WhitelistViewModel { get; set; }
        public HistoryViewModel HistoryViewModel { get; set; }
        public bool IsShow
        {
            get => _isShow;
            set
            {
                _isShow = value;
                OnPropertyChanged();
            }
        }
        public object DialogObject
        {
            get => _dialogObject;
            set
            {
                if (_dialogObject == value)
                    return;

                _dialogObject = value;
                OnPropertyChanged();
            }
        }
        public bool IsUpdateAvailable
        {
            get => isUpdateAvailable;
            set
            {
                isUpdateAvailable = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand ShowInfoCommand => new RelayCommand(this.ShowInfo);
        public ICommand ShowUpdateCommand => new RelayCommand(this.ShowUpdate);
        public ICommand ShowSettingsCommand => new RelayCommand(this.ShowSettings);
        #endregion

        #region Fields
        private bool _isShow;
        private object _dialogObject;
        private int currentTab;
        private bool isUpdateAvailable;
        private bool isHostedTabActive;
        #endregion

        public MainViewModel()
        {
            this.CurrentPage = new MainPage(this);

            this.Settings = new Models.Settings();

            this.InternetViewModel = new InternetViewModel(this);
            this.HostedViewModel = new HostedViewModel(this);
            this.WhitelistViewModel = new WhitelistViewModel(this);
            this.FavoritesViewModel = new FavoritesViewModel(this);
            this.SettingsViewModel = new SettingsViewModel(this);
            this.HistoryViewModel = new HistoryViewModel(this);

            // Update check
            var updateConnector = new UpdateConnector();

            var latestVersion = updateConnector.GetLatestVersion();
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (latestVersion != currentVersion && latestVersion != string.Empty)
                this.IsUpdateAvailable = true;
        }

        private void ShowInfo()
        {
            this.DialogObject = new InfoView(this);
            this.IsShow = true;
        }

        private void ShowUpdate()
        {
            Process.Start(new ProcessStartInfo("https://sa-multiplayer.com/?page_id=20"));
        }

        private void ShowSettings()
        {
            this.DialogObject = new SettingsView(this.SettingsViewModel);
            this.IsShow = true;

            this.OnPropertyChanged();
        }
    }
}
