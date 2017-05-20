using System.Collections.Generic;
using System;

namespace YAFMS_DB.PNNLOmics
{
    public class XYData: BaseData
    {                
        public XYData(double newX, double newY)
        {
            X = newX;
            Y = newY;
        }

        public double X
        {
            get;
            set;
        }
        public double Y
        {
            get;
            set;
        }
        public override void Clear()
        {
            X = 0;
            Y = 0;
        }
        public static List<XYData> Bin(List<XYData> data, double binSize)
        {            
            double lowMass       = data[0].X;
            double highMass      = data[data.Count - 1].X;
            return Bin(data, lowMass, highMass, binSize);
        }

        public  static List<XYData> Bin(List<XYData> data, double lowMass, double highMass, double binSize)
        {
            List<XYData> newData = new List<XYData>();
            int total            = Convert.ToInt32((highMass - lowMass)/binSize);

            for (int i = 0; i < total; i++)
            {
                XYData part = new XYData(lowMass * i, 0.0);
                newData.Add(part);
            }

            for (int i = 0; i < data.Count; i++)
            {
                double intensity = data[i].Y;
                int bin = Math.Min(total - 1, System.Convert.ToInt32((data[i].X - lowMass) / binSize));
                try
                {
                    newData[bin].Y += intensity;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return newData;
        }
    }
}