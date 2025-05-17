using GalaSoft.MvvmLight.Command;
using INPUTLAGFIX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace INPUTLAGFIX.ViewModels
{
    public class RegeditManagerViewModel : INotifyPropertyChanged
    {
        private RegeditManager _regeditManager;
        public ICommand FixMonitorResolutionsCommand { get; }

        public RegeditManagerViewModel()
        {
            _regeditManager = new RegeditManager();
            FixMonitorResolutionsCommand = new RelayCommand(FixMonitorResolutions);
        }
        public ObservableCollection<string> LogMessages
        {
            get => _regeditManager.AllLogMessages;
            set
            {
                _regeditManager.AllLogMessages = value;
                OnPropertyChanged();
            }
        }

        private void FixMonitorResolutions()
        {
            _regeditManager.MonitorInputLagFix();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
