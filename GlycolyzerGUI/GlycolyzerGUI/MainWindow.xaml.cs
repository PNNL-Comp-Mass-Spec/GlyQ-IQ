using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GlycolyzerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    { 
        public MainWindow()
        {
            InitializeComponent();
        }

        //makes the window fill the screen when F11 is pressed
        private void Window_KeyDown(object sender, KeyEventArgs e) 
        { 
            if (e.Key == Key.F11) 
            { 
                WindowStyle = WindowStyle.None; 
                WindowState = WindowState.Maximized; 
                ResizeMode = ResizeMode.NoResize; 
            } 
        }
    }
}
