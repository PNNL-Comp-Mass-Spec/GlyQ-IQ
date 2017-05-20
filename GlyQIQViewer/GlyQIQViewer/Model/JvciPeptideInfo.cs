
using System;
using System.Text.RegularExpressions;

namespace Sipper.Model
{
    public class JvciPeptideInfo
    {

        #region Constructors

        public JvciPeptideInfo()
        {

        }

        #endregion

        #region Properties

        public string MetagenomicOrfString { get; set; }

        public long ReadID { get; set; }

        public int Begin { get; set; }

        public int End { get; set; }

        public int Orientation { get; set; }

        public string CommonName { get; set; }

        public string Organism { get; set; }

        public string FibroNum { get; set; }


        public static JvciPeptideInfo Parse(string stringContainingInfo)
        {
            string regexPattern =
                @">fibr_(?<fibroAnno>[0-9\.]+)\s+JCVI_PEP_metagenomic.*/read_id=(?<readID>\d+)\s+/begin=(?<begin>\d+)\s+/end=(?<end>\d+)\s+/orientation=(?<orientation>[0-9.-]+)\s+/common_name=\042(?<commonName>.*)\042\s+/organism=\042(?<organism>.*)\042";
            var match=  Regex.Match(stringContainingInfo, regexPattern);

            if (match.Success)
            {
                JvciPeptideInfo info = new JvciPeptideInfo();

                info.FibroNum = match.Groups["fibroAnno"].Value;

                info.ReadID = Convert.ToInt64(match.Groups["readID"].Value);

                info.Begin = Convert.ToInt32(match.Groups["begin"].Value);

                info.End = Convert.ToInt32(match.Groups["end"].Value);

                info.Orientation = Convert.ToInt32(match.Groups["orientation"].Value);

                info.CommonName = match.Groups["commonName"].Value;

                info.Organism = match.Groups["organism"].Value;

                return info;
            }
            else
            {
                return null;
            }



        }


        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

    }
}
