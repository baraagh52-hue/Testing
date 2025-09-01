using System.Net.NetworkInformation;

namespace Ai_Assistant
{
    public class WifiPresence
    {
        public bool IsPhoneConnected(string phoneIp = "192.168.1.100")
        {
            try
            {
                using var ping = new Ping();
                var reply = ping.Send(phoneIp, 1000);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}