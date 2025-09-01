using System.Diagnostics;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class STTService
    {
        // Uses Vosk via command line for hardware efficiency
        public async Task<string> ListenAsync(string modelPath = "model")
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"vosk_stt.py {modelPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            string result = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return result.Trim();
        }
    }
}