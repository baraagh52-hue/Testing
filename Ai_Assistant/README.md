# AI Assistant

This is a personal AI assistant application with a Blazor web interface. It provides a dashboard with useful information from various integrations.

## Features

- **To-Do List Integration:** Displays overdue tasks from your to-do list.
- **Prayer Times:** Shows daily prayer times for your location.
- **ActivityWatch Integration:** Monitors your current PC activity.
- **Wi-Fi Presence:** Detects if your phone is connected to the local Wi-Fi network.
- **Speech-to-Text and Text-to-Speech:** (Coming Soon) Voice interaction capabilities.
- **Wake Word Detection:** (Coming Soon) Activate the assistant with a wake word.

## Technologies Used

- .NET 8
- C#
- Blazor Server
- ASP.NET Core

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/Ai_Assistant.git
   ```
2. Navigate to the project directory:
   ```bash
   cd Ai_Assistant/Ai_Assistant.Blazor
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Open your browser and navigate to `https://localhost:5001` (or the address shown in the console).

## Configuration

You will need to configure the following integrations in the `appsettings.json` file (you may need to create this file):

- **ToDoIntegration:** Your to-do list provider's API key and endpoint.
- **PrayerTimes:** Your city and country.
- **ActivityWatch:** The ActivityWatch server address.
- **WifiPresence:** The MAC address of your phone.
