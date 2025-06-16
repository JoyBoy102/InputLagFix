using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Management.Deployment;

namespace INPUTLAGFIX.Models
{
    public class Uninstaller
    {
        private List<string> registryPaths = new List<string>()
        {
             @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
             @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };
        private List<DeleteItem> _allDeleteItems = new List<DeleteItem>();
        public ObservableCollection<DeleteItem> DeletedItemsUWP = new ObservableCollection<DeleteItem>();
        public ObservableCollection<DeleteItem> AllDeleteItems = new ObservableCollection<DeleteItem>();
        private RegeditManager _regeditManager;

        public Uninstaller()
        {
            GetAllItemsForUninstall();
            DeletedItemsUWP = new ObservableCollection<DeleteItem>(GetDeleteItemsUWP());
            AllDeleteItems = new ObservableCollection<DeleteItem>(_allDeleteItems);
            _regeditManager = new RegeditManager();
        }

        public async Task<bool> UninstallProgramm(DeleteItem item)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.Arguments = $"/c \"{item.UninstallString}\" /S";
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                Process process = new Process();
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    Logger.GetLogger().AllLogMessages.Add($"Не удалось удалить программу {item.DisplayName}");
                }
                _regeditManager.DeleteSubKey(item.keyname, item.subkeyname);
                return process.ExitCode == 0 ? true : false;
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                Logger.GetLogger().AllLogMessages.Add($"Ошибка: {ex.Message}");
                return false;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка: {ex.Message}");
                Logger.GetLogger().AllLogMessages.Add($"Неизвестная ошибка: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UninstallUWPProgrammAsync(DeleteItem item)
        {
            var packageManager = new PackageManager();
            var removalResult = await packageManager.RemovePackageAsync(item.UninstallString);
            if (removalResult.IsRegistered)
                return true;
            else
            {
                MessageBox.Show($"Ошибка: {removalResult.ErrorText}");
                Logger.GetLogger().AllLogMessages.Add($"Ошибка: {removalResult.ErrorText}");
                return false;
            }
        }

        public void GetAllItemsForUninstall()
        {
            foreach (var registryPath in registryPaths)
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath);
                if (key != null)
                    _allDeleteItems.AddRange(GetDeleteItemsFromKey(key, $"HKEY_LOCAL_MACHINE\\{registryPath}"));


            }
        }

        public List<DeleteItem> GetDeleteItemsFromKey(RegistryKey key, string regPath)
        {
            List<DeleteItem> result = new List<DeleteItem>();
            foreach (var subkeyname in key.GetSubKeyNames())
            {
                using (RegistryKey subkey = key.OpenSubKey(subkeyname))
                {
                    string uninsString = subkey.GetValue("UninstallString", "").ToString();
                    string displayName = subkey.GetValue("DisplayName", "").ToString();
                    string systemcomp = subkey.GetValue("SystemComponent","").ToString();
                    if (!string.IsNullOrEmpty(uninsString) && !string.IsNullOrEmpty(displayName))
                    {
                        if (string.IsNullOrEmpty(systemcomp) || systemcomp == "0")
                        {
                            DeleteItem item = new DeleteItem { DisplayName = displayName, UninstallString = uninsString, isUWP = false, keyname = subkeyname, subkeyname = regPath};
                            result.Add(item);
                        }
                    }
                }
            }
            return result;
        }

        public List<DeleteItem> GetDeleteItemsUWP()
        {
           
            var packages = new List<DeleteItem>();
            var packageManager = new PackageManager();
            foreach (var package in packageManager.FindPackagesForUser(""))
            {
                if (package.Status.VerifyIsOK() && package.IsFramework == false && package.SignatureKind != Windows.ApplicationModel.PackageSignatureKind.System && Directory.Exists(package.InstalledLocation.Path))
                {
                    DeleteItem deleteItem = new DeleteItem();
                    deleteItem.isUWP = true;
                    deleteItem.DisplayName = package.Id.Name;
                    deleteItem.UninstallString = package.Id.FullName;
                    deleteItem.LogoUri = package.Logo;
                    packages.Add(deleteItem);
                }
            }
            
            return packages;
            
        }
    }
}
