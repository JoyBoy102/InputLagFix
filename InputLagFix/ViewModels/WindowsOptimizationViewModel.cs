﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        private RegeditManager _regeditManager;
        private AutoRunsModel _autoRunsModel;
        private XmlManager _xmlManager;
        private ObservableCollection<SettingsCategory> _settingsCategories;
        private SettingsCategory _selectedSettingsCategory;
        public IRelayCommand ApplySettingsCommand { get; }
        
        public WindowsOptimizationViewModel()
        {
            _regeditManager = new RegeditManager();
            _autoRunsModel = new AutoRunsModel();
            _xmlManager = new XmlManager();
            _settingsCategories = new ObservableCollection<SettingsCategory>
            {
                new SettingsCategory { CategoryName = "Основная оптимизация", Settings = _xmlManager.GetCollectionOfSettings("BaseOptimization.xml") },
                new SettingsCategory { CategoryName = "Настройки безопасности и конфиденциальности", Settings = _xmlManager.GetCollectionOfSettings("SecuritySettings.xml") },
                new SettingsCategory { CategoryName = "Настройки кастомизации Windows", Settings = _xmlManager.GetCollectionOfSettings("WindowsCustomizationSettings.xml") },
                new SettingsCategory { CategoryName = "Отключение служб", Settings = _xmlManager.GetCollectionOfSettings("ServicesSettings.xml")},
                new SettingsCategory { CategoryName = "Отключение телеметрии", Settings = _xmlManager.GetCollectionOfSettings("PrivacySettings.xml")},
                new SettingsCategory { CategoryName = "Отключение ненужных задач", Settings = _xmlManager.GetCollectionOfSettings("TasksSettings.xml")},
                new SettingsCategory { CategoryName = "Твики", Settings = _xmlManager.GetCollectionOfSettings("Tweaks.xml")}
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
                setting.ApplyOptimization(ref _regeditManager, ref _autoRunsModel);
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
