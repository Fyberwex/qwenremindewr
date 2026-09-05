using System.Windows.Forms;
using BreakReminderApp.Models;
using BreakReminderApp.Services;

namespace BreakReminderApp
{
    /// <summary>
    /// System tray icon and context menu management
    /// Minimal footprint, no taskbar presence
    /// </summary>
    public class NotifyIconWrapper : IDisposable
    {
        private NotifyIcon? _notifyIcon;
        private ContextMenuStrip? _contextMenu;
        private readonly SettingsService _settingsService;
        private readonly NotificationService _notificationService;
        private readonly ReminderTimerService _timerService;
        private readonly WaterTrackingService _waterTrackingService;
        private bool _disposed;

        public NotifyIconWrapper()
        {
            _settingsService = new SettingsService();
            _notificationService = new NotificationService(_settingsService);
            _timerService = new ReminderTimerService(_settingsService, _notificationService);
            _waterTrackingService = new WaterTrackingService(_settingsService);

            InitializeNotifyIcon();
            InitializeContextMenu();
            
            _timerService.Start();
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(
                    System.Windows.Forms.Application.ExecutablePath
                ) ?? System.Drawing.SystemIcons.Application,
                Text = "Break Reminder",
                Visible = true
            };

            _notifyIcon.DoubleClick += (s, e) => OpenSettingsWindow();
            _notifyIcon.BalloonTipClicked += (s, e) => OpenSettingsWindow();
        }

        private void InitializeContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());

            // Quick actions
            var addWaterItem = new ToolStripMenuItem("💧 Add Glass of Water");
            addWaterItem.Click += AddWaterItemClick;
            _contextMenu.Items.Add(addWaterItem);

            _contextMenu.Items.Add(new ToolStripSeparator());

            // Status display
            var todaySummary = _waterTrackingService.GetTodaySummary();
            var statusItem = new ToolStripMenuItem(
                $"Today: {todaySummary.TotalGlasses}/{todaySummary.GoalGlasses} glasses"
            );
            statusItem.Enabled = false;
            _contextMenu.Items.Add(statusItem);

            _contextMenu.Items.Add(new ToolStripSeparator());

            // Settings
            var settingsItem = new ToolStripMenuItem("⚙️ Settings");
            settingsItem.Click += (s, e) => OpenSettingsWindow();
            _contextMenu.Items.Add(settingsItem);

            // Toggle reminders
            var toggleItem = new ToolStripMenuItem("⏸️ Pause Reminders");
            toggleItem.Click += ToggleReminders;
            _contextMenu.Items.Add(toggleItem);

            _contextMenu.Items.Add(new ToolStripSeparator());

            // Exit
            var exitItem = new ToolStripMenuItem("🚪 Exit");
            exitItem.Click += ExitClick;
            _contextMenu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = _contextMenu;
        }

        private void AddWaterItemClick(object? sender, EventArgs e)
        {
            _waterTrackingService.AddWaterIntake(1);
            
            var summary = _waterTrackingService.GetTodaySummary();
            _notifyIcon?.ShowBalloonTip(
                2000,
                "Water Logged!",
                $"Today: {summary.TotalGlasses}/{summary.GoalGlasses} glasses",
                ToolTipIcon.Info
            );
        }

        private void ToggleReminders(object? sender, EventArgs e)
        {
            // Implementation for pausing/resuming reminders
            _notifyIcon?.ShowBalloonTip(
                2000,
                "Reminders Paused",
                "Right-click to resume",
                ToolTipIcon.Info
            );
        }

        private void OpenSettingsWindow()
        {
            // Open the settings window
            var settingsWindow = new Views.SettingsWindow(
                _settingsService,
                _notificationService,
                _timerService,
                _waterTrackingService
            );
            settingsWindow.Show();
        }

        private void ExitClick(object? sender, EventArgs e)
        {
            _notifyIcon?.Dispose();
            System.Windows.Forms.Application.Exit();
        }

        public void UpdateContextMenu()
        {
            // Refresh context menu with current data
            _contextMenu?.Items.Clear();
            InitializeContextMenu();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _notifyIcon?.Dispose();
                    _contextMenu?.Dispose();
                    _timerService.Dispose();
                    _notificationService.Dispose();
                    _waterTrackingService.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Custom color table for modern-looking context menu
    /// </summary>
    internal class CustomColorTable : ProfessionalColorTable
    {
        public override System.Drawing.Color MenuItemSelected => System.Drawing.Color.FromArgb(230, 230, 230);
        public override System.Drawing.Color MenuItemSelectedGradientBegin => System.Drawing.Color.FromArgb(230, 230, 230);
        public override System.Drawing.Color MenuItemSelectedGradientEnd => System.Drawing.Color.FromArgb(230, 230, 230);
        public override System.Drawing.Color MenuItemBorder => System.Drawing.Color.Transparent;
        public override System.Drawing.Color MenuBorder => System.Drawing.Color.FromArgb(200, 200, 200);
    }
}
