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
    }
}
