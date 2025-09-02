
using System;
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
            if (settings == null || string.IsNullOrEmpty(settings.ApiToken))
            {
                Console.WriteLine("ToDoIntegration: API token is not configured in settings.");
                return overdueTasks;
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiToken);
                var url = "https://graph.microsoft.com/v1.0/me/todo/lists";
                var listsResponse = await _httpClient.GetStringAsync(url);
                using var listsDoc = JsonDocument.Parse(listsResponse);
                if (listsDoc.RootElement.TryGetProperty("value", out var lists))
                {
                    foreach (var list in lists.EnumerateArray())
                    {
                        if (list.TryGetProperty("id", out var idElement))
                        {
                            var listId = idElement.GetString();
                            if (string.IsNullOrEmpty(listId)) continue;

                            var tasksUrl = $"https://graph.microsoft.com/v1.0/me/todo/lists/{listId}/tasks";
                            var tasksResponse = await _httpClient.GetStringAsync(tasksUrl);
                            using var tasksDoc = JsonDocument.Parse(tasksResponse);
                            if (tasksDoc.RootElement.TryGetProperty("value", out var tasks))
                            {
                                foreach (var task in tasks.EnumerateArray())
                                {
                                    if (task.TryGetProperty("status", out var statusProp) && statusProp.GetString() == "notStarted" &&
                                        task.TryGetProperty("dueDateTime", out var dueProp) &&
                                        dueProp.TryGetProperty("dateTime", out var dateTimeProp) && dateTimeProp.GetString() != null &&
                                        DateTime.TryParse(dateTimeProp.GetString(), out var dueDate) && dueDate < DateTime.UtcNow)
                                    {
                                        if (task.TryGetProperty("title", out var titleValue))
                                        {
                                            overdueTasks.Add(titleValue.GetString() ?? string.Empty);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching overdue tasks: {ex.Message}");
            }
            return overdueTasks;
        }
    }
}
