using System.Diagnostics;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class TTSService
    {
        // Uses Coqui TTS via command line for hardware efficiency
        public async Task SpeakAsync(string text)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"tts --text \"{text.Replace("\"", "'")}\" --out_path output.wav && ffplay -nodisp -autoexit output.wav",
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