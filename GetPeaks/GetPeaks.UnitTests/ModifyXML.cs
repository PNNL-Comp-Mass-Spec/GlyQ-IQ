using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using System.Text.RegularExpressions;

///http://www.regular-expressions.info/reference.html/

namespace GetPeaks.UnitTests
{
    public class ModifyXML
    {
        public List<string> Load(string pubName, int serverNumber, string configFilePath, int lineNumberToChange)
        {
            StringLoadTextFileLine loader = new StringLoadTextFileLine();
            List<string> lines = new List<string>();
            lines = loader.SingleFileByLine(configFilePath);
            string lineToChange = "";
            lineToChange = lines[lineNumberToChange];

            string pattern = @"Pub-[0-9.]*-[0-9.]";
            var match = Regex.Match(lineToChange, pattern);
            

            string newPubString = pubName + "-" + serverNumber.ToString();
            string newline = Regex.Replace(lineToChange, pattern, newPubString);

            lines[lineNumberToChange] = newline;

            return lines;


        }
    }
}
