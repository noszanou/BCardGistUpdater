using Newtonsoft.Json;

namespace BCardGistUpdater.GithubUpdater.JsonEntities
{
    public class Name
    {
        [JsonProperty("Cz")]
        public string Cz { get; set; }

        [JsonProperty("De")]
        public string De { get; set; }

        [JsonProperty("Es")]
        public string Es { get; set; }

        [JsonProperty("Fr")]
        public string Fr { get; set; }

        [JsonProperty("It")]
        public string It { get; set; }

        [JsonProperty("Pl")]
        public string Pl { get; set; }

        [JsonProperty("Ru")]
        public string Ru { get; set; }

        [JsonProperty("Tr")]
        public string Tr { get; set; }

        [JsonProperty("Uk")]
        public string Uk { get; set; }
    }
}