using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiDatasetToolBox
{
    public static class TransposeLines
    {
        public static List<string> SwapRowsAndColumns(List<string> datasetWithCode, string seperator)
        {

            int columns = datasetWithCode[0].Split(seperator.ToCharArray()).Count();
            int rows = datasetWithCode.Count;

            //set up memory once
            string[] existingRow = new string[2];
            string[] joinerTranspose = new string[columns];

            for (int row = 0; row < rows; row++)
            {
                string[] words = datasetWithCode[row].Split(seperator.ToCharArray());

                for (int column = 0; column < columns; column++)
                {
                    existingRow[0] = joinerTranspose[column];
                    existingRow[1] = words[column];

                    if (row == 0) //so we start with a value rather than a seperator.. parse to words one time
                    {
                        joinerTranspose[column] = words[column];
                    }
                    else
                    {
                        joinerTranspose[column] = string.Join(seperator, existingRow);
                    }
                }
            }

            return joinerTranspose.ToList(); ;
        }
    }
}
