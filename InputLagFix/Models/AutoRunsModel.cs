using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Xml;
using System.Xml.Serialization;
using Windows.ApplicationModel.Appointments;

namespace INPUTLAGFIX.Models
{
    public class AutoRunsModel:BackuperFromXml
    {
        private string _registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private string _registryPathServices = @"SYSTEM\CurrentControlSet\Services";
        private RegeditManager _regeditManager;
        private XmlManager _xmlManager;
        private Dictionary<RegistryKey, string> RegKeysInString = new Dictionary<RegistryKey, string>()
        {
            { Registry.LocalMachine, "HKEY_LOCAL_MACHINE"},
            { Registry.CurrentUser, "HKEY_CURRENT_USER"}
        };

        [XmlArray("RegeditItems")]
        [XmlArrayItem("RegeditItem")]
        public ObservableCollection<AutoRunsItem> AutoRunsItemsRegedit;

        [XmlArray("TasksItems")]
        [XmlArrayItem("TaskItem")]
        public ObservableCollection<AutoRunsItem> AutoRunsItemsTasks;

        [XmlArray("ServicesItems")]
        [XmlArrayItem("ServiceItem")]
        public ObservableCollection<AutoRunsItem> AutoRunsItemsServices;
        public AutoRunsModel()
        {
            _regeditManager = new RegeditManager();
            _xmlManager = new XmlManager();
            AutoRunsItemsRegedit = GetAllAutoRunsItemsRegedit();
            AutoRunsItemsTasks = GetAllAutoRunsItemsTasks();
            AutoRunsItemsServices = GetAllAutoRunsItemsServices();
        }
        private List<RegistryKey> allRegistryKeys = new List<RegistryKey>()
        {
            Registry.LocalMachine,
            Registry.CurrentUser
        };
        public ObservableCollection<AutoRunsItem> GetAllAutoRunsItemsRegedit()
        {
            var AutoRunsItemsRegedit = _xmlManager.GetSavedAutoRunsItemsRegedit();
            HashSet<string> UniqueDisplayNames = AutoRunsItemsRegedit.Select(x => x.DisplayName).ToHashSet();
            foreach (var key in allRegistryKeys)
            {
                var registryAutoRunsItems = key.OpenSubKey(_registryPath).GetValueNames().ToList();
                AutoRunsItemsRegedit.AddRange((registryAutoRunsItems
                                                .Select(x => new AutoRunsItem { DisplayName = x, SubKey = $"{RegKeysInString[key]}\\{_registryPath}", Type = "Regedit", State = true })
                                                .Where(x => !UniqueDisplayNames.Contains(x.DisplayName))).ToList());
                foreach (var name in AutoRunsItemsRegedit.Select(x => x.DisplayName))
                {
                    UniqueDisplayNames.Add(name);
                }
            }
            HashSet<AutoRunsItem> AutoRunsItemsRegeditHashSet = AutoRunsItemsRegedit.ToHashSet();

            return new ObservableCollection<AutoRunsItem>(AutoRunsItemsRegeditHashSet);
        }

        public ObservableCollection<AutoRunsItem> GetAllAutoRunsItemsTasks()
        {
            var AutoRunsItemsTasks = new List<AutoRunsItem>();
            using (TaskService ts = new TaskService())
            {

                foreach (Microsoft.Win32.TaskScheduler.Task task in ts.RootFolder.AllTasks)
                {

                    if (!task.Path.Contains("Windows"))
                        AutoRunsItemsTasks.Add(new AutoRunsItem { DisplayName = task.Name, Type = "Task", TaskPath = task.Path, State = (task.Enabled || task.State == Microsoft.Win32.TaskScheduler.TaskState.Running) });
                }
            }
            return new ObservableCollection<AutoRunsItem>(AutoRunsItemsTasks);
        }

        public ObservableCollection<AutoRunsItem> GetAllAutoRunsItemsServices()
        {
            var AutoRunsItemsServices = new List<AutoRunsItem>();
            RegistryKey key = Registry.LocalMachine.OpenSubKey(_registryPathServices);
            foreach (var subkeyname in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(subkeyname);
                var ImagePath = subkey.GetValue("ImagePath");
                string imgPathString = "";
                if (ImagePath != null)
                {
                    string[] imgPathStringParts = ImagePath.ToString().ToLower().Split('"');
                    if (imgPathStringParts.Length > 1)
                    {
                        imgPathString = imgPathStringParts[1];
                    }
                    else
                    {
                        imgPathString = imgPathStringParts[0];
                    }
                }
                if (!string.IsNullOrEmpty(imgPathString) && (!imgPathString.Contains("system") && !imgPathString.Contains(@"\windows\")))
                {
                    string start = subkey.GetValue("start").ToString();
                    bool state = (start == "2" || start == "3");
                    AutoRunsItemsServices.Add(new AutoRunsItem { DisplayName = subkeyname, ImagePath = imgPathString, Type = "Service", State = state, SubKey = $"HKEY_LOCAL_MACHINE\\{_registryPathServices}\\{subkeyname}" });
                }
            }
            return new ObservableCollection<AutoRunsItem>(AutoRunsItemsServices);
        }

        public void DeleteRegeditItem(AutoRunsItem item)
        {
            switch (item.Type)
            {
                case "Regedit":
                    if (!item.State)
                    {
                        string res = _regeditManager.DeleteKey(item.DisplayName, item.SubKey);
                        if (res == $"Ключ {item.DisplayName} удален из {item.SubKey}")
                        {
                            Logger.GetLogger().AllLogMessages.Add($"{item.DisplayName} удален из автозагрузок");
                        }
                        else
                        {
                            Logger.GetLogger().AllLogMessages.Add($"{item.DisplayName} не удалось удалить из автозагрузок");
                        }
                    }
                    else
                    {
                        string res = _regeditManager.ChangeRegistryValue(item.SubKey, item.DisplayName, "delete", RegistryValueKind.String);
                        if (res == $"Значение {item.DisplayName} в подразделе {item.SubKey} успешно изменено на delete")
                        {
                            Logger.GetLogger().AllLogMessages.Add($"{item.DisplayName} включен в автозагрузки");
                        }
                        else
                        {
                            Logger.GetLogger().AllLogMessages.Add($"{item.DisplayName} не удалось включить в автозагрузки");
                        }
                    }
                    break;
                case "Task":
                    Microsoft.Win32.TaskScheduler.Task itemTask = TaskService.Instance.GetTask(item.TaskPath);
                    if (itemTask.State == TaskState.Running)
                    {
                        itemTask.Stop();
                    }
                    if (itemTask.Enabled)
                    {
                        try
                        {
                            itemTask.Enabled = false;
                            Logger.GetLogger().AllLogMessages.Add($"Задача {item.DisplayName} была остановлена");
                        }
                        catch
                        {
                            Logger.GetLogger().AllLogMessages.Add($"Задачу {item.DisplayName} не удалось остановить");
                        }
                    }
                    else
                    {
                        try
                        {
                            itemTask.Enabled = true;
                            Logger.GetLogger().AllLogMessages.Add($"Задача {item.DisplayName} была запущена");
                        }
                        catch
                        {
                            Logger.GetLogger().AllLogMessages.Add($"Задачу {item.DisplayName} не удалось запустить");
                        }
                    }
                    break;
                case "Service":
                    if (!item.State)
                    {
                        try
                        {
                            _regeditManager.ChangeRegistryValue(item.SubKey, "Start", 4, RegistryValueKind.DWord);
                            Logger.GetLogger().AllLogMessages.Add($"Служба {item.DisplayName} была остановлена");
                        }
                        catch
                        {
                            Logger.GetLogger().AllLogMessages.Add($"Службу {item.DisplayName} не удалось остановить");
                            item.State = true;
                        }

                    }
                    else
                    {
                        try
                        {
                            _regeditManager.ChangeRegistryValue(item.SubKey, "Start", 2, RegistryValueKind.DWord);
                            Logger.GetLogger().AllLogMessages.Add($"Служба {item.DisplayName} была запущена");
                        }
                        catch
                        {
                            Logger.GetLogger().AllLogMessages.Add($"Службу {item.DisplayName} не удалось запустить");
                            item.State = false;
                        }
                    }
                    break;
            }

        }

        public void SetCollectionsFromBackup(BackupItem backupItem)
        {
            var serializer = new XmlSerializer(typeof(AutoRunsModel));
            string solutionPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string backupsPath = Path.Combine(solutionPath, "Backups", backupItem.BackupName);
            AutoRunsItemsRegedit.Clear();
            AutoRunsItemsTasks.Clear();
            AutoRunsItemsServices.Clear();

            using (var reader = XmlReader.Create(backupsPath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "RegeditItem")
                        {
                            var item = DeserializeAutoRunsItem<AutoRunsItem>(reader);
                            AutoRunsItemsRegedit.Add(item);
                        }
                        else if (reader.Name == "TaskItem")
                        {
                            var item = DeserializeAutoRunsItem<AutoRunsItem>(reader);
                            AutoRunsItemsTasks.Add(item);
                        }
                        else if (reader.Name == "ServiceItem")
                        {
                            var item = DeserializeAutoRunsItem<AutoRunsItem>(reader);
                            AutoRunsItemsServices.Add(item);
                        }
                    }
                }
            }
            foreach (var item in AutoRunsItemsRegedit.Concat(AutoRunsItemsServices).Concat(AutoRunsItemsTasks))
            {
                DeleteRegeditItem(item);
            }
        }
    }
}
