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
    /// Interaction logic for OmniFinderGMPage.xaml
    /// </summary>
    public partial class OmniFinderGMPage : Page
    {
        public OmniFinderGMPage()
        {
            InitializeComponent();
        }

        private void rangesButton_Click(object sender, RoutedEventArgs e)
        {
            App.omniFinderGMRangesPage = new OmniFinderGMRangesPage();
            this.NavigationService.Navigate(App.omniFinderGMRangesPage);
        }

        private void Hexose_UTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox changedTextBox = sender as TextBox;

            int tempInt = 0;

            string Str = changedTextBox.Text.Trim();
            int Num;
            bool isNum = int.TryParse(Str, out Num);

            if (isNum)
                tempInt = Convert.ToInt32(changedTextBox.Text);
            else if (changedTextBox.Text == "")
                tempInt = 0;
            else
                MessageBox.Show("Invalid number");

            switch (changedTextBox.Name)
            {
                case "Hexose_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfHexose_Int = tempInt;
                        break;
                    }
                case "HexNAc_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfHexNAc_Int = tempInt;
                        break;
                    }
                case "Deoxyhexose_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfDeoxyhexose_Int = tempInt;
                        break;
                    }
                case "Pentose_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfPentose_Int = tempInt;
                        break;
                    }
                case "NeuAc_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfNeuAc_Int = tempInt;
                        break;
                    }
                case "NeuGc_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfNeuGc_Int = tempInt;
                        break;
                    }
                case "KDN_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfKDN_Int = tempInt;
                        break;
                    }
                case "HexA_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfHexA_Int = tempInt;
                        break;
                    }
                case "UserUnitA_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfUserUnit1_Int = tempInt;
                        break;
                    }
                case "UserUnitB_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfUserUnit2_Int = tempInt;
                        break;
                    }
                case "naH_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfNaH_Int = tempInt;
                        break;
                    }
                case "cH3_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfCH3_Int = tempInt;
                        break;
                    }
                case "sO3_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfSO3_Int = tempInt;
                        break;
                    }
                case "OActyl_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfOAcetyl_Int = tempInt;
                        break;
                    }
                case "ala_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfAla_Int = tempInt;
                        break;
                    }
                case "arg_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfArg_Int = tempInt;
                        break;
                    }
                case "asn_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfAsn_Int = tempInt;
                        break;
                    }
                case "asp_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfAsp_Int = tempInt;
                        break;
                    }
                case "cys_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfCys_Int = tempInt;
                        break;
                    }
                case "gln_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfGln_Int = tempInt;
                        break;
                    }
                case "glu_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfGlu_Int = tempInt;
                        break;
                    }
                case "gly_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfGly_Int = tempInt;
                        break;
                    }
                case "his_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfHis_Int = tempInt;
                        break;
                    }
                case "ile_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfIle_Int = tempInt;
                        break;
                    }
                case "leu_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfLeu_Int = tempInt;
                        break;
                    }
                case "lys_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfLys_Int = tempInt;
                        break;
                    }
                case "met_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfMet_Int = tempInt;
                        break;
                    }
                case "phe_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfPhe_Int = tempInt;
                        break;
                    }
                case "ser_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfSer_Int = tempInt;
                        break;
                    }
                case "thr_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfThr_Int = tempInt;
                        break;
                    }
                case "trp_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfTrp_Int = tempInt;
                        break;
                    }
                case "tyr_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfTyr_Int = tempInt;
                        break;
                    }
                case "val_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfVal_Int = tempInt;
                        break;
                    }
                case "pro_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfPro_Int = tempInt;
                        break;
                    }
                case "pHex_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfpHex_Int = tempInt;
                        break;
                    }
                case "pHxNAc_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfpHxNAc_Int = tempInt;
                        break;
                    }
                case "pDxHex_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfpDxHex_Int = tempInt;
                        break;
                    }
                case "pPntos_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfpPntos_Int = tempInt;
                        break;
                    }
                case "pNuAc_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfpNuAc_Int = tempInt;
                        break;
                    }
                case "pNuGc_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfpNuGc_Int = tempInt;
                        break;
                    }
                case "pKDN_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfpKDN_Int = tempInt;
                        break;
                    }
                case "pHxA_UTextBox":
                    {
                        App.glycanMakerModel_Save.NumberOfpHxA_Int = tempInt;
                        break;
                    }
                case "ChargeCarrier_UTextBox":
                    {
                        if (tempInt == 0)
                            tempInt = 1;
                        App.glycanMakerModel_Save.NumberOfChargeCarrier_Int = tempInt;
                        break;
                    }
            }

            changedTextBox.Text = tempInt.ToString();
        }

        private void massOfUserUnitA_UTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox changedTextBox = sender as TextBox;

            double tempDouble = 0.0;

            string Str = changedTextBox.Text.Trim();
            double Num;
            bool isNum = double.TryParse(Str, out Num);

            if (isNum)
                tempDouble = Convert.ToDouble(changedTextBox.Text);
            else
                MessageBox.Show("Invalid number");

            switch (changedTextBox.Name)
            {
                case "massOfUserUnitA_UTextBox":
                    {
                        App.omniFinderGMRangesModel_Save.MassUserUnit1_Double = tempDouble;
                        break;
                    }
                case "massOfUserUnitB_UTextBox":
                    {
                        App.omniFinderGMRangesModel_Save.MassUserUnit2_Double = tempDouble;
                        break;
                    }
            }

            changedTextBox.Text = tempDouble.ToString();
        }
    }
}
