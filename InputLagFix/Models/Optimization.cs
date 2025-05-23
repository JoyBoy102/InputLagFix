﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class Optimization
    {
        public string ruName { get; set; }
        public bool CheckedState { get; set; }
        public ObservableCollection<Setting> settings { get; set; }
        public void ApplyOptimization(ref RegeditManager regeditManager)
        {
            foreach (Setting setting in settings)
            {
                try
                {
                    if (setting.value_if_false.ToString() != "delete")
                    {
                        if (CheckedState == true)
                            regeditManager.AllLogMessages.Add(regeditManager.ChangeRegistryValue(setting.valuePath, setting.valueName, setting.value_if_true, setting.valueKind));
                        else
                            regeditManager.AllLogMessages.Add(regeditManager.ChangeRegistryValue(setting.valuePath, setting.valueName, setting.value_if_false, setting.valueKind));
                    }
                    else
                    {
                        if (CheckedState == true)
                            regeditManager.AllLogMessages.Add(regeditManager.ChangeRegistryValue(setting.valuePath, setting.valueName, setting.value_if_true, setting.valueKind));
                        else
                            regeditManager.AllLogMessages.Add(regeditManager.DeleteKey(setting.valueName, setting.valuePath));
                    }
                }
                catch
                {
                    if (setting.value_if_false.ToString() != "delete")
                    {
                        if (CheckedState == true)
                            regeditManager.AllLogMessages.Add(PowerRunManager.ApplyRegSettingWithPowerRun(setting.valuePath, setting.valueName, setting.value_if_true, setting.valueKind));
                        else
                            regeditManager.AllLogMessages.Add(PowerRunManager.ApplyRegSettingWithPowerRun(setting.valuePath, setting.valueName, setting.value_if_false, setting.valueKind));
                    }
                    else
                    {
                        if (CheckedState == true)
                            regeditManager.AllLogMessages.Add(PowerRunManager.ApplyRegSettingWithPowerRun(setting.valuePath, setting.valueName, setting.value_if_true, setting.valueKind));
                        else
                            regeditManager.AllLogMessages.Add(PowerRunManager.DeleteKeyWithPowerRun(setting.valueName, setting.valuePath));
                    }
                }
            }
        }
    }

}
