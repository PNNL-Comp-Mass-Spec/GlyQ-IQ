using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace GlycolyzerGUI
{
    public class UtilitiesRangesPage
    {
        //Canvas to Draw the Box Outlines On
        public Canvas utilitiesRangesPage_Canvas = new Canvas();

        //Grid to Organize Design
        Grid utilitiesRangesPage_Grid = new Grid();
        Grid utilitiesRangesPage_MiniGrid = new Grid();

        //List Box to Hold the Ranges
        ListBox utilitiesRangesPage_ListBox = new ListBox();

        //Lists of Buttons, Min and Max TextBoxes, and Min and Max Labels
        List<Button> rangesButtons = new List<Button>();
        List<TextBox> rangesMinTextBoxes = new List<TextBox>();
        List<TextBox> rangesMaxTextBoxes = new List<TextBox>();

        UtilitiesRangesSubHomePage utilitiesRangesSubHomePage;

        #region TextBox Declarations
        //TextBox Declarations 
        //Sugars Ranges
        TextBox hexoseMin_TextBox = new TextBox();
        TextBox hexoseMax_TextBox = new TextBox();

        TextBox hexNAcMin_TextBox = new TextBox();
        TextBox hexNAcMax_TextBox = new TextBox();

        TextBox dxyHexMin_TextBox = new TextBox();
        TextBox dxyHexMax_TextBox = new TextBox();

        TextBox pentoseMin_TextBox = new TextBox();
        TextBox pentoseMax_TextBox = new TextBox();

        TextBox neuAcMin_TextBox = new TextBox();
        TextBox neuAcMax_TextBox = new TextBox();

        TextBox neuGcMin_TextBox = new TextBox();
        TextBox neuGcMax_TextBox = new TextBox();

        TextBox kDNMin_TextBox = new TextBox();
        TextBox kDNMax_TextBox = new TextBox();

        TextBox hexAMin_TextBox = new TextBox();
        TextBox hexAMax_TextBox = new TextBox();

        //User Unit Ranges
        TextBox userUnit1Min_TextBox = new TextBox();
        TextBox userUnit1Max_TextBox = new TextBox();

        TextBox userUnit2Min_TextBox = new TextBox();
        TextBox userUnit2Max_TextBox = new TextBox();

        /*TextBox userUnit3Min_TextBox = new TextBox();
        TextBox userUnit3Max_TextBox = new TextBox();

        TextBox userUnit4Min_TextBox = new TextBox();
        TextBox userUnit4Max_TextBox = new TextBox();

        TextBox userUnit5Min_TextBox = new TextBox();
        TextBox userUnit5Max_TextBox = new TextBox();

        TextBox userUnit6Min_TextBox = new TextBox();
        TextBox userUnit6Max_TextBox = new TextBox();

        TextBox userUnit7Min_TextBox = new TextBox();
        TextBox userUnit7Max_TextBox = new TextBox();

        TextBox userUnit8Min_TextBox = new TextBox();
        TextBox userUnit8Max_TextBox = new TextBox();

        TextBox userUnit9Min_TextBox = new TextBox();
        TextBox userUnit9Max_TextBox = new TextBox();

        TextBox userUnit10Min_TextBox = new TextBox();
        TextBox userUnit10Max_TextBox = new TextBox();*/

        //Special Ranges
        TextBox naHMin_TextBox = new TextBox();
        TextBox naHMax_TextBox = new TextBox();

        TextBox cH3Min_TextBox = new TextBox();
        TextBox cH3Max_TextBox = new TextBox();

        TextBox sO3Min_TextBox = new TextBox();
        TextBox sO3Max_TextBox = new TextBox();

        TextBox oAcetylMin_TextBox = new TextBox();
        TextBox oAcetylMax_TextBox = new TextBox();

        //Amino Acid Ranges
        TextBox alaMin_TextBox = new TextBox();
        TextBox alaMax_TextBox = new TextBox();

        TextBox argMin_TextBox = new TextBox();
        TextBox argMax_TextBox = new TextBox();

        TextBox asnMin_TextBox = new TextBox();
        TextBox asnMax_TextBox = new TextBox();

        TextBox aspMin_TextBox = new TextBox();
        TextBox aspMax_TextBox = new TextBox();

        TextBox cysMin_TextBox = new TextBox();
        TextBox cysMax_TextBox = new TextBox();

        TextBox glnMin_TextBox = new TextBox();
        TextBox glnMax_TextBox = new TextBox();

        TextBox gluMin_TextBox = new TextBox();
        TextBox gluMax_TextBox = new TextBox();

        TextBox glyMin_TextBox = new TextBox();
        TextBox glyMax_TextBox = new TextBox();

        TextBox hisMin_TextBox = new TextBox();
        TextBox hisMax_TextBox = new TextBox();

        TextBox ileMin_TextBox = new TextBox();
        TextBox ileMax_TextBox = new TextBox();

        TextBox leuMin_TextBox = new TextBox();
        TextBox leuMax_TextBox = new TextBox();

        TextBox lysMin_TextBox = new TextBox();
        TextBox lysMax_TextBox = new TextBox();

        TextBox metMin_TextBox = new TextBox();
        TextBox metMax_TextBox = new TextBox();

        TextBox pheMin_TextBox = new TextBox();
        TextBox pheMax_TextBox = new TextBox();

        TextBox serMin_TextBox = new TextBox();
        TextBox serMax_TextBox = new TextBox();

        TextBox thrMin_TextBox = new TextBox();
        TextBox thrMax_TextBox = new TextBox();

        TextBox trpMin_TextBox = new TextBox();
        TextBox trpMax_TextBox = new TextBox();

        TextBox tyrMin_TextBox = new TextBox();
        TextBox tyrMax_TextBox = new TextBox();

        TextBox valMin_TextBox = new TextBox();
        TextBox valMax_TextBox = new TextBox();

        TextBox proMin_TextBox = new TextBox();
        TextBox proMax_TextBox = new TextBox();

        //Permethyl Ranges
        TextBox pHexMin_TextBox = new TextBox();
        TextBox pHexMax_TextBox = new TextBox();

        TextBox pHxNAcMin_TextBox = new TextBox();
        TextBox pHxNAcMax_TextBox = new TextBox();

        TextBox pDxHexMin_TextBox = new TextBox();
        TextBox pDxHexMax_TextBox = new TextBox();

        TextBox pPntosMin_TextBox = new TextBox();
        TextBox pPntosMax_TextBox = new TextBox();

        TextBox pNuAcMin_TextBox = new TextBox();
        TextBox pNuAcMax_TextBox = new TextBox();

        TextBox pNuGcMin_TextBox = new TextBox();
        TextBox pNuGcMax_TextBox = new TextBox();

        TextBox pKDNMin_TextBox = new TextBox();
        TextBox pKDNMax_TextBox = new TextBox();

        TextBox pHxAMin_TextBox = new TextBox();
        TextBox pHxAMax_TextBox = new TextBox();
        #endregion


        public UtilitiesRangesPage(UtilitiesRangesSubHomePage utilitiesRangesSubHomePageP)
        {
            utilitiesRangesSubHomePage = utilitiesRangesSubHomePageP;
            UtilitiesRangesPage_Canvas();
        }

        private void UtilitiesRangesPage_Canvas()
        {
            //Specifics of Canvas so Background Color will Size with Window
            utilitiesRangesPage_Canvas.Background = new SolidColorBrush(Colors.DarkCyan);


            //Build Grid and Design
            UtilitiesRangesPage_Grid();

            //Add Grid to Canvas
            utilitiesRangesPage_Canvas.Children.Add(utilitiesRangesPage_Grid);
        }

        private void UtilitiesRangesPage_Grid()
        {
            #region Main Grid Row Definitions
            //2 Row x 1 Column Main Grid   
            RowDefinition Main_RowDefinition = new RowDefinition();
            Main_RowDefinition.Height = new System.Windows.GridLength(40);
            utilitiesRangesPage_Grid.RowDefinitions.Add(Main_RowDefinition);

            RowDefinition Main_RowDefinition1 = new RowDefinition();
            Main_RowDefinition1.Height = System.Windows.GridLength.Auto;
            utilitiesRangesPage_Grid.RowDefinitions.Add(Main_RowDefinition1);
            #endregion


            #region RangesPage Header Label
            //"Parameter Ranges" Head Label in 1st Row
            Label UtilitiesRangesPage_Label = new Label();
            UtilitiesRangesPage_Label.Content = "Parameter Ranges";
            UtilitiesRangesPage_Label.Margin = new Thickness(0, 0, 10, 0);
            UtilitiesRangesPage_Label.Style = (Style)Application.Current.Resources["secondaryHeaderLabelTextStyle"];
            Grid.SetRow(UtilitiesRangesPage_Label, 0);
            #endregion

            UtilitiesRangesPage_ListBox();
            Grid.SetRow(utilitiesRangesPage_ListBox, 1);

            //Add Controls to Grid
            utilitiesRangesPage_Grid.Children.Add(UtilitiesRangesPage_Label);
            utilitiesRangesPage_Grid.Children.Add(utilitiesRangesPage_ListBox);
        }

        private void UtilitiesRangesPage_ListBox()
        {
            #region ListBox Utilities
            //ListBox Utilities
            utilitiesRangesPage_ListBox.HorizontalAlignment = HorizontalAlignment.Center;
            utilitiesRangesPage_ListBox.Padding = new Thickness(0, 0, 10, 0);
            utilitiesRangesPage_ListBox.Background = new SolidColorBrush(Colors.DarkCyan);
            //utilitiesRangesPage_ListBox.BorderThickness = new Thickness(0, 0, 0, 0); ;
            utilitiesRangesPage_ListBox.Style = (Style)Application.Current.Resources["listBoxStyle"];
            //Don't Display a Horizontal Scroll Bar
            utilitiesRangesPage_ListBox.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled); 
            #endregion
            
            //Build the Range Controls Design
            UtilitiesRangesPage_Initialization();  
        }

        public void UtilitiesRangesPage_Initialization()
        {
            utilitiesRangesSubHomePage.Height = 400;
            utilitiesRangesSubHomePage.Width = 400;

            utilitiesRangesPage_Grid.Height = 400;
            utilitiesRangesPage_Grid.Width = 400;

            utilitiesRangesPage_ListBox.Height = 340;
            utilitiesRangesPage_ListBox.Width = 380;

            utilitiesRangesPage_MiniGrid.Height = 340;
            utilitiesRangesPage_MiniGrid.Width = 380;

            //Call to Make Small Grids
            UtilitiesRangesPage_MiniGrid();
        }

        private void UtilitiesRangesPage_MiniGrid()
        {
            //Empty and Reset MiniGrid and All Lists Each Time Tab is Opened
            utilitiesRangesPage_ListBox.Items.Clear();
            utilitiesRangesPage_MiniGrid.Children.Clear();
            rangesButtons.Clear();
            rangesMinTextBoxes.Clear();
            rangesMaxTextBoxes.Clear();

            //Add 1 column
            ColumnDefinition miniGrid_ColumnDefinition = new ColumnDefinition();
            miniGrid_ColumnDefinition.Width = new System.Windows.GridLength(380);
            utilitiesRangesPage_MiniGrid.ColumnDefinitions.Add(miniGrid_ColumnDefinition);

            //Call Method to Make the Needed Controls
            UtilitiesRangesPage_ListControls();

            int row = 0;
            int col = 0;

            //Add Controls to MiniGrid
            for (int i = 0; i <= (rangesButtons.Count - 1); i++)
            {
                //Grid UtilitiesRangesPage_MiniGrid = new Grid();
                Label minLabel = new Label();
                Label maxLabel = new Label();
                
                //Style the Labels
                minLabel.Style = (Style)Application.Current.Resources["minListBoxLabelTextStyle"];
                maxLabel.Style = (Style)Application.Current.Resources["maxListBoxLabelTextStyle"];

                RowDefinition miniGrid_RowDefinition = new RowDefinition();
                miniGrid_RowDefinition.Height = new System.Windows.GridLength(34);
                utilitiesRangesPage_MiniGrid.RowDefinitions.Add(miniGrid_RowDefinition);

                Grid.SetRow(rangesButtons[i], row);
                Grid.SetColumn(rangesButtons[i], col);
                Grid.SetRow(minLabel, row);
                Grid.SetColumn(minLabel, col);
                Grid.SetRow(rangesMinTextBoxes[i], row);
                Grid.SetColumn(rangesMinTextBoxes[i], col);
                Grid.SetRow(maxLabel, row);
                Grid.SetColumn(maxLabel, col);
                Grid.SetRow(rangesMaxTextBoxes[i], row);
                Grid.SetColumn(rangesMaxTextBoxes[i], col);

                row++;
                if (row == 20)
                {
                    row = 0;
                    col++;
                }

                //try to add children
                try
                {
                    //Add controls to MiniGrid
                    utilitiesRangesPage_MiniGrid.Children.Add(rangesButtons[i]);
                    utilitiesRangesPage_MiniGrid.Children.Add(minLabel);
                    utilitiesRangesPage_MiniGrid.Children.Add(rangesMinTextBoxes[i]);
                    utilitiesRangesPage_MiniGrid.Children.Add(maxLabel);
                    utilitiesRangesPage_MiniGrid.Children.Add(rangesMaxTextBoxes[i]);
                }
                // Catch invalid operation if child is already a child
                catch (InvalidOperationException e)
                {
                    utilitiesRangesPage_MiniGrid.Children.Remove(rangesButtons[i]);
                    utilitiesRangesPage_MiniGrid.Children.Remove(rangesMinTextBoxes[i]);
                    utilitiesRangesPage_MiniGrid.Children.Remove(rangesMaxTextBoxes[i]);
                    utilitiesRangesPage_MiniGrid.Children.Add(rangesButtons[i]);
                    utilitiesRangesPage_MiniGrid.Children.Add(rangesMinTextBoxes[i]);
                    utilitiesRangesPage_MiniGrid.Children.Add(rangesMaxTextBoxes[i]);
                }
            }

            //try to add children
            try
            {
                //Add MiniGrid to ListBox
                utilitiesRangesPage_ListBox.Items.Add(utilitiesRangesPage_MiniGrid);
            }
            // Catch invalid operation if child is already a child
            catch (InvalidOperationException e)
            {
                utilitiesRangesPage_ListBox.Items.Remove(utilitiesRangesPage_MiniGrid);
                utilitiesRangesPage_ListBox.Items.Add(utilitiesRangesPage_MiniGrid);
            }
        }

        private void UtilitiesRangesPage_ListControls()//20 per column
        {
            //Lists to Hold Possibles to Minimize Lines of Code
            List<Boolean> checkIfCheckBoxesChecked = new List<Boolean>();
            List<Button> possibleButtons = new List<Button>();
            List<String> possibleButtonContent = new List<String>();
            List<TextBox> possibleMinTextBoxes = new List<TextBox>();
            List<TextBox> possibleMaxTextBoxes = new List<TextBox>();
            List<int> possibleMinVariables = new List<int>();
            List<int> possibleMaxVariables = new List<int>();
            List<String> possibleButtonNames = new List<String>();
            List<String> possibleMinTextBoxNames = new List<String>();
            List<String> possibleMaxTextBoxNames = new List<String>();

            int columnCheck = 0;
            int numColumns = 1;
            int extraRowsCheck = 0;


            #region checkIfCheckBoxesChecked List
            //checkIfCheckBoxesChecked List
            //Sugars
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedHexose_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedHexNAc_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedDxyHex_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedPentose_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedNeuAc_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedNeuGc_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedKDN_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedHexA_Bool);

            //User Units
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedUserUnit1_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedUserUnit2_Bool);

            //Special                
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedNaH_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedCH3_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedSO3_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedOAcetyl_Bool);

            //Amino Acids               
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedAla_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedArg_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedAsn_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedAsp_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedCys_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedGln_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedGlu_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedGly_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedHis_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedIle_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedLeu_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedLys_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedMet_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedPhe_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedSer_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedThr_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedTrp_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedTyr_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedVal_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedPro_Bool);

            //Permethyl                  
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedpHex_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedpHxNAc_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedpDxHex_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedpPntos_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedpNuAc_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedpNuGc_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedpKDN_Bool);
            checkIfCheckBoxesChecked.Add(App.utilitiesOmniFinderVariables.CheckedpHxA_Bool);
            #endregion

            #region possibleButtons List
            //possibleButtons List
            //Sugars(8), User Units(2), Special(4), Amino Acids(20), Permethyl(8)
            for(int i = 0; i <= 42; i++)
                possibleButtons.Add(new Button());
            #endregion

            #region possibleButtonContent List
            //possibleButtonContent List
            //Sugars
            possibleButtonContent.Add("Hexose");
            possibleButtonContent.Add("HexNAc");
            possibleButtonContent.Add("DxyHex");
            possibleButtonContent.Add("Pentose");
            possibleButtonContent.Add("NeuAc");
            possibleButtonContent.Add("NeuGc");
            possibleButtonContent.Add("KDn");
            possibleButtonContent.Add("HexA");

            //User Units
            possibleButtonContent.Add("User 1");
            possibleButtonContent.Add("User 2");

            //Special               
            possibleButtonContent.Add("NaH");
            possibleButtonContent.Add("CH3");
            possibleButtonContent.Add("SO3");
            possibleButtonContent.Add("O-Acetyl");

            //Amino Acids           
            possibleButtonContent.Add("Ala");
            possibleButtonContent.Add("Arg");
            possibleButtonContent.Add("Asn");
            possibleButtonContent.Add("Asp");
            possibleButtonContent.Add("Cys");
            possibleButtonContent.Add("Gln");
            possibleButtonContent.Add("Glu");
            possibleButtonContent.Add("Gly");
            possibleButtonContent.Add("His");
            possibleButtonContent.Add("Ile");
            possibleButtonContent.Add("Leu");
            possibleButtonContent.Add("Lys");
            possibleButtonContent.Add("Met");
            possibleButtonContent.Add("Phe");
            possibleButtonContent.Add("Ser");
            possibleButtonContent.Add("Thr");
            possibleButtonContent.Add("Trp");
            possibleButtonContent.Add("Tyr");
            possibleButtonContent.Add("Val");
            possibleButtonContent.Add("Pro");

            //Permethyl             
            possibleButtonContent.Add("pHex");
            possibleButtonContent.Add("pHxNAc");
            possibleButtonContent.Add("pDxHex");
            possibleButtonContent.Add("pPntos");
            possibleButtonContent.Add("pNuAc");
            possibleButtonContent.Add("pNuGc");
            possibleButtonContent.Add("pKDn");
            possibleButtonContent.Add("pHxA");
            #endregion

            #region possibleMinTextBoxes List
            //possibleMinTextBoxes List
            //Sugars
            possibleMinTextBoxes.Add(hexoseMin_TextBox);
            possibleMinTextBoxes.Add(hexNAcMin_TextBox);
            possibleMinTextBoxes.Add(dxyHexMin_TextBox);
            possibleMinTextBoxes.Add(pentoseMin_TextBox);
            possibleMinTextBoxes.Add(neuAcMin_TextBox);
            possibleMinTextBoxes.Add(neuGcMin_TextBox);
            possibleMinTextBoxes.Add(kDNMin_TextBox);
            possibleMinTextBoxes.Add(hexAMin_TextBox);

            //User Units
            possibleMinTextBoxes.Add(userUnit1Min_TextBox);
            possibleMinTextBoxes.Add(userUnit2Min_TextBox);

            //Special               
            possibleMinTextBoxes.Add(naHMin_TextBox);
            possibleMinTextBoxes.Add(cH3Min_TextBox);
            possibleMinTextBoxes.Add(sO3Min_TextBox);
            possibleMinTextBoxes.Add(oAcetylMin_TextBox);

            //Amino Acids           
            possibleMinTextBoxes.Add(alaMin_TextBox);
            possibleMinTextBoxes.Add(argMin_TextBox);
            possibleMinTextBoxes.Add(asnMin_TextBox);
            possibleMinTextBoxes.Add(aspMin_TextBox);
            possibleMinTextBoxes.Add(cysMin_TextBox);
            possibleMinTextBoxes.Add(glnMin_TextBox);
            possibleMinTextBoxes.Add(gluMin_TextBox);
            possibleMinTextBoxes.Add(glyMin_TextBox);
            possibleMinTextBoxes.Add(hisMin_TextBox);
            possibleMinTextBoxes.Add(ileMin_TextBox);
            possibleMinTextBoxes.Add(leuMin_TextBox);
            possibleMinTextBoxes.Add(lysMin_TextBox);
            possibleMinTextBoxes.Add(metMin_TextBox);
            possibleMinTextBoxes.Add(pheMin_TextBox);
            possibleMinTextBoxes.Add(serMin_TextBox);
            possibleMinTextBoxes.Add(thrMin_TextBox);
            possibleMinTextBoxes.Add(trpMin_TextBox);
            possibleMinTextBoxes.Add(tyrMin_TextBox);
            possibleMinTextBoxes.Add(valMin_TextBox);
            possibleMinTextBoxes.Add(proMin_TextBox);

            //Permethyl             
            possibleMinTextBoxes.Add(pHexMin_TextBox);
            possibleMinTextBoxes.Add(pHxNAcMin_TextBox);
            possibleMinTextBoxes.Add(pDxHexMin_TextBox);
            possibleMinTextBoxes.Add(pPntosMin_TextBox);
            possibleMinTextBoxes.Add(pNuAcMin_TextBox);
            possibleMinTextBoxes.Add(pNuGcMin_TextBox);
            possibleMinTextBoxes.Add(pKDNMin_TextBox);
            possibleMinTextBoxes.Add(pHxAMin_TextBox);
            #endregion

            #region possibleMaxTextBoxes List
            //possibleMaxTextBoxes List
            //Sugars
            possibleMaxTextBoxes.Add(hexoseMax_TextBox);
            possibleMaxTextBoxes.Add(hexNAcMax_TextBox);
            possibleMaxTextBoxes.Add(dxyHexMax_TextBox);
            possibleMaxTextBoxes.Add(pentoseMax_TextBox);
            possibleMaxTextBoxes.Add(neuAcMax_TextBox);
            possibleMaxTextBoxes.Add(neuGcMax_TextBox);
            possibleMaxTextBoxes.Add(kDNMax_TextBox);
            possibleMaxTextBoxes.Add(hexAMax_TextBox);

            //User Units
            possibleMaxTextBoxes.Add(userUnit1Max_TextBox);
            possibleMaxTextBoxes.Add(userUnit2Max_TextBox);

            //Special               
            possibleMaxTextBoxes.Add(naHMax_TextBox);
            possibleMaxTextBoxes.Add(cH3Max_TextBox);
            possibleMaxTextBoxes.Add(sO3Max_TextBox);
            possibleMaxTextBoxes.Add(oAcetylMax_TextBox);

            //Amino Acids           
            possibleMaxTextBoxes.Add(alaMax_TextBox);
            possibleMaxTextBoxes.Add(argMax_TextBox);
            possibleMaxTextBoxes.Add(asnMax_TextBox);
            possibleMaxTextBoxes.Add(aspMax_TextBox);
            possibleMaxTextBoxes.Add(cysMax_TextBox);
            possibleMaxTextBoxes.Add(glnMax_TextBox);
            possibleMaxTextBoxes.Add(gluMax_TextBox);
            possibleMaxTextBoxes.Add(glyMax_TextBox);
            possibleMaxTextBoxes.Add(hisMax_TextBox);
            possibleMaxTextBoxes.Add(ileMax_TextBox);
            possibleMaxTextBoxes.Add(leuMax_TextBox);
            possibleMaxTextBoxes.Add(lysMax_TextBox);
            possibleMaxTextBoxes.Add(metMax_TextBox);
            possibleMaxTextBoxes.Add(pheMax_TextBox);
            possibleMaxTextBoxes.Add(serMax_TextBox);
            possibleMaxTextBoxes.Add(thrMax_TextBox);
            possibleMaxTextBoxes.Add(trpMax_TextBox);
            possibleMaxTextBoxes.Add(tyrMax_TextBox);
            possibleMaxTextBoxes.Add(valMax_TextBox);
            possibleMaxTextBoxes.Add(proMax_TextBox);

            //Permethyl             
            possibleMaxTextBoxes.Add(pHexMax_TextBox);
            possibleMaxTextBoxes.Add(pHxNAcMax_TextBox);
            possibleMaxTextBoxes.Add(pDxHexMax_TextBox);
            possibleMaxTextBoxes.Add(pPntosMax_TextBox);
            possibleMaxTextBoxes.Add(pNuAcMax_TextBox);
            possibleMaxTextBoxes.Add(pNuGcMax_TextBox);
            possibleMaxTextBoxes.Add(pKDNMax_TextBox);
            possibleMaxTextBoxes.Add(pHxAMax_TextBox);
            #endregion

            #region possibleMinVariables List
            //possibleMinVariables List
            //Sugars
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinHexose_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinHexNAc_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinDxyHex_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinPentose_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinNeuAc_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinNeuGc_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinKDN_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinHexA_Int);

            //User Units
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinUserUnit1_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinUserUnit2_Int);

            //Special               
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinNaH_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinCH3_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinSO3_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinOAcetyl_Int);

            //Amino Acids           
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinAla_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinArg_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinAsn_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinAsp_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinCys_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinGln_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinGlu_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinGly_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinHis_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinIle_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinLeu_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinLys_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinMet_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinPhe_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinSer_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinThr_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinTrp_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinTyr_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinVal_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinPro_Int);

            //Permethyl             
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinpHex_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinpHxNAc_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinpDxHex_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinpPntos_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinpNuAc_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinpNuGc_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinpKDN_Int);
            possibleMinVariables.Add(App.utilitiesRangesPageVariables.MinpHxA_Int);
            #endregion

            #region possibleMaxVariables List
            //possibleMaxVariables List
            //Sugars
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxHexose_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxHexNAc_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxDxyHex_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxPentose_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxNeuAc_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxNeuGc_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxKDN_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxHexA_Int);

            //User Units
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxUserUnit1_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxUserUnit2_Int);

            //Special               
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxNaH_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxCH3_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxSO3_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxOAcetyl_Int);

            //Amino Acids           
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxAla_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxArg_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxAsn_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxAsp_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxCys_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxGln_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxGlu_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxGly_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxHis_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxIle_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxLeu_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxLys_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxMet_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxPhe_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxSer_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxThr_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxTrp_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxTyr_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxVal_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxPro_Int);

            //Permethyl             
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxpHex_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxpHxNAc_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxpDxHex_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxpPntos_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxpNuAc_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxpNuGc_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxpKDN_Int);
            possibleMaxVariables.Add(App.utilitiesRangesPageVariables.MaxpHxA_Int);
            #endregion

            #region possibleButtonNames List
            //possibleButtonNames List
            //Sugars
            possibleButtonNames.Add("hexose_Button");
            possibleButtonNames.Add("hexNAc_Button");
            possibleButtonNames.Add("dxyHex_Button");
            possibleButtonNames.Add("pentose_Button");
            possibleButtonNames.Add("neuAc_Button");
            possibleButtonNames.Add("neuGc_Button");
            possibleButtonNames.Add("kDN_Button");
            possibleButtonNames.Add("hexA_Button");

            //User Units
            possibleButtonNames.Add("userUnit1_Button");
            possibleButtonNames.Add("userUnit2_Button");

            //Special  
            possibleButtonNames.Add("naH_Button");
            possibleButtonNames.Add("cH3_Button");
            possibleButtonNames.Add("sO3_Button");
            possibleButtonNames.Add("oAcetyl_Button");

            //Amino Acids
            possibleButtonNames.Add("ala_Button");
            possibleButtonNames.Add("arg_Button");
            possibleButtonNames.Add("asn_Button");
            possibleButtonNames.Add("asp_Button");
            possibleButtonNames.Add("cys_Button");
            possibleButtonNames.Add("gln_Button");
            possibleButtonNames.Add("glu_Button");
            possibleButtonNames.Add("gly_Button");
            possibleButtonNames.Add("his_Button");
            possibleButtonNames.Add("ile_Button");
            possibleButtonNames.Add("leu_Button");
            possibleButtonNames.Add("lys_Button");
            possibleButtonNames.Add("met_Button");
            possibleButtonNames.Add("phe_Button");
            possibleButtonNames.Add("ser_Button");
            possibleButtonNames.Add("thr_Button");
            possibleButtonNames.Add("trp_Button");
            possibleButtonNames.Add("tyr_Button");
            possibleButtonNames.Add("val_Button");
            possibleButtonNames.Add("pro_Button");

            //Permethyl 
            possibleButtonNames.Add("pHex_Button");
            possibleButtonNames.Add("pHxNAc_Button");
            possibleButtonNames.Add("pDxHex_Button");
            possibleButtonNames.Add("pPntos_Button");
            possibleButtonNames.Add("pNuAc_Button");
            possibleButtonNames.Add("pNuGc_Button");
            possibleButtonNames.Add("pKDN_Button");
            possibleButtonNames.Add("pHxA_Button");
            #endregion

            #region possibleMinTextBoxNames List
            //possibleMinTextBoxNames List
            //Sugars
            possibleMinTextBoxNames.Add("hexoseMin_TextBox");
            possibleMinTextBoxNames.Add("hexNAcMin_TextBox");
            possibleMinTextBoxNames.Add("dxyHexMin_TextBox");
            possibleMinTextBoxNames.Add("pentoseMin_TextBox");
            possibleMinTextBoxNames.Add("neuAcMin_TextBox");
            possibleMinTextBoxNames.Add("neuGcMin_TextBox");
            possibleMinTextBoxNames.Add("kDNMin_TextBox");
            possibleMinTextBoxNames.Add("hexAMin_TextBox");

            //User Units
            possibleMinTextBoxNames.Add("userUnit1Min_TextBox");
            possibleMinTextBoxNames.Add("userUnit2Min_TextBox");

            //Special  
            possibleMinTextBoxNames.Add("naHMin_TextBox");
            possibleMinTextBoxNames.Add("cH3Min_TextBox");
            possibleMinTextBoxNames.Add("sO3Min_TextBox");
            possibleMinTextBoxNames.Add("oAcetylMin_TextBox");

            //Amino Acids
            possibleMinTextBoxNames.Add("alaMin_TextBox");
            possibleMinTextBoxNames.Add("argMin_TextBox");
            possibleMinTextBoxNames.Add("asnMin_TextBox");
            possibleMinTextBoxNames.Add("aspMin_TextBox");
            possibleMinTextBoxNames.Add("cysMin_TextBox");
            possibleMinTextBoxNames.Add("glnMin_TextBox");
            possibleMinTextBoxNames.Add("gluMin_TextBox");
            possibleMinTextBoxNames.Add("glyMin_TextBox");
            possibleMinTextBoxNames.Add("hisMin_TextBox");
            possibleMinTextBoxNames.Add("ileMin_TextBox");
            possibleMinTextBoxNames.Add("leuMin_TextBox");
            possibleMinTextBoxNames.Add("lysMin_TextBox");
            possibleMinTextBoxNames.Add("metMin_TextBox");
            possibleMinTextBoxNames.Add("pheMin_TextBox");
            possibleMinTextBoxNames.Add("serMin_TextBox");
            possibleMinTextBoxNames.Add("thrMin_TextBox");
            possibleMinTextBoxNames.Add("trpMin_TextBox");
            possibleMinTextBoxNames.Add("tyrMin_TextBox");
            possibleMinTextBoxNames.Add("valMin_TextBox");
            possibleMinTextBoxNames.Add("proMin_TextBox");

            //Permethyl 
            possibleMinTextBoxNames.Add("pHexMin_TextBox");
            possibleMinTextBoxNames.Add("pHxNAcMin_TextBox");
            possibleMinTextBoxNames.Add("pDxHexMin_TextBox");
            possibleMinTextBoxNames.Add("pPntosMin_TextBox");
            possibleMinTextBoxNames.Add("pNuAcMin_TextBox");
            possibleMinTextBoxNames.Add("pNuGcMin_TextBox");
            possibleMinTextBoxNames.Add("pKDNMin_TextBox");
            possibleMinTextBoxNames.Add("pHxAMin_TextBox");
            #endregion

            #region possibleMaxTextBoxNames List
            //possibleMaxTextBoxNames List
            //Sugars
            possibleMaxTextBoxNames.Add("hexoseMax_TextBox");
            possibleMaxTextBoxNames.Add("hexNAcMax_TextBox");
            possibleMaxTextBoxNames.Add("dxyHexMax_TextBox");
            possibleMaxTextBoxNames.Add("pentoseMax_TextBox");
            possibleMaxTextBoxNames.Add("neuAcMax_TextBox");
            possibleMaxTextBoxNames.Add("neuGcMax_TextBox");
            possibleMaxTextBoxNames.Add("kDNMax_TextBox");
            possibleMaxTextBoxNames.Add("hexAMax_TextBox");

            //User Units
            possibleMaxTextBoxNames.Add("userUnit1Max_TextBox");
            possibleMaxTextBoxNames.Add("userUnit2Max_TextBox");

            //Special  
            possibleMaxTextBoxNames.Add("naHMax_TextBox");
            possibleMaxTextBoxNames.Add("cH3Max_TextBox");
            possibleMaxTextBoxNames.Add("sO3Max_TextBox");
            possibleMaxTextBoxNames.Add("oAcetylMax_TextBox");

            //Amino Acids
            possibleMaxTextBoxNames.Add("alaMax_TextBox");
            possibleMaxTextBoxNames.Add("argMax_TextBox");
            possibleMaxTextBoxNames.Add("asnMax_TextBox");
            possibleMaxTextBoxNames.Add("aspMax_TextBox");
            possibleMaxTextBoxNames.Add("cysMax_TextBox");
            possibleMaxTextBoxNames.Add("glnMax_TextBox");
            possibleMaxTextBoxNames.Add("gluMax_TextBox");
            possibleMaxTextBoxNames.Add("glyMax_TextBox");
            possibleMaxTextBoxNames.Add("hisMax_TextBox");
            possibleMaxTextBoxNames.Add("ileMax_TextBox");
            possibleMaxTextBoxNames.Add("leuMax_TextBox");
            possibleMaxTextBoxNames.Add("lysMax_TextBox");
            possibleMaxTextBoxNames.Add("metMax_TextBox");
            possibleMaxTextBoxNames.Add("pheMax_TextBox");
            possibleMaxTextBoxNames.Add("serMax_TextBox");
            possibleMaxTextBoxNames.Add("thrMax_TextBox");
            possibleMaxTextBoxNames.Add("trpMax_TextBox");
            possibleMaxTextBoxNames.Add("tyrMax_TextBox");
            possibleMaxTextBoxNames.Add("valMax_TextBox");
            possibleMaxTextBoxNames.Add("proMax_TextBox");

            //Permethyl 
            possibleMaxTextBoxNames.Add("pHexMax_TextBox");
            possibleMaxTextBoxNames.Add("pHxNAcMax_TextBox");
            possibleMaxTextBoxNames.Add("pDxHexMax_TextBox");
            possibleMaxTextBoxNames.Add("pPntosMax_TextBox");
            possibleMaxTextBoxNames.Add("pNuAcMax_TextBox");
            possibleMaxTextBoxNames.Add("pNuGcMax_TextBox");
            possibleMaxTextBoxNames.Add("pKDNMax_TextBox");
            possibleMaxTextBoxNames.Add("pHxAMax_TextBox");
            #endregion


            for (int i = 0; i <= (checkIfCheckBoxesChecked.Count - 1); i++)
                if (checkIfCheckBoxesChecked[i] == true)
                {
                    if (columnCheck >= 10 && columnCheck < 20 && numColumns == 1)
                    {
                        extraRowsCheck++;
                        utilitiesRangesPage_ListBox.Height = 340 + (34 * extraRowsCheck);
                        utilitiesRangesPage_MiniGrid.Height = 340 + (34 * extraRowsCheck);
                        utilitiesRangesPage_Grid.Height = 400 + (34 * extraRowsCheck);
                        utilitiesRangesSubHomePage.Height = 400 + (34 * extraRowsCheck);
                    }

                    if (columnCheck == 20)
                    {
                        numColumns++;
                        utilitiesRangesPage_ListBox.Width = 380 * numColumns;
                        utilitiesRangesPage_MiniGrid.Width = 380 * numColumns;
                        utilitiesRangesPage_Grid.Width = 400 * numColumns;
                        utilitiesRangesSubHomePage.Width = 400 * numColumns;

                        ColumnDefinition miniGrid_ColumnDefinition = new ColumnDefinition();
                        miniGrid_ColumnDefinition.Width = new System.Windows.GridLength(380);
                        utilitiesRangesPage_MiniGrid.ColumnDefinitions.Add(miniGrid_ColumnDefinition);

                        columnCheck = 0;
                        extraRowsCheck = 0;
                    }

                    possibleButtons[i].Content = possibleButtonContent[i];
                    possibleButtons[i].Name = possibleButtonNames[i];
                    possibleButtons[i].Style = (Style)Application.Current.Resources["listBoxButtonStyle"];
                    rangesButtons.Add(possibleButtons[i]);

                    possibleButtons[i].Click += this.hexose_Button_Click;

                    possibleMinTextBoxes[i].Name = possibleMinTextBoxNames[i];
                    possibleMinTextBoxes[i].Style = (Style)Application.Current.Resources["minTextBoxStyle"];
                    possibleMinTextBoxes[i].Text = (possibleMinVariables[i]).ToString();
                    rangesMinTextBoxes.Add(possibleMinTextBoxes[i]);

                    possibleMinTextBoxes[i].TextChanged += this.hexoseMin_TextBox_TextChanged;

                    possibleMaxTextBoxes[i].Name = possibleMaxTextBoxNames[i];
                    possibleMaxTextBoxes[i].Style = (Style)Application.Current.Resources["maxTextBoxStyle"];
                    possibleMaxTextBoxes[i].Text = (possibleMaxVariables[i]).ToString();
                    rangesMaxTextBoxes.Add(possibleMaxTextBoxes[i]);

                    possibleMaxTextBoxes[i].TextChanged += this.hexoseMin_TextBox_TextChanged;

                    columnCheck++;
                }
        }

        void hexose_Button_Click(Object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            switch (clickedButton.Name)
            {
                //Sugars
                case "hexose_Button":
                    {
                        App.parametersRangesPageVariables.MinHexose_Int = 3;
                        hexoseMin_TextBox.Text = App.parametersRangesPageVariables.MinHexose_Int.ToString();
                        App.parametersRangesPageVariables.MaxHexose_Int = 12;
                        hexoseMax_TextBox.Text = App.parametersRangesPageVariables.MaxHexose_Int.ToString();
                        break;
                    }
                case "hexNAc_Button":
                    {
                        App.parametersRangesPageVariables.MinHexNAc_Int = 2;
                        hexNAcMin_TextBox.Text = App.parametersRangesPageVariables.MinHexNAc_Int.ToString();
                        App.parametersRangesPageVariables.MaxHexNAc_Int = 9;
                        hexNAcMax_TextBox.Text = App.parametersRangesPageVariables.MaxHexNAc_Int.ToString();
                        break;
                    }
                case "dxyHex_Button":
                    {
                        App.parametersRangesPageVariables.MinDxyHex_Int = 0;
                        dxyHexMin_TextBox.Text = App.parametersRangesPageVariables.MinDxyHex_Int.ToString();
                        App.parametersRangesPageVariables.MaxDxyHex_Int = 5;
                        dxyHexMax_TextBox.Text = App.parametersRangesPageVariables.MaxDxyHex_Int.ToString();
                        break;
                    }
                case "pentose_Button":
                    {
                        App.parametersRangesPageVariables.MinPentose_Int = 0;
                        pentoseMin_TextBox.Text = App.parametersRangesPageVariables.MinPentose_Int.ToString();
                        App.parametersRangesPageVariables.MaxPentose_Int = 1;
                        pentoseMax_TextBox.Text = App.parametersRangesPageVariables.MaxPentose_Int.ToString();
                        break;
                    }
                case "neuAc_Button":
                    {
                        App.parametersRangesPageVariables.MinNeuAc_Int = 0;
                        neuAcMin_TextBox.Text = App.parametersRangesPageVariables.MinNeuAc_Int.ToString();
                        App.parametersRangesPageVariables.MaxNeuAc_Int = 5;
                        neuAcMax_TextBox.Text = App.parametersRangesPageVariables.MaxNeuAc_Int.ToString();
                        break;
                    }
                case "neuGc_Button":
                    {
                        App.parametersRangesPageVariables.MinNeuGc_Int = 0;
                        neuGcMin_TextBox.Text = App.parametersRangesPageVariables.MinNeuGc_Int.ToString();
                        App.parametersRangesPageVariables.MaxNeuGc_Int = 4;
                        neuGcMax_TextBox.Text = App.parametersRangesPageVariables.MaxNeuGc_Int.ToString();
                        break;
                    }
                case "kDN_Button":
                    {
                        App.parametersRangesPageVariables.MinKDN_Int = 0;
                        kDNMin_TextBox.Text = App.parametersRangesPageVariables.MinKDN_Int.ToString();
                        App.parametersRangesPageVariables.MaxKDN_Int = 1;
                        kDNMax_TextBox.Text = App.parametersRangesPageVariables.MaxKDN_Int.ToString();
                        break;
                    }
                case "hexA_Button":
                    {
                        App.parametersRangesPageVariables.MinHexA_Int = 0;
                        hexAMin_TextBox.Text = App.parametersRangesPageVariables.MinHexA_Int.ToString();
                        App.parametersRangesPageVariables.MaxHexA_Int = 1;
                        hexAMax_TextBox.Text = App.parametersRangesPageVariables.MaxHexA_Int.ToString();
                        break;
                    }
                //User Units
                case "userUnit1_Button":
                    {
                        App.parametersRangesPageVariables.MinUserUnit1_Int = 0;
                        userUnit1Min_TextBox.Text = App.parametersRangesPageVariables.MinUserUnit1_Int.ToString();
                        App.parametersRangesPageVariables.MaxUserUnit1_Int = 1;
                        userUnit1Max_TextBox.Text = App.parametersRangesPageVariables.MaxUserUnit1_Int.ToString();
                        break;
                    }
                case "userUnit2_Button":
                    {
                        App.parametersRangesPageVariables.MinUserUnit2_Int = 0;
                        userUnit2Min_TextBox.Text = App.parametersRangesPageVariables.MinUserUnit2_Int.ToString();
                        App.parametersRangesPageVariables.MaxUserUnit2_Int = 1;
                        userUnit2Max_TextBox.Text = App.parametersRangesPageVariables.MaxUserUnit2_Int.ToString();
                        break;
                    }

                //Specials
                case "naH_Button":
                    {
                        App.parametersRangesPageVariables.MinNaH_Int = 0;
                        naHMin_TextBox.Text = App.parametersRangesPageVariables.MinNaH_Int.ToString();
                        App.parametersRangesPageVariables.MaxNaH_Int = 4;
                        naHMax_TextBox.Text = App.parametersRangesPageVariables.MaxNaH_Int.ToString();
                        break;
                    }
                case "cH3_Button":
                    {
                        App.parametersRangesPageVariables.MinCH3_Int = 0;
                        cH3Min_TextBox.Text = App.parametersRangesPageVariables.MinCH3_Int.ToString();
                        App.parametersRangesPageVariables.MaxCH3_Int = 4;
                        cH3Max_TextBox.Text = App.parametersRangesPageVariables.MaxCH3_Int.ToString();
                        break;
                    }
                case "sO3_Button":
                    {
                        App.parametersRangesPageVariables.MinSO3_Int = 0;
                        sO3Min_TextBox.Text = App.parametersRangesPageVariables.MinSO3_Int.ToString();
                        App.parametersRangesPageVariables.MaxSO3_Int = 1;
                        sO3Max_TextBox.Text = App.parametersRangesPageVariables.MaxSO3_Int.ToString();
                        break;
                    }
                case "oAcetyl_Button":
                    {
                        App.parametersRangesPageVariables.MinOAcetyl_Int = 0;
                        oAcetylMin_TextBox.Text = App.parametersRangesPageVariables.MinOAcetyl_Int.ToString();
                        App.parametersRangesPageVariables.MaxOAcetyl_Int = 1;
                        oAcetylMax_TextBox.Text = App.parametersRangesPageVariables.MaxOAcetyl_Int.ToString();
                        break;
                    }

                //Amino Acids
                case "ala_Button":
                    {
                        App.parametersRangesPageVariables.MinAla_Int = 0;
                        alaMin_TextBox.Text = App.parametersRangesPageVariables.MinAla_Int.ToString();
                        App.parametersRangesPageVariables.MaxAla_Int = 1;
                        alaMax_TextBox.Text = App.parametersRangesPageVariables.MaxAla_Int.ToString();
                        break;
                    }
                case "arg_Button":
                    {
                        App.parametersRangesPageVariables.MinArg_Int = 0;
                        argMin_TextBox.Text = App.parametersRangesPageVariables.MinArg_Int.ToString();
                        App.parametersRangesPageVariables.MaxArg_Int = 1;
                        argMax_TextBox.Text = App.parametersRangesPageVariables.MaxArg_Int.ToString();
                        break;
                    }
                case "asn_Button":
                    {
                        App.parametersRangesPageVariables.MinAsn_Int = 0;
                        asnMin_TextBox.Text = App.parametersRangesPageVariables.MinAsn_Int.ToString();
                        App.parametersRangesPageVariables.MaxAsn_Int = 1;
                        asnMax_TextBox.Text = App.parametersRangesPageVariables.MaxAsn_Int.ToString();
                        break;
                    }
                case "asp_Button":
                    {
                        App.parametersRangesPageVariables.MinAsp_Int = 0;
                        aspMin_TextBox.Text = App.parametersRangesPageVariables.MinAsp_Int.ToString();
                        App.parametersRangesPageVariables.MaxAsp_Int = 1;
                        aspMax_TextBox.Text = App.parametersRangesPageVariables.MaxAsp_Int.ToString();
                        break;
                    }
                case "cys_Button":
                    {
                        App.parametersRangesPageVariables.MinCys_Int = 0;
                        cysMin_TextBox.Text = App.parametersRangesPageVariables.MinCys_Int.ToString();
                        App.parametersRangesPageVariables.MaxCys_Int = 1;
                        cysMax_TextBox.Text = App.parametersRangesPageVariables.MaxCys_Int.ToString();
                        break;
                    }
                case "gln_Button":
                    {
                        App.parametersRangesPageVariables.MinGln_Int = 0;
                        glnMin_TextBox.Text = App.parametersRangesPageVariables.MinGln_Int.ToString();
                        App.parametersRangesPageVariables.MaxGln_Int = 1;
                        glnMax_TextBox.Text = App.parametersRangesPageVariables.MaxGln_Int.ToString();
                        break;
                    }
                case "glu_Button":
                    {
                        App.parametersRangesPageVariables.MinGlu_Int = 0;
                        gluMin_TextBox.Text = App.parametersRangesPageVariables.MinGlu_Int.ToString();
                        App.parametersRangesPageVariables.MaxGlu_Int = 1;
                        gluMax_TextBox.Text = App.parametersRangesPageVariables.MaxGlu_Int.ToString();
                        break;
                    }
                case "gly_Button":
                    {
                        App.parametersRangesPageVariables.MinGly_Int = 0;
                        glyMin_TextBox.Text = App.parametersRangesPageVariables.MinGly_Int.ToString();
                        App.parametersRangesPageVariables.MaxGly_Int = 0;
                        glyMax_TextBox.Text = App.parametersRangesPageVariables.MaxGly_Int.ToString();
                        break;
                    }
                case "his_Button":
                    {
                        App.parametersRangesPageVariables.MinHis_Int = 0;
                        hisMin_TextBox.Text = App.parametersRangesPageVariables.MinHis_Int.ToString();
                        App.parametersRangesPageVariables.MaxHis_Int = 1;
                        hisMax_TextBox.Text = App.parametersRangesPageVariables.MaxHis_Int.ToString();
                        break;
                    }
                case "ile_Button":
                    {
                        App.parametersRangesPageVariables.MinIle_Int = 0;
                        ileMin_TextBox.Text = App.parametersRangesPageVariables.MinIle_Int.ToString();
                        App.parametersRangesPageVariables.MaxIle_Int = 1;
                        ileMax_TextBox.Text = App.parametersRangesPageVariables.MaxIle_Int.ToString();
                        break;
                    }
                case "leu_Button":
                    {
                        App.parametersRangesPageVariables.MinLeu_Int = 0;
                        leuMin_TextBox.Text = App.parametersRangesPageVariables.MinLeu_Int.ToString();
                        App.parametersRangesPageVariables.MaxLeu_Int = 1;
                        leuMax_TextBox.Text = App.parametersRangesPageVariables.MaxLeu_Int.ToString();
                        break;
                    }
                case "lys_Button":
                    {
                        App.parametersRangesPageVariables.MinLys_Int = 0;
                        lysMin_TextBox.Text = App.parametersRangesPageVariables.MinLys_Int.ToString();
                        App.parametersRangesPageVariables.MaxLys_Int = 1;
                        lysMax_TextBox.Text = App.parametersRangesPageVariables.MaxLys_Int.ToString();
                        break;
                    }
                case "met_Button":
                    {
                        App.parametersRangesPageVariables.MinMet_Int = 0;
                        metMin_TextBox.Text = App.parametersRangesPageVariables.MinMet_Int.ToString();
                        App.parametersRangesPageVariables.MaxMet_Int = 1;
                        metMax_TextBox.Text = App.parametersRangesPageVariables.MaxMet_Int.ToString();
                        break;
                    }
                case "phe_Button":
                    {
                        App.parametersRangesPageVariables.MinPhe_Int = 0;
                        pheMin_TextBox.Text = App.parametersRangesPageVariables.MinPhe_Int.ToString();
                        App.parametersRangesPageVariables.MaxPhe_Int = 1;
                        pheMax_TextBox.Text = App.parametersRangesPageVariables.MaxPhe_Int.ToString();
                        break;
                    }
                case "ser_Button":
                    {
                        App.parametersRangesPageVariables.MinSer_Int = 0;
                        serMin_TextBox.Text = App.parametersRangesPageVariables.MinSer_Int.ToString();
                        App.parametersRangesPageVariables.MaxSer_Int = 1;
                        serMax_TextBox.Text = App.parametersRangesPageVariables.MaxSer_Int.ToString();
                        break;
                    }
                case "thr_Button":
                    {
                        App.parametersRangesPageVariables.MinThr_Int = 0;
                        thrMin_TextBox.Text = App.parametersRangesPageVariables.MinThr_Int.ToString();
                        App.parametersRangesPageVariables.MaxThr_Int = 1;
                        thrMax_TextBox.Text = App.parametersRangesPageVariables.MaxThr_Int.ToString();
                        break;
                    }
                case "trp_Button":
                    {
                        App.parametersRangesPageVariables.MinTrp_Int = 0;
                        trpMin_TextBox.Text = App.parametersRangesPageVariables.MinTrp_Int.ToString();
                        App.parametersRangesPageVariables.MaxTrp_Int = 1;
                        trpMax_TextBox.Text = App.parametersRangesPageVariables.MaxTrp_Int.ToString();
                        break;
                    }
                case "tyr_Button":
                    {
                        App.parametersRangesPageVariables.MinTyr_Int = 0;
                        tyrMin_TextBox.Text = App.parametersRangesPageVariables.MinTyr_Int.ToString();
                        App.parametersRangesPageVariables.MaxTyr_Int = 1;
                        tyrMax_TextBox.Text = App.parametersRangesPageVariables.MaxTyr_Int.ToString();
                        break;
                    }
                case "val_Button":
                    {
                        App.parametersRangesPageVariables.MinVal_Int = 0;
                        valMin_TextBox.Text = App.parametersRangesPageVariables.MinVal_Int.ToString();
                        App.parametersRangesPageVariables.MaxVal_Int = 1;
                        valMax_TextBox.Text = App.parametersRangesPageVariables.MaxVal_Int.ToString();
                        break;
                    }
                case "pro_Button":
                    {
                        App.parametersRangesPageVariables.MinPro_Int = 0;
                        proMin_TextBox.Text = App.parametersRangesPageVariables.MinPro_Int.ToString();
                        App.parametersRangesPageVariables.MaxPro_Int = 1;
                        proMax_TextBox.Text = App.parametersRangesPageVariables.MaxPro_Int.ToString();
                        break;
                    }

                //Permethyl
                case "pHex_Button":
                    {
                        App.parametersRangesPageVariables.MinpHex_Int = 3;
                        pHexMin_TextBox.Text = App.parametersRangesPageVariables.MinpHex_Int.ToString();
                        App.parametersRangesPageVariables.MaxpHex_Int = 12;
                        pHexMax_TextBox.Text = App.parametersRangesPageVariables.MaxpHex_Int.ToString();
                        break;
                    }
                case "pHxNAc_Button":
                    {
                        App.parametersRangesPageVariables.MinpHxNAc_Int = 2;
                        pHxNAcMin_TextBox.Text = App.parametersRangesPageVariables.MinpHxNAc_Int.ToString();
                        App.parametersRangesPageVariables.MaxpHxNAc_Int = 9;
                        pHxNAcMax_TextBox.Text = App.parametersRangesPageVariables.MaxpHxNAc_Int.ToString();
                        break;
                    }
                case "pDxHex_Button":
                    {
                        App.parametersRangesPageVariables.MinpDxHex_Int = 0;
                        pDxHexMin_TextBox.Text = App.parametersRangesPageVariables.MinpDxHex_Int.ToString();
                        App.parametersRangesPageVariables.MaxpDxHex_Int = 5;
                        pDxHexMax_TextBox.Text = App.parametersRangesPageVariables.MaxpDxHex_Int.ToString();
                        break;
                    }
                case "pPntos_Button":
                    {
                        App.parametersRangesPageVariables.MinpPntos_Int = 0;
                        pPntosMin_TextBox.Text = App.parametersRangesPageVariables.MinpPntos_Int.ToString();
                        App.parametersRangesPageVariables.MaxpPntos_Int = 1;
                        pPntosMax_TextBox.Text = App.parametersRangesPageVariables.MaxpPntos_Int.ToString();
                        break;
                    }
                case "pNuAc_Button":
                    {
                        App.parametersRangesPageVariables.MinpNuAc_Int = 0;
                        pNuAcMin_TextBox.Text = App.parametersRangesPageVariables.MinpNuAc_Int.ToString();
                        App.parametersRangesPageVariables.MaxpNuAc_Int = 4;
                        pNuAcMax_TextBox.Text = App.parametersRangesPageVariables.MaxpNuAc_Int.ToString();
                        break;
                    }
                case "pNuGc_Button":
                    {
                        App.parametersRangesPageVariables.MinpNuGc_Int = 0;
                        pNuGcMin_TextBox.Text = App.parametersRangesPageVariables.MinpNuGc_Int.ToString();
                        App.parametersRangesPageVariables.MaxpNuGc_Int = 4;
                        pNuGcMax_TextBox.Text = App.parametersRangesPageVariables.MaxpNuGc_Int.ToString();
                        break;
                    }
                case "pKDN_Button":
                    {
                        App.parametersRangesPageVariables.MinpKDN_Int = 0;
                        pKDNMin_TextBox.Text = App.parametersRangesPageVariables.MinpKDN_Int.ToString();
                        App.parametersRangesPageVariables.MaxpKDN_Int = 1;
                        pKDNMax_TextBox.Text = App.parametersRangesPageVariables.MaxpKDN_Int.ToString();
                        break;
                    }
                case "pHxA_Button":
                    {
                        App.parametersRangesPageVariables.MinpHxA_Int = 0;
                        pHxAMin_TextBox.Text = App.parametersRangesPageVariables.MinpHxA_Int.ToString();
                        App.parametersRangesPageVariables.MaxpHxA_Int = 1;
                        pHxAMax_TextBox.Text = App.parametersRangesPageVariables.MaxpHxA_Int.ToString();
                        break;
                    }
            }
        }

        protected void hexoseMin_TextBox_TextChanged(object sender, EventArgs e)
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
                #region Min
                //Min
                //Sugars
                case "hexoseMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinHexose_Int = tempText;
                        hexoseMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinHexose_Int).ToString();
                        break;
                    }
                case "hexNAcMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinHexNAc_Int = tempText;
                        hexNAcMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinHexNAc_Int).ToString();
                        break;
                    }
                case "dxyHexMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinDxyHex_Int = tempText;
                        dxyHexMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinDxyHex_Int).ToString();
                        break;
                    }
                case "pentoseMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinPentose_Int = tempText;
                        pentoseMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinPentose_Int).ToString();
                        break;
                    }
                case "neuAcMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinNeuAc_Int = tempText;
                        neuAcMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinNeuAc_Int).ToString();
                        break;
                    }
                case "neuGcMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinNeuGc_Int = tempText;
                        neuGcMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinNeuGc_Int).ToString();
                        break;
                    }
                case "kDNMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinKDN_Int = tempText;
                        kDNMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinKDN_Int).ToString();
                        break;
                    }
                case "hexAMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinHexA_Int = tempText;
                        hexAMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinHexA_Int).ToString();
                        break;
                    }

                //Special
                case "naHMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinNaH_Int = tempText;
                        naHMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinNaH_Int).ToString();
                        break;
                    }
                case "cH3Min_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinCH3_Int = tempText;
                        cH3Min_TextBox.Text = (App.utilitiesRangesPageVariables.MinCH3_Int).ToString();
                        break;
                    }
                case "sO3Min_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinSO3_Int = tempText;
                        sO3Min_TextBox.Text = (App.utilitiesRangesPageVariables.MinSO3_Int).ToString();
                        break;
                    }
                case "oAcetylMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinOAcetyl_Int = tempText;
                        oAcetylMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinOAcetyl_Int).ToString();
                        break;
                    }


                //User
                case "userUnit1Min_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinUserUnit1_Int = tempText;
                        userUnit1Min_TextBox.Text = (App.utilitiesRangesPageVariables.MinUserUnit1_Int).ToString();
                        break;
                    }
                case "userUnit2Min_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinUserUnit2_Int = tempText;
                        userUnit2Min_TextBox.Text = (App.utilitiesRangesPageVariables.MinUserUnit2_Int).ToString();
                        break;
                    }

                //Amino Acids
                case "alaMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinAla_Int = tempText;
                        alaMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinAla_Int).ToString();
                        break;
                    }
                case "argMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinArg_Int = tempText;
                        argMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinArg_Int).ToString();
                        break;
                    }
                case "asnMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinAsn_Int = tempText;
                        asnMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinAsn_Int).ToString();
                        break;
                    }
                case "aspMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinAsp_Int = tempText;
                        aspMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinAsp_Int).ToString();
                        break;
                    }
                case "cysMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinCys_Int = tempText;
                        cysMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinCys_Int).ToString();
                        break;
                    }
                case "glnMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinGln_Int = tempText;
                        glnMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinGln_Int).ToString();
                        break;
                    }
                case "gluMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinGlu_Int = tempText;
                        gluMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinGlu_Int).ToString();
                        break;
                    }
                case "glyMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinGly_Int = tempText;
                        glyMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinGly_Int).ToString();
                        break;
                    }
                case "hisMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinHis_Int = tempText;
                        hisMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinHis_Int).ToString();
                        break;
                    }
                case "ileMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinIle_Int = tempText;
                        ileMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinIle_Int).ToString();
                        break;
                    }
                case "leuMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinLeu_Int = tempText;
                        leuMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinLeu_Int).ToString();
                        break;
                    }
                case "lysMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinLys_Int = tempText;
                        lysMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinLys_Int).ToString();
                        break;
                    }
                case "metMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinMet_Int = tempText;
                        metMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinMet_Int).ToString();
                        break;
                    }
                case "pheMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinPhe_Int = tempText;
                        pheMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinPhe_Int).ToString();
                        break;
                    }
                case "serMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinSer_Int = tempText;
                        serMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinSer_Int).ToString();
                        break;
                    }
                case "thrMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinThr_Int = tempText;
                        thrMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinThr_Int).ToString();
                        break;
                    }
                case "trpMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinTrp_Int = tempText;
                        trpMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinTrp_Int).ToString();
                        break;
                    }
                case "tyrMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinTyr_Int = tempText;
                        tyrMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinTyr_Int).ToString();
                        break;
                    }
                case "valMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinVal_Int = tempText;
                        valMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinVal_Int).ToString();
                        break;
                    }
                case "proMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinPro_Int = tempText;
                        proMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinPro_Int).ToString();
                        break;
                    }

                //Permethyl
                case "pHexMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinpHex_Int = tempText;
                        pHexMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinpHex_Int).ToString();
                        break;
                    }
                case "pHxNAcMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinpHxNAc_Int = tempText;
                        pHxNAcMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinpHxNAc_Int).ToString();
                        break;
                    }
                case "pDxHexMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinpDxHex_Int = tempText;
                        pDxHexMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinpDxHex_Int).ToString();
                        break;
                    }
                case "pPntosMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinpPntos_Int = tempText;
                        pPntosMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinpPntos_Int).ToString();
                        break;
                    }
                case "pNuAcMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinpNuAc_Int = tempText;
                        pNuAcMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinpNuAc_Int).ToString();
                        break;
                    }
                case "pNuGcMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinpNuGc_Int = tempText;
                        pNuGcMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinpNuGc_Int).ToString();
                        break;
                    }
                case "pKDNMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinpKDN_Int = tempText;
                        pKDNMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinpKDN_Int).ToString();
                        break;
                    }
                case "pHxAMin_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MinpHxA_Int = tempText;
                        pHxAMin_TextBox.Text = (App.utilitiesRangesPageVariables.MinpHxA_Int).ToString();
                        break;
                    }
                #endregion

                #region Max
                //Max
                //Sugars
                case "hexoseMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxHexose_Int = tempText;
                        hexoseMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxHexose_Int).ToString();
                        break;
                    }
                case "hexNAcMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxHexNAc_Int = tempText;
                        hexNAcMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxHexNAc_Int).ToString();
                        break;
                    }
                case "dxyHexMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxDxyHex_Int = tempText;
                        dxyHexMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxDxyHex_Int).ToString();
                        break;
                    }
                case "pentoseMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxPentose_Int = tempText;
                        pentoseMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxPentose_Int).ToString();
                        break;
                    }
                case "neuAcMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxNeuAc_Int = tempText;
                        neuAcMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxNeuAc_Int).ToString();
                        break;
                    }
                case "neuGcMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxNeuGc_Int = tempText;
                        neuGcMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxNeuGc_Int).ToString();
                        break;
                    }
                case "kDNMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxKDN_Int = tempText;
                        kDNMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxKDN_Int).ToString();
                        break;
                    }
                case "hexAMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxHexA_Int = tempText;
                        hexAMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxHexA_Int).ToString();
                        break;
                    }

                //Special
                case "naHMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxNaH_Int = tempText;
                        naHMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxNaH_Int).ToString();
                        break;
                    }
                case "cH3Max_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxCH3_Int = tempText;
                        cH3Max_TextBox.Text = (App.utilitiesRangesPageVariables.MaxCH3_Int).ToString();
                        break;
                    }
                case "sO3Max_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxSO3_Int = tempText;
                        sO3Max_TextBox.Text = (App.utilitiesRangesPageVariables.MaxSO3_Int).ToString();
                        break;
                    }
                case "oAcetylMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxOAcetyl_Int = tempText;
                        oAcetylMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxOAcetyl_Int).ToString();
                        break;
                    }

                //User
                case "userUnit1Max_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxUserUnit1_Int = tempText;
                        userUnit1Max_TextBox.Text = (App.utilitiesRangesPageVariables.MaxUserUnit1_Int).ToString();
                        break;
                    }
                case "userUnit2Max_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxUserUnit2_Int = tempText;
                        userUnit2Max_TextBox.Text = (App.utilitiesRangesPageVariables.MaxUserUnit2_Int).ToString();
                        break;
                    }

                //Amino Acids
                case "alaMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxAla_Int = tempText;
                        alaMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxAla_Int).ToString();
                        break;
                    }
                case "argMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxArg_Int = tempText;
                        argMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxArg_Int).ToString();
                        break;
                    }
                case "asnMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxAsn_Int = tempText;
                        asnMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxAsn_Int).ToString();
                        break;
                    }
                case "aspMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxAsp_Int = tempText;
                        aspMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxAsp_Int).ToString();
                        break;
                    }
                case "cysMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxCys_Int = tempText;
                        cysMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxCys_Int).ToString();
                        break;
                    }
                case "glnMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxGln_Int = tempText;
                        glnMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxGln_Int).ToString();
                        break;
                    }
                case "gluMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxGlu_Int = tempText;
                        gluMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxGlu_Int).ToString();
                        break;
                    }
                case "glyMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxGly_Int = tempText;
                        glyMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxGly_Int).ToString();
                        break;
                    }
                case "hisMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxHis_Int = tempText;
                        hisMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxHis_Int).ToString();
                        break;
                    }
                case "ileMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxIle_Int = tempText;
                        ileMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxIle_Int).ToString();
                        break;
                    }
                case "leuMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxLeu_Int = tempText;
                        leuMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxLeu_Int).ToString();
                        break;
                    }
                case "lysMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxLys_Int = tempText;
                        lysMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxLys_Int).ToString();
                        break;
                    }
                case "metMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxMet_Int = tempText;
                        metMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxMet_Int).ToString();
                        break;
                    }
                case "pheMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxPhe_Int = tempText;
                        pheMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxPhe_Int).ToString();
                        break;
                    }
                case "serMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxSer_Int = tempText;
                        serMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxSer_Int).ToString();
                        break;
                    }
                case "thrMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxThr_Int = tempText;
                        thrMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxThr_Int).ToString();
                        break;
                    }
                case "trpMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxTrp_Int = tempText;
                        trpMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxTrp_Int).ToString();
                        break;
                    }
                case "tyrMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxTyr_Int = tempText;
                        tyrMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxTyr_Int).ToString();
                        break;
                    }
                case "valMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxVal_Int = tempText;
                        valMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxVal_Int).ToString();
                        break;
                    }
                case "proMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxPro_Int = tempText;
                        proMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxPro_Int).ToString();
                        break;
                    }

                //Permethyl
                case "pHexMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxpHex_Int = tempText;
                        pHexMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxpHex_Int).ToString();
                        break;
                    }
                case "pHxNAcMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxpHxNAc_Int = tempText;
                        pHxNAcMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxpHxNAc_Int).ToString();
                        break;
                    }
                case "pDxHexMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxpDxHex_Int = tempText;
                        pDxHexMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxpDxHex_Int).ToString();
                        break;
                    }
                case "pPntosMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxpPntos_Int = tempText;
                        pPntosMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxpPntos_Int).ToString();
                        break;
                    }
                case "pNuAcMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxpNuAc_Int = tempText;
                        pNuAcMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxpNuAc_Int).ToString();
                        break;
                    }
                case "pNuGcMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxpNuGc_Int = tempText;
                        pNuGcMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxpNuGc_Int).ToString();
                        break;
                    }
                case "pKDNMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxpKDN_Int = tempText;
                        pKDNMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxpKDN_Int).ToString();
                        break;
                    }
                case "pHxAMax_TextBox":
                    {
                        App.utilitiesRangesPageVariables.MaxpHxA_Int = tempText;
                        pHxAMax_TextBox.Text = (App.utilitiesRangesPageVariables.MaxpHxA_Int).ToString();
                        break;
                    }
                #endregion
            }
        }
    }
}
