using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public static class GoAssignTransform
    {
        public static TransformerObject Assign(int MaxThreadCount, List<TransformerObject> transformerList, ref int threadToggle)
        {

            int activeTransformers = 0;
            foreach (TransformerObject tObject in transformerList)
            {
                if (tObject.active == true)
                {
                    activeTransformers++;
                }
            }
            Console.WriteLine("there are " + activeTransformers + " present");

            //new stuff
            TransformerObject transformerMulti;
            int counter = 0;
            bool assigned = false;
            while (assigned == false)
            {
                if (transformerList[counter].active == false)
                {
                    transformerList[counter].active = true;
                    transformerMulti = transformerList[counter];
                    counter = 0;
                    return transformerMulti;
                }
                else
                {
                    counter++;
                }
            }
            //fail senario
            transformerMulti = transformerList[transformerList.Count-1];
            Console.WriteLine("not enough threads");
            Console.ReadKey();
            //possible fail safe is to add a new transformer to the list and use it
            return transformerMulti;
        }

    }
}
