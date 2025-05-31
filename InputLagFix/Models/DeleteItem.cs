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

        public DateTime InstallDate { get; set; }

        public bool isUWP { get; set; }

    }
}
