using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Constants;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL.Functions
{

    public interface IConvert
    {
        List<decimal> TextToDecimal(List<string> textList);
        List<double> TextToDouble(List<string> textList);
        List<int> TextToInt(List<string> textList);
        double TextToMass(string sequence);
        List<double> XYDataToMass(List<XYData> XYlist);
        List<decimal> ListDoubleToListDecimal(List<double> doublesList);
        List<double> ListDecimalToListDouble(List<decimal> decimalList);
        List<double> FeatureViperToMass(List<FeatureAbstract> features);
        List<double> OmicsPeakToDouble(List<PNNLOmics.Data.Peak> peaks);
    }

    public class Converter : IConvert
    {
        #region Iterface for converting from a text list to a double, decimal or int

        List<decimal> IConvert.TextToDecimal(List<string> textList)
        {
            List<decimal> numberList = new List<decimal>();
            for (int i = 0; i < textList.Count; i++)
            {
                numberList.Add(decimal.Parse(textList[i]));
            }
            return numberList;
        }

        List<double> IConvert.TextToDouble(List<string> textList)
        {
            List<double> numberList = new List<double>();
            for (int i = 0; i < textList.Count; i++)
            {
                numberList.Add(double.Parse(textList[i]));
            }
            return numberList;
        }

        List<int> IConvert.TextToInt(List<string> textList)
        {
            List<int> numberList = new List<int>();
            for (int i = 0; i < textList.Count; i++)
            {
                numberList.Add(int.Parse(textList[i]));
            }
            return numberList;
        }

        double IConvert.TextToMass(string sequence)
        {
            double mass = 0;
            string aminoAcid;
            for (int i = 0; i < sequence.Length; i++)
            {
                aminoAcid = Convert.ToString(sequence[i]);
                mass += Constants.AminoAcids[aminoAcid].MassMonoIsotopic;
                //mass += AminoAcidConstantsTable.GetMass(sequence[i]);
            }
            mass += Constants.MiscellaneousMatter[MiscellaneousMatterName.Water].MassMonoIsotopic;
            return mass;
        }

        List<double> IConvert.XYDataToMass(List<XYData> XYlist)
        {
            List<double> massList = new List<double>();
            for (int i = 0; i < XYlist.Count; i++)
            {
                massList.Add(XYlist[i].X);
            }
            return massList;
        }

        List<decimal> IConvert.ListDoubleToListDecimal(List<double> doubleList)
        {
            List<decimal> decimalList = decimalList = doubleList.ConvertAll<decimal>(delegate(double str)
            {
                return (decimal)str;
            });

            return decimalList;
        }

        List<double> IConvert.ListDecimalToListDouble(List<decimal> decimalList)
        {
            List<double> doubleList = doubleList = decimalList.ConvertAll<double>(delegate(decimal str)
            {
                return (double)str;
            });

            return doubleList;
        }

        List<double> IConvert.FeatureViperToMass(List<FeatureAbstract> features)
        {
            List<double> doubleList = new List<double>();
            foreach (FeatureAbstract feature in features)
            {
                doubleList.Add(feature.UMCMonoMW);
            }

            return doubleList;
        }

        List<double> IConvert.OmicsPeakToDouble(List<PNNLOmics.Data.Peak> peakList)
        {
            List<double> doubleList = new List<double>();
            foreach (PNNLOmics.Data.Peak d in peakList)
            {
                doubleList.Add(d.XValue);
            }
            
            return doubleList;
        }
        #endregion
    }
}

