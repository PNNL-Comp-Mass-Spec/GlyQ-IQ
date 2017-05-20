using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Functions;
using PNNLOmics.Data.Constants;

namespace OmniFinder.Objects
{
    public class OmniFinderExactMassGenerator
    {
        public void CalculateExactMasses(List<decimal> buildingblockMasses, List<FactorialDesignFactor<int>> integerMatrix, out List<OmnifinderExactMassObject> exactMassesOutput)
        {
            exactMassesOutput = new List<OmnifinderExactMassObject>();

            //List<Decimal> exactMasses = new List<decimal>();
            List<string> compositionString = new List<string>();

            //create the summed mass lists and string of integers
            int rowsInMatrix = integerMatrix[0].FactorSet.Count();
            for (int j = 0; j < rowsInMatrix; j++)
            {
                OmnifinderExactMassObject newMass = new OmnifinderExactMassObject();
                newMass.MassExact = 0;
                exactMassesOutput.Add(newMass);
            }

            //create a matrix with masses instead of integers
            List<FactorialDesignFactor<decimal>> massMatrix = new List<FactorialDesignFactor<decimal>>();

            for (int i = 0; i < integerMatrix.Count; i++)
            {
                massMatrix.Add(new FactorialDesignFactor<decimal>());

                for (int j = 0; j < integerMatrix[i].FactorSet.Count; j++)
                {
                    massMatrix[i].FactorSet.Add(integerMatrix[i].FactorSet[j] * buildingblockMasses[i]);
                }
            }

            //calculate exact masses and compsition string
            for (int i = 0; i < integerMatrix.Count; i++)
            {
                for (int j = 0; j < integerMatrix[i].FactorSet.Count; j++)
                {
                    exactMassesOutput[j].MassExact += massMatrix[i].FactorSet[j];
                    exactMassesOutput[j].ListOfCompositions.Add(integerMatrix[i].FactorSet[j]);
                }
            }
        }
    }
}
