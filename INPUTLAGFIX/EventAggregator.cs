using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX
{
    public class EventAggregator
    {
        public event Action SettingsStartApplying;
        public event Action SettingsStopApplying;

        public void ShowLoadingUI()
        {
            SettingsStartApplying?.Invoke();
        }
        public void CollapseLoadingUI()
        {
            SettingsStopApplying?.Invoke();
        }
    }
}
