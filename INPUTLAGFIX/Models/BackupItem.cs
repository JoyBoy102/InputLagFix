using INPUTLAGFIX.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace INPUTLAGFIX.Models
{
    public class BackupItem
    {
        public string BackupName { get; set; }
        public DateTime BackupDateTime {  get; set; }

        private SerializeModels serializeModels;
        private string solutionPath;
        private string backupsPath;


        public BackupItem()
        {
            BackupName = Guid.NewGuid().ToString();
            BackupDateTime = DateTime.Now;
            serializeModels = new SerializeModels();
            solutionPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            backupsPath = Path.Combine(solutionPath, "Backups", BackupName);
            SaveBackupToXml();
        }

        public BackupItem(string backupName, DateTime backupDateTime)
        {
            BackupName = backupName;
            BackupDateTime = backupDateTime;
        }

        private void SaveBackupToXml()
        {
            var serializer = new XmlSerializer(typeof(SerializeModels));
            using (var writer = new StreamWriter(backupsPath))
            {
                serializer.Serialize(writer, serializeModels);
            }
        }

        public void ApplyBackup()
        {
            AutoRunsViewModel autoRunsViewModel = Application.Current.Resources["AutoRunsVM"] as AutoRunsViewModel;
            MsiModeViewModel msiModeViewModel = Application.Current.Resources["MsiModeVM"] as MsiModeViewModel;
            WindowsOptimizationViewModel windowsOptimizationViewModel = Application.Current.Resources["SharedWinOptimizationVM"] as WindowsOptimizationViewModel;
            DevicesViewModel devicesViewModel = Application.Current.Resources["DevicesVM"] as DevicesViewModel;
            autoRunsViewModel.SetCollectionsFromBackup(this);
            msiModeViewModel.SetCollectionsFromBackup(this);
            devicesViewModel.SetCollectionsFromBackup(this);
            windowsOptimizationViewModel.SetCollectionsFromBackup(this);
        }

    }
}
