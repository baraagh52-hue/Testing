
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class PrayerTimes
    {
        private readonly HttpClient _httpClient = new();
        private readonly ISettingsService _settingsService;

        public PrayerTimes(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<Dictionary<string, string>> GetPrayerTimesAsync()
        {
            var timings = new Dictionary<string, string>();
            try
            {
                var settings = await _settingsService.LoadSettingsAsync();
                var encodedCity = Uri.EscapeDataString(settings.City ?? "");
                var encodedCountry = Uri.EscapeDataString(settings.Country ?? "");
                var url = $"{settings.PrayerTimesApiUrl}?city={encodedCity}&country={encodedCountry}";
                Console.WriteLine($"Requesting: {url}");
                var response = await _httpClient.GetStringAsync(url);
                using var doc = JsonDocument.Parse(response);
                var timingsElement = doc.RootElement.GetProperty("data").GetProperty("timings");
                foreach (var timing in timingsElement.EnumerateObject())
                {
                    timings[timing.Name] = timing.Value.GetString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching prayer times: {ex.Message}");
            }
            return timings;
        }
    }
}
