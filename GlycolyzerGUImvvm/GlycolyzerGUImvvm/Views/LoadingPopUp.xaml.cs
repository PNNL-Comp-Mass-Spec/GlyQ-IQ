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
using System.Windows.Shapes;
using System.Threading;

namespace GlycolyzerGUImvvm.Views
{
    /// <summary>
    /// Interaction logic for LoadingPopUp.xaml
    /// </summary>
    public partial class LoadingPopUp : Window
    {
        public LoadingPopUp()
        {
            InitializeComponent();
        }

        public void FinishedLoading()
        {
            PopUpMessage.Content = "Parameters Loaded Successfully";
        }
    }
}
