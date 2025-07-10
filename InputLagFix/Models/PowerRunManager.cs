using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace INPUTLAGFIX.Models
{
    public class PowerRunManager
    {
        public static string ApplyRegSettingWithPowerRun(string valuePath, string valueName, object value, RegistryValueKind valueKind)
        {
            string regType;
            switch (valueKind)
            {
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    regType = "REG_SZ";
                    break;
                case RegistryValueKind.DWord:
                    regType = "REG_DWORD";
                    break;
                case RegistryValueKind.QWord:
                    regType = "REG_QWORD";
                    break;
                case RegistryValueKind.Binary:
                    regType = "REG_BINARY";
                    break;
                case RegistryValueKind.MultiString:
                    regType = "REG_MULTI_SZ";
                    break;
                default:
                    return "Unsupported registry value type";
            }

            // Экранирование специальных символов в значении
            string stringValue = value.ToString().Replace("\"", "\\\"");

            // Формирование команды для reg.exe
            string regCommand = $"add \"{valuePath}\" /v \"{valueName}\" /t {regType} /d \"{stringValue}\" /f";

            // Создание процесса PowerRun
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "PowerRun.exe",
                Arguments = $"/SW:0 \"Reg.exe\" {regCommand}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Запуск процесса
            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    return $"Failed to set registry value. Error: {error}";
                }
            }
            return $"Значение {valueName} в подразделе {valuePath} успешно изменено на {value}";
        }

        public static string DeleteKeyWithPowerRun(string valuePath, string valueName)
        {
            string regCommand = $"delete \"{valuePath}\" /v \"{valueName}\" /f";

            // Создание процесса PowerRun
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "PowerRun.exe",
                Arguments = $"/SW:0 \"Reg.exe\" {regCommand}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Запуск процесса
            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    return $"Failed to delete registry key. Error: {error}";
                }
            }
            return $"Ключ {valueName} в подразделе {valuePath} успешно удален";
        }

        public static string GetValueFromRegeditWithPowerRun(string valuePath, string valueName)
        {
            // Формирование команды для reg.exe
            string regCommand = $"query \"{valuePath}\" /v \"{valueName}\"";

            // Создание процесса PowerRun
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "PowerRun.exe",
                Arguments = $"/SW:0 \"Reg.exe\" {regCommand}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Запуск процесса
            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Если ключ или значение не найдены
                if (process.ExitCode != 0 ||
                    output.Contains("ERROR: The system was unable to find the specified registry key or value"))
                {
                    return "delete";
                }

                // Парсинг результата
                string result = ParseRegQueryOutput(output, valueName);
                return result == "Value not found in registry output" ? "delete" : result;
            }
        }


        private static string ParseRegQueryOutput(string output, string valueName)
        {
            try
            {
                // Разделяем вывод на строки
                string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // Ищем строку с нашим значением
                foreach (string line in lines)
                {
                    if (line.Contains(valueName))
                    {
                        // Разделяем строку по пробелам (формат: "valueName    REG_TYPE    value")
                        string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length >= 3)
                        {
                            // Объединяем все части после типа, так как значение может содержать пробелы
                            return string.Join(" ", parts.Skip(2));
                        }
                    }
                }

                return "Value not found in registry output";
            }
            catch (Exception ex)
            {
                return $"Error parsing registry output: {ex.Message}";
            }
        }
    }
}
