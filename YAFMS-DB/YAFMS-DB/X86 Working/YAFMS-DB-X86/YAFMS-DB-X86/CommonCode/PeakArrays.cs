using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YAFMS_DB
{
    public class PeakArrays
    {
        private int sizeOfArrays { get; set; }

        public double[] MzArray { get; set; }
        public double[] IntensityArray { get; set; }
        public double[] WidthArray { get; set; }
        public int[] ScanArray { get; set; }

        public PeakArrays()
        {
            MzArray = new double[0];
            IntensityArray = new double[0];
            WidthArray = new double[0];
            ScanArray = new int[0];
        }

        public PeakArrays(int size, EnumerationPeaksArrays dimensions )
        {
            sizeOfArrays = size;

            switch (dimensions)
            {
                    case EnumerationPeaksArrays.Peak:
                    {
                        MzArray = new double[sizeOfArrays];
                        IntensityArray = new double[sizeOfArrays];
                        WidthArray = new double[sizeOfArrays];
                        ScanArray = new int[sizeOfArrays];
                    }
                    break;
                    case EnumerationPeaksArrays.LC:
                    {
                        MzArray = new double[0];
                        IntensityArray = new double[sizeOfArrays];
                        WidthArray = new double[0];
                        ScanArray = new int[sizeOfArrays];
                    }
                    break;
                    case EnumerationPeaksArrays.MS:
                    {
                        MzArray = new double[sizeOfArrays];
                        IntensityArray = new double[sizeOfArrays];
                        WidthArray = new double[sizeOfArrays];
                        ScanArray = new int[0];
                    }
                    break;
            }

        }
    }
}
