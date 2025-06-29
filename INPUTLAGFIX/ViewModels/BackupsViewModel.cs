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
    public class BackupsViewModel:INotifyPropertyChanged
    {
        private ObservableCollection<BackupItem> _backupItems;
        public RelayCommand CreateBackupItemCommand { get; set; }

        public BackupsViewModel()
        {
            _backupItems = new ObservableCollection<BackupItem>();
            CreateBackupItemCommand = new RelayCommand(CreateBackupItem);
        }
        public ObservableCollection<BackupItem> BackupItems
        {
            get => _backupItems;
            set
            {
                _backupItems = value;
                OnPropertyChanged();
            }
        }

        private void CreateBackupItem()
        {
            _backupItems.Add(new BackupItem());
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
