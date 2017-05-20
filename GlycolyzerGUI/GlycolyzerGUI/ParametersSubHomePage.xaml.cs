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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GlycolyzerGUI
{
    /// <summary>
    /// Interaction logic for ParametersSubHomePage.xaml
    /// </summary>
    public partial class ParametersSubHomePage : Page
    {
        public ParametersSubHomePage()
        {
            InitializeComponent();
        }

        private void customLibraryBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "(*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                customLibraryBrowseTextBox.Text = filename;
            }
        }

        private void inputDataFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "(*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                inputDataFolderBrowseTextBox.Text = filename;
            }
        }

        private void outputDataFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "(*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                outputDataFolderBrowseTextBox.Text = filename;
            }
        }

        private void saveLocationFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "(*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                saveLocationFolderBrowseTextBox.Text = filename;
            }
        }

        private void saveLocationFolderSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.inputOutputSaveFolderVariables.SaveLocationFolder_String != "")
                SaveXMLFile(App.inputOutputSaveFolderVariables.SaveLocationFolder_String);
            else
                MessageBox.Show("No Save Location Chosen");                                                                    
        }

        public static void SaveXMLFile(String writeFile) //Can move to App?????????????????????????????????????????????????????????
        {                                                   //don't need save button(or does it have a dif purpose)?????????????????????
            if (writeFile == "")
                writeFile = "D:\\GlycolyzerXMLFile.xml";

            string writeFolder = System.IO.Path.GetDirectoryName(writeFile);

            string newPath = System.IO.Path.Combine(writeFolder, "GlycolyzerSubDir");
            System.IO.Directory.CreateDirectory(newPath);
            string newFileName = "Glycolyzer";
            newPath = System.IO.Path.Combine(newPath, newFileName);

            /*if (!System.IO.File.Exists(newPath))
            {
            }*/

            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(String));
            System.IO.StreamWriter file = new System.IO.StreamWriter(writeFile);
            writer.Serialize(file, "Individual Files in GlycolyzerSubDir Folder");
            file.Close();

            System.Xml.Serialization.XmlSerializer omniWriter = new System.Xml.Serialization.XmlSerializer(typeof(App.OmniFinderVariables));
            System.IO.StreamWriter omniFile = new System.IO.StreamWriter(newPath + "ParametersOmniFinder.xml");
            omniWriter.Serialize(omniFile, App.parametersOmniFinderVariables);
            //FileInfo omniInfo = new FileInfo(App.inputOutputSaveFolderVariables.SaveLocationFolder_String + "ParametersOmniFinder.xml");
            //omniInfo.Attributes = FileAttributes.Hidden;
            omniFile.Close();

            System.Xml.Serialization.XmlSerializer parametersWriter = new System.Xml.Serialization.XmlSerializer(typeof(App.ExtraScienceParameterVariables));
            System.IO.StreamWriter parametersFile = new System.IO.StreamWriter(newPath + "ExtraScienceParameters.xml");
            parametersWriter.Serialize(parametersFile, App.extraScienceParameterVariables);
            //FileInfo parametersInfo = new FileInfo(App.inputOutputSaveFolderVariables.SaveLocationFolder_String + "ExtraScienceParameters.xml");
            //parametersInfo.Attributes = FileAttributes.Hidden;
            parametersFile.Close();

            System.Xml.Serialization.XmlSerializer folderWriter = new System.Xml.Serialization.XmlSerializer(typeof(App.InputOutputSaveFolderVariables));
            System.IO.StreamWriter folderFile = new System.IO.StreamWriter(newPath + "InputOutputSaveFolder.xml");
            folderWriter.Serialize(folderFile, App.inputOutputSaveFolderVariables);
            //FileInfo folderInfo = new FileInfo(App.inputOutputSaveFolderVariables.SaveLocationFolder_String + "InputOutputSaveFolder.xml");
            //folderInfo.Attributes = FileAttributes.Hidden;
            folderFile.Close();

            System.Xml.Serialization.XmlSerializer parameterRangesWriter = new System.Xml.Serialization.XmlSerializer(typeof(App.RangesPageVariables));
            System.IO.StreamWriter parameterRangesFile = new System.IO.StreamWriter(newPath + "ParametersRanges.xml");
            parameterRangesWriter.Serialize(parameterRangesFile, App.parametersRangesPageVariables);
            //FileInfo rangesInfo = new FileInfo(App.inputOutputSaveFolderVariables.SaveLocationFolder_String + "Ranges.xml");
            //rangesInfo.Attributes = FileAttributes.Hidden;
            parameterRangesFile.Close();

            System.Xml.Serialization.XmlSerializer libraryWriter = new System.Xml.Serialization.XmlSerializer(typeof(App.LibraryVariables));
            System.IO.StreamWriter libraryFile = new System.IO.StreamWriter(newPath + "Library.xml");
            libraryWriter.Serialize(libraryFile, App.libraryVariables);
            //FileInfo libraryInfo = new FileInfo(App.inputOutputSaveFolderVariables.SaveLocationFolder_String + "Library.xml");
            //libraryInfo.Attributes = FileAttributes.Hidden;
            libraryFile.Close();

            System.Xml.Serialization.XmlSerializer glycanWriter = new System.Xml.Serialization.XmlSerializer(typeof(App.GlycanMakerVariables));
            System.IO.StreamWriter glycanFile = new System.IO.StreamWriter(newPath + "GlycanMaker.xml");
            glycanWriter.Serialize(glycanFile, App.glycanMakerVariables);
            //FileInfo glycanInfo = new FileInfo(App.inputOutputSaveFolderVariables.SaveLocationFolder_String + "GlycanMaker.xml");
            //glycanInfo.Attributes = FileAttributes.Hidden;
            glycanFile.Close();

            System.Xml.Serialization.XmlSerializer utilitiesOmniWriter = new System.Xml.Serialization.XmlSerializer(typeof(App.OmniFinderVariables));
            System.IO.StreamWriter utilitiesOmniFile = new System.IO.StreamWriter(newPath + "UtilitiesOmniFinder.xml");
            utilitiesOmniWriter.Serialize(utilitiesOmniFile, App.utilitiesOmniFinderVariables);
            //FileInfo utilitiesOmniInfo = new FileInfo(App.inputOutputSaveFolderVariables.SaveLocationFolder_String + "UtilitiesOmniFinder.xml");
            //utilitiesOmniInfo.Attributes = FileAttributes.Hidden;
            utilitiesOmniFile.Close();

            System.Xml.Serialization.XmlSerializer utilityRangesWriter = new System.Xml.Serialization.XmlSerializer(typeof(App.RangesPageVariables));
            System.IO.StreamWriter utilityRangesFile = new System.IO.StreamWriter(newPath + "UtilitiesRanges.xml");
            utilityRangesWriter.Serialize(utilityRangesFile, App.utilitiesRangesPageVariables);
            //FileInfo rangesInfo = new FileInfo(App.inputOutputSaveFolderVariables.SaveLocationFolder_String + "Ranges.xml");
            //rangesInfo.Attributes = FileAttributes.Hidden;
            utilityRangesFile.Close();
        }

        private void uploadDefaultLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an isntance of XmlTextReader and call Read method to read the file
            if (App.libraryVariables.ChosenDefaultLibrary_String != "")
            {
                //readXMLFile(App.libraryVariables.ChosenDefaultLibrary_String);
            }
            else
                MessageBox.Show("No Library Chosen to Upload");
        }

        private void uploadCustomLibraryButton_Click(object sender, RoutedEventArgs e)//OPEN SAVED DOCUMENT ???????????????????????????
        {
            // Create an isntance of XmlTextReader and call Read method to read the file
            if (App.libraryVariables.ChosenCustomLibrary_String != "")
            {
                /*HomePage.ReadXMLFile(App.libraryVariables.ChosenCustomLibrary_String);  
                InitializeParametersPage();
                App.parametersOmniFinderPageDesign.InitializeOmniFinderControls();*/
            }
            else
                MessageBox.Show("No Library Chosen to Upload");
        }

        private void Tab2_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.initializePages.ParametersOmniFinderPageDesign_InitializeFlag == true)
            {
                App.parametersOmniFinderPageDesign = new ParametersOmniFinderPageDesign();
                App.parametersOmniFinderPageDesign.InitializeOmniFinderControls();
                App.initializePages.ParametersOmniFinderPageDesign_InitializeFlag = false;
            }
            Tab2.Content = App.parametersOmniFinderPageDesign.parameterOmniFinderPage_Canvas;
        }

        private void Tab4_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.initializePages.ParametersRanges_InitializeFlag == true)
            {
                App.parametersRangesPage = new ParametersRangesPage();
                App.initializePages.ParametersRanges_InitializeFlag = false;
            }
            Tab4.Content = App.parametersRangesPage.parametersRangesPage_Canvas;
        }

        void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.initializePages.CheckTab_InitializeFlag == true)
            {
                if (Tab4.IsSelected)
                {
                    App.parametersRangesPage.ParametersRangesPage_Initialization();
                }
                else
                {
                    App.parametersSubHomePage.Height = 380;
                    App.parametersSubHomePage.Width = 400;
                }
            }
            else
                App.initializePages.CheckTab_InitializeFlag = true;
        }

        private void defaultLibrariesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.initializePages.Parameters_InitializeFlag != true)
            {
                ComboBox changedComboBox = (ComboBox)sender;
                switch (changedComboBox.Name)
                {
                    //Default Library
                    case "defaultLibrariesComboBox":
                        {
                            App.libraryVariables.ChosenDefaultLibrary_String = (defaultLibrariesComboBox.SelectedValue.ToString()).Substring(38);
                            String ChosenDefaultLibrary_Temp = (defaultLibrariesComboBox.SelectedValue.ToString()).Substring(38);
                            if (ChosenDefaultLibrary_Temp != "")
                            {
                                //Finds Selected Option To Be Displayed When There Is Saved Information
                                int index = -1;
                                string[] s = new string[13];
                                s[0] = "No Library Selected";
                                s[1] = "NLinked_Alditol";
                                s[2] = "NLinked_Alditol_2ndIsotope";
                                s[3] = "NLinked_Aldehyde";
                                s[4] = "Cell_Alditol";
                                s[5] = "Cell_Alditol_V2";
                                s[6] = "Cell_Alditol_Vmini";
                                s[7] = "Ant_Alditol";
                                s[8] = "NonCalibrated";
                                s[9] = "NLinked_Alditol_PolySA";
                                s[10] = "NLinked_Alditol8";
                                s[11] = "NLinked_Alditol9";
                                s[12] = "Hexose";
                                // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                                for (int j = 0; j < s.Length; j++)
                                {
                                    if (s[j] == ChosenDefaultLibrary_Temp)
                                        index = j;
                                }
                                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                                defaultLibrariesComboBox.SelectedIndex = index;
                            }
                            else
                                defaultLibrariesComboBox.SelectedIndex = 0;
                            break;
                        }

                    //m/z Tolerance
                    case "mzToleranceComboBox":
                        {
                            App.extraScienceParameterVariables.MzToleranceTypeExtraParameter_String = (mzToleranceComboBox.SelectedValue.ToString()).Substring(38);
                            String MzToleranceTypeExtraParameter_Temp = (mzToleranceComboBox.SelectedValue.ToString()).Substring(38);
                            if (MzToleranceTypeExtraParameter_Temp != "")
                            {
                                //Finds Selected Option To Be Displayed When There Is Saved Information
                                int index = -1;
                                string[] s = new string[2];
                                s[0] = "ppm";
                                s[1] = "Da";
                                // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                                for (int j = 0; j < s.Length; j++)
                                {
                                    if (s[j] == MzToleranceTypeExtraParameter_Temp)
                                        index = j;
                                }
                                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                                mzToleranceComboBox.SelectedIndex = index;
                            }
                            else
                                mzToleranceComboBox.SelectedIndex = 0;
                            break;
                        }

                    //Carbohydrate Type
                    case "carbohydrateTypeExtraScienceParametersComboBox":
                        {
                            App.extraScienceParameterVariables.CarbohydrateTypeExtraParameter_String = (carbohydrateTypeExtraScienceParametersComboBox.SelectedValue.ToString()).Substring(38);
                            String CarbohydrateTypeExtraParameter_Temp = (carbohydrateTypeExtraScienceParametersComboBox.SelectedValue.ToString()).Substring(38);
                            if (CarbohydrateTypeExtraParameter_Temp != "")
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
                                    if (s[j] == CarbohydrateTypeExtraParameter_Temp)
                                        index = j;
                                }
                                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                                carbohydrateTypeExtraScienceParametersComboBox.SelectedIndex = index;
                            }
                            else
                                carbohydrateTypeExtraScienceParametersComboBox.SelectedIndex = 0;
                            break;
                        }

                    //Charge Carrying Species
                    case "chargeCarryingSpeciesExtraScienceParameterComboBox":
                        {
                            App.extraScienceParameterVariables.ChargeCarryingSpeciesExtraParameter_String = (chargeCarryingSpeciesExtraScienceParameterComboBox.SelectedValue.ToString()).Substring(38);
                            String ChargeCarryingSpeciesExtraParameter_Temp = (chargeCarryingSpeciesExtraScienceParameterComboBox.SelectedValue.ToString()).Substring(38);
                            if (ChargeCarryingSpeciesExtraParameter_Temp != "")
                            {
                                //Finds Selected Option To Be Displayed When There Is Saved Information
                                int index = -1;
                                string[] s = new string[8];
                                s[0] = "H";
                                s[1] = "Na";
                                s[2] = "K";
                                s[3] = "-H";
                                s[4] = "NH4";
                                s[5] = "Water";
                                s[6] = "Neutral";
                                s[7] = "User Defined";
                                // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                                for (int j = 0; j < s.Length; j++)
                                {
                                    if (s[j] == ChargeCarryingSpeciesExtraParameter_Temp)
                                        index = j;
                                }
                                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                                chargeCarryingSpeciesExtraScienceParameterComboBox.SelectedIndex = index;
                            }
                            else
                                chargeCarryingSpeciesExtraScienceParameterComboBox.SelectedIndex = 0;
                            break;
                        }
                }
            }
        }

        private void customLibraryBrowseTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.initializePages.Parameters_InitializeFlag != true)
            {
                TextBox changedTextBox = (TextBox)sender;

                switch (changedTextBox.Name)
                {
                    //Custom Library
                    case "customLibraryBrowseTextBox":
                        {
                            App.libraryVariables.ChosenCustomLibrary_String = changedTextBox.Text;
                            customLibraryBrowseTextBox.Text = App.libraryVariables.ChosenCustomLibrary_String;
                            break;
                        }

                    //Number of Charges
                    case "numberOfChargesTextBox":
                        {
                            string Str = changedTextBox.Text.Trim();
                            int Num;
                            bool isNum = int.TryParse(Str, out Num);
                            if (isNum)
                                App.extraScienceParameterVariables.NumberOfChargesExtraParameter_Int = Convert.ToInt32(((TextBox)sender).Text);
                            else if (changedTextBox.Text == "")
                                App.extraScienceParameterVariables.NumberOfChargesExtraParameter_Int = 1;
                            else
                            {
                                MessageBox.Show("Invalid number");
                                App.extraScienceParameterVariables.NumberOfChargesExtraParameter_Int = 1;
                            }
                            numberOfChargesTextBox.Text = (App.extraScienceParameterVariables.NumberOfChargesExtraParameter_Int).ToString();
                            break;
                        }

                    //m/z Tolerance
                    case "mzToleranceTextBox":
                        {
                            string Str = changedTextBox.Text.Trim();
                            double Num;
                            bool isNum = double.TryParse(Str, out Num);
                            if (isNum)
                                App.extraScienceParameterVariables.MzToleranceExtraParameter_Double = Convert.ToDouble(((TextBox)sender).Text);
                            else if (changedTextBox.Text == "")
                                App.extraScienceParameterVariables.MzToleranceExtraParameter_Double = 0.0;
                            else
                            {
                                MessageBox.Show("Invalid number");
                                App.extraScienceParameterVariables.MzToleranceExtraParameter_Double = 0.0;
                            }
                            mzToleranceTextBox.Text = (App.extraScienceParameterVariables.MzToleranceExtraParameter_Double).ToString();
                            break;
                        }

                    //Input Data Folder
                    case "inputDataFolderBrowseTextBox":
                        {
                            App.inputOutputSaveFolderVariables.InputDataFolder_String = changedTextBox.Text;
                            inputDataFolderBrowseTextBox.Text = App.inputOutputSaveFolderVariables.InputDataFolder_String;
                            break;
                        }
                    //Output Data Folder
                    case "outputDataFolderBrowseTextBox":
                        {
                            App.inputOutputSaveFolderVariables.OutputDataFolder_String = changedTextBox.Text;
                            outputDataFolderBrowseTextBox.Text = App.inputOutputSaveFolderVariables.OutputDataFolder_String;
                            break;
                        }
                    //Save Location Folder
                    case "saveLocationFolderBrowseTextBox":
                        {
                            App.inputOutputSaveFolderVariables.SaveLocationFolder_String = changedTextBox.Text;
                            saveLocationFolderBrowseTextBox.Text = App.inputOutputSaveFolderVariables.SaveLocationFolder_String;
                            break;
                        }
                }
            }
        }

        public void InitializeParametersPage()
        {
            //Default Library
            String ChosenDefaultLibrary_Temp = App.libraryVariables.ChosenDefaultLibrary_String;
            if (ChosenDefaultLibrary_Temp != "")
            {
                //Finds Selected Option To Be Displayed When There Is Saved Information
                int index = -1;
                string[] s = new string[13];
                s[0] = "No Library Selected";
                s[1] = "NLinked_Alditol";
                s[2] = "NLinked_Alditol_2ndIsotope";
                s[3] = "NLinked_Aldehyde";
                s[4] = "Cell_Alditol";
                s[5] = "Cell_Alditol_V2";
                s[6] = "Cell_Alditol_Vmini";
                s[7] = "Ant_Alditol";
                s[8] = "NonCalibrated";
                s[9] = "NLinked_Alditol_PolySA";
                s[10] = "NLinked_Alditol8";
                s[11] = "NLinked_Alditol9";
                s[12] = "Hexose";
                // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                for (int j = 0; j < s.Length; j++)
                {
                    if (s[j] == ChosenDefaultLibrary_Temp)
                        index = j;
                }
                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                defaultLibrariesComboBox.SelectedIndex = index;
            }
            else
                defaultLibrariesComboBox.SelectedIndex = 0;

            //Custom Library
            customLibraryBrowseTextBox.Text = App.libraryVariables.ChosenCustomLibrary_String;

            //m/z Tolerance
            mzToleranceTextBox.Text = (App.extraScienceParameterVariables.MzToleranceExtraParameter_Double).ToString();
            String MzToleranceTypeExtraParameter_Temp = App.extraScienceParameterVariables.MzToleranceTypeExtraParameter_String;
            if (MzToleranceTypeExtraParameter_Temp != "")
            {
                //Finds Selected Option To Be Displayed When There Is Saved Information
                int index = -1;
                string[] s = new string[2];
                s[0] = "ppm";
                s[1] = "Da";
                // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                for (int j = 0; j < s.Length; j++)
                {
                    if (s[j] == MzToleranceTypeExtraParameter_Temp)
                        index = j;
                }
                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                mzToleranceComboBox.SelectedIndex = index;
            }
            else
                mzToleranceComboBox.SelectedIndex = 0;

            //Carbohydrate Type
            String CarbohydrateTypeExtraParameter_Temp = App.extraScienceParameterVariables.CarbohydrateTypeExtraParameter_String;
            if (CarbohydrateTypeExtraParameter_Temp != "")
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
                    if (s[j] == CarbohydrateTypeExtraParameter_Temp)
                        index = j;
                }
                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                carbohydrateTypeExtraScienceParametersComboBox.SelectedIndex = index;
            }
            else
                carbohydrateTypeExtraScienceParametersComboBox.SelectedIndex = 0;

            //Charge Carrier Species
            numberOfChargesTextBox.Text = (App.extraScienceParameterVariables.NumberOfChargesExtraParameter_Int).ToString();
            String ChargeCarryingSpeciesExtraParameter_Temp = App.extraScienceParameterVariables.ChargeCarryingSpeciesExtraParameter_String;
            if (ChargeCarryingSpeciesExtraParameter_Temp != "")
            {
                //Finds Selected Option To Be Displayed When There Is Saved Information
                int index = -1;
                string[] s = new string[8];
                s[0] = "H";
                s[1] = "Na";
                s[2] = "K";
                s[3] = "-H";
                s[4] = "NH4";
                s[5] = "Water";
                s[6] = "Neutral";
                s[7] = "User Defined";
                // Loop Through List With for Loop To Find The Selected Option If One Was Saved
                for (int j = 0; j < s.Length; j++)
                {
                    if (s[j] == ChargeCarryingSpeciesExtraParameter_Temp)
                        index = j;
                }
                //Assigns Saved Selected Option To Be Already Selected If One Was Saved
                chargeCarryingSpeciesExtraScienceParameterComboBox.SelectedIndex = index;
            }
            else
                chargeCarryingSpeciesExtraScienceParameterComboBox.SelectedIndex = 0;

            inputDataFolderBrowseTextBox.Text = App.inputOutputSaveFolderVariables.InputDataFolder_String;
            outputDataFolderBrowseTextBox.Text = App.inputOutputSaveFolderVariables.OutputDataFolder_String;
            saveLocationFolderBrowseTextBox.Text = App.inputOutputSaveFolderVariables.SaveLocationFolder_String;
        }
    }
}