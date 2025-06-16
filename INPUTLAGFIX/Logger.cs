using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INPUTLAGFIX
{
    public class Logger
    {
        private static Logger _logger;

        public ObservableCollection<string> AllLogMessages = new ObservableCollection<string>();

        public static Logger GetLogger()
        {
            if (_logger  == null)
                _logger = new Logger();
            return _logger;
        }

    }
}
