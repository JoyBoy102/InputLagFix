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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace INPUTLAGFIX.ViewModels
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<OptimizationsCategoryItem> _optimizationsCategoryItems;
        private OptimizationsCategoryItem _selectedOptimizationsCategory;
        private Visibility _contentControlVisibility = Visibility.Visible;
        private Visibility _loadingControlVisibility = Visibility.Collapsed;
        public MainWindowViewModel()
        {
            _optimizationsCategoryItems = new ObservableCollection<OptimizationsCategoryItem>()
            {
                new OptimizationsCategoryItem { DisplayName = "Оптимизация Windows", Control = new WindowsOptimization(), Img = new BitmapImage(new Uri("pack://application:,,,/Icons/WindowsOptimization.png"))},
                new OptimizationsCategoryItem { DisplayName = "Удаление программ", Control = new DeleteProgramsTabPage() },
                new OptimizationsCategoryItem { DisplayName = "Автозагрузки", Control = new AutoRuns() },
                new OptimizationsCategoryItem { DisplayName = "Устройства", Control = new Devices() },
                new OptimizationsCategoryItem { DisplayName = "MsiMode", Control = new MsiModeView() },
                new OptimizationsCategoryItem { DisplayName = "Очистка", Control = new CleanFilesView()},
                new OptimizationsCategoryItem { DisplayName = "Бэкапы", Control = new BackupsView() }
            };
            (Application.Current.Resources["EventAggregator"] as EventAggregator).SettingsStartApplying += () =>
            {
                LoadingControlVisibility = Visibility.Visible;
                ContentControlVisibility = Visibility.Collapsed;
            };
            (Application.Current.Resources["EventAggregator"] as EventAggregator).SettingsStopApplying += () =>
            {
                LoadingControlVisibility = Visibility.Collapsed;
                ContentControlVisibility = Visibility.Visible;
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

        public ObservableCollection<string> AllLogMessages
        {
            get => Logger.GetLogger().AllLogMessages;
            set
            {
                Logger.GetLogger().AllLogMessages = value;
                OnPropertyChanged();
            }
        }

        public Visibility ContentControlVisibility
        {
            get => _contentControlVisibility;
            set
            {
                _contentControlVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility LoadingControlVisibility
        {
            get => _loadingControlVisibility;
            set
            {
                _loadingControlVisibility = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
