﻿using System;
using System.Collections.Generic;

namespace IQ_X64.Workflows.FileIO.Importers
{
    /// <summary>
    /// Base importer class for importing text data
    /// </summary>
    /// <typeparam name="T">the import data type... i.e UMC or MassTag </typeparam>
    public abstract class ImporterBase<T>
    {
        const string DEFAULT_RETURN_STRING = "-1";


        Dictionary<string, int> columnIndexTable;

        protected char m_delimiter = '\t';
        protected List<string> m_columnHeaders = new List<string>();

        /// <summary>
        /// Name of this class. 
        /// </summary>
        public virtual string Name
        {
            get { return this.ToString(); }
            set { ;}
        }

        #region Public Methods

        public abstract T Import();

        #endregion

        #region Protected Methods


        /// <summary>
        /// This method retrieves a single cell of data (row, column) in the form of a string.  
        /// </summary>
        /// <param name="row">Single row of data </param>
        /// <param name="targetColumn">Column header name</param>
        /// <returns></returns>
        protected string LookupData(List<string> row, string targetColumn)
        {
            return LookupData(row, targetColumn, true);
        }


        protected string LookupData(List<string> row, string[] possibleColumnHeaders)
        {
         
            foreach (var possibleHeader in possibleColumnHeaders)
            {
                int columnIndex = GetColumnIndexForHeader(possibleHeader);
                if (columnIndex != -1)
                {
                    return row[columnIndex];
                }
            }

            return DEFAULT_RETURN_STRING;

        }



        protected string LookupData(List<string> row, string[] possibleColumnHeaders, string defaultValue)
        {
          
            foreach (var possibleHeader in possibleColumnHeaders)
            {
                int columnIndex = GetColumnIndexForHeader(possibleHeader);
                if (columnIndex != -1)
                {
                    return row[columnIndex];
                }
            }

            return defaultValue;

        }

        protected string LookupData(List<string> row, string targetColumn, bool ignoreCase)
        {


            int columnIndex = GetColumnIndexForHeader(targetColumn);
            if (columnIndex == -1) 
            {
              return DEFAULT_RETURN_STRING;
            }
            else
            {
               return row[columnIndex];
            }
            
        }


        protected void CreateHeaderLookupTable(string headerLine)
        {
            m_columnHeaders = ProcessLine(headerLine);

            this.columnIndexTable = new Dictionary<string, int>();


            for (int i = 0; i < m_columnHeaders.Count; i++)
            {
                string header = m_columnHeaders[i].ToLower();

                if (!this.columnIndexTable.ContainsKey(header))
                {
                    this.columnIndexTable.Add(header, i);
                }

            }

        }


        protected string ParseStringField(List<string> rowData, string[] headers, string defaultVal = "")
        {
            string rowValueString = LookupData(rowData, headers, string.Empty);

            if (string.IsNullOrEmpty(rowValueString))
            {
                return defaultVal;
            }

            return rowValueString;
        }



        protected bool ParseBoolField(string inputstring)
        {
            bool result = false;
            if (bool.TryParse(inputstring, out result))
                return result;
            else return false;
        }

        protected bool ParseBoolField(List<string>rowData, string[]headers, bool defaultVal = false)
        {
            string rowValueString = LookupData(rowData, headers, string.Empty).ToLower();

            if (string.IsNullOrEmpty(rowValueString))
            {
                return defaultVal;
            }
            
            if (rowValueString=="1" || rowValueString=="true")
            {
                return true;

            }
            
            if (rowValueString=="0"|| rowValueString=="false")
            {
                return false;
            }

            return defaultVal;


        }

        protected short ParseShortField(string inputstring)
        {
            short result = 0;
            if (Int16.TryParse(inputstring, out result))
                return result;
            else return 0;
        }


        protected short ParseShortField(List<string>rowData, string[]headers, short defaultVal = -1)
        {
            string rowValueString = LookupData(rowData, headers, string.Empty).ToLower();

            if (string.IsNullOrEmpty(rowValueString))
            {
                return defaultVal;
            }


            short result;
            if (Int16.TryParse(rowValueString, out result))
            {
                return result;
            }

            return defaultVal;


        }



        protected double ParseDoubleField(string inputstring)
        {
            double result = 0;
            if (double.TryParse(inputstring, out result))
                return result;
            else
            {
                return double.NaN;
            }
        }

        protected double ParseDoubleField(List<string>rowData, string[]headers, double defaultVal = double.NaN)
        {
            string rowValueString = LookupData(rowData, headers, string.Empty).ToLower();

            if (string.IsNullOrEmpty(rowValueString))
            {
                return defaultVal;
            }

            double result;
            if (double.TryParse(rowValueString, out result))
            {
                return result;
            }

            return defaultVal;
        }




        protected float ParseFloatField(string inputstring)
        {
            float result = 0;
            if (float.TryParse(inputstring, out result))
                return result;
            else return float.NaN;

        }

        protected float ParseFloatField(List<string> rowData, string[] headers, float defaultVal = float.NaN)
        {
            string rowValueString = LookupData(rowData, headers, string.Empty).ToLower();

            if (string.IsNullOrEmpty(rowValueString))
            {
                return defaultVal;
            }

            float result;
            if (float.TryParse(rowValueString, out result))
            {
                return result;
            }

            return defaultVal;
        }





        protected int ParseIntField(string inputstring)
        {
            int result = 0;
            if (Int32.TryParse(inputstring, out result))
                return result;
            else
            {
                double secondAttempt = ParseDoubleField(inputstring);
                if (secondAttempt != double.NaN)
                {
                    return Convert.ToInt32(secondAttempt);
                }
                else
                {
                    return -1;
                }
            }
        }

        protected int ParseIntField(List<string> rowData, string[] headers, int defaultVal = -1)
        {
            string rowValueString = LookupData(rowData, headers, string.Empty).ToLower();

            if (string.IsNullOrEmpty(rowValueString))
            {
                return defaultVal;
            }

            int result;
            if (Int32.TryParse(rowValueString, out result))
            {
                return result;
            }

            return defaultVal;
        }


        protected long ParseLongField(string inputstring)
        {
            long result = -1;
            if (Int64.TryParse(inputstring, out result))
                return result;
            else
            {
                return -1;
            }
        }


        protected long ParseLongField(List<string> rowData, string[] headers, long defaultVal = -1)
        {
            string rowValueString = LookupData(rowData, headers, string.Empty).ToLower();

            if (string.IsNullOrEmpty(rowValueString))
            {
                return defaultVal;
            }

            long result;
            if (Int64.TryParse(rowValueString, out result))
            {
                return result;
            }

            return defaultVal;
        }



        /// <summary>
        /// Parses a single line of data into a List of strings
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        protected List<string> ProcessLine(string inputLine)
        {
            char[] splitter = { m_delimiter };
            List<string> parsedLine = new List<string>();

            string[] arr = inputLine.Split(splitter);
            foreach (string str in arr)
            {
                parsedLine.Add(str);
            }
            return parsedLine;
        }




        protected int GetColumnIndexForHeader(string target)
        {

            string t = target.ToLower();


            if (this.columnIndexTable.ContainsKey(t))
            {
                return this.columnIndexTable[t];
            }
            else
            {
                return -1;
            }


        }

        #endregion

    }
}
