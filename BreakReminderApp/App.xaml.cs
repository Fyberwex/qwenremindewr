using System.Windows;

namespace BreakReminderApp
{
    /// <summary>
    /// Application entry point and lifecycle management
    /// </summary>
    public partial class App : Application
    {
        private static NotifyIconWrapper? _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the notification system and main context
            _notifyIcon = new NotifyIconWrapper();
            
            // No main window - app runs in system tray only
            Current.MainWindow = new SystemTrayContext();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
