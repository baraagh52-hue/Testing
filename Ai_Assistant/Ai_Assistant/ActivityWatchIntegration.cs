using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class ActivityWatchIntegration
    {
        private readonly HttpClient _httpClient = new();

        public async Task<string> GetCurrentActivityAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("http://localhost:5600/api/0/buckets");
                using var doc = JsonDocument.Parse(response);
                foreach (var bucket in doc.RootElement.EnumerateObject())
                {
                    var bucketName = bucket.Name;
                    if (bucketName.Contains("aw-watcher-window"))
                    {
                        var eventsUrl = $"http://localhost:5600/api/0/buckets/{bucketName}/events";
                        var eventsResponse = await _httpClient.GetStringAsync(eventsUrl);
                        using var eventsDoc = JsonDocument.Parse(eventsResponse);
                        var events = eventsDoc.RootElement;
                        if (events.GetArrayLength() > 0)
                        {
                            var lastEvent = events[events.GetArrayLength() - 1];
                            var app = lastEvent.GetProperty("data").GetProperty("app").GetString();
                            var title = lastEvent.GetProperty("data").GetProperty("title").GetString();
                            return $"{app}: {title}";
                        }
                    }
                }
            }
            catch
            {
                // Handle errors
            }
            return "No activity detected";
        }
    }
}