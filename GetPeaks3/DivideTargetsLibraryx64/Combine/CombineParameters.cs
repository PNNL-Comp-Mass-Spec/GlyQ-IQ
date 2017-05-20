using System;
using System.Collections.Generic;

using DivideTargetsLibraryX64.FromGetPeaks;

namespace DivideTargetsLibraryX64.Combine
{
    public class CombineParameters
    {
        public string OutputPath { get; set; }

        public List<string> InputPaths { get; set; }
 
        public CombineParameters()
        {
            InputPaths = new List<string>();
            OutputPath = "";
        }

        public CombineParameters(string combineParameterFile):this()
        {
            
           StringLoadTextFileLine loadSpectraL = new StringLoadTextFileLine();
            List<string> linesFromParameterFile = new List<string>();
            List<string> parameterList = new List<string>();
            //load strings
            linesFromParameterFile = loadSpectraL.SingleFileByLine(combineParameterFile); //loads all isos

            foreach (string line in linesFromParameterFile)
            {
                char spliter = ',';
                string[] wordArray = line.Split(spliter);
                if (wordArray.Length == 2)
                    parameterList.Add(wordArray[1]);
            }

            if (parameterList.Count <2)
            {
                Console.WriteLine("Missing Parameters");
                Console.ReadKey();
            }

            OutputPath = parameterList[0];

            for (int i = 1; i < linesFromParameterFile.Count; i++)//skip headder
            {
                string line = linesFromParameterFile[i];
                char spliter = ',';
                string[] wordArray = line.Split(spliter);
                if (wordArray[0]=="Input") InputPaths.Add(wordArray[1]);
            }

        }
    }
}
