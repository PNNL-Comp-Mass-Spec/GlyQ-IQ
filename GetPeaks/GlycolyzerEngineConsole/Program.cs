using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycolyzerGUImvvm.ViewModels;
using GlycolyzerGUImvvm.Models;
using GetPeaks_DLL.Objects.ParameterObjects;
using GetPeaks_DLL.Glycolyzer;

namespace GlycolyzerEngineConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //args  "C:\GlycolyzerData\SaveLocation\NLinked_Alditol_15_Alditol_Neutral.xml"
            
            string parameterFilePathIn = args[0];
            //parameterFilePathIn = @"V:\GlycolyzerOut\NLinked_Alditol_150_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\Cell_Alditol_V2_15_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\NLinked_Alditol_15_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\Cell_Alditol_V2_15_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\NLinked_Alditol_PolySA_15_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\Hexose_15_Alditol_Neutral.xml";
            //Cell_Alditol_V2_15_Alditol_Neutral.xml
            //string parameterFilePathIn = args[0].ToString();
            //string libraryFilePathIn = args[1].ToString();
            //string parameterFilePathIn = @"D:\Backup not sync\V Drive Copy\GlycolyzerGUITestFolders\SaveLocation\NLinked_Alditol_10_Alditol_Neutral.xml";
            //string parameterFilePathIn = @"C:\GlycolyzerData\SaveLocation\NLinked_Alditol_15_Alditol_Neutral.xml";
            //string libraryFilePathIn = @"R:\GlycolyzerData\GlycolyzerLibraryDirectorRDrive.txt";
            //string libraryFilePathIn = @"C:\GlycolyzerData\GlycolyzerLibraryDirectorCDrive.txt";
            string libraryFilePathIn = @"C:\GlycolyzerData\0_Libraries\GlycolyzerLibraryDirectorEDrive.txt";

            Console.WriteLine(parameterFilePathIn + " is loaded");
            Console.WriteLine("I am running.  ");
            //Console.WriteLine("Press a key to continue, HI"); Console.ReadKey();


            GUIImport.ImportParameters(parameterFilePathIn);
            ParameterModel parameterModel_Input = GUIImport.parameterModel_Input;

            GlycolyzerParametersGUI parameters = new GlycolyzerParametersGUI();
            parameters.ConvertFromGUI(parameterModel_Input);

            string libraryLocation = libraryFilePathIn;

            GlycolyzerController controller = new GlycolyzerController();
            controller.Glycolyze(parameters, libraryLocation);
        }
    }
}
