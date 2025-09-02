using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class AssistantService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly WakeWordService _wakeWordService;
        private readonly TTSService _ttsService;
        private readonly STTService _sttService;
        private readonly LocalLLMService _localLLMService;
        private readonly WeatherService _weatherService;
        private readonly ActivityWatchService _activityWatchService;
        private readonly WifiPresence _wifiPresence;
        private readonly PrayerTimes _prayerTimes;
        private readonly ToDoIntegration _toDoIntegration;
        private readonly PersonalityService _personalityService;

        public AssistantService(
            IServiceProvider serviceProvider,
            WakeWordService wakeWordService,
            TTSService ttsService,
            STTService sttService,
            LocalLLMService localLLMService,
            WeatherService weatherService,
            ActivityWatchService activityWatchService,
            WifiPresence wifiPresence,
            PrayerTimes prayerTimes,
            ToDoIntegration toDoIntegration,
            PersonalityService personalityService)
        {
            _serviceProvider = serviceProvider;
            _wakeWordService = wakeWordService;
            _ttsService = ttsService;
            _sttService = sttService;
            _localLLMService = localLLMService;
            _weatherService = weatherService;
            _activityWatchService = activityWatchService;
            _wifiPresence = wifiPresence;
            _prayerTimes = prayerTimes;
            _toDoIntegration = toDoIntegration;
            _personalityService = personalityService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000, stoppingToken); // Initial delay

            while (!stoppingToken.IsCancellationRequested)
            {
                if (await _wakeWordService.IsWakeWordDetected(stoppingToken))
                {
                    await HandleWakeWord();
                }
                await Task.Delay(100, stoppingToken); // Small delay to prevent tight loop
            }
        }

        private async Task HandleWakeWord()
        {
            var sttTask = _sttService.RecognizeFromMicrophoneAsync();
            var transcription = await sttTask;

            if (string.IsNullOrEmpty(transcription))
            {
                await _ttsService.SpeakAsync("Sorry, I didn't catch that.");
                return;
            }

            var response = await _localLLMService.GenerateResponse(transcription);

            // Keyword-based actions
            if (response.Contains("weather"))
            {
                var weather = await _weatherService.GetWeatherAsync();
                response = $"The current weather is {weather}";
            }
            else if (response.Contains("next prayer"))
            {
                var nextPrayer = await _prayerTimes.GetNextPrayer();
                response = $"The next prayer is {nextPrayer}";
            }
            else if (response.Contains("phone connected"))
            {
                var isConnected = await _wifiPresence.IsPhoneConnected();
                response = isConnected ? "Yes, your phone is connected to the network." : "No, your phone is not on the network.";
            }
            else if (response.Contains("unproductive time"))
            {
                var unproductiveTime = await _activityWatchService.GetTotalUnproductiveTimeAsync();
                response = $"You have spent {unproductiveTime} on unproductive tasks today.";
            }
            else if (response.Contains("add to-do"))
            {
                // Extract the to-do item from the response
                string? toDoItem = response.Split("add to-do").LastOrDefault()?.Trim();
                if (!string.IsNullOrEmpty(toDoItem))
                {
                    await _toDoIntegration.AddToDoItem(toDoItem);
                    response = $"I've added \"{toDoItem}\" to your to-do list.";
                }
                else
                {
                    response = "Sorry, I couldn't figure out what to add.";
                }
            }

            await _ttsService.SpeakAsync(response);
        }
    }
}
