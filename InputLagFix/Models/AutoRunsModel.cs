using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Windows.ApplicationModel.Appointments;

namespace INPUTLAGFIX.Models
{
    public class AutoRunsModel
    {
        private string _registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private string _registryPathServices = @"SYSTEM\CurrentControlSet\Services";
        private List<RegistryKey> allRegistryKeys = new List<RegistryKey>()
        {
            Registry.LocalMachine,
            Registry.CurrentUser
        };
        public ObservableCollection<AutoRunsItem> GetAllAutoRunsItemsRegedit()
        {
            List<AutoRunsItem> result = new List<AutoRunsItem>();
            foreach (var key in allRegistryKeys)
            {
                var registryAutoRunsItems = key.OpenSubKey(_registryPath).GetValueNames().ToList();
                result.AddRange((registryAutoRunsItems.Select(x => new AutoRunsItem { DisplayName = x }).ToList()));
            }
            return new ObservableCollection<AutoRunsItem>(result);
        }

        public ObservableCollection<AutoRunsItem> GetAllAutoRunsItemsTasks()
        {
            List<AutoRunsItem> result = new List<AutoRunsItem>();
            using (TaskService ts = new TaskService())
            {

                foreach (Microsoft.Win32.TaskScheduler.Task task in ts.RootFolder.AllTasks)
                {
                    if (!task.Path.Contains("Windows"))
                        result.Add(new AutoRunsItem { DisplayName = task.Name });
                }
            }
            return new ObservableCollection<AutoRunsItem>(result);
        }

        public ObservableCollection<AutoRunsItem> GetAllAutoRunsItemsServices()
        {
            List<AutoRunsItem> result = new List<AutoRunsItem>();
            RegistryKey key = Registry.LocalMachine.OpenSubKey(_registryPathServices);
            foreach (var subkeyname in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(subkeyname);
                var ImagePath = subkey.GetValue("ImagePath");
                string imgPathString = ImagePath == null? string.Empty : ImagePath.ToString().ToLower();
                if (!string.IsNullOrEmpty(imgPathString) && (!imgPathString.Contains("system") && !imgPathString.Contains(@"\windows\")))
                {
                    result.Add(new AutoRunsItem { DisplayName = subkeyname, ImagePath = imgPathString });
                }
            }
            return new ObservableCollection<AutoRunsItem>(result);
        }
    }
}
