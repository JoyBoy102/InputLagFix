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
                var settings = optimization.Elements();
                foreach (var setting in settings)
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
                
                res.Add(new Optimization { settings = settingsList, ruName = OptimizationRuName });
                
            }
            return res;
        }
    }
}
