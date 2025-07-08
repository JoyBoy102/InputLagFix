using INPUTLAGFIX.Models;
using INPUTLAGFIX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace INPUTLAGFIX
{
    [XmlRoot("Backup")]
    public class SerializeModels
    {
        [XmlElement("AutoRuns")]
        public AutoRunsModel autoRunsModel = (Application.Current.Resources["AutoRunsVM"] as AutoRunsViewModel).AutoRunsModel;
        [XmlElement("MsiMode")]
        public MsiModeModel msiModeModel = (Application.Current.Resources["MsiModeVM"] as MsiModeViewModel).MsiModeModel;
        [XmlIgnore]
        public WindowsOptimizationViewModel windowsOptimizationViewModel = Application.Current.Resources["SharedWinOptimizationVM"] as WindowsOptimizationViewModel;
        [XmlIgnore]
        public DevicesViewModel devicesViewModel = Application.Current.Resources["DevicesVM"] as DevicesViewModel;
    }
}
