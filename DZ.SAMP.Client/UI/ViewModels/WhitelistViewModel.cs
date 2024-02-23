using System.Threading.Tasks;

namespace DZ.SAMP.Client.UI.ViewModels
{
    public class WhitelistViewModel : ServerListViewModelBase
    {
        public WhitelistViewModel(MainViewModel vm) : base(vm)
        {
            Task.Factory.StartNew(() => base.Get("https://sa-multiplayer.com/servers/whitelist.txt"));
        }
    }
}
