using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sipper.Model
{
    public class ResultFilterCriteria
    {

        #region Constructors
        public ResultFilterCriteria()
        {
            AreaUnderRatioCurveRevisedMin = double.Epsilon;
            AreaUnderRatioCurveRevisedMax = 1e10;  //set super high
            ChromCorrelationAverageMin = 0.9;
            ChromCorrelationAverageMax = 1.1;    //set to above 1.0
            ChromCorrelationMedianMin = 0.9;
            ChromCorrelationMedianMax = 1.1;    //set to above 1.0
            IScoreMin = 0;
            IScoreMax = 0.15;
            RSquaredValForRatioCurveMin = 0.925;
            RSquaredValForRatioCurveMax = 1.1;  //above 1.0



        }


        public static ResultFilterCriteria GetFilterScheme1()
        {
            ResultFilterCriteria criteria=new ResultFilterCriteria();
            criteria.AreaUnderRatioCurveRevisedMin = double.Epsilon;
            criteria.AreaUnderRatioCurveRevisedMax = 1e10;  //set super high
            criteria.ChromCorrelationAverageMin = 0.9;
            criteria.ChromCorrelationAverageMax = 1.1;    //set to above 1.0
            criteria.ChromCorrelationMedianMin = 0.9;
            criteria.ChromCorrelationMedianMax = 1.1;    //set to above 1.0
            criteria.IScoreMin = 0;
            criteria.IScoreMax = 0.15;
            criteria.RSquaredValForRatioCurveMin = 0.925;
            criteria.RSquaredValForRatioCurveMax = 1.1;  //above 1.0

            return criteria;
        }


        #endregion

        #region Properties

        public double AreaUnderRatioCurveRevisedMin { get; set; }
        public double AreaUnderRatioCurveRevisedMax { get; set; }

        public double ChromCorrelationAverageMin { get; set; }
        public double ChromCorrelationAverageMax { get; set; }

        public double ChromCorrelationMedianMin { get; set; }
        public double ChromCorrelationMedianMax { get; set; }

        public double IScoreMin { get; set; }
        public double IScoreMax { get; set; }

        public double RSquaredValForRatioCurveMin { get; set; }
        public double RSquaredValForRatioCurveMax { get; set; }

        #endregion

    

    }
}
