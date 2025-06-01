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
namespace INPUTLAGFIX.ViewModels
{
    public class DeleteProgramsTabPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DeleteItemsCategory> _deleteItemsCategories;
        private ObservableCollection<DeleteItem> _recentlyDeletedItems;
        private ObservableCollection<DeleteItem> _notRecentlyDeletedItems;
        private ObservableCollection<DeleteItem> _uwpDeleteItems;
        private Uninstaller _uninstaller;
        public IAsyncRelayCommand<DeleteItem> UninstallProgrammInRecentlyDeletedCommand { get; }
        public IAsyncRelayCommand<DeleteItem> UninstallProgrammInNotRecentlyDeletedCommand { get; }
        public IAsyncRelayCommand<DeleteItem> UninstallUWPProgramCommand { get; }
        private DeleteItemsCategory _selectedDeleteItemCategory;
        private ObservableCollection<string> _logmessages;
        public DeleteProgramsTabPageViewModel()
        {
            _uninstaller = new Uninstaller();
            _recentlyDeletedItems = _uninstaller.RecentlyDeletedItems;
            _notRecentlyDeletedItems = _uninstaller.NotRecentlyDeletedItems;
            _deleteItemsCategories = new ObservableCollection<DeleteItemsCategory>
            {
                new DeleteItemsCategory { CategoryName = "Все программы", RecentlyDeletedItems = _recentlyDeletedItems, NotRecentlyDeletedItems = _notRecentlyDeletedItems, Control = new NotUWP()},
                new DeleteItemsCategory { CategoryName = "Программы UWP", AllItems = _uninstaller.DeletedItemsUWP, Control = new UWP()}
            };
            UninstallProgrammInRecentlyDeletedCommand = new AsyncRelayCommand<DeleteItem>(UninstallProgrammInRecentlyDeletedAsync);
            UninstallProgrammInNotRecentlyDeletedCommand = new AsyncRelayCommand<DeleteItem>(UninstallProgrammInNotRecentlyDeletedAsync);
            UninstallUWPProgramCommand = new AsyncRelayCommand<DeleteItem>(UninstallUWPProgramm);
        }

        public ObservableCollection<DeleteItemsCategory> DeleteItemsCategories
        {
            get => _deleteItemsCategories;
            set
            {
                _deleteItemsCategories = value;
                OnPropertyChanged();
            }
        }

        public DeleteItemsCategory SelectedDeleteItemsCategory
        {
            get => _selectedDeleteItemCategory;
            set
            {
                _selectedDeleteItemCategory = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> LogMessages
        {
            get => _uninstaller.AllLogMessages;
            set
            {
                _uninstaller.AllLogMessages = value;
                OnPropertyChanged();
            }
        }

        private async Task UninstallProgrammInRecentlyDeletedAsync(DeleteItem item)
        {
            bool res;
            if (!item.isUWP)
                res = await _uninstaller.UninstallProgramm(item.UninstallString, item.DisplayName);
            else
                res = false;
            if (res)
            {
                SelectedDeleteItemsCategory.RecentlyDeletedItems.Remove(item);
                LogMessages.Add($"Программа {item.DisplayName} успешно удалена с компьютера.");
            }
        }

        private async Task UninstallProgrammInNotRecentlyDeletedAsync(DeleteItem item)
        {
            bool res;
            if (!item.isUWP)
                res = await _uninstaller.UninstallProgramm(item.UninstallString, item.DisplayName);
            else
                res = false;
            if (res)
            {
                SelectedDeleteItemsCategory.NotRecentlyDeletedItems.Remove(item);
                LogMessages.Add($"Программа {item.DisplayName} успешно удалена с компьютера.");
            }
        }

        private async Task UninstallUWPProgramm(DeleteItem item)
        {
            bool res;
            if (item.isUWP)
            {
                res = await _uninstaller.UninstallUWPProgrammAsync(item.UninstallString);
            }
            else res = false;
            if (res)
            {
                SelectedDeleteItemsCategory.AllItems.Remove(item);
                LogMessages.Add($"Программа {item.DisplayName} успешно удалена с компьютера.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
