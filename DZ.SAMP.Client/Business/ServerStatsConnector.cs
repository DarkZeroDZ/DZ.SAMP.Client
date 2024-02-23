using DZ.SAMP.Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DZ.SAMP.Client.Business
{
    public class ServerStatsConnector
    {
        private IPEndPoint _endPoint;

        public async Task<Server> GetServerProperties(string ipaddess)
        {
            try
            {
                var ip = ipaddess.Split(':');
                var isValidIP = IPAddress.TryParse(ip[0], out var cleanIP);
                var ipAddresses = Dns.GetHostAddresses(ip[0]);

                if (isValidIP || ipAddresses.Any())
                {
                    if(isValidIP)
                        this._endPoint = new IPEndPoint(cleanIP, ip[1] == null ? 7777 : int.Parse(ip[1]));
                    else
                        this._endPoint = new IPEndPoint(ipAddresses[0], ip[1] == null ? 7777 : int.Parse(ip[1]));

                    var server = new Server();

                    // Server info
                    var startPing = DateTime.Now;
                    var sendServerInfoQueryTask = this.Send('i');

                    server.Ping = (DateTime.Now - startPing).Milliseconds;

                    if (sendServerInfoQueryTask != null)
                    {
                        using (var stream = new MemoryStream(sendServerInfoQueryTask))
                        {
                            using (var reader = new BinaryReader(stream))
                            {
                                reader.ReadBytes(11);
                                server.IPAddress = _endPoint.Address.ToString();
                                server.Port = _endPoint.Port.ToString();
                                server.Password = reader.ReadByte();
                                server.Players = reader.ReadInt16();
                                server.MaxPlayers = reader.ReadInt16();
                                server.Name = Encoding.Default.GetString(reader.ReadBytes(reader.ReadInt32()));
                                server.Mode = new string(Encoding.Default.GetChars(reader.ReadBytes(reader.ReadInt32()))); 
                                server.Language = new string(Encoding.Default.GetChars(reader.ReadBytes(reader.ReadInt32())));
                            }
                        }
                    }
                    else
                        return new Server();

                    try // Using a try block because we don't have to mind about it if we lose any UDP packages here
                    {
                        // Player list
                        var sendServerPlayersQueryTask = this.Send('c');
                        var playerList = new List<Player>();

                        if (sendServerPlayersQueryTask != null)
                        {
                            using (var stream = new MemoryStream(sendServerPlayersQueryTask))
                            {
                                using (var reader = new BinaryReader(stream))
                                {
                                    reader.ReadBytes(11);
                                    int maxPlayers = reader.ReadInt16();
                                    for (int i = 0; i < maxPlayers; i++)
                                    {
                                        var player = new Player();
                                        int usernameLength = reader.ReadByte();
                                        player.Name = new string(reader.ReadChars(usernameLength));
                                        player.Score = reader.ReadInt32();
                                        playerList.Add(player);
                                    }
                                }
                            }
                        }

                        server.PlayerList.Clear();
                        foreach (var player in playerList)
                        {
                            server.PlayerList.Add(player);
                        }
                    }
                    catch
                    {

                    }

                    try // Using a try block because we don't have to mind about it if we lose any UDP packages here
                    {
                        // Server rules
                        var sendServerRulesQueryTask = this.Send('r');
                        var resultList = new List<Rule>();

                        if (sendServerRulesQueryTask != null)
                        {
                            using (var stream = new MemoryStream(sendServerRulesQueryTask))
                            {
                                using (var reader = new BinaryReader(stream))
                                {
                                    reader.ReadBytes(11);
                                    int ruleCount = reader.ReadInt16();

                                    for (int i = 0; i < ruleCount; i++)
                                    {
                                        var rule = new Rule();
                                        int ruleLength = reader.ReadByte();
                                        rule.Name = new string(reader.ReadChars(ruleLength));

                                        ruleLength = reader.ReadByte();
                                        rule.Value = new string(reader.ReadChars(ruleLength));
                                        resultList.Add(rule);
                                    }
                                }
                            }
                        }

                        if (server.RuleList == null)
                            server.RuleList = new ObservableCollection<Rule>();

                        server.RuleList.Add(new Rule("Language", server.Language));

                        foreach (var rule in resultList)
                        {
                            switch (rule.Name)
                            {
                                case "weburl":
                                    server.URL = rule.Value;
                                    break;
                                case "mapname":
                                    server.Map = rule.Value;
                                    break;
                                default:
                                    server.RuleList.Add(rule);
                                    break;
                            }
                        }
                    }
                    catch 
                    {

                    }                    

                    return server;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return new Server();
        }

        public byte[] Send(char opCode)
        {
            byte[] datagram;

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    var ip = _endPoint.Address.GetAddressBytes();

                    writer.Write("SAMP".ToCharArray());

                    foreach (var pack in ip)
                        writer.Write(pack);

                    writer.Write((ushort)_endPoint.Port);
                    writer.Write(opCode);
                }
                datagram = stream.ToArray();
            }

            byte[] receivedData;

            using (var client = new UdpClient())
            {
                if(opCode != 'd')
                    client.Client.ReceiveTimeout = 500;
                else
                    client.Client.ReceiveTimeout = 1000;

                var result = client.Send(datagram, datagram.Length, _endPoint);

                if (result != 11)
                    return null;

                try
                {
                    receivedData = client.Receive(ref this._endPoint);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        Console.WriteLine("No data has been returned from host.");
                        return null;
                    }
                    else
                    {
                        Console.WriteLine($"Error while trying to receive data from host: {ex.Message}");
                        return null;
                    }
                }
            }
            
            return receivedData;
        }
    }
}