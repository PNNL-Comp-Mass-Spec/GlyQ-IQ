using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.TandemMSObjects;
using GetPeaks_DLL.Go_Decon_Modules;

namespace GetPeaks_DLL.TandemSupport
{
    public static class GenerateTandemObjects
    {
        public static List<TandemObject> WithScans(InputOutputFileName newFile, List<int> scanLevels, List<int> scanLevelsWithTandem, ParametersTHRASH parametersThrash)
        {
            List<TandemObject> collectedObjects = new List<TandemObject>();

            for (int i = 0; i < scanLevelsWithTandem.Count; i++)
            {
                //Console.WriteLine("Generating...");
                //for each 2 in the scan list
                int j = 1;//needed to move from precursor=1 to fragment spectra=2
                int currentScanLevel = scanLevelsWithTandem[i];
                int index = currentScanLevel;

                int check = scanLevels[currentScanLevel + j];

                while (check > 1 && index < scanLevels.Count - 1)
                {
                    TandemObject newTandemObject = new TandemObject(newFile, currentScanLevel, parametersThrash);

                    newTandemObject.FragmentationScanNumber = currentScanLevel + j;
                    newTandemObject.FragmentationMSLevel = scanLevels[currentScanLevel + j];

                    collectedObjects.Add(newTandemObject);
                    j++;

                    index = currentScanLevel + j;
                    check = scanLevels[scanLevelsWithTandem[i] + j];
                }
            }

            return collectedObjects;
        }
    }
}
