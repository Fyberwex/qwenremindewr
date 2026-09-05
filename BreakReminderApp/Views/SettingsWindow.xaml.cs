using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BreakReminderApp.Models;
using BreakReminderApp.Services;

namespace BreakReminderApp.Views
{
    /// <summary>
    /// Settings window with tabbed interface
    /// Clean, modern design with dark/light mode support
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly SettingsService _settingsService;
        private readonly NotificationService _notificationService;
        private readonly ReminderTimerService _timerService;
        private readonly WaterTrackingService _waterTrackingService;
        private AppSettings _settings;

        public SettingsWindow(
            SettingsService settingsService,
            NotificationService notificationService,
            ReminderTimerService timerService,
            WaterTrackingService waterTrackingService)
        {
            _settingsService = settingsService;
            _notificationService = notificationService;
            _timerService = timerService;
            _waterTrackingService = waterTrackingService;
            _settings = settingsService.LoadSettings();

            InitializeComponent();
            InitializeUi();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            Width = 650;
            Height = 550;
            Title = "Break Reminder - Settings";
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.CanMinimize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // Apply theme
            ApplyTheme();
        }

        private void InitializeUi()
        {
            var mainGrid = new Grid();
            mainGrid.Margin = new Thickness(20);

            var rowDef1 = new RowDefinition { Height = GridLength.Auto };
            var rowDef2 = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            var rowDef3 = new RowDefinition { Height = GridLength.Auto };
            mainGrid.RowDefinitions.Add(rowDef1);
            mainGrid.RowDefinitions.Add(rowDef2);
            mainGrid.RowDefinitions.Add(rowDef3);

            // Header
            var headerText = new TextBlock
            {
                Text = "⚙️ Settings",
                FontSize = 24,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 20)
            };
            Grid.SetRow(headerText, 0);
            mainGrid.Children.Add(headerText);

            // Tab Control
            var tabControl = new TabControl
            {
                Name = "MainTabControl",
                FontSize = 14
            };

            // Hydration Tab
            tabControl.Items.Add(CreateHydrationTab());
            
            // Break Tab
            tabControl.Items.Add(CreateBreakTab());
            
            // Schedule Tab
            tabControl.Items.Add(CreateScheduleTab());
            
            // Notifications Tab
            tabControl.Items.Add(CreateNotificationsTab());
            
            // Water Tracking Tab
            tabControl.Items.Add(CreateWaterTrackingTab());

            Grid.SetRow(tabControl, 1);
            mainGrid.Children.Add(tabControl);

            // Footer buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var saveButton = new Button
            {
                Content = "💾 Save Settings",
                Padding = new Thickness(20, 8, 20, 8),
                Margin = new Thickness(0, 0, 10, 0)
            };
            saveButton.Click += SaveButtonClick;

            var cancelButton = new Button
            {
                Content = "❌ Cancel",
                Padding = new Thickness(20, 8, 20, 8),
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(128, 128, 128))
            };
            cancelButton.Click += (s, e) => Close();

            buttonPanel.Children.Add(saveButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(buttonPanel, 2);
            mainGrid.Children.Add(buttonPanel);

            Content = mainGrid;
        }

        private TabItem CreateHydrationTab()
        {
            var grid = new Grid();
            grid.Margin = new Thickness(20);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int row = 0;

            // Hydration Interval
            grid.Children.Add(CreateSettingRow(
                "Hydration Reminder (minutes):",
                CreateNumericUpDown(AppSettings.Ranges.HydrationMin, AppSettings.Ranges.HydrationMax, _settings.HydrationIntervalMinutes, "HydrationInterval"),
                ref row,
                grid
            ));

            // Daily Goal
            grid.Children.Add(CreateSettingRow(
                "Daily Water Goal (glasses):",
                CreateNumericUpDown(AppSettings.Ranges.WaterGoalMin, AppSettings.Ranges.WaterGoalMax, _settings.DailyWaterGoalGlasses, "DailyGoal"),
                ref row,
                grid
            ));

            // Glass Size
            grid.Children.Add(CreateSettingRow(
                "Glass Size (ml):",
                CreateNumericUpDown(100, 500, _settings.GlassSizeMl, "GlassSize"),
                ref row,
                grid
            ));

            // Snooze Duration
            grid.Children.Add(CreateSettingRow(
                "Snooze Duration (minutes):",
                CreateNumericUpDown(1, 30, _settings.HydrationSnoozeMinutes, "HydrationSnooze"),
                ref row,
                grid
            ));

            return new TabItem
            {
                Header = "💧 Hydration",
                Content = new ScrollViewer { Content = grid }
            };
        }

        private TabItem CreateBreakTab()
        {
            var grid = new Grid();
            grid.Margin = new Thickness(20);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int row = 0;

            // Break Interval
            grid.Children.Add(CreateSettingRow(
                "Screen Break Interval (minutes):",
                CreateNumericUpDown(AppSettings.Ranges.BreakMin, AppSettings.Ranges.BreakMax, _settings.BreakIntervalMinutes, "BreakInterval"),
                ref row,
                grid
            ));

            // Break Duration
            grid.Children.Add(CreateSettingRow(
                "Break Duration (minutes):",
                CreateNumericUpDown(AppSettings.Ranges.BreakDurationMin, AppSettings.Ranges.BreakDurationMax, _settings.BreakDurationMinutes, "BreakDuration"),
                ref row,
                grid
            ));

            // Break Snooze
            grid.Children.Add(CreateSettingRow(
                "Break Snooze (minutes):",
                CreateNumericUpDown(1, 30, _settings.BreakSnoozeMinutes, "BreakSnooze"),
                ref row,
                grid
            ));

            return new TabItem
            {
                Header = "👁️ Screen Break",
                Content = new ScrollViewer { Content = grid }
            };
        }

        private TabItem CreateScheduleTab()
        {
            var grid = new Grid();
            grid.Margin = new Thickness(20);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int row = 0;

            // Active Hours Start
            grid.Children.Add(CreateSettingRow(
                "Active Hours Start:",
                CreateTimePicker(_settings.ActiveHoursStart, "ActiveHoursStart"),
                ref row,
                grid
            ));

            // Active Hours End
            grid.Children.Add(CreateSettingRow(
                "Active Hours End:",
                CreateTimePicker(_settings.ActiveHoursEnd, "ActiveHoursEnd"),
                ref row,
                grid
            ));

            // Start with Windows
            var startWithWindowsCheckbox = new CheckBox
            {
                Content = "Start with Windows",
                IsChecked = _settings.StartWithWindows,
                Name = "StartWithWindows",
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(startWithWindowsCheckbox, row++);
            Grid.SetColumn(startWithWindowsCheckbox, 1);
            grid.Children.Add(startWithWindowsCheckbox);

            return new TabItem
            {
                Header = "📅 Schedule",
                Content = new ScrollViewer { Content = grid }
            };
        }

        private TabItem CreateNotificationsTab()
        {
            var grid = new Grid();
            grid.Margin = new Thickness(20);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int row = 0;

            // Notification Style
            var styleCombo = new ComboBox
            {
                Name = "NotificationStyle",
                Width = 200,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            styleCombo.Items.Add("Toast");
            styleCombo.Items.Add("Popup");
            styleCombo.Items.Add("Sound");
            styleCombo.Items.Add("Silent");
            styleCombo.SelectedItem = _settings.NotificationStyle;
            grid.Children.Add(styleCombo);
            Grid.SetRow(styleCombo, row);
            Grid.SetColumn(styleCombo, 1);

            var styleLabel = new TextBlock
            {
                Text = "Notification Style:",
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(styleLabel, row);
            grid.Children.Add(styleLabel);
            row++;

            // Enable Sound
            var soundCheckbox = new CheckBox
            {
                Content = "Enable Sound Alerts",
                IsChecked = _settings.EnableSound,
                Name = "EnableSound",
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(soundCheckbox, row++);
            Grid.SetColumn(soundCheckbox, 1);
            grid.Children.Add(soundCheckbox);

            // Smart Features
            var idleCheckbox = new CheckBox
            {
                Content = "Pause when system is idle",
                IsChecked = _settings.EnableIdleDetection,
                Name = "EnableIdleDetection",
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(idleCheckbox, row++);
            Grid.SetColumn(idleCheckbox, 1);
            grid.Children.Add(idleCheckbox);

            var fullscreenCheckbox = new CheckBox
            {
                Content = "Don't interrupt fullscreen apps",
                IsChecked = _settings.EnableFullscreenDetection,
                Name = "EnableFullscreenDetection",
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(fullscreenCheckbox, row++);
            Grid.SetColumn(fullscreenCheckbox, 1);
            grid.Children.Add(fullscreenCheckbox);

            // Theme
            var themeCombo = new ComboBox
            {
                Name = "Theme",
                Width = 200,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            themeCombo.Items.Add("Light");
            themeCombo.Items.Add("Dark");
            themeCombo.SelectedItem = _settings.Theme;
            grid.Children.Add(themeCombo);
            Grid.SetRow(themeCombo, row);
            Grid.SetColumn(themeCombo, 1);

            var themeLabel = new TextBlock
            {
                Text = "Theme:",
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(themeLabel, row);
            grid.Children.Add(themeLabel);

            return new TabItem
            {
                Header = "🔔 Notifications",
                Content = new ScrollViewer { Content = grid }
            };
        }

        private TabItem CreateWaterTrackingTab()
        {
            var grid = new Grid();
            grid.Margin = new Thickness(20);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Today's Progress
            var todaySummary = _waterTrackingService.GetTodaySummary();
            var progressText = new TextBlock
            {
                Text = $"Today: {todaySummary.TotalGlasses}/{todaySummary.GoalGlasses} glasses ({todaySummary.CompletionPercentage:F0}%)",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 20)
            };
            Grid.SetRow(progressText, 0);
            grid.Children.Add(progressText);

            // Weekly Summary
            var weeklySummary = _waterTrackingService.GetWeeklySummary();
            var weeklyText = new TextBlock
            {
                Text = $"This Week: {weeklySummary.TotalGlasses} glasses | " +
                       $"Avg/Day: {weeklySummary.AverageGlassesPerDay} | " +
                       $"Days on Goal: {weeklySummary.DaysGoalAchieved} | " +
                       $"Streak: {weeklySummary.StreakDays} days",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(weeklyText, 1);
            grid.Children.Add(weeklyText);

            // Export Button
            var exportButton = new Button
            {
                Content = "📊 Export to CSV",
                Padding = new Thickness(15, 8, 15, 8),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            exportButton.Click += ExportCsvClick;
            Grid.SetRow(exportButton, 2);
            grid.Children.Add(exportButton);

            return new TabItem
            {
                Header = "📈 Statistics",
                Content = new ScrollViewer { Content = grid }
            };
        }

        private UIElement CreateSettingRow(string label, UIElement control, ref int row, Grid grid)
        {
            var panel = new Grid();
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            panel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var labelBlock = new TextBlock
            {
                Text = label,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 20, 5)
            };
            Grid.SetColumn(labelBlock, 0);
            panel.Children.Add(labelBlock);

            Grid.SetColumn(control, 1);
            Grid.SetRow(control, 0);
            panel.Children.Add(control);

            Grid.SetRow(panel, row++);
            return panel;
        }

        private UIElement CreateNumericUpDown(int min, int max, int value, string name)
        {
            var textBox = new TextBox
            {
                Width = 80,
                Text = value.ToString(),
                Tag = $"{min},{max}",
                Name = name
            };
            textBox.PreviewTextInput += (s, e) =>
            {
                e.Handled = !char.IsDigit(e.Text, 0);
            };

            return textBox;
        }

        private UIElement CreateTimePicker(TimeSpan time, string name)
        {
            var comboBox = new ComboBox
            {
                Width = 120,
                Name = name,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute += 15)
                {
                    var timeValue = new TimeSpan(hour, minute, 0);
                    var displayText = timeValue.ToString(@"hh\:mm");
                    comboBox.Items.Add(new ComboBoxItem
                    {
                        Content = displayText,
                        Tag = timeValue
                    });
                    
                    if (timeValue == time)
                        comboBox.SelectedItem = comboBox.Items[comboBox.Items.Count - 1];
                }
            }

            return comboBox;
        }

        private void LoadSettings()
        {
            // Settings are loaded in constructor and used to initialize UI controls
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            // Collect settings from UI
            CollectSettingsFromUi();
            
            // Save to file
            _settingsService.SaveSettings(_settings);
            
            // Update services
            _timerService.UpdateSettings();
            _notificationService.UpdateSettings();
            _waterTrackingService.UpdateSettings();

            MessageBox.Show("Settings saved successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CollectSettingsFromUi()
        {
            var mainGrid = Content as Grid;
            if (mainGrid == null) return;

            var tabControl = mainGrid.Children.OfType<TabControl>().FirstOrDefault();
            if (tabControl == null) return;

            // Hydration Tab
            var hydrationTab = tabControl.Items.OfType<TabItem>().FirstOrDefault(t => t.Header?.ToString()?.Contains("Hydration") == true);
            if (hydrationTab != null)
            {
                var hydrationInterval = FindTextBoxByName(hydrationTab.Content, "HydrationInterval");
                if (hydrationInterval != null && int.TryParse(hydrationInterval.Text, out var interval))
                    _settings.HydrationIntervalMinutes = Math.Clamp(interval, AppSettings.Ranges.HydrationMin, AppSettings.Ranges.HydrationMax);

                var dailyGoal = FindTextBoxByName(hydrationTab.Content, "DailyGoal");
                if (dailyGoal != null && int.TryParse(dailyGoal.Text, out var goal))
                    _settings.DailyWaterGoalGlasses = Math.Clamp(goal, AppSettings.Ranges.WaterGoalMin, AppSettings.Ranges.WaterGoalMax);

                var glassSize = FindTextBoxByName(hydrationTab.Content, "GlassSize");
                if (glassSize != null && int.TryParse(glassSize.Text, out var size))
                    _settings.GlassSizeMl = size;

                var hydrationSnooze = FindTextBoxByName(hydrationTab.Content, "HydrationSnooze");
                if (hydrationSnooze != null && int.TryParse(hydrationSnooze.Text, out var snooze))
                    _settings.HydrationSnoozeMinutes = snooze;
            }

            // Break Tab
            var breakTab = tabControl.Items.OfType<TabItem>().FirstOrDefault(t => t.Header?.ToString()?.Contains("Break") == true);
            if (breakTab != null)
            {
                var breakInterval = FindTextBoxByName(breakTab.Content, "BreakInterval");
                if (breakInterval != null && int.TryParse(breakInterval.Text, out var interval))
                    _settings.BreakIntervalMinutes = Math.Clamp(interval, AppSettings.Ranges.BreakMin, AppSettings.Ranges.BreakMax);

                var breakDuration = FindTextBoxByName(breakTab.Content, "BreakDuration");
                if (breakDuration != null && int.TryParse(breakDuration.Text, out var duration))
                    _settings.BreakDurationMinutes = Math.Clamp(duration, AppSettings.Ranges.BreakDurationMin, AppSettings.Ranges.BreakDurationMax);

                var breakSnooze = FindTextBoxByName(breakTab.Content, "BreakSnooze");
                if (breakSnooze != null && int.TryParse(breakSnooze.Text, out var snooze))
                    _settings.BreakSnoozeMinutes = snooze;
            }

            // Schedule Tab
            var scheduleTab = tabControl.Items.OfType<TabItem>().FirstOrDefault(t => t.Header?.ToString()?.Contains("Schedule") == true);
            if (scheduleTab != null)
            {
                var activeStart = FindComboBoxByName(scheduleTab.Content, "ActiveHoursStart");
                if (activeStart?.SelectedItem is ComboBoxItem startItem && startItem.Tag is TimeSpan start)
                    _settings.ActiveHoursStart = start;

                var activeEnd = FindComboBoxByName(scheduleTab.Content, "ActiveHoursEnd");
                if (activeEnd?.SelectedItem is ComboBoxItem endItem && endItem.Tag is TimeSpan end)
                    _settings.ActiveHoursEnd = end;

                var startWithWindows = FindCheckBoxByName(scheduleTab.Content, "StartWithWindows");
                if (startWithWindows != null)
                    _settings.StartWithWindows = startWithWindows.IsChecked ?? false;
            }

            // Notifications Tab
            var notificationsTab = tabControl.Items.OfType<TabItem>().FirstOrDefault(t => t.Header?.ToString()?.Contains("Notifications") == true);
            if (notificationsTab != null)
            {
                var styleCombo = FindComboBoxByName(notificationsTab.Content, "NotificationStyle");
                if (styleCombo?.SelectedItem != null)
                    _settings.NotificationStyle = styleCombo.SelectedItem.ToString() ?? "Toast";

                var soundCheck = FindCheckBoxByName(notificationsTab.Content, "EnableSound");
                if (soundCheck != null)
                    _settings.EnableSound = soundCheck.IsChecked ?? false;

                var idleCheck = FindCheckBoxByName(notificationsTab.Content, "EnableIdleDetection");
                if (idleCheck != null)
                    _settings.EnableIdleDetection = idleCheck.IsChecked ?? false;

                var fullscreenCheck = FindCheckBoxByName(notificationsTab.Content, "EnableFullscreenDetection");
                if (fullscreenCheck != null)
                    _settings.EnableFullscreenDetection = fullscreenCheck.IsChecked ?? false;

                var themeCombo = FindComboBoxByName(notificationsTab.Content, "Theme");
                if (themeCombo?.SelectedItem != null)
                    _settings.Theme = themeCombo.SelectedItem.ToString() ?? "Light";
            }
        }

        private TextBox? FindTextBoxByName(object content, string name)
        {
            if (content is Grid grid)
                return FindVisualChildByName<TextBox>(grid, name);
            return null;
        }

        private ComboBox? FindComboBoxByName(object content, string name)
        {
            if (content is Grid grid)
                return FindVisualChildByName<ComboBox>(grid, name);
            return null;
        }

        private CheckBox? FindCheckBoxByName(object content, string name)
        {
            if (content is Grid grid)
                return FindVisualChildByName<CheckBox>(grid, name);
            return null;
        }

        private T? FindVisualChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T element && element.Name == name)
                    return element;

                var result = FindVisualChildByName<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void ExportCsvClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = ".csv",
                FileName = $"water_tracking_{DateTime.Today:yyyy-MM-dd}.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                _waterTrackingService.SaveCsvExport(dialog.FileName);
                MessageBox.Show($"Data exported to {dialog.FileName}", "Export Complete",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ApplyTheme()
        {
            if (_settings.Theme == "Dark")
            {
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(30, 30, 30));
                Foreground = Brushes.White;
            }
            else
            {
                Background = Brushes.White;
                Foreground = Brushes.Black;
            }
        }
    }
}
