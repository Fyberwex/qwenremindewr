namespace BreakReminderApp.Models
{
    /// <summary>
    /// Water intake tracking model
    /// </summary>
    public class WaterIntakeEntry
    {
        public DateTime Timestamp { get; set; }
        public int Glasses { get; set; } = 1;
        public int VolumeMl { get; set; } = 250;
    }

    /// <summary>
    /// Daily water tracking summary
    /// </summary>
    public class DailyWaterSummary
    {
        public DateTime Date { get; set; }
        public int TotalGlasses { get; set; }
        public int GoalGlasses { get; set; }
        public List<WaterIntakeEntry> Entries { get; set; } = new();
        
        public double CompletionPercentage => GoalGlasses > 0 ? 
            Math.Min(100, (double)TotalGlasses / GoalGlasses * 100) : 0;
            
        public bool GoalAchieved => TotalGlasses >= GoalGlasses;
    }

    /// <summary>
    /// Weekly summary for reporting
    /// </summary>
    public class WeeklyWaterSummary
    {
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public List<DailyWaterSummary> Days { get; set; } = new();
        
        public int TotalGlasses => Days.Sum(d => d.TotalGlasses);
        public int AverageGlassesPerDay => Days.Any() ? TotalGlasses / Days.Count : 0;
        public int DaysGoalAchieved => Days.Count(d => d.GoalAchieved);
        public double StreakDays { get; set; } // Consecutive days meeting goal
    }
}
