using System.Net;

namespace DZ.SAMP.Client.Business
{
    public class UpdateConnector
    {
        public string GetLatestVersion()
        {
            var version = string.Empty;

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            try
            {
                using (var client = new WebClient())
                {
                    version = client.DownloadString("https://sa-multiplayer.com/servers/latestversion.txt");
                }
            }
            catch (System.Exception)
            {
                version = string.Empty;
            }

            ServicePointManager.ServerCertificateValidationCallback -= (sender, certificate, chain, sslPolicyErrors) => true;


            return version;
        }
    }
}
