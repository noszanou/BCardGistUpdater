using Newtonsoft.Json;

namespace BCardGistUpdater.GithubUpdater.JsonEntities
{
    public class Drop
    {
        public Name Name { get; set; }

        public int IconId { get; set; }

        [JsonProperty("Vnum")]
        public long Vnum { get; set; }

        [JsonProperty("Chance")]
        public double Chance { get; set; }

        [JsonProperty("Count")]
        public long Count { get; set; }
    }
}