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
using GalaSoft.MvvmLight.Command;
using INPUTLAGFIX.Views;
namespace INPUTLAGFIX.ViewModels
{
    public class DeleteProgramsTabPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DeleteItemsCategory> _deleteItemsCategories;
        private ObservableCollection<DeleteItem> _recentlyDeletedItems;
        private ObservableCollection<DeleteItem> _notRecentlyDeletedItems;
        private ObservableCollection<DeleteItem> _uwpDeleteItems;
        private Uninstaller _uninstaller;
        public ICommand UninstallProgrammInRecentlyDeletedCommand { get; }
        public ICommand UninstallProgrammInNotRecentlyDeletedCommand { get; }
        private DeleteItemsCategory _selectedDeleteItemCategory;

        public DeleteProgramsTabPageViewModel()
        {
            _uninstaller = new Uninstaller();
            _recentlyDeletedItems = _uninstaller.RecentlyDeletedItems;
            _notRecentlyDeletedItems = _uninstaller.NotRecentlyDeletedItems;
            _deleteItemsCategories = new ObservableCollection<DeleteItemsCategory>
            {
                new DeleteItemsCategory { CategoryName = "Все программы", RecentlyDeletedItems = _recentlyDeletedItems, NotRecentlyDeletedItems = _notRecentlyDeletedItems, Control = new NotUWP()},
                new DeleteItemsCategory { CategoryName = "Программы UWP", }
            };
            UninstallProgrammInRecentlyDeletedCommand = new RelayCommand<DeleteItem>(UninstallProgrammInRecentlyDeleted);
            UninstallProgrammInNotRecentlyDeletedCommand = new RelayCommand<DeleteItem>(UninstallProgrammInNotRecentlyDeleted);
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

        private void UninstallProgrammInRecentlyDeleted(DeleteItem item)
        {
            bool res;
            if (!item.isUWP)
                res = _uninstaller.UninstallProgramm(item.UninstallString);
            else
                res = _uninstaller.UninstallUWPProgramm(item.UninstallString);
            if (res)
            {
                SelectedDeleteItemsCategory.RecentlyDeletedItems.Remove(item);
            }
        }

        private void UninstallProgrammInNotRecentlyDeleted(DeleteItem item)
        {
            bool res;
            if (!item.isUWP)
                res = _uninstaller.UninstallProgramm(item.UninstallString);
            else
                res = _uninstaller.UninstallUWPProgramm(item.UninstallString);
            if (res)
            {
                SelectedDeleteItemsCategory.NotRecentlyDeletedItems.Remove(item);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
