
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
    }
}
