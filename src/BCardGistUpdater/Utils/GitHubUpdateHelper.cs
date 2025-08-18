using Newtonsoft.Json;
using System.Text;

namespace BCardGistUpdater.Utils
{
    public static class GitHubUpdateHelper
    {
        public static async Task<string?> GetCurrentContentAsync(HttpClient client, string url, string? path = null)
        {
            var resp = await client.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            dynamic data = JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync());

            if (path != null)
            {
                string encoded = data.content;
                return Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            }
            else
            {
                return data.files.First.First.Value.content.ToString();
            }
        }

        public static async Task<bool> UpdateRepoFileIfChangedAsync(HttpClient client,
            string owner, string repo, string branch, string path, string newContent, string sha)
        {
            string url = $"https://api.github.com/repos/{owner}/{repo}/contents/{path}";

            var getContent = await GetCurrentContentAsync(client, $"{url}?ref={branch}", path);
            if (getContent == newContent)
            {
                return false;
            }

            var updatePayload = new
            {
                message = $"Update {path} automatically",
                content = Convert.ToBase64String(Encoding.UTF8.GetBytes(newContent)),
                sha,
                branch
            };

            var updateResp = await client.PutAsync(
                url,
                new StringContent(JsonConvert.SerializeObject(updatePayload), Encoding.UTF8, "application/json")
            );
            updateResp.EnsureSuccessStatusCode();

            return true;
        }

        public static async Task<bool> UpdateGistIfChangedAsync(HttpClient client, string gistId, string fileName, string newContent)
        {
            string gistUrl = $"https://api.github.com/gists/{gistId}";
            var resp = await client.GetAsync(gistUrl);
            resp.EnsureSuccessStatusCode();
            dynamic gistData = JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync());
            string currentContent = gistData.files[fileName].content.ToString();

            if (currentContent == newContent)
            {
                return false;
            }

            var payload = new
            {
                files = new Dictionary<string, object>
                {
                    { fileName, new { content = newContent } }
                }
            };

            var updateResp = await client.PatchAsync(
                gistUrl,
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            );
            updateResp.EnsureSuccessStatusCode();

            return true;
        }
    }
}