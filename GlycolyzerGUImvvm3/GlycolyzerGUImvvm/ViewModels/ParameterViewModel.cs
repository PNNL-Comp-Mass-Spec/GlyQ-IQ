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
using System.Windows.Input;
using GlycolyzerGUImvvm.Commands;
using GlycolyzerGUImvvm.Models;
using System.Windows;
using System.IO;

namespace GlycolyzerGUImvvm.ViewModels
{
    public class ParameterViewModel : ObservableObject
    {
        private Models.LibrariesModel librariesModel;

        public Models.LibrariesModel LibrariesModel
        {
            get { return librariesModel; }
            set 
            {
                if (value != librariesModel) 
                { librariesModel = value; App.librariesModel_Save = librariesModel; this.OnPropertyChanged("librariesModel"); } 
            }
        }

        private Models.FolderModel folderModel;

        public Models.FolderModel FolderModel
        {
            get { return folderModel; }
            set
            {
                if (value != folderModel)
                { folderModel = value; App.folderModel_Save = folderModel; this.OnPropertyChanged("folderModel"); }
            }
        }

        private Models.ExtraScienceParameterModel extraScienceParameterModel;

        public Models.ExtraScienceParameterModel ExtraScienceParameterModel
        {
            get { return extraScienceParameterModel; }
            set
            {
                if (value != extraScienceParameterModel)
                {
                    extraScienceParameterModel = value; App.extraScienceParameterModel_Save = extraScienceParameterModel;
                    this.OnPropertyChanged("extraScienceParameterModel");
                }
            }
        }

        private ICommand m_ButtonCommand;
        public ICommand ButtonCommand
        {
            get
            {
                return m_ButtonCommand;
            }
            set
            {
                m_ButtonCommand = value;
            }
        }

        public ParameterViewModel()
        {
            librariesModel = new Models.LibrariesModel();
            folderModel = new Models.FolderModel();
            extraScienceParameterModel = new Models.ExtraScienceParameterModel();

            LibrariesModel = App.librariesModel_Save;
            FolderModel = App.folderModel_Save;
            ExtraScienceParameterModel = App.extraScienceParameterModel_Save;

            ButtonCommand = new RelayCommand(new Action<object>(ButtonAction));
        }

        public void ButtonAction(object obj)
        {
            switch ((String)obj)
            {
                case "customLibrary":
                    {
                        String tempPath = FileBrowse.fileBrowse("xml");
                        if (tempPath != "")
                        {
                            LibrariesModel.ChosenCustomLibrary_String = tempPath;
                            FolderModel.PreviousPath_String = LibrariesModel.ChosenCustomLibrary_String;
                        }
                        break;
                    }
                case "inputFile":
                    {
                        FolderModel.InputDataFile_String.Clear();

                        if (FolderModel.InputDataFileType_String == "Input Data File")
                        {
                            String tempPath = FileBrowse.fileBrowse("xml");
                            if (tempPath != "")
                            {
                                FolderModel.InputDataPath_String = tempPath;
                                FolderModel.InputDataFolder_String = Path.GetDirectoryName(tempPath);
                                FolderModel.InputDataFile_String.Add(Path.GetFileName(tempPath));

                                FolderModel.PreviousPath_String = tempPath;
                            }
                        }
                        else if (FolderModel.InputDataFileType_String == "Input Data Folder")
                        {
                            String tempPath = FolderBrowse.folderBrowse("inputFolder");
                            if (tempPath != "")
                            {
                                FolderModel.InputDataPath_String = tempPath;
                                FolderModel.InputDataFolder_String = Path.GetDirectoryName(tempPath);
                                FolderModel.InputDataFile_String = (Directory.GetFiles(tempPath).Select(fileName => Path.GetFileName(fileName))).ToList();

                                FolderModel.PreviousPath_String = tempPath;
                            }
                        }
                        break;
                    }
                case "outputFolder":
                    {
                        String tempPath = FolderBrowse.folderBrowse("outputFolder");
                        if (tempPath != "")
                        {
                            FolderModel.OutputDataFolder_String = tempPath;
                            FolderModel.PreviousPath_String = FolderModel.OutputDataFolder_String;
                        }
                        break;
                    }
                case "saveFolder":
                    {
                        String tempPath = FolderBrowse.folderBrowse("saveFolder");
                        if (tempPath != "")
                        {
                            FolderModel.SaveLocationFolder_String = tempPath;
                            FolderModel.PreviousPath_String = FolderModel.SaveLocationFolder_String;
                        }
                        break;
                    }
                case "saveData":
                    {
                        if (FolderModel.SaveLocationFolder_String != "" && FolderModel.SaveLocationFile_String != "")
                            GUIExport.ExportParameterXMLFile(FolderModel.SaveLocationFolder_String + "\\" + FolderModel.SaveLocationFile_String);
                        else
                            MessageBox.Show("Invalid Save Location Chosen");
                        break;
                    }
                case "executionFile":
                    {
                        String tempPath = FileBrowse.fileBrowse("exe");
                        if (tempPath != "")
                        {
                            FolderModel.ExecutionFile_String = tempPath;
                            FolderModel.PreviousPath_String = tempPath;
                        }
                        break;
                    }
                case "defaultExecutionFile":
                    {
                        FolderModel.ExecutionFile_String = "C:\\GlycolyzerData\\1_Application\\Release\\GlycolyzerEngineConsole.exe";
                        break;
                    }
                case "Go":
                    {
                        if (File.Exists(FolderModel.ExecutionFile_String))
                        {
                            //System.Diagnostics.Process.Start(FolderModel.ExecutionFile_String, App.folderModel_Save.InputDataFile_String[0]);
                            string parameterfile = App.folderModel_Save.SaveLocationFolder_String + "\\" + App.folderModel_Save.SaveLocationFile_String;
                            //string parameterfile = App.folderModel_Save.InputDataPath_String;
                            System.Diagnostics.Process.Start(FolderModel.ExecutionFile_String, parameterfile);
                        }
                        else
                            MessageBox.Show("Invalid Execution File");
                        break;
                    }
            }
        }

        public static void RunChangedAction(string command)
        {
            switch (command)
            {
                case "AutoFileName":
                    {
                        string chosenDefaultLibrary_Temp = App.librariesModel_Save.ChosenDefaultLibrary_String;
                        if (chosenDefaultLibrary_Temp == "No Library Selected")
                        {
                            chosenDefaultLibrary_Temp = "NoLibrarySelected";
                        }

                        App.folderModel_Save.SaveLocationFile_String = chosenDefaultLibrary_Temp + "_" +
                                App.extraScienceParameterModel_Save.MzToleranceExtraParameter_Double
                                + "_" + App.extraScienceParameterModel_Save.CarbohydrateTypeExtraParameter_String + "_" +
                                App.extraScienceParameterModel_Save.ChargeCarryingSpeciesExtraParameter_String + ".xml";
                        break;
                    }
                case "ClearInputDataString":
                    {
                        App.folderModel_Save.InputDataPath_String = "";

                        if (App.folderModel_Save.InputDataFileType_String == "OmniFinder Mass")
                        {
                            App.folderModel_Save.InputAndOmniFinderMassLabel_String = "OmniFinder Mass:";
                        }
                        else
                        {
                            App.folderModel_Save.InputAndOmniFinderMassLabel_String = "Input Data Location:";
                        }
                        break;
                    }
            }
        }
    }
}
