using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Objects
{
    public class XYDataInt64 : BaseData
    {
        public XYDataInt64(Int64 newX, double newY)
        {
            this.X = newX;
            this.Y = newY;
        }
        public Int64 X { get; set; }
        public double Y { get; set; }

        public override void Clear()
        {
            this.X = 0;
            this.Y = 0;
        }
    }
}
