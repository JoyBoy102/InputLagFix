using CommunityToolkit.Mvvm.ComponentModel;
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
    public class AutoRunsViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<AutoRunsItem> _regeditAutoRuns = new ObservableCollection<AutoRunsItem>();
        private ObservableCollection<AutoRunsItem> _TaskAutoRuns = new ObservableCollection<AutoRunsItem>();
        private ObservableCollection<AutoRunsItem> _ServicesAutoRuns = new ObservableCollection<AutoRunsItem>();
        private AutoRunsModel _autoRunsModel;
        public AutoRunsViewModel()
        {
            _autoRunsModel = new AutoRunsModel();
            _regeditAutoRuns = _autoRunsModel.GetAllAutoRunsItemsRegedit();
        }

        public ObservableCollection<AutoRunsItem> RegeditAutoRuns
        {
            get => _regeditAutoRuns;
            set
            {
                _regeditAutoRuns = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
