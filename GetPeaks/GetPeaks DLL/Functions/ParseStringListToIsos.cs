using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.Functions
{
    public class ParseStringListToIsos
    {
        public void Parse(List<string> stringList, FileIterator.deliminator textDeliminator, out List<IsosObject> outputIsosList, out string columnHeadders)
        {
            int startline = 1;//0 is the headder

            outputIsosList = new  List<IsosObject>();

            string[] wordArray;

            int scan_num;
            int charge;
            double abundance;
            double mz;
            float fit;
            double average_mw;
            double monoisotopic_mw;
            double mostabundant_mw;
            float fwhm;
            float signal_noise;
            double mono_abundance;
            double mono_plus2_abundance;
            int flag;
            float interference_score;


            char spliter;
            switch (textDeliminator)
            {
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma:
                    {
                        spliter = ',';
                    }
                    break;
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab:
                    {
                        spliter = '\t';
                    }
                    break;
                default:
                    {
                        spliter = ',';
                    }
                    break;
            }

            columnHeadders = stringList[0];
            int length = stringList.Count;
            for (int i = startline; i < length; i++)//i=0 is the headder
            {          
                string line = stringList[i];
                
                IsosObject newIsos = new IsosObject();

                wordArray = line.Split(spliter);

                //Tryparse is best and should be fastest
                int.TryParse(wordArray[0], out scan_num);
                int.TryParse(wordArray[1], out charge);
                double.TryParse(wordArray[2], out abundance);
                double.TryParse(wordArray[3], out mz);
                float.TryParse(wordArray[4], out fit);
                double.TryParse(wordArray[5], out average_mw);
                double.TryParse(wordArray[6], out monoisotopic_mw);
                double.TryParse(wordArray[7], out mostabundant_mw);
                float.TryParse(wordArray[8], out fwhm);
                float.TryParse(wordArray[9], out signal_noise);
                double.TryParse(wordArray[10], out mono_abundance);
                double.TryParse(wordArray[11], out mono_plus2_abundance);
                int.TryParse(wordArray[12], out flag);
                float.TryParse(wordArray[13], out interference_score);

                newIsos.scan_num = scan_num;
                newIsos.charge = charge;
                newIsos.abundance = abundance;
                newIsos.mz = mz;
                newIsos.fit = fit;
                newIsos.average_mw = average_mw;
                newIsos.monoisotopic_mw = monoisotopic_mw;
                newIsos.mostabundant_mw = mostabundant_mw;
                newIsos.fwhm = fwhm;
                newIsos.signal_noise = signal_noise;
                newIsos.mono_abundance = mono_abundance;
                newIsos.mono_plus2_abundance = mono_plus2_abundance;
                newIsos.flag = flag;
                newIsos.interference_score = interference_score;

                outputIsosList.Add(newIsos);
            }
        }
    }
}

            //PeptideTarget mt = new PeptideTarget();
            //mt.ChargeState = (short)parseIntField(getValue(new string[] { "z", "charge_state" }, lineData, "0"));

            //mt.ID = parseIntField(getValue(new string[] { "id", "mass_tag_id", "massTagid" }, lineData, "-1"));
            //mt.Code = getValue(new string[] { "peptide", "sequence" }, lineData, "");
            //mt.NormalizedElutionTime = parseFloatField(getValue(new string[] { "net", "avg_ganet" }, lineData, "-1"));
            //mt.ObsCount = parseIntField(getValue(new string[] { "obs", "obscount" }, lineData, "-1"));
            //mt.MonoIsotopicMass = parseDoubleField(getValue(new string[] { "mass", "monoisotopicmass", "monoisotopic_mass" }, lineData, "0"));
            //mt.EmpiricalFormula = getValue(new string[] { "formula", "empirical_formula" }, lineData, "");
            //mt.ModCount = parseShortField(getValue(new string[] { "modCount", "mod_count" }, lineData, "0"));
            //mt.ModDescription = getValue(new string[] { "mod", "mod_description" }, lineData, "");