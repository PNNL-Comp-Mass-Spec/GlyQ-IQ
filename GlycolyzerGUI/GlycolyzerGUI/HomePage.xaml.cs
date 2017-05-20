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
//using System.IO;

namespace GlycolyzerGUI
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void utilitiesOmniFinderButton_Click(object sender, RoutedEventArgs e)
        {
            // View OmniFinder With GlycanMaker Page
            if (App.initializePages.OmniFinder_InitializeFlag == true)
            {
                App.utilitiesOmniFinderPage = new UtilitiesOmniFinderPage();
                App.initializePages.OmniFinder_InitializeFlag = false;
            }
            this.NavigationService.Navigate(App.utilitiesOmniFinderPage);
        }

        private void parametersButton_Click(object sender, RoutedEventArgs e)
        {
            App.initializePages.CheckTab_InitializeFlag = false;

            // View ParametersSubHomePage
            if (App.initializePages.Parameters_InitializeFlag == true)
            {
                App.parametersSubHomePage = new ParametersSubHomePage();
                App.initializePages.Parameters_InitializeFlag = false;
            }
            App.parametersSubHomePage.InitializeParametersPage();
            this.NavigationService.Navigate(App.parametersSubHomePage);
        }
    }
}
