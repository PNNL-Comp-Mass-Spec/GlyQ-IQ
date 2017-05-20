using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Random;
using PNNLOmics.Data;

namespace IQGlyQ
{
    public static class HelloWorld
    {
        public static bool Check()
        {
            return true;
        } 

        public static bool CheckMathDotNetNumerics()
        {
            MathNet.Numerics.Random.Palf randomGenerator =new Palf();
            return true;
        }

        public static bool CheckAlgilib()
        {
            alglib.nearunityunit.nucosm1(1);
            return true;
        }

        public static bool CheckPNNLOmics()
        {
            PNNLOmics.Data.XYData test = new XYData(1,1);
            return true;
        }
    }
}
