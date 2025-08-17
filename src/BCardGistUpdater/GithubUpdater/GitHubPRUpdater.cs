using BCardGistUpdater.GithubUpdater.JsonEntities;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Za.NosGame.RessourceLoader.Manager;
using Za.NosGame.Shared;
using Za.NosGame.Shared.DatEntitys.Entitys;
using Za.NosGame.Shared.DatEntitys.Enums;
using Za.NosGame.Shared.DatEntitys.Interfaces;

namespace BCardGistUpdater.GithubUpdater
{
    public class GitHubPRUpdater : IGitHubPRUpdater
    {
        private readonly string _owner = "noszanou";
        private readonly string _repo = "NosTale-Data-Picker";
        private readonly string _baseBranch = "main";

        private readonly IGameEntityManager<Skill> _skillManager;
        private readonly IGameEntityManager<Card> _cardManager;
        private readonly IGameEntityManager<Item> _itemManager;
        private readonly IGameEntityManager<NpcMonster> _npcMonsterManager;
        private readonly IGameEntityManager<BCardLang> _bcardManager;
        private readonly DatFileFolder _datFileFolder;
        private readonly HashSet<string> _existingImages;

        public GitHubPRUpdater(IGameEntityManager<Skill> skillManager, IGameEntityManager<Card> cardManager, IGameEntityManager<BCardLang> bcardManager, IGameEntityManager<Item> itemManager,
            IGameEntityManager<NpcMonster> npcManager, DatFileFolder datFileFolder)
        {
            _skillManager = skillManager;
            _cardManager = cardManager;
            _bcardManager = bcardManager;
            _itemManager = itemManager;
            _npcMonsterManager = npcManager;
            _datFileFolder = datFileFolder;
            var imageFolder = Path.Combine(_datFileFolder.ParserFolder, "Image");
            _existingImages = Directory.Exists(imageFolder)
                ? new HashSet<string>(Directory.GetFiles(imageFolder).Select(f => Path.GetFileName(f)))
                : new HashSet<string>();
        }
        public async Task UpdateJsonFile()
        {
            // TODO: use consstring file parsing
            var files = new Dictionary<string, string>
            {
                { "js/skills.json", UpdateJsonSkill(_skillManager.Objects.Select(s => s.Value).ToList()) },
                { "js/cards.json", UpdateJsonCard(_cardManager.Objects.Select(s => s.Value).ToList()) },
                { "js/monsters.json", UpdateMonsterJson(_npcMonsterManager.Objects.Select(s => s.Value).ToList()) },
                { "js/items.json", UpdateJsonItem(_itemManager.Objects.Select(s => s.Value).ToList()) },
            };

            await CreatePullRequestMultipleFilesAsync(files);
        }

        private string UpdateJsonSkill(IReadOnlyList<Skill> skills)
        {
            var list = new List<SkillJson>();

            foreach (var skill in skills.OrderBy(s => s.Id))
            {
                var entity = new SkillJson
                {
                    Vnum = skill.Id,
                    IconId = _existingImages.Contains($"{skill.IconId}.png") ? skill.IconId : 0,
                    Name = Helper.ToName(skill.LangName)
                };

                foreach (var e in skill.LangDescription)
                {
                    entity.Description.Add(Helper.ToName(e));
                }

                list.Add(entity);
            }

            return JsonConvert.SerializeObject(list, Formatting.Indented);
        }

        private string UpdateJsonCard(IReadOnlyList<Card> cards)
        {
            var list = new List<CardJson>();
            foreach (var card in cards.OrderBy(s => s.Id))
            {
                var entity = new CardJson
                {
                    Vnum = card.Id,
                    IconId = _existingImages.Contains($"{card.IconId}.png") ? card.IconId : 0,
                    Name = Helper.ToName(card.LangName),
                    Description = Helper.ToName(card.LangDescription),
                    BuffGroup = (byte)card.BuffType,
                    BuffDuration = Helper.BuildDurationName(card.Duration, "Duraciуn", "Durée", "Duration", "Délka trvání", "Dauer", "Durata", "Czas trwania", "Длительность", "Süre"),
                    BuffId = Helper.BuildBuffList(_bcardManager, card.BCards.Where(s => !s.IsSecondBCardExecution.Value)),
                    BuffIdSecond = Helper.BuildBuffList(_bcardManager, card.BCards.Where(s => s.IsSecondBCardExecution.Value)),
                    BuffDurationSecond = Helper.BuildDurationName(card.SecondBCardsDelay, "Efecto secundario", "Effet secondaire", "Side effect", "Vedlejší účinky", "Nebenwirkung", "Secondo effetto", "Działanie uboczne", "Длительность", "Yan etki"),

                    // flemme to map
                    BuffLevel = new Name
                    {
                        Es = card.BuffType switch
                        {
                            BuffGroup.Good when card.BuffCategory == BuffCategory.GeneralEffect => "Efecto positivo",
                            BuffGroup.Good when card.BuffCategory == BuffCategory.DiseaseSeries => "Efecto de enfermedad positivo",
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.GeneralEffect => "Efecto negativo",
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.PoisonType => "Poison Debuff",
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.DiseaseSeries => "Efecto de veneno negativo",
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.MagicEffect => "Efecto mágico negativo",
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.GeneralEffect => "Efecto neutral",
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.MagicEffect => "Efecto mágico neutral",
                            _ => "Unknow report it please!",
                        } + $"<br>(nivel {card.Level})",
                        Fr = card.BuffType switch
                        {
                            BuffGroup.Good when card.BuffCategory == BuffCategory.GeneralEffect => "Effet positif",
                            BuffGroup.Good when card.BuffCategory == BuffCategory.DiseaseSeries => "Effet maladif positif",
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.GeneralEffect => "Effet négatif",
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.PoisonType => "Poison Debuff",
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.DiseaseSeries => "Effet maladif négatif",
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.MagicEffect => "Effet magique négatif",
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.GeneralEffect => "Effet Neutre",
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.MagicEffect => "Effet Magique Neutre",
                            _ => "Unknow report it please!",
                        } + $"<br>(niveau {card.Level})",
                        Uk = card.BuffType switch
                        {
                            BuffGroup.Good when card.BuffCategory == BuffCategory.GeneralEffect => "Buff", // 4371
                            BuffGroup.Good when card.BuffCategory == BuffCategory.DiseaseSeries => "Disease Buff", // 4374
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.GeneralEffect => "Debuff", // 4379
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.PoisonType => "Poison Debuff", // 4381
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.DiseaseSeries => "Disease Debuff", // 4382
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.MagicEffect => "Magic Debuff", // 4380
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.GeneralEffect => "Neutral Effect", // 4375
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.MagicEffect => "Neutral Magical Effect", // 4376
                            _ => "Unknow report it please!",
                        } + $"<br>(level {card.Level})",
                        Cz = card.BuffType switch
                        {
                            BuffGroup.Good when card.BuffCategory == BuffCategory.GeneralEffect => "Buff", // 4371
                            BuffGroup.Good when card.BuffCategory == BuffCategory.DiseaseSeries => "Chorobný Buff", // 4374
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.GeneralEffect => "Debuff", // 4379
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.PoisonType => "Jedový Debuff", // 4381
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.DiseaseSeries => "Chorobný Debuff", // 4382
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.MagicEffect => "Magický Debuff", // 4380
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.GeneralEffect => "Neutrální Efekt", // 4375
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.MagicEffect => "Neutrální Magický Efekt", // 4376
                            _ => "Unknow report it please!",
                        } + $"<br>(Úroveň {card.Level})",
                        De = card.BuffType switch
                        {
                            BuffGroup.Good when card.BuffCategory == BuffCategory.GeneralEffect => "Positiver Effekt", // 4371
                            BuffGroup.Good when card.BuffCategory == BuffCategory.DiseaseSeries => "Positiver Krankheitseffekt", // 4374
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.GeneralEffect => "Negativer Effekt", // 4379
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.PoisonType => "Negativer Gifteffekt", // 4381
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.DiseaseSeries => "Negativer Krankheitseffekt", // 4382
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.MagicEffect => "Negativer magischer Effekt", // 4380
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.GeneralEffect => "Neutraler Effekt", // 4375
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.MagicEffect => "Neutraler magischer Effekt", // 4376
                            _ => "Unknow report it please!",
                        } + $"<br>(Level {card.Level})",
                        It = card.BuffType switch
                        {
                            BuffGroup.Good when card.BuffCategory == BuffCategory.GeneralEffect => "Effetto positivo", // 4371
                            BuffGroup.Good when card.BuffCategory == BuffCategory.DiseaseSeries => "Effetto malattia positivo", // 4374
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.GeneralEffect => "Effetto negativo", // 4379
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.PoisonType => "Effetto velenoso negativo", // 4381
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.DiseaseSeries => "Effetto malattia negativo", // 4382
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.MagicEffect => "Effetto magico negativo", // 4380
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.GeneralEffect => "Effetto neutrale", // 4375
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.MagicEffect => "Effetto magico neutrale", // 4376
                            _ => "Unknow report it please!",
                        } + $"<br>(livello {card.Level})",
                        Pl = card.BuffType switch
                        {
                            BuffGroup.Good when card.BuffCategory == BuffCategory.GeneralEffect => "Pozytywny efekt", // 4371
                            BuffGroup.Good when card.BuffCategory == BuffCategory.DiseaseSeries => "Pozytywny efekt choroby", // 4374
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.GeneralEffect => "Negatywny efekt", // 4379
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.PoisonType => "Negatywny efekt trucizny", // 4381
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.DiseaseSeries => "Negatywny efekt choroby", // 4382
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.MagicEffect => "Negatywny efekt magiczny", // 4380
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.GeneralEffect => "Neutralny efekt", // 4375
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.MagicEffect => "Neutralny efekt magiczny", // 4376
                            _ => "Unknow report it please!",
                        } + $"<br>(Poziom {card.Level})",
                        Ru = card.BuffType switch
                        {
                            BuffGroup.Good when card.BuffCategory == BuffCategory.GeneralEffect => "Положительный эффект", // 4371
                            BuffGroup.Good when card.BuffCategory == BuffCategory.DiseaseSeries => "Положительный болезненный эффект", // 4374
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.GeneralEffect => "Негативный эффект", // 4379
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.PoisonType => "Негативный ядовитый эффект", // 4381
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.DiseaseSeries => "Негативный болезненный эффект", // 4382
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.MagicEffect => "Негативный магический эффект", // 4380
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.GeneralEffect => "Нейтральный эффект", // 4375
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.MagicEffect => "Нейтральный магический эффект", // 4376
                            _ => "Unknow report it please!",
                        } + $"<br>(Уровень {card.Level})",
                        Tr = card.BuffType switch
                        {
                            BuffGroup.Good when card.BuffCategory == BuffCategory.GeneralEffect => "Olumlu efekt", // 4371
                            BuffGroup.Good when card.BuffCategory == BuffCategory.DiseaseSeries => "Olumlu hastalýk efekti", // 4374
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.GeneralEffect => "Olumsuz efekt", // 4379
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.PoisonType => "Olumsuz zehir efekti", // 4381
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.DiseaseSeries => "Olumsuz hastalýk efekti", // 4382
                            BuffGroup.Bad when card.BuffCategory == BuffCategory.MagicEffect => "Olumsuz büyü efekti", // 4380
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.GeneralEffect => "Nötr efekt", // 4375
                            BuffGroup.Neutral when card.BuffCategory == BuffCategory.MagicEffect => "Nötr büyü efekti", // 4376
                            _ => "Unknow report it please!",
                        } + $"<br>(Seviye {card.Level})",
                    },
                };

                list.Add(entity);
            }
            return JsonConvert.SerializeObject(list, Formatting.Indented);
        }

        private string UpdateMonsterJson(IReadOnlyList<NpcMonster> monsters)
        {
            var list = new List<MonsterJson>();
            foreach (var monster in monsters.OrderBy(s => s.Id))
            {
                var entity = new MonsterJson
                {
                    IconId = _existingImages.Contains($"{monster.IconId}.png") ? monster.IconId : 0,// + 8000,
                    Element = monster.Element,
                    ElementRate = monster.ElementRate,
                    AttackClass = (byte)monster.AttackType,
                    AttackUpgrade = monster.AttackUpgrade,
                    BasicArea = monster.BasicRange,
                    BasicCooldown = monster.BasicCooldown,
                    BasicRange = monster.BasicRange,
                    BasicSkill = 200,
                    CloseDefence = monster.CloseDefence,
                    Concentrate = monster.Concentrate,
                    CriticalChance = monster.CriticalChance,
                    CriticalRate = monster.CriticalRate,
                    DamageMaximum = monster.DamageMaximum,
                    DamageMinimum = monster.DamageMinimum,
                    DarkResistance = monster.DarkResistance,
                    DefenceDodge = monster.DefenceDodge,
                    DefenceUpgrade = monster.DefenceUpgrade,
                    DistanceDefence = monster.DistanceDefence,
                    DistanceDefenceDodge = monster.DistanceDefenceDodge,
                    FireResistance = monster.FireResistance,
                    IsHostile = monster.HostilityType != 0,
                    JobXp = monster.JobXp,
                    Level = monster.Level,
                    LightResistance = monster.LightResistance,
                    MagicDefence = monster.MagicDefence,
                    MaxHp = monster.MaxHp,
                    MaxMp = monster.MaxMp,
                    NoticeRange = monster.NoticeRange,
                    Race = monster.Race,
                    RaceType = monster.RaceType,
                    RespawnTime = monster.RespawnTime,
                    Speed = monster.Speed,
                    Vnum = monster.Id,
                    WaterResistance = monster.WaterResistance,
                    Xp = monster.Xp,
                    HeroLevel = monster.HeroLevel,
                    Name = Helper.ToName(monster.LangName),
                    Drops = new List<JsonEntities.Drop>()
                };

                foreach (var c in monster.Drops)
                {
                    var nameItem = _itemManager.GetObjectById(c.ItemVNum);
                    var percent = (double)c.DropChance / 1000d;
                    entity.Drops.Add(new JsonEntities.Drop
                    {
                        Chance = percent,
                        Count = c.Amount,
                        IconId = nameItem.IconId,
                        Vnum = c.ItemVNum,
                        Name = Helper.ToName(nameItem.LangName)
                    });
                }
                list.Add(entity);
            }
            return JsonConvert.SerializeObject(list, Formatting.Indented);
        }
        private string UpdateJsonItem(IReadOnlyList<Item> items)
        {
            var list = new List<ItemJson>();
            foreach (var item in items.OrderBy(s => s.Id))
            {
                var entity = new ItemJson
                {
                    IconId = _existingImages.Contains($"{item.IconId}.png") ? item.IconId : 0,
                    EquipmentSlot = (byte)item.EquipmentSlot,
                    BasicUpgrade = item.BasicUpgrade,
                    Class = item.Class,
                    CriticalLuckRate = item.CriticalLuckRate,
                    CriticalRate = item.CriticalRate,
                    HitRate = item.HitRate,
                    ItemSubType = item.ItemSubType,
                    ItemType = (byte)item.ItemType,
                    Level = item.LevelMinimum,
                    MaximumDamage = item.DamageMaximum,
                    MinimumDamage = item.DamageMinimum,
                    Price = item.Price,
                    Type = (byte)item.Type,
                    Vnum = item.Id,
                    Name = Helper.ToName(item.LangName),
                    Description = Helper.ToName(item.LangDescription)
                };

                list.Add(entity);
            }
            return JsonConvert.SerializeObject(list, Formatting.Indented);
        }

        private async Task CreatePullRequestMultipleFilesAsync(Dictionary<string, string> filesToUpdate)
        {
            string token = Environment.GetEnvironmentVariable("TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("GitHub token not found.");
            }
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DotNet-GitHub-PR-Updater");

            string newBranchName = $"update-json-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var mainRefResp = await client.GetAsync($"https://api.github.com/repos/{_owner}/{_repo}/git/ref/heads/{_baseBranch}");
            mainRefResp.EnsureSuccessStatusCode();
            dynamic mainRef = JsonConvert.DeserializeObject(await mainRefResp.Content.ReadAsStringAsync());
            string mainSha = mainRef.@object.sha;

            var createRefPayload = new { @ref = $"refs/heads/{newBranchName}", sha = mainSha };
            var createRefResp = await client.PostAsync(
                $"https://api.github.com/repos/{_owner}/{_repo}/git/refs",
                new StringContent(JsonConvert.SerializeObject(createRefPayload), Encoding.UTF8, "application/json")
            );
            createRefResp.EnsureSuccessStatusCode();

            foreach (var kvp in filesToUpdate)
            {
                string path = kvp.Key;
                string content = kvp.Value;

                var getFileResp = await client.GetAsync($"https://api.github.com/repos/{_owner}/{_repo}/contents/{path}?ref={_baseBranch}");
                getFileResp.EnsureSuccessStatusCode();
                dynamic fileInfo = JsonConvert.DeserializeObject(await getFileResp.Content.ReadAsStringAsync());
                string sha = fileInfo.sha;

                var updatePayload = new
                {
                    message = $"Update {path} automatically",
                    content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
                    sha,
                    branch = newBranchName
                };
                var updateResp = await client.PutAsync(
                    $"https://api.github.com/repos/{_owner}/{_repo}/contents/{path}",
                    new StringContent(JsonConvert.SerializeObject(updatePayload), Encoding.UTF8, "application/json")
                );
                updateResp.EnsureSuccessStatusCode();
            }

            var prPayload = new
            {
                title = "Update json files automatically",
                head = newBranchName,
                @base = _baseBranch,
                body = "Automatic update of JSON files"
            };
            var prResp = await client.PostAsync(
                $"https://api.github.com/repos/{_owner}/{_repo}/pulls",
                new StringContent(JsonConvert.SerializeObject(prPayload), Encoding.UTF8, "application/json")
            );
            prResp.EnsureSuccessStatusCode();
        }
    }
}