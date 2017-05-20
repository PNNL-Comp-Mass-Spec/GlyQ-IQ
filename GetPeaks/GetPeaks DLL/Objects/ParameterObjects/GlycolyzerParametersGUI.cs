using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects;
using GetPeaks_DLL.Glycolyzer;
using GetPeaks_DLL.Glycolyzer.Enumerations;
using GlycolyzerGUImvvm.Models;
using GetPeaks_DLL.Glycolyzer.ParameterConverters;
using OmniFinder.Objects.Enumerations;
using PNNLOmics.Data.Constants;
using PNNLOmics.Data.Constants.Libraries;

namespace GetPeaks_DLL.Objects.ParameterObjects
{
    public class GlycolyzerParametersGUI
    {
        public OmniFinderParameters OmniFinderInput {get;set; }

        public FeatureOriginType FeatureOriginTypeInput { get; set; }

        public LibraryType LibraryTypeInput { get; set; }

        public List<string> FilesIn { get; set; }

        public string FolderIn { get; set; }

        public string FolderOut { get; set; }

        public string SaveLocation { get; set; }

        BatchMode MultiFileProcessor { get; set; }

        public GlycolyzerParametersGUI()
        {
            OmniFinderInput = new OmniFinderParameters();
        }

        public double MassErrorPPM { get; set; }

        public int Charge { get; set; }

        public void ConvertFromGUI(ParameterModel parameterModel_Input)
        {
            OmniFinderInput.BuildingBlocksAminoAcids = ConvertAminoAcids.Convert(parameterModel_Input);
            //OmniFinderInput.BuildingBlocksCrossRings
            //OmniFinderInput.BuildingBlocksElements
            OmniFinderInput.BuildingBlocksMiscellaneousMatter = ConvertMiscellaneousMatter.Convert(parameterModel_Input);
            OmniFinderInput.BuildingBlocksMonosacchcarides = ConvertMonosaccharides.Convert(parameterModel_Input);
            //OmniFinderInput.BuildingBlocksSubAtomicParticles
            OmniFinderInput.BuildingBlocksUserUnit = ConvertUserUnits.Convert(parameterModel_Input);

            OmniFinderInput.CarbohydrateType = ConvertCarbohydrateType.Convert(parameterModel_Input.ExtraScienceParameterModel_Save.CarbohydrateTypeExtraParameter_String);
            OmniFinderInput.ChargeCarryingAdduct = ConvertAdduct.Convert(parameterModel_Input.ExtraScienceParameterModel_Save.ChargeCarryingSpeciesExtraParameter_String);

            OmniFinderInput.UserUnitLibrary = ConvertUserUnits.ConvertLibrary(parameterModel_Input);

            FeatureOriginTypeInput = ConvertFeatureOriginType.Convert(parameterModel_Input.ExtraScienceParameterModel_Save.FeatureOriginTypeExtraParameter_String);

            LibraryTypeInput = ConvertLibraryType.Convert(parameterModel_Input.LibrariesModel_Save.ChosenDefaultLibrary_String);

            FilesIn = parameterModel_Input.FolderModel_Save.InputDataFile_String;

            //FolderIn = System.Text.RegularExpressions.Regex.Replace(parameterModel_Input.FolderModel_Save.InputDataPath_String, FileIn, "");

            string typeOfBatchRun = parameterModel_Input.FolderModel_Save.InputDataFileType_String;
            switch (typeOfBatchRun)
            {
                case "Input Data File":
                    {
                        MultiFileProcessor = BatchMode.SingleFile;
                        
                        FolderIn = parameterModel_Input.FolderModel_Save.InputDataFolder_String;
                        //if (parameterModel_Input.FolderModel_Save.InputDataFile_String.Count > 0)
                        //{
                        //    for (int i = 0; i < parameterModel_Input.FolderModel_Save.InputDataFile_String.Count; i++)
                        //    {
                        //        parameterModel_Input.FolderModel_Save.InputDataFile_String[i] = FolderIn + "\\" + parameterModel_Input.FolderModel_Save.InputDataFile_String[i];
                        //    }
                        //}
                        //else
                        //{
                        //    FilesIn.Add(parameterModel_Input.FolderModel_Save.InputDataPath_String);
                        //}
                    }
                    break;
                case "Input Data Folder":
                    {
                        MultiFileProcessor = BatchMode.MultiFile;
                        FolderIn = parameterModel_Input.FolderModel_Save.InputDataPath_String;
                        //for (int i = 0; i < parameterModel_Input.FolderModel_Save.InputDataFile_String.Count;i++ )
                        //{
                        //    parameterModel_Input.FolderModel_Save.InputDataFile_String[i] = FolderIn + "\\" + parameterModel_Input.FolderModel_Save.InputDataFile_String[i];
                        //}
                    }
                    break;
                default:
                    {
                        MultiFileProcessor = BatchMode.SingleFile;
                    }
                    break;
            }

            FolderOut = parameterModel_Input.FolderModel_Save.OutputDataFolder_String;

            SaveLocation = parameterModel_Input.FolderModel_Save.SaveLocationFolder_String +"\\" + parameterModel_Input.FolderModel_Save.SaveLocationFile_String;

            MassErrorPPM = parameterModel_Input.ExtraScienceParameterModel_Save.MzToleranceExtraParameter_Double;

            Charge = parameterModel_Input.ExtraScienceParameterModel_Save.NumberOfChargesExtraParameter_Int;
        }

        public enum BatchMode
        {
            SingleFile,
            MultiFile
        }

        

        

        

        

        

        

        

        
        

        

        

       
    }
}
