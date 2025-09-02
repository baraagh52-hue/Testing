
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class ToDoIntegration
    {
        private readonly HttpClient _httpClient = new();
        private readonly ISettingsService _settingsService;

        public ToDoIntegration(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<List<string>> GetOverdueTasksAsync()
        {
            var overdueTasks = new List<string>();
            var settings = await _settingsService.LoadSettingsAsync();
            if (string.IsNullOrEmpty(settings.ApiToken))
                return overdueTasks;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiToken);
            var url = "https://graph.microsoft.com/v1.0/me/todo/lists";
            var listsResponse = await _httpClient.GetStringAsync(url);
            using var listsDoc = JsonDocument.Parse(listsResponse);
            var lists = listsDoc.RootElement.GetProperty("value");
            foreach (var list in lists.EnumerateArray())
            {
                var listId = list.GetProperty("id").GetString();
                var tasksUrl = $"https://graph.microsoft.com/v1.0/me/todo/lists/{listId}/tasks";
                var tasksResponse = await _httpClient.GetStringAsync(tasksUrl);
                using var tasksDoc = JsonDocument.Parse(tasksResponse);
                var tasks = tasksDoc.RootElement.GetProperty("value");
                foreach (var task in tasks.EnumerateArray())
                {
                    if (task.TryGetProperty("status", out var statusProp) && statusProp.GetString() == "notStarted" &&
                        task.TryGetProperty("dueDateTime", out var dueProp) &&
                        dueProp.TryGetProperty("dateTime", out var dateTimeProp))
                    {
                        if (System.DateTime.TryParse(dateTimeProp.GetString(), out var dueDate) && dueDate < System.DateTime.UtcNow)
                        {
                            overdueTasks.Add(task.GetProperty("title").GetString());
                        }
                    }
                }
            }
            return overdueTasks;
        }
    }
}
