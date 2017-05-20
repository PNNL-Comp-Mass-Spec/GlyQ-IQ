using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiDatasetToolBox;

namespace FilesAndFolders
{
    public class GetCoefficients
    {

        public static List<LinearRegressionResult> GetDiabetesLCCoefficients()
        {
            List<LinearRegressionResult> dataLoaded = new List<LinearRegressionResult>();
            dataLoaded.Add(new LinearRegressionResult("C15_DB01_30Dec12_1_Family_iqResults", 0.00547703763689839,0.0347142182296491));
            dataLoaded.Add(new LinearRegressionResult("C14_DB02_30Dec12_1_Family_iqResults", -0.0185533051185462,0.00456322141898872));
            dataLoaded.Add(new LinearRegressionResult("C15_DB03_30Dec12_1_Family_iqResults", 0.0155253651402528,-0.0080059345930974));
            dataLoaded.Add(new LinearRegressionResult("C14_DB04_30Dec12_1_Family_iqResults", 0.0250165969248927,-0.014463258674328));
            dataLoaded.Add(new LinearRegressionResult("C15_DB05_30Dec12_1_Family_iqResults", 0.034410795296358,-0.0144314563486639));
            dataLoaded.Add(new LinearRegressionResult("C14_DB06_31Dec12_1_Family_iqResults", -0.0172017399565156,0.0464692379470432));
            dataLoaded.Add(new LinearRegressionResult("C15_DB07_31Dec12_1_Family_iqResults", -0.0107783988306264,-0.00204358827602165));
            dataLoaded.Add(new LinearRegressionResult("C14_DB08_31Dec12_1_Family_iqResults", 0.0371902139704919,-0.0146915032921703));
            dataLoaded.Add(new LinearRegressionResult("C15_DB09_31Dec12_1_Family_iqResults", -0.00683716247830402,0.00118136385771364));
            dataLoaded.Add(new LinearRegressionResult("C14_DB10_31Dec12_1_Family_iqResults", 0, 0));
            dataLoaded.Add(new LinearRegressionResult("C15_DB11_01Jan13_1_Family_iqResults", 0.0332777009181306,0.0227947891577253));
            dataLoaded.Add(new LinearRegressionResult("C14_DB12_01Jan13_1_Family_iqResults", 0.00987826560747646,0.00385804281541916));
            dataLoaded.Add(new LinearRegressionResult("C15_DB13_01Jan13_1_Family_iqResults", 0.0432146148528225,-0.0195614257053749));
            dataLoaded.Add(new LinearRegressionResult("C14_DB14_01Jan13_1_Family_iqResults", 0.0357744616124274,-0.0161484670158182));
            dataLoaded.Add(new LinearRegressionResult("C15_DB15_01Jan13_1_Family_iqResults", 0.04927694677265,-0.0171190488830381));
            dataLoaded.Add(new LinearRegressionResult("C14_DB16_01Jan13_1_Family_iqResults", -0.0544278672585406,0.0563831094259534));
            dataLoaded.Add(new LinearRegressionResult("C15_DB17_01Jan13_1_Family_iqResults", 0.000127475751837418,0.00861685858113928));
            dataLoaded.Add(new LinearRegressionResult("C14_DB18_01Jan13_1_Family_iqResults", 0.00849036212752382,-0.00685721034965564));
            dataLoaded.Add(new LinearRegressionResult("C15_DB19_01Jan13_1_Family_iqResults", 0.0867981325202858,-0.0310650069968422));
            dataLoaded.Add(new LinearRegressionResult("C14_DB20_01Jan13_1_Family_iqResults", 0.0860033477208023,-0.0236283841761588));
            dataLoaded.Add(new LinearRegressionResult("SN111SN114_1_Family_iqResults", 0.012625343668611, -0.0015192651331106));
            dataLoaded.Add(new LinearRegressionResult("SN112SN115_1_Family_iqResults", 0.0627001515799234, -0.0151718858193072));
            dataLoaded.Add(new LinearRegressionResult("SN113SN116_1_Family_iqResults", 0.0403288364435863, -0.00712680415655761));
            dataLoaded.Add(new LinearRegressionResult("SN117SN120_1_Family_iqResults", 0.171675076182036, -0.0554543639416045));
            dataLoaded.Add(new LinearRegressionResult("SN118SN121_1_Family_iqResults", 0.13947057046587, -0.0360932079342126));
            dataLoaded.Add(new LinearRegressionResult("SN119SN122_1_Family_iqResults", 0.150799046513072, -0.0289083245425121));
            return dataLoaded;
        }

    }
}
