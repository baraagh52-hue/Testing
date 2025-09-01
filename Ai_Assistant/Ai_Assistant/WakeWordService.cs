using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class WakeWordService
    {
        private readonly string _porcupineScript;
        private readonly string _keywordFile;
        private readonly string _modelFile;
        private readonly string _audioDevice;

        public WakeWordService(string porcupineScript = "porcupine_wakeword.py", string keywordFile = "keyword.ppn", string modelFile = "porcupine_params.pv", string audioDevice = "default")
        {
            _porcupineScript = porcupineScript;
            _keywordFile = keywordFile;
            _modelFile = modelFile;
            _audioDevice = audioDevice;
        }

        public async Task<bool> WaitForWakeWordAsync(CancellationToken cancellationToken = default)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"{_porcupineScript} --keyword_file {_keywordFile} --model_file {_modelFile} --audio_device {_audioDevice}",
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
