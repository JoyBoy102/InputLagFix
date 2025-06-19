using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX.Models
{
    public class CleaningCategoryItem
    {
        public string DisplayName { get; set; }
        public List<string> Folders = new List<string>();
        public string AdditionalInfo { get; set; }
    }
}
