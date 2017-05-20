using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL;

namespace GetPeaks_DLL.ConosleUtilities
{
    public class ConvertAToB
    {
        /// <summary>
        /// convert a string from the parameter file to the internal enummeration of deconvolution type
        /// </summary>
        /// <param name="DeconType"></param>
        /// <returns></returns>
        public DeconvolutionType stringTODeconvolutionType(string DeconType)
        {
            DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
            switch (DeconType)
            {
                case "Thrash":
                    {
                        loadedDeconvolutionType = DeconvolutionType.Thrash;
                    }
                    break;
                case "Rapid":
                    {
                        loadedDeconvolutionType = DeconvolutionType.Rapid;
                    }
                    break;
                default:
                    {
                        Console.WriteLine("Missing Decon Type.  Use Thrash or Rapid");
                        Console.ReadKey();
                    }
                    break;
            }
            return loadedDeconvolutionType;
        }
    }
}
