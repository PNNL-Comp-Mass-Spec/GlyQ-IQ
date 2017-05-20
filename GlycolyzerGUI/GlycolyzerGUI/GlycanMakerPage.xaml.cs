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
using GetPeaks_DLL.Objects;
using OmniFinder.Objects.Enumerations;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects;
using OmniFinder.Objects.BuildingBlocks;
using PNNLOmics.Data.Constants.Libraries;
using OmniFinder;

namespace GlycolyzerGUI
{
    /// <summary>
    /// Interaction logic for GlycanMakerPage.xaml
    /// </summary>
    public partial class GlycanMakerPage : Page
    {
        /*public GlycanMakerPage()
        {
            InitializeComponent();
        }

        private void numberOfHexoseTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int tempText;
            TextBox changedTextBox = (TextBox)sender;
            string Str = changedTextBox.Text.Trim();
            int Num;
            bool isNum = int.TryParse(Str, out Num);
            if (isNum)
                tempText = Convert.ToInt32(((TextBox)sender).Text);
            else if (changedTextBox.Text == "")
                tempText = 0;
            else
            {
                MessageBox.Show("Invalid number");
                tempText = 0;
            }

            switch (changedTextBox.Name)
            {
                //Number of Residues
                case "numberOfHexoseTextBox":
                    {
                        App.glycanMakerVariables.NumberOfHexose_Int = tempText;
                        numberOfHexoseTextBox.Text = (App.glycanMakerVariables.NumberOfHexose_Int).ToString();
                        break;
                    }
                case "numberOfHexNAcTextBox":
                    {
                        App.glycanMakerVariables.NumberOfHexNAc_Int = tempText;
                        numberOfHexNAcTextBox.Text = (App.glycanMakerVariables.NumberOfHexNAc_Int).ToString();
                        break;
                    }
                case "numberOfDeoxyhexoseTextBox":
                    {
                        App.glycanMakerVariables.NumberOfDeoxyhexose_Int = tempText;
                        numberOfDeoxyhexoseTextBox.Text = (App.glycanMakerVariables.NumberOfDeoxyhexose_Int).ToString();
                        break;
                    }
                case "numberOfUserUnitATextBox":
                    {
                        App.glycanMakerVariables.NumberOfUserUnit1_Int = tempText;
                        numberOfUserUnitATextBox.Text = (App.glycanMakerVariables.NumberOfUserUnit1_Int).ToString();
                        break;
                    }
                case "numberOfUserUnitBTextBox":
                    {
                        App.glycanMakerVariables.NumberOfUserUnit2_Int = tempText;
                        numberOfUserUnitBTextBox.Text = (App.glycanMakerVariables.NumberOfUserUnit2_Int).ToString();
                        break;
                    }                                                                                                        
                case "numberOfNeuGcTextBox":
                    {
                        App.glycanMakerVariables.NumberOfNeuGc_Int = tempText;
                        numberOfNeuGcTextBox.Text = (App.glycanMakerVariables.NumberOfNeuGc_Int).ToString();
                        break;
                    }
                case "numberOfKDNTextBox":                                                                                   
                    {
                        App.glycanMakerVariables.NumberOfKDN_Int = tempText;
                        numberOfKDNTextBox.Text = (App.glycanMakerVariables.NumberOfKDN_Int).ToString();
                        break;                                                                                               
                    }
                case "numberOfHexATextBox":
                    {                                                                                                        
                        App.glycanMakerVariables.NumberOfHexA_Int = tempText;
                        numberOfHexATextBox.Text = (App.glycanMakerVariables.NumberOfHexA_Int).ToString();                      
                        break;
                    }
                case "numberOfPentoseTextBox":
                    {
                        App.glycanMakerVariables.NumberOfPentose_Int = tempText;
                        numberOfPentoseTextBox.Text = (App.glycanMakerVariables.NumberOfPentose_Int).ToString();
                        break;
                    }
                case "numberOfNeuAcTextBox":                                                                                 
                    {
                        App.glycanMakerVariables.NumberOfNeuAc_Int = tempText;
                        numberOfNeuAcTextBox.Text = (App.glycanMakerVariables.NumberOfNeuAc_Int).ToString();
                        break;
                    }

                //Charge Carrier
                case "numberOfChargeCarrierTextBox":
                    {
                        if (tempText == 0)
                            App.glycanMakerVariables.NumberOfChargeCarrier_Int = 1;
                        else
                            App.glycanMakerVariables.NumberOfChargeCarrier_Int = tempText;
                        numberOfChargeCarrierTextBox.Text = (App.glycanMakerVariables.NumberOfChargeCarrier_Int).ToString();
                        break;
                    }*/

                /*//Number of C, H, O, N, Na   
                case "amountOfCTextBox":
                    {
                        App.glycanMakerVariables.NumberOfC_Int = tempText;
                        amountOfCTextBox.Text = (App.glycanMakerVariables.NumberOfC_Int).ToString();
                        break;
                    }
                case "amountOfHTextBox":
                    {
                        App.glycanMakerVariables.NumberOfH_Int = tempText;
                        amountOfHTextBox.Text = (App.glycanMakerVariables.NumberOfH_Int).ToString();
                        break;
                    }
                case "amountOfOTextBox":
                    {
                        App.glycanMakerVariables.NumberOfO_Int = tempText;
                        amountOfOTextBox.Text = (App.glycanMakerVariables.NumberOfO_Int).ToString();
                        break;
                    }
                case "amountOfNTextBox":
                    {
                        App.glycanMakerVariables.NumberOfN_Int = tempText;
                        amountOfNTextBox.Text = (App.glycanMakerVariables.NumberOfN_Int).ToString();
                        break;
                    }
                case "amountOfNaTextBox":
                    {
                        App.glycanMakerVariables.NumberOfNa_Int = tempText;
                        amountOfNaTextBox.Text = (App.glycanMakerVariables.NumberOfNa_Int).ToString();
                        break;
                    }*//*
            }   
        }

        private void massOfUserUnitATextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double tempText;
            TextBox changedTextBox = (TextBox)sender;
            string Str = changedTextBox.Text.Trim();
            double Num;
            bool isNum = double.TryParse(Str, out Num);
            if (isNum)
                tempText = Convert.ToDouble(((TextBox)sender).Text);
            else if (changedTextBox.Text == "")
                tempText = 0.0;
            else
            {
                MessageBox.Show("Invalid number");
                tempText = 0.0;
            }

            switch (changedTextBox.Name)
            {
                //Mass of User Units A and B
                case "massOfUserUnitATextBox":
                    {
                        App.glycanMakerVariables.MassOfUserUnit1_Double = tempText;
                        massOfUserUnitATextBox.Text = (App.glycanMakerVariables.MassOfUserUnit1_Double).ToString();
                        break;
                    }
                case "massOfUserUnitBTextBox":
                    {
                        App.glycanMakerVariables.MassOfUserUnit2_Double = tempText;
                        massOfUserUnitBTextBox.Text = (App.glycanMakerVariables.MassOfUserUnit2_Double).ToString();
                        break;
                    }*/
                /*//Neutral Mass                                                    
                case "neutralMassTextBox":
                    {
                        App.glycanMakerVariables.NeutralMass_Double = Convert.ToDouble(tempText);
                        neutralMassTextBox.Text = (App.glycanMakerVariables.NeutralMass_Double).ToString();
                        break;
                    }

                //Mass/Charge 
                case "massChargeTextBox":
                    {
                        App.glycanMakerVariables.MassCharge_Double = Convert.ToDouble(tempText);
                        massChargeTextBox.Text = (App.glycanMakerVariables.MassCharge_Double).ToString();
                        break;
                    }*//*
            }
        }

        private void chargeCarrierGlycanMakerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox changedComboBox = (ComboBox)sender;
            switch (changedComboBox.Name)
            {
                //Charge Carrier 
                case "chargeCarrierGlycanMakerComboBox":
                    {
                        App.glycanMakerVariables.TypeOfChargeCarrier_String = (chargeCarrierGlycanMakerComboBox.SelectedValue.ToString()).Substring(38);
                        if (App.glycanMakerVariables.TypeOfChargeCarrier_String != "")
                        {
                            //Finds Selected Option To Be Displayed When There Is Saved Information
                            int index = -1;
                            string[] s = new string[9];
                            s[0] = "H";
                            s[1] = "Na";
                            s[2] = "K";
                            s[3] = "-H";
                            s[4] = "NH4";
                            s[5] = "Water";
                            s[6] = "Neutral";
                            s[7] = "UserA";
                            s[8] = "UserB";
                            // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                            for (int j = 0; j < s.Length; j++)
                            {
                                if (s[j] == (App.glycanMakerVariables.TypeOfChargeCarrier_String))
                                    index = j;
                            }
                            //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                            chargeCarrierGlycanMakerComboBox.SelectedIndex = index;
                        }
                        else
                            chargeCarrierGlycanMakerComboBox.SelectedIndex = 0;
                        break;
                    }
                //Carbohydrate Type
                case "carbohydrateTypeGlycanMakerComboBox":
                    {
                        App.glycanMakerVariables.CarbohydrateType_String = (carbohydrateTypeGlycanMakerComboBox.SelectedValue.ToString()).Substring(38);
                        if (App.glycanMakerVariables.CarbohydrateType_String != "")
                        {
                            //Finds Selected Option To Be Displayed When There Is Saved Information
                            int index = -1;
                            string[] s = new string[3];
                            s[0] = "Aldehyde";
                            s[1] = "Alditol";
                            s[2] = "Fragment";
                            // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                            for (int j = 0; j < s.Length; j++)
                            {
                                if (s[j] == (App.glycanMakerVariables.CarbohydrateType_String))
                                    index = j;
                            }
                            //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                            carbohydrateTypeGlycanMakerComboBox.SelectedIndex = index;
                        }
                        else
                            carbohydrateTypeGlycanMakerComboBox.SelectedIndex = 0;
                        break;
                    } 
            }
        }                                                                                         
                                                                                                                                      
        private void calculateMassButton_Click(object sender, RoutedEventArgs e)                                                      
        {
            GlycanMakerObject setMeUp = new GlycanMakerObject();

            if (App.glycanMakerVariables.NumberOfChargeCarrier_Int == 0)
                setMeUp.Charge = 1;
            else
                setMeUp.Charge = App.glycanMakerVariables.NumberOfChargeCarrier_Int;
            setMeUp.MassTollerance = 0; //place holder since we do not need it for the glycan maker

            switch(App.glycanMakerVariables.TypeOfChargeCarrier_String)
            {
                case "H":
                    {
                        setMeUp.ChargeCarryingAdduct = Adducts.H;
                        break;
                    }
                case "Na":
                    {
                        setMeUp.ChargeCarryingAdduct = Adducts.Na;
                        break;
                    }
                case "K":
                    {
                        setMeUp.ChargeCarryingAdduct = Adducts.K;
                        break;
                    }
                case "-H":
                    {
                        setMeUp.ChargeCarryingAdduct = Adducts.DeProtonated;
                        break;
                    }
                case "NH4":
                    {
                        setMeUp.ChargeCarryingAdduct = Adducts.NH4;
                        break;
                    }
                case "Water":
                    {
                        setMeUp.ChargeCarryingAdduct = Adducts.Water;
                        break;
                    }
                case "Neutral":
                    {
                        setMeUp.ChargeCarryingAdduct = Adducts.Neutral;
                        break;
                    }
                case "UserA":
                    {
                        setMeUp.ChargeCarryingAdduct = Adducts.UserDefined;
                        break;
                    }
                case "UserB":
                    {
                        setMeUp.ChargeCarryingAdduct = Adducts.UserDefined;
                        break;
                    }
            }

            switch (App.glycanMakerVariables.CarbohydrateType_String)
            {
                case "Aldehyde":
                    {
                        setMeUp.CarbohydrateType = CarbType.Aldehyde;
                        break;
                    }
                case "Alditol":
                    {
                        setMeUp.CarbohydrateType = CarbType.Alditol;
                        break;
                    }
                case "Fragment":
                    {
                        setMeUp.CarbohydrateType = CarbType.Fragment;
                        break;
                    }
            }

            BuildingBlock hexoseBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Hexose,
                new RangesMinMax(App.glycanMakerVariables.NumberOfHexose_Int, App.glycanMakerVariables.NumberOfHexose_Int));
            BuildingBlock deoxyhexoseBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Deoxyhexose,
                new RangesMinMax(App.glycanMakerVariables.NumberOfDeoxyhexose_Int, App.glycanMakerVariables.NumberOfDeoxyhexose_Int));
            BuildingBlock hexNAcBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.HexuronicAcid,
                new RangesMinMax(App.glycanMakerVariables.NumberOfHexNAc_Int, App.glycanMakerVariables.NumberOfHexNAc_Int));
            BuildingBlock pentoseBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.Pentose,
                new RangesMinMax(App.glycanMakerVariables.NumberOfPentose_Int, App.glycanMakerVariables.NumberOfPentose_Int));
            BuildingBlock neuAcBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.NeuraminicAcid,
                new RangesMinMax(App.glycanMakerVariables.NumberOfNeuAc_Int, App.glycanMakerVariables.NumberOfNeuAc_Int));
            BuildingBlock neuGcBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.NGlycolylneuraminicAcid,
                new RangesMinMax(App.glycanMakerVariables.NumberOfNeuGc_Int, App.glycanMakerVariables.NumberOfNeuGc_Int));
            BuildingBlock kdnBlock = new BuildingBlockMonoSaccharide(MonosaccharideName.KDN,
                new RangesMinMax(App.glycanMakerVariables.NumberOfKDN_Int, App.glycanMakerVariables.NumberOfKDN_Int));
            BuildingBlock hexABlock = new BuildingBlockMonoSaccharide(MonosaccharideName.HexuronicAcid,
                new RangesMinMax(App.glycanMakerVariables.NumberOfHexA_Int, App.glycanMakerVariables.NumberOfHexA_Int));

            double mass1 = Convert.ToDouble(App.glycanMakerVariables.NumberOfUserUnit1_Int);
            double mass2 = Convert.ToDouble(App.glycanMakerVariables.NumberOfUserUnit2_Int);

            UserUnitLibrary myLibrary = new UserUnitLibrary();
            UserUnit unit1 = new UserUnit("user1", "u1", mass1,UserUnitName.User01);
            UserUnit unit2 = new UserUnit("user2", "u2", mass2,UserUnitName.User02);
            UserUnit unit3 = new UserUnit("user3", "u3", 100,UserUnitName.User03);
            myLibrary.SetLibrary(unit1, unit2, unit3);
            setMeUp.OmniFinderParameter.UserUnitLibrary = myLibrary;
                
            setMeUp.LegoBuildingBlocks.Add(hexoseBlock);
            setMeUp.LegoBuildingBlocks.Add(deoxyhexoseBlock);
            setMeUp.LegoBuildingBlocks.Add(hexNAcBlock);
            setMeUp.LegoBuildingBlocks.Add(pentoseBlock);
            setMeUp.LegoBuildingBlocks.Add(neuAcBlock);
            setMeUp.LegoBuildingBlocks.Add(neuGcBlock);
            setMeUp.LegoBuildingBlocks.Add(kdnBlock);
            setMeUp.LegoBuildingBlocks.Add(hexABlock);

            GlycanMakerObject inputForGlycanMaker = setMeUp;
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(inputForGlycanMaker, inputForGlycanMaker.Charge);
            
            //Neutral Mass get, set, output
            App.glycanMakerVariables.NeutralMass_Double = Convert.ToDouble(results.MassNeutral);
            neutralMassTextBox.Text = (App.glycanMakerVariables.NeutralMass_Double).ToString();

            //Mass/Charge get, set, output
            App.glycanMakerVariables.MassCharge_Double = Convert.ToDouble(results.MassToCharge);
            massChargeTextBox.Text = (App.glycanMakerVariables.MassCharge_Double).ToString();
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetGlycanMaker();
        }

        public void ResetGlycanMaker()
        {
            //Number of Residues
            App.glycanMakerVariables.NumberOfHexose_Int = 0;
            App.glycanMakerVariables.NumberOfHexNAc_Int = 0;
            App.glycanMakerVariables.NumberOfDeoxyhexose_Int = 0;
            App.glycanMakerVariables.NumberOfUserUnit1_Int = 0;
            App.glycanMakerVariables.NumberOfUserUnit2_Int = 0;
            App.glycanMakerVariables.NumberOfNeuGc_Int = 0;
            App.glycanMakerVariables.NumberOfKDN_Int = 0;
            App.glycanMakerVariables.NumberOfHexA_Int = 0;
            App.glycanMakerVariables.NumberOfPentose_Int = 0;
            App.glycanMakerVariables.NumberOfNeuAc_Int = 0;

            //Mass of User Units A and B
            App.glycanMakerVariables.MassOfUserUnit1_Double = 0.0;
            App.glycanMakerVariables.MassOfUserUnit2_Double = 0.0;

            //Charge Carrier
            App.glycanMakerVariables.TypeOfChargeCarrier_String = "H";
            App.glycanMakerVariables.NumberOfChargeCarrier_Int = 1;

            //Carbohydrate Type
            App.glycanMakerVariables.CarbohydrateType_String = "Aldehyde";

            //Neutral Mass
            App.glycanMakerVariables.NeutralMass_Double = 0.0;

            //Mass/Charge
            App.glycanMakerVariables.MassCharge_Double = 0.0;

            //Number of C, H, O, N, Na
            App.glycanMakerVariables.NumberOfC_Int = 0;
            App.glycanMakerVariables.NumberOfH_Int = 0;
            App.glycanMakerVariables.NumberOfO_Int = 0;
            App.glycanMakerVariables.NumberOfN_Int = 0;
            App.glycanMakerVariables.NumberOfNa_Int = 0;

            InitializeGlycanMaker();
        }

        public void InitializeGlycanMaker()
        {
            //Number of Residues
            numberOfHexoseTextBox.Text = (App.glycanMakerVariables.NumberOfHexose_Int).ToString();
            numberOfHexNAcTextBox.Text = (App.glycanMakerVariables.NumberOfHexNAc_Int).ToString();
            numberOfDeoxyhexoseTextBox.Text = (App.glycanMakerVariables.NumberOfDeoxyhexose_Int).ToString();
            numberOfUserUnitATextBox.Text = (App.glycanMakerVariables.NumberOfUserUnit1_Int).ToString();
            numberOfUserUnitBTextBox.Text = (App.glycanMakerVariables.NumberOfUserUnit2_Int).ToString();
            numberOfNeuGcTextBox.Text = (App.glycanMakerVariables.NumberOfNeuGc_Int).ToString();
            numberOfKDNTextBox.Text = (App.glycanMakerVariables.NumberOfKDN_Int).ToString();
            numberOfHexATextBox.Text = (App.glycanMakerVariables.NumberOfHexA_Int).ToString();
            numberOfPentoseTextBox.Text = (App.glycanMakerVariables.NumberOfPentose_Int).ToString();
            numberOfNeuAcTextBox.Text = (App.glycanMakerVariables.NumberOfNeuAc_Int).ToString();

            //Mass of User Units A and B
            massOfUserUnitATextBox.Text = (App.glycanMakerVariables.MassOfUserUnit1_Double).ToString();
            massOfUserUnitBTextBox.Text = (App.glycanMakerVariables.MassOfUserUnit2_Double).ToString();

            //Charge Carrier
            numberOfChargeCarrierTextBox.Text = (App.glycanMakerVariables.NumberOfChargeCarrier_Int).ToString();
            if (App.glycanMakerVariables.TypeOfChargeCarrier_String != "")
            {
                //Finds Selected Option To Be Displayed When There Is Saved Information
                int index = -1;
                string[] s = new string[9];
                s[0] = "H";
                s[1] = "Na";
                s[2] = "K";
                s[3] = "-H";
                s[4] = "NH4";
                s[5] = "Water";
                s[6] = "Neutral";
                s[7] = "UserA";
                s[8] = "UserB";
                // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                for (int j = 0; j < s.Length; j++)
                {
                    if (s[j] == (App.glycanMakerVariables.TypeOfChargeCarrier_String))
                        index = j;
                }
                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                chargeCarrierGlycanMakerComboBox.SelectedIndex = index;
            }
            else
                chargeCarrierGlycanMakerComboBox.SelectedIndex = 0;

            //Carbohydrate Type
            if (App.glycanMakerVariables.CarbohydrateType_String != "")
            {
                //Finds Selected Option To Be Displayed When There Is Saved Information
                int index = -1;
                string[] s = new string[3];
                s[0] = "Aldehyde";
                s[1] = "Alditol";
                s[2] = "Fragment";
                // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                for (int j = 0; j < s.Length; j++)
                {
                    if (s[j] == (App.glycanMakerVariables.CarbohydrateType_String))
                        index = j;
                }
                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                carbohydrateTypeGlycanMakerComboBox.SelectedIndex = index;
            }
            else
                carbohydrateTypeGlycanMakerComboBox.SelectedIndex = 0;

            //Neutral Mass
            neutralMassTextBox.Text = (App.glycanMakerVariables.NeutralMass_Double).ToString();

            //Mass/Charge
            massChargeTextBox.Text = (App.glycanMakerVariables.MassCharge_Double).ToString();

            //Number of C, H, O, N, Na
            amountOfCTextBox.Text = (App.glycanMakerVariables.NumberOfC_Int).ToString();
            amountOfHTextBox.Text = (App.glycanMakerVariables.NumberOfH_Int).ToString();
            amountOfOTextBox.Text = (App.glycanMakerVariables.NumberOfO_Int).ToString();
            amountOfNTextBox.Text = (App.glycanMakerVariables.NumberOfN_Int).ToString();
            amountOfNaTextBox.Text = (App.glycanMakerVariables.NumberOfNa_Int).ToString();
        }*/
    }
}