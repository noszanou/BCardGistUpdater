using BCardGistUpdater.GithubUpdater.JsonEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Za.NosGame.RessourceLoader.Manager;
using Za.NosGame.Shared.DatEntitys.Entitys;
using Za.NosGame.Shared.DatEntitys.Enums;

namespace BCardGistUpdater.Utils
{
    public class Helper
    {
        public static Name BuildDurationName(int duration, string es, string fr, string uk, string cz, string de, string it, string pl, string ru, string tr)
        {
            double seconds = duration / 10.0;
            return new Name
            {
                Es = $"{es}: {seconds} segundos",
                Fr = $"{fr}: {seconds} secondes",
                Uk = $"{uk}: {seconds} seconds",
                Cz = $"{cz}: {seconds} sekund",
                De = $"{de}: {seconds} Sekunden",
                It = $"{it}: {seconds} sec",
                Pl = $"{pl}: {seconds} sekund",
                Ru = $"{ru}: {seconds} секунд",
                Tr = $"{tr}: {seconds} saniye"
            };
        }

        public static Name ToName(I18N lang)
        {
            return new Name
            {
                Es = lang.GetName(RegionLanguageType.ES),
                Fr = lang.GetName(RegionLanguageType.FR),
                Uk = lang.GetName(RegionLanguageType.EN),
                Cz = lang.GetName(RegionLanguageType.CZ),
                De = lang.GetName(RegionLanguageType.DE),
                It = lang.GetName(RegionLanguageType.IT),
                Pl = lang.GetName(RegionLanguageType.PL),
                Ru = lang.GetName(RegionLanguageType.RU),
                Tr = lang.GetName(RegionLanguageType.TR)
            };
        }

        public static List<Name> BuildBuffList(IGameEntityManager<BCardLang> bcardManager, IEnumerable<BCardObject> cards)
        {
            return cards.Select(e => new Name
            {
                Es = BcardDescriptionToString(e, GetLang(bcardManager, e, RegionLanguageType.ES)),
                Fr = BcardDescriptionToString(e, GetLang(bcardManager, e, RegionLanguageType.FR)),
                Uk = BcardDescriptionToString(e, GetLang(bcardManager, e, RegionLanguageType.EN)),
                Cz = BcardDescriptionToString(e, GetLang(bcardManager, e, RegionLanguageType.CZ)),
                De = BcardDescriptionToString(e, GetLang(bcardManager, e, RegionLanguageType.DE)),
                It = BcardDescriptionToString(e, GetLang(bcardManager, e, RegionLanguageType.IT)),
                Pl = BcardDescriptionToString(e, GetLang(bcardManager, e, RegionLanguageType.PL)),
                Ru = BcardDescriptionToString(e, GetLang(bcardManager, e, RegionLanguageType.RU)),
                Tr = BcardDescriptionToString(e, GetLang(bcardManager, e, RegionLanguageType.TR)),
            }).ToList();
        }

        private static string GetLang(IGameEntityManager<BCardLang> bcardManager, BCardObject bcard, RegionLanguageType lang) => bcardManager.GetObjectById(bcard.Type).LangDescription[bcard.SubType].GetName(lang);
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string BcardDescriptionToString(BCardObject bcard, string stringText)
        {
            var values = new[]
            {
                bcard.FirstDataScalingType switch
                {
                    BCardScalingType.LEVEL_MULTIPLIED => $"(player level * {bcard.FirstData})",
                    BCardScalingType.LEVEL_DIVIDED => $"(player level / {bcard.FirstData})",
                    _ => bcard.FirstData.ToString()
                },
                bcard.SecondDataScalingType switch
                {
                    BCardScalingType.LEVEL_MULTIPLIED => $"(player level * {bcard.SecondData})",
                    BCardScalingType.LEVEL_DIVIDED => $"(player level / {bcard.SecondData})",
                    _ => bcard.SecondData.ToString()
                }
            };


            foreach (var val in values)
            {
                stringText = ReplaceFirst(stringText, "%s", val);
            }
            return stringText.Replace("%%", "%");
        }
    }
}
