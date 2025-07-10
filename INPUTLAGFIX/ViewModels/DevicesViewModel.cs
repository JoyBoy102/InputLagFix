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
        public DevicesModel DevicesModel;
        private ObservableCollection<DeviceItem> _unnecessaryDevices;
        private ObservableCollection<DeviceItem> _devices;
        public RelayCommand<DeviceItem> DisableEnableDeviceCommand { get; set; }

        public DevicesViewModel()
        {
            DevicesModel = new DevicesModel();
            DisableEnableDeviceCommand = new RelayCommand<DeviceItem>(DisableEnableDevice);
            _unnecessaryDevices = DevicesModel.UnnecessaryDevices;
            _devices = DevicesModel.DeviceItems;
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

        private void DisableEnableDevice(DeviceItem deviceItem)
        {
            DevicesModel.DisableEnableDevice(deviceItem);
        }

        public void SetCollectionsFromBackup(BackupItem backupItem)
        {
            DevicesModel.SetCollectionsFromBackup(backupItem);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
