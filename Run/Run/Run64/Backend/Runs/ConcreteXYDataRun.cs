using Run64.Backend.Core;
using Run64.Backend.Data;

namespace Run64.Backend.Runs
{
    public class ConcreteXYDataRun:XYDataRun
    {

        public ConcreteXYDataRun(double[]xvals, double[] yvals)
        {
            this.XYData.Xvalues = xvals;
            this.XYData.Yvalues = yvals;

        }


        
        public override double GetTime(int scanNum)
        {
            return -1;
        }


        public override XYData GetMassSpectrum(ScanSet scanset)
        {
            return XYData;
        }

  
    }
}
