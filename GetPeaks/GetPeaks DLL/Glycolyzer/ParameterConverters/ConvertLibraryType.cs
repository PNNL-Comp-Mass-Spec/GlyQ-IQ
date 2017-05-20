using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Glycolyzer.Enumerations;

namespace GetPeaks_DLL.Glycolyzer.ParameterConverters
{
    public static class ConvertLibraryType
    {
        public static LibraryType Convert(string librayName)
        {
            LibraryType result = LibraryType.Ant_Alditol;

            switch (librayName)
            {
                case "Ant_Alditol":
                    {
                        result = LibraryType.Ant_Alditol;
                    }
                    break;
                case "Cell_Alditol":
                    {
                        result = LibraryType.Cell_Alditol;
                    }
                    break;
                case "Cell_Alditol_V2":
                    {
                        result = LibraryType.Cell_Alditol_V2;
                    }
                    break;
                case "Cell_Alditol_Vmini":
                    {
                        result = LibraryType.Cell_Alditol_Vmini;
                    }
                    break;
                case "Hexose":
                    {
                        result = LibraryType.Hexose;
                    }
                    break;
                case "Xylose":
                    {
                        result = LibraryType.Xylose;
                    }
                    break;
                case "NLinked_Aldehyde":
                    {
                        result = LibraryType.NLinked_Aldehyde;
                    }
                    break;
                case "NLinked_Alditol":
                    {
                        result = LibraryType.NLinked_Alditol;
                    }
                    break;
                case "NLinked_Alditol_2ndIsotope":
                    {
                        result = LibraryType.NLinked_Alditol_2ndIsotope;
                    }
                    break;
                case "NLinked_Alditol_PolySA":
                    {
                        result = LibraryType.NLinked_Alditol_PolySA;
                    }
                    break;
                case "NLinked_Alditol8":
                    {
                        result = LibraryType.NLinked_Alditol8;
                    }
                    break;
                case "NLinked_Alditol9":
                    {
                        result = LibraryType.NLinked_Alditol9;
                    }
                    break;
                case "NLinked_Alditol10":
                    {
                        result = LibraryType.NLinked_Alditol10;
                    }
                    break;
                case "NonCalibrated":
                    {
                        result = LibraryType.NonCalibrated;
                    }
                    break;
                case "OmniFinder":
                    {
                        result = LibraryType.OmniFinder;
                    }
                    break;
                default:
                    {
                        Console.WriteLine("wrong libraryType");
                        Console.ReadKey();
                    }
                    break;
            }

            return result;
        }
    }
}
