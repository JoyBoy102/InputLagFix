using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace INPUTLAGFIX.Models
{
    public class Setting
    {
        public string valuePath { get; set; }
        public string valueName { get; set; }
        public object value { get; set; }
        public RegistryValueKind valueKind { get; set; }
        public bool isTask { get; set; }
    }
}
