using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using DeconTools.Utilities;
using DeconTools.Workflows.Backend;
using DeconTools.Workflows.Backend.Results;

namespace Sipper.Model
{
    public class HTMLReportGenerator
    {

        private TargetedResultRepository _resultRepository;
        private FileInputsInfo _fileInputsInfo;
        private List<string> _imageFilePaths;

        private ICollection<ResultWithImageInfo> _resultsWithImageInfo;
        private const int MSImageWidth = 400;
        private const int MSImageHeight = 350;

        private const int TheorMSImageWidth = 400;
        private const int TheorMSImageHeight = 350;

        private const int ChromImageWidth = 275;
        private const int ChromImageHeight = 350;


        #region Constructors

        public HTMLReportGenerator(ICollection<ResultWithImageInfo> resultImages, FileInputsInfo fileInputs)
        {
            _resultsWithImageInfo = resultImages;
            _fileInputsInfo = fileInputs;

            Check.Require(_fileInputsInfo != null, "FileInputs object is null");



        }


        public HTMLReportGenerator(TargetedResultRepository repo, FileInputsInfo fileInputs)
        {
            _resultRepository = repo;

            _fileInputsInfo = fileInputs;

            _resultsWithImageInfo = new List<ResultWithImageInfo>();

            foreach (var targetedResultDto in repo.Results)
            {
                ResultWithImageInfo r = new ResultWithImageInfo((SipperLcmsFeatureTargetedResultDTO)targetedResultDto);

                _resultsWithImageInfo.Add(r);
            }


            GetImageFileReferences(_fileInputsInfo.ResultImagesFolderPath);
            MapResultsToImages();

        }



        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public void GenerateHTMLReport()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(CreateHeaderHTML());


            foreach (var resultWithImageInfo in _resultsWithImageInfo)
            {
                stringBuilder.Append(getHTMLForTableRow(resultWithImageInfo));
            }

            stringBuilder.Append(CloseHTMLTags());

            string outputHTMLFilename = _fileInputsInfo.ResultImagesFolderPath + Path.DirectorySeparatorChar + "0_index.html";

            using (StreamWriter sw = new StreamWriter(outputHTMLFilename))
            {
                sw.Write(stringBuilder.ToString());

            }

        }


        #endregion

        #region Private Methods

        private void GetImageFileReferences(string resultImagesFolderPath)
        {

            DirectoryInfo directoryInfo = new DirectoryInfo(resultImagesFolderPath);

            if (directoryInfo.Exists)
            {
                _imageFilePaths = directoryInfo.GetFiles("*.png", SearchOption.AllDirectories).Select(p => p.FullName).ToList();
            }

        }



        private void MapResultsToImages()
        {

            foreach (var result in _resultsWithImageInfo)
            {

                string baseFileName = result.Result.DatasetName + "_ID" + result.Result.TargetID;

                var targetImages = (from n in _imageFilePaths where n.Contains(baseFileName) select n).ToList();

                string expectedMSImage = targetImages.First(p => p.Contains("_MS.png"));
                string expectedChromImageFilename = targetImages.First(p => p.Contains("_chrom.png"));
                string expectedTheorMSImageFilename = targetImages.First(p => p.Contains("_theorMS.png"));

                result.MSImageFilePath = expectedMSImage ?? "";
                result.ChromImageFilePath = expectedChromImageFilename ?? "";
                result.TheorMSImageFilePath = expectedTheorMSImageFilename ?? "";
            }


        }


        private string CreateHeaderHTML()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");

            sb.Append("<html>");
            sb.Append("<body>");
            sb.Append("<table>");

            return sb.ToString();
        }

        private string CloseHTMLTags()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("</table>");
            sb.Append("</body>");
            sb.Append("</html>");
            return sb.ToString();
        }


        private string getHTMLForTableRow(ResultWithImageInfo resultWithImageInfo)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                //add feature info table to a cell within the table

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                addResultInfoTable(writer, resultWithImageInfo);
                writer.RenderEndTag();

                addHTMLForChromImage(writer, resultWithImageInfo);

                addHTMLForTheorMSImage(writer, resultWithImageInfo);

                addHTMLForMSImage(writer, resultWithImageInfo);

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                addAnnotationTable(writer, resultWithImageInfo);
                writer.RenderEndTag();

                //this is a hook so that sub-classes can add additional stuff here
                //addHTMLForOtherData(writer);
                writer.RenderEndTag();

            }
            return stringWriter.ToString();
        }

        private void addAnnotationTable(HtmlTextWriter writer, ResultWithImageInfo resultWithImageInfo)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "color:black; font-style:bold; font-family:Calibri; font-size:125%");
            writer.AddAttribute(HtmlTextWriterAttribute.Width, "100");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            if (resultWithImageInfo.Result.ValidationCode == ValidationCode.None)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "2");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "Black");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "Solid");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "White");

            }
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("None");
            writer.RenderEndTag();
            writer.RenderEndTag();


            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            if (resultWithImageInfo.Result.ValidationCode == ValidationCode.Yes)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "2");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "Black");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "Solid");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "LightGreen");

            }
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("Yes");
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            if (resultWithImageInfo.Result.ValidationCode == ValidationCode.No)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "2");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "Black");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "Solid");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "Tomato");

            }
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("No");
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            if (resultWithImageInfo.Result.ValidationCode == ValidationCode.Maybe)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "2");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderColor, "Black");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, "Solid");
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "LightYellow");

            }
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("Maybe");
            writer.RenderEndTag();
            writer.RenderEndTag();


            writer.RenderEndTag();



        }


        private void addResultInfoTable(HtmlTextWriter writer, ResultWithImageInfo resultWithImageInfo)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "color:firebrick; font-style:bold; font-family:Calibri; font-size:100%");
            writer.AddAttribute(HtmlTextWriterAttribute.Width, "200");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);


            writer.RenderBeginTag(HtmlTextWriterTag.Tr);




            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("TargetID");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.TargetID);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("MassTagID");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.MatchedMassTagID);
            writer.RenderEndTag();
            writer.RenderEndTag();



            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("Scan");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.ScanLC);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("Intensity");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.Intensity);
            writer.RenderEndTag();
            writer.RenderEndTag();


            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("MonoMass");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.MonoMass.ToString("0.0000"));
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("MonoMZ");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.MonoMZ.ToString("0.0000"));
            writer.RenderEndTag();
            writer.RenderEndTag();


            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("z");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.ChargeState);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("i_score");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.IScore);
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("RawAreaMetric");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.AreaUnderRatioCurve.ToString("0.0"));
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("RevisedArea");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.AreaUnderRatioCurveRevised.ToString("0.0"));
            writer.RenderEndTag();
            writer.RenderEndTag();


            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("Linearity");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.RSquaredValForRatioCurve.ToString("0.000"));
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("ChromCorr_Med");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.ChromCorrelationMedian.ToString("0.000"));
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("ChromCorr_Avg");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.ChromCorrelationAverage.ToString("0.000"));
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write("ChromCorr_StDev");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(resultWithImageInfo.Result.ChromCorrelationStdev.ToString("0.0000"));
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.RenderEndTag();
        }


        protected void addHTMLForMSImage(HtmlTextWriter writer, ResultWithImageInfo resultWithImageInfo)
        {

            string relFilePath = GetRelativeFilePath(resultWithImageInfo.MSImageFilePath);



            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddAttribute(HtmlTextWriterAttribute.Href, resultWithImageInfo.MSImageFilePath);
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, relFilePath);
            writer.AddAttribute(HtmlTextWriterAttribute.Width, MSImageWidth.ToString("0"));
            writer.AddAttribute(HtmlTextWriterAttribute.Height, MSImageHeight.ToString("0"));
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        private string GetRelativeFilePath(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo != null)
            {
                var dir = fileInfo.Directory != null ? fileInfo.Directory.Name : String.Empty;

                return  dir + "/" + fileInfo.Name;
            }

            return String.Empty;


        }


        protected void addHTMLForChromImage(HtmlTextWriter writer, ResultWithImageInfo resultWithImageInfo)
        {

            string relFilePath = GetRelativeFilePath(resultWithImageInfo.ChromImageFilePath);


            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddAttribute(HtmlTextWriterAttribute.Href, relFilePath);
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, relFilePath);
            writer.AddAttribute(HtmlTextWriterAttribute.Width, ChromImageWidth.ToString("0"));
            writer.AddAttribute(HtmlTextWriterAttribute.Height, ChromImageHeight.ToString("0"));
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }


        protected void addHTMLForTheorMSImage(HtmlTextWriter writer, ResultWithImageInfo resultWithImageInfo)
        {
            string relFilePath = GetRelativeFilePath(resultWithImageInfo.TheorMSImageFilePath);


            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddAttribute(HtmlTextWriterAttribute.Href, relFilePath);
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, relFilePath);
            writer.AddAttribute(HtmlTextWriterAttribute.Width, TheorMSImageWidth.ToString("0"));
            writer.AddAttribute(HtmlTextWriterAttribute.Height, TheorMSImageHeight.ToString("0"));
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }







        #endregion

    }
}
