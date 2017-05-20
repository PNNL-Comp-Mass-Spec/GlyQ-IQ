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

namespace GlycolyzerGUImvvm.Views
{
    /// <summary>
    /// Interaction logic for OmniFinderGMRangesPage.xaml
    /// </summary>
    public partial class OmniFinderGMRangesPage : Page
    {
        public OmniFinderGMRangesPage()
        {
            InitializeComponent();
        }

        private void OmniFinderGMRangesPage_Loaded(object sender, RoutedEventArgs e)
        {
            RangesPage omniFinderGMRangesPage = new RangesPage();
            this.Content = omniFinderGMRangesPage.rangesPage_Canvas;
        }
    }
}
