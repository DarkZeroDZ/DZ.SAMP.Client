using System.Threading.Tasks;

namespace DZ.SAMP.Client.UI.ViewModels
{
    public class FavoritesViewModel : ServerListViewModelBase
    {
        public FavoritesViewModel(MainViewModel vm) : base(vm)
        {
            if (base.VM.Settings.Favorites.Count != 0)
                base.HasNoFavorites = false;
            else
                base.HasNoFavorites = true;

            foreach (var item in base.VM.Settings.Favorites)
            {
                var newServer = Task.Run(async () =>
                {
                    return await this.GetServerProperties(item);
                });

                if (!string.IsNullOrEmpty(newServer.Result.Name))
                    base.Servers.Add(newServer.Result);
            }

            base.OnPropertyChanged();

            base.VM.Settings.FavoritesChanged += (_, __) =>
            {
                base.Servers.Clear();

                if (base.VM.Settings.Favorites.Count != 0)
                    base.HasNoFavorites = false;
                else
                    base.HasNoFavorites = true;

                foreach (var item in base.VM.Settings.Favorites)
                {
                    var newServer = Task.Run(async () =>
                    {
                        return await this.GetServerProperties(item);
                    });

                    if (!string.IsNullOrEmpty(newServer.Result.Name))
                        base.Servers.Add(newServer.Result);
                }

                base.OnPropertyChanged();
            };
        }

        public async Task<Models.Server> GetServerProperties(Models.Server item)
        {
            return await base.ServerStatsConnector.GetServerProperties(item.FormattedIP);
        }
    }
}
