using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                Thread.Sleep(3000);

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
    }
}

