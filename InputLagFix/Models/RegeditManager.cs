using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace INPUTLAGFIX.Models
{
    public class RegeditManager
    {
        public ScreenResolution SelectedResolution;

        public string DeleteKey(string keyName, string subKeyPath)
        {
            string[] subKeyPathPathParts = subKeyPath.Split('\\');
            if (subKeyPathPathParts.Length < 2)
            {
                return $"Неверный путь в реестре: {subKeyPath}";
            }
            RegistryKey rootKey = GetRootKey(subKeyPathPathParts[0]);
            if (rootKey == null)
            {
                return $"Неизвестный корневой раздел реестра {subKeyPathPathParts[0]}";
            }
            string KeyPath = string.Join("\\", subKeyPathPathParts, 1, subKeyPathPathParts.Length - 1);
            using (RegistryKey targetKey = rootKey.OpenSubKey(KeyPath, true))
            {
                if (targetKey == null)
                    return $"Ключ {subKeyPath} не найден";
                targetKey.DeleteValue(keyName, false); // false - не выбрасывать исключение если ключ не существует
                return $"Ключ {keyName} удален из {subKeyPath}";
            }
        }

        public string DeleteSubKey(string keyName, string subKeyPath)
        {
            string[] subKeyPathPathParts = subKeyPath.Split('\\');
            if (subKeyPathPathParts.Length < 2)
            {
                return $"Неверный путь в реестре: {subKeyPath}";
            }
            RegistryKey rootKey = GetRootKey(subKeyPathPathParts[0]);
            if (rootKey == null)
            {
                return $"Неизвестный корневой раздел реестра {subKeyPathPathParts[0]}";
            }
            string KeyPath = string.Join("\\", subKeyPathPathParts, 1, subKeyPathPathParts.Length - 1);
            using (RegistryKey targetKey = rootKey.OpenSubKey(KeyPath, true))
            {
                if (targetKey == null)
                    return $"Ключ {subKeyPath} не найден";
                var allsubkeynames = targetKey.GetSubKeyNames();
                if (allsubkeynames.Contains(keyName))
                {
                    targetKey.DeleteSubKey(keyName);
                    return $"Ключ {keyName} удален из {subKeyPath}";
                }
                else
                {
                    return $"Ключа {keyName} не существует в {subKeyPath}"; 
                }
                    
            }
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

        public string ChangeRegistryValue(string valuePath, string valueName, object value, RegistryValueKind valueKind)
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
            
            using (RegistryKey key = rootKey.CreateSubKey(subKeyPath, true))
            {
                if (valueKind!=RegistryValueKind.Binary)
                    key.SetValue(valueName, value, valueKind);
                else
                {
                    byte[] byteArrVal = ConvertStringSettingToBytes(value);
                    key.SetValue(valueName, byteArrVal, valueKind);
                }
                    key.Flush();
                return $"Значение {valueName} в подразделе {valuePath} успешно изменено на {value}";
            }
        }

        private byte[] ConvertStringSettingToBytes(object value)
        {
            string valueInStr = value.ToString();
            byte[] byteArray = valueInStr.Split(' ')
                           .Select(hex => Convert.ToByte(hex, 16))
                           .ToArray();
            return byteArray;
        }

        public (RegistryKey, string) FindRegistryKeyRecursiveByName(string name, RegistryKey parentKey, string parentPath)
        {
            if (parentKey.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase))
            {
                return (parentKey, parentPath);
            }
            else
            {
                foreach (var subkeyname in parentKey.GetSubKeyNames())
                {
                    try
                    {
                        RegistryKey subkey = parentKey.OpenSubKey(subkeyname);
                        if (subkey != null)
                        {
                            var result = FindRegistryKeyRecursiveByName(name, subkey, $"{parentPath}\\{subkeyname}");
                            if (result.Item1 != null)
                            {
                                return result;
                            }
                        }
                    }
                    catch (SecurityException)
                    {
                        continue;
                    }
                }
            }
            return (null,null);
        }
        /*
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
                List<string> monitorIds = _devConManager.GetMonitorId();
                foreach (string monitorId in monitorIds)
                {
                    AllLogMessages.Add(_devConManager.ReinstallDeviceDriver(monitorId));
                    Thread.Sleep(3000);
                }
                List<string> GpuIds = _devConManager.GetGpuId();
                foreach (string GpuId in GpuIds)
                {
                    AllLogMessages.Add(_devConManager.RestartDeviceDriver(GpuId));
                    Thread.Sleep(3000);
                }
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
        */
        private void DeleteAllSimulatedMonitorsAndChangeToCorrectValues(RegistryKey configKey, string MonitorsConfigsFolder, ref int deletedCount)
        {
            if (configKey == null)
                Logger.GetLogger().AllLogMessages.Add("Раздел Configuration не найден в реестре");

            // Получаем все подразделы первого уровня
            string[] subKeyNames = configKey.GetSubKeyNames();

            foreach (string subKeyName in subKeyNames)
            {
                string fullPath = $"{MonitorsConfigsFolder}\\{subKeyName}";
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "ActiveSize.cx", SelectedResolution.Width, RegistryValueKind.DWord));
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "ActiveSize.cy", SelectedResolution.Height, RegistryValueKind.DWord));
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "PrimSurfSize.cx", SelectedResolution.Width, RegistryValueKind.DWord));
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "PrimSurfSize.cy", SelectedResolution.Height, RegistryValueKind.DWord));
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "Stride", (SelectedResolution.Width * 32 + 7) / 8, RegistryValueKind.DWord));
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00", "Stride", (SelectedResolution.Width * 32 + 7) / 8, RegistryValueKind.DWord));
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "DwmClipBox.bottom", SelectedResolution.Height, RegistryValueKind.DWord));
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "DwmClipBox.right", SelectedResolution.Width, RegistryValueKind.DWord));
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00", "PrimSurfSize.cx", SelectedResolution.Width, RegistryValueKind.DWord));
                Logger.GetLogger().AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00", "PrimSurfSize.cy", SelectedResolution.Height, RegistryValueKind.DWord));
            }
        }



        
    }
}
