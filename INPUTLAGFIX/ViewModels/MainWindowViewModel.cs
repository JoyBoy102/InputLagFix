﻿using INPUTLAGFIX.Models;
using INPUTLAGFIX.Views;
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
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<OptimizationsCategoryItem> _optimizationsCategoryItems;
        private OptimizationsCategoryItem _selectedOptimizationsCategory;
        public MainWindowViewModel()
        {
            _optimizationsCategoryItems = new ObservableCollection<OptimizationsCategoryItem>()
            {
                new OptimizationsCategoryItem { DisplayName = "Оптимизация Windows", Control = new WindowsOptimization() },
                new OptimizationsCategoryItem { DisplayName = "Удаление программ", Control = new DeleteProgramsTabPage() },
                new OptimizationsCategoryItem { DisplayName = "Автозагрузки", Control = new AutoRuns() },
                new OptimizationsCategoryItem { DisplayName = "Устройства", Control = new Devices() }
            };
        }

        public ObservableCollection<OptimizationsCategoryItem> OptimizationsCategoryItems
        {
            get => _optimizationsCategoryItems;
            set
            {
                _optimizationsCategoryItems = value;
                OnPropertyChanged();
            }
        }

        public OptimizationsCategoryItem SelectedOptimizationsCategory
        {
            get => _selectedOptimizationsCategory;
            set
            {
                _selectedOptimizationsCategory = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
