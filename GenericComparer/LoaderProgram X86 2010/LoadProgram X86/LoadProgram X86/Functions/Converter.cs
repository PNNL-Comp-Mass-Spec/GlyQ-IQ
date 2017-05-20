using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Constants.ConstantsDataUtilities;

namespace ConsoleApplication1
{
    public interface IConvert
    {
        void TextToDecimal(List<string> textList, List<decimal> numberList);
        void TextToDouble(List<string> textList, List<double> numberList);
        void TextToInt(List<string> textList, List<int> numberList);
        void TextToMass(string sequence, double mass);
        void XYDataToMass(List<XYData> XYlist, List<double> massList);
    }

    public class Converter : IConvert
    {
        public void ConvertXYToFeature(List<XYData> XYList, List<FeatureSynthetic> featureList)
        {
            for (int i = 0; i < XYList.Count; i++)
            {
                FeatureSynthetic featureS = new FeatureSynthetic();
                featureS.MassMonoisotopicD = (decimal)XYList[i].X;
                featureS.AbundanceD = (double)XYList[i].Y;
                featureS.ID = i;
                featureList.Add(featureS);
            }    
            return;
        }
    
    
        #region Iterface for converting from a text list to a double, decimal or int
            void IConvert.TextToDecimal(List<string> textList, List<decimal> numberList)
            {
                for (int i = 0; i < textList.Count; i++)
                {
                    numberList.Add(decimal.Parse(textList[i]));
                }
            }

            void IConvert.TextToDouble(List<string> textList, List<double> numberList)
            {
                for (int i = 0; i < textList.Count; i++)
                {
                    numberList.Add(double.Parse(textList[i]));
                }
            }
            void IConvert.TextToInt(List<string> textList, List<int> numberList)
            {
                for (int i = 0; i < textList.Count; i++)
                {
                    numberList.Add(int.Parse(textList[i]));
                }
            }
            void IConvert.TextToMass(string sequence, double mass)
            {
                mass = 0;
                for (int i = 0; i < sequence.Length; i++)
                {
                    mass += AminoAcidStaticLibrary.GetMonoisotopicMass(sequence[i]);
                }
            }
            void IConvert.XYDataToMass(List<XYData> XYlist, List<double> massList)
            {
                for (int i=0; i<XYlist.Count; i++)
                {
                    massList.Add(XYlist[i].X);
                }
            }
        #endregion
    }
}
