using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ai_Assistant;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register assistant services
builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddSingleton<ToDoIntegration>();
builder.Services.AddSingleton<PrayerTimes>();
builder.Services.AddSingleton<ActivityWatchIntegration>();
builder.Services.AddSingleton<WifiPresence>();
builder.Services.AddSingleton<TTSService>();
builder.Services.AddSingleton<STTService>();
builder.Services.AddSingleton<WakeWordService>();
builder.Services.AddHostedService<AssistantService>();
builder.Services.AddSingleton<PersonalityService>();
builder.Services.AddSingleton<LocalLLMService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
