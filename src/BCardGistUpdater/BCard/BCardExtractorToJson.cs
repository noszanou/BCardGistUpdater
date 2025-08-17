using Newtonsoft.Json;
using System.Text;
using Za.NosGame.RessourceLoader.Traduction;
using Za.NosGame.Shared;
using Za.NosGame.Shared.DatEntitys.Enums;

namespace BCardGistUpdater.BCard
{
    public class BCardExtractorToJson : IBCardExtractorToJson
    {
        private readonly II18NManager _i18nManager;
        private readonly DatFileFolder _datFileFolder;

        public BCardExtractorToJson(II18NManager i18nManager, DatFileFolder datFileFolder)
        {
            _i18nManager = i18nManager;
            _datFileFolder = datFileFolder;
        }

        public async Task Load()
        {
            string inputFile = Path.Combine(_datFileFolder.DatFolder, "BCard.dat");
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"{inputFile} should be present");
            }

            string outputFolder = Path.Combine(_datFileFolder.RessourceFolder, "BcardJSON");
            Directory.CreateDirectory(outputFolder);

            var allLines = await File.ReadAllLinesAsync(inputFile, Encoding.GetEncoding(1252));

            // nop
            /*foreach (RegionLanguageType region in Enum.GetValues<RegionLanguageType>())
            {
                var output = ParseBCardFile(allLines, region);
                string outputPath = Path.Combine(outputFolder, $"BCard_{region}.json");
                await File.WriteAllTextAsync(outputPath, JsonConvert.SerializeObject(output, Formatting.Indented));
            }*/

            var output = ParseBCardFile(allLines, RegionLanguageType.EN);
            string outputPath = Path.Combine(outputFolder, $"BCard_EN.json");
            await File.WriteAllTextAsync(outputPath, JsonConvert.SerializeObject(output, Formatting.Indented));
        }

        private List<BCardJson> ParseBCardFile(string[] lines, RegionLanguageType region)
        {
            var descMap = new Dictionary<(long, string), string>();

            return lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split('\t'))
                .Where(parts => parts.Length > 1)
                .Aggregate(
                    new { Current = (BCardJson)null, Result = new List<BCardJson>() },
                    (acc, parts) =>
                    {
                        string key = parts[1];
                        string value = parts.Length > 2 ? parts[2] : null;
                        var current = acc.Current;
                        var result = acc.Result;

                        if (key == "VNUM")
                        {
                            current = new BCardJson
                            {
                                Type = long.Parse(value),
                                Name = "NONE",
                                Info = new List<BCardListJson>()
                            };
                        }
                        else if (key == "NAME") current.Name = _i18nManager.GetDataTranslations(GameDataType.BCard, region, value);
                        else if (key.StartsWith("SUBJ")) descMap[(current.Type, key)] = value;
                        else if (key.StartsWith("LIST"))
                        {
                            long subtype = long.Parse(key.Substring(4).Replace("-", ""));
                            string subjKey = "SUBJ" + (subtype / 10);
                            descMap.TryGetValue((current.Type, subjKey), out var descId);

                            current.Info.Add(new BCardListJson
                            {
                                Subtype = subtype,
                                Name = _i18nManager.GetDataTranslations(GameDataType.BCard, region, value),
                                Description = _i18nManager.GetDataTranslations(GameDataType.BCard, region, descId ?? "NONE")
                            });
                        }
                        else if (key == "END") result.Add(current);

                        return new { Current = current, Result = result };
                    })
                .Result;
        }
    }
}