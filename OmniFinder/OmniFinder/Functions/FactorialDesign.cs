using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniFinder.Functions
{
    public class FactorialDesign
    {
        /// <summary>
        /// Where the Matrix is Stored
        /// </summary>
        public List<FactorialDesignFactor<int>> Matrix { get; set; }

        /// <summary>
        /// the numbers a factor is varied over.  the range produces a set of values
        /// </summary>
        private int Range { get; set; }

        /// <summary>
        /// how many times a set of values is repeated througout the factor
        /// </summary>
        private int NumberOfRepeats { get; set; }

        /// <summary>
        /// Create a factorial deisgn matrix of many integers
        /// </summary>
        /// <param name="numberOfFactors">how many columns it will have</param>
        /// <param name="rangeStart">ranges for each factor, starting range</param>
        /// <param name="rangeEnd">ranges for each factor, ending range</param>
        public void CreateIntegerMatrix(int numberOfFactors, List<int> rangeStart, List<int> rangeEnd)
        {
            //initialize variables
            int howManyRowsInTotal = 1;//default value
            int increment = 1; ;//default value=1 //how many times a value is repeated in a column
            int incrementPrevious = 1;//default value=1

            //calculate how many rows will be in the matrix
            for (int i = 0; i < numberOfFactors; i += 1)
            {
                Range = rangeEnd[i] - rangeStart[i] + 1;//+1 includes zero
                howManyRowsInTotal *= Range;
            }

            //Console.WriteLine("There are " + howManyRowsInTotal + " rows");

            //create an empty matrix
            List<FactorialDesignFactor<int>> matrix = new List<FactorialDesignFactor<int>>();
            for (int i = 0; i < numberOfFactors; i += 1)
            {
                matrix.Add(new FactorialDesignFactor<int>());
                for (int j = 0; j < howManyRowsInTotal; j++)
                {
                    matrix[i].FactorSet.Add(0);
                }
            }

            //fill the matrix, one factor at at time
            for (int i = 0; i < numberOfFactors; i += 1)
            {
                //calculate how many times the set is repeated in a factor
                NumberOfRepeats = howManyRowsInTotal;//initial condition
                for (int m = 0; m <= i; m++)//<= is needed so we enter the loop at i=0;
                {
                    Range = rangeEnd[m] - rangeStart[m] + 1;//+1 for including zero
                    NumberOfRepeats = NumberOfRepeats / Range;
                }

                //calculate the increment caused by repeating sets.  as we go to more factors, there is more incrementing
                if (i > 0)
                {
                    Range = rangeEnd[i - 1] - rangeStart[i - 1] + 1;//+1 for including zero//-1 for the range prior
                    increment = incrementPrevious * Range;
                }
                incrementPrevious = increment;

                //fill the matrix based on number of repeats, the set range, and the increment amount
                for (int j = 0; j < NumberOfRepeats; j++)
                {
                    Range = rangeEnd[i] - rangeStart[i] + 1;//+1 for including zero
                    for (int k = 0; k < Range; k++)
                    {
                        for (int n = 0; n < increment; n++)
                        {
                            //Console.WriteLine("I=" + i + " J=" + j + " K=" + k + " n=" + n);
                            int offset = j * (howManyRowsInTotal / NumberOfRepeats) + k * increment + n;
                            //Console.WriteLine("Offset=" + offset);
                            matrix[i].FactorSet[offset] = rangeStart[i] + k;
                        }
                    }
                }
            }
            Matrix = matrix;
        }


        public int GetSizeOfMatrixColumns()
        {
            int sizeOfMatrix = Matrix.Count;
            return sizeOfMatrix;
        }

        public int GetSizeOfMatrixRows()
        {
            int sizeOfMatrix = Matrix[0].FactorSet.Count;
            return sizeOfMatrix;
        }
    }
}



////create an empty matrix
//            List<List<int>> matrix = new List<List<int>>();
//            for (int i = 0; i < numberOfFactors; i += 1)
//            {
//                matrix.Add(new List<int>());
//                for (int j = 0; j < howManyRowsInTotal; j++)
//                {
//                    matrix[i].Add(0);
//                }
//            }

//            //fill the matrix, one factor at at time
//            for (int i = 0; i < numberOfFactors; i += 1)
//            {
//                //calculate how many times the set is repeated in a factor
//                NumberOfRepeats = howManyRowsInTotal;//initial condition
//                for (int m = 0; m <= i; m++)//<= is needed so we enter the loop at i=0;
//                {
//                    Range = rangeEnd[m] - rangeStart[m] + 1;//+1 for including zero
//                    NumberOfRepeats = NumberOfRepeats / Range;
//                }

//                //calculate the increment caused by repeating sets.  as we go to more factors, there is more incrementing
//                if (i > 0)
//                {
//                    Range = rangeEnd[i - 1] - rangeStart[i - 1] + 1;//+1 for including zero//-1 for the range prior
//                    increment = incrementPrevious * Range;
//                }
//                incrementPrevious = increment;

//                //fill the matrix based on number of repeats, the set range, and the increment amount
//                for (int j = 0; j < NumberOfRepeats; j++)
//                {
//                    Range = rangeEnd[i] - rangeStart[i] + 1;//+1 for including zero
//                    for (int k = 0; k < Range; k++)
//                    {
//                        for (int n = 0; n < increment; n++)
//                        {
//                            Console.WriteLine("I=" + i + " J=" + j + " K=" + k + " n=" + n);
//                            int offset = j * (howManyRowsInTotal / NumberOfRepeats) + k * increment + n;
//                            Console.WriteLine("Offset=" + offset);
//                            matrix[i][offset] = k;
//                        }
//                    }
//                }