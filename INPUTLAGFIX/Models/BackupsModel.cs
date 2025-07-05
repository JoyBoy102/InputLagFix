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
        private string solutionPath;
        private string backupsPath;
        public ObservableCollection<BackupItem> BackupItems { get; set; }

        public BackupsModel()
        {
            solutionPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            backupsPath = Path.Combine(solutionPath, "Backups");
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
                    var backupItem = new BackupItem
                    {
                        BackupName = Path.GetFileName(filePath),
                        BackupDateTime = File.GetCreationTime(filePath)
                    };

                    res.Add(backupItem);
                }
            }

            return res;
        }

    }
}
