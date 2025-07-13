using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class BackupsModel
    {
        private string appDataPath;
        private string backupsPath;
        public ObservableCollection<BackupItem> BackupItems { get; set; }

        public BackupsModel()
        {
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            backupsPath = Path.Combine(appDataPath, "InputLagFix", "Backups");
            BackupItems = GetSavedBackups();
        }

        private ObservableCollection<BackupItem> GetSavedBackups()
        {
            ObservableCollection<BackupItem> res = new ObservableCollection<BackupItem>();
            if (Directory.Exists(backupsPath))
            {
                var backupFiles = Directory.GetFiles(backupsPath);

                foreach (var filePath in backupFiles)
                {
                    string backupName = Path.GetFileName(filePath);
                    DateTime backupDateTime = File.GetCreationTime(filePath);
                    var backupItem = new BackupItem(backupName, backupDateTime);
                    res.Add(backupItem);
                }
            }

            return res;
        }

    }
}
