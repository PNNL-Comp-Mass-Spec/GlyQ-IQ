using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycolyzerGUImvvm.ViewModels;

using GlycolyzerGUImvvm.Models;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            GUIImport.ImportParameters(@"D:\SaveFolder\New Text Document.xml_NLinked_Alditol_PolySA_3.2_Fragment_Neutral.xml");
            ParameterModel parameterModel_Input = GUIImport.parameterModel_Input;
            Console.ReadKey();
        }
    }
}
