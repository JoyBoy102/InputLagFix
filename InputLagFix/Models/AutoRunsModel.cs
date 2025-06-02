using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace INPUTLAGFIX.Models
{
    public class AutoRunsModel
    {
        private string _registryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
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
    }
}
