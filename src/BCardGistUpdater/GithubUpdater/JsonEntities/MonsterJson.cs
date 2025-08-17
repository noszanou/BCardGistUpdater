using Newtonsoft.Json;

namespace BCardGistUpdater.GithubUpdater.JsonEntities
{
    public class MonsterJson
    {
        [JsonProperty("Vnum")]
        public long Vnum { get; set; }

        [JsonProperty("Name")]
        public Name Name { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("race")]
        public long Race { get; set; }

        [JsonProperty("raceType")]
        public long RaceType { get; set; }

        [JsonProperty("element")]
        public long Element { get; set; }

        [JsonProperty("elementRate")]
        public long ElementRate { get; set; }

        [JsonProperty("fireResistance")]
        public long FireResistance { get; set; }

        [JsonProperty("waterResistance")]
        public long WaterResistance { get; set; }

        [JsonProperty("lightResistance")]
        public long LightResistance { get; set; }

        [JsonProperty("darkResistance")]
        public long DarkResistance { get; set; }

        [JsonProperty("maxHp")]
        public long MaxHp { get; set; }

        [JsonProperty("maxMp")]
        public long MaxMp { get; set; }

        [JsonProperty("xp")]
        public long Xp { get; set; }

        [JsonProperty("jobXp")]
        public long JobXp { get; set; }

        [JsonProperty("isHostile")]
        public bool IsHostile { get; set; }

        [JsonProperty("noticeRange")]
        public long NoticeRange { get; set; }

        [JsonProperty("speed")]
        public long Speed { get; set; }

        [JsonProperty("respawnTime")]
        public long RespawnTime { get; set; }

        [JsonProperty("IconId")]
        public long IconId { get; set; }

        [JsonProperty("basicSkill")]
        public long BasicSkill { get; set; }

        [JsonProperty("attackClass")]
        public long AttackClass { get; set; }

        [JsonProperty("basicRange")]
        public long BasicRange { get; set; }

        [JsonProperty("basicArea")]
        public long BasicArea { get; set; }

        [JsonProperty("basicCooldown")]
        public long BasicCooldown { get; set; }

        [JsonProperty("attackUpgrade")]
        public long AttackUpgrade { get; set; }

        [JsonProperty("damageMinimum")]
        public long DamageMinimum { get; set; }

        [JsonProperty("damageMaximum")]
        public long DamageMaximum { get; set; }

        [JsonProperty("concentrate")]
        public long Concentrate { get; set; }

        [JsonProperty("criticalChance")]
        public long CriticalChance { get; set; }

        [JsonProperty("criticalRate")]
        public long CriticalRate { get; set; }

        [JsonProperty("defenceUpgrade")]
        public long DefenceUpgrade { get; set; }

        [JsonProperty("closeDefence")]
        public long CloseDefence { get; set; }

        [JsonProperty("distanceDefence")]
        public long DistanceDefence { get; set; }

        [JsonProperty("magicDefence")]
        public long MagicDefence { get; set; }

        [JsonProperty("defenceDodge")]
        public long DefenceDodge { get; set; }

        [JsonProperty("distanceDefenceDodge")]
        public long DistanceDefenceDodge { get; set; }

        [JsonProperty("heroLevel")]
        public long HeroLevel { get; set; }

        public List<Drop> Drops { get; set; }
    }
}