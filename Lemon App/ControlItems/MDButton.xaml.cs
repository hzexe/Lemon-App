﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lemon_App
{
    /// <summary>
    /// MDButton.xaml 的交互逻辑
    /// </summary>
    public partial class MDButton : UserControl
    {
        public MDButton()
        {
            InitializeComponent();
        }
        public MDButton(bool ab=false)
        {
            InitializeComponent();
            this.ab = ab;
            if (ab) {
                bd.BorderBrush = new SolidColorBrush(Color.FromRgb(147,161,174));
                tb.Foreground= new SolidColorBrush(Color.FromRgb(147, 161, 174));
            }
        }
        private bool ab = false;
        public string TName { get => tb.Text; set => tb.Text = value; }
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            bd.SetResourceReference(BorderBrushProperty, "ThemeColor");
            tb.SetResourceReference(ForegroundProperty, "ThemeColor");
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ab)
            {
                bd.BorderBrush = new SolidColorBrush(Color.FromRgb(147, 161, 174));
                tb.Foreground = new SolidColorBrush(Color.FromRgb(147, 161, 174));
            }
            else
            {
                bd.SetResourceReference(BorderBrushProperty, "BorderColorBrush");
                tb.SetResourceReference(ForegroundProperty, "ResuColorBrush");
            }
        }
    }
}
