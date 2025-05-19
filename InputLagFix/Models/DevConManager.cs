using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class DevConManager
    {
        public string RestartDeviceDriver(string hardwareId)
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
                    Arguments = $"disable \"{hardwareId}\"",
                };

                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        return $"Ошибка при отключении драйвера устройcтва {hardwareId}.";
                }

                // Включение драйвера
                processInfo.Arguments = $"enable \"{hardwareId}\"";
                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        return $"Ошибка при включении драйвера устройства {hardwareId}.";
                }

                return $"Драйвер устройства {hardwareId} перезагружен успешно.";
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
    }
}

