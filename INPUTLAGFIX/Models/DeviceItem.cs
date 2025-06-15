using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace INPUTLAGFIX.Models
{
    public class DeviceItem: INotifyPropertyChanged
    {
        private bool _state;
        public string DisplayName { get; set; }
        public string AdditionalInfo { get; set; }

        public Visibility Visibility { get; set; }
        public string HardwareID { get; set; }
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
         => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
