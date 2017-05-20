using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiDatasetToolBox
{
    public class LinearRegressionResult
    {
        public string DatasetName { get; set; }

        public double Slope { get; set; }

        public double Intercept { get; set; }

        public LinearRegressionResult()
        {
        }

        public LinearRegressionResult(string datasetName, double slope, double intercept)
        {
            DatasetName = datasetName;
            Slope = slope;
            Intercept = intercept;
        }
    }
}
