using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace GlycolyzerGUI
{
    public class ParametersOmniFinderPageDesign
    {
        #region Variables
        //Canvas to Draw the Box Outlines On
        public Canvas parameterOmniFinderPage_Canvas = new Canvas();

        //Grid to Organize Design
        Grid parameterOmniFinderPage_Grid = new Grid();

        //MiniGrid in Row 2
        Grid parametersOmniFinderPage_MiniGrid = new Grid();

        //Select Options ComboBox in 1st Row
        ComboBox selectOptions_ComboBox = new ComboBox();

        //Special Check Boxes
        CheckBox naH_CheckBox = new CheckBox();
        CheckBox cH3_CheckBox = new CheckBox();
        CheckBox sO3_CheckBox = new CheckBox();
        CheckBox oAcetyl_CheckBox = new CheckBox();

        //Sugars Checkboxes
        CheckBox hexose_CheckBox = new CheckBox();
        CheckBox hexNAc_CheckBox = new CheckBox();
        CheckBox dxyHex_CheckBox = new CheckBox();
        CheckBox pentose_CheckBox = new CheckBox();
        CheckBox neuAc_CheckBox = new CheckBox();
        CheckBox neuGc_CheckBox = new CheckBox();
        CheckBox kDN_CheckBox = new CheckBox();
        CheckBox hexA_CheckBox = new CheckBox();

        //User Text Box
        TextBox userNumberOf_TextBox = new TextBox();


        //Amino Acid Option Box Check Boxes
        CheckBox ala_CheckBox = new CheckBox();
        CheckBox arg_CheckBox = new CheckBox();
        CheckBox asn_CheckBox = new CheckBox();
        CheckBox asp_CheckBox = new CheckBox();
        CheckBox cys_CheckBox = new CheckBox();
        CheckBox gln_CheckBox = new CheckBox();
        CheckBox glu_CheckBox = new CheckBox();
        CheckBox gly_CheckBox = new CheckBox();
        CheckBox his_CheckBox = new CheckBox();
        CheckBox ile_CheckBox = new CheckBox();
        CheckBox leu_CheckBox = new CheckBox();
        CheckBox lys_CheckBox = new CheckBox();
        CheckBox met_CheckBox = new CheckBox();
        CheckBox phe_CheckBox = new CheckBox();
        CheckBox ser_CheckBox = new CheckBox();
        CheckBox thr_CheckBox = new CheckBox();
        CheckBox trp_CheckBox = new CheckBox();
        CheckBox tyr_CheckBox = new CheckBox();
        CheckBox val_CheckBox = new CheckBox();
        CheckBox pro_CheckBox = new CheckBox();

        //Permethyl Check Boxes
        CheckBox pHex_CheckBox = new CheckBox();
        CheckBox pHxNAc_CheckBox = new CheckBox();
        CheckBox pDxHex_CheckBox = new CheckBox();
        CheckBox pPntos_CheckBox = new CheckBox();
        CheckBox pNuAc_CheckBox = new CheckBox();
        CheckBox pNuGc_CheckBox = new CheckBox();
        CheckBox pKDN_CheckBox = new CheckBox();
        CheckBox pHxA_CheckBox = new CheckBox();
        #endregion

        public ParametersOmniFinderPageDesign()
        {
            ParametersOmniFinderPage_Canvas();
        }

        private void ParametersOmniFinderPage_Canvas()
        {
            //Initializes OmniFinder Values
            InitializeOmniFinderControls();

            //Specifics of Grid and Canvas so Background Color will Size with Window
            parameterOmniFinderPage_Grid.Height = 380;
            parameterOmniFinderPage_Grid.Width = 400;
            parameterOmniFinderPage_Canvas.Background = new SolidColorBrush(Colors.PaleVioletRed);

            //Build Grid and Design
            ParametersOmniFinderPage_Grid();

            //Add Grid to Canvas
            parameterOmniFinderPage_Canvas.Children.Add(parameterOmniFinderPage_Grid);
        }

        private void ParametersOmniFinderPage_Grid()
        {
            #region Main Grid Row Definitions
            //2 Row x 1 Column Main Grid   
            RowDefinition Main_RowDefinition = new RowDefinition();
            Main_RowDefinition.Height = new System.Windows.GridLength(50);
            parameterOmniFinderPage_Grid.RowDefinitions.Add(Main_RowDefinition);

            RowDefinition Main_RowDefinition1 = new RowDefinition();
            Main_RowDefinition1.Height = new System.Windows.GridLength(350);
            parameterOmniFinderPage_Grid.RowDefinitions.Add(Main_RowDefinition1);
            #endregion


            #region OmniFinder Header Label
            //"OmniFinder" Head Label in 1st Row
            Label omniFinder_Label = new Label();
            omniFinder_Label.Content = "OmniFinder";
            omniFinder_Label.Style = (Style)Application.Current.Resources["secondaryHeaderLabelTextStyle"];
            Grid.SetRow(omniFinder_Label, 0);
            Grid.SetColumn(omniFinder_Label, 0);
            #endregion


            #region Clear All Button
            //Initialize Button in 1st Row
            Button clearAll_Button = new Button();
            clearAll_Button.Content = "Clear All";
            clearAll_Button.Margin = new Thickness(0, 26, 140, 0);
            clearAll_Button.Style = (Style)Application.Current.Resources["optionButtonStyle"];
            clearAll_Button.Click += this.clearAllButton_Click;
            Grid.SetRow(clearAll_Button, 0);
            Grid.SetColumn(clearAll_Button, 0);
            #endregion


            #region Select Options ComboBox
            //Select Options ComboBox in 1st Row
            selectOptions_ComboBox.Height = 23;
            selectOptions_ComboBox.Width = 130;
            selectOptions_ComboBox.Margin = new Thickness(110, 26, 0, 0);
            selectOptions_ComboBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            selectOptions_ComboBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //selectOptions_ComboBox.Text = "Select Option";
            selectOptions_ComboBox.IsEditable = true;
            selectOptions_ComboBox.IsReadOnly = true;
            if (App.parametersOmniFinderVariables.SelectedOption_String != "")
                selectOptions_ComboBox.SelectedValue = App.parametersOmniFinderVariables.SelectedOption_String;
            selectOptions_ComboBox.Items.Add(new ComboBoxItem() { Content = "N Glycans" });
            selectOptions_ComboBox.Items.Add(new ComboBoxItem() { Content = "Amino Acids" });
            selectOptions_ComboBox.Items.Add(new ComboBoxItem() { Content = "No Option Selected" });
            selectOptions_ComboBox.SelectionChanged += this.selectOptions_ComboBox_SelectionChanged;
            Grid.SetRow(selectOptions_ComboBox, 0);
            Grid.SetColumn(selectOptions_ComboBox, 0);
            #endregion

            #region Build MiniGrid
            //Build MiniGrid Design
            ParametersOmniFinderPage_MiniGrid();

            //Put MiniGrid in 2nd Row of Main Grid
            Grid.SetRow(parametersOmniFinderPage_MiniGrid, 1);
            #endregion


            #region Add Controls and MiniGrid to Main Grid
            //Add Controls and 1st MiniGrid to Main Grid
            parameterOmniFinderPage_Grid.Children.Add(omniFinder_Label);
            parameterOmniFinderPage_Grid.Children.Add(clearAll_Button);
            parameterOmniFinderPage_Grid.Children.Add(selectOptions_ComboBox);
            parameterOmniFinderPage_Grid.Children.Add(parametersOmniFinderPage_MiniGrid);
            #endregion
        }

        private void ParametersOmniFinderPage_MiniGrid()
        {
            #region MiniGrid Column Definitions
            //2 MiniGrid Columns
            ColumnDefinition miniGrid_ColumnDefinition = new ColumnDefinition();
            miniGrid_ColumnDefinition.Width = new System.Windows.GridLength(150);
            parametersOmniFinderPage_MiniGrid.ColumnDefinitions.Add(miniGrid_ColumnDefinition);

            ColumnDefinition miniGrid_ColumnDefinition1 = new ColumnDefinition();
            miniGrid_ColumnDefinition1.Width = new System.Windows.GridLength(250);
            parametersOmniFinderPage_MiniGrid.ColumnDefinitions.Add(miniGrid_ColumnDefinition1);
            #endregion

            CreateLabels();
            CreateCheckBoxes();
            CreateUserTextBox();
            SugarsBox();
            UsersBox();
            SpecialsBox();
            AminoAcidsBox();
            PermethylBox();
        }

        private void CreateLabels()
        {
            List<Label> labels_List = new List<Label>();
            List<String> contents_List = new List<String>();
            List<int> leftMargin_List = new List<int>();
            List<int> bottomMargin_List = new List<int>();
            List<int> columnGrid_List = new List<int>();

            #region labels List
            //labels List
            //Sugars(8), User "#", Special(4), Amino Acids(20), Permethyl(8)
            for (int i = 1; i <= 41; i++)
                labels_List.Add(new Label());
            #endregion

            #region contents List
            //contents List
            //Sugars(8)
            contents_List.Add("Hexose");
            contents_List.Add("HexNAc");
            contents_List.Add("DxyHex");
            contents_List.Add("Pentose");
            contents_List.Add("NeuAc");
            contents_List.Add("NeuGc");
            contents_List.Add("KDN");
            contents_List.Add("Hex A");

            //User "#"
            contents_List.Add("#");

            //Specials(4)
            contents_List.Add("NaH");
            contents_List.Add("CH₃");
            contents_List.Add("SO₃­");
            contents_List.Add("OActyl­");

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
            #endregion

            #region leftMargin List
            //leftMargin List
            //Sugars(8)
            for (int i = 1; i <= 8; i++)
                leftMargin_List.Add(30);

            //User "#"
            leftMargin_List.Add(15);

            //Specials(4)
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(30);
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(90);

            //Amino Acids(20)
            for (int i = 1; i <= 10; i++)
                leftMargin_List.Add(30);
            for (int i = 1; i <= 10; i++)
                leftMargin_List.Add(140);

            //Permethyl(8)
            for (int i = 1; i <= 3; i++)
                leftMargin_List.Add(30);
            for (int i = 1; i <= 3; i++)
                leftMargin_List.Add(110);
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(190);
            #endregion

            #region bottomMargin List
            //bottomMargin List
            //Sugars(8)
            for (int i = 1; i <= 8; i++)
                bottomMargin_List.Add(285 - ((i - 1) * 40));

            //User "#"
            bottomMargin_List.Add(-100);

            //Specials(4)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 2; i++)
                    bottomMargin_List.Add(-210 - ((i - 1) * 35));
            }

            //Amino Acids(20)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 10; i++)
                    bottomMargin_List.Add(285 - ((i - 1) * 40));
            }

            //Permethyl(8)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 3; i++)
                    bottomMargin_List.Add(-165 - ((i - 1) * 40));
            }
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-165 - ((i - 1) * 40));
            #endregion

            #region columnGrid List
            //columnGrid List
            //Sugars(8), User "#", Specials(4)
            for (int i = 1; i <= 13; i++)
                columnGrid_List.Add(0);

            //Amino Acids(20), Permethyl(8)
            for (int i = 1; i <= 28; i++)
                columnGrid_List.Add(1);
            #endregion


            for (int i = 0; i <= (labels_List.Count - 1); i++)
            {
                labels_List[i].Content = contents_List[i];
                labels_List[i].Margin = new Thickness(leftMargin_List[i], 0, 0, bottomMargin_List[i]);
                labels_List[i].Style = (Style)Application.Current.Resources["blackLeftLineLabelTextStyle"];
                Grid.SetRow(labels_List[i], 0);
                Grid.SetColumn(labels_List[i], columnGrid_List[i]);

                //Add Controls to MiniGrid
                parametersOmniFinderPage_MiniGrid.Children.Add(labels_List[i]);
            }
        }

        private void CreateCheckBoxes()
        {
            List<CheckBox> checkBoxes_List = new List<CheckBox>();
            List<String> names_List = new List<String>();
            List<Boolean> checkBoxVariables_List = new List<Boolean>();
            List<int> leftMargin_List = new List<int>();
            List<int> bottomMargin_List = new List<int>();
            List<int> columnGrid_List = new List<int>();

            #region checkboxes List
            //checkboxes List
            //Sugars(8)
            checkBoxes_List.Add(hexose_CheckBox);
            checkBoxes_List.Add(hexNAc_CheckBox);
            checkBoxes_List.Add(dxyHex_CheckBox);
            checkBoxes_List.Add(pentose_CheckBox);
            checkBoxes_List.Add(neuAc_CheckBox);
            checkBoxes_List.Add(neuGc_CheckBox);
            checkBoxes_List.Add(kDN_CheckBox);
            checkBoxes_List.Add(hexA_CheckBox);

            //Specials(4)
            checkBoxes_List.Add(naH_CheckBox);
            checkBoxes_List.Add(cH3_CheckBox);
            checkBoxes_List.Add(sO3_CheckBox);
            checkBoxes_List.Add(oAcetyl_CheckBox);

            //Amino Acids(20)
            checkBoxes_List.Add(ala_CheckBox);
            checkBoxes_List.Add(arg_CheckBox);
            checkBoxes_List.Add(asn_CheckBox);
            checkBoxes_List.Add(asp_CheckBox);
            checkBoxes_List.Add(cys_CheckBox);
            checkBoxes_List.Add(gln_CheckBox);
            checkBoxes_List.Add(glu_CheckBox);
            checkBoxes_List.Add(gly_CheckBox);
            checkBoxes_List.Add(his_CheckBox);
            checkBoxes_List.Add(ile_CheckBox);
            checkBoxes_List.Add(leu_CheckBox);
            checkBoxes_List.Add(lys_CheckBox);
            checkBoxes_List.Add(met_CheckBox);
            checkBoxes_List.Add(phe_CheckBox);
            checkBoxes_List.Add(ser_CheckBox);
            checkBoxes_List.Add(thr_CheckBox);
            checkBoxes_List.Add(trp_CheckBox);
            checkBoxes_List.Add(tyr_CheckBox);
            checkBoxes_List.Add(val_CheckBox);
            checkBoxes_List.Add(pro_CheckBox);

            //Permethyl(8)
            checkBoxes_List.Add(pHex_CheckBox);
            checkBoxes_List.Add(pHxNAc_CheckBox);
            checkBoxes_List.Add(pDxHex_CheckBox);
            checkBoxes_List.Add(pPntos_CheckBox);
            checkBoxes_List.Add(pNuAc_CheckBox);
            checkBoxes_List.Add(pNuGc_CheckBox);
            checkBoxes_List.Add(pKDN_CheckBox);
            checkBoxes_List.Add(pHxA_CheckBox);
            #endregion

            #region names List
            //names List
            //Sugars(8)
            names_List.Add("hexose_CheckBox");
            names_List.Add("hexNAc_CheckBox");
            names_List.Add("dxyHex_CheckBox");
            names_List.Add("pentose_CheckBox");
            names_List.Add("neuAc_CheckBox");
            names_List.Add("neuGc_CheckBox");
            names_List.Add("kDN_CheckBox");
            names_List.Add("hexA_CheckBox");

            //Specials(3)
            names_List.Add("naH_CheckBox");
            names_List.Add("cH3_CheckBox");
            names_List.Add("sO3_CheckBox");
            names_List.Add("oAcetyl_CheckBox");

            //Amino Acids(20)
            names_List.Add("ala_CheckBox");
            names_List.Add("arg_CheckBox");
            names_List.Add("asn_CheckBox");
            names_List.Add("asp_CheckBox");
            names_List.Add("cys_CheckBox");
            names_List.Add("gln_CheckBox");
            names_List.Add("glu_CheckBox");
            names_List.Add("gly_CheckBox");
            names_List.Add("his_CheckBox");
            names_List.Add("ile_CheckBox");
            names_List.Add("leu_CheckBox");
            names_List.Add("lys_CheckBox");
            names_List.Add("met_CheckBox");
            names_List.Add("phe_CheckBox");
            names_List.Add("ser_CheckBox");
            names_List.Add("thr_CheckBox");
            names_List.Add("trp_CheckBox");
            names_List.Add("tyr_CheckBox");
            names_List.Add("val_CheckBox");
            names_List.Add("pro_CheckBox");

            //Permethyl(8)
            names_List.Add("pHex_CheckBox");
            names_List.Add("pHxNAc_CheckBox");
            names_List.Add("pDxHex_CheckBox");
            names_List.Add("pPntos_CheckBox");
            names_List.Add("pNuAc_CheckBox");
            names_List.Add("pNuGc_CheckBox");
            names_List.Add("pKDN_CheckBox");
            names_List.Add("pHxA_CheckBox");
            #endregion

            #region checkBoxVariables List
            //checkBoxVariables List
            //Sugars(8)
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedHexose_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedHexNAc_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedDxyHex_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedPentose_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedNeuAc_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedNeuGc_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedKDN_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedHexA_Bool);

            //Specials(4)
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedNaH_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedCH3_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedSO3_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedOAcetyl_Bool);

            //Amino Acids(20)
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedAla_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedArg_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedAsn_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedAsp_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedCys_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedGln_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedGlu_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedGly_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedHis_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedIle_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedLeu_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedLys_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedMet_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedPhe_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedSer_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedThr_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedTrp_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedTyr_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedVal_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedPro_Bool);

            //Permethyl(8)
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedpHex_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedpHxNAc_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedpDxHex_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedpPntos_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedpNuAc_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedpNuGc_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedpKDN_Bool);
            checkBoxVariables_List.Add(App.parametersOmniFinderVariables.CheckedpHxA_Bool);
            #endregion

            #region leftMargin List
            //leftMargin List
            //Sugars(8)
            for (int i = 1; i <= 8; i++)
                leftMargin_List.Add(15);

            //Specials(4)
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(15);
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(75);

            //Amino Acids(20) 
            for (int i = 1; i <= 10; i++)
                leftMargin_List.Add(15);
            for (int i = 1; i <= 10; i++)
                leftMargin_List.Add(125);

            //Permethyl(8)
            for (int i = 1; i <= 3; i++)
                leftMargin_List.Add(15);
            for (int i = 1; i <= 3; i++)
                leftMargin_List.Add(95);
            for (int i = 1; i <= 2; i++)
                leftMargin_List.Add(175);
            #endregion

            #region bottomMargin List
            //bottomMargin List
            //Sugars(8)
            for (int i = 1; i <= 8; i++)
                bottomMargin_List.Add(285 - ((i - 1) * 40));

            //Specials(4)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 2; i++)
                    bottomMargin_List.Add(-210 - ((i - 1) * 35));
            }

            //Amino Acids(20)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 10; i++)
                    bottomMargin_List.Add(285 - ((i - 1) * 40));
            }

            //Permethyl(8)
            for (int x = 1; x <= 2; x++)
            {
                for (int i = 1; i <= 3; i++)
                    bottomMargin_List.Add(-165 - ((i - 1) * 40));
            }
            for (int i = 1; i <= 2; i++)
                bottomMargin_List.Add(-165 - ((i - 1) * 40));
            #endregion

            #region columnGrid List
            //columnGrid List
            //Sugars(8), Specials(4)
            for (int i = 1; i <= 12; i++)
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
                Grid.SetRow(checkBoxes_List[i], 0);
                Grid.SetColumn(checkBoxes_List[i], columnGrid_List[i]);

                //Add Controls to MiniGrid
                parametersOmniFinderPage_MiniGrid.Children.Add(checkBoxes_List[i]);
            }
        }

        private void CreateUserTextBox()
        {
            userNumberOf_TextBox.Width = 83;
            userNumberOf_TextBox.Margin = new Thickness(15, 100, 0, 0);
            userNumberOf_TextBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            userNumberOf_TextBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            if ((App.parametersOmniFinderVariables.NumberForUser_Int) != 0)
                userNumberOf_TextBox.Text = (App.parametersOmniFinderVariables.NumberForUser_Int).ToString();
            userNumberOf_TextBox.TextChanged += this.userNumberOf_TextBox_TextChanged;
            Grid.SetRow(userNumberOf_TextBox, 0);
            Grid.SetColumn(userNumberOf_TextBox, 0);

            //Add Controls to MiniGrid
            parametersOmniFinderPage_MiniGrid.Children.Add(userNumberOf_TextBox);
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
            sugars_LineGeometry1.StartPoint = new Point(45, 20);
            sugars_LineGeometry1.EndPoint = new Point(10, 20);

            LineGeometry sugars_LineGeometry2 = new LineGeometry();
            sugars_LineGeometry2.StartPoint = new Point(10, 20);
            sugars_LineGeometry2.EndPoint = new Point(10, 185);

            LineGeometry sugars_LineGeometry3 = new LineGeometry();
            sugars_LineGeometry3.StartPoint = new Point(10, 185);
            sugars_LineGeometry3.EndPoint = new Point(140, 185);

            LineGeometry sugars_LineGeometry4 = new LineGeometry();
            sugars_LineGeometry4.StartPoint = new Point(140, 185);
            sugars_LineGeometry4.EndPoint = new Point(140, 20);

            LineGeometry sugars_LineGeometry5 = new LineGeometry();
            sugars_LineGeometry5.StartPoint = new Point(140, 20);
            sugars_LineGeometry5.EndPoint = new Point(105, 20);

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
            parametersOmniFinderPage_MiniGrid.Children.Add(sugars_Label);
            parametersOmniFinderPage_MiniGrid.Children.Add(sugars_Path);
        }

        private void UsersBox()
        {
            //"User" Label
            Label user_Label = new Label();
            user_Label.Content = "User";
            user_Label.Margin = new Thickness(0, 185, 0, 0);
            user_Label.Style = (Style)Application.Current.Resources["tertiaryHeaderLabelTextStyle"];
            Grid.SetRow(user_Label, 0);
            Grid.SetColumn(user_Label, 0);

            #region Users White Box
            //White Outline for User Option Box
            GeometryGroup user_Geometry = new GeometryGroup();

            LineGeometry user_LineGeometry1 = new LineGeometry();
            user_LineGeometry1.StartPoint = new Point(50, 205);
            user_LineGeometry1.EndPoint = new Point(10, 205);

            LineGeometry user_LineGeometry2 = new LineGeometry();
            user_LineGeometry2.StartPoint = new Point(10, 205);
            user_LineGeometry2.EndPoint = new Point(10, 245);

            LineGeometry user_LineGeometry3 = new LineGeometry();
            user_LineGeometry3.StartPoint = new Point(10, 245);
            user_LineGeometry3.EndPoint = new Point(140, 245);

            LineGeometry user_LineGeometry4 = new LineGeometry();
            user_LineGeometry4.StartPoint = new Point(140, 245);
            user_LineGeometry4.EndPoint = new Point(140, 205);

            LineGeometry user_LineGeometry5 = new LineGeometry();
            user_LineGeometry5.StartPoint = new Point(140, 205);
            user_LineGeometry5.EndPoint = new Point(100, 205);

            user_Geometry.Children.Add(user_LineGeometry1);
            user_Geometry.Children.Add(user_LineGeometry2);
            user_Geometry.Children.Add(user_LineGeometry3);
            user_Geometry.Children.Add(user_LineGeometry4);
            user_Geometry.Children.Add(user_LineGeometry5);

            Path user_Path = new Path();
            user_Path.Stroke = new SolidColorBrush(Colors.White);
            user_Path.StrokeThickness = 3.0;
            user_Path.Data = user_Geometry;
            Grid.SetRow(user_Path, 0);
            Grid.SetColumn(user_Path, 0);
            #endregion

            //Add Box to MiniGrid
            parametersOmniFinderPage_MiniGrid.Children.Add(user_Label);
            parametersOmniFinderPage_MiniGrid.Children.Add(user_Path);
        }

        private void SpecialsBox()
        {
            //"Special" Label
            Label special_Label = new Label();
            special_Label.Content = "Special";
            special_Label.Margin = new Thickness(0, 245, 0, 0);
            special_Label.Style = (Style)Application.Current.Resources["tertiaryHeaderLabelTextStyle"];
            Grid.SetRow(special_Label, 1);
            Grid.SetColumn(special_Label, 0);

            #region Specials White Box
            //White Outline for Special Option Box
            GeometryGroup special_Geometry = new GeometryGroup();

            LineGeometry special_LineGeometry1 = new LineGeometry();
            special_LineGeometry1.StartPoint = new Point(45, 265);
            special_LineGeometry1.EndPoint = new Point(10, 265);

            LineGeometry special_LineGeometry2 = new LineGeometry();
            special_LineGeometry2.StartPoint = new Point(10, 265);
            special_LineGeometry2.EndPoint = new Point(10, 310);

            LineGeometry special_LineGeometry3 = new LineGeometry();
            special_LineGeometry3.StartPoint = new Point(10, 310);
            special_LineGeometry3.EndPoint = new Point(140, 310);

            LineGeometry special_LineGeometry4 = new LineGeometry();
            special_LineGeometry4.StartPoint = new Point(140, 310);
            special_LineGeometry4.EndPoint = new Point(140, 265);

            LineGeometry special_LineGeometry5 = new LineGeometry();
            special_LineGeometry5.StartPoint = new Point(140, 265);
            special_LineGeometry5.EndPoint = new Point(105, 265);

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
            parametersOmniFinderPage_MiniGrid.Children.Add(special_Label);
            parametersOmniFinderPage_MiniGrid.Children.Add(special_Path);
        }

        private void AminoAcidsBox()
        {
            //"Amino Acids" Label
            Label aminoAcids_Label = new Label();
            aminoAcids_Label.Content = "Amino Acids";
            aminoAcids_Label.Style = (Style)Application.Current.Resources["tertiaryHeaderLabelTextStyle"];
            Grid.SetRow(aminoAcids_Label, 0);
            Grid.SetColumn(aminoAcids_Label, 1);

            #region Amino Acids White Box
            //White Outline for Amino Acids Option Box
            GeometryGroup aminoAcids_Geometry = new GeometryGroup();

            LineGeometry aminoAcids_LineGeometry1 = new LineGeometry();
            aminoAcids_LineGeometry1.StartPoint = new Point(75, 20);
            aminoAcids_LineGeometry1.EndPoint = new Point(10, 20);

            LineGeometry aminoAcids_LineGeometry2 = new LineGeometry();
            aminoAcids_LineGeometry2.StartPoint = new Point(10, 20);
            aminoAcids_LineGeometry2.EndPoint = new Point(10, 225);

            LineGeometry aminoAcids_LineGeometry3 = new LineGeometry();
            aminoAcids_LineGeometry3.StartPoint = new Point(10, 225);
            aminoAcids_LineGeometry3.EndPoint = new Point(240, 225);

            LineGeometry aminoAcids_LineGeometry4 = new LineGeometry();
            aminoAcids_LineGeometry4.StartPoint = new Point(240, 225);
            aminoAcids_LineGeometry4.EndPoint = new Point(240, 20);

            LineGeometry aminoAcids_LineGeometry5 = new LineGeometry();
            aminoAcids_LineGeometry5.StartPoint = new Point(240, 20);
            aminoAcids_LineGeometry5.EndPoint = new Point(175, 20);

            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry1);
            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry2);
            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry3);
            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry4);
            aminoAcids_Geometry.Children.Add(aminoAcids_LineGeometry5);

            Path aminoAcids_Path = new Path();
            aminoAcids_Path.Stroke = new SolidColorBrush(Colors.White);
            aminoAcids_Path.StrokeThickness = 3.0;
            aminoAcids_Path.Data = aminoAcids_Geometry;
            Grid.SetRow(aminoAcids_Path, 0);
            Grid.SetColumn(aminoAcids_Path, 1);
            #endregion

            //Add Box to MiniGrid
            parametersOmniFinderPage_MiniGrid.Children.Add(aminoAcids_Label);
            parametersOmniFinderPage_MiniGrid.Children.Add(aminoAcids_Path);
        }

        private void PermethylBox()
        {
            //"Permethyl" Label
            Label permethyl_Label = new Label();
            permethyl_Label.Content = "Permethyl";
            permethyl_Label.Margin = new Thickness(0, 225, 0, 0);
            permethyl_Label.Style = (Style)Application.Current.Resources["tertiaryHeaderLabelTextStyle"];
            Grid.SetRow(permethyl_Label, 1);
            Grid.SetColumn(permethyl_Label, 1);

            #region Permethyl White Box
            //White Outline for Permethyl Option Box
            GeometryGroup permethyl_Geometry = new GeometryGroup();

            LineGeometry permethyl_LineGeometry1 = new LineGeometry();
            permethyl_LineGeometry1.StartPoint = new Point(85, 245);
            permethyl_LineGeometry1.EndPoint = new Point(10, 245);

            LineGeometry permethyl_LineGeometry2 = new LineGeometry();
            permethyl_LineGeometry2.StartPoint = new Point(10, 245);
            permethyl_LineGeometry2.EndPoint = new Point(10, 310);

            LineGeometry permethyl_LineGeometry3 = new LineGeometry();
            permethyl_LineGeometry3.StartPoint = new Point(10, 310);
            permethyl_LineGeometry3.EndPoint = new Point(240, 310);

            LineGeometry permethyl_LineGeometry4 = new LineGeometry();
            permethyl_LineGeometry4.StartPoint = new Point(240, 310);
            permethyl_LineGeometry4.EndPoint = new Point(240, 245);

            LineGeometry permethyl_LineGeometry5 = new LineGeometry();
            permethyl_LineGeometry5.StartPoint = new Point(240, 245);
            permethyl_LineGeometry5.EndPoint = new Point(165, 245);

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
            parametersOmniFinderPage_MiniGrid.Children.Add(permethyl_Label);
            parametersOmniFinderPage_MiniGrid.Children.Add(permethyl_Path);
        }

        private void clearAllButton_Click(object sender, RoutedEventArgs e)
        {
            clearAllOmniFinderControls();
        }

        public void selectOptions_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (App.initializePages.ParametersOmniFinderPageDesign_InitializeFlag != true)
            {
                App.parametersOmniFinderVariables.SelectedOption_String = (selectOptions_ComboBox.SelectedValue.ToString()).Substring(38);
                String SelectedOption_Temp = (selectOptions_ComboBox.SelectedValue.ToString()).Substring(38);
                switch (SelectedOption_Temp)
                {
                    case "N Glycans":
                        {
                            App.parametersOmniFinderVariables.CheckedHexose_Bool = true;
                            App.parametersOmniFinderVariables.CheckedHexNAc_Bool = true;
                            App.parametersOmniFinderVariables.CheckedDxyHex_Bool = true;
                            App.parametersOmniFinderVariables.CheckedNeuAc_Bool = true;
                            App.parametersOmniFinderVariables.CheckedNaH_Bool = true;
                            hexose_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexose_Bool;
                            hexNAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexNAc_Bool;
                            dxyHex_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedDxyHex_Bool;
                            neuAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNeuAc_Bool;
                            naH_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNaH_Bool;
                            break;
                        }
                    case "Amino Acids":
                        {
                            App.parametersOmniFinderVariables.CheckedAla_Bool = true;
                            App.parametersOmniFinderVariables.CheckedArg_Bool = true;
                            App.parametersOmniFinderVariables.CheckedAsn_Bool = true;
                            App.parametersOmniFinderVariables.CheckedAsp_Bool = true;
                            App.parametersOmniFinderVariables.CheckedCys_Bool = true;
                            App.parametersOmniFinderVariables.CheckedGln_Bool = true;
                            App.parametersOmniFinderVariables.CheckedGlu_Bool = true;
                            App.parametersOmniFinderVariables.CheckedGly_Bool = true;
                            App.parametersOmniFinderVariables.CheckedHis_Bool = true;
                            App.parametersOmniFinderVariables.CheckedIle_Bool = true;
                            App.parametersOmniFinderVariables.CheckedLeu_Bool = true;
                            App.parametersOmniFinderVariables.CheckedLys_Bool = true;
                            App.parametersOmniFinderVariables.CheckedMet_Bool = true;
                            App.parametersOmniFinderVariables.CheckedPhe_Bool = true;
                            App.parametersOmniFinderVariables.CheckedSer_Bool = true;
                            App.parametersOmniFinderVariables.CheckedThr_Bool = true;
                            App.parametersOmniFinderVariables.CheckedTrp_Bool = true;
                            App.parametersOmniFinderVariables.CheckedTyr_Bool = true;
                            App.parametersOmniFinderVariables.CheckedVal_Bool = true;
                            App.parametersOmniFinderVariables.CheckedPro_Bool = true;
                            ala_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAla_Bool;
                            arg_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedArg_Bool;
                            asn_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAsn_Bool;
                            asp_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAsp_Bool;
                            cys_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedCys_Bool;
                            gln_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGln_Bool;
                            glu_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGlu_Bool;
                            gly_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGly_Bool;
                            his_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHis_Bool;
                            ile_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedIle_Bool;
                            leu_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedLeu_Bool;
                            lys_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedLys_Bool;
                            met_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedMet_Bool;
                            phe_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPhe_Bool;
                            ser_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedSer_Bool;
                            thr_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedThr_Bool;
                            trp_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedTrp_Bool;
                            tyr_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedTyr_Bool;
                            val_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedVal_Bool;
                            pro_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPro_Bool;
                            break;
                        }
                    case "No Option Selected":
                        {
                            App.parametersOmniFinderVariables.CheckedHexose_Bool = false;
                            App.parametersOmniFinderVariables.CheckedHexNAc_Bool = false;
                            App.parametersOmniFinderVariables.CheckedDxyHex_Bool = false;
                            App.parametersOmniFinderVariables.CheckedNeuAc_Bool = false;
                            App.parametersOmniFinderVariables.CheckedNaH_Bool = false;
                            hexose_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexose_Bool;
                            hexNAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexNAc_Bool;
                            dxyHex_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedDxyHex_Bool;
                            neuAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNeuAc_Bool;
                            naH_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNaH_Bool;

                            App.parametersOmniFinderVariables.CheckedAla_Bool = false;
                            App.parametersOmniFinderVariables.CheckedArg_Bool = false;
                            App.parametersOmniFinderVariables.CheckedAsn_Bool = false;
                            App.parametersOmniFinderVariables.CheckedAsp_Bool = false;
                            App.parametersOmniFinderVariables.CheckedCys_Bool = false;
                            App.parametersOmniFinderVariables.CheckedGln_Bool = false;
                            App.parametersOmniFinderVariables.CheckedGlu_Bool = false;
                            App.parametersOmniFinderVariables.CheckedGly_Bool = false;
                            App.parametersOmniFinderVariables.CheckedHis_Bool = false;
                            App.parametersOmniFinderVariables.CheckedIle_Bool = false;
                            App.parametersOmniFinderVariables.CheckedLeu_Bool = false;
                            App.parametersOmniFinderVariables.CheckedLys_Bool = false;
                            App.parametersOmniFinderVariables.CheckedMet_Bool = false;
                            App.parametersOmniFinderVariables.CheckedPhe_Bool = false;
                            App.parametersOmniFinderVariables.CheckedSer_Bool = false;
                            App.parametersOmniFinderVariables.CheckedThr_Bool = false;
                            App.parametersOmniFinderVariables.CheckedTrp_Bool = false;
                            App.parametersOmniFinderVariables.CheckedTyr_Bool = false;
                            App.parametersOmniFinderVariables.CheckedVal_Bool = false;
                            App.parametersOmniFinderVariables.CheckedPro_Bool = false;
                            ala_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAla_Bool;
                            arg_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedArg_Bool;
                            asn_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAsn_Bool;
                            asp_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAsp_Bool;
                            cys_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedCys_Bool;
                            gln_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGln_Bool;
                            glu_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGlu_Bool;
                            gly_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGly_Bool;
                            his_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHis_Bool;
                            ile_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedIle_Bool;
                            leu_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedLeu_Bool;
                            lys_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedLys_Bool;
                            met_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedMet_Bool;
                            phe_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPhe_Bool;
                            ser_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedSer_Bool;
                            thr_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedThr_Bool;
                            trp_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedTrp_Bool;
                            tyr_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedTyr_Bool;
                            val_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedVal_Bool;
                            pro_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPro_Bool;
                            break;
                        }
                }
            }
        }

        void naH_CheckBox_Click(Object sender, EventArgs e)
        {
            if (App.initializePages.ParametersOmniFinderPageDesign_InitializeFlag != true)
            {
                CheckBox checkedCheckBox = (CheckBox)sender;
                switch (checkedCheckBox.Name)
                {
                    //Special
                    case "naH_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedNaH_Bool = (Boolean)checkedCheckBox.IsChecked;
                            naH_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNaH_Bool;
                            break;
                        }
                    case "cH3_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedCH3_Bool = (Boolean)checkedCheckBox.IsChecked;
                            cH3_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedCH3_Bool;
                            break;
                        }
                    case "sO3_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedSO3_Bool = (Boolean)checkedCheckBox.IsChecked;
                            sO3_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedSO3_Bool;
                            break;
                        }
                    case "oAcetyl_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedOAcetyl_Bool = (Boolean)checkedCheckBox.IsChecked;
                            oAcetyl_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedOAcetyl_Bool;
                            break;
                        }

                    //Sugars
                    case "hexose_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedHexose_Bool = (Boolean)checkedCheckBox.IsChecked;
                            hexose_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexose_Bool;
                            break;
                        }
                    case "hexNAc_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedHexNAc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            hexNAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexNAc_Bool;
                            break;
                        }
                    case "dxyHex_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedDxyHex_Bool = (Boolean)checkedCheckBox.IsChecked;
                            dxyHex_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedDxyHex_Bool;
                            break;
                        }
                    case "pentose_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedPentose_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pentose_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPentose_Bool;
                            break;
                        }
                    case "neuAc_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedNeuAc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            neuAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNeuAc_Bool;
                            break;
                        }
                    case "neuGc_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedNeuGc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            neuGc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNeuGc_Bool;
                            break;
                        }
                    case "kDN_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedKDN_Bool = (Boolean)checkedCheckBox.IsChecked;
                            kDN_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedKDN_Bool;
                            break;
                        }
                    case "hexA_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedHexA_Bool = (Boolean)checkedCheckBox.IsChecked;
                            hexA_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexA_Bool;
                            break;
                        }

                    //Amino Acids
                    case "ala_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedAla_Bool = (Boolean)checkedCheckBox.IsChecked;
                            ala_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAla_Bool;
                            break;
                        }
                    case "arg_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedArg_Bool = (Boolean)checkedCheckBox.IsChecked;
                            arg_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedArg_Bool;
                            break;
                        }
                    case "asn_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedAsn_Bool = (Boolean)checkedCheckBox.IsChecked;
                            asn_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAsn_Bool;
                            break;
                        }
                    case "asp_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedAsp_Bool = (Boolean)checkedCheckBox.IsChecked;
                            asp_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAsp_Bool;
                            break;
                        }
                    case "cys_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedCys_Bool = (Boolean)checkedCheckBox.IsChecked;
                            cys_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedCys_Bool;
                            break;
                        }
                    case "gln_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedGln_Bool = (Boolean)checkedCheckBox.IsChecked;
                            gln_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGln_Bool;
                            break;
                        }
                    case "glu_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedGlu_Bool = (Boolean)checkedCheckBox.IsChecked;
                            glu_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGlu_Bool;
                            break;
                        }
                    case "gly_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedGly_Bool = (Boolean)checkedCheckBox.IsChecked;
                            gly_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGly_Bool;
                            break;
                        }
                    case "his_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedHis_Bool = (Boolean)checkedCheckBox.IsChecked;
                            his_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHis_Bool;
                            break;
                        }
                    case "ile_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedIle_Bool = (Boolean)checkedCheckBox.IsChecked;
                            ile_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedIle_Bool;
                            break;
                        }
                    case "leu_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedLeu_Bool = (Boolean)checkedCheckBox.IsChecked;
                            leu_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedLeu_Bool;
                            break;
                        }
                    case "lys_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedLys_Bool = (Boolean)checkedCheckBox.IsChecked;
                            lys_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedLys_Bool;
                            break;
                        }
                    case "met_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedMet_Bool = (Boolean)checkedCheckBox.IsChecked;
                            met_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedMet_Bool;
                            break;
                        }
                    case "phe_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedPhe_Bool = (Boolean)checkedCheckBox.IsChecked;
                            phe_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPhe_Bool;
                            break;
                        }
                    case "ser_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedSer_Bool = (Boolean)checkedCheckBox.IsChecked;
                            ser_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedSer_Bool;
                            break;
                        }
                    case "thr_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedThr_Bool = (Boolean)checkedCheckBox.IsChecked;
                            thr_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedThr_Bool;
                            break;
                        }
                    case "trp_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedTrp_Bool = (Boolean)checkedCheckBox.IsChecked;
                            trp_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedTrp_Bool;
                            break;
                        }
                    case "tyr_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedTyr_Bool = (Boolean)checkedCheckBox.IsChecked;
                            tyr_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedTyr_Bool;
                            break;
                        }
                    case "val_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedVal_Bool = (Boolean)checkedCheckBox.IsChecked;
                            val_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedVal_Bool;
                            break;
                        }
                    case "pro_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedPro_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pro_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPro_Bool;
                            break;
                        }

                    //Permethyl
                    case "pHex_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedpHex_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pHex_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpHex_Bool;
                            break;
                        }
                    case "pHxNAc_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedpHxNAc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pHxNAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpHxNAc_Bool;
                            break;
                        }
                    case "pDxHex_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedpDxHex_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pDxHex_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpDxHex_Bool;
                            break;
                        }
                    case "pPntos_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedpPntos_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pPntos_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpPntos_Bool;
                            break;
                        }
                    case "pNuAc_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedpNuAc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pNuAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpNuAc_Bool;
                            break;
                        }
                    case "pNuGc_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedpNuGc_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pNuGc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpNuGc_Bool;
                            break;
                        }
                    case "pKDN_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedpKDN_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pKDN_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpKDN_Bool;
                            break;
                        }
                    case "pHxA_CheckBox":
                        {
                            App.parametersOmniFinderVariables.CheckedpHxA_Bool = (Boolean)checkedCheckBox.IsChecked;
                            pHxA_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpHxA_Bool;
                            break;
                        }
                }
            }
        }

        protected void userNumberOf_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (App.initializePages.ParametersOmniFinderPageDesign_InitializeFlag != true)
            {
                //User Number
                TextBox changedTextBox = (TextBox)sender;
                string Str = changedTextBox.Text.Trim();
                int Num;
                bool isNum = int.TryParse(Str, out Num);
                if (isNum)
                {
                    if (Num > 10)
                    {
                        MessageBox.Show("Number must be 10 or less");
                        App.parametersOmniFinderVariables.NumberForUser_Int = 0;
                    }
                    else
                        App.parametersOmniFinderVariables.NumberForUser_Int = Convert.ToInt32(((TextBox)sender).Text);
                }
                else if (changedTextBox.Text == "")
                    App.parametersOmniFinderVariables.NumberForUser_Int = 0;
                else
                {
                    MessageBox.Show("Invalid number");
                    App.parametersOmniFinderVariables.NumberForUser_Int = 0;
                }

                userNumberOf_TextBox.Text = (App.parametersOmniFinderVariables.NumberForUser_Int).ToString();
            }
        }

        public void clearAllOmniFinderControls()
        {
            //Select Options
            App.parametersOmniFinderVariables.SelectedOption_String = "No Option Selected";

            //Special
            App.parametersOmniFinderVariables.CheckedNaH_Bool = false;
            App.parametersOmniFinderVariables.CheckedCH3_Bool = false;
            App.parametersOmniFinderVariables.CheckedSO3_Bool = false;
            App.parametersOmniFinderVariables.CheckedOAcetyl_Bool = false;

            //Sugars
            App.parametersOmniFinderVariables.CheckedHexose_Bool = false;
            App.parametersOmniFinderVariables.CheckedHexNAc_Bool = false;
            App.parametersOmniFinderVariables.CheckedDxyHex_Bool = false;
            App.parametersOmniFinderVariables.CheckedPentose_Bool = false;
            App.parametersOmniFinderVariables.CheckedNeuAc_Bool = false;
            App.parametersOmniFinderVariables.CheckedNeuGc_Bool = false;
            App.parametersOmniFinderVariables.CheckedKDN_Bool = false;
            App.parametersOmniFinderVariables.CheckedHexA_Bool = false;

            //User Number
            App.parametersOmniFinderVariables.NumberForUser_Int = 0;

            //Amino Acids
            App.parametersOmniFinderVariables.CheckedAla_Bool = false;
            App.parametersOmniFinderVariables.CheckedArg_Bool = false;
            App.parametersOmniFinderVariables.CheckedAsn_Bool = false;
            App.parametersOmniFinderVariables.CheckedAsp_Bool = false;
            App.parametersOmniFinderVariables.CheckedCys_Bool = false;
            App.parametersOmniFinderVariables.CheckedGln_Bool = false;
            App.parametersOmniFinderVariables.CheckedGlu_Bool = false;
            App.parametersOmniFinderVariables.CheckedGly_Bool = false;
            App.parametersOmniFinderVariables.CheckedHis_Bool = false;
            App.parametersOmniFinderVariables.CheckedIle_Bool = false;
            App.parametersOmniFinderVariables.CheckedLeu_Bool = false;
            App.parametersOmniFinderVariables.CheckedLys_Bool = false;
            App.parametersOmniFinderVariables.CheckedMet_Bool = false;
            App.parametersOmniFinderVariables.CheckedPhe_Bool = false;
            App.parametersOmniFinderVariables.CheckedSer_Bool = false;
            App.parametersOmniFinderVariables.CheckedThr_Bool = false;
            App.parametersOmniFinderVariables.CheckedTrp_Bool = false;
            App.parametersOmniFinderVariables.CheckedTyr_Bool = false;
            App.parametersOmniFinderVariables.CheckedVal_Bool = false;
            App.parametersOmniFinderVariables.CheckedPro_Bool = false;

            //Permethyl
            App.parametersOmniFinderVariables.CheckedpHex_Bool = false;
            App.parametersOmniFinderVariables.CheckedpHxNAc_Bool = false;
            App.parametersOmniFinderVariables.CheckedpDxHex_Bool = false;
            App.parametersOmniFinderVariables.CheckedpPntos_Bool = false;
            App.parametersOmniFinderVariables.CheckedpNuAc_Bool = false;
            App.parametersOmniFinderVariables.CheckedpNuGc_Bool = false;
            App.parametersOmniFinderVariables.CheckedpKDN_Bool = false;
            App.parametersOmniFinderVariables.CheckedpHxA_Bool = false;

            InitializeOmniFinderControls();
        }

        public void InitializeOmniFinderControls()
        {
            //Select Options ComboBox
            String SelectedOption_Temp = App.parametersOmniFinderVariables.SelectedOption_String;
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
                selectOptions_ComboBox.SelectedIndex = index;
            }
            else
                selectOptions_ComboBox.SelectedIndex = 2;

            //Special Check Boxes
            naH_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNaH_Bool;
            cH3_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedCH3_Bool;
            sO3_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedSO3_Bool;
            oAcetyl_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedOAcetyl_Bool;

            //Sugars Checkboxes
            hexose_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexose_Bool;
            hexNAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexNAc_Bool;
            dxyHex_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedDxyHex_Bool;
            pentose_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPentose_Bool;
            neuAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNeuAc_Bool;
            neuGc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedNeuGc_Bool;
            kDN_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedKDN_Bool;
            hexA_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHexA_Bool;

            //User Text Box
            userNumberOf_TextBox.Text = (App.parametersOmniFinderVariables.NumberForUser_Int).ToString();


            //Amino Acid Option Box Check Boxes
            ala_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAla_Bool;
            arg_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedArg_Bool;
            asn_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAsn_Bool;
            asp_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedAsp_Bool;
            cys_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedCys_Bool;
            gln_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGln_Bool;
            glu_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGlu_Bool;
            gly_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedGly_Bool;
            his_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedHis_Bool;
            ile_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedIle_Bool;
            leu_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedLeu_Bool;
            lys_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedLys_Bool;
            met_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedMet_Bool;
            phe_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPhe_Bool;
            ser_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedSer_Bool;
            thr_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedThr_Bool;
            trp_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedTrp_Bool;
            tyr_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedTyr_Bool;
            val_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedVal_Bool;
            pro_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedPro_Bool;

            //Permethyl Check Boxes
            pHex_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpHex_Bool;
            pHxNAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpHxNAc_Bool;
            pDxHex_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpDxHex_Bool;
            pPntos_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpPntos_Bool;
            pNuAc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpNuAc_Bool;
            pNuGc_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpNuGc_Bool;
            pKDN_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpKDN_Bool;
            pHxA_CheckBox.IsChecked = App.parametersOmniFinderVariables.CheckedpHxA_Bool;
        }
    }
}
