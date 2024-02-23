using DZ.SAMP.Client.Business;
using System.Threading.Tasks;

namespace DZ.SAMP.Client.UI.ViewModels
{
    public class InternetViewModel : ServerListViewModelBase
    {
        public InternetViewModel(MainViewModel vm) : base(vm)
        {
            Task.Factory.StartNew(() => base.Get("https://sa-multiplayer.com/servers/serverlist.txt"));
        }
    }
}