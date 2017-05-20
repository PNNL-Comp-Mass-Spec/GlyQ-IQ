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
    /// Interaction logic for OmniFinderPage.xaml
    /// </summary>
    public partial class UtilitiesOmniFinderPage : Page
    {
        public UtilitiesOmniFinderPage()
        {
            InitializeComponent();
            if (App.initializePages.UtilitiesOmniFinderPageDesign_InitializeFlag == true)
            {
                App.utilitiesSpecialOmniFinderPageDesign = new OmniFinderWithGlycanMaker();
                App.initializePages.UtilitiesOmniFinderPageDesign_InitializeFlag = false;
            }
            this.Content = App.utilitiesSpecialOmniFinderPageDesign.utilitiesOmniFinderPage_Canvas;
        }

        public void GoToRangesPage()
        {
            if (App.initializePages.UtilitiesRangesSubHomePage_InitializeFlag == true)
            {
                App.utilitiesRangesSubHomePage = new UtilitiesRangesSubHomePage();
                UtilitiesRangesSubHomePage.GetPageObject(App.utilitiesRangesSubHomePage);
                App.initializePages.UtilitiesRangesSubHomePage_InitializeFlag = false;
            }
            App.utilitiesRangesPage.UtilitiesRangesPage_Initialization();
            this.NavigationService.Navigate(App.utilitiesRangesSubHomePage);
        }
    }
}