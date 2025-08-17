using Newtonsoft.Json;

namespace BCardGistUpdater.GithubUpdater.JsonEntities
{
    public class CardJson
    {
        [JsonProperty("Name")]
        public Name Name { get; set; }

        [JsonProperty("Description")]
        public Name Description { get; set; }

        [JsonProperty("Vnum")]
        public long Vnum { get; set; }

        [JsonProperty("IconId")]
        public int IconId { get; set; }

        public byte BuffGroup { get; set; }

        public Name BuffDuration { get; set; }
        public Name BuffLevel { get; set; }

        public List<Name> BuffId { get; set; }

        public List<Name> BuffIdSecond { get; set; }
        public Name BuffDurationSecond { get; set; }
    }
}
