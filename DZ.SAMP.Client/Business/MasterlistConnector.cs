using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DZ.SAMP.Client.Business
{
    public class MasterlistConnector
    {
        public List<string> GetServers(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            var servers = new List<string>();

            using (var client = new WebClient())
            {
                var ips = client.DownloadString(url);

                using (var reader = new StringReader(ips))
                {
                    for (var ip = reader.ReadLine(); ip != null; ip = reader.ReadLine())
                        servers.Add(ip);
                }
            }

            ServicePointManager.ServerCertificateValidationCallback -= (sender, certificate, chain, sslPolicyErrors) => true;

            return servers;
        }     
    }
}
