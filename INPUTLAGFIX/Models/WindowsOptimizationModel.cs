using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Web.AtomPub;


namespace INPUTLAGFIX.Models
{
    public class WindowsOptimizationModel:BackuperFromXml
    {
        public ObservableCollection<Optimization> BaseOptimizationSettings;
        public ObservableCollection<Optimization> SecuritySettings;
        public ObservableCollection<Optimization> WindowsCustomizationSettings;
        public ObservableCollection<Optimization> ServicesSettings;
        public ObservableCollection<Optimization> PrivacySettings;
        public ObservableCollection<Optimization> TasksSettings;
        public ObservableCollection<Optimization> TweaksSettings;

        [XmlArray("BaseOptimizationBackupItems")]
        [XmlArrayItem("BaseOptimizationBackupItem")]
        public ObservableCollection<Optimization> BaseOptimizationSettingsBackup;

        [XmlArray("SecuritySettingsBackupItems")]
        [XmlArrayItem("SecuritySettingsBackupItem")]
        public ObservableCollection<Optimization> SecuritySettingsBackup;

        [XmlArray("WindowsCustomizationSettingsBackupItems")]
        [XmlArrayItem("WindowsCustomizationSettingsBackupItem")]
        public ObservableCollection<Optimization> WindowsCustomizationSettingsBackup;

        [XmlArray("ServicesSettingsBackupItems")]
        [XmlArrayItem("ServicesSettingsBackupItem")]
        public ObservableCollection<Optimization> ServicesSettingsBackup;

        [XmlArray("PrivacySettingsBackupItems")]
        [XmlArrayItem("PrivacySettingsBackupItem")]
        public ObservableCollection<Optimization> PrivacySettingsBackup;

        [XmlArray("TasksSettingsBackupItems")]
        [XmlArrayItem("TasksSettingsBackupItem")]
        public ObservableCollection<Optimization> TasksSettingsBackup;

        [XmlArray("TweaksSettingsBackupItems")]
        [XmlArrayItem("TweaksSettingsBackupItem")]
        public ObservableCollection<Optimization> TweaksSettingsBackup;

        private XmlManager _xmlManager;
        private RegeditManager _regeditManager;
        public WindowsOptimizationModel()
        {
            _xmlManager = new XmlManager();
            _regeditManager = new RegeditManager();
            BaseOptimizationSettings = _xmlManager.GetCollectionOfSettings("BaseOptimization.xml").Item1;
            SecuritySettings = _xmlManager.GetCollectionOfSettings("SecuritySettings.xml").Item1;
            WindowsCustomizationSettings = _xmlManager.GetCollectionOfSettings("WindowsCustomizationSettings.xml").Item1;
            ServicesSettings = _xmlManager.GetCollectionOfSettings("ServicesSettings.xml").Item1;
            PrivacySettings = _xmlManager.GetCollectionOfSettings("PrivacySettings.xml").Item1;
            TasksSettings = _xmlManager.GetCollectionOfSettings("TasksSettings.xml").Item1;
            TweaksSettings = _xmlManager.GetCollectionOfSettings("Tweaks.xml").Item1;

            BaseOptimizationSettingsBackup = _xmlManager.GetCollectionOfSettings("BaseOptimization.xml").Item2;
            SecuritySettingsBackup = _xmlManager.GetCollectionOfSettings("SecuritySettings.xml").Item2;
            WindowsCustomizationSettingsBackup = _xmlManager.GetCollectionOfSettings("WindowsCustomizationSettings.xml").Item2;
            ServicesSettingsBackup = _xmlManager.GetCollectionOfSettings("ServicesSettings.xml").Item2;
            PrivacySettingsBackup = _xmlManager.GetCollectionOfSettings("PrivacySettings.xml").Item2;
            TasksSettingsBackup = _xmlManager.GetCollectionOfSettings("TasksSettings.xml").Item2;
            TweaksSettingsBackup = _xmlManager.GetCollectionOfSettings("Tweaks.xml").Item2;

        }

        public void SetCollectionsFromBackup(BackupItem backupItem)
        {
            var serializer = new XmlSerializer(typeof(DevicesModel));
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string backupPath = Path.Combine(appDataPath, "InputLagFix", "Backups", backupItem.BackupName);

            BaseOptimizationSettings.Clear();
            SecuritySettings.Clear();
            WindowsCustomizationSettings.Clear();
            ServicesSettings.Clear();
            PrivacySettings.Clear();
            TweaksSettings.Clear();
            TasksSettings.Clear();

            using (var reader = XmlReader.Create(backupPath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "BaseOptimizationBackupItem")
                        {
                            var item = DeserializeAutoRunsItem<Optimization>(reader);
                            BaseOptimizationSettings.Add(item);
                        }
                        else if (reader.Name == "SecuritySettingsBackupItem")
                        {
                            var item = DeserializeAutoRunsItem<Optimization>(reader);
                            SecuritySettings.Add(item);
                        }
                        else if (reader.Name == "WindowsCustomizationSettingsBackupItem")
                        {
                            var item = DeserializeAutoRunsItem<Optimization>(reader);
                            WindowsCustomizationSettings.Add(item);
                        }
                        else if (reader.Name == "ServicesSettingsBackupItem")
                        {
                            var item = DeserializeAutoRunsItem<Optimization>(reader);
                            ServicesSettings.Add(item);
                        }
                        else if (reader.Name == "PrivacySettingsBackupItem")
                        {
                            var item = DeserializeAutoRunsItem<Optimization>(reader);
                            PrivacySettings.Add(item);
                        }
                        else if (reader.Name == "TasksSettingsBackupItem")
                        {
                            var item = DeserializeAutoRunsItem<Optimization>(reader);
                            TasksSettings.Add(item);
                        }
                        else if (reader.Name == "TweaksSettingsBackupItem")
                        {
                            var item = DeserializeAutoRunsItem<Optimization>(reader);
                            TweaksSettings.Add(item);
                        }


                    }
                }
            }
            foreach (var item in BaseOptimizationSettings.Concat(SecuritySettings).Concat(WindowsCustomizationSettings).Concat(ServicesSettings).Concat(PrivacySettings).Concat(TasksSettings).Concat(TweaksSettings))
            {
                item.ApplyOptimization(ref _regeditManager, true);
            }
        }
    }
}
