
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class SettingsService : ISettingsService
    {
        private const string SettingsFilePath = "Ai_Assistant/Ai_Assistant/settings.json";

        public async Task<Settings> LoadSettingsAsync()
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = await File.ReadAllTextAsync(SettingsFilePath);
                return JsonSerializer.Deserialize<Settings>(json);
            }
            return new Settings();
        }

        public async Task SaveSettingsAsync(Settings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(SettingsFilePath, json);
        }
    }
}
