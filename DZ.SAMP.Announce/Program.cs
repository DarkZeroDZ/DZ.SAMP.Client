using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DZ.SAMP.Announce
{
    class Program
    {
        static async Task Main(string[] args)
        {
#if RELEASE
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a port as a command-line argument.");
                return;
            }

            if (!int.TryParse(args[0], out int port) || port < 1 || port > 99999)
            {
                Console.WriteLine("Invalid port.");
                return;
            }

            var accessKey = "Y8MIh9lgaD8FivYqJf22phxexr3RW9vv";
            var publicIP = await GetPublicIpAddress();
            var url = $"http://sa-multiplayer.com/servers/serverlist.php?accessKey={accessKey}&server={publicIP}:{port}";
#endif
#if DEBUG
            var port = 7777;
            if (port < 1 || port > 99999)
            {
                Console.WriteLine("Invalid port.");
                return;
            }

            var accessKey = "Y8MIh9lgaD8FivYqJf22phxexr3RW9vv";
            var publicIP = await GetPublicIpAddress();
            var url = $"http://sa-multiplayer.com/servers/serverlist.php?accessKey={accessKey}&server={publicIP}:{port}";
#endif

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Server successfully added to master list.");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"HTTP error: {e}");
                }
            }
        }

        static async Task<string> GetPublicIpAddress()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string response = await client.GetStringAsync("http://ipv4.icanhazip.com/");
                    return response.Trim();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Error while calling IP address: {e}");
                    return null;
                }
            }
        }
    }
}
