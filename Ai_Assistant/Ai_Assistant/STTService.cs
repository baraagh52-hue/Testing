using System.Diagnostics;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class STTService
    {
        private readonly ISettingsService _settingsService;

        public STTService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<string> RecognizeFromMicrophoneAsync()
        {
            var settings = await _settingsService.LoadSettingsAsync();
            if (string.IsNullOrEmpty(settings?.SttScript) || string.IsNullOrEmpty(settings.SttModelPath))
            {
                return "STT settings are not configured.";
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"{settings.SttScript} --model_path {settings.SttModelPath}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string? result = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return result?.Trim() ?? string.Empty;
        }
    }
}
