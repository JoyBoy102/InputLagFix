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
        private BackupsModel _backupsModel;
        public IRelayCommand<BackupItem> ApplyBackupCommand { get; set; }

        public RelayCommand CreateBackupItemCommand { get; set; }

        public BackupsViewModel()
        {
            _backupsModel = new BackupsModel();
            CreateBackupItemCommand = new RelayCommand(CreateBackupItem);
            ApplyBackupCommand = new RelayCommand<BackupItem>(ApplyBackup);
        }
        public ObservableCollection<BackupItem> BackupItems
        {
            get => _backupsModel.BackupItems;
            set
            {
                _backupsModel.BackupItems = value;
                OnPropertyChanged();
            }
        }

        private void CreateBackupItem()
        {
            _backupsModel.BackupItems.Add(new BackupItem());
        }

        private void ApplyBackup(BackupItem backupItem)
        {
            backupItem.ApplyBackup();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
