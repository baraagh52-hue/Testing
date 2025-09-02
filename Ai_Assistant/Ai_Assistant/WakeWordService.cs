using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class WakeWordService
    {
        private readonly SettingsService _settingsService;

        public WakeWordService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<bool> IsWakeWordDetected(CancellationToken cancellationToken)
        {
            var settings = await _settingsService.LoadSettingsAsync();
            if (string.IsNullOrEmpty(settings?.WakeWordScript))
            {
                return false;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"{settings.WakeWordScript} --keyword_file_path {settings.WakeWordKeywordFile} --model_file_path {settings.WakeWordModelFile} --audio_device_index {settings.WakeWordAudioDevice}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            await process.WaitForExitAsync(cancellationToken);

            return process.ExitCode == 0;
        }
    }
}
