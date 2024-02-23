using System.Threading.Tasks;

namespace DZ.SAMP.Client.UI.ViewModels
{
    public class HistoryViewModel : ServerListViewModelBase
    {
        public HistoryViewModel(MainViewModel vm) : base(vm)
        {
            if (base.VM.Settings.History.Count != 0)
                base.HasNoHistory = false;
            else
                base.HasNoHistory = true;

            foreach (var item in base.VM.Settings.History)
            {
                if (item == null)
                    continue;

                var newServer = Task.Run(async () =>
                {
                    return await this.GetServerProperties(item);
                });

                if (!string.IsNullOrEmpty(newServer.Result.Name))
                    base.Servers.Add(newServer.Result);
            }

            base.OnPropertyChanged();

            base.VM.Settings.HistoryChanged += (_, __) =>
            {
                base.Servers.Clear();

                if (base.VM.Settings.History.Count != 0)
                    base.HasNoHistory = false;
                else
                    base.HasNoHistory = true;

                foreach (var item in base.VM.Settings.History)
                {
                    if (item == null)
                        continue;

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
