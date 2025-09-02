
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class TTSService
    {
        private readonly ISettingsService _settingsService;

        public TTSService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        // Uses Coqui TTS via command line for hardware efficiency
        public async Task SpeakAsync(string text)
        {
            var settings = await _settingsService.LoadSettingsAsync();
            var ttsScript = settings.TtsScript ?? "tts";
            var outputPath = settings.TtsOutputPath ?? "output.wav";

            var psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"{ttsScript} --text \"{text.Replace("\"", "'")}\" --out_path {outputPath} && ffplay -nodisp -autoexit {outputPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            await process.WaitForExitAsync();
        }
    }
}
