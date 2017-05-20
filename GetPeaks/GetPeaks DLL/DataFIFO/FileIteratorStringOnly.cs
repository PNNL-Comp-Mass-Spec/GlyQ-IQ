using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.DataFIFO
{
    public class FileIteratorStringOnly
    {
        public List<string> loadStrings(string filename)
		
        {
            Console.WriteLine("Load strings\n");

            List<string> stringProject = new List<string>();

            
            //Instantiate classes so they are not inside the loop;
            StringLoadTextFileLine loadSpectraL = null;

            Console.WriteLine("Load file: " + filename + "\n");
                
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFile = new List<string>();

            #region load data and parse it

            //load strings
            stringListFromFile = loadSpectraL.SingleFileByLine(filename);//loads strings

            //parse
            List<string> parametersString = new List<string>();
            parametersString = this.ParseToStringXword(stringListFromFile, deliminator.Comma, 1);

           #endregion


            return parametersString;
        }

        #region private Methods
       
        


        private List<string> ParseToStringXword(List<string> stringList, deliminator textDeliminator, int selectedWord)
        {
            List<string> outputList = new List<string>();

            string[] wordArray;

            string word = "";
            
            foreach (string line in stringList)
            {

                switch (textDeliminator)
                {
                    case deliminator.Comma:
                        {
                            wordArray = line.Split(',');
                        }
                        break;
                    case deliminator.Tab:
                        {
                            wordArray = line.Split('\t');

                        }
                        break;
                    default:
                        {
                            wordArray = line.Split(',');
                        }
                        break;
                }


                word = wordArray[selectedWord];
                outputList.Add(word);
            }
            return outputList;
        }

        #endregion

        public enum deliminator
        {
            Comma,
            Tab
        }
    }
}
