
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class ActivityWatchIntegration
    {
        private readonly HttpClient _httpClient = new();
        private readonly ISettingsService _settingsService;

        public ActivityWatchIntegration(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<string> GetCurrentActivityAsync()
        {
            try
            {
                var settings = await _settingsService.LoadSettingsAsync();
                var activityWatchUrl = settings?.ActivityWatchUrl ?? "http://localhost:5600";
                if (string.IsNullOrEmpty(activityWatchUrl))
                {
                    Console.WriteLine("ActivityWatchIntegration: URL is not configured.");
                    return "No activity detected";
                }

                var response = await _httpClient.GetStringAsync($"{activityWatchUrl}/api/0/buckets");
                using var doc = JsonDocument.Parse(response);
                foreach (var bucket in doc.RootElement.EnumerateObject())
                {
                    var bucketName = bucket.Name;
                    if (bucketName.Contains("aw-watcher-window"))
                    {
                        var eventsUrl = $"{activityWatchUrl}/api/0/buckets/{bucketName}/events";
                        var eventsResponse = await _httpClient.GetStringAsync(eventsUrl);
                        using var eventsDoc = JsonDocument.Parse(eventsResponse);
                        var events = eventsDoc.RootElement;
                        if (events.GetArrayLength() > 0)
                        {
                            var lastEvent = events[events.GetArrayLength() - 1];
                            if (lastEvent.TryGetProperty("data", out var data) &&
                                data.TryGetProperty("app", out var app) &&
                                data.TryGetProperty("title", out var title))
                            {
                                return $"{app.GetString() ?? "Unknown"}: {title.GetString() ?? "Unknown"}";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching activity: {ex.Message}");
            }
            return "No activity detected";
        }
    }
}
