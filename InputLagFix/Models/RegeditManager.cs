using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class RegeditManager
    {
        public ObservableCollection<string>AllLogMessages = new ObservableCollection<string>();
        private string DeleteFolder(string folderPath)
        {
            string[] folderPathParts = folderPath.Split('\\');
            if (folderPathParts.Length < 2)
            {
                return $"Неверный путь в реестре: {folderPath}";
            }
            RegistryKey rootKey = GetRootKey(folderPathParts[0]);
            if (rootKey == null)
            {
                return $"Неизвестный корневой раздел реестра {folderPathParts[0]}";
            }
            string subKeyPath = string.Join("\\", folderPathParts, 1, folderPathParts.Length - 1);
            rootKey.DeleteSubKeyTree(subKeyPath);
            rootKey.Close();
            return $"Подраздел {folderPath} удален";
        }

        private RegistryKey GetRootKey(string rootKeyName)
        {
            switch (rootKeyName.ToUpper())
            {
                case "HKEY_CLASSES_ROOT":
                case "HKCR":
                    return Registry.ClassesRoot;
                case "HKEY_CURRENT_USER":
                case "HKCU":
                    return Registry.CurrentUser;
                case "HKEY_LOCAL_MACHINE":
                case "HKLM":
                    return Registry.LocalMachine;
                case "HKEY_USERS":
                    return Registry.Users;
                case "HKEY_CURRENT_CONFIG":
                case "HKCC":
                    return Registry.CurrentConfig;
                default:
                    return null;
            }
        }

        private string ChangeRegistryValue(string valuePath, string valueName, object value, RegistryValueKind valueKind)
        {
            string[] valuePathParts = valuePath.Split('\\');
            if (valuePathParts.Length < 2)
            {
                return $"Неверный путь в реестре: {valuePath}";
            }
            RegistryKey rootKey = GetRootKey(valuePathParts[0]);
            if (rootKey == null)
            {
                return $"Неизвестный корневой раздел реестра {valuePathParts[0]}";
            }
            string subKeyPath = string.Join("\\", valuePathParts, 1, valuePathParts.Length - 1);
            using (RegistryKey key = rootKey.CreateSubKey(subKeyPath))
            {
                if (ValueExists(valuePath, valueName))
                {
                    key.SetValue(valueName, value, valueKind);
                    return $"Значение {valueName} в подразделе {valuePath} успешно изменено на {value}";
                }
                else
                {
                    return $"Значение {valueName} в подразделе {valuePath} успешно создано и изменено на {value}";
                }
            }
        }

        private bool ValueExists(string keyPath, string valueName)
        {
            try
            {
                string[] pathParts = keyPath.Split('\\');
                RegistryKey rootKey = GetRootKey(pathParts[0]);

                if (rootKey == null) return false;

                string subKeyPath = string.Join("\\", pathParts, 1, pathParts.Length - 1);

                using (RegistryKey key = rootKey.OpenSubKey(subKeyPath))
                {
                    return key?.GetValue(valueName) != null;
                }
            }
            catch
            {
                return false;
            }
        }

        public void MonitorInputLagFix()
        {
            const string MonitorsConfigsFolder = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\GraphicsDrivers\Configuration";
            
            try
            {
                int deletedCount = 0;
                string[] pathParts = MonitorsConfigsFolder.Split('\\');
                RegistryKey rootKey = GetRootKey(pathParts[0]);
                if (rootKey == null)
                    AllLogMessages.Add("Не удалось определить корневой раздел реестра");

                string subKeyPath = string.Join("\\", pathParts, 1, pathParts.Length - 1);
                

                using (RegistryKey configKey = rootKey.OpenSubKey(subKeyPath))
                {
                    DeleteAllSimulatedMonitorsAndChangeToCorrectValues(configKey, MonitorsConfigsFolder, ref deletedCount);
                }

                AllLogMessages.Add($"Удалено {deletedCount} подразделов с SIMULATED");
            }
            catch (UnauthorizedAccessException)
            {
                AllLogMessages.Add("Ошибка: Недостаточно прав (требуются права администратора)");
            }
            catch (Exception ex)
            {
                AllLogMessages.Add($"Ошибка при обработке: {ex.Message}");
            }
        }

        private void DeleteAllSimulatedMonitorsAndChangeToCorrectValues(RegistryKey configKey, string MonitorsConfigsFolder, ref int deletedCount)
        {
            if (configKey == null)
                AllLogMessages.Add("Раздел Configuration не найден в реестре");

            // Получаем все подразделы первого уровня
            string[] subKeyNames = configKey.GetSubKeyNames();

            foreach (string subKeyName in subKeyNames)
            {
                // Проверяем, начинается ли имя подраздела с "SIMULATED"
                if (subKeyName.StartsWith("SIMULATED", StringComparison.OrdinalIgnoreCase))
                {
                    string fullPath = $"{MonitorsConfigsFolder}\\{subKeyName}";
                    string result = DeleteFolder(fullPath);

                    if (result.Contains("удален"))
                        deletedCount++;
                    AllLogMessages.Add(result);
                }
                else
                {
                    string fullPath = $"{MonitorsConfigsFolder}\\{subKeyName}";
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "ActiveSize.cx", 780, RegistryValueKind.DWord));
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "ActiveSize.cy", 438, RegistryValueKind.DWord));
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "PrimSurfSize.cx", 780, RegistryValueKind.DWord));
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "PrimSurfSize.cy", 438, RegistryValueKind.DWord));
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "Stride", 7680, RegistryValueKind.DWord));
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00", "Stride", 7680, RegistryValueKind.DWord));
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "DwmClipBox.left", 780, RegistryValueKind.DWord));
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "DwmClipBox.right", 438, RegistryValueKind.DWord));
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00", "PrimSurfSize.cx", 780, RegistryValueKind.DWord));
                    AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00", "PrimSurfSize.cy", 438, RegistryValueKind.DWord));
                }

            }
        }
    }
}
