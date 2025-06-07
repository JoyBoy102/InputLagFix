using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class DeleteItem
    {
        public string UninstallString { get; set; }
        public string DisplayName { get; set; }

        public Uri LogoUri {  get; set; }

        public bool isUWP { get; set; }

        public string keyname;

        public string subkeyname;
    }
}
