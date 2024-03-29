﻿using System.Collections.Generic;
using System.Linq;
using Run64.Utilities;

namespace Run64.Backend.Core
{
    public class ChromCorrelationData
    {

        #region Constructors
        public ChromCorrelationData()
        {
            CorrelationDataItems = new List<ChromCorrelationDataItem>();
        }
        #endregion

    

        #region Properties

        public List<ChromCorrelationDataItem> CorrelationDataItems { get; set; }

        public double? RSquaredValsMedian
        {
            get
            {
                var validItems = CorrelationDataItems.Select(p => p.CorrelationRSquaredVal).Where(n => n.HasValue);

                if (validItems.Any())
                {
                    return MathUtils.GetMedian(validItems.Select(r=>r.GetValueOrDefault()).ToList());  
                }
                return null;
            }
        }
        
        public double? RSquaredValsAverage
        {
            get
            {
                var validItems = CorrelationDataItems.Select(p => p.CorrelationRSquaredVal).Where(n => n.HasValue);

                if (validItems.Any())
                {
                    return validItems.Average(p=>p.Value);  
                }
                return null;
            }
        }

        public double? RSquaredValsStDev
        {
            get
            {
                var validItems = CorrelationDataItems.Select(p => p.CorrelationRSquaredVal).Where(n => n.HasValue).ToList();

                if (validItems.Count > 2)
                {
                    return MathUtils.GetStDev(validItems.Select(p=>p.GetValueOrDefault()).ToList()); 
                }
                return null;
            }
        }

        #endregion

        #region Public Methods
        
        public void AddCorrelationData(double correlationSlope, double correlationIntercept, double correlationRSquaredVal)
        {
            ChromCorrelationDataItem data = new ChromCorrelationDataItem(correlationSlope,correlationIntercept,correlationRSquaredVal);
            CorrelationDataItems.Add(data);
        }

        public void AddCorrelationData(ChromCorrelationDataItem chromCorrelationDataItem)
        {
            CorrelationDataItems.Add(chromCorrelationDataItem);
        }

        

        #endregion

        #region Private Methods

        #endregion

    }
}
