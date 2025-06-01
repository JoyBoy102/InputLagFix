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
        private DateTime _currentDateTime;
        private List<DeleteItem> _allDeleteItems = new List<DeleteItem>();
        public ObservableCollection<DeleteItem> RecentlyDeletedItems = new ObservableCollection<DeleteItem>();
        public ObservableCollection<DeleteItem> NotRecentlyDeletedItems = new ObservableCollection<DeleteItem>();
        public ObservableCollection<DeleteItem> DeletedItemsUWP = new ObservableCollection<DeleteItem>();
        public ObservableCollection<string> AllLogMessages = new ObservableCollection<string>();

        public Uninstaller()
        {
            _currentDateTime = DateTime.Now;
            GetAllItemsForUninstall();
            RecentlyDeletedItems = new ObservableCollection<DeleteItem>(_allDeleteItems.Where(x=> (_currentDateTime - x.InstallDate).Days <= 4));
            NotRecentlyDeletedItems = new ObservableCollection<DeleteItem>(_allDeleteItems.Where(x => (_currentDateTime - x.InstallDate).Days > 4));
            DeletedItemsUWP = new ObservableCollection<DeleteItem>(GetDeleteItemsUWP());
        }

        public async Task<bool> UninstallProgramm(string uninstallString, string displayName)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.Arguments = $"/c \"{uninstallString}\" /S";
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
                    AllLogMessages.Add($"Не удалось удалить программу {displayName}");
                }
                return process.ExitCode == 0? true: false;
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                AllLogMessages.Add($"Ошибка: {ex.Message}");
                return false;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка: {ex.Message}");
                AllLogMessages.Add($"Неизвестная ошибка: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UninstallUWPProgrammAsync(string packageFullName)
        {
            var packageManager = new PackageManager();
            var removalResult = await packageManager.RemovePackageAsync(packageFullName);
            if (removalResult.IsRegistered)
                return true;
            else
            {
                MessageBox.Show($"Ошибка: {removalResult.ErrorText}");
                AllLogMessages.Add($"Ошибка: {removalResult.ErrorText}");
                return false;
            }
        }

        public void GetAllItemsForUninstall()
        {
            foreach (var registryPath in registryPaths)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
                {
                    if (key != null)
                        _allDeleteItems.AddRange(GetDeleteItemsFromKey(key));
                }
            }
            //var filteredByDateResult = result.Where(x => (_currentDateTime - x.InstallDate).Days <= 4).ToList();
        }

        public List<DeleteItem> GetDeleteItemsFromKey(RegistryKey key)
        {
            List<DeleteItem> result = new List<DeleteItem>();
            foreach (var subkeyname in key.GetSubKeyNames())
            {
                using (RegistryKey subkey = key.OpenSubKey(subkeyname))
                {
                    string uninsString = subkey.GetValue("UninstallString", "").ToString();
                    string displayName = subkey.GetValue("DisplayName", "").ToString();
                    string installDateInString = subkey.GetValue("InstallDate", "").ToString();
 
                    if (!string.IsNullOrEmpty(uninsString) && !string.IsNullOrEmpty(displayName))
                    {
                        if (uninsString.Contains("C:\\Program Files"))
                        {
                            DateTime installDate = new DateTime();
                            if (!string.IsNullOrEmpty(installDateInString))
                                installDate = DateTime.ParseExact(installDateInString, "yyyyMMdd", CultureInfo.InvariantCulture);
                            else installDate = DateTime.MinValue;
                            DeleteItem item = new DeleteItem { DisplayName = displayName, UninstallString = uninsString, InstallDate = installDate, isUWP = false};
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
                    deleteItem.InstallDate = DateTime.MinValue;
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
