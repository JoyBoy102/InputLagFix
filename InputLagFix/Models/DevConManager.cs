using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class DevConManager
    {
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

        public string ReinstallDeviceDriver(string hardwareId)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "devcon.exe",
                    UseShellExecute = true,
                    Verb = "runas", // Запуск с правами администратора
                    CreateNoWindow = true,
                    Arguments = $"remove \"{hardwareId}\""

                };
                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        return $"Не удалось удалить устройство - {hardwareId}.";
                }

                // Небольшая задержка для завершения операции
                Thread.Sleep(2000);

                // Шаг 2: Сканирование для повторного обнаружения монитора
                processInfo.Arguments = "rescan"; // Автоматическая переустановка драйвера
                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        return "Ошибка при повторном сканировании устройств.";
                }

                return $"Устройство {hardwareId} успешно перезагружено";
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        public List<string> GetMonitorId()
        {
            List<string> res = new List<string>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Service = 'monitor'"))
                {
                    foreach (ManagementObject device in searcher.Get())
                    {
                        string pnpDeviceId = device["PNPDeviceID"]?.ToString();
                        if (!string.IsNullOrEmpty(pnpDeviceId) && pnpDeviceId.Contains("DISPLAY"))
                        {
                            pnpDeviceId = pnpDeviceId.Replace("DISPLAY", "MONITOR");
                            string[] deviceIdParts = pnpDeviceId.Split("\\");
                            res.Add($"{deviceIdParts[0]}\\{deviceIdParts[1]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.Add($"Ошибка при поиске монитора: {ex.Message}");
            }
            return res;
        }

        public List<string> GetGpuId()
        {
            List<string> res = new List<string>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject device in searcher.Get())
                    {
                        string pnpDeviceId = device["PNPDeviceID"]?.ToString();
                        if (!string.IsNullOrEmpty(pnpDeviceId))
                        {
                            string[] deviceIdParts = pnpDeviceId.Split("\\");
                            res.Add($"{deviceIdParts[0]}\\{deviceIdParts[1]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.Add($"Ошибка при поиске видеокарты: {ex.Message}");
            }
            return res;
        }

        public List<string> GetMouseIds()
        {
            List<string> res = new List<string>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PointingDevice"))
                {
                    foreach (ManagementObject device in searcher.Get())
                    {
                        string pnpDeviceId = device["PNPDeviceID"]?.ToString();
                        if (!string.IsNullOrEmpty(pnpDeviceId))
                        {
                            string[] deviceIdParts = pnpDeviceId.Split("\\");
                            res.Add($"{deviceIdParts[0]}\\{deviceIdParts[1]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.Add($"Ошибка при поиске мыши: {ex.Message}");
            }
            return res;
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

    }
    
}

