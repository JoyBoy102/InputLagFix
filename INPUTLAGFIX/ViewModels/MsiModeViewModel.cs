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
    public class MsiModeViewModel:INotifyPropertyChanged
    {
        private ObservableCollection<MsiModeDeviceItem> _msiModeDeviceItems;
        private MsiModeModel _msiModeModel;
        public MsiModeViewModel()
        {
            _msiModeModel = new MsiModeModel();
            _msiModeDeviceItems = _msiModeModel.GetAllMsiModeDeviceItems();
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
