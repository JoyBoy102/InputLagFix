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
        private ObservableCollection<ScreenResolution> _screenResolutions;
        public ICommand FixMonitorResolutionsCommand { get; }

        public RegeditManagerViewModel()
        {
            _regeditManager = new RegeditManager();
            _screenResolutions = new ObservableCollection<ScreenResolution>()
            {
                new ScreenResolution { Name = "640x480 (VGA)", Width = 640, Height = 480 },
                new ScreenResolution { Name = "800x600 (SVGA)", Width = 800, Height = 600 },
                new ScreenResolution { Name = "1024x768 (XGA)", Width = 1024, Height = 768 },
                new ScreenResolution { Name = "1152x864 (XGA+)", Width = 1152, Height = 864 },
                new ScreenResolution { Name = "1280x960 (SXGA-)", Width = 1280, Height = 960 },
                new ScreenResolution { Name = "1400x1050 (SXGA+)", Width = 1400, Height = 1050 },
                new ScreenResolution { Name = "1600x1200 (UXGA)", Width = 1600, Height = 1200 },

                // Широкоформатные (16:9, 16:10)
                new ScreenResolution { Name = "1280x720 (HD)", Width = 1280, Height = 720 },
                new ScreenResolution { Name = "1366x768 (HD)", Width = 1366, Height = 768 },
                new ScreenResolution { Name = "1600x900 (HD+)", Width = 1600, Height = 900 },
                new ScreenResolution { Name = "1920x1080 (Full HD)", Width = 1920, Height = 1080 },
                new ScreenResolution { Name = "2560x1440 (QHD / 2K)", Width = 2560, Height = 1440 },
                new ScreenResolution { Name = "3840x2160 (4K UHD)", Width = 3840, Height = 2160 },
                new ScreenResolution { Name = "7680x4320 (8K UHD)", Width = 7680, Height = 4320 },

                // Широкоформатные (16:10)
                new ScreenResolution { Name = "1280x800 (WXGA)", Width = 1280, Height = 800 },
                new ScreenResolution { Name = "1440x900 (WXGA+)", Width = 1440, Height = 900 },
                new ScreenResolution { Name = "1680x1050 (WSXGA+)", Width = 1680, Height = 1050 },
                new ScreenResolution { Name = "1920x1200 (WUXGA)", Width = 1920, Height = 1200 },
                new ScreenResolution { Name = "2560x1600 (WQXGA)", Width = 2560, Height = 1600 },

                // Ультраширокие (21:9, 32:9)
                new ScreenResolution { Name = "2560x1080 (UltraWide Full HD)", Width = 2560, Height = 1080 },
                new ScreenResolution { Name = "3440x1440 (UltraWide QHD)", Width = 3440, Height = 1440 },
                new ScreenResolution { Name = "3840x1600 (UltraWide 4K)", Width = 3840, Height = 1600 },
                new ScreenResolution { Name = "5120x1440 (Super UltraWide 5K)", Width = 5120, Height = 1440 },
                new ScreenResolution { Name = "3840x1080 (Super UltraWide 32:9)", Width = 3840, Height = 1080 },

                // Профессиональные (5K, 6K, 8K)
                new ScreenResolution { Name = "5120x2880 (5K)", Width = 5120, Height = 2880 },
                new ScreenResolution { Name = "6016x3384 (6K)", Width = 6016, Height = 3384 },
                new ScreenResolution { Name = "7680x4320 (8K UHD)", Width = 7680, Height = 4320 }
            };
            FixMonitorResolutionsCommand = new RelayCommand(FixMonitorResolutions);
            SelectedResolution = ScreenResolutions.First();
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

        public ObservableCollection<ScreenResolution> ScreenResolutions
        {
            get => _screenResolutions;
            set
            {
                _screenResolutions = value;
                OnPropertyChanged();
            }
        }

        public ScreenResolution SelectedResolution
        {
            get => _regeditManager.SelectedResolution;
            set
            {
                _regeditManager.SelectedResolution= value;
                OnPropertyChanged();
            }
        }

        private void FixMonitorResolutions()
        {
            _regeditManager.MonitorInputLagFix();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
