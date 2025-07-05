using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace INPUTLAGFIX.Models
{
    [Serializable]
    public class AutoRunsItem: INotifyPropertyChanged
    {
       private bool _state;
       public string DisplayName { get; set; }
       public string ImagePath { get; set; }

       public string SubKey;
       public string Type;
       public string TaskPath;
       public bool State
       {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
       }

       [XmlElement("Name")]
       public string XmlDisplayName => DisplayName;

       [XmlElement("RegistryPath")]
       public string XmlSubKey => SubKey;

       [XmlElement ("Type")]
       public string XmlType => Type;

       public event PropertyChangedEventHandler? PropertyChanged;
       private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
