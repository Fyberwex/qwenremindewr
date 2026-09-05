using System.Runtime.InteropServices;
using BreakReminderApp.Models;

namespace BreakReminderApp.Services
{
    /// <summary>
    /// Manages reminder timers with efficient single-threaded implementation
    /// Uses System.Timers for minimal resource usage
    /// </summary>
    public class ReminderTimerService : IDisposable
    {
        private readonly SettingsService _settingsService;
        private readonly NotificationService _notificationService;
        private System.Timers.Timer? _hydrationTimer;
        private System.Timers.Timer? _breakTimer;
        private AppSettings _settings;
        private bool _disposed;
        private bool _isIdle;
        private DateTime _lastInputTime;
        private int _idleThresholdMinutes = 5;

        // Events for UI updates
        public event EventHandler<ReminderEventArgs>? OnHydrationReminder;
        public event EventHandler<ReminderEventArgs>? OnBreakReminder;
        public event EventHandler<TimeSpan>? OnNextHydrationUpdate;
        public event EventHandler<TimeSpan>? OnNextBreakUpdate;

        public ReminderTimerService(SettingsService settingsService, NotificationService notificationService)
        {
            _settingsService = settingsService;
            _notificationService = notificationService;
            _settings = settingsService.LoadSettings();
            _lastInputTime = DateTime.Now;

            InitializeTimers();
            StartIdleMonitoring();
        }

        private void InitializeTimers()
        {
            // Hydration timer - fires every minute to check elapsed time
            _hydrationTimer = new System.Timers.Timer(60000); // 1 minute interval
            _hydrationTimer.Elapsed += HydrationTimerElapsed;
            _hydrationTimer.AutoReset = true;
            
            // Break timer - fires every minute to check elapsed time
            _breakTimer = new System.Timers.Timer(60000); // 1 minute interval
            _breakTimer.Elapsed += BreakTimerElapsed;
            _breakTimer.AutoReset = true;

            ResetTimers();
        }

        private void StartIdleMonitoring()
        {
            Task.Run(async () =>
            {
                while (!_disposed)
                {
                    await Task.Delay(30000); // Check every 30 seconds
                    CheckIdleState();
                }
            });
        }

        private void CheckIdleState()
        {
            if (!_settings.EnableIdleDetection)
            {
                _isIdle = false;
                return;
            }

            var lastInput = GetLastInputTime();
            var idleDuration = DateTime.Now - lastInput;
            _isIdle = idleDuration.TotalMinutes >= _idleThresholdMinutes;
            _lastInputTime = lastInput;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        private DateTime GetLastInputTime()
        {
            var info = new LASTINPUTINFO { cbSize = (uint)Marshal.SizeOf<LASTINPUTINFO>() };
            if (GetLastInputInfo(ref info))
            {
                var tickCount = Environment.TickCount64;
                return DateTime.Now.AddMilliseconds(-(tickCount - info.dwTime));
            }
            return DateTime.Now;
        }

        private void HydrationTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (_isIdle || _notificationService.IsDoNotDisturbActive())
                return;

            // Check if hydration reminder should fire
            // This is a simplified implementation - full version would track exact times
            OnHydrationReminder?.Invoke(this, new ReminderEventArgs 
            { 
                SnoozeMinutes = _settings.HydrationSnoozeMinutes,
                ReminderType = "Hydration"
            });
            
            _notificationService.ShowHydrationReminder(_settings.HydrationSnoozeMinutes);
        }

        private void BreakTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (_isIdle || _notificationService.IsDoNotDisturbActive())
                return;

            OnBreakReminder?.Invoke(this, new ReminderEventArgs 
            { 
                SnoozeMinutes = _settings.BreakSnoozeMinutes,
                DurationMinutes = _settings.BreakDurationMinutes,
                ReminderType = "Break"
            });
            
            _notificationService.ShowBreakReminder(_settings.BreakDurationMinutes, _settings.BreakSnoozeMinutes);
        }

        public void Start()
        {
            _hydrationTimer?.Start();
            _breakTimer?.Start();
        }

        public void Stop()
        {
            _hydrationTimer?.Stop();
            _breakTimer?.Stop();
        }

        public void ResetTimers()
        {
            _hydrationTimer?.Stop();
            _breakTimer?.Stop();
            
            // Set initial intervals
            if (_hydrationTimer != null)
                _hydrationTimer.Interval = _settings.HydrationIntervalMinutes * 60000;
            
            if (_breakTimer != null)
                _breakTimer.Interval = _settings.BreakIntervalMinutes * 60000;
        }

        public void SnoozeHydration()
        {
            _hydrationTimer?.Stop();
            if (_hydrationTimer != null)
            {
                _hydrationTimer.Interval = _settings.HydrationSnoozeMinutes * 60000;
                _hydrationTimer.Start();
            }
        }

        public void SnoozeBreak()
        {
            _breakTimer?.Stop();
            if (_breakTimer != null)
            {
                _breakTimer.Interval = _settings.BreakSnoozeMinutes * 60000;
                _breakTimer.Start();
            }
        }

        public void UpdateSettings()
        {
            _settings = _settingsService.LoadSettings();
            _notificationService.UpdateSettings();
            ResetTimers();
        }

        public TimeSpan GetTimeUntilNextHydration()
        {
            if (_hydrationTimer == null)
                return TimeSpan.Zero;
            
            return TimeSpan.FromMilliseconds(_hydrationTimer.Interval);
        }

        public TimeSpan GetTimeUntilNextBreak()
        {
            if (_breakTimer == null)
                return TimeSpan.Zero;
            
            return TimeSpan.FromMilliseconds(_breakTimer.Interval);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _hydrationTimer?.Dispose();
                    _breakTimer?.Dispose();
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

    public class ReminderEventArgs : EventArgs
    {
        public int SnoozeMinutes { get; set; }
        public int DurationMinutes { get; set; }
        public string ReminderType { get; set; } = string.Empty;
    }
}
