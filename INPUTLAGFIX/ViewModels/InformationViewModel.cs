using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.ViewModels
{
    public class InformationViewModel: INotifyPropertyChanged
    {
        public IRelayCommand OpenDefenderSettingsCommand { get; set; }
        public InformationViewModel()
        {
            OpenDefenderSettingsCommand = new RelayCommand(OpenDefenderSetting);
        }

        private void OpenDefenderSetting()
        {
            Process.Start(new ProcessStartInfo("ms-settings:windowsdefender")
            {
                UseShellExecute = true
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
