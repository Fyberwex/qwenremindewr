using System.Runtime.InteropServices;
using BreakReminderApp.Models;

namespace BreakReminderApp.Services
{
    /// <summary>
    /// Manages water intake tracking and statistics
    /// </summary>
    public class WaterTrackingService : IDisposable
    {
        private readonly SettingsService _settingsService;
        private List<DailyWaterSummary> _waterData;
        private AppSettings _settings;
        private bool _disposed;

        public event EventHandler<DailyWaterSummary>? OnDailyGoalAchieved;
        public event EventHandler? OnWaterIntakeAdded;

        public WaterTrackingService(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _settings = settingsService.LoadSettings();
            _waterData = settingsService.LoadWaterData();
            
            // Clean up old data (keep last 90 days)
            CleanupOldData();
        }

        /// <summary>
        /// Add a glass of water to today's tracking
        /// </summary>
        public void AddWaterIntake(int glasses = 1)
        {
            var today = DateTime.Today;
            var dailySummary = GetOrCreateDailySummary(today);
            
            var entry = new WaterIntakeEntry
            {
                Timestamp = DateTime.Now,
                Glasses = glasses,
                VolumeMl = glasses * _settings.GlassSizeMl
            };
            
            dailySummary.Entries.Add(entry);
            dailySummary.TotalGlasses += glasses;
            
            _settingsService.SaveWaterData(_waterData);
            
            OnWaterIntakeAdded?.Invoke(this, EventArgs.Empty);
            
            // Check if goal achieved
            if (dailySummary.TotalGlasses >= _settings.DailyWaterGoalGlasses && 
                dailySummary.TotalGlasses - glasses < _settings.DailyWaterGoalGlasses)
            {
                OnDailyGoalAchieved?.Invoke(this, dailySummary);
            }
        }

        /// <summary>
        /// Get today's water summary
        /// </summary>
        public DailyWaterSummary GetTodaySummary()
        {
            return GetOrCreateDailySummary(DateTime.Today);
        }

        /// <summary>
        /// Get weekly summary
        /// </summary>
        public WeeklyWaterSummary GetWeeklySummary()
        {
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var weekEnd = weekStart.AddDays(6);
            
            var weekData = _waterData
                .Where(d => d.Date >= weekStart && d.Date <= weekEnd)
                .OrderBy(d => d.Date)
                .ToList();
            
            var summary = new WeeklyWaterSummary
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                Days = weekData
            };
            
            // Calculate streak
            summary.StreakDays = CalculateStreak();
            
            return summary;
        }

        private double CalculateStreak()
        {
            double streak = 0;
            var currentDate = DateTime.Today;
            
            while (true)
            {
                var dayData = _waterData.FirstOrDefault(d => d.Date == currentDate);
                if (dayData == null || !dayData.GoalAchieved)
                    break;
                    
                streak++;
                currentDate = currentDate.AddDays(-1);
                
                // Limit to prevent infinite loops
                if (streak > 365)
                    break;
            }
            
            return streak;
        }

        private DailyWaterSummary GetOrCreateDailySummary(DateTime date)
        {
            var existing = _waterData.FirstOrDefault(d => d.Date.Date == date.Date);
            if (existing != null)
                return existing;
            
            var newSummary = new DailyWaterSummary
            {
                Date = date.Date,
                GoalGlasses = _settings.DailyWaterGoalGlasses,
                TotalGlasses = 0,
                Entries = new List<WaterIntakeEntry>()
            };
            
            _waterData.Add(newSummary);
            return newSummary;
        }

        /// <summary>
        /// Reset today's data (for testing/correction)
        /// </summary>
        public void ResetTodayData()
        {
            var today = DateTime.Today;
            var existing = _waterData.FirstOrDefault(d => d.Date.Date == today.Date);
            if (existing != null)
            {
                _waterData.Remove(existing);
                _settingsService.SaveWaterData(_waterData);
            }
        }

        /// <summary>
        /// Export data to CSV file
        /// </summary>
        public string ExportToCsv()
        {
            return _settingsService.ExportToCsv(_waterData);
        }

        /// <summary>
        /// Save CSV to file
        /// </summary>
        public void SaveCsvExport(string filePath)
        {
            var csvContent = ExportToCsv();
            File.WriteAllText(filePath, csvContent);
        }

        private void CleanupOldData()
        {
            var cutoffDate = DateTime.Today.AddDays(-90);
            var oldEntries = _waterData.Where(d => d.Date < cutoffDate).ToList();
            
            foreach (var old in oldEntries)
            {
                _waterData.Remove(old);
            }
            
            if (oldEntries.Any())
            {
                _settingsService.SaveWaterData(_waterData);
            }
        }

        public void UpdateSettings()
        {
            _settings = _settingsService.LoadSettings();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clean up managed resources
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
}
