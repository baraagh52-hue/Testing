using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ai_Assistant
{
    public class LocalLLMService
    {
        private readonly SettingsService _settingsService;
        private readonly HttpClient _httpClient;

        public LocalLLMService(SettingsService settingsService, HttpClient httpClient)
        {
            _settingsService = settingsService;
            _httpClient = httpClient;
        }

        public async Task<string> GenerateResponse(string prompt)
        {
            var settings = await _settingsService.LoadSettingsAsync();
            if (settings == null || string.IsNullOrEmpty(settings.LocalLLMUrl) || string.IsNullOrEmpty(settings.LocalLLMModel))
            {
                return "Settings for local LLM are not configured.";
            }

            var requestData = new
            {
                model = settings.LocalLLMModel,
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(settings.LocalLLMUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic? jsonResponse = JsonConvert.DeserializeObject(responseBody);
                return jsonResponse?.response ?? "Error: Unexpected response format";
            }
            else
            {
                // Log the error or handle it more gracefully
                return $"Error: {response.ReasonPhrase}";
            }
        }
    }
}
