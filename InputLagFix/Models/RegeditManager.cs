using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;


namespace INPUTLAGFIX.Models
{
    public class RegeditManager
    {
        public ObservableCollection<string>AllLogMessages = new ObservableCollection<string>();
        public ScreenResolution SelectedResolution;
        private DevConManager _devConManager;

        public RegeditManager()
        {
            _devConManager = new DevConManager();
        }
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
                key.SetValue(valueName, value, valueKind);
                key.Flush();
                return $"Значение {valueName} в подразделе {valuePath} успешно изменено на {value}";
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
                AllLogMessages.Add(_devConManager.ReinstallDeviceDriver("MONITOR\\HKM2500"));
                Thread.Sleep(3000);
                AllLogMessages.Add(_devConManager.RestartDeviceDriver("PCI\\VEN_10DE&DEV_2803&SUBSYS_F3061569&REV_A1"));
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
                string fullPath = $"{MonitorsConfigsFolder}\\{subKeyName}";
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "ActiveSize.cx", SelectedResolution.Width, RegistryValueKind.DWord));
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "ActiveSize.cy", SelectedResolution.Height, RegistryValueKind.DWord));
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "PrimSurfSize.cx", SelectedResolution.Width, RegistryValueKind.DWord));
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "PrimSurfSize.cy", SelectedResolution.Height, RegistryValueKind.DWord));
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "Stride", (SelectedResolution.Width * 32 + 7) / 8, RegistryValueKind.DWord));
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00", "Stride", (SelectedResolution.Width * 32 + 7) / 8, RegistryValueKind.DWord));
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "DwmClipBox.bottom", SelectedResolution.Height, RegistryValueKind.DWord));
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00\\00", "DwmClipBox.right", SelectedResolution.Width, RegistryValueKind.DWord));
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00", "PrimSurfSize.cx", SelectedResolution.Width, RegistryValueKind.DWord));
                AllLogMessages.Add(ChangeRegistryValue($"{fullPath}\\00", "PrimSurfSize.cy", SelectedResolution.Height, RegistryValueKind.DWord));
            }
        }

        
    }
}
