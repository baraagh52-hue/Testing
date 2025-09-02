using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Ai_Assistant
{
    public class PrayerTimes
    {
        private readonly ISettingsService _settingsService;
        private readonly HttpClient _httpClient;

        public PrayerTimes(ISettingsService settingsService, HttpClient httpClient)
        {
            _settingsService = settingsService;
            _httpClient = httpClient;
        }

        public async Task<string> GetPrayerTimesAsync()
        {
            var settings = await _settingsService.LoadSettingsAsync();
            if (string.IsNullOrEmpty(settings?.City) || string.IsNullOrEmpty(settings.Country) || string.IsNullOrEmpty(settings.PrayerTimesApiUrl))
            {
                return "Prayer times settings are not configured.";
            }

            try
            {
                var response = await _httpClient.GetStringAsync($"{settings.PrayerTimesApiUrl}?city={settings.City}&country={settings.Country}");
                var prayerData = JObject.Parse(response);
                var times = prayerData["data"]?["timings"];

                if (times == null)
                {
                    return "Could not retrieve prayer times.";
                }

                var now = DateTime.Now;
                string? nextPrayerName = null;
                DateTime nextPrayerTime = DateTime.MaxValue;

                foreach (var prayer in times.Children<JProperty>())
                {
                    if (DateTime.TryParse(prayer.Value.ToString(), out DateTime prayerTime) && prayerTime > now)
                    {
                        if (prayerTime < nextPrayerTime)
                        {
                            nextPrayerTime = prayerTime;
                            nextPrayerName = prayer.Name;
                        }
                    }
                }

                return nextPrayerName != null ? $"{nextPrayerName} at {nextPrayerTime:h:mm tt}" : "No more prayers for today.";
            }
            catch (Exception ex)
            {
                // Log the exception
                return $"Error retrieving prayer times: {ex.Message}";
            }
        }
    }
}
