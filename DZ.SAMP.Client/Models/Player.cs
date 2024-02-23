using Newtonsoft.Json;

namespace DZ.SAMP.Client.Models
{
    public class Player
    {
        [JsonProperty("Nickname")]
        public string Name { get; set; }
        [JsonProperty("Score")]
        public int Score { get; set; }
    }
}
