﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace INPUTLAGFIX.Models
{
    public class OptimizationsCategoryItem
    {
        public string DisplayName {  get; set; }
        public BitmapImage Img { get; set; }
        public UserControl Control { get; set; }
    }
}
