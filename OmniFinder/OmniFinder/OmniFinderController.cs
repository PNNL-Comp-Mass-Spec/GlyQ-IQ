using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects;
using PNNLOmics.Data.Constants;
using OmniFinder.Functions;

namespace OmniFinder
{
    public class OmniFinderController
    {
        public OmniFinderOutput FindCompositions(OmniFinderParameters parameters)
        {
            OmniFinderOutput results = new OmniFinderOutput();

            //convert parameters to Lists
            List<decimal> buildingblockMasses;
            List<string> buidingblockNames;
            List<int> rangeStart;
            List<int> rangeEnd;
            Dictionary<ElementName, int> compoundElementComposition;

            ConvertParameters converter = new ConvertParameters();
            converter.ConvertBuildingBlockEnumerationsToValues(parameters, out buildingblockMasses, out buidingblockNames, out rangeStart, out rangeEnd, out compoundElementComposition);

            //create integer matrix
            List<FactorialDesignFactor<int>> integerMatrix = new List<FactorialDesignFactor<int>>();
            FactorialDesign newDesigner = new FactorialDesign();
            newDesigner.CreateIntegerMatrix(buildingblockMasses.Count, rangeStart, rangeEnd);
            integerMatrix = newDesigner.Matrix;

            //convert integer matrix to Exact Mass objects
            OmniFinderExactMassGenerator newGenerator = new OmniFinderExactMassGenerator();
            List<OmnifinderExactMassObject> exactMasses;
            newGenerator.CalculateExactMasses(buildingblockMasses, integerMatrix, out exactMasses);

            //add adducts and carb types
            MiscellaneousMatterName convertCarbTypeToConstants = ConvertCarbTypeAndAdduct.SetCarbType(parameters, ref compoundElementComposition);

            decimal adductMass = ConvertCarbTypeAndAdduct.SetAdductMass(parameters, ref compoundElementComposition);

            foreach (OmnifinderExactMassObject massObject in exactMasses)
            {
                massObject.MassExact += Convert.ToDecimal(Constants.MiscellaneousMatter[convertCarbTypeToConstants].MassMonoIsotopic);
                massObject.MassExact += adductMass;
            }

            results.CarbType = parameters.CarbohydrateType;
            results.Adduct = parameters.ChargeCarryingAdduct;
            results.MassAndComposition = exactMasses;
            results.CompositionHeaders = buidingblockNames;
            return results;
        }

        
    }
}
