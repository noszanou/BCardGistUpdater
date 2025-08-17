using Newtonsoft.Json;

namespace BCardGistUpdater.GithubUpdater.JsonEntities
{
    public class ItemJson
    {
        [JsonProperty("Name")]
        public Name Name { get; set; }

        [JsonProperty("Description")]
        public Name Description { get; set; }

        [JsonProperty("BasicUpgrade")]
        public long BasicUpgrade { get; set; }

        [JsonProperty("Class")]
        public long Class { get; set; }

        [JsonProperty("CriticalLuckRate")]
        public long CriticalLuckRate { get; set; }

        [JsonProperty("CriticalRate")]
        public long CriticalRate { get; set; }

        [JsonProperty("EquipmentSlot")]
        public long EquipmentSlot { get; set; }

        [JsonProperty("HitRate")]
        public long HitRate { get; set; }

        [JsonProperty("IconId")]
        public long IconId { get; set; }

        [JsonProperty("ItemSubType")]
        public long ItemSubType { get; set; }

        [JsonProperty("ItemType")]
        public long ItemType { get; set; }

        [JsonProperty("Level")]
        public long Level { get; set; }

        [JsonProperty("MaximumDamage")]
        public long MaximumDamage { get; set; }

        [JsonProperty("MinimumDamage")]
        public long MinimumDamage { get; set; }

        [JsonProperty("Price")]
        public long Price { get; set; }

        [JsonProperty("Type")]
        public long Type { get; set; }

        [JsonProperty("Vnum")]
        public long Vnum { get; set; }
    }
}