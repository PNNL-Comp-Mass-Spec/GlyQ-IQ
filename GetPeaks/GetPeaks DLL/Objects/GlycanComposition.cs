using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;

namespace GetPeaks_DLL.Objects
{
    public class GlycanComposition
    {
        public Dictionary<MonosaccharideName, int> Compositions { get; set; }

        public Dictionary<MonosaccharideName, bool> Connetions { get; set; }

        public List<MonosaccharideName> Monosaccharides { get; set; }

        public bool Contains1Connection { get; set; }

        public GlycanComposition(List<MonosaccharideName> monosaccharideNames)
        {
            Monosaccharides = monosaccharideNames;
            Compositions = new Dictionary<MonosaccharideName, int>();
            foreach (var monosaccharideName in monosaccharideNames)
            {
                Compositions.Add(monosaccharideName, 0);
            }
            Connetions = new Dictionary<MonosaccharideName, bool>();
            foreach (var monosaccharideName in monosaccharideNames)
            {
                Connetions.Add(monosaccharideName,false);
            }
            Contains1Connection = false;
        }

        public GlycanComposition(List<MonosaccharideName> monosaccharideNames, List<int> quantities)
            : this(monosaccharideNames)
        {
            Compositions = SetCompositionHNFS(Monosaccharides, quantities);
        }



        public static Dictionary<MonosaccharideName, int> SetCompositionHNFS(List<MonosaccharideName> monosaccharideNames, List<int> quantities )
        {
            Dictionary<MonosaccharideName, int> GlycanComposition = new Dictionary<PNNLOmics.Data.Constants.MonosaccharideName, int>();
            for (int i = 0; i < monosaccharideNames.Count;i++ )
            {
                GlycanComposition.Add(monosaccharideNames[i],quantities[i]);
            }
            return GlycanComposition;
        }

        public void ResetConnections()
        {
            List<MonosaccharideName> keys = Connetions.Keys.ToList();
            
            for (int i = 0; i < Connetions.Count;i++ )
            {
                Connetions[keys[i]] = false;
            }
            Contains1Connection = false;
        }
    }

    public static class GlycanCompositionUtilities
    {
        public static void SingleLinkCompositions(ref GlycanComposition baseGlycan, GlycanComposition glycanToCompareWith)
        {
            var keys = baseGlycan.Compositions.Keys.ToList();
            int sumTestSquared = 0;//this is so all monosacharides need to add up to 1 and no more so 5401 and 5402 (=1) passes and 5401 and 5502 does not (=2) 
            for(int i=0;i<baseGlycan.Compositions.Count;i++)
            {
                MonosaccharideName testName = keys[i];
                int test = baseGlycan.Compositions[testName] - glycanToCompareWith.Compositions[testName];
                int testSquared = test*test;

                sumTestSquared += testSquared;

            }

            if(sumTestSquared==1)
            {
                baseGlycan.Contains1Connection = true;

                //set the individual hit
                for (int i = 0; i < baseGlycan.Compositions.Count; i++)
                {
                    MonosaccharideName testName = keys[i];
                    int test = baseGlycan.Compositions[testName] - glycanToCompareWith.Compositions[testName];
                    int testSquared = test * test;
                    if (testSquared == 1)
                    {
                        baseGlycan.Connetions[testName] = true;
                    }
                }
            }
        }
    }
}
