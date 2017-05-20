using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DivideTargetsLibraryX64
{
    public static class Converter
    {
        public static char[] ConvertEnding(string dataFileEnding)
        {
            char[] endingInCharData = new char[dataFileEnding.Count()];
            for (int j = 0; j < dataFileEnding.Length; j++)
            {
                endingInCharData[j] = dataFileEnding[j];
            }
            return endingInCharData;
        }

        public static int ConvertStringToInt(string coresString)
        {
            CultureInfo myCultureInfo = new CultureInfo("en-GB");
            double coresDouble = 0;
            Double.TryParse(coresString, System.Globalization.NumberStyles.Integer, myCultureInfo, out coresDouble);

            int cores = (int)coresDouble;
            return cores;
        }

        //public static List<int> ConvertStringGlycanCodeToIntegers32000(string code)
        //{
        //    //Hex:HexNAc:Fucose:SialicAcid
        //    //HH:N:F:SS  102124

        //    List<int> compositions = new List<int>();

        //    int codeAsInt = Convert.ToInt32(code);
        //    double codeAsDouble = Convert.ToDouble(codeAsInt);

        //    int preHexoseFactor = 10000;
        //    double preHexose = codeAsDouble / preHexoseFactor;

        //    int hexose = Convert.ToInt32(Math.Truncate(preHexose));
        //    compositions.Add(hexose);

        //    int preHexNAcFactor = 1000;
        //    double preHexNAc = (codeAsInt - hexose * preHexoseFactor) / preHexNAcFactor;

        //    int hexNAc = Convert.ToInt32(Math.Truncate(preHexNAc));

        //    compositions.Add(hexNAc);

        //    int preFucoseFactor = 100;
        //    double preFucose = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor) / preFucoseFactor;

        //    int fucose = Convert.ToInt32(Math.Truncate(preFucose));
        //    compositions.Add(fucose);

        //    int preSialicAcidFactor = 1;
        //    double preSialicAcid = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor - fucose * preFucoseFactor) / preSialicAcidFactor;

        //    int sialicAcid = Convert.ToInt32(Math.Truncate(preSialicAcid));
        //    compositions.Add(sialicAcid);

        //    return compositions;
        //}

        //public static List<int> ConvertStringGlycanCodeToIntegers320000(string code)
        //{
        //    //Hex:HexNAc:Fucose:SialicAcid
        //    //HH:N:F:SS  102124

        //    List<int> compositions = new List<int>();

        //    int codeAsInt = Convert.ToInt32(code);
        //    double codeAsDouble = Convert.ToDouble(codeAsInt);

        //    int preHexoseFactor = 100000;
        //    double preHexose = codeAsDouble / preHexoseFactor;
        //    int hexose = Convert.ToInt32(Math.Truncate(preHexose));
        //    compositions.Add(hexose);

        //    int preHexNAcFactor = 10000;
        //    double preHexNAc = (codeAsInt - hexose * preHexoseFactor) / preHexNAcFactor;
        //    int hexNAc = Convert.ToInt32(Math.Truncate(preHexNAc));
        //    compositions.Add(hexNAc);

        //    int preFucoseFactor = 1000;
        //    double preFucose = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor) / preFucoseFactor;
        //    int fucose = Convert.ToInt32(Math.Truncate(preFucose));
        //    compositions.Add(fucose);

        //    int preSialicAcidFactor = 10;
        //    double preSialicAcid = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor - fucose * preFucoseFactor) / preSialicAcidFactor;
        //    int sialicAcid = Convert.ToInt32(Math.Truncate(preSialicAcid));
        //    compositions.Add(sialicAcid);

        //    int preLactoneFactor = 1;
        //    double preLactone = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor - fucose * preFucoseFactor - sialicAcid * preSialicAcidFactor) / preLactoneFactor;
        //    int lactone = Convert.ToInt32(Math.Truncate(preLactone));
        //    compositions.Add(lactone);

        //    return compositions;
        //}

        //public static List<int> ConvertStringGlycanCodeToIntegers0302000000(string code)                                                         
        //{
        //    //Hex:HexNAc:Fucose:SialicAcid
        //    //HH:N:F:SS  102124

        //    List<int> compositions = new List<int>();

        //    int codeAsInt = Convert.ToInt32(code);
        //    double codeAsDouble = Convert.ToDouble(codeAsInt);

        //    int preHexoseFactor = 100000000;
        //    double preHexose = codeAsDouble / preHexoseFactor;
        //    int hexose = Convert.ToInt32(Math.Truncate(preHexose));
        //    compositions.Add(hexose);

        //    int preHexNAcFactor = 1000000;
        //    double preHexNAc = (codeAsInt - hexose * preHexoseFactor) / preHexNAcFactor;
        //    int hexNAc = Convert.ToInt32(Math.Truncate(preHexNAc));
        //    compositions.Add(hexNAc);

        //    int preFucoseFactor = 10000;
        //    double preFucose = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor) / preFucoseFactor;
        //    int fucose = Convert.ToInt32(Math.Truncate(preFucose));
        //    compositions.Add(fucose);

        //    int preSialicAcidFactor = 100;
        //    double preSialicAcid = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor - fucose * preFucoseFactor) / preSialicAcidFactor;
        //    int sialicAcid = Convert.ToInt32(Math.Truncate(preSialicAcid));
        //    compositions.Add(sialicAcid);

        //    int preLactoneFactor = 1;
        //    double preLactone = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor - fucose * preFucoseFactor - sialicAcid * preSialicAcidFactor) / preLactoneFactor;
        //    int lactone = Convert.ToInt32(Math.Truncate(preLactone));
        //    compositions.Add(lactone);

        //    return compositions;
        //}

        public static List<int> ConvertStringGlycanCodeToIntegers(string code)
        {
            //Hex-HexNAc-Fucose-SialicAcid-Lactose
            //H-N-F-S-L 5-4-2-2-1

            List<int> compositions = new List<int>();
            char splitter = '-';
            string[] letters = code.Split(splitter);

            if (letters.Length == 1)
            {
                Console.WriteLine("We are missing the - in the nomenclature");
                Console.ReadKey();
            }

            foreach (string number in letters)
                compositions.Add(Convert.ToInt32(number));

            return compositions;
        }
    }
}
