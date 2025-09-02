
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public interface ISettingsService
    {
        Task<Settings> LoadSettingsAsync();
        Task SaveSettingsAsync(Settings settings);
    }
}
