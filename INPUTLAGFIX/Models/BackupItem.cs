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
            //StreamWriter streamWriter = new StreamWriter(backupsPath);
            //SaveAutoRunsToXml(streamWriter);
            // SaveMsiModeToXml(streamWriter);
            // streamWriter.Dispose();
            SaveBackupToXml();
        }

        public BackupItem(string backupName, DateTime backupDateTime)
        {
            BackupName = backupName;
            BackupDateTime = backupDateTime;
        }

        private void SaveAutoRunsToXml(StreamWriter writer)
        {
            var autoRunsViewModel = Application.Current.Resources["AutoRunsVM"];
            var serializer = new XmlSerializer(typeof(AutoRunsModel)); 
            if (autoRunsViewModel is AutoRunsViewModel viewModel)
            {
                serializer.Serialize(writer, viewModel.AutoRunsModel);
            }
        }

        private void SaveMsiModeToXml(StreamWriter writer)
        {
            var msiModeViewModel = Application.Current.Resources["MsiModeVM"];
            var serializer = new XmlSerializer(typeof(MsiModeModel));
            if (msiModeViewModel is MsiModeViewModel viewModel)
            {
               serializer.Serialize(writer, viewModel.MsiModeModel);
            }
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
        }

    }
}
