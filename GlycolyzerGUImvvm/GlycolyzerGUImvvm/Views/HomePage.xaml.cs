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
using GlycolyzerGUImvvm.ViewModels;

namespace GlycolyzerGUImvvm.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        HomeViewModel homeVM = new HomeViewModel();

        public HomePage()
        {
            InitializeComponent();

            Binding folderBinding = new Binding();
            folderBinding.Source = homeVM;
            folderBinding.Path = new PropertyPath("FolderModel.SaveAppParametersLocationFolder_String");
            folderBinding.Mode = BindingMode.TwoWay;
            appParametersFolderBrowseTextBox.SetBinding(TextBox.TextProperty, folderBinding);
        }

        private void OmniFinderGMButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.initializingFlagsModel.OmniFinderGMModel_InitializeFlag)
            {
                App.omniFinderGMPage = new OmniFinderGMPage();
                App.initializingFlagsModel.OmniFinderGMModel_InitializeFlag = false;
            }
            App.rangesBGColor = "DarkCyan";
            App.initializingFlagsModel.ParameterRangesSave_InitializeFlag = false;
            App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag = true;
            this.NavigationService.Navigate(App.omniFinderGMPage);
        }

        private void parametersButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.initializingFlagsModel.ParameterModel_InitializeFlag)
            {
                App.parameterPage = new ParameterPage();
                App.initializingFlagsModel.ParameterModel_InitializeFlag = false;
            }
            App.rangesBGColor = "PaleVioletRed";
            App.initializingFlagsModel.ParameterRangesSave_InitializeFlag = true;
            App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag = false;
            this.NavigationService.Navigate(App.parameterPage);
        }

        private void loadParametersButton_Click(object sender, RoutedEventArgs e)
        {
            GUIImport.ImportXMLParameterFiles("customLoad");
            
            App.initializingFlagsModel.OmniFinderGMModel_InitializeFlag = true;
            App.initializingFlagsModel.ParameterModel_InitializeFlag = true;
        }

        private void saveParametersButton_Click(object sender, RoutedEventArgs e)
        {
            GUIExport.ExportXMLParameterFiles("customSave");
        }
    }
}
