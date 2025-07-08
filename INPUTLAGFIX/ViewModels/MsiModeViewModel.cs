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
        public MsiModeModel MsiModeModel;
        private ObservableCollection<MsiModeDeviceItem> _msiModeDeviceItems = new ObservableCollection<MsiModeDeviceItem>();
        public RelayCommand<MsiModeDeviceItem> TurnOnOffMsiModeCommand { get; set; }
        public MsiModeViewModel()
        {
            MsiModeModel = new MsiModeModel();
            _msiModeDeviceItems = MsiModeModel.MsiModeDeviceItems;
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
            MsiModeModel.TurnOffOnMsiMode(msiModeDeviceItem);
        }

        public void SetCollectionsFromBackup(BackupItem backupItem)
        {
            MsiModeModel.SetCollectionsFromBackup(backupItem);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
