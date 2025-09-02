
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class PersonalityService
    {
        private readonly ISettingsService _settingsService;
        private List<Personality> _personalities = new();
        private Personality? _activePersonality;

        public PersonalityService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task LoadPersonalitiesAsync()
        {
            var settings = await _settingsService.LoadSettingsAsync();
            _personalities = settings.Personalities ?? new List<Personality>();
            _activePersonality = _personalities.FirstOrDefault(p => p.Name == settings.ActivePersonalityName) ?? _personalities.FirstOrDefault();
        }

        public List<Personality> GetPersonalities()
        {
            return _personalities;
        }

        public Personality? GetActivePersonality()
        {
            return _activePersonality;
        }

        public async Task SetActivePersonalityAsync(string name)
        {
            var settings = await _settingsService.LoadSettingsAsync();
            settings.ActivePersonalityName = name;
            await _settingsService.SaveSettingsAsync(settings);
            _activePersonality = _personalities.FirstOrDefault(p => p.Name == name);
        }

        public async Task AddPersonalityAsync(Personality personality)
        {
            _personalities.Add(personality);
            var settings = await _settingsService.LoadSettingsAsync();
            settings.Personalities = _personalities;
            await _settingsService.SaveSettingsAsync(settings);
        }

        public async Task UpdatePersonalityAsync(Personality personality)
        {
            var existingPersonality = _personalities.FirstOrDefault(p => p.Name == personality.Name);
            if (existingPersonality != null)
            {
                existingPersonality.Prompt = personality.Prompt;
                var settings = await _settingsService.LoadSettingsAsync();
                settings.Personalities = _personalities;
                await _settingsService.SaveSettingsAsync(settings);
            }
        }

        public async Task DeletePersonalityAsync(string name)
        {
            _personalities.RemoveAll(p => p.Name == name);
            var settings = await _settingsService.LoadSettingsAsync();
            settings.Personalities = _personalities;
            await _settingsService.SaveSettingsAsync(settings);
        }
    }
}
