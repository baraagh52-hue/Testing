using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath = "settings.json";
        private Settings? _currentSettings;

        public async Task<Settings> LoadSettingsAsync()
        {
            if (_currentSettings == null && File.Exists(_settingsFilePath))
            {
                var json = await File.ReadAllTextAsync(_settingsFilePath);
                _currentSettings = JsonSerializer.Deserialize<Settings>(json);
            }
            return _currentSettings;
        }

        public async Task SaveSettingsAsync(Settings settings)
        {
            _currentSettings = settings;
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_settingsFilePath, json);
        }
    }
}
