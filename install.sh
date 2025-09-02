#!/bin/bash

# Install .NET 8 SDK
if [ ! -f "dotnet-install.sh" ]; then
    wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
fi
chmod +x ./dotnet-install.sh
./dotnet-install.sh --version latest

# Add .dotnet to path and make it permanent
export PATH="$HOME/.dotnet:$PATH"
echo 'export PATH="$HOME/.dotnet:$PATH"' >> "$HOME/.bashrc"

# Install Dependencies
sudo apt-get update
sudo apt-get install -y libicu-dev libssl-dev

# Build and Publish the application
$HOME/.dotnet/dotnet publish Ai_Assistant/Ai_Assistant.sln -c Release -r linux-x64 --self-contained true

# Create .desktop file for application launcher
DESKTOP_FILE_PATH="$HOME/.local/share/applications/ai-assistant.desktop"
mkdir -p "$(dirname "$DESKTOP_FILE_PATH")"
cat << EOF > "$DESKTOP_FILE_PATH"
[Desktop Entry]
Name=AI Assistant
Exec=$(pwd)/Ai_Assistant/Ai_Assistant.Blazor/bin/Release/net8.0/linux-x64/publish/Ai_Assistant.Blazor
Terminal=false
Type=Application
Categories=Utility;
EOF

# Create symbolic link to the executable
sudo rm -f /usr/local/bin/ai-assistant
sudo ln -s "$(pwd)/Ai_Assistant/Ai_Assistant.Blazor/bin/Release/net8.0/linux-x64/publish/Ai_Assistant.Blazor" /usr/local/bin/ai-assistant

echo "Installation complete!"
echo "You can now run 'ai-assistant' from your terminal or find 'AI Assistant' in your application menu."
echo "You may need to log out and log back in for the changes to take effect."
