/* Written by Myanna Harris
* for the Department of Energy (PNNL, Richland, WA)
* Battelle Memorial Institute
* E-mail: Myanna.Harris@pnnl.gov
* Website: http://omics.pnl.gov/software
* -----------------------------------------------------
* 
 * Notice: This computer software was prepared by Battelle Memorial Institute,
* hereinafter the Contractor, under Contract No. DE-AC05-76RL0 1830 with the
* Department of Energy (DOE).  All rights in the computer software are reserved
* by DOE on behalf of the United States Government and the Contractor as
* provided in the Contract.
* 
 * NEITHER THE GOVERNMENT NOR THE CONTRACTOR MAKES ANY WARRANTY, EXPRESS OR
* IMPLIED, OR ASSUMES ANY LIABILITY FOR THE USE OF THIS SOFTWARE.
* 
 * This notice including this sentence must appear on any copies of this computer
* software.
* -----------------------------------------------------*/

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
using System.Threading;

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
