
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

        public async Task<bool> WaitForWakeWordAsync(CancellationToken cancellationToken = default)
        {
            var settings = await _settingsService.LoadSettingsAsync();
            var wakeWordScript = settings.WakeWordScript ?? "porcupine_wakeword.py";
            var keywordFile = settings.WakeWordKeywordFile ?? "keyword.ppn";
            var modelFile = settings.WakeWordModelFile ?? "porcupine_params.pv";
            var audioDevice = settings.WakeWordAudioDevice ?? "default";

            var psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"{wakeWordScript} --keyword_file {keywordFile} --model_file {modelFile} --audio_device {audioDevice}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            while (!process.HasExited && !cancellationToken.IsCancellationRequested)
            {
                var line = await process.StandardOutput.ReadLineAsync();
                if (line != null && line.Contains("wakeword_detected"))
                {
                    process.Kill();
                    return true;
                }
            }
            return false;
        }
    }
}
