using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibrary;
using DivideTargetsLibrary.Parameters;

namespace DivideTargetsNodes
{
    public class ParametersNodes
    {
        
        public string ResultsLocation  { get; set; }
        public string LaunchHomeLocation { get; set; }
        public string PexecLocation { get; set; }
        public string SleepExeLocation { get; set; }
        public string RunMeSeccond100XFileName { get; set; }
        public string NodeShareFolderLocationOnNode { get; set; }
        public string NodeShareFolder { get; set; }
        public string AdminAccount { get; set; }
        public string AdminPassword { get; set; }
        
        public List<string> NodeLibraries { get; set; }
        public List<Tuple<string,string>> NodeConnections  { get; set; }
        public int Nodes { get; set; }


        public ParameterDivideTargets divideTargetParameters { get; set; }

        public ParametersNodes()
        {
            NodeConnections = new List<Tuple<string, string>>();
            NodeLibraries = new List<string>();
        }

        public void SetParameters(string divideTargetsParameterFile)
        {
            if(NodeConnections==null)
            {
                NodeConnections = new List<Tuple<string, string>>();
            }

            StringLoadTextFileLine loadSpectraL = new StringLoadTextFileLine();
            List<string> linesFromParameterFile = new List<string>();
            List<string> parameterList = new List<string>();
            //load strings
            linesFromParameterFile = loadSpectraL.SingleFileByLine(divideTargetsParameterFile); //loads all isos

            foreach (string line in linesFromParameterFile)
            {
                char spliter = ',';
                string[] wordArray = line.Split(spliter);
                if (wordArray.Length == 2)
                    parameterList.Add(wordArray[1]);
            }

            if (parameterList.Count < 8)
            {
                Console.WriteLine("Missing Parameters");
                Console.ReadKey();
            }


            ResultsLocation = parameterList[0];
            LaunchHomeLocation = parameterList[1];
            PexecLocation = parameterList[2];
            SleepExeLocation = parameterList[3];
            RunMeSeccond100XFileName = parameterList[4];
            NodeShareFolderLocationOnNode = parameterList[5];
            NodeShareFolder = parameterList[6];
            AdminAccount = parameterList[7];
            AdminPassword = parameterList[8];

            for (int i = 9; i < linesFromParameterFile.Count; i++)
            {
                string line = linesFromParameterFile[i];
                char spliter = ';';
                string[] wordArray = line.Split(spliter);

                string name = "";
                string ip = "";
                if (wordArray.Length == 2)
                {
                    name = wordArray[0];
                    ip = wordArray[1];
                }

                Tuple<string, string> node = new Tuple<string, string>(name,ip);

                NodeConnections.Add(node);
                NodeLibraries.Add("");
            }

            Nodes = NodeLibraries.Count;
        }
    }


}
