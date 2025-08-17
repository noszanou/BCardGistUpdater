using Newtonsoft.Json;

namespace BCardGistUpdater.GithubUpdater.JsonEntities
{
    public class SkillJson
    {
        [JsonProperty("Name")]
        public Name Name { get; set; }

        [JsonProperty("Description")]
        public List<Name> Description { get; set; } = new List<Name>();

        [JsonProperty("Vnum")]
        public long Vnum { get; set; }

        [JsonProperty("IconId")]
        public int IconId { get; set; }
    }
}
