
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class WifiPresence
    {
        private readonly SettingsService _settingsService;

        public WifiPresence(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<bool> IsPhoneConnected()
        {
            var settings = await _settingsService.GetSettings();
            if (string.IsNullOrEmpty(settings?.PhoneIp))
            {
                return false;
            }

            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(settings.PhoneIp, 1000); // 1-second timeout
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Handle exceptions (e.g., invalid IP format)
                return false;
            }
        }
    }
}
