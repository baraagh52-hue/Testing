
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class LocalLLMService
    {
        private readonly HttpClient _httpClient = new();
        private readonly ISettingsService _settingsService;
        private readonly PersonalityService _personalityService;

        public LocalLLMService(ISettingsService settingsService, PersonalityService personalityService)
        {
            _settingsService = settingsService;
            _personalityService = personalityService;
        }

        public async Task<string> GetResponseAsync(string userInput)
        {
            var settings = await _settingsService.LoadSettingsAsync();
            if (string.IsNullOrEmpty(settings.LocalLLMUrl))
            {
                return "Local LLM URL is not configured.";
            }

            await _personalityService.LoadPersonalitiesAsync();
            var activePersonality = _personalityService.GetActivePersonality();
            var prompt = activePersonality?.Prompt ?? "You are a helpful assistant.";

            var requestBody = new
            {
                model = "mistral", // This can be customized if needed
                prompt = $"{prompt}\n\nUser: {userInput}\nAssistant:",
                stream = false
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(settings.LocalLLMUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseBody);
                return jsonDoc.RootElement.GetProperty("response").GetString();
            }
            else
            {
                return "Error communicating with the local LLM.";
            }
        }
    }
}
