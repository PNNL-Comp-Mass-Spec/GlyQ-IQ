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

namespace GlycolyzerGUImvvm.Views
{
    /// <summary>
    /// Interaction logic for SavingPopUp.xaml
    /// </summary>
    public partial class SavingPopUp : Window
    {
        public SavingPopUp()
        {
            InitializeComponent();
        }

        public void FinishedSaving()
        {
            savingPopUpMessage.Content = "Application Parameters Saved Successfully";
        }
    }
}
