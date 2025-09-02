using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class WakeWordService
    {
        private readonly ISettingsService _settingsService;

        public WakeWordService(ISettingsService settingsService)
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
                    FileName = settings.WakeWordScript,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);

            return output.Trim().ToLower() == "true";
        }
    }
}
