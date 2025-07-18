﻿using INPUTLAGFIX.ViewModels;
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
        private string appDataPath;
        private string backupsFolderPath;


        public BackupItem()
        {
            BackupName = Guid.NewGuid().ToString();
            BackupDateTime = DateTime.Now;
            serializeModels = new SerializeModels();
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            backupsFolderPath = Path.Combine(appDataPath, "InputLagFix", "Backups");
            SaveBackupToXml();
        }

        public BackupItem(string backupName, DateTime backupDateTime)
        {
            BackupName = backupName;
            BackupDateTime = backupDateTime;
        }

        private void SaveBackupToXml()
        {
            Directory.CreateDirectory(backupsFolderPath);
            string filePath = Path.Combine(backupsFolderPath, BackupName);
            var serializer = new XmlSerializer(typeof(SerializeModels));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, serializeModels);
                writer.Flush();
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
