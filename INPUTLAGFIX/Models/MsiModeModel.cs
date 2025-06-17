using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using Microsoft.Win32;
namespace INPUTLAGFIX.Models
{
    public class MsiModeModel
    {
        private string _subKey = "SYSTEM\\CurrentControlSet\\Enum\\PCI";
        private RegeditManager _regeditManager;
        private DevConManager _devConManager;

        public MsiModeModel()
        {
            _regeditManager = new RegeditManager();
            _devConManager = new DevConManager();
        }
        public ObservableCollection<MsiModeDeviceItem> GetAllMsiModeDeviceItems()
        {
            ObservableCollection<MsiModeDeviceItem> result = new ObservableCollection<MsiModeDeviceItem>();
            foreach (var subkeyname in Registry.LocalMachine.OpenSubKey(_subKey).GetSubKeyNames())
            {
                string parentPath = $"{_subKey}\\{subkeyname}";
                RegistryKey parentKey = Registry.LocalMachine.OpenSubKey(parentPath);
                if (parentKey != null) {
                    var regKeyInfo = _regeditManager.FindRegistryKeyRecursiveByName("MessageSignaledInterruptProperties", parentKey, parentPath);
                    if (regKeyInfo.Item1 != null)
                    {
                        var msiVal = regKeyInfo.Item1.GetValue("MSISupported").ToString();
                        if (!string.IsNullOrEmpty(msiVal))
                        {
                            string HardwareDisplayName = _devConManager.GetDisplayNameFromHardwareID($"PCI\\{subkeyname}");
                            MsiModeDeviceItem msiModeDeviceItem = new MsiModeDeviceItem() {DisplayName = $"{HardwareDisplayName}", FullRegPath = $"HKEY_LOCAL_MACHINE\\{regKeyInfo.Item2}", State = msiVal == "1" };
                            result.Add(msiModeDeviceItem);
                        }
                        regKeyInfo.Item1.Dispose();
                    }
                    
                }
            }
            
            return result;
        }

    }
}
