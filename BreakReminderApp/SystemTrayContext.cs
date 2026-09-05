using System.Windows;

namespace BreakReminderApp
{
    /// <summary>
    /// Hidden window that serves as the application context
    /// No taskbar presence, runs silently in system tray
    /// </summary>
    public class SystemTrayContext : Window
    {
        public SystemTrayContext()
        {
            // Make window invisible and remove from taskbar
            Width = 0;
            Height = 0;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = System.Windows.Media.Brushes.Transparent;
            Opacity = 0;
            
            // Hide immediately
            Hide();
            
            // Prevent user from showing this window
            IsVisibleChanged += (s, e) =>
            {
                if (IsVisible)
                    Hide();
            };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            // Ensure window stays hidden
            Hide();
        }
    }
}
