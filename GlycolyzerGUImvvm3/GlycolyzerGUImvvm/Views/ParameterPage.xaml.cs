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

namespace GlycolyzerGUImvvm.Views
{
    /// <summary>
    /// Interaction logic for ParameterPage.xaml
    /// </summary>
    public partial class ParameterPage : Page
    {
        RangesPage parameterRangesPage;
        public ParameterPage()
        {
            InitializeComponent();
        }

        private void Tab4_Loaded(object sender, RoutedEventArgs e)
        {
            parameterRangesPage = new RangesPage();
            Tab4.Content = parameterRangesPage.rangesPage_Canvas;
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.initializingFlagsModel.ParameterModel_InitializeFlag == false)
            {
                if (Tab4.IsSelected)
                {
                    App.initializingFlagsModel.Tab4_ResizeFlag = true;
                    parameterRangesPage = new RangesPage();
                    Tab4.Content = parameterRangesPage.rangesPage_Canvas;
                    
                }
                else
                {
                    App.initializingFlagsModel.Tab4_ResizeFlag = false;
                    App.parameterPage.Height = 432;
                    App.parameterPage.Width = 400;
                }
            }
        }

        private void mzToleranceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox changedTextBox = sender as TextBox;

            switch (changedTextBox.Name)
            {
                case "mzToleranceTextBox":
                    {
                        string Str = changedTextBox.Text.Trim();
                        double Num;
                        bool isNum = double.TryParse(Str, out Num);

                        if (isNum)
                            App.extraScienceParameterModel_Save.MzToleranceExtraParameter_Double = Convert.ToDouble(changedTextBox.Text);
                        else if (changedTextBox.Text == "")
                            App.extraScienceParameterModel_Save.MzToleranceExtraParameter_Double = 0.0;
                        else
                        {
                            MessageBox.Show("Invalid number");
                            App.extraScienceParameterModel_Save.MzToleranceExtraParameter_Double = 0.0;
                        }

                        changedTextBox.Text = App.extraScienceParameterModel_Save.MzToleranceExtraParameter_Double.ToString();
                        break;
                    }
                case "numberOfChargesTextBox":
                    {
                        string Str = changedTextBox.Text.Trim();
                        int Num;
                        bool isNum = int.TryParse(Str, out Num);

                        if (isNum)
                            App.extraScienceParameterModel_Save.NumberOfChargesExtraParameter_Int = Convert.ToInt32(changedTextBox.Text);
                        else if (changedTextBox.Text == "")
                            App.extraScienceParameterModel_Save.NumberOfChargesExtraParameter_Int = 1;
                        else
                        {
                            MessageBox.Show("Invalid number");
                            App.extraScienceParameterModel_Save.NumberOfChargesExtraParameter_Int = 1;
                        }

                        changedTextBox.Text = App.extraScienceParameterModel_Save.NumberOfChargesExtraParameter_Int.ToString();
                        break;
                    }
                case "inputDataFileBrowseTextBox":
                    {
                        if (App.folderModel_Save.InputDataFileType_String == "OmniFInder Mass")
                        {
                            double tempDouble = 0.0;

                            string Str = changedTextBox.Text.Trim();
                            double Num;
                            bool isNum = double.TryParse(Str, out Num);

                            if (isNum)
                                tempDouble = Convert.ToDouble(changedTextBox.Text);
                            else
                                MessageBox.Show("Invalid number");

                            App.omniFinderModel_Save.OmniFinderMass_String = tempDouble;
                        }
                        break;
                    }
            }
        }
    }
}
