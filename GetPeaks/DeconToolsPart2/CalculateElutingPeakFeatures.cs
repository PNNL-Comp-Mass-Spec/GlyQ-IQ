using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Go_Decon_Modules;
using DeconToolsPart2;

namespace GetPeaks_DLL
{
    public class CalculateElutingPeakFeatures
    {
        public int SingleThread(InputOutputFileName newFile, List<ElutingPeakOmics> discoveredOmicsPeaks, SimpleWorkflowParameters Parameters)
        {
            Object databaseLockMulti = new Object();//global lock for database
            GoTransformParameters transformerParameterSetup = new GoTransformParameters();
            HornTransformParameters hornParameters = new HornTransformParameters();
            transformerParameterSetup.Parameters = hornParameters;
            GoTransformPrep setupTransformers = new GoTransformPrep();
            List<TransformerObject> transformerList = setupTransformers.PreparArmyOfTransformers(transformerParameterSetup, 1, newFile, databaseLockMulti);
            int dataFileIterator = 0;
            int elutingPeakTotal = 0;
            int counter = 0;
            int hits = 0;
            int totalCount = discoveredOmicsPeaks.Count;

            TransformerObject transformerMulti = new TransformerObject();// = transformer2;
            transformerMulti = transformerList[0];


            foreach (ElutingPeakOmics ePeak in discoveredOmicsPeaks)
            {

                Console.WriteLine("new ePeak " + ePeak.Mass + "Peak " + counter + " out of " + totalCount + " with " + hits + " hits");
                counter++;

                ElutingPeakFinderPart2 EPPart2 = new ElutingPeakFinderPart2();
                //SimpleWorkflowParameters newpar = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
                //ElutingPeakOmics ThreadEPeak = ObjectCopier.Clone<ElutingPeakOmics>(ePeak);
                SimpleWorkflowParameters paralellParameters = new SimpleWorkflowParameters();
                paralellParameters = GetPeaks_DLL.ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
                List<ElutingPeakOmics> newSingleList = new List<ElutingPeakOmics>(0);
                newSingleList.Add(ePeak);
                int elutingpeakHits = 0;
                //EPPart2.SimpleWorkflowExecutePart2e(newSingleList, newFile.InputFileName, newFile.OutputSQLFileName, ref elutingpeakHits);

                EPPart2.SimpleWorkflowExecutePart2Memory(newSingleList, newFile, paralellParameters, ref elutingpeakHits, transformerMulti, ref dataFileIterator);

                if (elutingpeakHits > 0)
                {
                    hits++;
                }

                paralellParameters.Dispose();
                paralellParameters = null;
                //TODO memory leak.  perhaps need idisposable on single list inside
                newSingleList[0].Dispose();
                newSingleList = null;
                elutingPeakTotal += elutingpeakHits;

                EPPart2.Dispose();

            }
            return hits;
        }
    }
}
