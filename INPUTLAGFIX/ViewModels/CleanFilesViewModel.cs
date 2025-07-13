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
using Windows.Graphics.Printing.PrintSupport;

namespace INPUTLAGFIX.ViewModels
{
    public class CleanFilesViewModel: INotifyPropertyChanged
    {
        private CleanFilesModel _cleanFilesModel;

        private ObservableCollection<CleaningCategoryItem> _cleaningCategoryItems;
        public RelayCommand<CleaningCategoryItem> CleanFilesCategoryItemCommand { get; set; }
        public CleanFilesViewModel()
        {
            _cleanFilesModel = new CleanFilesModel();
            _cleaningCategoryItems = _cleanFilesModel.GetAllCleaningCategoryItems();
            CleanFilesCategoryItemCommand = new RelayCommand<CleaningCategoryItem>(CleanFilesCategoryItem);
        }

        public ObservableCollection<CleaningCategoryItem> CleaningCategoryItems
        {
            get => _cleaningCategoryItems;
            set
            {
                _cleaningCategoryItems = value;
                OnPropertyChanged();
            }
        }

        private void CleanFilesCategoryItem(CleaningCategoryItem item)
        {
            foreach (var folder in item.Folders)
            {
                _cleanFilesModel.CleanFolder(folder);
            }
            _cleanFilesModel.SetCategoryFoldersSize(item);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
