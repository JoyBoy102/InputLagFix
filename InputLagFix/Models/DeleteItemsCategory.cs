using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace INPUTLAGFIX.Models
{
    public class DeleteItemsCategory
    {
        public ObservableCollection<DeleteItem> RecentlyDeletedItems { get; set; }
        public ObservableCollection<DeleteItem> NotRecentlyDeletedItems { get; set; }

        public UserControl Control { get; set; }
        public string CategoryName { get; set; }

    }
}
