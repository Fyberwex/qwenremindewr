using System.IO;
using Newtonsoft.Json;
using BreakReminderApp.Models;

namespace BreakReminderApp.Services
{
    /// <summary>
    /// Settings persistence service using JSON in AppData
    /// </summary>
    public class SettingsService
    {
        private readonly string _settingsPath;
        private readonly string _waterDataPath;
        private AppSettings? _cachedSettings;

        public SettingsService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BreakReminderApp"
            );
            
            Directory.CreateDirectory(appDataPath);
            
            _settingsPath = Path.Combine(appDataPath, "settings.json");
            _waterDataPath = Path.Combine(appDataPath, "water_data.json");
        }

        /// <summary>
        /// Load settings from file or return defaults
        /// </summary>
        public AppSettings LoadSettings()
        {
            if (_cachedSettings != null)
                return _cachedSettings;

            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    _cachedSettings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                    ValidateSettings(_cachedSettings);
                    return _cachedSettings;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            _cachedSettings = new AppSettings();
            return _cachedSettings;
        }

        /// <summary>
        /// Save settings to file
        /// </summary>
        public void SaveSettings(AppSettings settings)
        {
            try
            {
                _cachedSettings = settings;
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Ensure settings are within valid ranges
        /// </summary>
        private void ValidateSettings(AppSettings settings)
        {
            settings.HydrationIntervalMinutes = Math.Clamp(
                settings.HydrationIntervalMinutes,
                AppSettings.Ranges.HydrationMin,
                AppSettings.Ranges.HydrationMax
            );
            
            settings.BreakIntervalMinutes = Math.Clamp(
                settings.BreakIntervalMinutes,
                AppSettings.Ranges.BreakMin,
                AppSettings.Ranges.BreakMax
            );
            
            settings.BreakDurationMinutes = Math.Clamp(
                settings.BreakDurationMinutes,
                AppSettings.Ranges.BreakDurationMin,
                AppSettings.Ranges.BreakDurationMax
            );
            
            settings.DailyWaterGoalGlasses = Math.Clamp(
                settings.DailyWaterGoalGlasses,
                AppSettings.Ranges.WaterGoalMin,
                AppSettings.Ranges.WaterGoalMax
            );
        }

        /// <summary>
        /// Load water intake data
        /// </summary>
        public List<DailyWaterSummary> LoadWaterData()
        {
            try
            {
                if (File.Exists(_waterDataPath))
                {
                    var json = File.ReadAllText(_waterDataPath);
                    return JsonConvert.DeserializeObject<List<DailyWaterSummary>>(json) ?? new List<DailyWaterSummary>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading water data: {ex.Message}");
            }

            return new List<DailyWaterSummary>();
        }

        /// <summary>
        /// Save water intake data
        /// </summary>
        public void SaveWaterData(List<DailyWaterSummary> data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(_waterDataPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving water data: {ex.Message}");
            }
        }

        /// <summary>
        /// Export water data as CSV
        /// </summary>
        public string ExportToCsv(List<DailyWaterSummary> data)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Date,Total Glasses,Goal Glasses,Completion %,Goal Achieved");
            
            foreach (var day in data.OrderByDescending(d => d.Date))
            {
                sb.AppendLine($"{day.Date:yyyy-MM-dd},{day.TotalGlasses},{day.GoalGlasses}," +
                             $"{day.CompletionPercentage:F1},{day.GoalAchieved}");
            }
            
            return sb.ToString();
        }
    }
}
