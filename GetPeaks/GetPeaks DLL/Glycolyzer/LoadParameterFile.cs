using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycolyzerGUImvvm.Models;
using GlycolyzerGUImvvm.ViewModels;


namespace GetPeaks_DLL.Glycolyzer
{
    public class LoadParameterFile
    {
        public void load()
        {
            GUIImport.ImportParameters(@"D:\SaveFolder\New Text Document.xml_NLinked_Alditol_PolySA_3.2_Fragment_Neutral.xml");
            ParameterModel parameterModel_Input = GUIImport.parameterModel_Input;

            //convert
            Console.ReadKey();

 
        }
    }
}
