# Break Reminder App

A minimal, resource-efficient Windows desktop application that reminds users to take regular screen breaks and stay hydrated.

## Features

### Core Functionality
- **Hydration Reminders**: Configurable intervals (default: every 25 minutes)
- **Screen Break Reminders**: Configurable intervals (default: every 60 minutes)
- **Water Tracking**: Simple tracker with daily goals and weekly summaries
- **System Tray Integration**: Runs silently with no taskbar presence

### Smart Features
- **Idle Detection**: Automatically pauses when system is idle for 5+ minutes
- **Fullscreen Detection**: Doesn't interrupt presentations, videos, or games
- **Active Hours**: Only remind during configured working hours (default: 9 AM - 6 PM)
- **Do Not Disturb**: Respects fullscreen applications

### Customization
- Hydration interval: 15-120 minutes
- Break interval: 30-180 minutes  
- Break duration: 1-15 minutes
- Daily water goal: 1-20 glasses (250ml each by default)
- Notification styles: Toast, Popup, Sound, or Silent
- Theme support: Light and Dark modes

### Resource Efficiency
- Memory Usage: Under 30MB RAM idle
- CPU Usage: Less than 1% average
- Storage: Under 20MB installed
- Single executable with no external dependencies
- Efficient timer implementation (single thread, no polling)

## Technical Requirements

- **OS**: Windows 10/11 (64-bit)
- **Framework**: .NET 8.0 with WPF
- **Language**: C#

## Building

```bash
cd BreakReminderApp
dotnet restore
dotnet build --configuration Release
```

## Running

```bash
dotnet run --project BreakReminderApp.csproj
```

Or publish as a single executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Configuration

Settings are stored in `%LOCALAPPDATA%\BreakReminderApp\settings.json`

Water tracking data is stored in `%LOCALAPPDATA%\BreakReminderApp\water_data.json`

## Default Settings

| Setting | Default Value |
|---------|--------------|
| Hydration Reminder | Every 25 minutes |
| Screen Break | Every 60 minutes |
| Break Duration | 5 minutes |
| Daily Water Goal | 8 glasses (250ml each) |
| Active Hours | 9:00 AM - 6:00 PM |
| Hydration Snooze | 5 minutes |
| Break Snooze | 10 minutes |
| Theme | Light |

## Project Structure

```
BreakReminderApp/
├── Models/
│   ├── AppSettings.cs       # Application settings model
│   └── WaterTracking.cs     # Water tracking data models
├── Services/
│   ├── SettingsService.cs        # Settings persistence (JSON)
│   ├── NotificationService.cs    # Windows toast notifications
│   ├── ReminderTimerService.cs   # Timer management
│   └── WaterTrackingService.cs   # Water intake tracking
├── Views/
│   └── SettingsWindow.xaml.cs    # Settings UI
├── Assets/                       # Icons and resources
├── App.xaml                      # Application definition
├── App.xaml.cs                   # Application entry point
├── NotifyIconWrapper.cs          # System tray integration
└── SystemTrayContext.cs          # Hidden context window
```

## License

Copyright 2024 Break Reminder Team
