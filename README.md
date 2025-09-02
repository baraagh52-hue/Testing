# AI Assistant

This is a personal AI assistant application built with .NET and Blazor. It provides a range of features to help you with your daily tasks and information needs.

## Features

*   **Voice Interaction:** Uses Speech-to-Text (STT) and Text-to-Speech (TTS) for hands-free interaction with a local Large Language Model (LLM).
*   **Wake Word Detection:** Listens for a wake word to start interacting.
*   **Prayer Times:** Calculates and displays daily prayer times in 24-hour format, along with the time remaining until the next prayer.
*   **To-Do List Integration:** Manages your tasks by integrating with a to-do list.
*   **Presence Detection:** Can detect user presence via WiFi networks.
*   **Activity Tracking:** Integrates with ActivityWatch to monitor your activity and provide "nudges" when you're on unproductive apps.
*   **Dashboard:** A central dashboard to view information from all integrated services.
*   **Centralized Settings:** All service settings are managed in a single `settings.json` file.
*   **Settings UI:** A dedicated settings page in the Blazor UI allows for easy modification of all settings.

## Getting Started

This project is designed to be packaged and installed as a Debian (`.deb`) file for easy installation on Debian-based Linux distributions like Ubuntu.

### Prerequisites

To build the package, you will need to have the .NET 8 SDK installed.

```bash
# Download and run the install script
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --version latest

# Add .NET to your PATH
export PATH="$HOME/.dotnet:$PATH"
```

### Configuration

Before building, you may need to configure the application by creating a `settings.json` file in the `Ai_Assistant/Ai_Assistant.Blazor/` directory. This file holds the configuration for the various services. An example `settings.example.json` is provided.

### Setting Up ActivityWatch

The AI Assistant uses ActivityWatch to monitor your PC activity. To get this feature working, you need to:

1.  **Install ActivityWatch:** Download and install the latest version of ActivityWatch from the [official website](https://activitywatch.net/downloads).
2.  **Enable the REST API:** The AI Assistant communicates with ActivityWatch through its REST API. Make sure it is enabled in your ActivityWatch settings.
3.  **Configure the URL:** In the AI Assistant's settings page, ensure the "ActivityWatch URL" is set to the correct address (by default, it's `http://localhost:5600`).

### Building the Debian Package

1.  **Publish the Application:**
    This command builds the project and stages it in the correct directory for packaging.

    ```bash
    $HOME/.dotnet/dotnet publish Ai_Assistant/Ai_Assistant.sln -c Release -r linux-x64 --self-contained true -o Ai_Assistant/Ai_Assistant.Blazor/debian/opt/ai-assistant
    ```

2.  **Build the Package:**
    This command uses `dpkg-deb` to create the `.deb` file.

    ```bash
    dpkg-deb --build Ai_Assistant/Ai_Assistant.Blazor/debian
    ```

    This will create a `debian.deb` file in your project directory.

### Installation

Install the application using `apt`. This will also automatically handle the installation of required dependencies.

```bash
sudo apt install ./debian.deb
```

## Usage

After installation, you can launch the AI Assistant from your desktop's application menu. The application will run in the background.

You can also run it from the terminal with the command:

```bash
ai-assistant
```
The application will be accessible in your web browser, typically at `http://localhost:5000`.
