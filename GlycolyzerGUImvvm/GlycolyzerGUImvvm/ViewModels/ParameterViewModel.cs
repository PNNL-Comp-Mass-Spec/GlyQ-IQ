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
                        String tempPath = FileBrowse.fileBrowse();
                        if (tempPath != "")
                        {
                            LibrariesModel.ChosenCustomLibrary_String = tempPath;
                            FolderModel.PreviousPath_String = LibrariesModel.ChosenCustomLibrary_String;
                        }
                        break;
                    }
                case "inputFile":
                    {
                        if (FolderModel.InputDataFileType_String == "Input Data File")
                        {
                            String tempPath = FileBrowse.fileBrowse();
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
                            MessageBox.Show("No Save Location Chosen");
                        break;
                    }
                case "Go":
                    {
                        if (File.Exists(@"C:\GlycolyzerData\Application\Release\GlycolyzerEngineConsole.exe"))
                        {
                            System.Diagnostics.Process.Start(@"C:\GlycolyzerData\Application\Release\GlycolyzerEngineConsole.exe",
                                App.folderModel_Save.InputDataFile_String[0]);
                        }

                        MessageBox.Show("Done");
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
                        break;
                    }
                case "GetDefaultAddress":
                    {
                        switch (App.librariesModel_Save.ChosenDefaultLibrary_String)
                        {
                            case "NLinked_Alditol":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "NLinked_Alditol_2ndIsotope":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "NLinked_Aldehyde":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "Cell_Alditol":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "Cell_Alditol_V2":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "Cell_Alditol_Vmini":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "Ant_Alditol":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "NonCalibrated":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "NLinked_Alditol_PolySA":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "NLinked_Alditol8":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "NLinked_Alditol9":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                            case "Hexose":
                                {
                                    //App.librariesModel_Save.AddressDefaultLibrary_String = ;
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
