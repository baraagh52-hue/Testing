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

        public async Task<Dictionary<string, string>> GetPrayerTimesAsync(string city, string country)
        {
            var timings = new Dictionary<string, string>();
            try
            {
                var encodedCity = Uri.EscapeDataString(city ?? "");
                var encodedCountry = Uri.EscapeDataString(country ?? "");
                var url = $"https://api.aladhan.com/v1/timingsByCity?city={encodedCity}&country={encodedCountry}";
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