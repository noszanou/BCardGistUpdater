namespace BCardGistUpdater.BCard
{
    public class BCardJson
    {
        public long Type { get; set; }

        public required string Name { get; set; }

        public List<BCardListJson> Info { get; set; } = new();
    }
}
