using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.TaskScheduler;
using System.Xml.Serialization;

namespace INPUTLAGFIX.Models
{
    [Serializable]
    public class AutoRunsItem
    {
       public string DisplayName { get; set; }
       public string ImagePath { get; set; }

       public string SubKey;
       public string Type;
       [XmlIgnore]
       public Microsoft.Win32.TaskScheduler.Task Task;
       public bool State {  get; set; }

       [XmlElement("Name")]
       public string XmlDisplayName => DisplayName;

       [XmlElement("RegistryPath")]
       public string XmlSubKey => SubKey;

       [XmlElement ("Type")]
       public string XmlType => Type;
    }
}
