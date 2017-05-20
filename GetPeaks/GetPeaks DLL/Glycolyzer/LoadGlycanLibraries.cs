using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Glycolyzer.Enumerations;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.Glycolyzer
{
    public class LoadGlycanLibraries
    {
        public Dictionary<LibraryType, string> Load(string pathToLibraries)
        {
            Dictionary<LibraryType, string> libraries = new Dictionary<LibraryType, string>();

            StringLoadTextFileLine loader = new StringLoadTextFileLine();
            List<string> lines = loader.SingleFileByLine(pathToLibraries);

            string columnHeadders = lines[0];
            int startline = 1;//0 is the headder

            for (int i = startline; i < lines.Count; i++)//i=0 is the headder
            {
                FileIterator.deliminator textDeliminator = FileIterator.deliminator.Comma;

                string[] wordArray;
     
                char spliter;
                switch (textDeliminator)
                {
                    case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma:
                        {
                            spliter = ',';
                        }
                        break;
                    case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab:
                        {
                            spliter = '\t';
                        }
                        break;
                    default:
                        {
                            spliter = ',';
                        }
                        break;
                }

                string line = lines[i];

                wordArray = line.Split(spliter);

                LibraryType desiredLibrary = LibraryType.Hexose;
                switch (wordArray[0])
                {
                    #region inside
                    case "Ant_Alditol":
                        {
                            desiredLibrary = LibraryType.Ant_Alditol;
                        }
                        break;
                    case "Cell_Alditol":
                        {
                            desiredLibrary = LibraryType.Cell_Alditol;
                        }
                        break;
                    case "Cell_Alditol_V2":
                        {
                            desiredLibrary = LibraryType.Cell_Alditol_V2;
                        }
                        break;
                    case "Cell_Alditol_Vmini":
                        {
                            desiredLibrary = LibraryType.Cell_Alditol_Vmini;
                        }
                        break;
                    case "Hexose":
                        {
                            desiredLibrary = LibraryType.Hexose;
                        }
                        break;
                    case "Xylose":
                        {
                            desiredLibrary = LibraryType.Xylose;
                        }
                        break;
                    case "NLinked_Aldehyde":
                        {
                            desiredLibrary = LibraryType.NLinked_Aldehyde;
                        }
                        break;
                    case "NLinked_Alditol":
                        {
                            desiredLibrary = LibraryType.NLinked_Alditol;
                        }
                        break;
                    case "NLinked_Alditol_2ndIsotope":
                        {
                            desiredLibrary = LibraryType.NLinked_Alditol_2ndIsotope;
                        }
                        break;  
                    case "NLinked_Alditol_PolySA":
                        {
                            desiredLibrary = LibraryType.NLinked_Alditol_PolySA;
                        }
                        break;
                    case "NLinked_Alditol10":
                        {
                            desiredLibrary = LibraryType.NLinked_Alditol10;
                        }
                        break;
                    case "NLinked_Alditol8":
                        {
                            desiredLibrary = LibraryType.NLinked_Alditol8;
                        }
                        break;
                    case "NLinked_Alditol9":
                        {
                            desiredLibrary = LibraryType.NLinked_Alditol9;
                        }
                        break;
                    
                    case "NonCalibrated":
                        {
                            desiredLibrary = LibraryType.NonCalibrated;
                        }
                        break;
                    case "DiagnosticIons":
                        {
                            desiredLibrary = LibraryType.DiagnosticIons;
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("missing glycan type for this library");
                            Console.ReadKey();
                        }
                        break;
                    #endregion
                }

                libraries.Add(desiredLibrary,wordArray[1]);

            }

            return libraries;
        }
    }
}
