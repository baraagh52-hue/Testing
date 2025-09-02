#!/bin/bash

# Install .NET 8 SDK
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --version latest

# Add .dotnet to path
export PATH="$HOME/.dotnet:$PATH"

# Install Dependencies
sudo apt-get update
sudo apt-get install -y libicu-dev libssl-dev

# Clone Repository
git clone https://github.com/prompt-engineering/ai-assistant.git
cd ai-assistant

# Build and Publish
$HOME/.dotnet/dotnet publish Ai_Assistant/Ai_Assistant.sln -c Release -r linux-x64 --self-contained true

# Create .desktop file
cat << EOF > ~/.local/share/applications/ai-assistant.desktop
[Desktop Entry]
Name=AI Assistant
Exec=$HOME/ai-assistant/Ai_Assistant/Ai_Assistant.Blazor/bin/Release/net8.0/linux-x64/publish/Ai_Assistant.Blazor
Icon=$HOME/ai-assistant/icon.png
Terminal=false
Type=Application
Categories=Utility;
EOF

# Create symbolic link
sudo ln -s $HOME/ai-assistant/Ai_Assistant/Ai_Assistant.Blazor/bin/Release/net8.0/linux-x64/publish/Ai_Assistant.Blazor /usr/local/bin/ai-assistant
