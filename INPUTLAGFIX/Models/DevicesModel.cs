using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace INPUTLAGFIX.Models
{
    public class DevicesModel:BackuperFromXml
    {
        [XmlArray("DeviceItems")]
        [XmlArrayItem("DeviceItem")]
        public ObservableCollection<DeviceItem> DeviceItems;
        [XmlArray("UnnecessaryDeviceItems")]
        [XmlArrayItem("UnnecessaryDeviceItem")]
        public ObservableCollection<DeviceItem> UnnecessaryDevices;

        public DevicesModel()
        {
            DeviceItems = GetDevices();
            UnnecessaryDevices = GetUnnecessaryDevices();
        }

        private ObservableCollection<DeviceItem> GetUnnecessaryDevices()
        {
            Dictionary<string, string> AllDevicesIds = new Dictionary<string, string>()
            {
                { "ROOT\\CompositeBus", "Перечислитель композитной шины" },
                { "ROOT\\vdrvroot", "Перечислитель виртуальных дисков" },
                { "root\\umbus", "UMBus перечислитель корневой шины" },
                { "ROOT\\NdisVirtualBus", "Перечислитель виртуальных сетевых адаптеров" }
            };
            return GetDevices(AllDevicesIds);
        }

        private ObservableCollection<DeviceItem> GetDevices()
        {
            Dictionary<string, string> AllDevicesIds = new Dictionary<string, string>()
            {
                { "SWD\\PRINTENUM\\PrintQueues", "Корневая очередь печати" },
                { "SWD\\MSRRAS\\MS_PPPOEMINIPORT", "WAN Miniport (PPPOE)" },
                { "SWD\\MSRRAS\\MS_PPTPMINIPORT", "WAN Miniport (PPTP)" },
                { "SWD\\MSRRAS\\MS_AGILEVPNMINIPORT", "WAN Miniport (IKEv2)" },
                { "SWD\\MSRRAS\\MS_NDISWANBH", "WAN Miniport (Network Monitor)" },
                {"SWD\\MSRRAS\\MS_NDISWANIP", "WAN Miniport (IP)"},
                {"SWD\\MSRRAS\\MS_SSTPMINIPORT", "WAN Miniport (SSTP)"},
                {"SWD\\MSRRAS\\MS_NDISWANIPV6", "WAN Miniport (IPv6)"},
                {"SWD\\MSRRAS\\MS_L2TPMINIPORT", "WAN Miniport (L2TP)"}
            };
            return GetDevices(AllDevicesIds);
        }
        public string RestartDeviceDriver(DeviceItem deviceItem)
        {
            try
            {
                DisableDevice(deviceItem);
                EnableDevice(deviceItem);

                return $"Драйвер устройства {deviceItem.DisplayName} перезагружен успешно.";
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        public bool CheckDeviceStatus(string deviceID)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "devcon.exe",
                    Arguments = $"status \"{deviceID}\"",
                    RedirectStandardOutput = true, 
                    UseShellExecute = false,      
                    CreateNoWindow = true,       
                    Verb = "runas"
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();

                    // Читаем вывод devcon
                    string output = process.StandardOutput.ReadToEnd();

                    // Проверяем, содержит ли вывод "Device is started"
                    return output.Contains("Driver is running");
                }


            }
            catch 
            {
                return false;
            }
        }

        public string EnableDevice(DeviceItem deviceItem)
        {
            try
            {
                // Запуск с правами администратора
                var processInfo = new ProcessStartInfo
                {
                    FileName = "devcon.exe",
                    UseShellExecute = true,
                    Verb = "runas", // Запрос прав администратора
                    CreateNoWindow = true,
                    Arguments = $"enable \"{deviceItem.HardwareID}\"",
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        return $"Ошибка при включении драйвера устройcтва {deviceItem.DisplayName}.";
                    else
                        return $"Устройство {deviceItem.DisplayName} включено";
                }
                
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        public string DisableDevice(DeviceItem deviceItem)
        {
            try
            {
                // Запуск с правами администратора
                var processInfo = new ProcessStartInfo
                {
                    FileName = "devcon.exe",
                    UseShellExecute = true,
                    Verb = "runas", // Запрос прав администратора
                    CreateNoWindow = true,
                    Arguments = $"disable \"{deviceItem.HardwareID}\"",
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        return $"Ошибка при отключении драйвера устройcтва {deviceItem.DisplayName}.";
                    else
                        return $"Устройство {deviceItem.DisplayName} отключено";
                }

            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        public ObservableCollection<DeviceItem> GetDevices(Dictionary<string, string> AllDevicesIds)
        {
            ObservableCollection<DeviceItem> result = new ObservableCollection<DeviceItem>();
            foreach (var deviceId in AllDevicesIds)
            {
                if (CheckIfDeviceExists(deviceId.Key))
                {
                    result.Add(new DeviceItem
                    {
                        DisplayName = deviceId.Value,
                        HardwareID = deviceId.Key,
                        State = CheckDeviceStatus(deviceId.Key),
                        Visibility = System.Windows.Visibility.Visible
                    });
                }
                else
                {
                    result.Add(new DeviceItem
                    {
                        DisplayName = deviceId.Value,
                        HardwareID = deviceId.Key,
                        State = false,
                        AdditionalInfo = "Устройство не найдено на комьютере",
                        Visibility = System.Windows.Visibility.Collapsed
                    });
                }
            }
            return result;
        }

        private bool CheckIfDeviceExists(string hardwareId)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "devcon.exe",
                    Arguments = $"find \"{hardwareId}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    return !string.IsNullOrEmpty(output) && !output.Contains("No matching devices found");
                }
            }
            catch
            {
                return false;
            }
        }

        public string GetDisplayNameFromHardwareID(string hardwareId)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "devcon.exe",
                    Arguments = $"find \"{hardwareId}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string pattern = $@":\s+(.*?)\s*\n";
                    Match match = Regex.Match(output, pattern);
                    if (match.Success)
                    {
                        string deviceName = match.Groups[1].Value;
                        return deviceName;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public void DisableEnableDevice(DeviceItem deviceItem)
        {
            if (!deviceItem.State)
                Logger.GetLogger().AllLogMessages.Add(DisableDevice(deviceItem));
            else
                Logger.GetLogger().AllLogMessages.Add(EnableDevice(deviceItem));
        }

        public void SetCollectionsFromBackup(BackupItem backupItem)
        {
            var serializer = new XmlSerializer(typeof(DevicesModel));
            string solutionPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string backupsPath = Path.Combine(solutionPath, "Backups", backupItem.BackupName);
            DeviceItems.Clear();
            UnnecessaryDevices.Clear();
            using (var reader = XmlReader.Create(backupsPath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "DeviceItem")
                        {
                            var item = DeserializeAutoRunsItem<DeviceItem>(reader);
                            DeviceItems.Add(item);
                        }
                        else if (reader.Name == "UnnecessaryDeviceItem")
                        {
                            var item = DeserializeAutoRunsItem<DeviceItem>(reader);
                            UnnecessaryDevices.Add(item);
                        }


                    }
                }
            }
            foreach (var item in DeviceItems.Concat(UnnecessaryDevices))
            {
                DisableEnableDevice(item);
            }
        }
    }
    
}

