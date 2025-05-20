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
        private XmlManager _xmlManager;
        private ObservableCollection<SettingsCategory> _settingsCategories;
        private SettingsCategory _selectedSettingsCategory;
        public ICommand ApplySettingsCommand { get; }
        public RegeditManagerViewModel()
        {
            _regeditManager = new RegeditManager();
            _xmlManager = new XmlManager();
            _settingsCategories = new ObservableCollection<SettingsCategory>
            {
                new SettingsCategory { CategoryName = "Основная оптимизация", Settings = _xmlManager.GetCollectionOfSettings("BaseOptimization.xml") },
            };
            ApplySettingsCommand = new RelayCommand(ApplySettings);
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

        public ObservableCollection<SettingsCategory> SettingsCategories
        {
            get => _settingsCategories;
            set
            {
                _settingsCategories = value;
                OnPropertyChanged();
            }
        }

        public SettingsCategory SelectedCategory
        {
            get => _selectedSettingsCategory;
            set
            {
                _selectedSettingsCategory = value;
                OnPropertyChanged();
            }
        }
        private void ApplySettings()
        {
            foreach (var setting in _selectedSettingsCategory.Settings)
            {
                setting.ApplyOptimization(ref _regeditManager);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
