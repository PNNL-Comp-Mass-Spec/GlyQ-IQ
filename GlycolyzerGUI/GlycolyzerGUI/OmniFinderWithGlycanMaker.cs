using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using OmniFinder.Objects.BuildingBlocks;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;
using OmniFinder.Objects;
using PNNLOmics.Data.Constants.Libraries;
using OmniFinder;

namespace GlycolyzerGUI
{
    public class OmniFinderWithGlycanMaker
    {
        #region Variables
        //Canvas to Draw the Box Outlines On
        public Canvas utilitiesOmniFinderPage_Canvas = new Canvas();

        //Grid to Organize Design
        Grid utilitiesOmniFinderPage_Grid = new Grid();

        //MiniGrid in 2nd Row
        Grid utilitiesOmniFinderPage_MiniGrid = new Grid();

        //ComboBoxes
        ComboBox selectOptions_UComboBox = new ComboBox();
        ComboBox chargeCarrier_UComboBox = new ComboBox();
        ComboBox carbohydrateType_UComboBox = new ComboBox();


        //CheckBoxes
        //Sugars Checkboxes
        CheckBox hexose_UCheckBox = new CheckBox();
        CheckBox hexNAc_UCheckBox = new CheckBox();
        CheckBox dxyHex_UCheckBox = new CheckBox();
        CheckBox pentose_UCheckBox = new CheckBox();
        CheckBox neuAc_UCheckBox = new CheckBox();
        CheckBox neuGc_UCheckBox = new CheckBox();
        CheckBox kDN_UCheckBox = new CheckBox();
        CheckBox hexA_UCheckBox = new CheckBox();
        
        //User Text Box
        CheckBox userUnitA_UCheckBox = new CheckBox();
        CheckBox userUnitB_UCheckBox = new CheckBox();
        
        //Special Check Boxes
        CheckBox naH_UCheckBox = new CheckBox();
        CheckBox cH3_UCheckBox = new CheckBox();
        CheckBox sO3_UCheckBox = new CheckBox();
        CheckBox oAcetyl_UCheckBox = new CheckBox();

        //Amino Acid Option Box Check Boxes
        CheckBox ala_UCheckBox = new CheckBox();
        CheckBox arg_UCheckBox = new CheckBox();
        CheckBox asn_UCheckBox = new CheckBox();
        CheckBox asp_UCheckBox = new CheckBox();
        CheckBox cys_UCheckBox = new CheckBox();
        CheckBox gln_UCheckBox = new CheckBox();
        CheckBox glu_UCheckBox = new CheckBox();
        CheckBox gly_UCheckBox = new CheckBox();
        CheckBox his_UCheckBox = new CheckBox();
        CheckBox ile_UCheckBox = new CheckBox();
        CheckBox leu_UCheckBox = new CheckBox();
        CheckBox lys_UCheckBox = new CheckBox();
        CheckBox met_UCheckBox = new CheckBox();
        CheckBox phe_UCheckBox = new CheckBox();
        CheckBox ser_UCheckBox = new CheckBox();
        CheckBox thr_UCheckBox = new CheckBox();
        CheckBox trp_UCheckBox = new CheckBox();
        CheckBox tyr_UCheckBox = new CheckBox();
        CheckBox val_UCheckBox = new CheckBox();
        CheckBox pro_UCheckBox = new CheckBox();

        //Permethyl Check Boxes
        CheckBox pHex_UCheckBox = new CheckBox();
        CheckBox pHxNAc_UCheckBox = new CheckBox();
        CheckBox pDxHex_UCheckBox = new CheckBox();
        CheckBox pPntos_UCheckBox = new CheckBox();
        CheckBox pNuAc_UCheckBox = new CheckBox();
        CheckBox pNuGc_UCheckBox = new CheckBox();
        CheckBox pKDN_UCheckBox = new CheckBox();
        CheckBox pHxA_UCheckBox = new CheckBox();

       
        //TextBoxes
        //Sugars TextBoxes
        TextBox hexose_UTextBox = new TextBox();
        TextBox hexNAc_UTextBox = new TextBox();
        TextBox dxyHex_UTextBox = new TextBox();
        TextBox pentose_UTextBox = new TextBox();
        TextBox neuAc_UTextBox = new TextBox();
        TextBox neuGc_UTextBox = new TextBox();
        TextBox kDN_UTextBox = new TextBox();
        TextBox hexA_UTextBox = new TextBox();
        
        //User TextBoxes
        TextBox userUnitA_UTextBox = new TextBox();
        TextBox userUnitB_UTextBox = new TextBox();
        TextBox massOfUserUnitA_UTextBox = new TextBox();
        TextBox massOfUserUnitB_UTextBox = new TextBox();
        
        //Special TextBoxes
        TextBox naH_UTextBox = new TextBox();
        TextBox cH3_UTextBox = new TextBox();
        TextBox sO3_UTextBox = new TextBox();
        TextBox oAcetyl_UTextBox = new TextBox();

        //Amino Acid Option Box Check Boxes
        TextBox ala_UTextBox = new TextBox();
        TextBox arg_UTextBox = new TextBox();
        TextBox asn_UTextBox = new TextBox();
        TextBox asp_UTextBox = new TextBox();
        TextBox cys_UTextBox = new TextBox();
        TextBox gln_UTextBox = new TextBox();
        TextBox glu_UTextBox = new TextBox();
        TextBox gly_UTextBox = new TextBox();
        TextBox his_UTextBox = new TextBox();
        TextBox ile_UTextBox = new TextBox();
        TextBox leu_UTextBox = new TextBox();
        TextBox lys_UTextBox = new TextBox();
        TextBox met_UTextBox = new TextBox();
        TextBox phe_UTextBox = new TextBox();
        TextBox ser_UTextBox = new TextBox();
        TextBox thr_UTextBox = new TextBox();
        TextBox trp_UTextBox = new TextBox();
        TextBox tyr_UTextBox = new TextBox();
        TextBox val_UTextBox = new TextBox();
        TextBox pro_UTextBox = new TextBox();

        //Permethyl Check Boxes
        TextBox pHex_UTextBox = new TextBox();
        TextBox pHxNAc_UTextBox = new TextBox();
        TextBox pDxHex_UTextBox = new TextBox();
        TextBox pPntos_UTextBox = new TextBox();
        TextBox pNuAc_UTextBox = new TextBox();
        TextBox pNuGc_UTextBox = new TextBox();
        TextBox pKDN_UTextBox = new TextBox();
        TextBox pHxA_UTextBox = new TextBox();

        //Other TextBoxes
        TextBox numberOfChargeCarrier_UTextBox = new TextBox();
        TextBox neutralMass_UTextBox = new TextBox();
        TextBox massCharge_UTextBox = new TextBox();
        TextBox amountOfC_UTextBox = new TextBox();
        TextBox amountOfH_UTextBox = new TextBox();
        TextBox amountOfO_UTextBox = new TextBox();
        TextBox amountOfN_UTextBox = new TextBox();
        TextBox amountOfNa_UTextBox = new TextBox();
        #endregion

        public OmniFinderWithGlycanMaker()
        {
            UtilitiesOmniFinderPage_Canvas();
        }

        private void UtilitiesOmniFinderPage_Canvas()
        {
            //Initializes OmniFinder Values
            InitializeOmniFinderControls();

            //Specifics of Grid and Canvas so Background Color will Size with Window
            utilitiesOmniFinderPage_Grid.Height = 690;
            utilitiesOmniFinderPage_Grid.Width = 630;
            utilitiesOmniFinderPage_Canvas.Background = new SolidColorBrush(Colors.DarkCyan);


            //Build Grid and Design
            UtilitiesOmniFinderPage_Grid();

            //Add Grid to Canvas
            utilitiesOmniFinderPage_Canvas.Children.Add(utilitiesOmniFinderPage_Grid);
        }

        private void UtilitiesOmniFinderPage_Grid()
        {
            #region Main Grid Row Definitions
            //2 Row x 1 Column Main Grid   
            RowDefinition Main_RowDefinition = new RowDefinition();
            Main_RowDefinition.Height = new System.Windows.GridLength(60);
            utilitiesOmniFinderPage_Grid.RowDefinitions.Add(Main_RowDefinition);

            RowDefinition Main_RowDefinition1 = new RowDefinition();
            Main_RowDefinition1.Height = new System.Windows.GridLength(590);
            utilitiesOmniFinderPage_Grid.RowDefinitions.Add(Main_RowDefinition1);

            RowDefinition Main_RowDefinition2 = new RowDefinition();
            Main_RowDefinition2.Height = new System.Windows.GridLength(40);
            utilitiesOmniFinderPage_Grid.RowDefinitions.Add(Main_RowDefinition2);
            #endregion


            #region OmniFinder Header Label
            //"OmniFinder" Head Label in 1st Row
            Label omniFinder_Label = new Label();
            omniFinder_Label.Content = "OmniFinder";
            omniFinder_Label.Style = (Style)Application.Current.Resources["secondaryHeaderLabelTextStyle"];
            Grid.SetRow(omniFinder_Label, 0);
            Grid.SetColumn(omniFinder_Label, 0);
            #endregion


            #region Reset Button
            //Reset Button in 1st Row
            Button reset_Button = new Button();
            reset_Button.Content = "Reset";
            reset_Button.Margin = new Thickness(0, 26, 140, 0);
            reset_Button.Style = (Style)Application.Current.Resources["optionButtonStyle"];
            reset_Button.Click += this.resetButton_Click;
            Grid.SetRow(reset_Button, 0);
            Grid.SetColumn(reset_Button, 0);
            #endregion


            #region Select Options ComboBox
            //Select Options ComboBox in 1st Row
            selectOptions_UComboBox.Height = 23;
            selectOptions_UComboBox.Width = 130;
            selectOptions_UComboBox.Margin = new Thickness(110, 26, 0, 0);
            selectOptions_UComboBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            selectOptions_UComboBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            selectOptions_UComboBox.Name = "selectOptions_UComboBox";
            selectOptions_UComboBox.IsEditable = true;
            selectOptions_UComboBox.IsReadOnly = true;
            if (App.utilitiesOmniFinderVariables.SelectedOption_String != "")
                selectOptions_UComboBox.SelectedValue = App.utilitiesOmniFinderVariables.SelectedOption_String;
            selectOptions_UComboBox.Items.Add(new ComboBoxItem() { Content = "N Glycans" });
            selectOptions_UComboBox.Items.Add(new ComboBoxItem() { Content = "Amino Acids" });
            selectOptions_UComboBox.Items.Add(new ComboBoxItem() { Content = "No Option Selected" });
            selectOptions_UComboBox.SelectionChanged += this.selectOptions_ComboBox_SelectionChanged;
            Grid.SetRow(selectOptions_UComboBox, 0);
            Grid.SetColumn(selectOptions_UComboBox, 0);
            #endregion

            #region Build MiniGrid
            //Build MiniGrid Design
            UtilitiesOmniFinderPage_MiniGrid();

            //Put MiniGrid in 2nd Row of Main Grid
            Grid.SetRow(utilitiesOmniFinderPage_MiniGrid, 1);
            #endregion

            #region Calculate Mass Button
            //Calculate Mass Button in 3rd Row
            Button calculateMass_Button = new Button();
            calculateMass_Button.Content = "Calculate Mass";
            calculateMass_Button.Width = 180;
            calculateMass_Button.Background = new SolidColorBrush(Colors.PaleGreen);
            calculateMass_Button.Margin = new Thickness(0, 0, 350, 0);
            calculateMass_Button.Style = (Style)Application.Current.Resources["specialButtonStyle"];
            calculateMass_Button.Click += this.calculateMassButton_Click;
            Grid.SetRow(calculateMass_Button, 2);
            Grid.SetColumn(calculateMass_Button, 0);
            #endregion

            #region Ranges Button
            //Ranges Button in 3rd Row
            Button ranges_Button = new Button();
            ranges_Button.Content = "Ranges";
            ranges_Button.Width = 180;
            ranges_Button.Background = new SolidColorBrush(Colors.Pink);
            ranges_Button.Margin = new Thickness(300, 0, 0, 0);
            ranges_Button.Style = (Style)Application.Current.Resources["specialButtonStyle"];
            ranges_Button.Click += this.rangesButton_Click;
            Grid.SetRow(ranges_Button, 2);
            Grid.SetColumn(ranges_Button, 0);
            #endregion

            #region Add Controls and MiniGrid to Main Grid
            //Add Controls and 1st MiniGrid to Main Grid
            utilitiesOmniFinderPage_Grid.Children.Add(omniFinder_Label);
            utilitiesOmniFinderPage_Grid.Children.Add(reset_Button);
            utilitiesOmniFinderPage_Grid.Children.Add(selectOptions_UComboBox);
            utilitiesOmniFinderPage_Grid.Children.Add(utilitiesOmniFinderPage_MiniGrid);
            utilitiesOmniFinderPage_Grid.Children.Add(calculateMass_Button);
            utilitiesOmniFinderPage_Grid.Children.Add(ranges_Button);
            #endregion
        }

        private void UtilitiesOmniFinderPage_MiniGrid()
        {
            utilitiesOmniFinderPage_MiniGrid.Width = 630;
            utilitiesOmniFinderPage_MiniGrid.Height = 590;

            #region MiniGrid Row Definitions
            //2 Row x 1 Column Main Grid   
            RowDefinition miniGrid_RowDefinition = new RowDefinition();
            miniGrid_RowDefinition.Height = new System.Windows.GridLength(260);
            utilitiesOmniFinderPage_MiniGrid.RowDefinitions.Add(miniGrid_RowDefinition);

            RowDefinition miniGrid_RowDefinition1 = new RowDefinition();
            miniGrid_RowDefinition1.Height = new System.Windows.GridLength(180);
            utilitiesOmniFinderPage_MiniGrid.RowDefinitions.Add(miniGrid_RowDefinition1);

            RowDefinition miniGrid_RowDefinition2 = new RowDefinition();
            miniGrid_RowDefinition2.Height = new System.Windows.GridLength(140);
            utilitiesOmniFinderPage_MiniGrid.RowDefinitions.Add(miniGrid_RowDefinition2);
            #endregion

            #region 1st MiniGrid Column Definitions
            //2 Column MiniGrid in 2nd and 3rd Rows of Main Grid
            ColumnDefinition miniGrid_ColumnDefinition = new ColumnDefinition();
            miniGrid_ColumnDefinition.Width = new System.Windows.GridLength(290);
            utilitiesOmniFinderPage_MiniGrid.ColumnDefinitions.Add(miniGrid_ColumnDefinition);

            ColumnDefinition miniGrid_ColumnDefinition1 = new ColumnDefinition();
            miniGrid_ColumnDefinition1.Width = new System.Windows.GridLength(340);
            utilitiesOmniFinderPage_MiniGrid.ColumnDefinitions.Add(miniGrid_ColumnDefinition1);
            #endregion

            CreateLabels();
            CreateCheckBoxes();
            CreateTextBoxes();
            CreateComboBoxes();
            SugarsBox();
            UsersBox();
            SpecialsBox();
            AminoAcidsBox();
            PermethylBox();
            ChargeCarrierBox();
            ResultsBox();
        }

        private void CreateLabels()
        {
            List<Label> labels_List = new List<Label>();
            List<String> contents_List = new List<String>();
            List<int> leftMargin_List = new List<int>();
            List<int> bottomMargin_List = new List<int>();
            List<int> rowGrid_List = new List<int>();
            List<int> columnGrid_List = new List<int>();

            #region labels List
            //labels List
            //Sugars(8), User Units(2), Special(4), Charge Carrier, Carbohydrate Type
            //Amino Acids(20), Permethyl(8), Neutral Mass, Mass/Charge, C, H, O, N, Na
            //"#"(43), "-->"(4)
            for (int i = 1; i <= 98; i++)
                labels_List.Add(new Label());
            #endregion

            #region contents List
            //contents List
            //Sugars(8)
            contents_List.Add("Hexose (162.05282)");
            contents_List.Add("HexNAc (203.07937)");
            contents_List.Add("DxyHex (146.05790)");
            contents_List.Add("Pentose (132.04225)");
            contents_List.Add("NeuAc (291.09541)");
            contents_List.Add("NeuGc (307.09033)");
            contents_List.Add("KDN (250.06886)");
            contents_List.Add("Hex A (176.03208)");

            //User Units(2)
            contents_List.Add("User Unit A");
            contents_List.Add("User Unit B");

            //Specials(4)
            contents_List.Add("NaH");
            contents_List.Add("CH₃");
            contents_List.Add("SO₃­");
            contents_List.Add("O-Acetyl­");

            //Charge Carrier
            contents_List.Add("Charge Carrier");

            //Carbohydrate Type
            contents_List.Add("Carbohydrate Type");

            //Amino Acids(20)
            contents_List.Add("Ala");
            contents_List.Add("Arg");
            contents_List.Add("Asn");
            contents_List.Add("Asp");
            contents_List.Add("Cys");
            contents_List.Add("Gln");
            contents_List.Add("Glu");
            contents_List.Add("Gly");
            contents_List.Add("His");
            contents_List.Add("Ile");
            contents_List.Add("Leu");
            contents_List.Add("Lys");
            contents_List.Add("Met");
            contents_List.Add("Phe");
            contents_List.Add("Ser");
            contents_List.Add("Thr");
            contents_List.Add("Trp");
            contents_List.Add("Tyr");
            contents_List.Add("Val");
            contents_List.Add("Pro");

            //Permethyl(8)
            contents_List.Add("pHex");
            contents_List.Add("pHxNAc");
            contents_List.Add("pDxHex");
            contents_List.Add("pPntos");
            contents_List.Add("pNuAc");
            contents_List.Add("pNuGc");
            contents_List.Add("pKDN");
            contents_List.Add("pHxA");

            //Neutral Mass, Mass/Charge, C, H, O, N, Na
            contents_List.Add("Neutral Mass");
            contents_List.Add("Mass/Charge");
            contents_List.Add("C");
            contents_List.Add("H");
            contents_List.Add("O");
            contents_List.Add("N");
            contents_List.Add("Na");

            //"#"(43)
            for(int i = 1; i <= 43; i ++)
                contents_List.Add("#");

            //"-->"(4)
            for (int i = 1; i <= 4; i++)
                contents_List.Add("-->");
            #endregion

            #region leftMargin List
            //leftMargin List
            //Sugars(8), User Units(2)
            for(int i = 1; i <= 10; i++)
                leftMargin_List.Add(30);
            
            //Specials(4)
            for(int i = 1; i <= 2; i++)
                leftMargin_List.Add(30);
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(155);

            //Charge Carrier, Carbohydrate Type
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(20);

            //Amino Acids(20)
            for (int i = 1; i <= 7; i++)
                leftMargin_List.Add(30);
            for (int i = 1; i <= 7; i++)
                leftMargin_List.Add(140);
            for (int i = 1; i <= 6; i++)
                leftMargin_List.Add(250);

            //Permethyl(8)
            for (int i = 1; i <= 4; i++)
                leftMargin_List.Add(30);
            for (int i = 1; i <= 4; i++)
                leftMargin_List.Add(200);

            //Neutral Mass, Mass/Charge, C
            for (int i = 1; i <= 3; i++)
                leftMargin_List.Add(20);

            //H, O, N, Na
            leftMargin_List.Add(130);
            leftMargin_List.Add(230);
            leftMargin_List.Add(20);
            leftMargin_List.Add(130);

            //"#"(43)
            for (int i = 1; i <= 10; i++) //Sugars, User Units
                leftMargin_List.Add(220);
            for (int i = 1; i <= 2; i++) //Specials
                leftMargin_List.Add(70);
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(220);
            leftMargin_List.Add(140);     //Charge Carrier
            for (int i = 1; i <= 7; i++)  //Amino Acids
                leftMargin_List.Add(60);
            for (int i = 1; i <= 7; i++)
                leftMargin_List.Add(170);
            for (int i = 1; i <= 6; i++)
                leftMargin_List.Add(280);
            for (int i = 1; i <= 4; i++)  //Permethyl
                leftMargin_List.Add(90);
            for (int i = 1; i <= 4; i++)
                leftMargin_List.Add(250);

            //"-->"(4)
            for (int i = 1; i <= 2; i++)  //User Units
                leftMargin_List.Add(110);
            for (int i = 1; i <= 2; i++)  //Neutral Mass, Mass/Charge
                leftMargin_List.Add(120);
            #endregion

            #region bottomMargin List
            //bottomMargin List
            //Sugars(8)
            for (int i = 1; i <= 8; i++)
                bottomMargin_List.Add(187 - ((i-1)*57));

            //User Units(2)
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(110 - ((i - 1) * 65));

            //Specials(4)
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-60 - ((i - 1) * 70));
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-60 - ((i - 1) * 70));

            //Charge Carrier, Carbohydrate Type
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(100 - ((i - 1) * 130));

            //Amino Acids(20)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 7; i++)
                    bottomMargin_List.Add(187 - ((i - 1) * 67));
            }
            for (int i = 1; i <= 6; i++)
                bottomMargin_List.Add(187 - ((i - 1) * 67));

            //Permethyl(8)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 4; i++)
                    bottomMargin_List.Add(95 - ((i - 1) * 70));
            }

            //Neutral Mass, Mass/Charge, C, H, O, N, Na
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(100 - ((i - 1) * 60));
            for (int i = 1; i <= 3; i++)
                bottomMargin_List.Add(-30);
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-90);

            //"#"(43)
            for (int i = 1; i <= 8; i++)                      //Sugars
                bottomMargin_List.Add(187 - ((i - 1) * 57));
            for (int i = 1; i <= 2; i++)                      //User Units
                bottomMargin_List.Add(110 - ((i - 1) * 65));
            for (int i = 1; i <= 2; i++)                      //Specials
                bottomMargin_List.Add(-60 - ((i - 1) * 70));
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-60 - ((i - 1) * 70));
            bottomMargin_List.Add(30);                       //Charge Carrier
            for (int x = 1; x <= 2; x++)                      //Amino Acids
            {
                for (int i = 1; i <= 7; i++)
                    bottomMargin_List.Add(187 - ((i - 1) * 67));
            }
            for (int i = 1; i <= 6; i++)
                bottomMargin_List.Add(187 - ((i - 1) * 67));
            for (int x = 1; x <= 2; x++)                      //Permethyl
            {
                for (int i = 1; i <= 4; i++)
                    bottomMargin_List.Add(95 - ((i - 1) * 70));
            }

            //"-->"(4)
            for (int i = 1; i <= 2; i++)                      //User Units
                bottomMargin_List.Add(110 - ((i - 1) * 65));
            for (int i = 1; i <= 2; i++)                      //Neutral Mass, Mass/Charge
                bottomMargin_List.Add(100 - ((i - 1) * 60));
            #endregion

            #region rowGrid List
            //rowGrid List
            //Sugars(8)
            for (int i = 1; i <= 8; i++)
                rowGrid_List.Add(0);

            //User Units(2), Specials(4)
            for (int i = 1; i <= 6; i++)
                rowGrid_List.Add(1);

            //Charge Type, Carbohydrate Type
            for (int i = 1; i <= 2; i++)
                rowGrid_List.Add(2);

            //Amino Acids(20)
            for (int i = 1; i <= 20; i++)
                rowGrid_List.Add(0);

            //Permethyl(8)
            for (int i = 1; i <= 8; i++)
                rowGrid_List.Add(1);

            //Neutral Mass, Mass/Charge, C, H, O, N, Na
            for (int i = 1; i <= 7; i++)
                rowGrid_List.Add(2);

            //"#"(43)
            for (int i = 1; i <= 8; i++) //Sugars
                rowGrid_List.Add(0);
            for (int i = 1; i <= 6; i++) //User Units, Specials
                rowGrid_List.Add(1);
            rowGrid_List.Add(2);         //Charge Carrier
            for (int i = 1; i <= 20; i++)//Amino Acids
                rowGrid_List.Add(0);
            for (int i = 1; i <= 8; i++) //Permethyl
                rowGrid_List.Add(1);

            //"-->"(4)
            for (int i = 1; i <= 2; i++) //User Units
                rowGrid_List.Add(1);
            for (int i = 1; i <= 2; i++) //Neutral Mass, Mass/Charge
                rowGrid_List.Add(2);
            #endregion

            #region columnGrid List
            //columnGrid List
            //Sugars(8), User Units(2), Specials(4), Charge Carrier, Carbohydrate Type
            for (int i = 1; i <= 16; i++)
                columnGrid_List.Add(0);

            //Amino Acids(20), Permethyl(8), Neutral Mass, Mass/Charge, C, H, O, N, Na
            for (int i = 1; i <= 35; i++)
                columnGrid_List.Add(1);

            //"#"(43)
            for (int i = 1; i <= 15; i++) //Sugars, User Units, Specials, Charge Carrier
                columnGrid_List.Add(0);
            for (int i = 1; i <= 28; i++) //Amino Acids, Permethyl
                columnGrid_List.Add(1);

            //"-->"(4)
            for (int i = 1; i <= 2; i++)  //User Units
                columnGrid_List.Add(0);
            for (int i = 1; i <= 2; i++)  //Neutral Mass, Mass/Charge
                columnGrid_List.Add(1);
            #endregion



            for (int i = 0; i <= (labels_List.Count-1); i++)
            {
                labels_List[i].Content = contents_List[i];
                labels_List[i].Margin = new Thickness(leftMargin_List[i], 0, 0, bottomMargin_List[i]);
                labels_List[i].Style = (Style)Application.Current.Resources["blackLeftLineLabelTextStyle"];
                Grid.SetRow(labels_List[i], rowGrid_List[i]);
                Grid.SetColumn(labels_List[i], columnGrid_List[i]);

                //Add Controls to MiniGrid
                utilitiesOmniFinderPage_MiniGrid.Children.Add(labels_List[i]);
            }
        }

        private void CreateCheckBoxes()
        {
            List<CheckBox> checkBoxes_List = new List<CheckBox>();
            List<String> names_List = new List<String>();
            List<Boolean> checkBoxVariables_List = new List<Boolean>();
            List<int> leftMargin_List = new List<int>();
            List<int> bottomMargin_List = new List<int>();
            List<int> rowGrid_List = new List<int>();
            List<int> columnGrid_List = new List<int>();

            #region checkboxes List
            //checkboxes List
            //Sugars(8)
            checkBoxes_List.Add(hexose_UCheckBox);
            checkBoxes_List.Add(hexNAc_UCheckBox);
            checkBoxes_List.Add(dxyHex_UCheckBox);
            checkBoxes_List.Add(pentose_UCheckBox);
            checkBoxes_List.Add(neuAc_UCheckBox);
            checkBoxes_List.Add(neuGc_UCheckBox);
            checkBoxes_List.Add(kDN_UCheckBox);
            checkBoxes_List.Add(hexA_UCheckBox);

            //User Units(2)
            checkBoxes_List.Add(userUnitA_UCheckBox);
            checkBoxes_List.Add(userUnitB_UCheckBox);

            //Specials(4)
            checkBoxes_List.Add(naH_UCheckBox);
            checkBoxes_List.Add(cH3_UCheckBox);
            checkBoxes_List.Add(sO3_UCheckBox);
            checkBoxes_List.Add(oAcetyl_UCheckBox);

            //Amino Acids(20)
            checkBoxes_List.Add(ala_UCheckBox);
            checkBoxes_List.Add(arg_UCheckBox);
            checkBoxes_List.Add(asn_UCheckBox);
            checkBoxes_List.Add(asp_UCheckBox);
            checkBoxes_List.Add(cys_UCheckBox);
            checkBoxes_List.Add(gln_UCheckBox);
            checkBoxes_List.Add(glu_UCheckBox);
            checkBoxes_List.Add(gly_UCheckBox);
            checkBoxes_List.Add(his_UCheckBox);
            checkBoxes_List.Add(ile_UCheckBox);
            checkBoxes_List.Add(leu_UCheckBox);
            checkBoxes_List.Add(lys_UCheckBox);
            checkBoxes_List.Add(met_UCheckBox);
            checkBoxes_List.Add(phe_UCheckBox);
            checkBoxes_List.Add(ser_UCheckBox);
            checkBoxes_List.Add(thr_UCheckBox);
            checkBoxes_List.Add(trp_UCheckBox);
            checkBoxes_List.Add(tyr_UCheckBox);
            checkBoxes_List.Add(val_UCheckBox);
            checkBoxes_List.Add(pro_UCheckBox);

            //Permethyl(8)
            checkBoxes_List.Add(pHex_UCheckBox);
            checkBoxes_List.Add(pHxNAc_UCheckBox);
            checkBoxes_List.Add(pDxHex_UCheckBox);
            checkBoxes_List.Add(pPntos_UCheckBox);
            checkBoxes_List.Add(pNuAc_UCheckBox);
            checkBoxes_List.Add(pNuGc_UCheckBox);
            checkBoxes_List.Add(pKDN_UCheckBox);
            checkBoxes_List.Add(pHxA_UCheckBox);
            #endregion

            #region names List
            //names List
            //Sugars(8)
            names_List.Add("hexose_UCheckBox");
            names_List.Add("hexNAc_UCheckBox");
            names_List.Add("dxyHex_UCheckBox");
            names_List.Add("pentose_UCheckBox");
            names_List.Add("neuAc_UCheckBox");
            names_List.Add("neuGc_UCheckBox");
            names_List.Add("kDN_UCheckBox");
            names_List.Add("hexA_UCheckBox");

            //User Units(2)
            names_List.Add("userUnitA_UCheckBox");
            names_List.Add("userUnitB_UCheckBox");

            //Specials(3)
            names_List.Add("naH_UCheckBox");
            names_List.Add("cH3_UCheckBox");
            names_List.Add("sO3_UCheckBox");
            names_List.Add("oAcetyl_UCheckBox");

            //Amino Acids(20)
            names_List.Add("ala_UCheckBox");
            names_List.Add("arg_UCheckBox");
            names_List.Add("asn_UCheckBox");
            names_List.Add("asp_UCheckBox");
            names_List.Add("cys_UCheckBox");
            names_List.Add("gln_UCheckBox");
            names_List.Add("glu_UCheckBox");
            names_List.Add("gly_UCheckBox");
            names_List.Add("his_UCheckBox");
            names_List.Add("ile_UCheckBox");
            names_List.Add("leu_UCheckBox");
            names_List.Add("lys_UCheckBox");
            names_List.Add("met_UCheckBox");
            names_List.Add("phe_UCheckBox");
            names_List.Add("ser_UCheckBox");
            names_List.Add("thr_UCheckBox");
            names_List.Add("trp_UCheckBox");
            names_List.Add("tyr_UCheckBox");
            names_List.Add("val_UCheckBox");
            names_List.Add("pro_UCheckBox");

            //Permethyl(8)
            names_List.Add("pHex_UCheckBox");
            names_List.Add("pHxNAc_UCheckBox");
            names_List.Add("pDxHex_UCheckBox");
            names_List.Add("pPntos_UCheckBox");
            names_List.Add("pNuAc_UCheckBox");
            names_List.Add("pNuGc_UCheckBox");
            names_List.Add("pKDN_UCheckBox");
            names_List.Add("pHxA_UCheckBox");
            #endregion   

            #region checkBoxVariables List
            //checkBoxVariables List
            //Sugars(8)
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedHexose_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedPentose_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedNeuGc_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedKDN_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedHexA_Bool);

            //User Units(2)
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedUserUnit1_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedUserUnit2_Bool);

            //Specials(4)
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedNaH_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedCH3_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedSO3_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedOAcetyl_Bool);

            //Amino Acids(20)
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedAla_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedArg_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedAsn_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedAsp_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedCys_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedGln_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedGlu_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedGly_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedHis_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedIle_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedLeu_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedLys_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedMet_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedPhe_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedSer_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedThr_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedTrp_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedTyr_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedVal_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedPro_Bool);

            //Permethyl(8)
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedpHex_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedpHxNAc_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedpDxHex_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedpPntos_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedpNuAc_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedpNuGc_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedpKDN_Bool);
            checkBoxVariables_List.Add(App.utilitiesOmniFinderVariables.CheckedpHxA_Bool);
            #endregion

            #region leftMargin List
            //leftMargin List
            //Sugars(8), User Units(2)
            for (int i = 1; i <= 10; i++) 
                leftMargin_List.Add(15);

            //Specials(4)
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(15);
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(140);
            
            //Amino Acids(20) 
            for (int i = 1; i <= 7; i++)  
                leftMargin_List.Add(15);
            for (int i = 1; i <= 7; i++)
                leftMargin_List.Add(125);
            for (int i = 1; i <= 6; i++)
                leftMargin_List.Add(235);
            
            //Permethyl(8)
            for (int i = 1; i <= 4; i++)  
                leftMargin_List.Add(15);
            for (int i = 1; i <= 4; i++)
                leftMargin_List.Add(185);
            #endregion

            #region bottomMargin List
            //bottomMargin List
            //Sugars(8)
            for (int i = 1; i <= 8; i++)                      
                bottomMargin_List.Add(187 - ((i - 1) * 57));
            
            //User Units(2)
            for (int i = 1; i <= 2; i++)                      
                bottomMargin_List.Add(110 - ((i - 1) * 65));
            
            //Specials(4)
            for (int i = 1; i <= 2; i++)                      
                bottomMargin_List.Add(-60 - ((i - 1) * 70));
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-60 - ((i - 1) * 70));

            //Amino Acids(20)
            for (int x = 1; x <= 2; x++)                      
            {
                for (int i = 1; i <= 7; i++)
                    bottomMargin_List.Add(187 - ((i - 1) * 67));
            }
            for (int i = 1; i <= 6; i++)
                bottomMargin_List.Add(187 - ((i - 1) * 67));
            
            //Permethyl(8)
            for (int x = 1; x <= 2; x++)                      
            {
                for (int i = 1; i <= 4; i++)
                    bottomMargin_List.Add(95 - ((i - 1) * 70));
            }
            #endregion

            #region rowGrid List
            //rowGrid List
            //Sugars(8)
            for (int i = 1; i <= 8; i++) 
                rowGrid_List.Add(0);
            
            //User Units(2), Specials(4)
            for (int i = 1; i <= 6; i++) 
                rowGrid_List.Add(1);       
            
            //Amino Acids(20)
            for (int i = 1; i <= 20; i++)
                rowGrid_List.Add(0);
            
            //Permethyl(8)
            for (int i = 1; i <= 8; i++) 
                rowGrid_List.Add(1);
            #endregion

            #region columnGrid List
            //columnGrid List
            //Sugars(8), User Units(2), Specials(4)
            for (int i = 1; i <= 14; i++) 
                columnGrid_List.Add(0);
            
            //Amino Acids(20), Permethyl(8)
            for (int i = 1; i <= 28; i++) 
                columnGrid_List.Add(1);
            #endregion

            for (int i = 0; i <= (checkBoxes_List.Count - 1); i++)
            {
                checkBoxes_List[i].Name = names_List[i];
                checkBoxes_List[i].Margin = new Thickness(leftMargin_List[i], 0, 0, bottomMargin_List[i]);
                checkBoxes_List[i].HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                checkBoxes_List[i].VerticalAlignment = System.Windows.VerticalAlignment.Center;
                if (checkBoxVariables_List[i] != false)
                    checkBoxes_List[i].IsChecked = checkBoxVariables_List[i];
                checkBoxes_List[i].Click += new System.Windows.RoutedEventHandler(this.naH_CheckBox_Click);
                Grid.SetRow(checkBoxes_List[i], rowGrid_List[i]);
                Grid.SetColumn(checkBoxes_List[i], columnGrid_List[i]);

                //Add Controls to MiniGrid
                utilitiesOmniFinderPage_MiniGrid.Children.Add(checkBoxes_List[i]);
            }
        }

        private void CreateTextBoxes()
        {
            List<TextBox> textBoxes_List = new List<TextBox>();
            List<String> names_List = new List<String>();
            List<int> intVariables_List = new List<int>();
            List<int> leftMargin_List = new List<int>();
            List<int> bottomMargin_List = new List<int>();
            List<int> rowGrid_List = new List<int>();
            List<int> columnGrid_List = new List<int>();

            #region textBoxes List
            //textBoxes List
            //Sugars(8)
            textBoxes_List.Add(hexose_UTextBox);
            textBoxes_List.Add(hexNAc_UTextBox);
            textBoxes_List.Add(dxyHex_UTextBox);
            textBoxes_List.Add(pentose_UTextBox);
            textBoxes_List.Add(neuAc_UTextBox);
            textBoxes_List.Add(neuGc_UTextBox);
            textBoxes_List.Add(kDN_UTextBox);
            textBoxes_List.Add(hexA_UTextBox);

            //User Units(2) -mass and numberOf
            textBoxes_List.Add(userUnitA_UTextBox);
            textBoxes_List.Add(userUnitB_UTextBox);
            textBoxes_List.Add(massOfUserUnitA_UTextBox);
            textBoxes_List.Add(massOfUserUnitB_UTextBox);
            
            //Specials(4)
            textBoxes_List.Add(naH_UTextBox);
            textBoxes_List.Add(cH3_UTextBox);
            textBoxes_List.Add(sO3_UTextBox);
            textBoxes_List.Add(oAcetyl_UTextBox);

            //Amino Acids(20)
            textBoxes_List.Add(ala_UTextBox);
            textBoxes_List.Add(arg_UTextBox);
            textBoxes_List.Add(asn_UTextBox);
            textBoxes_List.Add(asp_UTextBox);
            textBoxes_List.Add(cys_UTextBox);
            textBoxes_List.Add(gln_UTextBox);
            textBoxes_List.Add(glu_UTextBox);
            textBoxes_List.Add(gly_UTextBox);
            textBoxes_List.Add(his_UTextBox);
            textBoxes_List.Add(ile_UTextBox);
            textBoxes_List.Add(leu_UTextBox);
            textBoxes_List.Add(lys_UTextBox);
            textBoxes_List.Add(met_UTextBox);
            textBoxes_List.Add(phe_UTextBox);
            textBoxes_List.Add(ser_UTextBox);
            textBoxes_List.Add(thr_UTextBox);
            textBoxes_List.Add(trp_UTextBox);
            textBoxes_List.Add(tyr_UTextBox);
            textBoxes_List.Add(val_UTextBox);
            textBoxes_List.Add(pro_UTextBox);

            //Permethyl(8)
            textBoxes_List.Add(pHex_UTextBox);
            textBoxes_List.Add(pHxNAc_UTextBox);
            textBoxes_List.Add(pDxHex_UTextBox);
            textBoxes_List.Add(pPntos_UTextBox);
            textBoxes_List.Add(pNuAc_UTextBox);
            textBoxes_List.Add(pNuGc_UTextBox);
            textBoxes_List.Add(pKDN_UTextBox);
            textBoxes_List.Add(pHxA_UTextBox);

            //Other TextBoxes(8)
            textBoxes_List.Add(numberOfChargeCarrier_UTextBox);
            textBoxes_List.Add(neutralMass_UTextBox);
            textBoxes_List.Add(massCharge_UTextBox);
            textBoxes_List.Add(amountOfC_UTextBox);
            textBoxes_List.Add(amountOfH_UTextBox);
            textBoxes_List.Add(amountOfO_UTextBox);
            textBoxes_List.Add(amountOfN_UTextBox);
            textBoxes_List.Add(amountOfNa_UTextBox);
            #endregion

            #region names List
            //names List
            //Sugars(8)
            names_List.Add("hexose_UTextBox");
            names_List.Add("hexNAc_UTextBox");
            names_List.Add("dxyHex_UTextBox");
            names_List.Add("pentose_UTextBox");
            names_List.Add("neuAc_UTextBox");
            names_List.Add("neuGc_UTextBox");
            names_List.Add("kDN_UTextBox");
            names_List.Add("hexA_UTextBox");

            //User Units(2) -mass and numberOf
            names_List.Add("userUnitA_UTextBox");
            names_List.Add("userUnitB_UTextBox");
            names_List.Add("massOfUserUnitA_UTextBox");
            names_List.Add("massOfUserUnitB_UTextBox");

            //Specials(3)
            names_List.Add("naH_UTextBox");
            names_List.Add("cH3_UTextBox");
            names_List.Add("sO3_UTextBox");
            names_List.Add("oAcetyl_UTextBox");

            //Amino Acids(20)
            names_List.Add("ala_UTextBox");
            names_List.Add("arg_UTextBox");
            names_List.Add("asn_UTextBox");
            names_List.Add("asp_UTextBox");
            names_List.Add("cys_UTextBox");
            names_List.Add("gln_UTextBox");
            names_List.Add("glu_UTextBox");
            names_List.Add("gly_UTextBox");
            names_List.Add("his_UTextBox");
            names_List.Add("ile_UTextBox");
            names_List.Add("leu_UTextBox");
            names_List.Add("lys_UTextBox");
            names_List.Add("met_UTextBox");
            names_List.Add("phe_UTextBox");
            names_List.Add("ser_UTextBox");
            names_List.Add("thr_UTextBox");
            names_List.Add("trp_UTextBox");
            names_List.Add("tyr_UTextBox");
            names_List.Add("val_UTextBox");
            names_List.Add("pro_UTextBox");

            //Permethyl(8)
            names_List.Add("pHex_UTextBox");
            names_List.Add("pHxNAc_UTextBox");
            names_List.Add("pDxHex_UTextBox");
            names_List.Add("pPntos_UTextBox");
            names_List.Add("pNuAc_UTextBox");
            names_List.Add("pNuGc_UTextBox");
            names_List.Add("pKDN_UTextBox");
            names_List.Add("pHxA_UTextBox");

            //Other TextBoxes(8)
            names_List.Add("numberOfChargeCarrier_UTextBox");
            names_List.Add("neutralMass_UTextBox");
            names_List.Add("massCharge_UTextBox");
            names_List.Add("amountOfC_UTextBox");
            names_List.Add("amountOfH_UTextBox");
            names_List.Add("amountOfO_UTextBox");
            names_List.Add("amountOfN_UTextBox");
            names_List.Add("amountOfNa_UTextBox");
            #endregion

            #region intVariables List
            //intVariables List
            //Sugars(8)
            intVariables_List.Add(App.glycanMakerVariables.NumberOfHexose_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfHexNAc_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfDeoxyhexose_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfPentose_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfNeuAc_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfNeuGc_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfKDN_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfHexA_Int);

            //User Units(2)
            intVariables_List.Add(App.glycanMakerVariables.NumberOfUserUnit1_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfUserUnit2_Int);
            intVariables_List.Add(0);
            intVariables_List.Add(0);
            
            //Speacials(4)
            intVariables_List.Add(App.glycanMakerVariables.NumberOfNaH_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfCH3_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfSO3_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfOAcetyl_Int);

            //Amino Acids(20)
            intVariables_List.Add(App.glycanMakerVariables.NumberOfAla_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfArg_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfAsn_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfAsp_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfCys_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfGln_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfGlu_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfGly_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfHis_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfIle_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfLeu_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfLys_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfMet_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfPhe_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfSer_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfThr_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfTrp_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfTyr_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfVal_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfPro_Int);

            //Perethyl(8)
            intVariables_List.Add(App.glycanMakerVariables.NumberOfpHex_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfpHxNAc_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfpDxHex_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfpPntos_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfpNuAc_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfpNuGc_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfpKDN_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfpHxA_Int);

            //Others(6)
            intVariables_List.Add(App.glycanMakerVariables.NumberOfChargeCarrier_Int);
            intVariables_List.Add(0);
            intVariables_List.Add(0);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfC_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfH_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfO_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfN_Int);
            intVariables_List.Add(App.glycanMakerVariables.NumberOfNa_Int);
            #endregion

            #region leftMargin List
            //leftMargin List
            //Sugars(8), User Units(2)
            for (int i = 1; i <= 10; i++)
                leftMargin_List.Add(240);

            //User Unit Mass
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(139);

            //Specials(4)
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(90);
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(240);

            //Amino Acids(20)
            for (int i = 1; i <= 7; i++)
                leftMargin_List.Add(80);
            for (int i = 1; i <= 7; i++)
                leftMargin_List.Add(190);
            for (int i = 1; i <= 6; i++)
                leftMargin_List.Add(300);

            //Permethyl(8)
            for (int i = 1; i <= 4; i++)
                leftMargin_List.Add(110);
            for (int i = 1; i <= 4; i++)
                leftMargin_List.Add(270);

            //Charge Carrier
            leftMargin_List.Add(160);

            //Neutral Mass, Mass/Charge
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(170);

            //C, H, O, N, Na
            leftMargin_List.Add(50);
            leftMargin_List.Add(160);
            leftMargin_List.Add(260);
            leftMargin_List.Add(50);
            leftMargin_List.Add(160);
            #endregion

            #region bottomMargin List
            //bottomMargin List
            //Sugars(8)
            for (int i = 1; i <= 8; i++)
                bottomMargin_List.Add(187 - ((i - 1) * 57));

            //User Units(2)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 2; i++)
                    bottomMargin_List.Add(110 - ((i - 1) * 65));
            }

            //Specials(4)
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-60 - ((i - 1) * 70));
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-60 - ((i - 1) * 70));

            //Amino Acids(20)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 7; i++)
                    bottomMargin_List.Add(187 - ((i - 1) * 67));
            }
            for (int i = 1; i <= 6; i++)
                bottomMargin_List.Add(187 - ((i - 1) * 67));

            //Permethyl(8)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 4; i++)
                    bottomMargin_List.Add(95 - ((i - 1) * 70));
            }

            //Charge Carrier
            bottomMargin_List.Add(30);

            //Neutral Mass, Mass/Charge, C, H, O, N, Na
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(100 - ((i - 1) * 60));
            for (int i = 1; i <= 3; i++)
                bottomMargin_List.Add(-30);
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-90);
            #endregion

            #region rowGrid List
            //rowGrid List
            //Sugars(8)
            for (int i = 1; i <= 8; i++)
                rowGrid_List.Add(0);

            //User Units(4), Specials(4)
            for (int i = 1; i <= 8; i++)
                rowGrid_List.Add(1);

            //Amino Acids(20)
            for (int i = 1; i <= 20; i++)
                rowGrid_List.Add(0);

            //Permethyl(8)
            for (int i = 1; i <= 8; i++)
                rowGrid_List.Add(1);

            //Charge Type, Neutral Mass, Mass/Charge, C, H, O, N, Na
            for (int i = 1; i <= 8; i++)
                rowGrid_List.Add(2);
            #endregion

            #region columnGrid List
            //columnGrid List
            //Sugars(8), User Units(4), Specials(4)
            for (int i = 1; i <= 16; i++)
                columnGrid_List.Add(0);

            //Amino Acids(20), Permethyl(8)
            for (int i = 1; i <= 28; i++)
                columnGrid_List.Add(1);

            //Charge Carrier
            columnGrid_List.Add(0);

            //Neutral Mass, Mass/Charge, C, H, O, N, Na
            for (int i = 1; i <= 7; i++)
                columnGrid_List.Add(1);
            #endregion

            for (int i = 0; i <= (textBoxes_List.Count - 1); i++)
            {
                textBoxes_List[i].Name = names_List[i];
                if (textBoxes_List[i].Name == "massOfUserUnitA_UTextBox" || textBoxes_List[i].Name == "massOfUserUnitB_UTextBox" ||
                    textBoxes_List[i].Name == "neutralMass_UTextBox" || textBoxes_List[i].Name == "massCharge_UTextBox")
                    textBoxes_List[i].Width = 80;
                else
                    textBoxes_List[i].Width = 25;
                if (i >= 45)
                    textBoxes_List[i].IsReadOnly = true;
                textBoxes_List[i].Margin = new Thickness(leftMargin_List[i], 0, 0, bottomMargin_List[i]);
                textBoxes_List[i].HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                textBoxes_List[i].VerticalAlignment = System.Windows.VerticalAlignment.Center;
                switch (textBoxes_List[i].Name)
                {
                    case "massOfUserUnitA_UTextBox":
                        {
                            if (App.glycanMakerVariables.MassOfUserUnit1_Double != 0.0)
                                textBoxes_List[i].Text = App.glycanMakerVariables.MassOfUserUnit1_Double.ToString();
                            textBoxes_List[i].TextChanged += this.massOfUserUnitA_UTextBox_TextChanged;
                            break;
                        }
                    case "massOfUserUnitB_UTextBox":
                        {
                            if (App.glycanMakerVariables.MassOfUserUnit2_Double != 0.0)
                                textBoxes_List[i].Text = App.glycanMakerVariables.MassOfUserUnit2_Double.ToString();
                            textBoxes_List[i].TextChanged += this.massOfUserUnitA_UTextBox_TextChanged;
                            break;
                        }
                    case "neutralMass_UTextBox":
                        {
                            if (App.glycanMakerVariables.NeutralMass_Double != 0.0)
                                textBoxes_List[i].Text = App.glycanMakerVariables.NeutralMass_Double.ToString();
                            textBoxes_List[i].TextChanged += this.massOfUserUnitA_UTextBox_TextChanged;
                            break;
                        }
                    case "massCharge_UTextBox":
                        {
                            if (App.glycanMakerVariables.MassCharge_Double != 0.0)
                                textBoxes_List[i].Text = App.glycanMakerVariables.MassCharge_Double.ToString();
                            textBoxes_List[i].TextChanged += this.massOfUserUnitA_UTextBox_TextChanged;
                            break;
                        }
                    default:
                        {
                            if (intVariables_List[i] != 0)
                                textBoxes_List[i].Text = intVariables_List[i].ToString();
                            textBoxes_List[i].TextChanged += this.hexose_UTextBox_TextChanged;
                            break;
                        }
                }
                Grid.SetRow(textBoxes_List[i], rowGrid_List[i]);
                Grid.SetColumn(textBoxes_List[i], columnGrid_List[i]);

                //Add Controls to MiniGrid
                utilitiesOmniFinderPage_MiniGrid.Children.Add(textBoxes_List[i]);
            }
        }

        private void CreateComboBoxes()
        {
            #region Charge Carrier ComboBox
            //Select Options ComboBox in 1st Row
            chargeCarrier_UComboBox.Height = 23;
            chargeCarrier_UComboBox.Width = 70;
            chargeCarrier_UComboBox.Margin = new Thickness(30, 0, 0, 30);
            chargeCarrier_UComboBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            chargeCarrier_UComboBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            chargeCarrier_UComboBox.Name = "chargeCarrier_UComboBox";
            chargeCarrier_UComboBox.IsEditable = true;
            chargeCarrier_UComboBox.IsReadOnly = true;
            if (App.glycanMakerVariables.TypeOfChargeCarrier_String != "")
                chargeCarrier_UComboBox.SelectedValue = App.glycanMakerVariables.TypeOfChargeCarrier_String;
            chargeCarrier_UComboBox.Items.Add(new ComboBoxItem() { Content = "H" });
            chargeCarrier_UComboBox.Items.Add(new ComboBoxItem() { Content = "Na" });
            chargeCarrier_UComboBox.Items.Add(new ComboBoxItem() { Content = "K" });
            chargeCarrier_UComboBox.Items.Add(new ComboBoxItem() { Content = "-H" });
            chargeCarrier_UComboBox.Items.Add(new ComboBoxItem() { Content = "NH4" });
            chargeCarrier_UComboBox.Items.Add(new ComboBoxItem() { Content = "Water" });
            chargeCarrier_UComboBox.Items.Add(new ComboBoxItem() { Content = "Neutral" });
            chargeCarrier_UComboBox.Items.Add(new ComboBoxItem() { Content = "UserA" });
            chargeCarrier_UComboBox.Items.Add(new ComboBoxItem() { Content = "UserB" });
            //chargeCarrier_UComboBox.SelectedValuePath = "App.glycanMakerVariables.TypeOfChargeCarrier_String";
            chargeCarrier_UComboBox.SelectionChanged += this.selectOptions_ComboBox_SelectionChanged;
            Grid.SetRow(chargeCarrier_UComboBox, 2);
            Grid.SetColumn(chargeCarrier_UComboBox, 0);
            #endregion

            #region Carbohydrate Type ComboBox
            //Select Options ComboBox in 1st Row
            carbohydrateType_UComboBox.Height = 23;
            carbohydrateType_UComboBox.Width = 130;
            carbohydrateType_UComboBox.Margin = new Thickness(30, 0, 0, -80);
            carbohydrateType_UComboBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            carbohydrateType_UComboBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            carbohydrateType_UComboBox.Name = "carbohydrateType_UComboBox";
            carbohydrateType_UComboBox.IsEditable = true;
            carbohydrateType_UComboBox.IsReadOnly = true;
            if (App.glycanMakerVariables.CarbohydrateType_String != "")
                carbohydrateType_UComboBox.SelectedValue = App.glycanMakerVariables.CarbohydrateType_String;
            carbohydrateType_UComboBox.Items.Add(new ComboBoxItem() { Content = "Aldehyde" });
            carbohydrateType_UComboBox.Items.Add(new ComboBoxItem() { Content = "Alditol" });
            carbohydrateType_UComboBox.Items.Add(new ComboBoxItem() { Content = "Fragment" });
            carbohydrateType_UComboBox.SelectionChanged += this.selectOptions_ComboBox_SelectionChanged;
            Grid.SetRow(carbohydrateType_UComboBox, 2);
            Grid.SetColumn(carbohydrateType_UComboBox, 0);
            #endregion

            //Add Controls to MiniGrid
            utilitiesOmniFinderPage_MiniGrid.Children.Add(chargeCarrier_UComboBox);
            utilitiesOmniFinderPage_MiniGrid.Children.Add(carbohydrateType_UComboBox);
        }

        private void SugarsBox()
        {
            //"Sugars" Label
            Label sugars_Label = new Label();
            sugars_Label.Content = "Sugars";
            sugars_Label.Style = (Style)Application.Current.Resources["tertiaryHeaderLabelTextStyle"];
            Grid.SetRow(sugars_Label, 0);
            Grid.SetColumn(sugars_Label, 0);

            #region Sugars White Box
            //White Outline for Sugars Option Box
            GeometryGroup sugars_Geometry = new GeometryGroup();

            LineGeometry sugars_LineGeometry1 = new LineGeometry();
            sugars_LineGeometry1.StartPoint = new Point(115, 20);
            sugars_LineGeometry1.EndPoint = new Point(10, 20);

            LineGeometry sugars_LineGeometry2 = new LineGeometry();
            sugars_LineGeometry2.StartPoint = new Point(10, 20);
            sugars_LineGeometry2.EndPoint = new Point(10, 255);

            LineGeometry sugars_LineGeometry3 = new LineGeometry();
            sugars_LineGeometry3.StartPoint = new Point(10, 255);
            sugars_LineGeometry3.EndPoint = new Point(285, 255);

            LineGeometry sugars_LineGeometry4 = new LineGeometry();
            sugars_LineGeometry4.StartPoint = new Point(285, 255);
            sugars_LineGeometry4.EndPoint = new Point(285, 20);

            LineGeometry sugars_LineGeometry5 = new LineGeometry();
            sugars_LineGeometry5.StartPoint = new Point(285, 20);
            sugars_LineGeometry5.EndPoint = new Point(175, 20);

            sugars_Geometry.Children.Add(sugars_LineGeometry1);
            sugars_Geometry.Children.Add(sugars_LineGeometry2);
            sugars_Geometry.Children.Add(sugars_LineGeometry3);
            sugars_Geometry.Children.Add(sugars_LineGeometry4);
            sugars_Geometry.Children.Add(sugars_LineGeometry5);

            Path sugars_Path = new Path();
            sugars_Path.Stroke = new SolidColorBrush(Colors.White);
            sugars_Path.StrokeThickness = 3.0;
            sugars_Path.Data = sugars_Geometry;
            Grid.SetRow(sugars_Path, 0);
            Grid.SetColumn(sugars_Path, 0);
            #endregion

            //Add Box to MiniGrid
            utilitiesOmniFinderPage_MiniGrid.Children.Add(sugars_Label);
            utilitiesOmniFinderPage_MiniGrid.Children.Add(sugars_Path);
        }

        private void UsersBox()
        {
            //"User" Label
            Label user_Label = new Label();
            user_Label.Content = "Users";
            user_Label.Style = (Style)Application.Current.Resources["tertiaryHeaderLabelTextStyle"];
            Grid.SetRow(user_Label, 1);
            Grid.SetColumn(user_Label, 0);

            #region Users White Box
            //White Outline for User Option Box
            GeometryGroup user_Geometry = new GeometryGroup();

            LineGeometry user_LineGeometry1 = new LineGeometry();
            user_LineGeometry1.StartPoint = new Point(120, 18);
            user_LineGeometry1.EndPoint = new Point(10, 18);

            LineGeometry user_LineGeometry2 = new LineGeometry();
            user_LineGeometry2.StartPoint = new Point(10, 18);
            user_LineGeometry2.EndPoint = new Point(10, 85);

            LineGeometry user_LineGeometry3 = new LineGeometry();
            user_LineGeometry3.StartPoint = new Point(10, 85);
            user_LineGeometry3.EndPoint = new Point(285, 85);

            LineGeometry user_LineGeometry4 = new LineGeometry();
            user_LineGeometry4.StartPoint = new Point(285, 85);
            user_LineGeometry4.EndPoint = new Point(285, 18);

            LineGeometry user_LineGeometry5 = new LineGeometry();
            user_LineGeometry5.StartPoint = new Point(285, 18);
            user_LineGeometry5.EndPoint = new Point(170, 18);

            user_Geometry.Children.Add(user_LineGeometry1);
            user_Geometry.Children.Add(user_LineGeometry2);
            user_Geometry.Children.Add(user_LineGeometry3);
            user_Geometry.Children.Add(user_LineGeometry4);
            user_Geometry.Children.Add(user_LineGeometry5);

            Path user_Path = new Path();
            user_Path.Stroke = new SolidColorBrush(Colors.White);
            user_Path.StrokeThickness = 3.0;
            user_Path.Data = user_Geometry;
            Grid.SetRow(user_Path, 1);
            Grid.SetColumn(user_Path, 0);
            #endregion

            //Add Box to MiniGrid
            utilitiesOmniFinderPage_MiniGrid.Children.Add(user_Label);
            utilitiesOmniFinderPage_MiniGrid.Children.Add(user_Path);
        }

        private void SpecialsBox()
        {
            //"Special" Label
            Label special_Label = new Label();
            special_Label.Content = "Special";
            special_Label.Margin = new Thickness(0, 82, 0, 0);
            special_Label.Style = (Style)Application.Current.Resources["tertiaryHeaderLabelTextStyle"];
            Grid.SetRow(special_Label, 1);
            Grid.SetColumn(special_Label, 0);

            #region Specials White Box
            //White Outline for Special Option Box
            GeometryGroup special_Geometry = new GeometryGroup();

            LineGeometry special_LineGeometry1 = new LineGeometry();
            special_LineGeometry1.StartPoint = new Point(115, 100);
            special_LineGeometry1.EndPoint = new Point(10, 100);

            LineGeometry special_LineGeometry2 = new LineGeometry();
            special_LineGeometry2.StartPoint = new Point(10, 100);
            special_LineGeometry2.EndPoint = new Point(10, 175);

            LineGeometry special_LineGeometry3 = new LineGeometry();
            special_LineGeometry3.StartPoint = new Point(10, 175);
            special_LineGeometry3.EndPoint = new Point(285, 175);

            LineGeometry special_LineGeometry4 = new LineGeometry();
            special_LineGeometry4.StartPoint = new Point(285, 175);
            special_LineGeometry4.EndPoint = new Point(285, 100);

            LineGeometry special_LineGeometry5 = new LineGeometry();
            special_LineGeometry5.StartPoint = new Point(285, 100);
            special_LineGeometry5.EndPoint = new Point(175, 100);

            special_Geometry.Children.Add(special_LineGeometry1);
            special_Geometry.Children.Add(special_LineGeometry2);
            special_Geometry.Children.Add(special_LineGeometry3);
            special_Geometry.Children.Add(special_LineGeometry4);
            special_Geometry.Children.Add(special_LineGeometry5);

            Path special_Path = new Path();
            special_Path.Stroke = new SolidColorBrush(Colors.White);
            special_Path.StrokeThickness = 3.0;
            special_Path.Data = special_Geometry;
            Grid.SetRow(special_Path, 1);
            Grid.SetColumn(special_Path, 0);
            #endregion

            //Add Box to MiniGrid
            utilitiesOmniFinderPage_MiniGrid.Children.Add(special_Label);
            utilitiesOmniFinderPage_MiniGrid.Children.Add(special_Path);
        }

        private void AminoAcidsBox()
        {
            //"Amino Acids" Label
            Label aminoAcids_Label = new Label();
            aminoAcids_Label.Content = "Amino Acids";
            aminoAcids_Label.Style = (Style)Application.Current.Resources["tertiaryHeaderLabelTextStyle"];
            Grid.SetColumn(aminoAcids_Label, 1);
            Grid.SetRow(aminoAcids_Label, 0);

            #region Amino Acids Box
            //White Outline for Amino Acids Option Box
            GeometryGroup aminoAcids_Geometry = new GeometryGroup();

            LineGeometry aminoAcids_LineGeometry1 = new LineGeometry();
            aminoAcids_LineGeometry1.StartPoint = new Point(120, 20);
            aminoAcids_LineGeometry1.EndPoint = new Point(10, 20);

            LineGeometry aminoAcids_LineGeometry2 = new LineGeometry();
            aminoAcids_LineGeometry2.StartPoint = new Point(10, 20);
            aminoAcids_LineGeometry2.EndPoint = new Point(10, 255);

            LineGeometry aminoAcids_LineGeometry3 = new LineGeometry();
            aminoAcids_LineGeometry3.StartPoint = new Point(10, 255);
            aminoAcids_LineGeometry3.EndPoint = new Point(330, 255);

            LineGeometry aminoAcids_LineGeometry4 = new LineGeometry();
            aminoAcids_LineGeometry4.StartPoint = new Point(330, 255);
            aminoAcids_LineGeometry4.EndPoint = new Point(330, 20);

            LineGeometry aminoAcids_LineGeometry5 = new LineGeometry();
            aminoAcids_LineGeometry5.StartPoint = new Point(330, 20);
            aminoAcids_LineGeometry5.EndPoint = new Point(220, 20);

            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry1);
            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry2);
            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry3);
            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry4);
            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry5);

            Path aminoAcids_Path = new Path();
            aminoAcids_Path.Stroke = new SolidColorBrush(Colors.White);
            aminoAcids_Path.StrokeThickness = 3.0;
            aminoAcids_Path.Data = aminoAcids_Geometry;
            Grid.SetColumn(aminoAcids_Path, 1);
            Grid.SetRow(aminoAcids_Path, 0);
            #endregion

            //Add Box to MiniGrid
            utilitiesOmniFinderPage_MiniGrid.Children.Add(aminoAcids_Label);
            utilitiesOmniFinderPage_MiniGrid.Children.Add(aminoAcids_Path);
        }

        private void PermethylBox()
        {
            //"Permethyl" Label
            Label permethyl_Label = new Label();
            permethyl_Label.Content = "Permethyl";
            permethyl_Label.Style = (Style)Application.Current.Resources["tertiaryHeaderLabelTextStyle"];
            Grid.SetRow(permethyl_Label, 1);
            Grid.SetColumn(permethyl_Label, 1);

            #region Permethyl White Box
            //White Outline for Permethyl Option Box
            GeometryGroup permethyl_Geometry = new GeometryGroup();

            LineGeometry permethyl_LineGeometry1 = new LineGeometry();
            permethyl_LineGeometry1.StartPoint = new Point(125, 20);
            permethyl_LineGeometry1.EndPoint = new Point(10, 20);

            LineGeometry permethyl_LineGeometry2 = new LineGeometry();
            permethyl_LineGeometry2.StartPoint = new Point(10, 20);
            permethyl_LineGeometry2.EndPoint = new Point(10, 175);

            LineGeometry permethyl_LineGeometry3 = new LineGeometry();
            permethyl_LineGeometry3.StartPoint = new Point(10, 175);
            permethyl_LineGeometry3.EndPoint = new Point(330, 175);

            LineGeometry permethyl_LineGeometry4 = new LineGeometry();
            permethyl_LineGeometry4.StartPoint = new Point(330, 175);
            permethyl_LineGeometry4.EndPoint = new Point(330, 20);

            LineGeometry permethyl_LineGeometry5 = new LineGeometry();
            permethyl_LineGeometry5.StartPoint = new Point(330, 20);
            permethyl_LineGeometry5.EndPoint = new Point(215, 20);

            permethyl_Geometry.Children.Add(permethyl_LineGeometry1);
            permethyl_Geometry.Children.Add(permethyl_LineGeometry2);
            permethyl_Geometry.Children.Add(permethyl_LineGeometry3);
            permethyl_Geometry.Children.Add(permethyl_LineGeometry4);
            permethyl_Geometry.Children.Add(permethyl_LineGeometry5);

            Path permethyl_Path = new Path();
            permethyl_Path.Stroke = new SolidColorBrush(Colors.White);
            permethyl_Path.StrokeThickness = 3.0;
            permethyl_Path.Data = permethyl_Geometry;
            Grid.SetRow(permethyl_Path, 1);
            Grid.SetColumn(permethyl_Path, 1);
            #endregion

            //Add Box to MiniGrid
            utilitiesOmniFinderPage_MiniGrid.Children.Add(permethyl_Label);
            utilitiesOmniFinderPage_MiniGrid.Children.Add(permethyl_Path);
        }

        private void ChargeCarrierBox()
        {
            //White Outline for Permethyl Option Box
            GeometryGroup chargeCarrier_Geometry = new GeometryGroup();

            LineGeometry chargeCarrier_LineGeometry1 = new LineGeometry();
            chargeCarrier_LineGeometry1.StartPoint = new Point(10, 5);
            chargeCarrier_LineGeometry1.EndPoint = new Point(10, 135);

            LineGeometry chargeCarrier_LineGeometry2 = new LineGeometry();
            chargeCarrier_LineGeometry2.StartPoint = new Point(10, 135);
            chargeCarrier_LineGeometry2.EndPoint = new Point(285, 135);

            LineGeometry chargeCarrier_LineGeometry3 = new LineGeometry();
            chargeCarrier_LineGeometry3.StartPoint = new Point(285, 135);
            chargeCarrier_LineGeometry3.EndPoint = new Point(285, 5);

            LineGeometry chargeCarrier_LineGeometry4 = new LineGeometry();
            chargeCarrier_LineGeometry4.StartPoint = new Point(285, 5);
            chargeCarrier_LineGeometry4.EndPoint = new Point(10, 5);

            chargeCarrier_Geometry.Children.Add(chargeCarrier_LineGeometry1);
            chargeCarrier_Geometry.Children.Add(chargeCarrier_LineGeometry2);
            chargeCarrier_Geometry.Children.Add(chargeCarrier_LineGeometry3);
            chargeCarrier_Geometry.Children.Add(chargeCarrier_LineGeometry4);

            Path chargeCarrier_Path = new Path();
            chargeCarrier_Path.Stroke = new SolidColorBrush(Colors.White);
            chargeCarrier_Path.StrokeThickness = 3.0;
            chargeCarrier_Path.Data = chargeCarrier_Geometry;
            Grid.SetRow(chargeCarrier_Path, 2);
            Grid.SetColumn(chargeCarrier_Path, 0);

            //Add Box to MiniGrid
            utilitiesOmniFinderPage_MiniGrid.Children.Add(chargeCarrier_Path);
        }

        private void ResultsBox()
        {
            //White Outline for Permethyl Option Box
            GeometryGroup results_Geometry = new GeometryGroup();

            LineGeometry results_LineGeometry1 = new LineGeometry();
            results_LineGeometry1.StartPoint = new Point(10, 5);
            results_LineGeometry1.EndPoint = new Point(10, 135);

            LineGeometry results_LineGeometry2 = new LineGeometry();
            results_LineGeometry2.StartPoint = new Point(10, 135);
            results_LineGeometry2.EndPoint = new Point(330, 135);

            LineGeometry results_LineGeometry3 = new LineGeometry();
            results_LineGeometry3.StartPoint = new Point(330, 135);
            results_LineGeometry3.EndPoint = new Point(330, 5);

            LineGeometry results_LineGeometry4 = new LineGeometry();
            results_LineGeometry4.StartPoint = new Point(330, 5);
            results_LineGeometry4.EndPoint = new Point(10, 5);

            results_Geometry.Children.Add(results_LineGeometry1);
            results_Geometry.Children.Add(results_LineGeometry2);
            results_Geometry.Children.Add(results_LineGeometry3);
            results_Geometry.Children.Add(results_LineGeometry4);

            Path results_Path = new Path();
            results_Path.Stroke = new SolidColorBrush(Colors.White);
            results_Path.StrokeThickness = 3.0;
            results_Path.Data = results_Geometry;
            Grid.SetRow(results_Path, 2);
            Grid.SetColumn(results_Path, 1);

            //Add Box to MiniGrid
            utilitiesOmniFinderPage_MiniGrid.Children.Add(results_Path);
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            clearAllOmniFinderControls();
        }

        private void calculateMassButton_Click(object sender, RoutedEventArgs e)
        {
            GlycanMakerObject setMeUp = new GlycanMakerObject();

            if (App.glycanMakerVariables.NumberOfChargeCarrier_Int == 0)
                setMeUp.Charge = 1;
            else
                setMeUp.Charge = App.glycanMakerVariables.NumberOfChargeCarrier_Int;
            setMeUp.MassTollerance = 0; //place holder since we do not need it for the glycan maker

            //Type of Charge Carrier
            switch (App.glycanMakerVariables.TypeOfChargeCarrier_String)
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

            //Carbohydrate Type
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

            //Sugars
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

            //User Units
            double mass1 = App.glycanMakerVariables.MassOfUserUnit1_Double;
            double mass2 = App.glycanMakerVariables.MassOfUserUnit2_Double;

            UserUnitLibrary myLibrary = new UserUnitLibrary();
            UserUnit unit1 = new UserUnit("user1", "u1", mass1, UserUnitName.User01);
            UserUnit unit2 = new UserUnit("user2", "u2", mass2, UserUnitName.User02);
            UserUnit unit3 = new UserUnit("user3", "u3", 0, UserUnitName.User03);
            myLibrary.SetLibrary(unit1, unit2, unit3);
            setMeUp.OmniFinderParameter.UserUnitLibrary = myLibrary;

            BuildingBlock user1Block = new BuildingBlockUserUnit(UserUnitName.User01, 
                new RangesMinMax(App.glycanMakerVariables.NumberOfUserUnit1_Int, App.glycanMakerVariables.NumberOfUserUnit1_Int));
            BuildingBlock user2Block = new BuildingBlockUserUnit(UserUnitName.User02,
                new RangesMinMax(App.glycanMakerVariables.NumberOfUserUnit2_Int, App.glycanMakerVariables.NumberOfUserUnit2_Int));

            //Special
            BuildingBlock naHBlock = new BuildingBlockMiscellaneousMatter(MiscellaneousMatterName.NaMinusH,
                new RangesMinMax(App.glycanMakerVariables.NumberOfNaH_Int, App.glycanMakerVariables.NumberOfNaH_Int));
            BuildingBlock cH3Block = new BuildingBlockMiscellaneousMatter(MiscellaneousMatterName.Methyl,
                new RangesMinMax(App.glycanMakerVariables.NumberOfCH3_Int, App.glycanMakerVariables.NumberOfCH3_Int));
            BuildingBlock sO3Block = new BuildingBlockMiscellaneousMatter(MiscellaneousMatterName.Sulfate,
                new RangesMinMax(App.glycanMakerVariables.NumberOfSO3_Int, App.glycanMakerVariables.NumberOfSO3_Int));
            BuildingBlock oAcetylBlock = new BuildingBlockMiscellaneousMatter(MiscellaneousMatterName.OAcetyl,
                new RangesMinMax(App.glycanMakerVariables.NumberOfOAcetyl_Int, App.glycanMakerVariables.NumberOfOAcetyl_Int));

            //Amino Acids
            BuildingBlock alaBlock = new BuildingBlockAminoAcid(AminoAcidName.Alanine, 
                new RangesMinMax(App.glycanMakerVariables.NumberOfAla_Int, App.glycanMakerVariables.NumberOfAla_Int));
            BuildingBlock argBlock = new BuildingBlockAminoAcid(AminoAcidName.Arginine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfArg_Int, App.glycanMakerVariables.NumberOfArg_Int));
            BuildingBlock asnBlock = new BuildingBlockAminoAcid(AminoAcidName.Asparagine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfAsn_Int, App.glycanMakerVariables.NumberOfAsn_Int));
            BuildingBlock aspBlock = new BuildingBlockAminoAcid(AminoAcidName.AsparticAcid,
                new RangesMinMax(App.glycanMakerVariables.NumberOfAsp_Int, App.glycanMakerVariables.NumberOfAsp_Int));
            BuildingBlock cysBlock = new BuildingBlockAminoAcid(AminoAcidName.Cysteine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfCys_Int, App.glycanMakerVariables.NumberOfCys_Int));
            BuildingBlock glnBlock = new BuildingBlockAminoAcid(AminoAcidName.Glutamine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfGln_Int, App.glycanMakerVariables.NumberOfGln_Int));//??????????????????????????????
            BuildingBlock gluBlock = new BuildingBlockAminoAcid(AminoAcidName.GlutamicAcid,
                new RangesMinMax(App.glycanMakerVariables.NumberOfGlu_Int, App.glycanMakerVariables.NumberOfGlu_Int));//??????????????????????????????
            BuildingBlock glyBlock = new BuildingBlockAminoAcid(AminoAcidName.Glycine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfGly_Int, App.glycanMakerVariables.NumberOfGly_Int));
            BuildingBlock hisBlock = new BuildingBlockAminoAcid(AminoAcidName.Histidine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfHis_Int, App.glycanMakerVariables.NumberOfHis_Int));
            BuildingBlock ileBlock = new BuildingBlockAminoAcid(AminoAcidName.Isoleucine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfIle_Int, App.glycanMakerVariables.NumberOfIle_Int));
            BuildingBlock leuBlock = new BuildingBlockAminoAcid(AminoAcidName.Leucine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfLeu_Int, App.glycanMakerVariables.NumberOfLeu_Int));
            BuildingBlock lysBlock = new BuildingBlockAminoAcid(AminoAcidName.Lysine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfLys_Int, App.glycanMakerVariables.NumberOfLys_Int));
            BuildingBlock metBlock = new BuildingBlockAminoAcid(AminoAcidName.Methionine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfMet_Int, App.glycanMakerVariables.NumberOfMet_Int));
            BuildingBlock pheBlock = new BuildingBlockAminoAcid(AminoAcidName.Phenylalanine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfPhe_Int, App.glycanMakerVariables.NumberOfPhe_Int));
            BuildingBlock serBlock = new BuildingBlockAminoAcid(AminoAcidName.Serine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfSer_Int, App.glycanMakerVariables.NumberOfSer_Int));
            BuildingBlock thrBlock = new BuildingBlockAminoAcid(AminoAcidName.Threonine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfThr_Int, App.glycanMakerVariables.NumberOfThr_Int));
            BuildingBlock trpBlock = new BuildingBlockAminoAcid(AminoAcidName.Tryptophan,
                new RangesMinMax(App.glycanMakerVariables.NumberOfTrp_Int, App.glycanMakerVariables.NumberOfTrp_Int));
            BuildingBlock tyrBlock = new BuildingBlockAminoAcid(AminoAcidName.Tyrosine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfTyr_Int, App.glycanMakerVariables.NumberOfTyr_Int));
            BuildingBlock valBlock = new BuildingBlockAminoAcid(AminoAcidName.Valine,
                new RangesMinMax(App.glycanMakerVariables.NumberOfVal_Int, App.glycanMakerVariables.NumberOfVal_Int));
            BuildingBlock proBlock = new BuildingBlockAminoAcid(AminoAcidName.Proline,
                new RangesMinMax(App.glycanMakerVariables.NumberOfPro_Int, App.glycanMakerVariables.NumberOfPro_Int));

            //Permethyl

            
            //Add Sugars, User Units, Specials, Amino Acids to SetMeUp
            setMeUp.LegoBuildingBlocks.Add(hexoseBlock);
            setMeUp.LegoBuildingBlocks.Add(deoxyhexoseBlock);
            setMeUp.LegoBuildingBlocks.Add(hexNAcBlock);
            setMeUp.LegoBuildingBlocks.Add(pentoseBlock);
            setMeUp.LegoBuildingBlocks.Add(neuAcBlock);
            setMeUp.LegoBuildingBlocks.Add(neuGcBlock);
            setMeUp.LegoBuildingBlocks.Add(kdnBlock);
            setMeUp.LegoBuildingBlocks.Add(hexABlock);

            setMeUp.LegoBuildingBlocks.Add(user1Block);
            setMeUp.LegoBuildingBlocks.Add(user2Block);

            setMeUp.LegoBuildingBlocks.Add(naHBlock);
            setMeUp.LegoBuildingBlocks.Add(cH3Block);
            setMeUp.LegoBuildingBlocks.Add(sO3Block);
            setMeUp.LegoBuildingBlocks.Add(oAcetylBlock);

            setMeUp.LegoBuildingBlocks.Add(alaBlock);
            setMeUp.LegoBuildingBlocks.Add(argBlock);
            setMeUp.LegoBuildingBlocks.Add(asnBlock);
            setMeUp.LegoBuildingBlocks.Add(aspBlock);
            setMeUp.LegoBuildingBlocks.Add(cysBlock);
            setMeUp.LegoBuildingBlocks.Add(glnBlock);
            setMeUp.LegoBuildingBlocks.Add(gluBlock);
            setMeUp.LegoBuildingBlocks.Add(glyBlock);
            setMeUp.LegoBuildingBlocks.Add(hisBlock);
            setMeUp.LegoBuildingBlocks.Add(ileBlock);
            setMeUp.LegoBuildingBlocks.Add(leuBlock);
            setMeUp.LegoBuildingBlocks.Add(lysBlock);
            setMeUp.LegoBuildingBlocks.Add(metBlock);
            setMeUp.LegoBuildingBlocks.Add(pheBlock);
            setMeUp.LegoBuildingBlocks.Add(serBlock);
            setMeUp.LegoBuildingBlocks.Add(thrBlock);
            setMeUp.LegoBuildingBlocks.Add(trpBlock);
            setMeUp.LegoBuildingBlocks.Add(tyrBlock);
            setMeUp.LegoBuildingBlocks.Add(valBlock);
            setMeUp.LegoBuildingBlocks.Add(proBlock);

            GlycanMakerObject inputForGlycanMaker = setMeUp;
            GlycanMakerOutput results = GlycanMakerController.CalculateMass(inputForGlycanMaker, inputForGlycanMaker.Charge);

            //Neutral Mass get, set, output
            App.glycanMakerVariables.NeutralMass_Double = Convert.ToDouble(results.MassNeutral);
            neutralMass_UTextBox.Text = (App.glycanMakerVariables.NeutralMass_Double).ToString();

            //Mass/Charge get, set, output
            App.glycanMakerVariables.MassCharge_Double = Convert.ToDouble(results.MassToCharge);
            massCharge_UTextBox.Text = (App.glycanMakerVariables.MassCharge_Double).ToString();

            //C, H, O, N, Na
        }

        private void rangesButton_Click(object sender, RoutedEventArgs e)
        {
            App.utilitiesOmniFinderPage.GoToRangesPage();
        }

        public void selectOptions_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (App.initializePages.UtilitiesOmniFinderPageDesign_InitializeFlag != true)
            {
                ComboBox changedComboBox = (ComboBox)sender;
                switch (changedComboBox.Name)
                {
                    case "selectOptions_UComboBox":
                        {
                            App.utilitiesOmniFinderVariables.SelectedOption_String = (selectOptions_UComboBox.SelectedValue.ToString()).Substring(38);
                            String SelectedOption_Temp = (selectOptions_UComboBox.SelectedValue.ToString()).Substring(38);
                            switch (SelectedOption_Temp)
                            {
                                case "N Glycans":
                                    {
                                        App.utilitiesOmniFinderVariables.CheckedHexose_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedNaH_Bool = true;
                                        hexose_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexose_Bool;
                                        hexNAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool;
                                        dxyHex_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool;
                                        neuAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool;
                                        naH_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNaH_Bool;
                                        break;
                                    }
                                case "Amino Acids":
                                    {
                                        App.utilitiesOmniFinderVariables.CheckedAla_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedArg_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedAsn_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedAsp_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedCys_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedGln_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedGlu_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedGly_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedHis_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedIle_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedLeu_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedLys_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedMet_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedPhe_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedSer_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedThr_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedTrp_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedTyr_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedVal_Bool = true;
                                        App.utilitiesOmniFinderVariables.CheckedPro_Bool = true;
                                        ala_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAla_Bool;
                                        arg_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedArg_Bool;
                                        asn_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAsn_Bool;
                                        asp_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAsp_Bool;
                                        cys_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedCys_Bool;
                                        gln_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGln_Bool;
                                        glu_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGlu_Bool;
                                        gly_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGly_Bool;
                                        his_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHis_Bool;
                                        ile_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedIle_Bool;
                                        leu_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedLeu_Bool;
                                        lys_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedLys_Bool;
                                        met_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedMet_Bool;
                                        phe_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPhe_Bool;
                                        ser_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedSer_Bool;
                                        thr_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedThr_Bool;
                                        trp_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedTrp_Bool;
                                        tyr_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedTyr_Bool;
                                        val_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedVal_Bool;
                                        pro_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPro_Bool;
                                        break;
                                    }
                                case "No Option Selected":
                                    {
                                        App.utilitiesOmniFinderVariables.CheckedHexose_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedNaH_Bool = false;
                                        hexose_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexose_Bool;
                                        hexNAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool;
                                        dxyHex_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool;
                                        neuAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool;
                                        naH_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNaH_Bool;

                                        App.utilitiesOmniFinderVariables.CheckedAla_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedArg_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedAsn_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedAsp_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedCys_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedGln_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedGlu_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedGly_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedHis_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedIle_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedLeu_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedLys_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedMet_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedPhe_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedSer_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedThr_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedTrp_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedTyr_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedVal_Bool = false;
                                        App.utilitiesOmniFinderVariables.CheckedPro_Bool = false;
                                        ala_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAla_Bool;
                                        arg_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedArg_Bool;
                                        asn_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAsn_Bool;
                                        asp_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAsp_Bool;
                                        cys_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedCys_Bool;
                                        gln_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGln_Bool;
                                        glu_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGlu_Bool;
                                        gly_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGly_Bool;
                                        his_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHis_Bool;
                                        ile_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedIle_Bool;
                                        leu_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedLeu_Bool;
                                        lys_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedLys_Bool;
                                        met_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedMet_Bool;
                                        phe_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPhe_Bool;
                                        ser_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedSer_Bool;
                                        thr_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedThr_Bool;
                                        trp_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedTrp_Bool;
                                        tyr_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedTyr_Bool;
                                        val_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedVal_Bool;
                                        pro_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPro_Bool;
                                        break;
                                    }
                            }
                            break;
                        }

                    //Charge Carrier 
                    case "chargeCarrier_UComboBox":
                        {
                            App.glycanMakerVariables.TypeOfChargeCarrier_String = (chargeCarrier_UComboBox.SelectedValue.ToString()).Substring(38);
                            String TypeOfChargeCarrier_Temp = (chargeCarrier_UComboBox.SelectedValue.ToString()).Substring(38);
                            if (TypeOfChargeCarrier_Temp != "")
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
                                    if (s[j] == TypeOfChargeCarrier_Temp)
                                        index = j;
                                }
                                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                                chargeCarrier_UComboBox.SelectedIndex = index;
                            }
                            else
                                chargeCarrier_UComboBox.SelectedIndex = 0;
                            break;
                        }
                    //Carbohydrate Type
                    case "carbohydrateType_UComboBox":
                        {
                            App.glycanMakerVariables.CarbohydrateType_String = (carbohydrateType_UComboBox.SelectedValue.ToString()).Substring(38);
                            String CarbohydrateType_Temp = (carbohydrateType_UComboBox.SelectedValue.ToString()).Substring(38);
                            if (CarbohydrateType_Temp != "")
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
                                    if (s[j] == CarbohydrateType_Temp)
                                        index = j;
                                }
                                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                                carbohydrateType_UComboBox.SelectedIndex = index;
                            }
                            else
                                carbohydrateType_UComboBox.SelectedIndex = 0;
                            break;
                        }
                }
            }
        }

        private void naH_CheckBox_Click(Object sender, EventArgs e)
        {
            if (App.initializePages.UtilitiesOmniFinderPageDesign_InitializeFlag != true)
            {
                CheckBox checkedCheckBox = (CheckBox)sender;
                switch (checkedCheckBox.Name)
                {
                    //Sugars
                    case "hexose_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedHexose_Bool = (Boolean)checkedCheckBox.IsChecked;
                            hexose_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexose_Bool;
                            break;
                        }
                    case "hexNAc_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            hexNAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool;
                            break;
                        }
                    case "dxyHex_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool = (Boolean)checkedCheckBox.IsChecked;
                            dxyHex_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool;
                            break;
                        }
                    case "pentose_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedPentose_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pentose_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPentose_Bool;
                            break;
                        }
                    case "neuAc_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            neuAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool;
                            break;
                        }
                    case "neuGc_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedNeuGc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            neuGc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNeuGc_Bool;
                            break;
                        }
                    case "kDN_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedKDN_Bool = (Boolean)checkedCheckBox.IsChecked;
                            kDN_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedKDN_Bool;
                            break;
                        }
                    case "hexA_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedHexA_Bool = (Boolean)checkedCheckBox.IsChecked;
                            hexA_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexA_Bool;
                            break;
                        }

                    //User Units
                    case "userUnitA_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedUserUnit1_Bool = (Boolean)checkedCheckBox.IsChecked;
                            userUnitA_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedUserUnit1_Bool;
                            break;
                        }
                    case "userUnitB_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedUserUnit2_Bool = (Boolean)checkedCheckBox.IsChecked;
                            userUnitB_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedUserUnit2_Bool;
                            break;
                        }

                    //Special
                    case "naH_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedNaH_Bool = (Boolean)checkedCheckBox.IsChecked;
                            naH_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNaH_Bool;
                            break;
                        }
                    case "cH3_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedCH3_Bool = (Boolean)checkedCheckBox.IsChecked;
                            cH3_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedCH3_Bool;
                            break;
                        }
                    case "sO3_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedSO3_Bool = (Boolean)checkedCheckBox.IsChecked;
                            sO3_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedSO3_Bool;
                            break;
                        }
                    case "oAcetyl_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedOAcetyl_Bool = (Boolean)checkedCheckBox.IsChecked;
                            oAcetyl_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedOAcetyl_Bool;
                            break;
                        }


                    //Amino Acids
                    case "ala_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedAla_Bool = (Boolean)checkedCheckBox.IsChecked;
                            ala_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAla_Bool;
                            break;
                        }
                    case "arg_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedArg_Bool = (Boolean)checkedCheckBox.IsChecked;
                            arg_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedArg_Bool;
                            break;
                        }
                    case "asn_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedAsn_Bool = (Boolean)checkedCheckBox.IsChecked;
                            asn_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAsn_Bool;
                            break;
                        }
                    case "asp_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedAsp_Bool = (Boolean)checkedCheckBox.IsChecked;
                            asp_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAsp_Bool;
                            break;
                        }
                    case "cys_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedCys_Bool = (Boolean)checkedCheckBox.IsChecked;
                            cys_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedCys_Bool;
                            break;
                        }
                    case "gln_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedGln_Bool = (Boolean)checkedCheckBox.IsChecked;
                            gln_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGln_Bool;
                            break;
                        }
                    case "glu_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedGlu_Bool = (Boolean)checkedCheckBox.IsChecked;
                            glu_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGlu_Bool;
                            break;
                        }
                    case "gly_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedGly_Bool = (Boolean)checkedCheckBox.IsChecked;
                            gly_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGly_Bool;
                            break;
                        }
                    case "his_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedHis_Bool = (Boolean)checkedCheckBox.IsChecked;
                            his_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHis_Bool;
                            break;
                        }
                    case "ile_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedIle_Bool = (Boolean)checkedCheckBox.IsChecked;
                            ile_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedIle_Bool;
                            break;
                        }
                    case "leu_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedLeu_Bool = (Boolean)checkedCheckBox.IsChecked;
                            leu_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedLeu_Bool;
                            break;
                        }
                    case "lys_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedLys_Bool = (Boolean)checkedCheckBox.IsChecked;
                            lys_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedLys_Bool;
                            break;
                        }
                    case "met_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedMet_Bool = (Boolean)checkedCheckBox.IsChecked;
                            met_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedMet_Bool;
                            break;
                        }
                    case "phe_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedPhe_Bool = (Boolean)checkedCheckBox.IsChecked;
                            phe_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPhe_Bool;
                            break;
                        }
                    case "ser_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedSer_Bool = (Boolean)checkedCheckBox.IsChecked;
                            ser_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedSer_Bool;
                            break;
                        }
                    case "thr_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedThr_Bool = (Boolean)checkedCheckBox.IsChecked;
                            thr_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedThr_Bool;
                            break;
                        }
                    case "trp_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedTrp_Bool = (Boolean)checkedCheckBox.IsChecked;
                            trp_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedTrp_Bool;
                            break;
                        }
                    case "tyr_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedTyr_Bool = (Boolean)checkedCheckBox.IsChecked;
                            tyr_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedTyr_Bool;
                            break;
                        }
                    case "val_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedVal_Bool = (Boolean)checkedCheckBox.IsChecked;
                            val_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedVal_Bool;
                            break;
                        }
                    case "pro_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedPro_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pro_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPro_Bool;
                            break;
                        }

                    //Permethyl
                    case "pHex_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedpHex_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pHex_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpHex_Bool;
                            break;
                        }
                    case "pHxNAc_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedpHxNAc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pHxNAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpHxNAc_Bool;
                            break;
                        }
                    case "pDxHex_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedpDxHex_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pDxHex_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpDxHex_Bool;
                            break;
                        }
                    case "pPntos_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedpPntos_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pPntos_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpPntos_Bool;
                            break;
                        }
                    case "pNuAc_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedpNuAc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pNuAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpNuAc_Bool;
                            break;
                        }
                    case "pNuGc_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedpNuGc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pNuGc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpNuGc_Bool;
                            break;
                        }
                    case "pKDN_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedpKDN_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pKDN_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpKDN_Bool;
                            break;
                        }
                    case "pHxA_UCheckBox":
                        {
                            App.utilitiesOmniFinderVariables.CheckedpHxA_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pHxA_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpHxA_Bool;
                            break;
                        }
                }
            }
        }

        protected void hexose_UTextBox_TextChanged(object sender, EventArgs e)
        {
            if (App.initializePages.UtilitiesOmniFinderPageDesign_InitializeFlag != true)
            {
                //Questionable???????????????????????????????????????????????????????????????????????????????????????????????????????????????
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
                    //Sugars
                    case "hexose_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfHexose_Int = tempText;
                            hexose_UTextBox.Text = (App.glycanMakerVariables.NumberOfHexose_Int).ToString();
                            break;
                        }
                    case "hexNAc_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfHexNAc_Int = tempText;
                            hexNAc_UTextBox.Text = (App.glycanMakerVariables.NumberOfHexNAc_Int).ToString();
                            break;
                        }
                    case "dxyHex_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfDeoxyhexose_Int = tempText;
                            dxyHex_UTextBox.Text = (App.glycanMakerVariables.NumberOfDeoxyhexose_Int).ToString();
                            break;
                        }
                    case "pentose_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfPentose_Int = tempText;
                            pentose_UTextBox.Text = (App.glycanMakerVariables.NumberOfPentose_Int).ToString();
                            break;
                        }
                    case "neuAc_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfNeuAc_Int = tempText;
                            neuAc_UTextBox.Text = (App.glycanMakerVariables.NumberOfNeuAc_Int).ToString();
                            break;
                        }

                    case "neuGc_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfNeuGc_Int = tempText;
                            neuGc_UTextBox.Text = (App.glycanMakerVariables.NumberOfNeuGc_Int).ToString();
                            break;
                        }
                    case "kDN_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfKDN_Int = tempText;
                            kDN_UTextBox.Text = (App.glycanMakerVariables.NumberOfKDN_Int).ToString();
                            break;
                        }
                    case "hexA_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfHexA_Int = tempText;
                            hexA_UTextBox.Text = (App.glycanMakerVariables.NumberOfHexA_Int).ToString();
                            break;
                        }

                    //User Units
                    case "userUnitA_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfUserUnit1_Int = tempText;
                            userUnitA_UTextBox.Text = (App.glycanMakerVariables.NumberOfUserUnit1_Int).ToString();
                            break;
                        }
                    case "userUnitB_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfUserUnit2_Int = tempText;
                            userUnitB_UTextBox.Text = (App.glycanMakerVariables.NumberOfUserUnit2_Int).ToString();
                            break;
                        }

                    //Specials
                    case "naH_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfNaH_Int = tempText;
                            naH_UTextBox.Text = (App.glycanMakerVariables.NumberOfNaH_Int).ToString();
                            break;
                        }
                    case "cH3_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfCH3_Int = tempText;
                            cH3_UTextBox.Text = (App.glycanMakerVariables.NumberOfCH3_Int).ToString();
                            break;
                        }
                    case "sO3_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfSO3_Int = tempText;
                            sO3_UTextBox.Text = (App.glycanMakerVariables.NumberOfSO3_Int).ToString();
                            break;
                        }
                    case "oAcetyl_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfOAcetyl_Int = tempText;
                            oAcetyl_UTextBox.Text = (App.glycanMakerVariables.NumberOfOAcetyl_Int).ToString();
                            break;
                        }

                    //Amino Acids
                    case "ala_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfAla_Int = tempText;
                            ala_UTextBox.Text = (App.glycanMakerVariables.NumberOfAla_Int).ToString();
                            break;
                        }
                    case "arg_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfArg_Int = tempText;
                            arg_UTextBox.Text = (App.glycanMakerVariables.NumberOfArg_Int).ToString();
                            break;
                        }
                    case "asn_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfAsn_Int = tempText;
                            asn_UTextBox.Text = (App.glycanMakerVariables.NumberOfAsn_Int).ToString();
                            break;
                        }
                    case "asp_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfAsp_Int = tempText;
                            asp_UTextBox.Text = (App.glycanMakerVariables.NumberOfAsp_Int).ToString();
                            break;
                        }
                    case "cys_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfCys_Int = tempText;
                            cys_UTextBox.Text = (App.glycanMakerVariables.NumberOfCys_Int).ToString();
                            break;
                        }
                    case "gln_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfGln_Int = tempText;
                            gln_UTextBox.Text = (App.glycanMakerVariables.NumberOfGln_Int).ToString();
                            break;
                        }
                    case "glu_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfGlu_Int = tempText;
                            glu_UTextBox.Text = (App.glycanMakerVariables.NumberOfGlu_Int).ToString();
                            break;
                        }
                    case "gly_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfGly_Int = tempText;
                            gly_UTextBox.Text = (App.glycanMakerVariables.NumberOfGly_Int).ToString();
                            break;
                        }
                    case "his_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfHis_Int = tempText;
                            his_UTextBox.Text = (App.glycanMakerVariables.NumberOfHis_Int).ToString();
                            break;
                        }
                    case "ile_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfIle_Int = tempText;
                            ile_UTextBox.Text = (App.glycanMakerVariables.NumberOfIle_Int).ToString();
                            break;
                        }
                    case "leu_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfLeu_Int = tempText;
                            leu_UTextBox.Text = (App.glycanMakerVariables.NumberOfLeu_Int).ToString();
                            break;
                        }
                    case "lys_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfLys_Int = tempText;
                            lys_UTextBox.Text = (App.glycanMakerVariables.NumberOfLys_Int).ToString();
                            break;
                        }
                    case "met_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfMet_Int = tempText;
                            met_UTextBox.Text = (App.glycanMakerVariables.NumberOfMet_Int).ToString();
                            break;
                        }
                    case "phe_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfPhe_Int = tempText;
                            phe_UTextBox.Text = (App.glycanMakerVariables.NumberOfPhe_Int).ToString();
                            break;
                        }
                    case "ser_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfSer_Int = tempText;
                            ser_UTextBox.Text = (App.glycanMakerVariables.NumberOfSer_Int).ToString();
                            break;
                        }
                    case "thr_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfThr_Int = tempText;
                            thr_UTextBox.Text = (App.glycanMakerVariables.NumberOfThr_Int).ToString();
                            break;
                        }
                    case "trp_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfTrp_Int = tempText;
                            trp_UTextBox.Text = (App.glycanMakerVariables.NumberOfTrp_Int).ToString();
                            break;
                        }
                    case "tyr_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfTyr_Int = tempText;
                            tyr_UTextBox.Text = (App.glycanMakerVariables.NumberOfTyr_Int).ToString();
                            break;
                        }
                    case "val_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfVal_Int = tempText;
                            val_UTextBox.Text = (App.glycanMakerVariables.NumberOfVal_Int).ToString();
                            break;
                        }
                    case "pro_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfPro_Int = tempText;
                            pro_UTextBox.Text = (App.glycanMakerVariables.NumberOfPro_Int).ToString();
                            break;
                        }

                    //Permethyl
                    case "pHex_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfpHex_Int = tempText;
                            pHex_UTextBox.Text = (App.glycanMakerVariables.NumberOfpHex_Int).ToString();
                            break;
                        }
                    case "pHxNAc_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfpHxNAc_Int = tempText;
                            pHxNAc_UTextBox.Text = (App.glycanMakerVariables.NumberOfpHxNAc_Int).ToString();
                            break;
                        }
                    case "pDxHex_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfpDxHex_Int = tempText;
                            pDxHex_UTextBox.Text = (App.glycanMakerVariables.NumberOfpDxHex_Int).ToString();
                            break;
                        }
                    case "pPntos_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfpPntos_Int = tempText;
                            pPntos_UTextBox.Text = (App.glycanMakerVariables.NumberOfpPntos_Int).ToString();
                            break;
                        }
                    case "pNuAc_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfpNuAc_Int = tempText;
                            pNuAc_UTextBox.Text = (App.glycanMakerVariables.NumberOfpNuAc_Int).ToString();
                            break;
                        }
                    case "pNuGc_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfpNuGc_Int = tempText;
                            pNuGc_UTextBox.Text = (App.glycanMakerVariables.NumberOfpNuGc_Int).ToString();
                            break;
                        }
                    case "pKDN_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfpKDN_Int = tempText;
                            pKDN_UTextBox.Text = (App.glycanMakerVariables.NumberOfpKDN_Int).ToString();
                            break;
                        }
                    case "pHxA_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfpHxA_Int = tempText;
                            pHxA_UTextBox.Text = (App.glycanMakerVariables.NumberOfpHxA_Int).ToString();
                            break;
                        }

                    //Charge Carrier
                    case "numberOfChargeCarrier_UTextBox":
                        {
                            if (tempText == 0)
                                App.glycanMakerVariables.NumberOfChargeCarrier_Int = 1;
                            else
                                App.glycanMakerVariables.NumberOfChargeCarrier_Int = tempText;
                            numberOfChargeCarrier_UTextBox.Text = (App.glycanMakerVariables.NumberOfChargeCarrier_Int).ToString();
                            break;
                        }

                    /*//Number of C, H, O, N, Na   
                    case "amountOfC_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfC_Int = tempText;
                            amountOfC_UTextBox.Text = (App.glycanMakerVariables.NumberOfC_Int).ToString();
                            break;
                        }
                    case "amountOfH_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfH_Int = tempText;
                            amountOfH_UTextBox.Text = (App.glycanMakerVariables.NumberOfH_Int).ToString();
                            break;
                        }
                    case "amountOfO_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfO_Int = tempText;
                            amountOfO_UTextBox.Text = (App.glycanMakerVariables.NumberOfO_Int).ToString();
                            break;
                        }
                    case "amountOfN_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfN_Int = tempText;
                            amountOfN_UTextBox.Text = (App.glycanMakerVariables.NumberOfN_Int).ToString();
                            break;
                        }
                    case "amountOfNa_UTextBox":
                        {
                            App.glycanMakerVariables.NumberOfNa_Int = tempText;
                            amountOfNa_UTextBox.Text = (App.glycanMakerVariables.NumberOfNa_Int).ToString();
                            break;
                        }*/
                }
            }
        }

        protected void massOfUserUnitA_UTextBox_TextChanged(object sender, EventArgs e)
        {
            if (App.initializePages.UtilitiesOmniFinderPageDesign_InitializeFlag != true)
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
                    case "massOfUserUnitA_UTextBox":
                        {
                            App.glycanMakerVariables.MassOfUserUnit1_Double = tempText;
                            massOfUserUnitA_UTextBox.Text = (App.glycanMakerVariables.MassOfUserUnit1_Double).ToString();
                            break;
                        }
                    case "massOfUserUnitB_UTextBox":
                        {
                            App.glycanMakerVariables.MassOfUserUnit2_Double = tempText;
                            massOfUserUnitB_UTextBox.Text = (App.glycanMakerVariables.MassOfUserUnit2_Double).ToString();
                            break;
                        }
                    /*//Neutral Mass                                                    
                    case "neutralMass_UTextBox":
                        {
                            App.glycanMakerVariables.NeutralMass_Double = Convert.ToDouble(tempText);
                            neutralMass_UTextBox.Text = (App.glycanMakerVariables.NeutralMass_Double).ToString();
                            break;
                        }

                    //Mass/Charge 
                    case "massCharge_UTextBox":
                        {
                            App.glycanMakerVariables.MassCharge_Double = Convert.ToDouble(tempText);
                            massCharge_UTextBox.Text = (App.glycanMakerVariables.MassCharge_Double).ToString();
                            break;
                        }*/
                }
            }
        }

        public void clearAllOmniFinderControls()
        {
            //Select Options
            App.utilitiesOmniFinderVariables.SelectedOption_String = "No Option Selected";


            //Charge Carrier
            App.glycanMakerVariables.NumberOfChargeCarrier_Int = 1;
            App.glycanMakerVariables.TypeOfChargeCarrier_String = "H";
            

            //Carbohydrate Type
            App.glycanMakerVariables.CarbohydrateType_String = "Aldehyde";


            //Special
            App.utilitiesOmniFinderVariables.CheckedNaH_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedCH3_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedSO3_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedOAcetyl_Bool = false;

            App.glycanMakerVariables.NumberOfNaH_Int = 0;
            App.glycanMakerVariables.NumberOfCH3_Int = 0;
            App.glycanMakerVariables.NumberOfSO3_Int = 0;
            App.glycanMakerVariables.NumberOfOAcetyl_Int = 0;


            //Sugars
            App.utilitiesOmniFinderVariables.CheckedHexose_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedPentose_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedNeuGc_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedKDN_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedHexA_Bool = false;

            App.glycanMakerVariables.NumberOfHexose_Int = 0;
            App.glycanMakerVariables.NumberOfHexNAc_Int = 0;
            App.glycanMakerVariables.NumberOfDeoxyhexose_Int = 0;
            App.glycanMakerVariables.NumberOfPentose_Int = 0;
            App.glycanMakerVariables.NumberOfNeuAc_Int = 0;
            App.glycanMakerVariables.NumberOfNeuGc_Int = 0;
            App.glycanMakerVariables.NumberOfKDN_Int = 0;
            App.glycanMakerVariables.NumberOfHexA_Int = 0;


            //User Units
            App.utilitiesOmniFinderVariables.CheckedUserUnit1_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedUserUnit2_Bool = false;

            App.glycanMakerVariables.NumberOfUserUnit1_Int = 0;
            App.glycanMakerVariables.NumberOfUserUnit2_Int = 0;
            App.glycanMakerVariables.MassOfUserUnit1_Double = 0.0;
            App.glycanMakerVariables.MassOfUserUnit2_Double = 0.0;


            //Amino Acids
            App.utilitiesOmniFinderVariables.CheckedAla_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedArg_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedAsn_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedAsp_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedCys_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedGln_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedGlu_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedGly_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedHis_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedIle_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedLeu_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedLys_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedMet_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedPhe_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedSer_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedThr_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedTrp_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedTyr_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedVal_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedPro_Bool = false;

            App.glycanMakerVariables.NumberOfAla_Int = 0;
            App.glycanMakerVariables.NumberOfArg_Int = 0;
            App.glycanMakerVariables.NumberOfAsn_Int = 0;
            App.glycanMakerVariables.NumberOfAsp_Int = 0;
            App.glycanMakerVariables.NumberOfCys_Int = 0;
            App.glycanMakerVariables.NumberOfGln_Int = 0;
            App.glycanMakerVariables.NumberOfGlu_Int = 0;
            App.glycanMakerVariables.NumberOfGly_Int = 0;
            App.glycanMakerVariables.NumberOfHis_Int = 0;
            App.glycanMakerVariables.NumberOfIle_Int = 0;
            App.glycanMakerVariables.NumberOfLeu_Int = 0;
            App.glycanMakerVariables.NumberOfLys_Int = 0;
            App.glycanMakerVariables.NumberOfMet_Int = 0;
            App.glycanMakerVariables.NumberOfPhe_Int = 0;
            App.glycanMakerVariables.NumberOfSer_Int = 0;
            App.glycanMakerVariables.NumberOfThr_Int = 0;
            App.glycanMakerVariables.NumberOfTrp_Int = 0;
            App.glycanMakerVariables.NumberOfTyr_Int = 0;
            App.glycanMakerVariables.NumberOfVal_Int = 0;
            App.glycanMakerVariables.NumberOfPro_Int = 0;


            //Permethyl
            App.utilitiesOmniFinderVariables.CheckedpHex_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedpHxNAc_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedpDxHex_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedpPntos_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedpNuAc_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedpNuGc_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedpKDN_Bool = false;
            App.utilitiesOmniFinderVariables.CheckedpHxA_Bool = false;

            App.glycanMakerVariables.NumberOfpHex_Int = 0;
            App.glycanMakerVariables.NumberOfpHxNAc_Int = 0;
            App.glycanMakerVariables.NumberOfpDxHex_Int = 0;
            App.glycanMakerVariables.NumberOfpPntos_Int = 0;
            App.glycanMakerVariables.NumberOfpNuAc_Int = 0;
            App.glycanMakerVariables.NumberOfpNuGc_Int = 0;
            App.glycanMakerVariables.NumberOfpKDN_Int = 0;
            App.glycanMakerVariables.NumberOfpHxA_Int = 0;


            //Neutral Mass, Mass Charge, C, H, O, N, Na Results
            App.glycanMakerVariables.NeutralMass_Double = 0.0;
            App.glycanMakerVariables.MassCharge_Double = 0.0;
            App.glycanMakerVariables.NumberOfC_Int = 0;
            App.glycanMakerVariables.NumberOfH_Int = 0;
            App.glycanMakerVariables.NumberOfO_Int = 0;
            App.glycanMakerVariables.NumberOfN_Int = 0;
            App.glycanMakerVariables.NumberOfNa_Int = 0;


            InitializeOmniFinderControls();
        }

        public void InitializeOmniFinderControls()
        {
            //Select Options ComboBox
            String SelectedOption_Temp = App.utilitiesOmniFinderVariables.SelectedOption_String;
            if (SelectedOption_Temp != "")
            {
                //Finds Selected Option To Be Displayed When There Is Saved Information
                int index = -1;
                string[] s = new string[3];
                s[0] = "N Glycans";
                s[1] = "Amino Acids";
                s[2] = "No Option Selected";
                // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                for (int j = 0; j < s.Length; j++)
                {
                    if (s[j] == SelectedOption_Temp)
                        index = j;
                }
                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                selectOptions_UComboBox.SelectedIndex = index;
            }
            else
                selectOptions_UComboBox.SelectedIndex = 2;


            //Charge Carrier
            numberOfChargeCarrier_UTextBox.Text = (App.glycanMakerVariables.NumberOfChargeCarrier_Int).ToString();
            String TypeOfChargeCarrier_Temp = App.glycanMakerVariables.TypeOfChargeCarrier_String;
            if (TypeOfChargeCarrier_Temp != "")
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
                    if (s[j] == TypeOfChargeCarrier_Temp)
                        index = j;
                }
                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                chargeCarrier_UComboBox.SelectedIndex = index;
            }
            else
                chargeCarrier_UComboBox.SelectedIndex = 0;


            //Carbohydrate Type
            String CarbohydrateType_Temp = App.glycanMakerVariables.CarbohydrateType_String;
            if (CarbohydrateType_Temp != "")
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
                    if (s[j] == CarbohydrateType_Temp)
                        index = j;
                }
                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                carbohydrateType_UComboBox.SelectedIndex = index;
            }
            else
                carbohydrateType_UComboBox.SelectedIndex = 0;


            //Special Check Boxes
            naH_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNaH_Bool;
            cH3_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedCH3_Bool;
            sO3_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedSO3_Bool;
            oAcetyl_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedOAcetyl_Bool;

            naH_UTextBox.Text = App.glycanMakerVariables.NumberOfNaH_Int.ToString();
            cH3_UTextBox.Text = App.glycanMakerVariables.NumberOfCH3_Int.ToString();
            sO3_UTextBox.Text = App.glycanMakerVariables.NumberOfSO3_Int.ToString();
            oAcetyl_UTextBox.Text = App.glycanMakerVariables.NumberOfOAcetyl_Int.ToString();


            //Sugars Checkboxes
            hexose_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexose_Bool;
            hexNAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool;
            dxyHex_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool;
            pentose_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPentose_Bool;
            neuAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool;
            neuGc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedNeuGc_Bool;
            kDN_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedKDN_Bool;
            hexA_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHexA_Bool;

            hexose_UTextBox.Text = App.glycanMakerVariables.NumberOfHexose_Int.ToString();
            hexNAc_UTextBox.Text = App.glycanMakerVariables.NumberOfHexNAc_Int.ToString();
            dxyHex_UTextBox.Text = App.glycanMakerVariables.NumberOfDeoxyhexose_Int.ToString();
            pentose_UTextBox.Text = App.glycanMakerVariables.NumberOfPentose_Int.ToString();
            neuAc_UTextBox.Text = App.glycanMakerVariables.NumberOfNeuAc_Int.ToString();
            neuGc_UTextBox.Text = App.glycanMakerVariables.NumberOfNeuGc_Int.ToString();
            kDN_UTextBox.Text = App.glycanMakerVariables.NumberOfKDN_Int.ToString();
            hexA_UTextBox.Text = App.glycanMakerVariables.NumberOfHexA_Int.ToString();


            //User Text Box
            userUnitA_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedUserUnit1_Bool;
            userUnitB_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedUserUnit2_Bool;

            userUnitA_UTextBox.Text = App.glycanMakerVariables.NumberOfUserUnit1_Int.ToString();
            userUnitB_UTextBox.Text = App.glycanMakerVariables.NumberOfUserUnit2_Int.ToString();
            massOfUserUnitA_UTextBox.Text = App.glycanMakerVariables.MassOfUserUnit1_Double.ToString();
            massOfUserUnitB_UTextBox.Text = App.glycanMakerVariables.MassOfUserUnit2_Double.ToString();


            //Amino Acid Option Box Check Boxes
            ala_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAla_Bool;
            arg_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedArg_Bool;
            asn_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAsn_Bool;
            asp_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedAsp_Bool;
            cys_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedCys_Bool;
            gln_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGln_Bool;
            glu_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGlu_Bool;
            gly_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedGly_Bool;
            his_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedHis_Bool;
            ile_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedIle_Bool;
            leu_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedLeu_Bool;
            lys_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedLys_Bool;
            met_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedMet_Bool;
            phe_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPhe_Bool;
            ser_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedSer_Bool;
            thr_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedThr_Bool;
            trp_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedTrp_Bool;
            tyr_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedTyr_Bool;
            val_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedVal_Bool;
            pro_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedPro_Bool;

            ala_UTextBox.Text = App.glycanMakerVariables.NumberOfAla_Int.ToString();
            arg_UTextBox.Text = App.glycanMakerVariables.NumberOfArg_Int.ToString();
            asn_UTextBox.Text = App.glycanMakerVariables.NumberOfAsn_Int.ToString();
            asp_UTextBox.Text = App.glycanMakerVariables.NumberOfAsp_Int.ToString();
            cys_UTextBox.Text = App.glycanMakerVariables.NumberOfCys_Int.ToString();
            gln_UTextBox.Text = App.glycanMakerVariables.NumberOfGln_Int.ToString();
            glu_UTextBox.Text = App.glycanMakerVariables.NumberOfGlu_Int.ToString();
            gly_UTextBox.Text = App.glycanMakerVariables.NumberOfGly_Int.ToString();
            his_UTextBox.Text = App.glycanMakerVariables.NumberOfHis_Int.ToString();
            ile_UTextBox.Text = App.glycanMakerVariables.NumberOfIle_Int.ToString();
            leu_UTextBox.Text = App.glycanMakerVariables.NumberOfLeu_Int.ToString();
            lys_UTextBox.Text = App.glycanMakerVariables.NumberOfLys_Int.ToString();
            met_UTextBox.Text = App.glycanMakerVariables.NumberOfMet_Int.ToString();
            phe_UTextBox.Text = App.glycanMakerVariables.NumberOfPhe_Int.ToString();
            ser_UTextBox.Text = App.glycanMakerVariables.NumberOfSer_Int.ToString();
            thr_UTextBox.Text = App.glycanMakerVariables.NumberOfThr_Int.ToString();
            trp_UTextBox.Text = App.glycanMakerVariables.NumberOfTrp_Int.ToString();
            tyr_UTextBox.Text = App.glycanMakerVariables.NumberOfTyr_Int.ToString();
            val_UTextBox.Text = App.glycanMakerVariables.NumberOfVal_Int.ToString();
            pro_UTextBox.Text = App.glycanMakerVariables.NumberOfPro_Int.ToString();


            //Permethyl Check Boxes
            pHex_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpHex_Bool;
            pHxNAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpHxNAc_Bool;
            pDxHex_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpDxHex_Bool;
            pPntos_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpPntos_Bool;
            pNuAc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpNuAc_Bool;
            pNuGc_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpNuGc_Bool;
            pKDN_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpKDN_Bool;
            pHxA_UCheckBox.IsChecked = App.utilitiesOmniFinderVariables.CheckedpHxA_Bool;

            pHex_UTextBox.Text = App.glycanMakerVariables.NumberOfpHex_Int.ToString();
            pHxNAc_UTextBox.Text = App.glycanMakerVariables.NumberOfpHxNAc_Int.ToString();
            pDxHex_UTextBox.Text = App.glycanMakerVariables.NumberOfpDxHex_Int.ToString();
            pPntos_UTextBox.Text = App.glycanMakerVariables.NumberOfpPntos_Int.ToString();
            pNuAc_UTextBox.Text = App.glycanMakerVariables.NumberOfpNuAc_Int.ToString();
            pNuGc_UTextBox.Text = App.glycanMakerVariables.NumberOfpNuGc_Int.ToString();
            pKDN_UTextBox.Text = App.glycanMakerVariables.NumberOfpKDN_Int.ToString();
            pHxA_UTextBox.Text = App.glycanMakerVariables.NumberOfpHxA_Int.ToString();


            //Other TextBoxes
            neutralMass_UTextBox.Text = App.glycanMakerVariables.NeutralMass_Double.ToString();
            massCharge_UTextBox.Text = App.glycanMakerVariables.MassCharge_Double.ToString();
            amountOfC_UTextBox.Text = App.glycanMakerVariables.NumberOfC_Int.ToString();
            amountOfH_UTextBox.Text = App.glycanMakerVariables.NumberOfH_Int.ToString();
            amountOfO_UTextBox.Text = App.glycanMakerVariables.NumberOfO_Int.ToString();
            amountOfN_UTextBox.Text = App.glycanMakerVariables.NumberOfN_Int.ToString();
            amountOfNa_UTextBox.Text = App.glycanMakerVariables.NumberOfNa_Int.ToString();
        }
    }
}