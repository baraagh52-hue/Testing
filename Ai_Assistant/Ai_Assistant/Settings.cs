
using System.Collections.Generic;

namespace Ai_Assistant
{
    public class Settings
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string ApiToken { get; set; }
        public string PhoneIp { get; set; }
        public List<Personality> Personalities { get; set; }
        public string ActivePersonalityName { get; set; }
        public string LocalLLMUrl { get; set; }
        public string ActivityWatchUrl { get; set; }
        public string LocalLLMModel { get; set; }
        public string PrayerTimesApiUrl { get; set; }
        public string TtsScript { get; set; }
        public string TtsOutputPath { get; set; }
        public string SttScript { get; set; }
        public string SttModelPath { get; set; }
        public string WakeWordScript { get; set; }
        public string WakeWordKeywordFile { get; set; }
        public string WakeWordModelFile { get; set; }
        public string WakeWordAudioDevice { get; set; }
    }
}
