using INPUTLAGFIX.Views;
using System.Collections.ObjectModel;
using Microsoft.Win32.TaskScheduler;

namespace INPUTLAGFIX.Models
{
    public class Optimization
    {
        public string ruName { get; set; }
        public bool CheckedState { get; set; }

        public bool AddWindow { get; set; }
        public ObservableCollection<Setting> settings { get; set; }
        public void ApplyOptimization(ref RegeditManager regeditManager, bool isBackup = false)
        {
            foreach (Setting setting in settings)
            {
                try
                {
                    if (CheckedState == true || isBackup)
                    {
                        if (!setting.isTask)
                        {
                            if (setting.value != "delete")
                                Logger.GetLogger().AllLogMessages.Add(regeditManager.ChangeRegistryValue(setting.valuePath, setting.valueName, setting.value, setting.valueKind));
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
                                            Logger.GetLogger().AllLogMessages.Add($"Задача {taskPath} была остановлена");
                                        }
                                        catch
                                        {
                                            Logger.GetLogger().AllLogMessages.Add($"Задачу {taskPath} не удалось остановить");
                                        }
                                    }

                                }
                                else
                                {
                                    Logger.GetLogger().AllLogMessages.Add($"Задача {taskPath} не найдена.");
                                }
                            }

                        }
                    }
                }
                catch
                {
                    if (CheckedState == true || isBackup)
                    {
                        if (setting.value!="delete")
                            Logger.GetLogger().AllLogMessages.Add(PowerRunManager.ApplyRegSettingWithPowerRun(setting.valuePath, setting.valueName, setting.value, setting.valueKind));
                    }
                }
            }
            if (AddWindow && CheckedState)
            {
                var infoWindow = new Information();
                infoWindow.Show();
            }
        }
    }

}
