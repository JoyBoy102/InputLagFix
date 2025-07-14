using INPUTLAGFIX.Views;
using Microsoft.Win32.TaskScheduler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace INPUTLAGFIX.Models
{
    public class Optimization:INotifyPropertyChanged
    {
        public string ruName { get; set; }
        public bool CheckedState { get; set; }

        public bool AddWindow { get; set; }
        private ObservableCollection<Setting> _settings { get; set; }

        public ObservableCollection<Setting> settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visibility = Visibility.Collapsed;

        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }

        public bool State {  get; set; }
        public void ApplyOptimization(RegeditManager regeditManager, bool isBackup = false)
        {
            foreach (Setting setting in _settings)
            {
                try
                {
                    if (CheckedState == true || isBackup)
                    {
                        if (!setting.isTask)
                        {
                            if (setting.value != "delete")
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Logger.GetLogger().AllLogMessages.Add(regeditManager.ChangeRegistryValue(setting.valuePath, setting.valueName, setting.value, setting.valueKind));
                                });
                                
                            }
                        }
                        else
                        {
                            using (TaskService taskService = new TaskService())
                            {
                                string taskPath = setting.valuePath.Replace("Microsoft\\Windows", "\\Microsoft\\Windows");
                                Microsoft.Win32.TaskScheduler.Task task = taskService.GetTask(taskPath);
                                if (task != null)
                                {
                                    if (task.Enabled)
                                    {
                                        try
                                        {
                                            task.Enabled = false;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                Logger.GetLogger().AllLogMessages.Add($"Задача {taskPath} была остановлена");
                                            });
                                            
                                        }
                                        catch
                                        {
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                Logger.GetLogger().AllLogMessages.Add($"Задачу {taskPath} не удалось остановить");
                                            });
                                        }
                                    }

                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        Logger.GetLogger().AllLogMessages.Add($"Задача {taskPath} не найдена.");
                                    });
                                }
                            }

                        }
                    }
                }
                catch
                {
                    if (CheckedState == true || isBackup)
                    {

                        if (setting.value != "delete")
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Logger.GetLogger().AllLogMessages.Add(PowerRunManager.ApplyRegSettingWithPowerRun(setting.valuePath, setting.valueName, setting.value, setting.valueKind));
                            });
                        }
                    }
                }
                Thread.Sleep(250);
            }
            if (AddWindow && CheckedState)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var infoWindow = new Information();
                    infoWindow.Show();
                });
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
