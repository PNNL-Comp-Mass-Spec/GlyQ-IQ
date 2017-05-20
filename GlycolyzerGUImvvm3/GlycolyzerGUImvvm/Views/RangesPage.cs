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
using System.Windows.Controls;
using GlycolyzerGUImvvm.ViewModels;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;

namespace GlycolyzerGUImvvm.Views
{
    public class RangesPage
    {
        //Canvas to Draw the Box Outlines On
        public Canvas rangesPage_Canvas = new Canvas();

        //Grid to Organize Design
        Grid rangesPage_Grid = new Grid();
        Grid rangesPage_MiniGrid = new Grid();

        //List Box to Hold the Ranges
        ListBox rangesPage_ListBox = new ListBox();

        //Lists of Buttons, Min and Max TextBoxes, Min and Max Labels, and User Unit Masses
        List<Button> rangesButtons = new List<Button>();
        List<TextBox> rangesMinTextBoxes = new List<TextBox>();
        List<TextBox> rangesMaxTextBoxes = new List<TextBox>();
        List<TextBox> possibleUserUnitMass_TextBoxes = new List<TextBox>();
        List<String> possibleUserUnitMass_Variables = new List<String>();

        //View Model
        private RangesViewModel rangesVM = new RangesViewModel();

       
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
        TextBox userUnit1Mass_TextBox = new TextBox();

        TextBox userUnit2Min_TextBox = new TextBox();
        TextBox userUnit2Max_TextBox = new TextBox();
        TextBox userUnit2Mass_TextBox = new TextBox();

        TextBox userUnit3Min_TextBox = new TextBox();
        TextBox userUnit3Max_TextBox = new TextBox();
        TextBox userUnit3Mass_TextBox = new TextBox();

        TextBox userUnit4Min_TextBox = new TextBox();
        TextBox userUnit4Max_TextBox = new TextBox();
        TextBox userUnit4Mass_TextBox = new TextBox();

        TextBox userUnit5Min_TextBox = new TextBox();
        TextBox userUnit5Max_TextBox = new TextBox();
        TextBox userUnit5Mass_TextBox = new TextBox();

        TextBox userUnit6Min_TextBox = new TextBox();
        TextBox userUnit6Max_TextBox = new TextBox();
        TextBox userUnit6Mass_TextBox = new TextBox();

        TextBox userUnit7Min_TextBox = new TextBox();
        TextBox userUnit7Max_TextBox = new TextBox();
        TextBox userUnit7Mass_TextBox = new TextBox();

        TextBox userUnit8Min_TextBox = new TextBox();
        TextBox userUnit8Max_TextBox = new TextBox();
        TextBox userUnit8Mass_TextBox = new TextBox();

        TextBox userUnit9Min_TextBox = new TextBox();
        TextBox userUnit9Max_TextBox = new TextBox();
        TextBox userUnit9Mass_TextBox = new TextBox();

        TextBox userUnit10Min_TextBox = new TextBox();
        TextBox userUnit10Max_TextBox = new TextBox();
        TextBox userUnit10Mass_TextBox = new TextBox();

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

        TextBox proMin_TextBox = new TextBox();
        TextBox proMax_TextBox = new TextBox();

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


        public RangesPage()
        {
            RangesPage_Canvas();
        }

        private void RangesPage_Canvas()
        {
            //Build Grid and Design
            RangesPage_Grid();

            //Add Grid to Canvas
            rangesPage_Canvas.Children.Add(rangesPage_Grid);
        }

        private void RangesPage_Grid()
        {
            #region Main Grid Row Definitions
            //2 Row x 1 Column Main Grid   
            RowDefinition Main_RowDefinition = new RowDefinition();
            Main_RowDefinition.Height = new System.Windows.GridLength(40);
            rangesPage_Grid.RowDefinitions.Add(Main_RowDefinition);

            RowDefinition Main_RowDefinition1 = new RowDefinition();
            Main_RowDefinition1.Height = System.Windows.GridLength.Auto;
            rangesPage_Grid.RowDefinitions.Add(Main_RowDefinition1);
            #endregion


            #region RangesPage Header Label
            //"Parameter Ranges" Head Label in 1st Row
            Label RangesPage_Label = new Label();
            RangesPage_Label.Content = "Ranges";
            RangesPage_Label.Margin = new Thickness(0, 0, 10, 0);
            RangesPage_Label.Style = (Style)Application.Current.Resources["secondaryHeaderLabelTextStyle"];
            Grid.SetRow(RangesPage_Label, 0);
            #endregion

            RangesPage_ListBox();
            Grid.SetRow(rangesPage_ListBox, 1);

            //Add Controls to Grid
            rangesPage_Grid.Children.Add(RangesPage_Label);
            rangesPage_Grid.Children.Add(rangesPage_ListBox);
        }

        private void RangesPage_ListBox()
        {
            #region ListBox 
            //ListBox 
            rangesPage_ListBox.HorizontalAlignment = HorizontalAlignment.Center;
            rangesPage_ListBox.Padding = new Thickness(0, 0, 10, 0);
            /*BrushConverter conv = new BrushConverter();
            SolidColorBrush brush = conv.ConvertFromString(App.rangesBGColor) as SolidColorBrush;
            rangesPage_ListBox.Background = brush;*/
            rangesPage_ListBox.Background = new SolidColorBrush(Colors.Transparent);
            rangesPage_ListBox.Style = (Style)Application.Current.Resources["listBoxStyle"];
            //Don't Display a Horizontal Scroll Bar
            rangesPage_ListBox.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled); 
            #endregion
            
            //Build the Range Controls Design
            RangesPage_Initialization();  
        }

        public void RangesPage_Initialization()
        {
            if (App.initializingFlagsModel.ParameterRangesSave_InitializeFlag)
            {
                App.parameterPage.Height = 432;
                App.parameterPage.Width = 400;
            }
            else if (App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag)
            {
                App.omniFinderGMRangesPage.Height = 400;
                App.omniFinderGMRangesPage.Width = 400;
            }

            rangesPage_Grid.Height = 400;
            rangesPage_Grid.Width = 400;

            rangesPage_ListBox.Height = 340;
            rangesPage_ListBox.Width = 380;

            rangesPage_MiniGrid.Height = 340;
            rangesPage_MiniGrid.Width = 380;

            //Call to Make Small Grids
            RangesPage_MiniGrid();
        }

        private void RangesPage_MiniGrid()
        {
            //Empty and Reset MiniGrid and All Lists Each Time Tab is Opened
            rangesPage_ListBox.Items.Clear();
            rangesPage_MiniGrid.Children.Clear();
            rangesButtons.Clear();
            rangesMinTextBoxes.Clear();
            rangesMaxTextBoxes.Clear();

            //Add 1 column
            ColumnDefinition miniGrid_ColumnDefinition = new ColumnDefinition();
            miniGrid_ColumnDefinition.Width = new System.Windows.GridLength(380);
            rangesPage_MiniGrid.ColumnDefinitions.Add(miniGrid_ColumnDefinition);

            //Call Method to Make the Needed Controls
            RangesPage_ListControls();

            #region possibleUserUnitMass_TextBoxes List
            //possibleUserUnitMass_TextBoxes List
            //User Units(10)
            possibleUserUnitMass_TextBoxes.Add(userUnit1Mass_TextBox);
            possibleUserUnitMass_TextBoxes.Add(userUnit2Mass_TextBox);
            possibleUserUnitMass_TextBoxes.Add(userUnit3Mass_TextBox);
            possibleUserUnitMass_TextBoxes.Add(userUnit4Mass_TextBox);
            possibleUserUnitMass_TextBoxes.Add(userUnit5Mass_TextBox);
            possibleUserUnitMass_TextBoxes.Add(userUnit6Mass_TextBox);
            possibleUserUnitMass_TextBoxes.Add(userUnit7Mass_TextBox);
            possibleUserUnitMass_TextBoxes.Add(userUnit8Mass_TextBox);
            possibleUserUnitMass_TextBoxes.Add(userUnit9Mass_TextBox);
            possibleUserUnitMass_TextBoxes.Add(userUnit10Mass_TextBox);
            #endregion

            #region possibleUserUnitMass_Variables List
            //possibleUserUnitMass_Variables List
            //User Units(10)
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit1_Double");
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit2_Double");
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit3_Double");
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit4_Double");
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit5_Double");
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit6_Double");
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit7_Double");
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit8_Double");
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit9_Double");
            possibleUserUnitMass_Variables.Add("RangesModel.MassUserUnit10_Double");
            #endregion

            int x = 0;
            int row = 0;
            int col = 0;

            //Add Controls to MiniGrid
            for (int i = 0; i <= (rangesButtons.Count - 1); i++)
            {
                #region Add Controls
                //Grid RangesPage_MiniGrid = new Grid();
                Label minLabel = new Label();
                Label maxLabel = new Label();
                
                //Style the Labels
                minLabel.Style = (Style)Application.Current.Resources["minListBoxLabelTextStyle"];
                maxLabel.Style = (Style)Application.Current.Resources["maxListBoxLabelTextStyle"];

                RowDefinition miniGrid_RowDefinition = new RowDefinition();
                miniGrid_RowDefinition.Height = new System.Windows.GridLength(34);
                rangesPage_MiniGrid.RowDefinitions.Add(miniGrid_RowDefinition);

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
                    rangesPage_MiniGrid.Children.Add(rangesButtons[i]);
                    rangesPage_MiniGrid.Children.Add(minLabel);
                    rangesPage_MiniGrid.Children.Add(rangesMinTextBoxes[i]);
                    rangesPage_MiniGrid.Children.Add(maxLabel);
                    rangesPage_MiniGrid.Children.Add(rangesMaxTextBoxes[i]);
                }
                // Catch invalid operation if child is already a child
                catch (InvalidOperationException e)
                {
                    rangesPage_MiniGrid.Children.Remove(rangesButtons[i]);
                    rangesPage_MiniGrid.Children.Remove(rangesMinTextBoxes[i]);
                    rangesPage_MiniGrid.Children.Remove(rangesMaxTextBoxes[i]);
                    rangesPage_MiniGrid.Children.Add(rangesButtons[i]);
                    rangesPage_MiniGrid.Children.Add(minLabel);
                    rangesPage_MiniGrid.Children.Add(rangesMinTextBoxes[i]);
                    rangesPage_MiniGrid.Children.Add(maxLabel);
                    rangesPage_MiniGrid.Children.Add(rangesMaxTextBoxes[i]);
                }
                #endregion

                #region Add User Unit Mass Controls
                if (rangesButtons[i].Name == "userUnit_Button" + (x+1))
                {
                    Label massLabel = new Label();
                    massLabel.Style = (Style)Application.Current.Resources["massListBoxLabelTextStyle"];
                    
                    possibleUserUnitMass_TextBoxes[x].Name = "userUnit" + (x + 1) + "Mass_TextBox";
                    possibleUserUnitMass_TextBoxes[x].Style = (Style)Application.Current.Resources["massTextBoxStyle"];

                    possibleUserUnitMass_TextBoxes[x].TextChanged += this.userUnit1Mass_TextBox_TextChanged;

                    Binding massBinding = new Binding();
                    massBinding.Source = rangesVM;
                    massBinding.Path = new PropertyPath(possibleUserUnitMass_Variables[x]);
                    massBinding.Mode = BindingMode.TwoWay;
                    possibleUserUnitMass_TextBoxes[x].SetBinding(TextBox.TextProperty, massBinding);

                    RowDefinition miniGrid1_RowDefinition = new RowDefinition();
                    miniGrid1_RowDefinition.Height = new System.Windows.GridLength(34);
                    rangesPage_MiniGrid.RowDefinitions.Add(miniGrid1_RowDefinition);

                    Grid.SetRow(possibleUserUnitMass_TextBoxes[x], row);
                    Grid.SetColumn(possibleUserUnitMass_TextBoxes[x], col);
                    Grid.SetRow(massLabel, row);
                    Grid.SetColumn(massLabel, col);

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
                        rangesPage_MiniGrid.Children.Add(possibleUserUnitMass_TextBoxes[x]);
                        rangesPage_MiniGrid.Children.Add(massLabel);
                    }
                    // Catch invalid operation if child is already a child
                    catch (InvalidOperationException e)
                    {
                        rangesPage_MiniGrid.Children.Remove(possibleUserUnitMass_TextBoxes[x]);
                        rangesPage_MiniGrid.Children.Add(possibleUserUnitMass_TextBoxes[x]);
                        rangesPage_MiniGrid.Children.Add(massLabel);
                    }

                    x++;
                }
                #endregion
            }

            //try to add children
            try
            {
                //Add MiniGrid to ListBox
                rangesPage_ListBox.Items.Add(rangesPage_MiniGrid);
            }
            // Catch invalid operation if child is already a child
            catch (InvalidOperationException e)
            {
                rangesPage_ListBox.Items.Remove(rangesPage_MiniGrid);
                rangesPage_ListBox.Items.Add(rangesPage_MiniGrid);
            }
        }

        private void RangesPage_ListControls()//20 per column
        {
            //Lists to Hold Possibles to Minimize Lines of Code
            List<Boolean> checkIf_CheckBoxesChecked = new List<Boolean>();
            List<Button> possible_Buttons = new List<Button>();
            List<String> possible_ButtonContent = new List<String>();
            List<TextBox> possibleMin_TextBoxes = new List<TextBox>();
            List<TextBox> possibleMax_TextBoxes = new List<TextBox>();
            List<String> possibleMin_Variables = new List<String>();
            List<String> possibleMax_Variables = new List<String>();
            List<String> possible_ButtonNames = new List<String>();
            List<String> possibleMin_TextBoxNames = new List<String>();
            List<String> possibleMax_TextBoxNames = new List<String>();


            int columnCheck = 0;
            int numColumns = 1;
            int extraRowsCheck = 0;


            #region checkIf_CheckBoxesChecked List
            //checkIf_CheckBoxesChecked List

            if (App.initializingFlagsModel.ParameterRangesSave_InitializeFlag)
            {
                //Sugars
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedHexose_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedHexNAc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedDxyHex_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedPentose_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedNeuAc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedNeuGc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedKDN_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedHexA_Bool);

                //User Units
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit1_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit2_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit3_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit4_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit5_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit6_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit7_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit8_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit9_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedUserUnit10_Bool);

                //Special                
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedNaH_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedCH3_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedSO3_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedOAcetyl_Bool);

                //Amino Acids               
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedAla_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedArg_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedAsn_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedAsp_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedCys_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedGln_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedGlu_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedGly_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedHis_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedIle_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedLeu_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedLys_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedMet_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedPhe_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedPro_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedSer_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedThr_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedTrp_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedTyr_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedVal_Bool);

                //Permethyl                  
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedpHex_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedpHxNAc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedpDxHex_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedpPntos_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedpNuAc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedpNuGc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedpKDN_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderModel_Save.CheckedpHxA_Bool);
            }
            else if (App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag)
            {
                //Sugars
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedHexose_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedHexNAc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedDxyHex_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedPentose_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedNeuAc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedNeuGc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedKDN_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedHexA_Bool);

                //User Units
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit1_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit2_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit3_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit4_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit5_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit6_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit7_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit8_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit9_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedUserUnit10_Bool);

                //Special                
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedNaH_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedCH3_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedSO3_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedOAcetyl_Bool);

                //Amino Acids               
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedAla_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedArg_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedAsn_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedAsp_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedCys_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedGln_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedGlu_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedGly_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedHis_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedIle_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedLeu_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedLys_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedMet_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedPhe_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedSer_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedThr_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedTrp_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedTyr_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedVal_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedPro_Bool);

                //Permethyl                  
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedpHex_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedpHxNAc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedpDxHex_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedpPntos_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedpNuAc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedpNuGc_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedpKDN_Bool);
                checkIf_CheckBoxesChecked.Add(App.omniFinderGMModel_Save.CheckedpHxA_Bool);
            }
            #endregion

            #region possible_Buttons List
            //possible_Buttons List
            //Sugars(8), User Units(10), Special(4), Amino Acids(20), Permethyl(8)
            for(int i = 0; i <= 50; i++)
                possible_Buttons.Add(new Button());
            #endregion

            #region possible_ButtonContent List
            //possible_ButtonContent List
            //Sugars
            possible_ButtonContent.Add("Hexose");
            possible_ButtonContent.Add("HexNAc");
            possible_ButtonContent.Add("DxyHex");
            possible_ButtonContent.Add("Pentose");
            possible_ButtonContent.Add("NeuAc");
            possible_ButtonContent.Add("NeuGc");
            possible_ButtonContent.Add("KDn");
            possible_ButtonContent.Add("HexA");

            //User Units
            for (int i = 1; i <= 10; i++)
                possible_ButtonContent.Add("User " + i);

            //Special               
            possible_ButtonContent.Add("NaH");
            possible_ButtonContent.Add("CH3");
            possible_ButtonContent.Add("SO3");
            possible_ButtonContent.Add("O-Acetyl");

            //Amino Acids           
            possible_ButtonContent.Add("Ala");
            possible_ButtonContent.Add("Arg");
            possible_ButtonContent.Add("Asn");
            possible_ButtonContent.Add("Asp");
            possible_ButtonContent.Add("Cys");
            possible_ButtonContent.Add("Gln");
            possible_ButtonContent.Add("Glu");
            possible_ButtonContent.Add("Gly");
            possible_ButtonContent.Add("His");
            possible_ButtonContent.Add("Ile");
            possible_ButtonContent.Add("Leu");
            possible_ButtonContent.Add("Lys");
            possible_ButtonContent.Add("Met");
            possible_ButtonContent.Add("Phe");
            possible_ButtonContent.Add("Pro");
            possible_ButtonContent.Add("Ser");
            possible_ButtonContent.Add("Thr");
            possible_ButtonContent.Add("Trp");
            possible_ButtonContent.Add("Tyr");
            possible_ButtonContent.Add("Val");

            //Permethyl             
            possible_ButtonContent.Add("pHex");
            possible_ButtonContent.Add("pHxNAc");
            possible_ButtonContent.Add("pDxHex");
            possible_ButtonContent.Add("pPntos");
            possible_ButtonContent.Add("pNuAc");
            possible_ButtonContent.Add("pNuGc");
            possible_ButtonContent.Add("pKDn");
            possible_ButtonContent.Add("pHxA");
            #endregion

            #region possibleMin_TextBoxes List
            //possibleMin_TextBoxes List
            //Sugars
            possibleMin_TextBoxes.Add(hexoseMin_TextBox);
            possibleMin_TextBoxes.Add(hexNAcMin_TextBox);
            possibleMin_TextBoxes.Add(dxyHexMin_TextBox);
            possibleMin_TextBoxes.Add(pentoseMin_TextBox);
            possibleMin_TextBoxes.Add(neuAcMin_TextBox);
            possibleMin_TextBoxes.Add(neuGcMin_TextBox);
            possibleMin_TextBoxes.Add(kDNMin_TextBox);
            possibleMin_TextBoxes.Add(hexAMin_TextBox);

            //User Units
            possibleMin_TextBoxes.Add(userUnit1Min_TextBox);
            possibleMin_TextBoxes.Add(userUnit2Min_TextBox);
            possibleMin_TextBoxes.Add(userUnit3Min_TextBox);
            possibleMin_TextBoxes.Add(userUnit4Min_TextBox);
            possibleMin_TextBoxes.Add(userUnit5Min_TextBox);
            possibleMin_TextBoxes.Add(userUnit6Min_TextBox);
            possibleMin_TextBoxes.Add(userUnit7Min_TextBox);
            possibleMin_TextBoxes.Add(userUnit8Min_TextBox);
            possibleMin_TextBoxes.Add(userUnit9Min_TextBox);
            possibleMin_TextBoxes.Add(userUnit10Min_TextBox);

            //Special               
            possibleMin_TextBoxes.Add(naHMin_TextBox);
            possibleMin_TextBoxes.Add(cH3Min_TextBox);
            possibleMin_TextBoxes.Add(sO3Min_TextBox);
            possibleMin_TextBoxes.Add(oAcetylMin_TextBox);

            //Amino Acids           
            possibleMin_TextBoxes.Add(alaMin_TextBox);
            possibleMin_TextBoxes.Add(argMin_TextBox);
            possibleMin_TextBoxes.Add(asnMin_TextBox);
            possibleMin_TextBoxes.Add(aspMin_TextBox);
            possibleMin_TextBoxes.Add(cysMin_TextBox);
            possibleMin_TextBoxes.Add(glnMin_TextBox);
            possibleMin_TextBoxes.Add(gluMin_TextBox);
            possibleMin_TextBoxes.Add(glyMin_TextBox);
            possibleMin_TextBoxes.Add(hisMin_TextBox);
            possibleMin_TextBoxes.Add(ileMin_TextBox);
            possibleMin_TextBoxes.Add(leuMin_TextBox);
            possibleMin_TextBoxes.Add(lysMin_TextBox);
            possibleMin_TextBoxes.Add(metMin_TextBox);
            possibleMin_TextBoxes.Add(pheMin_TextBox);
            possibleMin_TextBoxes.Add(proMin_TextBox);
            possibleMin_TextBoxes.Add(serMin_TextBox);
            possibleMin_TextBoxes.Add(thrMin_TextBox);
            possibleMin_TextBoxes.Add(trpMin_TextBox);
            possibleMin_TextBoxes.Add(tyrMin_TextBox);
            possibleMin_TextBoxes.Add(valMin_TextBox);

            //Permethyl             
            possibleMin_TextBoxes.Add(pHexMin_TextBox);
            possibleMin_TextBoxes.Add(pHxNAcMin_TextBox);
            possibleMin_TextBoxes.Add(pDxHexMin_TextBox);
            possibleMin_TextBoxes.Add(pPntosMin_TextBox);
            possibleMin_TextBoxes.Add(pNuAcMin_TextBox);
            possibleMin_TextBoxes.Add(pNuGcMin_TextBox);
            possibleMin_TextBoxes.Add(pKDNMin_TextBox);
            possibleMin_TextBoxes.Add(pHxAMin_TextBox);
            #endregion

            #region possibleMax_TextBoxes List
            //possibleMax_TextBoxes List
            //Sugars
            possibleMax_TextBoxes.Add(hexoseMax_TextBox);
            possibleMax_TextBoxes.Add(hexNAcMax_TextBox);
            possibleMax_TextBoxes.Add(dxyHexMax_TextBox);
            possibleMax_TextBoxes.Add(pentoseMax_TextBox);
            possibleMax_TextBoxes.Add(neuAcMax_TextBox);
            possibleMax_TextBoxes.Add(neuGcMax_TextBox);
            possibleMax_TextBoxes.Add(kDNMax_TextBox);
            possibleMax_TextBoxes.Add(hexAMax_TextBox);

            //User Units
            possibleMax_TextBoxes.Add(userUnit1Max_TextBox);
            possibleMax_TextBoxes.Add(userUnit2Max_TextBox);
            possibleMax_TextBoxes.Add(userUnit3Max_TextBox);
            possibleMax_TextBoxes.Add(userUnit4Max_TextBox);
            possibleMax_TextBoxes.Add(userUnit5Max_TextBox);
            possibleMax_TextBoxes.Add(userUnit6Max_TextBox);
            possibleMax_TextBoxes.Add(userUnit7Max_TextBox);
            possibleMax_TextBoxes.Add(userUnit8Max_TextBox);
            possibleMax_TextBoxes.Add(userUnit9Max_TextBox);
            possibleMax_TextBoxes.Add(userUnit10Max_TextBox);

            //Special               
            possibleMax_TextBoxes.Add(naHMax_TextBox);
            possibleMax_TextBoxes.Add(cH3Max_TextBox);
            possibleMax_TextBoxes.Add(sO3Max_TextBox);
            possibleMax_TextBoxes.Add(oAcetylMax_TextBox);

            //Amino Acids           
            possibleMax_TextBoxes.Add(alaMax_TextBox);
            possibleMax_TextBoxes.Add(argMax_TextBox);
            possibleMax_TextBoxes.Add(asnMax_TextBox);
            possibleMax_TextBoxes.Add(aspMax_TextBox);
            possibleMax_TextBoxes.Add(cysMax_TextBox);
            possibleMax_TextBoxes.Add(glnMax_TextBox);
            possibleMax_TextBoxes.Add(gluMax_TextBox);
            possibleMax_TextBoxes.Add(glyMax_TextBox);
            possibleMax_TextBoxes.Add(hisMax_TextBox);
            possibleMax_TextBoxes.Add(ileMax_TextBox);
            possibleMax_TextBoxes.Add(leuMax_TextBox);
            possibleMax_TextBoxes.Add(lysMax_TextBox);
            possibleMax_TextBoxes.Add(metMax_TextBox);
            possibleMax_TextBoxes.Add(pheMax_TextBox);
            possibleMax_TextBoxes.Add(proMax_TextBox);
            possibleMax_TextBoxes.Add(serMax_TextBox);
            possibleMax_TextBoxes.Add(thrMax_TextBox);
            possibleMax_TextBoxes.Add(trpMax_TextBox);
            possibleMax_TextBoxes.Add(tyrMax_TextBox);
            possibleMax_TextBoxes.Add(valMax_TextBox);

            //Permethyl             
            possibleMax_TextBoxes.Add(pHexMax_TextBox);
            possibleMax_TextBoxes.Add(pHxNAcMax_TextBox);
            possibleMax_TextBoxes.Add(pDxHexMax_TextBox);
            possibleMax_TextBoxes.Add(pPntosMax_TextBox);
            possibleMax_TextBoxes.Add(pNuAcMax_TextBox);
            possibleMax_TextBoxes.Add(pNuGcMax_TextBox);
            possibleMax_TextBoxes.Add(pKDNMax_TextBox);
            possibleMax_TextBoxes.Add(pHxAMax_TextBox);
            #endregion

            #region possibleMin_Variables List
            //possibleMin_Variables List
            //Sugars
            possibleMin_Variables.Add("RangesModel.MinHexose_Int");
            possibleMin_Variables.Add("RangesModel.MinHexNAc_Int");
            possibleMin_Variables.Add("RangesModel.MinDxyHex_Int");
            possibleMin_Variables.Add("RangesModel.MinPentose_Int");
            possibleMin_Variables.Add("RangesModel.MinNeuAc_Int");
            possibleMin_Variables.Add("RangesModel.MinNeuGc_Int");
            possibleMin_Variables.Add("RangesModel.MinKDN_Int");
            possibleMin_Variables.Add("RangesModel.MinHexA_Int");
                                     
            //User Units             
            possibleMin_Variables.Add("RangesModel.MinUserUnit1_Int");
            possibleMin_Variables.Add("RangesModel.MinUserUnit2_Int");
            possibleMin_Variables.Add("RangesModel.MinUserUnit3_Int");
            possibleMin_Variables.Add("RangesModel.MinUserUnit4_Int");
            possibleMin_Variables.Add("RangesModel.MinUserUnit5_Int");
            possibleMin_Variables.Add("RangesModel.MinUserUnit6_Int");
            possibleMin_Variables.Add("RangesModel.MinUserUnit7_Int");
            possibleMin_Variables.Add("RangesModel.MinUserUnit8_Int");
            possibleMin_Variables.Add("RangesModel.MinUserUnit9_Int");
            possibleMin_Variables.Add("RangesModel.MinUserUnit10_Int");
                                    
            //Special               
            possibleMin_Variables.Add("RangesModel.MinNaH_Int");
            possibleMin_Variables.Add("RangesModel.MinCH3_Int");
            possibleMin_Variables.Add("RangesModel.MinSO3_Int");
            possibleMin_Variables.Add("RangesModel.MinOAcetyl_Int");
                                     
            //Amino Acids            
            possibleMin_Variables.Add("RangesModel.MinAla_Int");
            possibleMin_Variables.Add("RangesModel.MinArg_Int");
            possibleMin_Variables.Add("RangesModel.MinAsn_Int");
            possibleMin_Variables.Add("RangesModel.MinAsp_Int");
            possibleMin_Variables.Add("RangesModel.MinCys_Int");
            possibleMin_Variables.Add("RangesModel.MinGln_Int");
            possibleMin_Variables.Add("RangesModel.MinGlu_Int");
            possibleMin_Variables.Add("RangesModel.MinGly_Int");
            possibleMin_Variables.Add("RangesModel.MinHis_Int");
            possibleMin_Variables.Add("RangesModel.MinIle_Int");
            possibleMin_Variables.Add("RangesModel.MinLeu_Int");
            possibleMin_Variables.Add("RangesModel.MinLys_Int");
            possibleMin_Variables.Add("RangesModel.MinMet_Int");
            possibleMin_Variables.Add("RangesModel.MinPhe_Int");
            possibleMin_Variables.Add("RangesModel.MinPro_Int");
            possibleMin_Variables.Add("RangesModel.MinSer_Int");
            possibleMin_Variables.Add("RangesModel.MinThr_Int");
            possibleMin_Variables.Add("RangesModel.MinTrp_Int");
            possibleMin_Variables.Add("RangesModel.MinTyr_Int");
            possibleMin_Variables.Add("RangesModel.MinVal_Int");
                                     
            //Permethyl              
            possibleMin_Variables.Add("RangesModel.MinpHex_Int");
            possibleMin_Variables.Add("RangesModel.MinpHxNAc_Int");
            possibleMin_Variables.Add("RangesModel.MinpDxHex_Int");
            possibleMin_Variables.Add("RangesModel.MinpPntos_Int");
            possibleMin_Variables.Add("RangesModel.MinpNuAc_Int");
            possibleMin_Variables.Add("RangesModel.MinpNuGc_Int");
            possibleMin_Variables.Add("RangesModel.MinpKDN_Int");
            possibleMin_Variables.Add("RangesModel.MinpHxA_Int");
            #endregion

            #region possibleMax_Variables List
            //possibleMax_Variables List
            //Sugars
            possibleMax_Variables.Add("RangesModel.MaxHexose_Int");
            possibleMax_Variables.Add("RangesModel.MaxHexNAc_Int");
            possibleMax_Variables.Add("RangesModel.MaxDxyHex_Int");
            possibleMax_Variables.Add("RangesModel.MaxPentose_Int");
            possibleMax_Variables.Add("RangesModel.MaxNeuAc_Int");
            possibleMax_Variables.Add("RangesModel.MaxNeuGc_Int");
            possibleMax_Variables.Add("RangesModel.MaxKDN_Int");
            possibleMax_Variables.Add("RangesModel.MaxHexA_Int");
                                     
            //User Units             
            possibleMax_Variables.Add("RangesModel.MaxUserUnit1_Int");
            possibleMax_Variables.Add("RangesModel.MaxUserUnit2_Int");
            possibleMax_Variables.Add("RangesModel.MaxUserUnit3_Int");
            possibleMax_Variables.Add("RangesModel.MaxUserUnit4_Int");
            possibleMax_Variables.Add("RangesModel.MaxUserUnit5_Int");
            possibleMax_Variables.Add("RangesModel.MaxUserUnit6_Int");
            possibleMax_Variables.Add("RangesModel.MaxUserUnit7_Int");
            possibleMax_Variables.Add("RangesModel.MaxUserUnit8_Int");
            possibleMax_Variables.Add("RangesModel.MaxUserUnit9_Int");
            possibleMax_Variables.Add("RangesModel.MaxUserUnit10_Int");
                                     
            //Special                
            possibleMax_Variables.Add("RangesModel.MaxNaH_Int");
            possibleMax_Variables.Add("RangesModel.MaxCH3_Int");
            possibleMax_Variables.Add("RangesModel.MaxSO3_Int");
            possibleMax_Variables.Add("RangesModel.MaxOAcetyl_Int");
                                    
            //Amino Acids           
            possibleMax_Variables.Add("RangesModel.MaxAla_Int");
            possibleMax_Variables.Add("RangesModel.MaxArg_Int");
            possibleMax_Variables.Add("RangesModel.MaxAsn_Int");
            possibleMax_Variables.Add("RangesModel.MaxAsp_Int");
            possibleMax_Variables.Add("RangesModel.MaxCys_Int");
            possibleMax_Variables.Add("RangesModel.MaxGln_Int");
            possibleMax_Variables.Add("RangesModel.MaxGlu_Int");
            possibleMax_Variables.Add("RangesModel.MaxGly_Int");
            possibleMax_Variables.Add("RangesModel.MaxHis_Int");
            possibleMax_Variables.Add("RangesModel.MaxIle_Int");
            possibleMax_Variables.Add("RangesModel.MaxLeu_Int");
            possibleMax_Variables.Add("RangesModel.MaxLys_Int");
            possibleMax_Variables.Add("RangesModel.MaxMet_Int");
            possibleMax_Variables.Add("RangesModel.MaxPhe_Int");
            possibleMax_Variables.Add("RangesModel.MaxPro_Int");
            possibleMax_Variables.Add("RangesModel.MaxSer_Int");
            possibleMax_Variables.Add("RangesModel.MaxThr_Int");
            possibleMax_Variables.Add("RangesModel.MaxTrp_Int");
            possibleMax_Variables.Add("RangesModel.MaxTyr_Int");
            possibleMax_Variables.Add("RangesModel.MaxVal_Int");
                                    
            //Permethyl             
            possibleMax_Variables.Add("RangesModel.MaxpHex_Int");
            possibleMax_Variables.Add("RangesModel.MaxpHxNAc_Int");
            possibleMax_Variables.Add("RangesModel.MaxpDxHex_Int");
            possibleMax_Variables.Add("RangesModel.MaxpPntos_Int");
            possibleMax_Variables.Add("RangesModel.MaxpNuAc_Int");
            possibleMax_Variables.Add("RangesModel.MaxpNuGc_Int");
            possibleMax_Variables.Add("RangesModel.MaxpKDN_Int");
            possibleMax_Variables.Add("RangesModel.MaxpHxA_Int");
            #endregion

            #region possible_ButtonNames List
            //possible_ButtonNames List
            //Sugars
            possible_ButtonNames.Add("hexose_Button");
            possible_ButtonNames.Add("hexNAc_Button");
            possible_ButtonNames.Add("dxyHex_Button");
            possible_ButtonNames.Add("pentose_Button");
            possible_ButtonNames.Add("neuAc_Button");
            possible_ButtonNames.Add("neuGc_Button");
            possible_ButtonNames.Add("kDN_Button");
            possible_ButtonNames.Add("hexA_Button");

            //User Units
            for (int i = 1; i <= 10; i++)
                possible_ButtonNames.Add("userUnit_Button" + i);

            //Special  
            possible_ButtonNames.Add("naH_Button");
            possible_ButtonNames.Add("cH3_Button");
            possible_ButtonNames.Add("sO3_Button");
            possible_ButtonNames.Add("oAcetyl_Button");

            //Amino Acids
            possible_ButtonNames.Add("ala_Button");
            possible_ButtonNames.Add("arg_Button");
            possible_ButtonNames.Add("asn_Button");
            possible_ButtonNames.Add("asp_Button");
            possible_ButtonNames.Add("cys_Button");
            possible_ButtonNames.Add("gln_Button");
            possible_ButtonNames.Add("glu_Button");
            possible_ButtonNames.Add("gly_Button");
            possible_ButtonNames.Add("his_Button");
            possible_ButtonNames.Add("ile_Button");
            possible_ButtonNames.Add("leu_Button");
            possible_ButtonNames.Add("lys_Button");
            possible_ButtonNames.Add("met_Button");
            possible_ButtonNames.Add("phe_Button");
            possible_ButtonNames.Add("pro_Button");
            possible_ButtonNames.Add("ser_Button");
            possible_ButtonNames.Add("thr_Button");
            possible_ButtonNames.Add("trp_Button");
            possible_ButtonNames.Add("tyr_Button");
            possible_ButtonNames.Add("val_Button");

            //Permethyl 
            possible_ButtonNames.Add("pHex_Button");
            possible_ButtonNames.Add("pHxNAc_Button");
            possible_ButtonNames.Add("pDxHex_Button");
            possible_ButtonNames.Add("pPntos_Button");
            possible_ButtonNames.Add("pNuAc_Button");
            possible_ButtonNames.Add("pNuGc_Button");
            possible_ButtonNames.Add("pKDN_Button");
            possible_ButtonNames.Add("pHxA_Button");
            #endregion

            #region possibleMin_TextBoxNames List
            //possibleMin_TextBoxNames List
            //Sugars
            possibleMin_TextBoxNames.Add("hexoseMin_TextBox");
            possibleMin_TextBoxNames.Add("hexNAcMin_TextBox");
            possibleMin_TextBoxNames.Add("dxyHexMin_TextBox");
            possibleMin_TextBoxNames.Add("pentoseMin_TextBox");
            possibleMin_TextBoxNames.Add("neuAcMin_TextBox");
            possibleMin_TextBoxNames.Add("neuGcMin_TextBox");
            possibleMin_TextBoxNames.Add("kDNMin_TextBox");
            possibleMin_TextBoxNames.Add("hexAMin_TextBox");

            //User Units
            for (int i = 1; i <= 10; i++)
                possibleMin_TextBoxNames.Add("userUnit" + i + "Min_TextBox");

            //Special  
            possibleMin_TextBoxNames.Add("naHMin_TextBox");
            possibleMin_TextBoxNames.Add("cH3Min_TextBox");
            possibleMin_TextBoxNames.Add("sO3Min_TextBox");
            possibleMin_TextBoxNames.Add("oAcetylMin_TextBox");

            //Amino Acids
            possibleMin_TextBoxNames.Add("alaMin_TextBox");
            possibleMin_TextBoxNames.Add("argMin_TextBox");
            possibleMin_TextBoxNames.Add("asnMin_TextBox");
            possibleMin_TextBoxNames.Add("aspMin_TextBox");
            possibleMin_TextBoxNames.Add("cysMin_TextBox");
            possibleMin_TextBoxNames.Add("glnMin_TextBox");
            possibleMin_TextBoxNames.Add("gluMin_TextBox");
            possibleMin_TextBoxNames.Add("glyMin_TextBox");
            possibleMin_TextBoxNames.Add("hisMin_TextBox");
            possibleMin_TextBoxNames.Add("ileMin_TextBox");
            possibleMin_TextBoxNames.Add("leuMin_TextBox");
            possibleMin_TextBoxNames.Add("lysMin_TextBox");
            possibleMin_TextBoxNames.Add("metMin_TextBox");
            possibleMin_TextBoxNames.Add("pheMin_TextBox");
            possibleMin_TextBoxNames.Add("proMin_TextBox");
            possibleMin_TextBoxNames.Add("serMin_TextBox");
            possibleMin_TextBoxNames.Add("thrMin_TextBox");
            possibleMin_TextBoxNames.Add("trpMin_TextBox");
            possibleMin_TextBoxNames.Add("tyrMin_TextBox");
            possibleMin_TextBoxNames.Add("valMin_TextBox");

            //Permethyl 
            possibleMin_TextBoxNames.Add("pHexMin_TextBox");
            possibleMin_TextBoxNames.Add("pHxNAcMin_TextBox");
            possibleMin_TextBoxNames.Add("pDxHexMin_TextBox");
            possibleMin_TextBoxNames.Add("pPntosMin_TextBox");
            possibleMin_TextBoxNames.Add("pNuAcMin_TextBox");
            possibleMin_TextBoxNames.Add("pNuGcMin_TextBox");
            possibleMin_TextBoxNames.Add("pKDNMin_TextBox");
            possibleMin_TextBoxNames.Add("pHxAMin_TextBox");
            #endregion

            #region possibleMax_TextBoxNames List
            //possibleMax_TextBoxNames List
            //Sugars
            possibleMax_TextBoxNames.Add("hexoseMax_TextBox");
            possibleMax_TextBoxNames.Add("hexNAcMax_TextBox");
            possibleMax_TextBoxNames.Add("dxyHexMax_TextBox");
            possibleMax_TextBoxNames.Add("pentoseMax_TextBox");
            possibleMax_TextBoxNames.Add("neuAcMax_TextBox");
            possibleMax_TextBoxNames.Add("neuGcMax_TextBox");
            possibleMax_TextBoxNames.Add("kDNMax_TextBox");
            possibleMax_TextBoxNames.Add("hexAMax_TextBox");

            //User Units
            for (int i = 1; i <= 10; i++)
                possibleMax_TextBoxNames.Add("userUnit" + i + "Max_TextBox");

            //Special  
            possibleMax_TextBoxNames.Add("naHMax_TextBox");
            possibleMax_TextBoxNames.Add("cH3Max_TextBox");
            possibleMax_TextBoxNames.Add("sO3Max_TextBox");
            possibleMax_TextBoxNames.Add("oAcetylMax_TextBox");

            //Amino Acids
            possibleMax_TextBoxNames.Add("alaMax_TextBox");
            possibleMax_TextBoxNames.Add("argMax_TextBox");
            possibleMax_TextBoxNames.Add("asnMax_TextBox");
            possibleMax_TextBoxNames.Add("aspMax_TextBox");
            possibleMax_TextBoxNames.Add("cysMax_TextBox");
            possibleMax_TextBoxNames.Add("glnMax_TextBox");
            possibleMax_TextBoxNames.Add("gluMax_TextBox");
            possibleMax_TextBoxNames.Add("glyMax_TextBox");
            possibleMax_TextBoxNames.Add("hisMax_TextBox");
            possibleMax_TextBoxNames.Add("ileMax_TextBox");
            possibleMax_TextBoxNames.Add("leuMax_TextBox");
            possibleMax_TextBoxNames.Add("lysMax_TextBox");
            possibleMax_TextBoxNames.Add("metMax_TextBox");
            possibleMax_TextBoxNames.Add("pheMax_TextBox");
            possibleMax_TextBoxNames.Add("proMax_TextBox");
            possibleMax_TextBoxNames.Add("serMax_TextBox");
            possibleMax_TextBoxNames.Add("thrMax_TextBox");
            possibleMax_TextBoxNames.Add("trpMax_TextBox");
            possibleMax_TextBoxNames.Add("tyrMax_TextBox");
            possibleMax_TextBoxNames.Add("valMax_TextBox");

            //Permethyl 
            possibleMax_TextBoxNames.Add("pHexMax_TextBox");
            possibleMax_TextBoxNames.Add("pHxNAcMax_TextBox");
            possibleMax_TextBoxNames.Add("pDxHexMax_TextBox");
            possibleMax_TextBoxNames.Add("pPntosMax_TextBox");
            possibleMax_TextBoxNames.Add("pNuAcMax_TextBox");
            possibleMax_TextBoxNames.Add("pNuGcMax_TextBox");
            possibleMax_TextBoxNames.Add("pKDNMax_TextBox");
            possibleMax_TextBoxNames.Add("pHxAMax_TextBox");
            #endregion


            for (int i = 0; i <= (checkIf_CheckBoxesChecked.Count - 1); i++)
                if (checkIf_CheckBoxesChecked[i] == true)
                {
                    if (columnCheck >= 10 && columnCheck < 20 && numColumns == 1)
                    {
                        extraRowsCheck++;
                        AddRow(extraRowsCheck);
                    }

                    if (columnCheck == 20)
                    {
                        numColumns++;
                        AddColumn(numColumns);
                        columnCheck = 0;
                        extraRowsCheck = 0;
                    }

                    possible_Buttons[i].Content = possible_ButtonContent[i];
                    possible_Buttons[i].Name = possible_ButtonNames[i];
                    possible_Buttons[i].Style = (Style)Application.Current.Resources["listBoxButtonStyle"];
                    rangesButtons.Add(possible_Buttons[i]);

                    possible_Buttons[i].Command = rangesVM.ButtonCommand;
                    possible_Buttons[i].CommandParameter = possible_Buttons[i].Name;

                    possibleMin_TextBoxes[i].Name = possibleMin_TextBoxNames[i];
                    possibleMin_TextBoxes[i].Style = (Style)Application.Current.Resources["minTextBoxStyle"];
                    rangesMinTextBoxes.Add(possibleMin_TextBoxes[i]);

                    possibleMin_TextBoxes[i].TextChanged += this.hexoseMin_TextBox_TextChanged;
 
                    Binding minBinding = new Binding();
                    minBinding.Source = rangesVM; 
                    minBinding.Path = new PropertyPath(possibleMin_Variables[i]); 
                    minBinding.Mode = BindingMode.TwoWay;
                    possibleMin_TextBoxes[i].SetBinding(TextBox.TextProperty, minBinding);

                    possibleMax_TextBoxes[i].Name = possibleMax_TextBoxNames[i];
                    possibleMax_TextBoxes[i].Style = (Style)Application.Current.Resources["maxTextBoxStyle"];
                    rangesMaxTextBoxes.Add(possibleMax_TextBoxes[i]);

                    possibleMax_TextBoxes[i].TextChanged += this.hexoseMin_TextBox_TextChanged;

                    Binding maxBinding = new Binding();
                    maxBinding.Source = rangesVM;
                    maxBinding.Path = new PropertyPath(possibleMax_Variables[i]);
                    maxBinding.Mode = BindingMode.TwoWay;
                    possibleMax_TextBoxes[i].SetBinding(TextBox.TextProperty, maxBinding);

                    if (i >= 8 && i <= 17)
                    {
                        columnCheck++;

                        if (columnCheck >= 10 && columnCheck < 20 && numColumns == 1)
                        {
                                extraRowsCheck++;
                                AddRow(extraRowsCheck);
                        }

                        if (columnCheck == 20)
                        {
                            numColumns++;
                            AddColumn(numColumns);
                            columnCheck = 0;
                            extraRowsCheck = 0;
                        }
                    }

                    columnCheck++;
                }
        }

        protected void AddRow(int extraRowsCheck)
        {
            rangesPage_ListBox.Height = 340 + (34 * extraRowsCheck);
            rangesPage_MiniGrid.Height = 340 + (34 * extraRowsCheck);
            rangesPage_Grid.Height = 400 + (34 * extraRowsCheck);

            if (App.initializingFlagsModel.ParameterRangesSave_InitializeFlag)
            {
                if (App.initializingFlagsModel.Tab4_ResizeFlag)
                    App.parameterPage.Height = 432 + (34 * extraRowsCheck);
            }
            else if (App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag)
            {
                App.omniFinderGMRangesPage.Height = 400 + (34 * extraRowsCheck);
            }
        }

        protected void AddColumn(int numColumns)
        {
            rangesPage_ListBox.Width = 380 * numColumns;
            rangesPage_MiniGrid.Width = 380 * numColumns;
            rangesPage_Grid.Width = 400 * numColumns;

            if (App.initializingFlagsModel.ParameterRangesSave_InitializeFlag)
            {
                if (App.initializingFlagsModel.Tab4_ResizeFlag)
                    App.parameterPage.Width = 400 * numColumns;
            }
            else if (App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag)
            {
                App.omniFinderGMRangesPage.Width = 400 * numColumns;
            }

            ColumnDefinition miniGrid_ColumnDefinition = new ColumnDefinition();
            miniGrid_ColumnDefinition.Width = new System.Windows.GridLength(380);
            rangesPage_MiniGrid.ColumnDefinitions.Add(miniGrid_ColumnDefinition);
        }

        protected void hexoseMin_TextBox_TextChanged(object sender, EventArgs e)
        {
            int tempInt = 0;
            TextBox changedTextBox = (TextBox)sender;
            string Str = changedTextBox.Text.Trim();
            int Num;
            bool isNum = int.TryParse(Str, out Num);
            if (isNum)
                tempInt = Convert.ToInt32(((TextBox)sender).Text);
            else if (changedTextBox.Text == "")
                tempInt = 0;
            else
                MessageBox.Show("Invalid number");

            if (App.initializingFlagsModel.ParameterRangesSave_InitializeFlag)
            {
                switch (changedTextBox.Name)
                {
                    #region Min
                    //Min
                    //Sugars
                    case "hexoseMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinHexose_Int = tempInt;
                            break;
                        }
                    case "hexNAcMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinHexNAc_Int = tempInt;
                            break;
                        }
                    case "dxyHexMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinDxyHex_Int = tempInt;
                            break;
                        }
                    case "pentoseMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinPentose_Int = tempInt;
                            break;
                        }
                    case "neuAcMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinNeuAc_Int = tempInt;
                            break;
                        }
                    case "neuGcMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinNeuGc_Int = tempInt;
                            break;
                        }
                    case "kDNMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinKDN_Int = tempInt;
                            break;
                        }
                    case "hexAMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinHexA_Int = tempInt;
                            break;
                        }

                    //Special
                    case "naHMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinNaH_Int = tempInt;
                            break;
                        }
                    case "cH3Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinCH3_Int = tempInt;
                            break;
                        }
                    case "sO3Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinSO3_Int = tempInt;
                            break;
                        }
                    case "oAcetylMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinOAcetyl_Int = tempInt;
                            break;
                        }

                    //User
                    case "userUnit1Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit1_Int = tempInt;
                            break;
                        }
                    case "userUnit2Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit2_Int = tempInt;
                            break;
                        }
                    case "userUnit3Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit3_Int = tempInt;
                            break;
                        }
                    case "userUnit4Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit4_Int = tempInt;
                            break;
                        }
                    case "userUnit5Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit5_Int = tempInt;
                            break;
                        }
                    case "userUnit6Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit6_Int = tempInt;
                            break;
                        }
                    case "userUnit7Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit7_Int = tempInt;
                            break;
                        }
                    case "userUnit8Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit8_Int = tempInt;
                            break;
                        }
                    case "userUnit9Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit9_Int = tempInt;
                            break;
                        }
                    case "userUnit10Min_TextBox":
                        {
                            App.parameterRangesModel_Save.MinUserUnit10_Int = tempInt;
                            break;
                        }

                    //Amino Acids
                    case "alaMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinAla_Int = tempInt;
                            break;
                        }
                    case "argMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinArg_Int = tempInt;
                            break;
                        }
                    case "asnMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinAsn_Int = tempInt;
                            break;
                        }
                    case "aspMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinAsp_Int = tempInt;
                            break;
                        }
                    case "cysMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinCys_Int = tempInt;
                            break;
                        }
                    case "glnMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinGln_Int = tempInt;
                            break;
                        }
                    case "gluMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinGlu_Int = tempInt;
                            break;
                        }
                    case "glyMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinGly_Int = tempInt;
                            break;
                        }
                    case "hisMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinHis_Int = tempInt;
                            break;
                        }
                    case "ileMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinIle_Int = tempInt;
                            break;
                        }
                    case "leuMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinLeu_Int = tempInt;
                            break;
                        }
                    case "lysMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinLys_Int = tempInt;
                            break;
                        }
                    case "metMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinMet_Int = tempInt;
                            break;
                        }
                    case "pheMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinPhe_Int = tempInt;
                            break;
                        }
                    case "proMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinPro_Int = tempInt;
                            break;
                        }
                    case "serMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinSer_Int = tempInt;
                            break;
                        }
                    case "thrMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinThr_Int = tempInt;
                            break;
                        }
                    case "trpMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinTrp_Int = tempInt;
                            break;
                        }
                    case "tyrMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinTyr_Int = tempInt;
                            break;
                        }
                    case "valMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinVal_Int = tempInt;
                            break;
                        }

                    //Permethyl
                    case "pHexMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinpHex_Int = tempInt;
                            break;
                        }
                    case "pHxNAcMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinpHxNAc_Int = tempInt;
                            break;
                        }
                    case "pDxHexMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinpDxHex_Int = tempInt;
                            break;
                        }
                    case "pPntosMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinpPntos_Int = tempInt;
                            break;
                        }
                    case "pNuAcMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinpNuAc_Int = tempInt;
                            break;
                        }
                    case "pNuGcMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinpNuGc_Int = tempInt;
                            break;
                        }
                    case "pKDNMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinpKDN_Int = tempInt;
                            break;
                        }
                    case "pHxAMin_TextBox":
                        {
                            App.parameterRangesModel_Save.MinpHxA_Int = tempInt;
                            break;
                        }
                    #endregion

                    #region Max
                    //Max
                    //Sugars
                    case "hexoseMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxHexose_Int = tempInt;
                            break;
                        }
                    case "hexNAcMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxHexNAc_Int = tempInt;
                            break;
                        }
                    case "dxyHexMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxDxyHex_Int = tempInt;
                            break;
                        }
                    case "pentoseMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxPentose_Int = tempInt;
                            break;
                        }
                    case "neuAcMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxNeuAc_Int = tempInt;
                            break;
                        }
                    case "neuGcMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxNeuGc_Int = tempInt;
                            break;
                        }
                    case "kDNMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxKDN_Int = tempInt;
                            break;
                        }
                    case "hexAMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxHexA_Int = tempInt;
                            break;
                        }

                    //Special
                    case "naHMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxNaH_Int = tempInt;
                            break;
                        }
                    case "cH3Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxCH3_Int = tempInt;
                            break;
                        }
                    case "sO3Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxSO3_Int = tempInt;
                            break;
                        }
                    case "oAcetylMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxOAcetyl_Int = tempInt;
                            break;
                        }

                    //User
                    case "userUnit1Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit1_Int = tempInt;
                            break;
                        }
                    case "userUnit2Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit2_Int = tempInt;
                            break;
                        }
                    case "userUnit3Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit3_Int = tempInt;
                            break;
                        }
                    case "userUnit4Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit4_Int = tempInt;
                            break;
                        }
                    case "userUnit5Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit5_Int = tempInt;
                            break;
                        }
                    case "userUnit6Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit6_Int = tempInt;
                            break;
                        }
                    case "userUnit7Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit7_Int = tempInt;
                            break;
                        }
                    case "userUnit8Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit8_Int = tempInt;
                            break;
                        }
                    case "userUnit9Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit9_Int = tempInt;
                            break;
                        }
                    case "userUnit10Max_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxUserUnit10_Int = tempInt;
                            break;
                        }

                    //Amino Acids
                    case "alaMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxAla_Int = tempInt;
                            break;
                        }
                    case "argMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxArg_Int = tempInt;
                            break;
                        }
                    case "asnMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxAsn_Int = tempInt;
                            break;
                        }
                    case "aspMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxAsp_Int = tempInt;
                            break;
                        }
                    case "cysMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxCys_Int = tempInt;
                            break;
                        }
                    case "glnMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxGln_Int = tempInt;
                            break;
                        }
                    case "gluMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxGlu_Int = tempInt;
                            break;
                        }
                    case "glyMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxGly_Int = tempInt;
                            break;
                        }
                    case "hisMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxHis_Int = tempInt;
                            break;
                        }
                    case "ileMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxIle_Int = tempInt;
                            break;
                        }
                    case "leuMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxLeu_Int = tempInt;
                            break;
                        }
                    case "lysMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxLys_Int = tempInt;
                            break;
                        }
                    case "metMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxMet_Int = tempInt;
                            break;
                        }
                    case "pheMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxPhe_Int = tempInt;
                            break;
                        }
                    case "proMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxPro_Int = tempInt;
                            break;
                        }
                    case "serMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxSer_Int = tempInt;
                            break;
                        }
                    case "thrMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxThr_Int = tempInt;
                            break;
                        }
                    case "trpMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxTrp_Int = tempInt;
                            break;
                        }
                    case "tyrMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxTyr_Int = tempInt;
                            break;
                        }
                    case "valMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxVal_Int = tempInt;
                            break;
                        }

                    //Permethyl
                    case "pHexMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxpHex_Int = tempInt;
                            break;
                        }
                    case "pHxNAcMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxpHxNAc_Int = tempInt;
                            break;
                        }
                    case "pDxHexMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxpDxHex_Int = tempInt;
                            break;
                        }
                    case "pPntosMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxpPntos_Int = tempInt;
                            break;
                        }
                    case "pNuAcMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxpNuAc_Int = tempInt;
                            break;
                        }
                    case "pNuGcMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxpNuGc_Int = tempInt;
                            break;
                        }
                    case "pKDNMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxpKDN_Int = tempInt;
                            break;
                        }
                    case "pHxAMax_TextBox":
                        {
                            App.parameterRangesModel_Save.MaxpHxA_Int = tempInt;
                            break;
                        }
                    #endregion
                }
            }
            else if (App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag)
            {
                switch (changedTextBox.Name)
                {
                    #region Min
                    //Min
                    //Sugars
                    case "hexoseMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinHexose_Int = tempInt;
                            break;
                        }
                    case "hexNAcMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinHexNAc_Int = tempInt;
                            break;
                        }
                    case "dxyHexMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinDxyHex_Int = tempInt;
                            break;
                        }
                    case "pentoseMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinPentose_Int = tempInt;
                            break;
                        }
                    case "neuAcMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinNeuAc_Int = tempInt;
                            break;
                        }
                    case "neuGcMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinNeuGc_Int = tempInt;
                            break;
                        }
                    case "kDNMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinKDN_Int = tempInt;
                            break;
                        }
                    case "hexAMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinHexA_Int = tempInt;
                            break;
                        }

                    //Special
                    case "naHMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinNaH_Int = tempInt;
                            break;
                        }
                    case "cH3Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinCH3_Int = tempInt;
                            break;
                        }
                    case "sO3Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinSO3_Int = tempInt;
                            break;
                        }
                    case "oAcetylMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinOAcetyl_Int = tempInt;
                            break;
                        }

                    //User
                    case "userUnit1Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit1_Int = tempInt;
                            break;
                        }
                    case "userUnit2Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit2_Int = tempInt;
                            break;
                        }
                    case "userUnit3Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit3_Int = tempInt;
                            break;
                        }
                    case "userUnit4Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit4_Int = tempInt;
                            break;
                        }
                    case "userUnit5Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit5_Int = tempInt;
                            break;
                        }
                    case "userUnit6Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit6_Int = tempInt;
                            break;
                        }
                    case "userUnit7Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit7_Int = tempInt;
                            break;
                        }
                    case "userUnit8Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit8_Int = tempInt;
                            break;
                        }
                    case "userUnit9Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit9_Int = tempInt;
                            break;
                        }
                    case "userUnit10Min_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinUserUnit10_Int = tempInt;
                            break;
                        }

                    //Amino Acids
                    case "alaMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinAla_Int = tempInt;
                            break;
                        }
                    case "argMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinArg_Int = tempInt;
                            break;
                        }
                    case "asnMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinAsn_Int = tempInt;
                            break;
                        }
                    case "aspMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinAsp_Int = tempInt;
                            break;
                        }
                    case "cysMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinCys_Int = tempInt;
                            break;
                        }
                    case "glnMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinGln_Int = tempInt;
                            break;
                        }
                    case "gluMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinGlu_Int = tempInt;
                            break;
                        }
                    case "glyMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinGly_Int = tempInt;
                            break;
                        }
                    case "hisMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinHis_Int = tempInt;
                            break;
                        }
                    case "ileMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinIle_Int = tempInt;
                            break;
                        }
                    case "leuMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinLeu_Int = tempInt;
                            break;
                        }
                    case "lysMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinLys_Int = tempInt;
                            break;
                        }
                    case "metMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinMet_Int = tempInt;
                            break;
                        }
                    case "pheMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinPhe_Int = tempInt;
                            break;
                        }
                    case "proMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinPro_Int = tempInt;
                            break;
                        }
                    case "serMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinSer_Int = tempInt;
                            break;
                        }
                    case "thrMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinThr_Int = tempInt;
                            break;
                        }
                    case "trpMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinTrp_Int = tempInt;
                            break;
                        }
                    case "tyrMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinTyr_Int = tempInt;
                            break;
                        }
                    case "valMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinVal_Int = tempInt;
                            break;
                        }

                    //Permethyl
                    case "pHexMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinpHex_Int = tempInt;
                            break;
                        }
                    case "pHxNAcMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinpHxNAc_Int = tempInt;
                            break;
                        }
                    case "pDxHexMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinpDxHex_Int = tempInt;
                            break;
                        }
                    case "pPntosMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinpPntos_Int = tempInt;
                            break;
                        }
                    case "pNuAcMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinpNuAc_Int = tempInt;
                            break;
                        }
                    case "pNuGcMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinpNuGc_Int = tempInt;
                            break;
                        }
                    case "pKDNMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinpKDN_Int = tempInt;
                            break;
                        }
                    case "pHxAMin_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MinpHxA_Int = tempInt;
                            break;
                        }
                    #endregion

                    #region Max
                    //Max
                    //Sugars
                    case "hexoseMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxHexose_Int = tempInt;
                            break;
                        }
                    case "hexNAcMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxHexNAc_Int = tempInt;
                            break;
                        }
                    case "dxyHexMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxDxyHex_Int = tempInt;
                            break;
                        }
                    case "pentoseMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxPentose_Int = tempInt;
                            break;
                        }
                    case "neuAcMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxNeuAc_Int = tempInt;
                            break;
                        }
                    case "neuGcMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxNeuGc_Int = tempInt;
                            break;
                        }
                    case "kDNMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxKDN_Int = tempInt;
                            break;
                        }
                    case "hexAMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxHexA_Int = tempInt;
                            break;
                        }

                    //Special
                    case "naHMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxNaH_Int = tempInt;
                            break;
                        }
                    case "cH3Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxCH3_Int = tempInt;
                            break;
                        }
                    case "sO3Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxSO3_Int = tempInt;
                            break;
                        }
                    case "oAcetylMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxOAcetyl_Int = tempInt;
                            break;
                        }

                    //User
                    case "userUnit1Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit1_Int = tempInt;
                            break;
                        }
                    case "userUnit2Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit2_Int = tempInt;
                            break;
                        }
                    case "userUnit3Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit3_Int = tempInt;
                            break;
                        }
                    case "userUnit4Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit4_Int = tempInt;
                            break;
                        }
                    case "userUnit5Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit5_Int = tempInt;
                            break;
                        }
                    case "userUnit6Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit6_Int = tempInt;
                            break;
                        }
                    case "userUnit7Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit7_Int = tempInt;
                            break;
                        }
                    case "userUnit8Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit8_Int = tempInt;
                            break;
                        }
                    case "userUnit9Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit9_Int = tempInt;
                            break;
                        }
                    case "userUnit10Max_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxUserUnit10_Int = tempInt;
                            break;
                        }

                    //Amino Acids
                    case "alaMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxAla_Int = tempInt;
                            break;
                        }
                    case "argMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxArg_Int = tempInt;
                            break;
                        }
                    case "asnMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxAsn_Int = tempInt;
                            break;
                        }
                    case "aspMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxAsp_Int = tempInt;
                            break;
                        }
                    case "cysMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxCys_Int = tempInt;
                            break;
                        }
                    case "glnMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxGln_Int = tempInt;
                            break;
                        }
                    case "gluMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxGlu_Int = tempInt;
                            break;
                        }
                    case "glyMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxGly_Int = tempInt;
                            break;
                        }
                    case "hisMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxHis_Int = tempInt;
                            break;
                        }
                    case "ileMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxIle_Int = tempInt;
                            break;
                        }
                    case "leuMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxLeu_Int = tempInt;
                            break;
                        }
                    case "lysMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxLys_Int = tempInt;
                            break;
                        }
                    case "metMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxMet_Int = tempInt;
                            break;
                        }
                    case "pheMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxPhe_Int = tempInt;
                            break;
                        }
                    case "proMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxPro_Int = tempInt;
                            break;
                        }
                    case "serMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxSer_Int = tempInt;
                            break;
                        }
                    case "thrMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxThr_Int = tempInt;
                            break;
                        }
                    case "trpMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxTrp_Int = tempInt;
                            break;
                        }
                    case "tyrMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxTyr_Int = tempInt;
                            break;
                        }
                    case "valMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxVal_Int = tempInt;
                            break;
                        }

                    //Permethyl
                    case "pHexMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxpHex_Int = tempInt;
                            break;
                        }
                    case "pHxNAcMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxpHxNAc_Int = tempInt;
                            break;
                        }
                    case "pDxHexMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxpDxHex_Int = tempInt;
                            break;
                        }
                    case "pPntosMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxpPntos_Int = tempInt;
                            break;
                        }
                    case "pNuAcMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxpNuAc_Int = tempInt;
                            break;
                        }
                    case "pNuGcMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxpNuGc_Int = tempInt;
                            break;
                        }
                    case "pKDNMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxpKDN_Int = tempInt;
                            break;
                        }
                    case "pHxAMax_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MaxpHxA_Int = tempInt;
                            break;
                        }
                    #endregion
                }
            }

            changedTextBox.Text = tempInt.ToString();
        }

        protected void userUnit1Mass_TextBox_TextChanged(object sender, EventArgs e)
        {
            double tempDouble = 0.0;
            TextBox changedTextBox = (TextBox)sender;
            string Str = changedTextBox.Text.Trim();
            double Num;
            bool isNum = double.TryParse(Str, out Num);
            if (isNum)
                tempDouble = Convert.ToInt32(((TextBox)sender).Text);
            else if (changedTextBox.Text == "")
                tempDouble = 0.0;
            else
                MessageBox.Show("Invalid number");

            if (App.initializingFlagsModel.ParameterRangesSave_InitializeFlag)
            {
                switch (changedTextBox.Name)
                {
                    #region User Unit Mass
                    //User Unit Mass
                    //User
                    case "userUnit1Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit1_Double = tempDouble;
                            break;
                        }
                    case "userUnit2Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit2_Double = tempDouble;
                            break;
                        }
                    case "userUnit3Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit3_Double = tempDouble;
                            break;
                        }
                    case "userUnit4Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit4_Double = tempDouble;
                            break;
                        }
                    case "userUnit5Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit5_Double = tempDouble;
                            break;
                        }
                    case "userUnit6Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit6_Double = tempDouble;
                            break;
                        }
                    case "userUnit7Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit7_Double = tempDouble;
                            break;
                        }
                    case "userUnit8Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit8_Double = tempDouble;
                            break;
                        }
                    case "userUnit9Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit9_Double = tempDouble;
                            break;
                        }
                    case "userUnit10Mass_TextBox":
                        {
                            App.parameterRangesModel_Save.MassUserUnit10_Double = tempDouble;
                            break;
                        }
                    #endregion
                }
            }
            else if (App.initializingFlagsModel.OmniFinderGMRangesSave_InitializeFlag)
            {
                switch (changedTextBox.Name)
                {
                    #region User Unit Mass
                    //User Unit Mass
                    //User
                    case "userUnit1Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit1_Double = tempDouble;
                            break;
                        }
                    case "userUnit2Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit2_Double = tempDouble;
                            break;
                        }
                    case "userUnit3Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit3_Double = tempDouble;
                            break;
                        }
                    case "userUnit4Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit4_Double = tempDouble;
                            break;
                        }
                    case "userUnit5Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit5_Double = tempDouble;
                            break;
                        }
                    case "userUnit6Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit6_Double = tempDouble;
                            break;
                        }
                    case "userUnit7Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit7_Double = tempDouble;
                            break;
                        }
                    case "userUnit8Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit8_Double = tempDouble;
                            break;
                        }
                    case "userUnit9Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit9_Double = tempDouble;
                            break;
                        }
                    case "userUnit10Mass_TextBox":
                        {
                            App.omniFinderGMRangesModel_Save.MassUserUnit10_Double = tempDouble;
                            break;
                        }
                    #endregion
                }
            }
            changedTextBox.Text = tempDouble.ToString();
        }
    }
}
