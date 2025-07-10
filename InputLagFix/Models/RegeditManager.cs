using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class RegeditManager
    {
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
                if (valueKind != RegistryValueKind.Binary)
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
            return (null, null);
        }

        public string GetValueFromRegedit(string valuePath, string valueName)
        {
            string[] valuePathParts = valuePath.Split('\\');
            if (valuePathParts.Length < 2)
            {
                throw new ArgumentException();
            }
            RegistryKey rootKey = GetRootKey(valuePathParts[0]);
            if (rootKey == null)
            {
                throw new ArgumentException();
            }

            string subKeyPath = string.Join("\\", valuePathParts, 1, valuePathParts.Length - 1);

            try
            {
                using (RegistryKey key = rootKey.CreateSubKey(subKeyPath, true))
                {
                    object val = key.GetValue(valueName);
                    return val == null? "delete": val.ToString();
                }
            }
            catch
            {
                return PowerRunManager.GetValueFromRegeditWithPowerRun(valuePath, valueName);
            }
        }
    }
}
