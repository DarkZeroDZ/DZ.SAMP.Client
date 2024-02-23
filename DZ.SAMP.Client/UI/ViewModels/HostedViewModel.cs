using DZ.SAMP.Client.Business;
using System.Net;
using System.Threading.Tasks;

namespace DZ.SAMP.Client.UI.ViewModels
{
    public class HostedViewModel : ServerListViewModelBase
    {
        public HostedViewModel(MainViewModel vm) : base(vm)
        {
            var result = string.Empty;

            try
            {
                using (var client = new WebClient())
                {
                    result = client.DownloadString("http://server.sa-mp.com/0.3.7/hosted");
                }
            }
            catch (System.Exception)
            {
                result = string.Empty;
            }

            if (!string.IsNullOrEmpty(result))
            {
                Task.Factory.StartNew(() => base.Get("http://server.sa-mp.com/0.3.7/hosted"));
                base.VM.IsHostedTabActive = true;
            }
            else
                base.VM.IsHostedTabActive = false;
        }
    }
}
