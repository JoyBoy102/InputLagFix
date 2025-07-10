using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace INPUTLAGFIX.Models
{
    public class XmlManager
    {
        private Dictionary<string, RegistryValueKind> stringToObject = new Dictionary<string, RegistryValueKind>()
        {
            { "REG_SZ", RegistryValueKind.String },
            { "REG_DWORD", RegistryValueKind.DWord },
            { "REG_BINARY", RegistryValueKind.Binary},
            { "REG_EXPAND_SZ", RegistryValueKind.ExpandString }
        };
        private RegeditManager _regeditManager;
        public XmlManager()
        {
            _regeditManager = new RegeditManager();
        }
        public (ObservableCollection<Optimization>, ObservableCollection<Optimization>) GetCollectionOfSettings(string xml)
        {
            ObservableCollection<Optimization> res = new ObservableCollection<Optimization>();
            ObservableCollection<Optimization> resForBackup = new ObservableCollection<Optimization>();
            XDocument xdoc = XDocument.Load(xml);
            XElement root = xdoc.Root;
            foreach (var optimization in root.Elements())
            {
                ObservableCollection<Setting> settingsList = new ObservableCollection<Setting>();
                ObservableCollection<Setting> settingsListBackup = new ObservableCollection<Setting>();
                string? OptimizationRuName = optimization.Attribute("ruName")?.Value;
                bool AddWindow = optimization.Attribute("AddWindow")?.Value == "true" ? true : false;
                var settings = optimization.Elements();
                foreach (var setting in settings)
                {
                    if (!setting.HasAttributes)
                    {
                        Setting sett = new Setting
                        {
                            valuePath = setting.Element("valuePath")?.Value,
                            valueName = setting.Element("valueName")?.Value,
                            valueKind = stringToObject[setting.Element("valueKind")?.Value],
                            value = setting.Element("value_if_true")?.Value,
                        };
                        Setting settForBackup = new Setting
                        {
                            valuePath = setting.Element("valuePath")?.Value,
                            valueName = setting.Element("valueName")?.Value,
                            valueKind = stringToObject[setting.Element("valueKind")?.Value],
                            value = _regeditManager.GetValueFromRegedit(setting.Element("valuePath")?.Value, setting.Element("valueName")?.Value)
                        };
                        settingsList.Add(sett);
                        settingsListBackup.Add(settForBackup);
                    }
                    else
                    {
                        Setting sett = new Setting
                        {
                            valuePath = setting.Element("valuePath")?.Value,
                            value = setting.Element("value_if_true")?.Value,
                            isTask = true
                        };
                        settingsList.Add(sett);
                    }
                }
                
                res.Add(new Optimization { settings = settingsList, ruName = OptimizationRuName, AddWindow = AddWindow });
                resForBackup.Add(new Optimization { settings = settingsListBackup, ruName = OptimizationRuName, AddWindow = false });
            }
            return (res, resForBackup);
        }

        

        public List<AutoRunsItem> GetSavedAutoRunsItemsRegedit()
        {
            List<AutoRunsItem> autoRunsItems = new List<AutoRunsItem>();
            XDocument xdoc = XDocument.Load("AutoRunsItems.xml");
            XElement root = xdoc.Root;
            HashSet<string> uniqueDisplayNames = new HashSet<string>();
            foreach (var autorunitemxml in root.Elements())
            {
                AutoRunsItem autoRunsItem = new AutoRunsItem
                {
                    SubKey = autorunitemxml.Element("SubKey").Value,
                    Type = autorunitemxml.Element("Type").Value,
                    DisplayName = autorunitemxml.Element("DisplayName").Value,
                    State = autorunitemxml.Element("State").Value == "true"? true:false
                };
                if (!uniqueDisplayNames.Contains(autoRunsItem.DisplayName))
                {
                    uniqueDisplayNames.Add(autoRunsItem.DisplayName);
                    autoRunsItems.Add(autoRunsItem);
                }
            }
            return autoRunsItems;
        }

        public List<CleaningCategoryItem> GetAllCleaningCategoryItems()
        {
            List<CleaningCategoryItem> cleaningCategoryItems = new List<CleaningCategoryItem>();
            XDocument xdoc = XDocument.Load("CleanFolders.xml");
            XElement root = xdoc.Root;
            foreach (var cleaningCategory in root.Elements())
            {
                string cleaningCategoryDisplayName = cleaningCategory.Attribute("ruName")?.Value;

                CleaningCategoryItem cleaningCategoryItem = new CleaningCategoryItem
                {
                    DisplayName = cleaningCategoryDisplayName,
                    Folders = cleaningCategory.Elements().Select(folder => folder.Value.ToString()).ToList()
                };
                cleaningCategoryItems.Add(cleaningCategoryItem);
            }
            return cleaningCategoryItems;
        }
    }
}
