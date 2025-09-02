
using System.Diagnostics;
using System.Threading;
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

        // Uses Vosk via command line for hardware efficiency
        public async Task<string> ListenAsync(CancellationToken cancellationToken = default)
        {
            var settings = await _settingsService.LoadSettingsAsync();
            var sttScript = settings.SttScript ?? "vosk_stt.py";
            var modelPath = settings.SttModelPath ?? "model";

            var psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"{sttScript} {modelPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            cancellationToken.Register(() => process.Kill());
            string result = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return result.Trim();
        }
    }
}
