using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using PNNLOmics.Data;
using GetPeaks_DLL.SQLite.DataTransferConverters;
using GetPeaks_DLL.Objects.TandemMSObjects;
using YAFMS_DB.GetPeaks;
using ScanCentric = GetPeaks_DLL.Objects.TandemMSObjects.ScanCentric;

namespace GetPeaks_DLL.SQLite
{
    public class DatabaseAdaptor:IDisposable
    {
        #region properties

        private string m_fileName { get; set; }
        private DatabaseLayer m_DatabaseLayer { get; set; }

        #endregion

        public DatabaseAdaptor(string fileName)
        {
            this.m_fileName = fileName;
            this.m_DatabaseLayer = new DatabaseLayer();
        }

        public virtual void Close()
        {
            //this.m_DatabaseLayer = null;
        }

        public virtual void Dispose()
        {
            this.Close();
        }


        #region this may not be needed.  Lets find out (off)

        //public bool ObjectToSQLite(DatabaseFeatureLiteObject data)
        //{
        //    bool didThisWork = false;
        //    //DatabaseLayer newDataBaseLayer = new DatabaseLayer();
        //    didThisWork = m_DatabaseLayer.WriteFeatrueLitedata(data, m_fileName);
        //    return didThisWork;
        //}

        //public bool ObjectToSQLite(DatabaseElutingPeakObject data)
        //{
        //    bool didThisWork = false;
        //    //DatabaseLayer newDataBaseLayer = new DatabaseLayer();
        //    didThisWork = m_DatabaseLayer.WriteElutingPeakdata(data, m_fileName);
        //    return didThisWork;
        //}

        #endregion

        #region Writers
        /// <summary>
        /// Decon ElutingPeak
        /// </summary>
        /// <param name="ePeak"></param>
        /// <returns></returns>
        public bool WriteElutingPeakToSQLite_Old(ElutingPeak ePeak)
        {
            #region convert data to DTO
            DatabaseAdaptor adaptor = new DatabaseAdaptor(m_fileName);
            FeatureLight elutingFeatureLite = new FeatureLight();
            //elutingFeatureLite = adaptor.ConvertToFeature(ePeak);
            elutingFeatureLite = ConvertFeaturesToOtherFeatures.ConvertElutingPeakToFeatureLite(ePeak);

            //DatabaseFeatureLiteObject featureObject = adaptor.SetFeatureLite(elutingFeatureLite);
            //DatabaseElutingPeakObject elutingPeakObject = adaptor.SetElutingPeak(ePeak);

            DatabaseFeatureLiteObject featureObject = SetDataTransferObjects.SetFeatureLite(elutingFeatureLite);
            DatabaseElutingPeakObject elutingPeakObject = SetDataTransferObjects.SetElutingPeak(ePeak);
            #endregion

            #region write DTO
            
            bool didThisWorkFeature = false;
            bool didThisWorkFeatureincrement = false;
            bool didThisWorkElutingPeak = false;
            bool didThisWorkOverall = false;

            //DatabaseLayer newDataBaseLayer = new DatabaseLayer();

            //didThisWorkFeature = m_DatabaseLayer.WriteFeatrueLitedata(featureObject, m_fileName);
            didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObject(featureObject, m_fileName);
            didThisWorkFeatureincrement = m_DatabaseLayer.IncrementTableInformationByOne(m_fileName);

            //didThisWorkElutingPeak = m_DatabaseLayer.WriteElutingPeakdata(elutingPeakObject, m_fileName);
            didThisWorkElutingPeak = m_DatabaseLayer.WriteDataTransferObject(elutingPeakObject, m_fileName);

            #endregion

            if (didThisWorkFeature == true && didThisWorkElutingPeak == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        /// <summary>
        /// Omcis ElutingPeak
        /// </summary>
        /// <param name="ePeak"></param>
        /// <returns></returns>
        public bool WriteElutingPeakToSQLite_Old(ElutingPeakOmics ePeak)
        {
            #region convert data to DTO
            DatabaseAdaptor adaptor = new DatabaseAdaptor(m_fileName);
            FeatureLight elutingFeatureLite = new FeatureLight();
            //elutingFeatureLite = adaptor.ConvertToFeature(ePeak);
            elutingFeatureLite = ConvertFeaturesToOtherFeatures.ConvertElutingPeakOmicsToFeatureLight(ePeak);

            //DatabaseFeatureLiteObject featureObject = adaptor.SetFeatureLite(elutingFeatureLite);
            //DatabaseElutingPeakObject elutingPeakObject = adaptor.SetElutingPeak(ePeak);

            DatabaseFeatureLiteObject featureObject = SetDataTransferObjects.SetFeatureLite(elutingFeatureLite);
            DatabaseElutingPeakObject elutingPeakObject = SetDataTransferObjects.SetElutingPeak(ePeak);
            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkFeatureincrement = false;
            bool didThisWorkElutingPeak = false;
            bool didThisWorkOverall = false;

            //DatabaseLayer newDataBaseLayer = new DatabaseLayer();

            //didThisWorkFeature = m_DatabaseLayer.WriteFeatrueLitedata(featureObject, m_fileName);
            didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObject(featureObject, m_fileName);
            didThisWorkFeatureincrement = m_DatabaseLayer.IncrementTableInformationByOne(m_fileName);

            //didThisWorkElutingPeak = m_DatabaseLayer.WriteElutingPeakdata(elutingPeakObject, m_fileName);
            didThisWorkElutingPeak = m_DatabaseLayer.WriteDataTransferObject(elutingPeakObject, m_fileName);

            #endregion

            if (didThisWorkFeature == true && didThisWorkElutingPeak == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        /// <summary>
        /// Omics Isotope object
        /// </summary>
        /// <param name="ePeak"></param>
        /// <returns></returns>
        public bool WriteIsotopeStorageToSQLite_Old(IsotopeObject iObject)
        {
            #region convert data to DTO
            DatabaseAdaptor adaptor = new DatabaseAdaptor(m_fileName);
            DatabaseIsotopeObject databaseIsotope = new DatabaseIsotopeObject();
            //databaseIsotope = adaptor.SetToIsotopeStorageOutput(iObject);
            databaseIsotope = SetDataTransferObjects.SetToIsotopeStorageOutput(iObject,0);

            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkOverall = false;

            //didThisWorkFeature = m_DatabaseLayer.WriteIsotopeStorageData(databaseIsotope, m_fileName);
            didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObject(databaseIsotope, m_fileName);

            #endregion

            if (didThisWorkFeature == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        /// <summary>
        /// Omics Isotope object
        /// </summary>
        /// <param name="ePeak"></param>
        /// <returns></returns>
        public bool WriteIsotopeStorageMonoListToSQLite(List<IsotopeObject> iObjectList, int scanNum, string tableName, string tablenameCount, List<string> columnNamesForCount)
        {
            #region convert data to DTO
            //DatabaseAdaptor adaptor = new DatabaseAdaptor(m_fileName);

            DatabaseIsotopeObjectList iObjectPile = new DatabaseIsotopeObjectList();

            foreach (IsotopeObject iObject in iObjectList)
            {
                DatabaseIsotopeObject databaseIsotope = new DatabaseIsotopeObject();
                databaseIsotope = SetDataTransferObjects.SetToIsotopeStorageOutput(iObject, scanNum);
                databaseIsotope.TableName = tableName;
                iObjectPile.DatabaseTransferObjects.Add(databaseIsotope);
            }

            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkOverall = false;

            //didThisWorkFeature = m_DatabaseLayer.WriteIsotopeStorageData(databaseIsotope, m_fileName);

            //see where we are on the table size
            //int sizeOfDatabaseThusFar = m_DatabaseLayer.ReadDatabaseSize(m_fileName, "Count");
            //increment table size to reflect the added data
            //m_DatabaseLayer.IncrementTableInformation(m_fileName, "Count", iObjectList.Count);
            //sizeOfDatabaseThusFar++;
            didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectList(iObjectPile, m_fileName);

            //organize increment values
            List<int> incrementValues = new List<int>();
            incrementValues.Add(iObjectList.Count);
            int isotopeCount = 0;
            foreach (IsotopeObject incrementValue in iObjectList)
            {
                isotopeCount += incrementValue.IsotopeList.Count;
            }
            incrementValues.Add(isotopeCount);

            //m_DatabaseLayer.ReadDatabaseSize(m_fileName, columnNamesForCount[0]);

            m_DatabaseLayer.IncrementTableInformation(m_fileName, tablenameCount, columnNamesForCount, incrementValues);

            #endregion

            if (didThisWorkFeature == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        /// <summary>
        /// Writes a list of processed peaks
        /// </summary>
        /// <param name="pObjectList"></param>
        /// <param name="scanNum"></param>
        /// <param name="tableName"></param>
        /// <param name="tablenameCount"></param>
        /// <param name="columnNamesForCount"></param>
        /// <returns></returns>
        public bool WriteProcessedPeakListToSQLite(List<ProcessedPeak> pObjectList, string tableName, string tablenameCount)
        {
            #region convert data to DTO

            DatabaseProcessedPeakObjectList pObjectPile = new DatabaseProcessedPeakObjectList();
            pObjectPile.TableName = tableName;

            //there has to be a better way than this
            DatabasePeakProcessedObject databasePeakColumnAndValuesLocation = new DatabasePeakProcessedObject();
            pObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
            pObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;

            for (int i=0;i< pObjectList.Count;i++)
            {
                ProcessedPeak pObject = pObjectList[i];
                DatabasePeakProcessedObject databasePeak = new DatabasePeakProcessedObject();
                databasePeak = SetDataTransferObjects.SetPeakProcessedOutput(pObject, i);
                databasePeak.TableName = tableName;
                pObjectPile.DatabaseTransferObjects.Add(databasePeak);
            }

            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkOverall = false;

            //didThisWorkFeature = m_DatabaseLayer.WriteIsotopeStorageData(databaseIsotope, m_fileName);

            //see where we are on the table size
            //int sizeOfDatabaseThusFar = m_DatabaseLayer.ReadDatabaseSize(m_fileName, "Count");
            //increment table size to reflect the added data
            //m_DatabaseLayer.IncrementTableInformation(m_fileName, "Count", iObjectList.Count);
            //sizeOfDatabaseThusFar++;
            didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectList(pObjectPile, m_fileName);

            //organize increment values
            int incrementValues = pObjectList.Count;
           
            //m_DatabaseLayer.ReadDatabaseSize(m_fileName, columnNamesForCount[0]);
            string columnNamesForCount = tableName;

            m_DatabaseLayer.IncrementTableInformation(m_fileName, tablenameCount, columnNamesForCount, incrementValues);

            #endregion

            if (didThisWorkFeature == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        /// <summary>
        /// Writes a list of processed peaks
        /// </summary>
        /// <param name="pObjectList"></param>
        /// <param name="scanNum"></param>
        /// <param name="tableName"></param>
        /// <param name="tablenameCount"></param>
        /// <param name="columnNamesForCount"></param>
        /// <returns></returns>
        public bool WriteScansToSQLite(List<MSSpectra> spectraList, string tableName, string tablenameCount)
        {
            #region convert data to DTO

            DatabaseScanObjectList sObjectPile = new DatabaseScanObjectList();
            sObjectPile.TableName = tableName;

            //there has to be a better way than this
            DatabaseScanObject databasePeakColumnAndValuesLocation = new DatabaseScanObject();
            sObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
            sObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;

            for (int i = 0; i < spectraList.Count; i++)
            {
                MSSpectra sObject = spectraList[i];
                DatabaseScanObject databaseScan = new DatabaseScanObject();
                databaseScan = SetDataTransferObjects.SetScanOutput(sObject, i);
                databaseScan.TableName = tableName;
                sObjectPile.DatabaseTransferObjects.Add(databaseScan);
            }

            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkOverall = false;

            //didThisWorkFeature = m_DatabaseLayer.WriteIsotopeStorageData(databaseIsotope, m_fileName);

            //see where we are on the table size
            //int sizeOfDatabaseThusFar = m_DatabaseLayer.ReadDatabaseSize(m_fileName, "Count");
            //increment table size to reflect the added data
            //m_DatabaseLayer.IncrementTableInformation(m_fileName, "Count", iObjectList.Count);
            //sizeOfDatabaseThusFar++;
            didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectList(sObjectPile, m_fileName);

            //organize increment values
            int incrementValues = spectraList.Count;

            //m_DatabaseLayer.ReadDatabaseSize(m_fileName, columnNamesForCount[0]);
            //List<string> columnNamesForCount = pageNamesForCounting;
            string columnNamesForCount = tableName;

            m_DatabaseLayer.IncrementTableInformation(m_fileName, tablenameCount, columnNamesForCount, incrementValues);

            #endregion

            if (didThisWorkFeature == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        //public bool WritePeakToSQLite(Peak ePeak, int peakIndex, int scanNum)
        public bool WritePeakToSQLite(PNNLOmics.Data.Peak ePeak, int peakIndex, int scanNum)
        {
            //http://stackoverflow.com/questions/8039185/how-can-you-use-last-insert-rowid-to-insert-multiple-rows?rq=1
            
            #region convert data to DTO
            DatabaseAdaptor adaptor = new DatabaseAdaptor(m_fileName);

            DatabaseDeconPeakObject dPeak = SetDataTransferObjects.SetPeakOmicsOutput(ePeak, scanNum);

            #endregion

            #region write DTO

            bool didThisWorkPeak = false;
            bool didThisWorkPeakIncrement = false;
            bool didThisWorkOverall = false;

            dPeak.PeakIndex = m_DatabaseLayer.ReadDatabaseSize(m_fileName, "TableInfo", "Count");
            dPeak.Values[0] = dPeak.PeakIndex;
            didThisWorkPeak = m_DatabaseLayer.WriteDataTransferObject(dPeak, m_fileName);
            didThisWorkPeakIncrement = m_DatabaseLayer.IncrementTableInformationByOne(m_fileName);


            #endregion

            if (didThisWorkPeak == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        //public bool WritePeakToSQLite(Peak ePeak, int peakIndex, int scanNum)
        public bool WritePrecursorPeakToSQLite(PNNLOmics.Data.ProcessedPeak ePeak, double rawMass, int tandemScanNum, int peakIndex, string tableName, string tablenameCount, int charge)
        {
            //http://stackoverflow.com/questions/8039185/how-can-you-use-last-insert-rowid-to-insert-multiple-rows?rq=1

            #region convert data to DTO
            DatabaseAdaptor adaptor = new DatabaseAdaptor(m_fileName);

            int precursorScanNumber = ePeak.ScanNumber;
            DatabasePeakProcessedWithMZObject dPeak = SetDataTransferObjects.SetPrecursorPeakOutput(ePeak, rawMass, tandemScanNum, precursorScanNumber, peakIndex, charge);

            DatabaseTransferObjectList convertToList = new DatabasePeakProcessedWithMZObject.DatabasePeakProcessedWithMZObjectList();
            convertToList.DatabaseTransferObjects.Add(dPeak);
            convertToList.Columns = dPeak.Columns;
            convertToList.ValuesTypes = dPeak.ValuesTypes;
            convertToList.TableName = tableName;

            dPeak.TableName = tableName;
            dPeak.PeakNumber = m_DatabaseLayer.ReadDatabaseSize(m_fileName, tablenameCount, tableName);
            dPeak.Values[2] = dPeak.PeakNumber;

            #endregion

            #region write DTO

            bool didThisWorkPeak = false;
            bool didThisWorkPeakIncrement = false;
            bool didThisWorkOverall = false;

            //didThisWorkPeak = m_DatabaseLayer.WriteDataTransferObject(dPeak, m_fileName);
            didThisWorkPeak = m_DatabaseLayer.WriteDataTransferObjectList(convertToList, m_fileName);

            int incrementValues = 1;
            string columnNamesForCount = tableName;
            m_DatabaseLayer.IncrementTableInformation(m_fileName, tablenameCount, columnNamesForCount, incrementValues);

            #endregion

            if (didThisWorkPeak == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        public bool WriteIsosToSQLite(PNNLOmics.Data.Peak ePeak, int peakIndex, int scanNum)
        {
            //http://stackoverflow.com/questions/8039185/how-can-you-use-last-insert-rowid-to-insert-multiple-rows?rq=1

            #region convert data to DTO
            DatabaseAdaptor adaptor = new DatabaseAdaptor(m_fileName);

            DatabaseDeconPeakObject dPeak = SetDataTransferObjects.SetPeakOmicsOutput(ePeak, scanNum);

            #endregion

            #region write DTO

            bool didThisWorkPeak = false;
            bool didThisWorkPeakIncrement = false;
            bool didThisWorkOverall = false;

            dPeak.PeakIndex = m_DatabaseLayer.ReadDatabaseSize(m_fileName, "TableInfo", "Count");
            dPeak.Values[0] = dPeak.PeakIndex;
            didThisWorkPeak = m_DatabaseLayer.WriteDataTransferObject(dPeak, m_fileName);
            didThisWorkPeakIncrement = m_DatabaseLayer.IncrementTableInformationByOne(m_fileName);


            #endregion

            if (didThisWorkPeak == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }








        #region centric area

        /// <summary>
        /// Writes a list of processed peaks
        /// </summary>
        /// <param name="scanObjectList"></param>
        /// <param name="scanNum"></param>
        /// <param name="tableName"></param>
        /// <param name="tablenameCount"></param>
        /// <param name="columnNamesForCount"></param>
        /// <returns></returns>
        public bool WriteScanCentricListToSQLite(List<ScanCentric> scanObjectList, string tableName, string tablenameCount)
        {
            #region convert data to DTO

            DatabaseScanCentricObjectList currentObjectPile = new DatabaseScanCentricObjectList();
            currentObjectPile.TableName = tableName;

            //there has to be a better way than this
            DatabaseScanCentricObject databasePeakColumnAndValuesLocation = new DatabaseScanCentricObject();
            currentObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
            currentObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;

            for (int i = 0; i < scanObjectList.Count; i++)
            {
                ScanCentric currentObject = scanObjectList[i];
                DatabaseScanCentricObject databasePeak = SetDataTransferObjects.SetScanCentricOutput(currentObject, currentObject.ScanID);//pObject.ID used to be i
                databasePeak.TableName = tableName;
                currentObjectPile.DatabaseTransferObjects.Add(databasePeak);
            }

            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkOverall = false;

            didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectList(currentObjectPile, m_fileName);

            //organize increment values
            int incrementValues = scanObjectList.Count;

            string columnNamesForCount = tableName;

            m_DatabaseLayer.IncrementTableInformation(m_fileName, tablenameCount, columnNamesForCount, incrementValues);

            #endregion

            if (didThisWorkFeature == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        /// <summary>
        /// Writes a list of processed peaks
        /// </summary>
        /// <param name="peakObjectList"></param>
        /// <param name="scanNum"></param>
        /// <param name="tableName"></param>
        /// <param name="tablenameCount"></param>
        /// <param name="columnNamesForCount"></param>
        /// <returns></returns>
        public bool WritePeakCentricListToSQLite(List<PeakCentric> peakObjectList, string tableName, string tablenameCount)
        {
            #region convert data to DTO

            DatabasePeakCentricObjectList currentObjectPile = new DatabasePeakCentricObjectList();
            //currentObjectPile.IndexedColumns = indexes;
            //DatabasePeakCentricLiteObjectList currentObjectPile = new DatabasePeakCentricLiteObjectList();
            currentObjectPile.TableName = tableName;

            //there has to be a better way than this
            DatabasePeakCentricObject databasePeakColumnAndValuesLocation = new DatabasePeakCentricObject();
            currentObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
            currentObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;

            for (int i = 0; i < peakObjectList.Count; i++)
            {
                PeakCentric currentObject = peakObjectList[i];
                DatabasePeakCentricObject databasePeak = SetDataTransferObjects.SetPeakCentricOutput(currentObject, currentObject.PeakID, currentObject.ScanID);//pObject.ID used to be i
                databasePeak.TableName = tableName;
                currentObjectPile.DatabaseTransferObjects.Add(databasePeak);
            }

            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkOverall = false;

            didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectList(currentObjectPile, m_fileName);
            
            if (didThisWorkFeature == true)//add index here
            {
                //didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectIndex(indexes, tableName , m_fileName);
            }
            //organize increment values
            int incrementValues = peakObjectList.Count;

            string columnNamesForCount = tableName;

            m_DatabaseLayer.IncrementTableInformation(m_fileName, tablenameCount, columnNamesForCount, incrementValues);

            #endregion

            if (didThisWorkFeature == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        /// <summary>
        /// Writes a list of processed peaks but only the essentials
        /// </summary>
        /// <param name="peakObjectList"></param>
        /// <param name="scanNum"></param>
        /// <param name="tableName"></param>
        /// <param name="tablenameCount"></param>
        /// <param name="columnNamesForCount"></param>
        /// <returns></returns>
        public bool WriteXYDataToSQLite(List<PeakCentric> peakObjectList, string tableName, string tablenameCount)
        {
            #region convert data to DTO

            DatabasePeakCentricObjectList currentObjectPile = new DatabasePeakCentricObjectList();
            //currentObjectPile.IndexedColumns = indexes;
            //DatabasePeakCentricLiteObjectList currentObjectPile = new DatabasePeakCentricLiteObjectList();
            currentObjectPile.TableName = tableName;

            //there has to be a better way than this
            DatabasePeakCentricObject databasePeakColumnAndValuesLocation = new DatabasePeakCentricObject();

            //dumb down peak centric

            currentObjectPile.Columns = new List<string>();
            currentObjectPile.ValuesTypes = new List<DbType>();
            //Peak ID
            currentObjectPile.Columns.Add(databasePeakColumnAndValuesLocation.Columns[0]);
            currentObjectPile.ValuesTypes.Add(databasePeakColumnAndValuesLocation.ValuesTypes[0]);
            //Scan ID
            currentObjectPile.Columns.Add(databasePeakColumnAndValuesLocation.Columns[1]);
            currentObjectPile.ValuesTypes.Add(databasePeakColumnAndValuesLocation.ValuesTypes[1]);
            //MZ
            currentObjectPile.Columns.Add(databasePeakColumnAndValuesLocation.Columns[5]);
            currentObjectPile.ValuesTypes.Add(databasePeakColumnAndValuesLocation.ValuesTypes[5]);
            //Height
            currentObjectPile.Columns.Add(databasePeakColumnAndValuesLocation.Columns[7]);
            currentObjectPile.ValuesTypes.Add(databasePeakColumnAndValuesLocation.ValuesTypes[7]);
            //Width
            //currentObjectPile.Columns.Add(databasePeakColumnAndValuesLocation.Columns[8]);
            //currentObjectPile.ValuesTypes.Add(databasePeakColumnAndValuesLocation.ValuesTypes[8]);


            for (int i = 0; i < peakObjectList.Count; i++)
            {
                PeakCentric currentObject = peakObjectList[i];
                //DatabasePeakCentricObject databasePeak = SetDataTransferObjects.SetPeakCentricOutput(currentObject, currentObject.PeakID, currentObject.ScanID);//pObject.ID used to be i
                DatabasePeakCentricObject databasePeak = SetDataTransferObjects.SetPeakCentricLightOutput(currentObject, currentObject.PeakID, currentObject.ScanID);//pObject.ID used to be i
                databasePeak.TableName = tableName;
                currentObjectPile.DatabaseTransferObjects.Add(databasePeak);
            }

            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkOverall = false;

            didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectList(currentObjectPile, m_fileName);

            if (didThisWorkFeature == true)//add index here
            {
                //didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectIndex(indexes, tableName , m_fileName);
            }
            //organize increment values
            int incrementValues = peakObjectList.Count;

            string columnNamesForCount = tableName;

            m_DatabaseLayer.IncrementTableInformation(m_fileName, tablenameCount, columnNamesForCount, incrementValues);

            #endregion

            if (didThisWorkFeature == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        #region off
        ///// <summary>
        ///// Writes a list of processed peaks
        ///// </summary>
        ///// <param name="fragmentObjectList"></param>
        ///// <param name="scanNum"></param>
        ///// <param name="tableName"></param>
        ///// <param name="tablenameCount"></param>
        ///// <param name="columnNamesForCount"></param>
        ///// <returns></returns>
        //public bool WriteFragmentCentricListToSQLite(List<FragmentCentric> fragmentObjectList, string tableName, string tablenameCount)
        //{
        //    #region convert data to DTO

        //    DatabaseFragmentCentricObjectList currentObjectPile = new DatabaseFragmentCentricObjectList();
        //    currentObjectPile.TableName = tableName;

        //    //there has to be a better way than this
        //    DatabaseFragmentCentricObject databasePeakColumnAndValuesLocation = new DatabaseFragmentCentricObject();
        //    currentObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
        //    currentObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;

        //    for (int i = 0; i < fragmentObjectList.Count; i++)
        //    {
        //        FragmentCentric currentObject = fragmentObjectList[i];
        //        DatabaseFragmentCentricObject databasePeak = SetDataTransferObjects.SetFragmentCentricOutput(currentObject, currentObject.ScanID);//pObject.ID used to be i
        //        databasePeak.TableName = tableName;
        //        currentObjectPile.DatabaseTransferObjects.Add(databasePeak);
        //    }

        //    #endregion

        //    #region write DTO

        //    bool didThisWorkFeature = false;
        //    bool didThisWorkOverall = false;

        //    didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectList(currentObjectPile, m_fileName);

        //    //organize increment values
        //    int incrementValues = fragmentObjectList.Count;

        //    string columnNamesForCount = tableName;

        //    m_DatabaseLayer.IncrementTableInformation(m_fileName, tablenameCount, columnNamesForCount, incrementValues);

        //    #endregion

        //    if (didThisWorkFeature == true)
        //    {
        //        didThisWorkOverall = true;
        //    }
        //    return didThisWorkOverall;
        //}
        #endregion

        #region off

        ///// <summary>
        ///// Writes a list of processed peaks
        ///// </summary>
        ///// <param name="pObjectList"></param>
        ///// <param name="scanNum"></param>
        ///// <param name="tableName"></param>
        ///// <param name="tablenameCount"></param>
        ///// <param name="columnNamesForCount"></param>
        ///// <returns></returns>
        //public bool WriteAttributeCentricListToSQLite(List<AttributeCentric> attributrObjectList, string tableName, string tablenameCount)
        //{
        //    #region convert data to DTO

        //    DatabaseAttributeCentricObjectList currentObjectPile = new DatabaseAttributeCentricObjectList();
        //    currentObjectPile.TableName = tableName;

        //    //there has to be a better way than this
        //    DatabaseAttributeCentricObject databasePeakColumnAndValuesLocation = new DatabaseAttributeCentricObject();
        //    currentObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
        //    currentObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;

        //    for (int i = 0; i < attributrObjectList.Count; i++)
        //    {
        //        AttributeCentric currentobjectObject = attributrObjectList[i];
        //        DatabaseAttributeCentricObject databasePeak = SetDataTransferObjects.SetAttributeCentricOutput(currentobjectObject, currentobjectObject.PeakID);//pObject.ID used to be i
        //        databasePeak.TableName = tableName;
        //        currentObjectPile.DatabaseTransferObjects.Add(databasePeak);
        //    }

        //    #endregion

        //    #region write DTO

        //    bool didThisWorkFeature = false;
        //    bool didThisWorkOverall = false;

        //    didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectList(currentObjectPile, m_fileName);

        //    //organize increment values
        //    int incrementValues = attributrObjectList.Count;

        //    string columnNamesForCount = tableName;

        //    m_DatabaseLayer.IncrementTableInformation(m_fileName, tablenameCount, columnNamesForCount, incrementValues);

        //    #endregion

        //    if (didThisWorkFeature == true)
        //    {
        //        didThisWorkOverall = true;
        //    }
        //    return didThisWorkOverall;
        //}


        ///// <summary>
        ///// Writes a list of processed peaks
        ///// </summary>
        ///// <param name="attributeObjectList"></param>
        ///// <param name="scanNum"></param>
        ///// <param name="tableName"></param>
        ///// <param name="tablenameCount"></param>
        ///// <param name="columnNamesForCount"></param>
        ///// <returns></returns>
        //public bool UpdateAttributeCentricListToSQLite(List<AttributeCentric> attributeObjectList, string tableName, List<int> activeColumns)
        //{
        //    #region convert data to DTO

        //    DatabaseAttributeCentricObjectList attributeObjectPile = new DatabaseAttributeCentricObjectList();
        //    attributeObjectPile.TableName = tableName;

        //    //there has to be a better way than this
        //    DatabaseAttributeCentricObject databasePeakColumnAndValuesLocation = new DatabaseAttributeCentricObject();

        //    //this is a hack
        //    //this needs to be modified too
        //    //attributeObjectPile.Values; if column int matrix=1
        //    //attributeObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
        //    //attributeObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;
        //    for (int i = 0; i < activeColumns.Count; i++)
        //    {
        //        if (activeColumns[i] == 1)
        //        {
        //            attributeObjectPile.Columns.Add(databasePeakColumnAndValuesLocation.Columns[i]);
        //            attributeObjectPile.ValuesTypes.Add(databasePeakColumnAndValuesLocation.ValuesTypes[i]);
        //        }
        //    }

        //    for (int i = 0; i < attributeObjectList.Count; i++)
        //    {
        //        AttributeCentric currentObject = attributeObjectList[i];
        //        DatabaseAttributeCentricObject databaseItem = SetDataTransferObjects.SetAttributeCentricOutput(currentObject, currentObject.PeakID);//pObject.ID used to be i


        //        databaseItem.TableName = tableName;
        //        databaseItem.Columns = attributeObjectPile.Columns;//new
        //        databaseItem.ValuesTypes = attributeObjectPile.ValuesTypes;//new
        //        for (int j = activeColumns.Count - 1; j >= 0; j--)
        //        {
        //            if (activeColumns[j] == 0)
        //            {
        //                databaseItem.Values.RemoveAt(j);
        //            }
        //        }

        //        attributeObjectPile.DatabaseTransferObjects.Add(databaseItem);
        //    }

        //    #endregion

        //    #region write DTO

        //    bool didThisWorkFeature = false;
        //    bool didThisWorkOverall = false;

        //    didThisWorkFeature = m_DatabaseLayer.UpdateDataTransferObjectList(attributeObjectPile, m_fileName);

        //    #endregion

        //    if (didThisWorkFeature == true)
        //    {
        //        didThisWorkOverall = true;
        //    }
        //    return didThisWorkOverall;
        //}

        ///// <summary>
        ///// Writes a list of processed peaks
        ///// </summary>
        ///// <param name="fragmentObjectList"></param>
        ///// <param name="scanNum"></param>
        ///// <param name="tableName"></param>
        ///// <param name="tablenameCount"></param>
        ///// <param name="columnNamesForCount"></param>
        ///// <returns></returns>
        //public bool UpdateFragmentCentricListToSQLite(List<FragmentCentric> fragmentObjectList, string tableName, List<int> activeColumns)
        //{
        //    #region convert data to DTO

        //    DatabaseFragmentCentricObjectList fragmentObjectPile = new DatabaseFragmentCentricObjectList();
        //    fragmentObjectPile.TableName = tableName;

        //    DatabaseFragmentCentricObject databasePeakColumnAndValuesLocation = new DatabaseFragmentCentricObject();

        //    //hack away
        //    //there has to be a better way than this
        //    //pObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
        //    //pObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;

        //    for (int i = 0; i < activeColumns.Count; i++)
        //    {
        //        if (activeColumns[i] == 1)
        //        {
        //            fragmentObjectPile.Columns.Add(databasePeakColumnAndValuesLocation.Columns[i]);
        //            fragmentObjectPile.ValuesTypes.Add(databasePeakColumnAndValuesLocation.ValuesTypes[i]);
        //        }
        //    }

        //    for (int i = 0; i < fragmentObjectList.Count; i++)
        //    {
        //        FragmentCentric currentObject = fragmentObjectList[i];
        //        DatabaseFragmentCentricObject databaseItem = SetDataTransferObjects.SetFragmentCentricOutput(currentObject, currentObject.ScanID);//pObject.ID used to be i
        //        databaseItem.TableName = tableName;

        //        databaseItem.Columns = fragmentObjectPile.Columns;//new
        //        databaseItem.ValuesTypes = fragmentObjectPile.ValuesTypes;//new
        //        for (int j = activeColumns.Count - 1; j >= 0; j--)
        //        {
        //            if (activeColumns[j] == 0)
        //            {
        //                databaseItem.Values.RemoveAt(j);
        //            }
        //        }

        //        fragmentObjectPile.DatabaseTransferObjects.Add(databaseItem);
        //    }

        //    #endregion

        //    #region write DTO

        //    bool didThisWorkFeature = false;
        //    bool didThisWorkOverall = false;

        //    didThisWorkFeature = m_DatabaseLayer.UpdateDataTransferObjectList(fragmentObjectPile, m_fileName);

        //    #endregion

        //    if (didThisWorkFeature == true)
        //    {
        //        didThisWorkOverall = true;
        //    }
        //    return didThisWorkOverall;
        //}

        #endregion

        /// <summary>
        /// Writes a list of processed peaks
        /// </summary>
        /// <param name="peakObjectList"></param>
        /// <param name="scanNum"></param>
        /// <param name="tableName"></param>
        /// <param name="tablenameCount"></param>
        /// <param name="columnNamesForCount"></param>
        /// <returns></returns>
        public bool UpdatePeakCentricListToSQLite(List<PeakCentric> peakObjectList, string tableName, List<int> activeColumns)
        {
            #region convert data to DTO

            DatabasePeakCentricObjectList currentObjectPile = new DatabasePeakCentricObjectList();
            currentObjectPile.TableName = tableName;

            DatabasePeakCentricObject databasePeakColumnAndValuesLocation = new DatabasePeakCentricObject();
            
            //hack away
            //there has to be a better way than this
            //pObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
            //pObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;

            for (int i = 0; i < activeColumns.Count; i++)
            {
                if (activeColumns[i] == 1)
                {
                    currentObjectPile.Columns.Add(databasePeakColumnAndValuesLocation.Columns[i]);
                    currentObjectPile.ValuesTypes.Add(databasePeakColumnAndValuesLocation.ValuesTypes[i]);
                }
            }

            for (int i = 0; i < peakObjectList.Count; i++)
            {
                PeakCentric currentObject = peakObjectList[i];
                DatabasePeakCentricObject databaseItem = SetDataTransferObjects.SetPeakCentricOutput(currentObject, currentObject.PeakID, currentObject.ScanID);//pObject.ID used to be i
                databaseItem.TableName = tableName;

                databaseItem.Columns = currentObjectPile.Columns;//new
                databaseItem.ValuesTypes = currentObjectPile.ValuesTypes;//new
                for (int j = activeColumns.Count - 1; j >= 0; j--)
                {
                    if (activeColumns[j] == 0)
                    {
                        databaseItem.Values.RemoveAt(j);
                    }
                }

                currentObjectPile.DatabaseTransferObjects.Add(databaseItem);
            }

            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkOverall = false;

            didThisWorkFeature = m_DatabaseLayer.UpdateDataTransferObjectList(currentObjectPile, m_fileName);

            #endregion

            if (didThisWorkFeature == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }

        /// <summary>
        /// Writes a list of processed peaks
        /// </summary>
        /// <param name="fragmentObjectList"></param>
        /// <param name="scanNum"></param>
        /// <param name="tableName"></param>
        /// <param name="tablenameCount"></param>
        /// <param name="columnNamesForCount"></param>
        /// <returns></returns>
        public bool UpdateScanCentricListToSQLite(List<ScanCentric> fragmentObjectList, string tableName, List<int> activeColumns)
        {
            #region convert data to DTO

            DatabaseScanCentricObjectList scanObjectPile = new DatabaseScanCentricObjectList();
            scanObjectPile.TableName = tableName;

            DatabaseScanCentricObject databasePeakColumnAndValuesLocation = new DatabaseScanCentricObject();

            //hack away
            //there has to be a better way than this
            //pObjectPile.Columns = databasePeakColumnAndValuesLocation.Columns;
            //pObjectPile.ValuesTypes = databasePeakColumnAndValuesLocation.ValuesTypes;

            for (int i = 0; i < activeColumns.Count; i++)
            {
                if (activeColumns[i] == 1)
                {
                    scanObjectPile.Columns.Add(databasePeakColumnAndValuesLocation.Columns[i]);
                    scanObjectPile.ValuesTypes.Add(databasePeakColumnAndValuesLocation.ValuesTypes[i]);
                }
            }

            for (int i = 0; i < fragmentObjectList.Count; i++)
            {
                ScanCentric currentObject = fragmentObjectList[i];
                DatabaseScanCentricObject databaseItem = SetDataTransferObjects.SetScanCentricOutput(currentObject, currentObject.ScanID);//pObject.ID used to be i
                databaseItem.TableName = tableName;

                databaseItem.Columns = scanObjectPile.Columns;//new
                databaseItem.ValuesTypes = scanObjectPile.ValuesTypes;//new
                for (int j = activeColumns.Count - 1; j >= 0; j--)
                {
                    if (activeColumns[j] == 0)
                    {
                        databaseItem.Values.RemoveAt(j);
                    }
                }

                scanObjectPile.DatabaseTransferObjects.Add(databaseItem);
            }

            #endregion

            #region write DTO

            bool didThisWorkFeature = false;
            bool didThisWorkOverall = false;

            didThisWorkFeature = m_DatabaseLayer.UpdateDataTransferObjectList(scanObjectPile, m_fileName);

            #endregion

            if (didThisWorkFeature == true)
            {
                didThisWorkOverall = true;
            }
            return didThisWorkOverall;
        }


        #endregion


        #endregion

        #region old converters (off)

        ///// <summary>
        ///// DeconTools ElutingPeak
        ///// </summary>
        ///// <param name="ePeak"></param>
        ///// <returns></returns>
        //public FeatureLight ConvertToFeature(ElutingPeak ePeak)
        //{
        //    FeatureLight newFeature;
        //    if (ePeak.ID == 1)
        //    {
        //        newFeature = new FeatureLight();

        //        newFeature.ID = ePeak.ID;
        //        newFeature.Abundance = (long)ePeak.Intensity;
        //        newFeature.ChargeState = ePeak.ChargeState;
        //        newFeature.DriftTime = 99;
        //        newFeature.MassMonoisotopic = ePeak.Mass;
        //        newFeature.RetentionTime = ePeak.ScanMaxIntensity;
        //        newFeature.Score = ePeak.FitScore;
        //    }
        //    else
        //    {
        //        newFeature = new FeatureLight();
        //        newFeature.ID = 0;
        //    }
        //    return newFeature;
        //}

        ///// <summary>
        ///// Omics ElutingPeak
        ///// </summary>
        ///// <param name="ePeak"></param>
        ///// <returns></returns>
        //public FeatureLight ConvertToFeature(ElutingPeakOmics ePeak)
        //{
        //    FeatureLight newFeature;
        //    if (ePeak.ID == 1)
        //    {
        //        newFeature = new FeatureLight();

        //        newFeature.ID = ePeak.ID;
        //        newFeature.Abundance = (long)ePeak.Intensity;
        //        newFeature.ChargeState = ePeak.ChargeState;
        //        newFeature.DriftTime = 99;
        //        newFeature.MassMonoisotopic = ePeak.Mass;
        //        newFeature.RetentionTime = ePeak.ScanMaxIntensity;
        //        newFeature.Score = ePeak.FitScore;
        //    }
        //    else
        //    {
        //        newFeature = new FeatureLight();
        //        newFeature.ID = 0;
        //    }
        //    return newFeature;
        //}

        #endregion

        #region old stuff (off)
        //public class DatabaseFeatureLiteObject
        //{
        //    public int ID { get; set; }
        //    public float Abundance { get; set; }
        //    public double Mass { get; set; }
        //    public double RetentionTime { get; set; }
        //    public double DriftTime { get; set; }
        //    public int ChargeState { get; set; }
        //    public double MassMonoisotopic { get; set; }
        //    public double Score { get; set; }
        //}

        //public class DatabaseElutingPeakObject
        //{
        //    public int ElutingPeakID { get; set; }
        //    public double ElutingPeakMass { get; set; }

        //    public int ElutingPeakScanStart { get; set; }
        //    public int ElutingPeakScanEnd { get; set; }
        //    public int ElutingPeakScanMaxIntensity { get; set; }

        //    public int ElutingPeakNumberofPeaks { get; set; }
        //    public int ElutingPeakNumberOfPeaksFlag { get; set; }
        //    public string ElutingPeakNumberOfPeaksMode { get; set; }

        //    public float ElutingPeakSummedIntensity { get; set; }
        //    public double ElutingPeakIntensityAggregate { get; set; }
        //}

        #endregion


        public static List<MSSpectra> ConvertSpectraType(List<MSSpectra> spectraListIn, ScanTypeOptions chooseScanType)
        {
             //always send the parent into this function
            List<MSSpectra> spectraList = new List<MSSpectra>();
            switch (chooseScanType)
            {
                 case ScanTypeOptions.Parent:
                    {
                        spectraList = spectraListIn;
                    }
                    break;
                    case ScanTypeOptions.Child:
                    {
                        foreach (MSSpectra spectra in spectraListIn)
                        {
                            foreach (MSSpectra childspectra in spectra.ChildSpectra)
                            {
                                spectraList.Add(childspectra);
                            }
                        }
                    }
                    break;
                default:
                    {
                        spectraList = spectraListIn;
                    }
                    break;
            }
            return spectraList;
        }

        public enum ScanTypeOptions
        {
            Parent,
            Child
        }
    }

}
