using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using INPUTLAGFIX.Models;
using INPUTLAGFIX.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.WindowManagement;

namespace INPUTLAGFIX.ViewModels
{
    public class WindowsOptimizationViewModel : INotifyPropertyChanged
    {
        public WindowsOptimizationModel WindowsOptimizationModel;
        private RegeditManager _regeditManager;
        private AutoRunsModel _autoRunsModel;
        private ObservableCollection<SettingsCategory> _settingsCategories;
        private SettingsCategory _selectedSettingsCategory;
        public IRelayCommand ApplySettingsCommand { get; }
        
        public WindowsOptimizationViewModel()
        {
            _regeditManager = new RegeditManager();
            WindowsOptimizationModel = new WindowsOptimizationModel();
            _autoRunsModel = new AutoRunsModel();
            _settingsCategories = new ObservableCollection<SettingsCategory>
            {
                new SettingsCategory { CategoryName = "Основная оптимизация", Settings = WindowsOptimizationModel.BaseOptimizationSettings },
                new SettingsCategory { CategoryName = "Настройки безопасности и конфиденциальности", Settings = WindowsOptimizationModel.SecuritySettings },
                new SettingsCategory { CategoryName = "Настройки кастомизации Windows", Settings = WindowsOptimizationModel.WindowsCustomizationSettings },
                new SettingsCategory { CategoryName = "Отключение служб", Settings = WindowsOptimizationModel.ServicesSettings},
                new SettingsCategory { CategoryName = "Отключение телеметрии", Settings = WindowsOptimizationModel.PrivacySettings},
                new SettingsCategory { CategoryName = "Отключение ненужных задач", Settings = WindowsOptimizationModel.TasksSettings},
                new SettingsCategory { CategoryName = "Твики", Settings = WindowsOptimizationModel.TweaksSettings}
            };
            ApplySettingsCommand = new RelayCommand(ApplySettings);
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

        public void SetCollectionsFromBackup(BackupItem backupItem)
        {
            WindowsOptimizationModel.SetCollectionsFromBackup(backupItem);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
