using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    class Program
    {
        static ToDoIntegration toDoIntegration = new(/* accessToken here */);
        static ActivityWatchIntegration activityWatchIntegration = new();
        static PrayerTimes prayerTimes = new();
        static WifiPresence wifiPresence = new();
        static TTSService ttsService = new();
        static STTService sttService = new();
        static WakeWordService wakeWordService = new();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting AI Assistant...");
            await ttsService.SpeakAsync("Starting AI Assistant");

            // Initialize components
            InitializeVoiceInterface();
            await InitializeTaskManagementIntegration();
            await InitializePrayerReminders();
            InitializeHomePresenceDetection();
            await InitializeActivityMonitoring();

            Console.WriteLine("AI Assistant is running. Listening for commands...");
            await ttsService.SpeakAsync("AI Assistant is running. Listening for commands.");

            // Wake word loop
            var cts = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    bool detected = await wakeWordService.WaitForWakeWordAsync(cts.Token);
                    if (detected)
                    {
                        await ttsService.SpeakAsync("Wake word detected. How can I help you?");
                        // You can add STT listening and command handling here
                    }
                }
            });

            // Example: Periodically check and display status
            while (true)
            {
                await CheckTasks();
                await CheckPrayerTimes();
                CheckWifiPresence();
                await CheckActivity();
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }

        static void InitializeVoiceInterface()
        {
            Console.WriteLine("Initializing voice interface...");
            ttsService.SpeakAsync("Voice interface initialized").Wait();
            // Wake word detection is now running in background
        }

        static async Task InitializeTaskManagementIntegration()
        {
            Console.WriteLine("Initializing task management integration...");
            await ttsService.SpeakAsync("Task management integration initialized");
            await Task.CompletedTask;
        }

        static async Task InitializePrayerReminders()
        {
            Console.WriteLine("Initializing prayer reminders...");
            await ttsService.SpeakAsync("Prayer reminders initialized");
            await Task.CompletedTask;
        }

        static void InitializeHomePresenceDetection()
        {
            Console.WriteLine("Initializing home presence detection...");
            ttsService.SpeakAsync("Home presence detection initialized").Wait();
        }

        static async Task InitializeActivityMonitoring()
        {
            Console.WriteLine("Initializing activity monitoring...");
            await ttsService.SpeakAsync("Activity monitoring initialized");
            await Task.CompletedTask;
        }

        static async Task CheckTasks()
        {
            var overdueTasks = await toDoIntegration.GetOverdueTasksAsync();
            if (overdueTasks.Count > 0)
            {
                var msg = $"You have {overdueTasks.Count} overdue tasks: {string.Join(", ", overdueTasks)}";
                Console.WriteLine(msg);
                await ttsService.SpeakAsync(msg);
            }
        }

        static async Task CheckPrayerTimes()
        {
            var times = await prayerTimes.GetPrayerTimesAsync("YourCity", "YourCountry");
            if (times.Count > 0)
            {
                var msg = "Today's prayer times: " + string.Join(", ", times.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
                Console.WriteLine(msg);
                await ttsService.SpeakAsync(msg);
            }
        }

        static void CheckWifiPresence()
        {
            bool isConnected = wifiPresence.IsPhoneConnected();
            var msg = isConnected ? "Phone is connected to Wi-Fi." : "Phone is not connected.";
            Console.WriteLine(msg);
            ttsService.SpeakAsync(msg).Wait();
        }

        static async Task CheckActivity()
        {
            var activity = await activityWatchIntegration.GetCurrentActivityAsync();
            var msg = $"Current PC activity: {activity}";
            Console.WriteLine(msg);
            await ttsService.SpeakAsync(msg);
        }
    }
}