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
        public void ApplyOptimization(ref RegeditManager regeditManager, ref AutoRunsModel autoRunsModel)
        {
            foreach (Setting setting in settings)
            {
                try
                {
                    if (setting.value_if_false.ToString() != "delete")
                    {
                        if (CheckedState == true)
                        {
                            if (!setting.isTask)
                                Logger.GetLogger().AllLogMessages.Add(regeditManager.ChangeRegistryValue(setting.valuePath, setting.valueName, setting.value_if_true, setting.valueKind));
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
                    else
                    {
                        if (CheckedState == true)
                        {
                            Logger.GetLogger().AllLogMessages.Add(regeditManager.ChangeRegistryValue(setting.valuePath, setting.valueName, setting.value_if_true, setting.valueKind));
                        }
                    }
                }
                catch
                {
                    if (setting.value_if_false.ToString() != "delete")
                    {
                        if (CheckedState == true)
                            Logger.GetLogger().AllLogMessages.Add(PowerRunManager.ApplyRegSettingWithPowerRun(setting.valuePath, setting.valueName, setting.value_if_true, setting.valueKind));
                    }
                    else
                    {
                        if (CheckedState == true)
                            Logger.GetLogger().AllLogMessages.Add(PowerRunManager.ApplyRegSettingWithPowerRun(setting.valuePath, setting.valueName, setting.value_if_true, setting.valueKind));
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
