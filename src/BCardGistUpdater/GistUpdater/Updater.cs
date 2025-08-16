using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Za.NosGame.RessourceLoader.I18N;
using Za.NosGame.Shared;
using Za.NosGame.Shared.Loggers;

namespace BCardGistUpdater.GistUpdater
{
    public class Updater : IUpdater
    {
        private readonly DatFileFolder _datFileFolder;

        public Updater(DatFileFolder datFileFolder)
        {
            _datFileFolder = datFileFolder;
        }

        public async Task UpdateGistAsync()
        {
            string fileName = "BCard_EN.json";
            string bcardFolder = Path.Combine(_datFileFolder.RessourceFolder, "BcardJSON", fileName);
            string gistId = "04f87cbde1db24870b72c88048f70bf3";

            if (!File.Exists(bcardFolder))
            {
                return;
            }
            string content = await File.ReadAllTextAsync(bcardFolder);

            string token = Environment.GetEnvironmentVariable("TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            var files = new Dictionary<string, object>
            {
                { fileName, new { content } }
            };
            var payload = new Dictionary<string, object>
            {
                { "files", files }
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DotNet-Gist-Updater");

            var json = JsonConvert.SerializeObject(payload);
            var response = await client.PatchAsync(
                $"https://api.github.com/gists/{gistId}",
                new StringContent(json, Encoding.UTF8, "application/json")
            );
        }
    }
}
