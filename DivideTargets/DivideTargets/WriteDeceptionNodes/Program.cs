using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SleepDLL;

namespace WriteDeceptionNodes
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> nodeNames = new List<string>();
            List<string> macAddress = new List<string>();
            nodeNames.Add("Deception-0205"); macAddress.Add("0025905218a5");
            nodeNames.Add("Deception-0206 "); macAddress.Add("002590523abd");
            nodeNames.Add("Deception-0207 "); macAddress.Add("2590548887");
            nodeNames.Add("Deception-0208"); macAddress.Add("002590520f41");
            nodeNames.Add("Deception-0209"); macAddress.Add("00259052cf97");
            nodeNames.Add("Deception-0210"); macAddress.Add("00259052d035");
            nodeNames.Add("Deception-0211"); macAddress.Add("2590521239");
            nodeNames.Add("Deception-0212"); macAddress.Add("00259052cfa7");
            nodeNames.Add("Deception-0213"); macAddress.Add("002590520ee7");
            nodeNames.Add("Deception-0214"); macAddress.Add("002590520f95");
            nodeNames.Add("Deception-0215"); macAddress.Add("0025905217bd");
            nodeNames.Add("Deception-0216"); macAddress.Add("0025905217a7");
            nodeNames.Add("Deception-0217"); macAddress.Add("00259055ca25");
            nodeNames.Add("Deception-0218"); macAddress.Add("00259052cfdd");
            nodeNames.Add("Deception-0219"); macAddress.Add("00259052d1ff");
            nodeNames.Add("Deception-0220"); macAddress.Add("00259052d1e3");
            nodeNames.Add("Deception-0221"); macAddress.Add("00259052172d");
            nodeNames.Add("Deception-0222"); macAddress.Add("00259052d175");
            nodeNames.Add("Deception-0223"); macAddress.Add("00259052d1d9");
            nodeNames.Add("Deception-0224"); macAddress.Add("00259052d1e9");
            nodeNames.Add("Deception-0225"); macAddress.Add("00259052cfa3");
            nodeNames.Add("Deception-0226"); macAddress.Add("00259052d07f");
            nodeNames.Add("Deception-0227"); macAddress.Add("00259052120d");
            nodeNames.Add("Deception-0228"); macAddress.Add("00259052d08f");
            nodeNames.Add("Deception-0229"); macAddress.Add("00259052ce9b");
            nodeNames.Add("Deception-0230"); macAddress.Add("00259052d097");
            nodeNames.Add("Deception-0231"); macAddress.Add("00259052d15d");
            nodeNames.Add("Deception-0232"); macAddress.Add("00259052cf47");
            nodeNames.Add("Deception-0233"); macAddress.Add("2590521959");
            nodeNames.Add("Deception-0234"); macAddress.Add("0025905218a3");
            nodeNames.Add("Deception-0235"); macAddress.Add("00259052d0dd");
            nodeNames.Add("Deception-0236"); macAddress.Add("002590521aa7");
            nodeNames.Add("Deception-0237"); macAddress.Add("00259052cf55");
            nodeNames.Add("Deception-0238"); macAddress.Add("00259052d1cf");
            nodeNames.Add("Deception-0239"); macAddress.Add("00259052ce79");
            nodeNames.Add("Deception-0240"); macAddress.Add("00259052d0c9");
            nodeNames.Add("Deception-0241"); macAddress.Add("002590523a57");
            nodeNames.Add("Deception-0242"); macAddress.Add("00259052cf39");
            nodeNames.Add("Deception-0243"); macAddress.Add("2590521795");
            nodeNames.Add("Deception-0244"); macAddress.Add("2590521829");
            nodeNames.Add("Deception-0245"); macAddress.Add("00259052d227");
            nodeNames.Add("Deception-0246"); macAddress.Add("00259052d223");
            nodeNames.Add("Deception-0247"); macAddress.Add("00259052cfc5");
            nodeNames.Add("Deception-0248"); macAddress.Add("0025905214b5");
            nodeNames.Add("Deception-0249"); macAddress.Add("00259052d1b7");
            nodeNames.Add("Deception-0250"); macAddress.Add("00259052d1b3");
            nodeNames.Add("Deception-0251"); macAddress.Add("00259052d105");
            nodeNames.Add("Deception-0252"); macAddress.Add("00259052d1cd");

            StringListToDisk writer = new StringListToDisk();
            List<string> lines = new List<string>();

            string q = "\"";

            string header1 = @"<?xml version=" + q + "1.0" + q + " encoding=" + q + "utf-8" + q + "?>";
            string header2 = @"<Nodes xmlns:xsi=" + q + "http://www.w3.org/2001/XMLSchema-instance" + q + " xmlns:xsd=" + q + "http://www.w3.org/2001/XMLSchema" + q + " xmlns=" + q + "http://schemas.microsoft.com/HpcNodeConfigurationFile/2007/12" + q + ">";

            string footer1 = @"</Nodes>";
             
            lines.Add(header1);
            lines.Add(header2);
            lines.Add("");
            
            for (int i = 0; i < nodeNames.Count; i++)
            {

                string xml_1 = @"  <Node";
                string xml_2 = @"    Name=" + q + nodeNames[i] + q;
                string xml_3 = @"    Domain=" + q + "PNL" + q + ">";
                string xml_4 = @"    <MacAddress>" + macAddress[i] + @"</MacAddress>";
                string xml_5 = @"  </Node>";

                lines.Add(xml_1);
                lines.Add(xml_2);
                lines.Add(xml_3);
                lines.Add(xml_4);
                lines.Add(xml_5);
            }
            lines.Add("");
            lines.Add(footer1);

            string writeLocation = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Deception2" + @"\" + "NodesAndMAC.xml";
            writer.toDiskStringList(writeLocation, lines);
        }
    }
}
