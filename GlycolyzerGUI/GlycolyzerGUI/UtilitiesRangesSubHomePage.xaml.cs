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
    /// Interaction logic for UtilitiesRangesSubHomePage.xaml
    /// </summary>
    public partial class UtilitiesRangesSubHomePage : Page
    {
        public UtilitiesRangesSubHomePage()
        {
            InitializeComponent();
        }

        public static void GetPageObject(UtilitiesRangesSubHomePage utilitiesRangesSubHomePageP)
        {
            GetUtilitiesRangesPageDesign(utilitiesRangesSubHomePageP);
        }

        private static void GetUtilitiesRangesPageDesign(UtilitiesRangesSubHomePage utilitiesRangesSubHomePageP)
        {
            if (App.initializePages.UtilitiesRangesPage_InitializeFlag == true)
            {
                App.utilitiesRangesPage = new UtilitiesRangesPage(utilitiesRangesSubHomePageP);
                App.initializePages.UtilitiesRangesPage_InitializeFlag = false;
            }
            utilitiesRangesSubHomePageP.Content = App.utilitiesRangesPage.utilitiesRangesPage_Canvas;
        }
    }
}
