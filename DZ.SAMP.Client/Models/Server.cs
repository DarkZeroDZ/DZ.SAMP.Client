using DZ.SAMP.Client.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace DZ.SAMP.Client.Models
{
    public class Server : AbstractXMLItem
    {
        [JsonProperty("IP")]
        public string IPAddress { get; set; }
        [JsonProperty("Port")]
        public string Port { get; set; }
        [JsonProperty("Hostname")]
        public string Name { get; set; }      
        [XMLSerializePriority(Priority.Skip)]
        [JsonProperty("Players")]
        public int Players { get; set; }
        [XMLSerializePriority(Priority.Skip)]
        [JsonProperty("MaxPlayers")]
        public int MaxPlayers { get; set; }
        [XMLSerializePriority(Priority.Skip)]
        [JsonProperty("Gamemode")]
        public string Mode { get; set; }
        [XMLSerializePriority(Priority.Skip)]
        [JsonProperty("Language")]
        public string Language { get; set; }       
        [XMLSerializePriority(Priority.Skip)]
        [JsonProperty("Password")]
        public byte Password { get; set; }
        [XMLSerializePriority(Priority.Skip)]
        [JsonProperty("WebURL")]
        public string URL { get; set; }
        [XMLSerializePriority(Priority.Skip)]
        [JsonProperty("Mapname")]
        public string Map { get; set; }
        [XMLSerializePriority(Priority.Skip)]
        public int Ping { get; set; }
        [XMLSerializePriority(Priority.Skip)]
        public ObservableCollection<Player> PlayerList { get; set; }
        public ObservableCollection<Rule> RuleList { get; set; }

        //Formatted stuff
        [XMLSerializePriority(Priority.Skip)]
        public string FormattedPlayers => this.Players + "/" + this.MaxPlayers;
        [XMLSerializePriority(Priority.Skip)]
        public string FormattedIP => this.IPAddress + ":" + this.Port;
        [XMLSerializePriority(Priority.Skip)]
        public bool IsPassword
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(this.Password);
                }
                catch
                {
                    return false;
                }
            }
        }

        public Server()
        {
            this.PlayerList = new ObservableCollection<Player>();
        }

        public Server(XElement e) : base(e)
        { }

        public Server Clone()
        {
            Server clonedServer = new Server
            {
                IPAddress = this.IPAddress,
                Port = this.Port,
                Name = this.Name,
                Players = this.Players,
                MaxPlayers = this.MaxPlayers,
                Mode = this.Mode,
                Language = this.Language,
                Password = this.Password,
                URL = this.URL,
                Map = this.Map,
                Ping = this.Ping,
                PlayerList = new ObservableCollection<Player>(),
                RuleList = new ObservableCollection<Rule>()
            };

            foreach (var player in this.PlayerList)
            {
                clonedServer.PlayerList.Add(player);
            }

            foreach (var rule in this.RuleList)
            {
                clonedServer.RuleList.Add(rule);
            }

            return clonedServer;
        }
    }
}
