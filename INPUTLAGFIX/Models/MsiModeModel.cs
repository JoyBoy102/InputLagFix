using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
namespace INPUTLAGFIX.Models
{
    public class MsiModeModel:BackuperFromXml
    {
        private string _subKey = "SYSTEM\\CurrentControlSet\\Enum\\PCI";
        private RegeditManager _regeditManager;
        private DevicesModel _devConManager;
        [XmlArray("MsiModeItems")]
        [XmlArrayItem("MsiModeItem")]
        public ObservableCollection<MsiModeDeviceItem> MsiModeDeviceItems;

        public MsiModeModel()
        {
            _regeditManager = new RegeditManager();
            _devConManager = new DevicesModel();
            MsiModeDeviceItems = GetAllMsiModeDeviceItems();
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

        public void TurnOffOnMsiMode(MsiModeDeviceItem item)
        {
            if (item.State)
            {
                Logger.GetLogger().AllLogMessages.Add(_regeditManager.ChangeRegistryValue(item.FullRegPath, "MSISupported", 1, RegistryValueKind.DWord));
            }
            else
            {
                Logger.GetLogger().AllLogMessages.Add(_regeditManager.ChangeRegistryValue(item.FullRegPath, "MSISupported", 0, RegistryValueKind.DWord));
            }
        }

        public void SetCollectionsFromBackup(BackupItem backupItem)
        {
            var serializer = new XmlSerializer(typeof(AutoRunsModel));
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string backupPath = Path.Combine(appDataPath, "InputLagFix", "Backups", backupItem.BackupName);
            MsiModeDeviceItems.Clear();

            using (var reader = XmlReader.Create(backupPath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "MsiModeItem")
                        {
                            var item = DeserializeAutoRunsItem<MsiModeDeviceItem>(reader);
                            MsiModeDeviceItems.Add(item);
                        }

                    }
                }
            }
            foreach (var item in MsiModeDeviceItems)
            {
                TurnOffOnMsiMode(item);
            }
        }

    }
}
