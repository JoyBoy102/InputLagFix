using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class CleaningCategoryItem : INotifyPropertyChanged
    {
        private string _additionalInfo;
        public string DisplayName { get; set; }
        public List<string> Folders = new List<string>();
        public string AdditionalInfo
        {
            get => _additionalInfo;
            set
            {
                _additionalInfo = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
