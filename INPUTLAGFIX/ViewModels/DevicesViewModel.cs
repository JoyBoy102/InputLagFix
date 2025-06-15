using CommunityToolkit.Mvvm.Input;
using INPUTLAGFIX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.ViewModels
{
    public class DevicesViewModel: INotifyPropertyChanged
    {
        private DevConManager _devConManager;
        private ObservableCollection<DeviceItem> _unnecessaryDevices;
        private ObservableCollection<DeviceItem> _devices;
        public RelayCommand<DeviceItem> DisableEnableDeviceCommand { get; set; }

        private ObservableCollection<String> _allLogMessages = new ObservableCollection<String>();

        public DevicesViewModel()
        {
            _devConManager = new DevConManager();
            _unnecessaryDevices = GetUnnecessaryDevices();
            _devices = GetDevices();
            DisableEnableDeviceCommand = new RelayCommand<DeviceItem>(DisableEnableDevice);
        }

        private ObservableCollection<DeviceItem> GetUnnecessaryDevices()
        {
            Dictionary<string, string> AllDevicesIds = new Dictionary<string, string>()
            {
                { "ROOT\\CompositeBus", "Перечислитель композитной шины" },
                { "ROOT\\vdrvroot", "Перечислитель виртуальных дисков" },
                { "root\\umbus", "UMBus перечислитель корневой шины" },
                { "ROOT\\NdisVirtualBus", "Перечислитель виртуальных сетевых адаптеров" }
            };
            return _devConManager.GetDevices(AllDevicesIds);
        }

        private ObservableCollection<DeviceItem> GetDevices()
        {
            Dictionary<string, string> AllDevicesIds = new Dictionary<string, string>()
            {
                { "SWD\\PRINTENUM\\PrintQueues", "Корневая очередь печати" },
                { "SWD\\MSRRAS\\MS_PPPOEMINIPORT", "WAN Miniport (PPPOE)" },
                { "SWD\\MSRRAS\\MS_PPTPMINIPORT", "WAN Miniport (PPTP)" },
                { "SWD\\MSRRAS\\MS_AGILEVPNMINIPORT", "WAN Miniport (IKEv2)" },
                { "SWD\\MSRRAS\\MS_NDISWANBH", "WAN Miniport (Network Monitor)" },
                {"SWD\\MSRRAS\\MS_NDISWANIP", "WAN Miniport (IP)"},
                {"SWD\\MSRRAS\\MS_SSTPMINIPORT", "WAN Miniport (SSTP)"},
                {"SWD\\MSRRAS\\MS_NDISWANIPV6", "WAN Miniport (IPv6)"},
                {"SWD\\MSRRAS\\MS_L2TPMINIPORT", "WAN Miniport (L2TP)"}
            };
            return _devConManager.GetDevices(AllDevicesIds);
        }
        

        public ObservableCollection<DeviceItem> UnnecessaryDevices
        {
            get => _unnecessaryDevices;
            set
            {
                _unnecessaryDevices = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DeviceItem> Devices
        {
            get => _devices;
            set
            {
                _devices = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AllLogMessage
        {
            get => _allLogMessages;
            set
            {
                _allLogMessages = value;
                OnPropertyChanged();
            }
        }

        private void DisableEnableDevice(DeviceItem deviceItem)
        {
            if (!deviceItem.State)
                _allLogMessages.Add(_devConManager.DisableDevice(deviceItem));
            else
               _allLogMessages.Add(_devConManager.EnableDevice(deviceItem));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
