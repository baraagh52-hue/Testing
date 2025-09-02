using System.Diagnostics;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class TTSService
    {
        private readonly SettingsService _settingsService;

        public TTSService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task SpeakAsync(string text)
        {
            var settings = await _settingsService.LoadSettingsAsync();
            if (string.IsNullOrEmpty(settings?.TtsScript) || string.IsNullOrEmpty(settings.TtsOutputPath))
            {
                // Handle missing settings gracefully
                return;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"{settings.TtsScript} --text \"{text}\" --output_path {settings.TtsOutputPath}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            await process.WaitForExitAsync();
        }
    }
}
