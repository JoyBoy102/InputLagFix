using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class MsiModeDeviceItem: INotifyPropertyChanged
    {
        public string DisplayName { get; set; }
        public string FullRegPath;
        public bool State {  get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
