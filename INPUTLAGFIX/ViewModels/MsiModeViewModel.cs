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
using Windows.ApplicationModel.VoiceCommands;

namespace INPUTLAGFIX.ViewModels
{
    public class MsiModeViewModel:INotifyPropertyChanged
    {
        private ObservableCollection<MsiModeDeviceItem> _msiModeDeviceItems;
        private MsiModeModel _msiModeModel;
        public RelayCommand<MsiModeDeviceItem> TurnOnOffMsiModeCommand { get; set; }
        public MsiModeViewModel()
        {
            _msiModeModel = new MsiModeModel();
            _msiModeDeviceItems = _msiModeModel.GetAllMsiModeDeviceItems();
            TurnOnOffMsiModeCommand = new RelayCommand<MsiModeDeviceItem>(TurnOnOffMsiMode);
        }

        public ObservableCollection<MsiModeDeviceItem> MsiModeDeviceItems
        {
            get => _msiModeDeviceItems;
            set
            {
                _msiModeDeviceItems = value;
                OnPropertyChanged();
            }
        }

        private void TurnOnOffMsiMode(MsiModeDeviceItem msiModeDeviceItem)
        {
            _msiModeModel.TurnOffOnMsiMode(msiModeDeviceItem);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
