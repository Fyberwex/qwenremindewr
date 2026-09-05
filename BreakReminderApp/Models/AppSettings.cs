namespace BreakReminderApp.Models
{
    /// <summary>
    /// Application settings model with default values as specified
    /// </summary>
    public class AppSettings
    {
        // Hydration Settings
        public int HydrationIntervalMinutes { get; set; } = 25;
        public int DailyWaterGoalGlasses { get; set; } = 8;
        public int GlassSizeMl { get; set; } = 250;
        
        // Break Settings
        public int BreakIntervalMinutes { get; set; } = 60;
        public int BreakDurationMinutes { get; set; } = 5;
        
        // Schedule Settings
        public TimeSpan ActiveHoursStart { get; set; } = TimeSpan.FromHours(9); // 9:00 AM
        public TimeSpan ActiveHoursEnd { get; set; } = TimeSpan.FromHours(18); // 6:00 PM
        
        // Snooze Settings
        public int HydrationSnoozeMinutes { get; set; } = 5;
        public int BreakSnoozeMinutes { get; set; } = 10;
        
        // Notification Settings
        public string NotificationStyle { get; set; } = "Toast"; // Toast, Popup, Sound, Silent
        public bool EnableSound { get; set; } = true;
        public bool StartWithWindows { get; set; } = false;
        
        // UI Settings
        public string Theme { get; set; } = "Light"; // Light, Dark
        public string AccentColor { get; set; } = "#4A90D9";
        public double FontSize { get; set; } = 14;
        
        // Smart Features
        public bool EnableIdleDetection { get; set; } = true;
        public bool EnableFullscreenDetection { get; set; } = true;
        public bool EnableCalendarIntegration { get; set; } = false;
        public bool EnableAdaptiveTiming { get; set; } = false;
        
        // Validation ranges
        public static class Ranges
        {
            public const int HydrationMin = 15;
            public const int HydrationMax = 120;
            public const int BreakMin = 30;
            public const int BreakMax = 180;
            public const int BreakDurationMin = 1;
            public const int BreakDurationMax = 15;
            public const int WaterGoalMin = 1;
            public const int WaterGoalMax = 20;
        }
    }
}
