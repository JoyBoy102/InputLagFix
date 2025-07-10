using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class Setting
    {
        public string valuePath;
        public string valueName;
        public object value;
        public RegistryValueKind valueKind;
        public bool isTask;
    }
}
