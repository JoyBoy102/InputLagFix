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
        public ObservableCollection<Optimization> GetCollectionOfSettings(string xml)
        {
            ObservableCollection<Optimization> res = new ObservableCollection<Optimization>();
            XDocument xdoc = XDocument.Load(xml);
            XElement root = xdoc.Root;
            foreach (var optimization in root.Elements())
            {
                ObservableCollection<Setting> settingsList = new ObservableCollection<Setting>();
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
                            value_if_false = setting.Element("value_if_false")?.Value,
                            value_if_true = setting.Element("value_if_true")?.Value,

                        };
                        settingsList.Add(sett);
                    }
                    else
                    {
                        Setting sett = new Setting
                        {
                            valuePath = setting.Element("valuePath")?.Value,
                            value_if_false = setting.Element("value_if_false")?.Value,
                            value_if_true = setting.Element("value_if_true")?.Value,
                            isTask = true
                        };
                        settingsList.Add(sett);
                    }
                }
                
                res.Add(new Optimization { settings = settingsList, ruName = OptimizationRuName, AddWindow = AddWindow });
                
            }
            return res;
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
    }
}
