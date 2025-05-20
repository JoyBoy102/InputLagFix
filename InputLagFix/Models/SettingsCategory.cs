using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class SettingsCategory
    {
        public string CategoryName { get; set; }
        public ObservableCollection<Optimization> Settings { get; set; }
    }
}
