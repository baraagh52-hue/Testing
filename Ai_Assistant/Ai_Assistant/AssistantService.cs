
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class AssistantService : BackgroundService
    {
        private readonly ToDoIntegration _toDoIntegration;
        private readonly ActivityWatchIntegration _activityWatchIntegration;
        private readonly PrayerTimes _prayerTimes;
        private readonly WifiPresence _wifiPresence;
        private readonly TTSService _ttsService;
        private readonly STTService _sttService;
        private readonly WakeWordService _wakeWordService;
        private readonly ISettingsService _settingsService;
        private readonly LocalLLMService _localLLMService;
        private bool _isConversing = false;

        public AssistantService(
            ToDoIntegration toDoIntegration,
            ActivityWatchIntegration activityWatchIntegration,
            PrayerTimes prayerTimes,
            WifiPresence wifiPresence,
            TTSService ttsService,
            STTService sttService,
            WakeWordService wakeWordService,
            ISettingsService settingsService,
            LocalLLMService localLLMService)
        {
            _toDoIntegration = toDoIntegration;
            _activityWatchIntegration = activityWatchIntegration;
            _prayerTimes = prayerTimes;
            _wifiPresence = wifiPresence;
            _ttsService = ttsService;
            _sttService = sttService;
            _wakeWordService = wakeWordService;
            _settingsService = settingsService;
            _localLLMService = localLLMService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Starting AI Assistant Service...");
            await _ttsService.SpeakAsync("Starting AI Assistant Service");

            Console.WriteLine("AI Assistant Service is running. Listening for commands...");
            await _ttsService.SpeakAsync("AI Assistant is running. Listening for commands.");

            // Wake word loop
            _ = Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    bool detected = await _wakeWordService.WaitForWakeWordAsync(stoppingToken);
                    if (detected)
                    {
                        _isConversing = true;
                        await _ttsService.SpeakAsync("Yes?");
                        var userInput = await _sttService.ListenAsync(stoppingToken);
                        if (!string.IsNullOrEmpty(userInput))
                        {
                            await _ttsService.SpeakAsync("Thinking...");
                            var response = await _localLLMService.GetResponseAsync(userInput);
                            await _ttsService.SpeakAsync(response);
                        }
                        else
                        {
                            await _ttsService.SpeakAsync("Sorry, I didn't catch that.");
                        }
                        _isConversing = false;
                    }
                }
            }, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_isConversing)
                {
                    await CheckTasks();
                    await CheckPrayerTimes();
                    await CheckWifiPresence();
                    await CheckActivity();
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CheckTasks()
        {
            if (_isConversing) return;
            var overdueTasks = await _toDoIntegration.GetOverdueTasksAsync();
            if (overdueTasks.Count > 0)
            {
                var msg = $"You have {overdueTasks.Count} overdue tasks: {string.Join(", ", overdueTasks)}";
                Console.WriteLine(msg);
                await _ttsService.SpeakAsync(msg);
            }
        }

        private async Task CheckPrayerTimes()
        {
            if (_isConversing) return;
            var settings = await _settingsService.LoadSettingsAsync();
            var times = await _prayerTimes.GetPrayerTimesAsync(settings.City, settings.Country);
            if (times.Count > 0)
            {
                var msg = "Today's prayer times: " + string.Join(", ", times.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
                Console.WriteLine(msg);
                await _ttsService.SpeakAsync(msg);
            }
        }

        private async Task CheckWifiPresence()
        {
            if (_isConversing) return;
            bool isConnected = _wifiPresence.IsPhoneConnected();
            var msg = isConnected ? "Phone is connected to Wi-Fi." : "Phone is not connected.";
            Console.WriteLine(msg);
            await _ttsService.SpeakAsync(msg);
        }

        private async Task CheckActivity()
        {
            if (_isConversing) return;
            var activity = await _activityWatchIntegration.GetCurrentActivityAsync();
            var msg = $"Current PC activity: {activity}";
            Console.WriteLine(msg);
            await _ttsService.SpeakAsync(msg);
        }
    }
}
