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

        public BackupItem()
        {
            BackupName = Guid.NewGuid().ToString();
            BackupDateTime = DateTime.Now;
            SaveAutoRunsToXml();
        }

        private void SaveAutoRunsToXml()
        {
            var autoRunsViewModel = Application.Current.Resources["AutoRunsVM"];
            var serializer = new XmlSerializer(typeof(AutoRunsModel));
            string solutionPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string backupsPath = Path.Combine(solutionPath, "Backups", BackupName);
            if (autoRunsViewModel is AutoRunsViewModel viewModel)
            {
                using (var writer = new StreamWriter(backupsPath))
                {
                    serializer.Serialize(writer, viewModel.AutoRunsModel);
                }
            }
        }
    }
}
