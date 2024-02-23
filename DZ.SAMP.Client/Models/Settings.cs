using DZ.SAMP.Client.Settings;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DZ.SAMP.Client.Models
{
    public class Settings : AbstractXMLSettings
    {
        #region Properties
        public string PlayerName
        {
            get => playerName;
            set
            {
                playerName = value;

                Save();
            }
        }

        public ObservableCollection<Server> Favorites
        {
            get => favorites;
            set
            {
                favorites = value;

                Save();
            }
        }

        public ObservableCollection<Server> History
        {
            get => history;
            set
            {
                history = value;

                Save();
            }
        }

        public Language Language
        {
            get => language;
            set
            {
                language = value;

                Save();
            }
        }

        public string SingleplayerLocation
        {
            get => singleplayerLocation;
            set
            {
                singleplayerLocation = value;

                Save();
            }
        }
        #endregion

        #region Events
        public event EventHandler FavoritesChanged;
        public event EventHandler HistoryChanged;
        #endregion

        #region Fields
        private string playerName;
        private string singleplayerLocation;
        private Language language;
        private ObservableCollection<Server> favorites;
        private ObservableCollection<Server> history;
        #endregion

        public Settings() : base(true)
        {
            try
            {
                PropertyChanged += (_, __) =>
                {
                    Save();
                };

                this.Favorites.CollectionChanged += (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
                        FavoritesChanged?.Invoke(this, new EventArgs());
                };

                this.History.CollectionChanged += (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
                        HistoryChanged?.Invoke(this, new EventArgs());
                };

                if (this.Language == null)
                    this.Language = new Language("English", "en-US");
            }
            catch
            {

            }
        }
    }
}
