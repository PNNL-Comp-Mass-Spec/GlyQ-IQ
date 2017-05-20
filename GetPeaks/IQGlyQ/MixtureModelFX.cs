using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using Accord.MachineLearning;
using Accord.Statistics.Distributions.Multivariate;

namespace IQGlyQ
{
    public static class MixtureModelFX
    {
        public static void MixtureModel(List<XYData> loadFragmentBaseEic, int numberOfPeaks)
        {
            Console.WriteLine("We are looking for " + numberOfPeaks + "peaks");

            double[][] sampleArray = new double[loadFragmentBaseEic.Count][];

            double[] xValues = { -2, -1.9, -1.8, -1.7, -1.6, -1.5, -1.4, -1.3, -1.2, -1.1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 2.8, 2.9, 3, 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9 };
            int[] yValues = { 0, 0, 0, 0, 0, 1, 2, 5, 11, 23, 44, 79, 135, 216, 325, 458, 608, 759, 892, 988, 1032, 1023, 968, 884, 793, 716, 667, 651, 667, 704, 748, 786, 805, 799, 765, 705, 625, 532, 436, 343, 259, 188, 131, 88, 57, 35, 21, 12, 7, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
            List<int> normalizedArray = new List<int>();
            double normalizationFactor = 10000;
            double maxYValue = 0;

            bool switcher = false;
            if (switcher)
            {
                bool useManualData = true;
                if (useManualData)
                {
                    List<XYData> manualData = GetManualDataShorter2Peak(); numberOfPeaks = 2;
                    //List<XYData> manualData = GetManualDataShorter1Peak();numberOfPeaks = 1;
                    xValues = new double[manualData.Count];
                    yValues = new int[manualData.Count];
                    for(int i=0;i<manualData.Count;i++)
                    {
                        xValues[i] = manualData[i].X;
                        yValues[i] = Convert.ToInt32(manualData[i].Y);
                    }
                    
                }
                
                List<double> sampleList = new List<double>();

                for (int f = 0; f < xValues.Length; f++)
                {
                    int numObservations = yValues[f];
                    double valueToObserve = xValues[f];

                    for (int h = 0; h < numObservations; h++)
                    {
                        sampleList.Add(valueToObserve);
                    }
                }

                sampleArray = new double[sampleList.Count][];
                for (int f = 0; f < sampleList.Count; f++)
                {
                    sampleArray[f] = new double[] { sampleList[f] };
                }
            }
            else
            {
                //normalize Y.  we will un normalize later
                normalizedArray = new List<int>();

                bool useManualData = true;
                if (useManualData)
                {
                    //List<XYData> manualData = GetManualDataShorter2Peak(); numberOfPeaks = 2;
                    List<XYData> manualData = GetManualDataShorter1Peak();numberOfPeaks = 1;
                    
                    loadFragmentBaseEic = manualData;
                }

                foreach (var point in loadFragmentBaseEic)
                {
                    if (point.Y > maxYValue)
                    {
                        maxYValue = point.Y;
                    }
                }


                foreach (var point in loadFragmentBaseEic)
                {
                    double newValue = point.Y / maxYValue * normalizationFactor;
                    //double newValue = point.Y;
                    normalizedArray.Add(Convert.ToInt32(Math.Round(newValue, 0)));
                }


                //we need to convert the xydata into a 1D array  when y=2, we need 2 corresponding x values, y=3, we need 3 values  00,111, etc
                List<double> sampleList = new List<double>();

                for (int i = 0; i < loadFragmentBaseEic.Count; i++)
                {
                    int numObservations = normalizedArray[i];
                    //int numObservations = Convert.ToInt32(Math.Round(loadFragmentBaseEic[i].Y,0));
                    double valueToObserve = Math.Round(loadFragmentBaseEic[i].X,4);

                    for (int j = 0; j < numObservations; j++)
                    {
                        sampleList.Add(valueToObserve);//add copies
                    }
                }

                sampleArray = new double[sampleList.Count][];
                for (int i = 0; i < sampleList.Count; i++)
                {
                    sampleArray[i] = new double[] { sampleList[i] };
                }
            }


            //NormalDistribution[] components = new NormalDistribution[2];
            //components[0] = new NormalDistribution(2, 1);
            //components[1] = new NormalDistribution(5, 1);

            //MultivariateNormalDistribution[] compoentnes2 = new MultivariateNormalDistribution[2];


            //http://crsouza.blogspot.com/2010/10/gaussian-mixture-models-and-expectation.html#!/2010/10/gaussian-mixture-models-and-expectation.html

            GaussianMixtureModel model = new GaussianMixtureModel(numberOfPeaks);
            //GaussianMixtureModel gmm = new GaussianMixtureModel(5);

            //gmm.Compute(sampleArray, 0.000001);
            model.Compute(sampleArray, 0.000001);

            MultivariateMixture<MultivariateNormalDistribution> mixture = model.ToMixtureDistribution();
            MultivariateNormalDistribution[] mixtureComponents = mixture.Components;

            MultivariateNormalDistribution normalDistribution1; 
            MultivariateNormalDistribution normalDistribution2;

            List<string> strings = new List<string>();

            if (switcher)
            {
                normalDistribution1 = mixtureComponents[0];
                normalDistribution2 = mixtureComponents[1];

                //for (double j = -2; j < 4; j += 0.1)
                //{
                //    int printFactor = 1000;
                //    strings.Add("Fit\t " + j + "\t" + normalDistribution1.ProbabilityDensityFunction(new double[] { j }) * printFactor + "\t" + normalDistribution2.ProbabilityDensityFunction(new double[] { j }) * printFactor);
                //}

                for (int j = 0; j < yValues.Length; j++)
                {
                    double xCoordinate = xValues[j];
                    int printFactor = 1000;
                    strings.Add("Fit\t " + j + "\t" + normalDistribution1.ProbabilityDensityFunction(new double[] { xCoordinate }) * printFactor + "\t" + normalDistribution2.ProbabilityDensityFunction(new double[] { xCoordinate }) * printFactor);
                }

                for (int g = 0; g < yValues.Length; g++)
                {
                    sampleArray[g] = new double[] { xValues[g] };
                    Console.WriteLine(sampleArray[g][0] + "\t" + yValues[g] + "\t" + strings[g]);
                }
            }
            else
            {
                for (int i = 0; i < loadFragmentBaseEic.Count; i++)
                //for (double j = loadFragmentBaseEic[0].X; j < loadFragmentBaseEic[loadFragmentBaseEic.Count - 1].X; j += 3)
                {
                    double j = loadFragmentBaseEic[i].X;

                    string line = "Fit\t " + j + "\t";

                    foreach (MultivariateNormalDistribution normalDistribution in mixtureComponents)
                    {
                        int printFactor = 1;
                        line += Convert.ToString(Math.Round(normalDistribution.ProbabilityDensityFunction(new double[] { j }) * printFactor, 5)) + "\t";
                    }
                    strings.Add(line);
                }

                for (int g = 0; g < loadFragmentBaseEic.Count; g++)
                {
                    sampleArray[g] = new double[] { loadFragmentBaseEic[g].X };
                    Console.WriteLine(Math.Round(sampleArray[g][0],5) + "\t" + normalizedArray[g] + "\t" + Math.Round(loadFragmentBaseEic[g].Y,5) + "\t" + strings[g]);
                }
            }

            Console.WriteLine("MixtureModel");
            Console.ReadKey();

        }

        private static List<XYData> GetManualData()
        {
            List<XYData> manualData = new List<XYData>();
            manualData.Add(new XYData(-0.141999999999825, 9.3977766913991E-16));
            manualData.Add(new XYData(-0.141499999999951, 1.41917842291121E-15));
            manualData.Add(new XYData(-0.140999999999849, 2.13870840059866E-15));
            manualData.Add(new XYData(-0.140499999999975, 3.21639107530381E-15));
            manualData.Add(new XYData(-0.139999999999873, 4.82712804173417E-15));
            manualData.Add(new XYData(-0.139499999999998, 7.22955367798538E-15));
            manualData.Add(new XYData(-0.138999999999896, 1.08053005613152E-14));
            manualData.Add(new XYData(-0.138500000000022, 1.61162840880328E-14));
            manualData.Add(new XYData(-0.13799999999992, 2.39880902501257E-14));
            manualData.Add(new XYData(-0.137499999999818, 3.56310918984119E-14));
            manualData.Add(new XYData(-0.136999999999944, 5.28159731081345E-14));
            manualData.Add(new XYData(-0.136499999999842, 7.81275378156437E-14));
            manualData.Add(new XYData(-0.135999999999967, 1.1533090156682E-13));
            manualData.Add(new XYData(-0.135499999999865, 1.69898652192652E-13));
            manualData.Add(new XYData(-0.134999999999991, 2.49768046759123E-13));
            manualData.Add(new XYData(-0.134499999999889, 3.66426200084306E-13));
            manualData.Add(new XYData(-0.134000000000015, 5.36461868040639E-13));
            manualData.Add(new XYData(-0.133499999999913, 7.83779488042665E-13));
            manualData.Add(new XYData(-0.132999999999811, 1.14275104450466E-12));
            manualData.Add(new XYData(-0.132499999999936, 1.66269295880029E-12));
            manualData.Add(new XYData(-0.131999999999834, 2.41421078490543E-12));
            manualData.Add(new XYData(-0.13149999999996, 3.49817080644758E-12));
            manualData.Add(new XYData(-0.130999999999858, 5.05835766278279E-12));
            manualData.Add(new XYData(-0.130499999999984, 7.29929227797399E-12));
            manualData.Add(new XYData(-0.129999999999882, 1.05112574057507E-11));
            manualData.Add(new XYData(-0.129500000000007, 1.51053671717564E-11));
            manualData.Add(new XYData(-0.128999999999905, 2.16626008943143E-11));
            manualData.Add(new XYData(-0.128499999999804, 3.10022067354892E-11));
            manualData.Add(new XYData(-0.127999999999929, 4.42769110377376E-11));
            manualData.Add(new XYData(-0.127499999999827, 6.31051377526677E-11));
            manualData.Add(new XYData(-0.126999999999953, 8.97542083437484E-11));
            manualData.Add(new XYData(-0.126499999999851, 1.27393602683659E-10));
            manualData.Add(new XYData(-0.125999999999976, 1.80444278425828E-10));
            manualData.Add(new XYData(-0.125499999999874, 2.55059385790145E-10));
            manualData.Add(new XYData(-0.125, 3.59784293792921E-10));
            manualData.Add(new XYData(-0.124499999999898, 5.06460744241411E-10));
            manualData.Add(new XYData(-0.124000000000024, 7.11462599027132E-10));
            manualData.Add(new XYData(-0.123499999999922, 9.97380923930713E-10));
            manualData.Add(new XYData(-0.12299999999982, 1.3953165258741E-09));
            manualData.Add(new XYData(-0.122499999999945, 1.94799176101689E-09));
            manualData.Add(new XYData(-0.121999999999844, 2.713964676405E-09));
            manualData.Add(new XYData(-0.121499999999969, 3.77332283004982E-09));
            manualData.Add(new XYData(-0.120999999999867, 5.23535858396059E-09));
            manualData.Add(new XYData(-0.120499999999993, 7.24889151381529E-09));
            manualData.Add(new XYData(-0.119999999999891, 1.00161187465049E-08));
            manualData.Add(new XYData(-0.119500000000016, 1.38111558664201E-08));
            manualData.Add(new XYData(-0.118999999999915, 1.90047992188659E-08));
            manualData.Add(new XYData(-0.118499999999813, 2.60975201934114E-08));
            manualData.Add(new XYData(-0.117999999999938, 3.57633255625202E-08));
            manualData.Add(new XYData(-0.117499999999836, 4.89079261166887E-08));
            manualData.Add(new XYData(-0.116999999999962, 6.67457006084038E-08));
            manualData.Add(new XYData(-0.11649999999986, 9.09012890785991E-08));
            manualData.Add(new XYData(-0.115999999999985, 1.23543381819873E-07));
            manualData.Add(new XYData(-0.115499999999884, 1.67560491658735E-07));
            manualData.Add(new XYData(-0.115000000000009, 2.26791338414649E-07));
            manualData.Add(new XYData(-0.114499999999907, 3.06326098357201E-07));
            manualData.Add(new XYData(-0.113999999999805, 4.12899381286205E-07));
            manualData.Add(new XYData(-0.113499999999931, 5.55401645577565E-07));
            manualData.Add(new XYData(-0.112999999999829, 7.45543159091175E-07));
            manualData.Add(new XYData(-0.112499999999955, 9.98713945936763E-07));
            manualData.Add(new XYData(-0.111999999999853, 1.33509489865136E-06));
            manualData.Add(new XYData(-0.111499999999978, 1.78108996173295E-06));
            manualData.Add(new XYData(-0.110999999999876, 2.3711677130774E-06));
            manualData.Add(new XYData(-0.110500000000002, 3.15022364534053E-06));
            manualData.Add(new XYData(-0.1099999999999, 4.17660302304908E-06));
            manualData.Add(new XYData(-0.109500000000025, 5.52595962393797E-06));
            manualData.Add(new XYData(-0.108999999999924, 7.29616948165642E-06));
            manualData.Add(new XYData(-0.108499999999822, 9.61357275088523E-06));
            manualData.Add(new XYData(-0.107999999999947, 1.26408831881181E-05));
            manualData.Add(new XYData(-0.107499999999845, 1.65871860675004E-05));
            manualData.Add(new XYData(-0.106999999999971, 2.17205446953275E-05));
            manualData.Add(new XYData(-0.106499999999869, 2.83838566653896E-05));
            manualData.Add(new XYData(-0.105999999999995, 3.70147478602181E-05));
            manualData.Add(new XYData(-0.105499999999893, 4.81704699232196E-05));
            manualData.Add(new XYData(-0.105000000000018, 6.25589812961071E-05));
            manualData.Add(new XYData(-0.104499999999916, 8.10776496506894E-05));
            manualData.Add(new XYData(-0.103999999999814, 0.000104861322392548));
            manualData.Add(new XYData(-0.10349999999994, 0.000135341880771663));
            manualData.Add(new XYData(-0.102999999999838, 0.000174321832159524));
            manualData.Add(new XYData(-0.102499999999964, 0.000224065015779256));
            manualData.Add(new XYData(-0.101999999999862, 0.000287408112628826));
            manualData.Add(new XYData(-0.101499999999987, 0.000367897375131085));
            manualData.Add(new XYData(-0.100999999999885, 0.000469955842467884));
            manualData.Add(new XYData(-0.100500000000011, 0.000599087301638401));
            manualData.Add(new XYData(-0.0999999999999091, 0.000762124411827605));
            manualData.Add(new XYData(-0.0994999999998072, 0.000967529752250158));
            manualData.Add(new XYData(-0.0989999999999327, 0.00122576010452821));
            manualData.Add(new XYData(-0.0984999999998308, 0.00154970606473627));
            manualData.Add(new XYData(-0.0979999999999563, 0.00195522112375824));
            manualData.Add(new XYData(-0.0974999999998545, 0.00246175668490011));
            manualData.Add(new XYData(-0.09699999999998, 0.00309312213282709));
            manualData.Add(new XYData(-0.0964999999998781, 0.0038783920560445));
            manualData.Add(new XYData(-0.0960000000000036, 0.00485298608399772));
            manualData.Add(new XYData(-0.0954999999999018, 0.00605995055574767));
            manualData.Add(new XYData(-0.0950000000000273, 0.00755147541401873));
            manualData.Add(new XYData(-0.0944999999999254, 0.00939068433654632));
            manualData.Add(new XYData(-0.0939999999998236, 0.0116537411913572));
            manualData.Add(new XYData(-0.0934999999999491, 0.0144323214424854));
            manualData.Add(new XYData(-0.0929999999998472, 0.017836503137659));
            manualData.Add(new XYData(-0.0924999999999727, 0.0219981385689748));
            manualData.Add(new XYData(-0.0919999999998709, 0.0270747745877448));
            manualData.Add(new XYData(-0.0914999999999964, 0.0332541968361689));
            manualData.Add(new XYData(-0.0909999999998945, 0.0407596807735384));
            manualData.Add(new XYData(-0.09050000000002, 0.0498560402443297));
            manualData.Add(new XYData(-0.0899999999999181, 0.0608565723566319));
            manualData.Add(new XYData(-0.0894999999998163, 0.0741310054814331));
            manualData.Add(new XYData(-0.0889999999999418, 0.0901145650857729));
            manualData.Add(new XYData(-0.0884999999998399, 0.10931827968204));
            manualData.Add(new XYData(-0.0879999999999654, 0.132340656182637));
            manualData.Add(new XYData(-0.0874999999998636, 0.159880860127062));
            manualData.Add(new XYData(-0.0869999999999891, 0.192753541291159));
            manualData.Add(new XYData(-0.0864999999998872, 0.231905448749877));
            manualData.Add(new XYData(-0.0860000000000127, 0.278433981159247));
            manualData.Add(new XYData(-0.0854999999999109, 0.333607817425698));
            manualData.Add(new XYData(-0.084999999999809, 0.398889769580148));
            manualData.Add(new XYData(-0.0844999999999345, 0.475961993077055));
            manualData.Add(new XYData(-0.0839999999998327, 0.566753679374825));
            manualData.Add(new XYData(-0.0834999999999582, 0.673471340984347));
            manualData.Add(new XYData(-0.0829999999998563, 0.798631779648488));
            manualData.Add(new XYData(-0.0824999999999818, 0.945097803390612));
            manualData.Add(new XYData(-0.0819999999998799, 1.11611672731448));
            manualData.Add(new XYData(-0.0815000000000055, 1.31536165575351));
            manualData.Add(new XYData(-0.0809999999999036, 1.54697549920734));
            manualData.Add(new XYData(-0.0804999999998017, 1.81561762809281));
            manualData.Add(new XYData(-0.0799999999999272, 2.1265130063928));
            manualData.Add(new XYData(-0.0794999999998254, 2.48550358164652));
            manualData.Add(new XYData(-0.0789999999999509, 2.89910163336744));
            manualData.Add(new XYData(-0.078499999999849, 3.37454470004902));
            manualData.Add(new XYData(-0.0779999999999745, 3.91985161576496));
            manualData.Add(new XYData(-0.0774999999998727, 4.54387909155386));
            manualData.Add(new XYData(-0.0769999999999982, 5.25637817510169));
            manualData.Add(new XYData(-0.0764999999998963, 6.06804981577102));
            manualData.Add(new XYData(-0.0760000000000218, 6.99059865212247));
            manualData.Add(new XYData(-0.07549999999992, 8.03678402737938));
            manualData.Add(new XYData(-0.0749999999998181, 9.22046712674592));
            manualData.Add(new XYData(-0.0744999999999436, 10.5566530213511));
            manualData.Add(new XYData(-0.0739999999998417, 12.0615262993972));
            manualData.Add(new XYData(-0.0734999999999673, 13.7524788686595));
            manualData.Add(new XYData(-0.0729999999998654, 15.64812842888));
            manualData.Add(new XYData(-0.0724999999999909, 17.7683260411042));
            manualData.Add(new XYData(-0.071999999999889, 20.134151167064));
            manualData.Add(new XYData(-0.0715000000000146, 22.7678925188628));
            manualData.Add(new XYData(-0.0709999999999127, 25.6930130510441));
            manualData.Add(new XYData(-0.0704999999998108, 28.9340974471568));
            manualData.Add(new XYData(-0.0699999999999363, 32.5167805045449));
            manualData.Add(new XYData(-0.0694999999998345, 36.4676549074442));
            manualData.Add(new XYData(-0.06899999999996, 40.8141570023444));
            manualData.Add(new XYData(-0.0684999999998581, 45.5844293533093));
            manualData.Add(new XYData(-0.0679999999999836, 50.8071590602943));
            manualData.Add(new XYData(-0.0674999999998818, 56.5113910715149));
            manualData.Add(new XYData(-0.0670000000000073, 62.7263160118654));
            manualData.Add(new XYData(-0.0664999999999054, 69.4810323826199));
            manualData.Add(new XYData(-0.0659999999998035, 76.8042833615195));
            manualData.Add(new XYData(-0.0654999999999291, 84.7241688441568));
            manualData.Add(new XYData(-0.0649999999998272, 93.2678338134647));
            manualData.Add(new XYData(-0.0644999999999527, 102.461134599101));
            manualData.Add(new XYData(-0.0639999999998508, 112.328285086427));
            manualData.Add(new XYData(-0.0634999999999764, 122.891485448283));
            manualData.Add(new XYData(-0.0629999999998745, 134.170536493478));
            manualData.Add(new XYData(-0.0625, 146.182443244409));
            manualData.Add(new XYData(-0.0619999999998981, 158.94101186222));
            manualData.Add(new XYData(-0.0615000000000236, 172.456444520447));
            manualData.Add(new XYData(-0.0609999999999218, 186.734937275537));
            manualData.Add(new XYData(-0.0604999999998199, 201.77828638328));
            manualData.Add(new XYData(-0.0599999999999454, 217.583508852044));
            manualData.Add(new XYData(-0.0594999999998436, 234.142483295171));
            manualData.Add(new XYData(-0.0589999999999691, 251.441617334803));
            manualData.Add(new XYData(-0.0584999999998672, 269.461547907499));
            manualData.Add(new XYData(-0.0579999999999927, 288.176880819004));
            manualData.Add(new XYData(-0.0574999999998909, 307.555975783828));
            manualData.Add(new XYData(-0.0570000000000164, 327.56078295882));
            manualData.Add(new XYData(-0.0564999999999145, 348.146736634664));
            manualData.Add(new XYData(-0.0559999999998126, 369.262711283543));
            manualData.Add(new XYData(-0.0554999999999382, 390.85104457569));
            manualData.Add(new XYData(-0.0549999999998363, 412.847631275539));
            manualData.Add(new XYData(-0.0544999999999618, 435.182091115713));
            manualData.Add(new XYData(-0.0539999999998599, 457.77801283273));
            manualData.Add(new XYData(-0.0534999999999854, 480.553275543637));
            manualData.Add(new XYData(-0.0529999999998836, 503.42044756157));
            manualData.Add(new XYData(-0.0525000000000091, 526.287261607052));
            manualData.Add(new XYData(-0.0519999999999072, 549.057164189064));
            manualData.Add(new XYData(-0.0514999999998054, 571.629935725973));
            manualData.Add(new XYData(-0.0509999999999309, 593.902376773143));
            manualData.Add(new XYData(-0.050499999999829, 615.769054544248));
            manualData.Add(new XYData(-0.0499999999999545, 637.123102780412));
            manualData.Add(new XYData(-0.0494999999998527, 657.85706695868));
            manualData.Add(new XYData(-0.0489999999999782, 677.863785861941));
            manualData.Add(new XYData(-0.0484999999998763, 697.037299677991));
            manualData.Add(new XYData(-0.0480000000000018, 715.273774076305));
            manualData.Add(new XYData(-0.0474999999999, 732.472429145275));
            manualData.Add(new XYData(-0.0470000000000255, 748.536461675641));
            manualData.Add(new XYData(-0.0464999999999236, 763.373949060071));
            manualData.Add(new XYData(-0.0459999999998217, 776.898723053241));
            manualData.Add(new XYData(-0.0454999999999472, 789.031201806625));
            manualData.Add(new XYData(-0.0449999999998454, 799.699168958763));
            manualData.Add(new XYData(-0.0444999999999709, 808.838489122393));
            manualData.Add(new XYData(-0.043999999999869, 816.393749857501));
            manualData.Add(new XYData(-0.0434999999999945, 822.318821143322));
            manualData.Add(new XYData(-0.0429999999998927, 826.577324447876));
            manualData.Add(new XYData(-0.0425000000000182, 829.143004722543));
            manualData.Add(new XYData(-0.0419999999999163, 830));
            manualData.Add(new XYData(-0.0414999999998145, 829.143004722531));
            manualData.Add(new XYData(-0.04099999999994, 826.577324447851));
            manualData.Add(new XYData(-0.0404999999998381, 822.318821143285));
            manualData.Add(new XYData(-0.0399999999999636, 816.393749857452));
            manualData.Add(new XYData(-0.0394999999998618, 808.838489122331));
            manualData.Add(new XYData(-0.0389999999999873, 799.69916895869));
            manualData.Add(new XYData(-0.0384999999998854, 789.031201806542));
            manualData.Add(new XYData(-0.0380000000000109, 776.898723053147));
            manualData.Add(new XYData(-0.0374999999999091, 763.373949059967));
            manualData.Add(new XYData(-0.0369999999998072, 748.536461675528));
            manualData.Add(new XYData(-0.0364999999999327, 732.472429145153));
            manualData.Add(new XYData(-0.0359999999998308, 715.273774076175));
            manualData.Add(new XYData(-0.0354999999999563, 697.037299677854));
            manualData.Add(new XYData(-0.0349999999998545, 677.863785861798));
            manualData.Add(new XYData(-0.03449999999998, 657.857066958531));
            manualData.Add(new XYData(-0.0339999999998781, 637.123102780258));
            manualData.Add(new XYData(-0.0335000000000036, 615.76905454409));
            manualData.Add(new XYData(-0.0329999999999018, 593.902376772981));
            manualData.Add(new XYData(-0.0325000000000273, 571.629935725809));
            manualData.Add(new XYData(-0.0319999999999254, 549.057164188898));
            manualData.Add(new XYData(-0.0314999999998236, 526.287261606885));
            manualData.Add(new XYData(-0.0309999999999491, 503.420447561403));
            manualData.Add(new XYData(-0.0304999999998472, 480.55327554347));
            manualData.Add(new XYData(-0.0299999999999727, 457.778012832564));
            manualData.Add(new XYData(-0.0294999999998709, 435.182091115548));
            manualData.Add(new XYData(-0.0289999999999964, 412.847631275377));
            manualData.Add(new XYData(-0.0284999999998945, 390.85104457553));
            manualData.Add(new XYData(-0.02800000000002, 369.262711283387));
            manualData.Add(new XYData(-0.0274999999999181, 348.146736634511));
            manualData.Add(new XYData(-0.0269999999998163, 327.560782958672));
            manualData.Add(new XYData(-0.0264999999999418, 307.555975783684));
            manualData.Add(new XYData(-0.0259999999998399, 288.176880818865));
            manualData.Add(new XYData(-0.0254999999999654, 269.461547907365));
            manualData.Add(new XYData(-0.0249999999998636, 251.441617334673));
            manualData.Add(new XYData(-0.0244999999999891, 234.142483295047));
            manualData.Add(new XYData(-0.0239999999998872, 217.583508851926));
            manualData.Add(new XYData(-0.0235000000000127, 201.778286383167));
            manualData.Add(new XYData(-0.0229999999999109, 186.73493727543));
            manualData.Add(new XYData(-0.022499999999809, 172.456444520346));
            manualData.Add(new XYData(-0.0219999999999345, 158.941011862124));
            manualData.Add(new XYData(-0.0214999999998327, 146.182443244318));
            manualData.Add(new XYData(-0.0209999999999582, 134.170536493393));
            manualData.Add(new XYData(-0.0204999999998563, 122.891485448203));
            manualData.Add(new XYData(-0.0199999999999818, 112.328285086353));
            manualData.Add(new XYData(-0.0194999999998799, 102.461134599032));
            manualData.Add(new XYData(-0.0190000000000055, 93.2678338133999));
            manualData.Add(new XYData(-0.0184999999999036, 84.7241688440966));
            manualData.Add(new XYData(-0.0179999999998017, 76.8042833614638));
            manualData.Add(new XYData(-0.0174999999999272, 69.4810323825685));
            manualData.Add(new XYData(-0.0169999999998254, 62.726316011818));
            manualData.Add(new XYData(-0.0164999999999509, 56.5113910714713));
            manualData.Add(new XYData(-0.015999999999849, 50.8071590602543));
            manualData.Add(new XYData(-0.0154999999999745, 45.5844293532728));
            manualData.Add(new XYData(-0.0149999999998727, 40.8141570023111));
            manualData.Add(new XYData(-0.0144999999999982, 36.4676549074139));
            manualData.Add(new XYData(-0.0139999999998963, 32.5167805045173));
            manualData.Add(new XYData(-0.0135000000000218, 28.9340974471319));
            manualData.Add(new XYData(-0.01299999999992, 25.6930130510216));
            manualData.Add(new XYData(-0.0124999999998181, 22.7678925188425));
            manualData.Add(new XYData(-0.0119999999999436, 20.1341511670458));
            manualData.Add(new XYData(-0.0114999999998417, 17.7683260410878));
            manualData.Add(new XYData(-0.0109999999999673, 15.6481284288653));
            manualData.Add(new XYData(-0.0104999999998654, 13.7524788686464));
            manualData.Add(new XYData(-0.00999999999999091, 12.0615262993856));
            manualData.Add(new XYData(-0.00949999999988904, 10.5566530213407));
            manualData.Add(new XYData(-0.00900000000001455, 9.22046712673673));
            manualData.Add(new XYData(-0.00849999999991269, 8.03678402737124));
            manualData.Add(new XYData(-0.00799999999981083, 6.99059865211528));
            manualData.Add(new XYData(-0.00749999999993634, 6.0680498157647));
            manualData.Add(new XYData(-0.00699999999983447, 5.25637817509613));
            manualData.Add(new XYData(-0.00649999999995998, 4.54387909154898));
            manualData.Add(new XYData(-0.00599999999985812, 3.9198516157607));
            manualData.Add(new XYData(-0.00549999999998363, 3.37454470004529));
            manualData.Add(new XYData(-0.00499999999988177, 2.8991016333642));
            manualData.Add(new XYData(-0.00450000000000728, 2.4855035816437));
            manualData.Add(new XYData(-0.00399999999990541, 2.12651300639036));
            manualData.Add(new XYData(-0.00349999999980355, 1.8156176280907));
            manualData.Add(new XYData(-0.00299999999992906, 1.54697549920552));
            manualData.Add(new XYData(-0.0024999999998272, 1.31536165575194));
            manualData.Add(new XYData(-0.00199999999995271, 1.11611672731313));
            manualData.Add(new XYData(-0.00149999999985084, 0.945097803389455));
            manualData.Add(new XYData(-0.000999999999976353, 0.798631779647498));
            manualData.Add(new XYData(-0.00049999999987449, 0.673471340983502));
            manualData.Add(new XYData(0, 0.566753679374106));
            manualData.Add(new XYData(0.000500000000101863, 0.475961993076444));
            manualData.Add(new XYData(0.000999999999976353, 0.39888976957963));
            manualData.Add(new XYData(0.00150000000007822, 0.33360781742526));
            manualData.Add(new XYData(0.00200000000018008, 0.278433981158876));
            manualData.Add(new XYData(0.00250000000005457, 0.231905448749565));
            manualData.Add(new XYData(0.00300000000015643, 0.192753541290897));
            manualData.Add(new XYData(0.00350000000003092, 0.159880860126842));
            manualData.Add(new XYData(0.00400000000013279, 0.132340656182453));
            manualData.Add(new XYData(0.00450000000000728, 0.109318279681886));
            manualData.Add(new XYData(0.00500000000010914, 0.0901145650856448));
            manualData.Add(new XYData(0.00549999999998363, 0.0741310054813268));
            manualData.Add(new XYData(0.00600000000008549, 0.0608565723565436));
            manualData.Add(new XYData(0.00650000000018736, 0.0498560402442566));
            manualData.Add(new XYData(0.00700000000006185, 0.040759680773478));
            manualData.Add(new XYData(0.00750000000016371, 0.0332541968361193));
            manualData.Add(new XYData(0.0080000000000382, 0.0270747745877039));
            manualData.Add(new XYData(0.00850000000014006, 0.0219981385689413));
            manualData.Add(new XYData(0.00900000000001455, 0.0178365031376315));
            manualData.Add(new XYData(0.00950000000011642, 0.0144323214424629));
            manualData.Add(new XYData(0.00999999999999091, 0.0116537411913389));
            manualData.Add(new XYData(0.0105000000000928, 0.00939068433653142));
            manualData.Add(new XYData(0.0110000000001946, 0.00755147541400664));
            manualData.Add(new XYData(0.0115000000000691, 0.00605995055573788));
            manualData.Add(new XYData(0.012000000000171, 0.0048529860839898));
            manualData.Add(new XYData(0.0125000000000455, 0.00387839205603811));
            manualData.Add(new XYData(0.0130000000001473, 0.00309312213282196));
            manualData.Add(new XYData(0.0135000000000218, 0.00246175668489598));
            manualData.Add(new XYData(0.0140000000001237, 0.00195522112375493));
            manualData.Add(new XYData(0.0144999999999982, 0.00154970606473362));
            manualData.Add(new XYData(0.0150000000001, 0.00122576010452609));
            manualData.Add(new XYData(0.0154999999999745, 0.000967529752248477));
            manualData.Add(new XYData(0.0160000000000764, 0.000762124411826269));
            manualData.Add(new XYData(0.0165000000001783, 0.000599087301637342));
            manualData.Add(new XYData(0.0170000000000528, 0.000469955842467046));
            manualData.Add(new XYData(0.0175000000001546, 0.000367897375130423));
            manualData.Add(new XYData(0.0180000000000291, 0.000287408112628304));
            manualData.Add(new XYData(0.018500000000131, 0.000224065015778846));
            manualData.Add(new XYData(0.0190000000000055, 0.000174321832159202));
            manualData.Add(new XYData(0.0195000000001073, 0.000135341880771411));
            manualData.Add(new XYData(0.0199999999999818, 0.000104861322392351));
            manualData.Add(new XYData(0.0205000000000837, 8.10776496505365E-05));
            manualData.Add(new XYData(0.0210000000001855, 6.25589812959882E-05));
            manualData.Add(new XYData(0.02150000000006, 4.81704699231274E-05));
            manualData.Add(new XYData(0.0220000000001619, 3.70147478601467E-05));
            manualData.Add(new XYData(0.0225000000000364, 2.83838566653344E-05));
            manualData.Add(new XYData(0.0230000000001382, 2.17205446952849E-05));
            manualData.Add(new XYData(0.0235000000000127, 1.65871860674677E-05));
            manualData.Add(new XYData(0.0240000000001146, 1.26408831880929E-05));
            manualData.Add(new XYData(0.0244999999999891, 9.61357275086597E-06));
            manualData.Add(new XYData(0.0250000000000909, 7.29616948164167E-06));
            manualData.Add(new XYData(0.0255000000001928, 5.52595962392672E-06));
            manualData.Add(new XYData(0.0260000000000673, 4.17660302304052E-06));
            manualData.Add(new XYData(0.0265000000001692, 3.15022364533401E-06));
            manualData.Add(new XYData(0.0270000000000437, 2.37116771307246E-06));
            manualData.Add(new XYData(0.0275000000001455, 1.78108996172921E-06));
            manualData.Add(new XYData(0.02800000000002, 1.33509489864854E-06));
            manualData.Add(new XYData(0.0285000000001219, 9.98713945934642E-07));
            manualData.Add(new XYData(0.0289999999999964, 7.4554315908958E-07));
            manualData.Add(new XYData(0.0295000000000982, 5.55401645576368E-07));
            manualData.Add(new XYData(0.0300000000002001, 4.12899381285309E-07));
            manualData.Add(new XYData(0.0305000000000746, 3.06326098356531E-07));
            manualData.Add(new XYData(0.0310000000001764, 2.26791338414149E-07));
            manualData.Add(new XYData(0.0315000000000509, 1.67560491658363E-07));
            manualData.Add(new XYData(0.0320000000001528, 1.23543381819598E-07));
            manualData.Add(new XYData(0.0325000000000273, 9.09012890783947E-08));
            manualData.Add(new XYData(0.0330000000001291, 6.67457006082528E-08));
            manualData.Add(new XYData(0.0335000000000036, 4.89079261165774E-08));
            manualData.Add(new XYData(0.0340000000001055, 3.57633255624383E-08));
            manualData.Add(new XYData(0.03449999999998, 2.60975201933511E-08));
            manualData.Add(new XYData(0.0350000000000819, 1.90047992188218E-08));
            manualData.Add(new XYData(0.0355000000001837, 1.38111558663878E-08));
            manualData.Add(new XYData(0.0360000000000582, 1.00161187464813E-08));
            manualData.Add(new XYData(0.0365000000001601, 7.24889151379812E-09));
            manualData.Add(new XYData(0.0370000000000346, 5.23535858394811E-09));
            manualData.Add(new XYData(0.0375000000001364, 3.77332283004077E-09));
            manualData.Add(new XYData(0.0380000000000109, 2.71396467639845E-09));
            manualData.Add(new XYData(0.0385000000001128, 1.94799176101216E-09));
            manualData.Add(new XYData(0.0389999999999873, 1.39531652587069E-09));
            manualData.Add(new XYData(0.0395000000000891, 9.97380923928261E-10));
            manualData.Add(new XYData(0.040000000000191, 7.11462599025372E-10));
            manualData.Add(new XYData(0.0405000000000655, 5.06460744240152E-10));
            manualData.Add(new XYData(0.0410000000001673, 3.59784293792022E-10));
            manualData.Add(new XYData(0.0415000000000418, 2.55059385789503E-10));
            manualData.Add(new XYData(0.0420000000001437, 1.80444278425371E-10));
            manualData.Add(new XYData(0.0425000000000182, 1.27393602683334E-10));
            manualData.Add(new XYData(0.0430000000001201, 8.97542083435179E-11));
            manualData.Add(new XYData(0.0434999999999945, 6.31051377525049E-11));
            manualData.Add(new XYData(0.0440000000000964, 4.42769110376228E-11));
            manualData.Add(new XYData(0.0445000000001983, 3.10022067354083E-11));
            manualData.Add(new XYData(0.0450000000000728, 2.16626008942574E-11));
            manualData.Add(new XYData(0.0455000000001746, 1.51053671717166E-11));
            manualData.Add(new XYData(0.0460000000000491, 1.05112574057228E-11));
            manualData.Add(new XYData(0.046500000000151, 7.29929227795449E-12));
            manualData.Add(new XYData(0.0470000000000255, 5.05835766276924E-12));
            manualData.Add(new XYData(0.0475000000001273, 3.49817080643816E-12));
            manualData.Add(new XYData(0.0480000000000018, 2.41421078489888E-12));
            manualData.Add(new XYData(0.0485000000001037, 1.66269295879574E-12));
            manualData.Add(new XYData(0.0489999999999782, 1.14275104450153E-12));
            manualData.Add(new XYData(0.04950000000008, 7.83779488040504E-13));
            manualData.Add(new XYData(0.0500000000001819, 5.36461868039152E-13));
            manualData.Add(new XYData(0.0505000000000564, 3.66426200083283E-13));
            manualData.Add(new XYData(0.0510000000001583, 2.49768046758422E-13));
            manualData.Add(new XYData(0.0515000000000327, 1.69898652192173E-13));
            manualData.Add(new XYData(0.0520000000001346, 1.15330901566493E-13));
            manualData.Add(new XYData(0.0525000000000091, 7.81275378154211E-14));
            manualData.Add(new XYData(0.053000000000111, 5.28159731079833E-14));
            manualData.Add(new XYData(0.0534999999999854, 3.56310918983093E-14));
            manualData.Add(new XYData(0.0540000000000873, 2.39880902500561E-14));
            manualData.Add(new XYData(0.0545000000001892, 1.6116284087986E-14));
            manualData.Add(new XYData(0.0550000000000637, 1.08053005612835E-14));
            manualData.Add(new XYData(0.0555000000001655, 7.22955367796411E-15));
            manualData.Add(new XYData(0.05600000000004, 4.8271280417199E-15));
            manualData.Add(new XYData(0.0565000000001419, 3.21639107529423E-15));
            manualData.Add(new XYData(0.0570000000000164, 2.13870840059227E-15));
            manualData.Add(new XYData(0.0575000000001182, 1.41917842290694E-15));
            manualData.Add(new XYData(0.0579999999999927, 9.39777669137079E-16));



            return manualData;
        }

        private static List<XYData> GetManualDataShort()
        {
            List<XYData> manualData = new List<XYData>();
            manualData.Add(new XYData(-0.0219999999999345, 9.3977766913991E-16));
            manualData.Add(new XYData(-0.02150000000006, 4.82712804173417E-15));
            manualData.Add(new XYData(-0.0209999999999582, 2.39880902501257E-14));
            manualData.Add(new XYData(-0.0205000000000837, 1.1533090156682E-13));
            manualData.Add(new XYData(-0.0199999999999818, 5.36461868040639E-13));
            manualData.Add(new XYData(-0.0195000000001073, 2.41421078490543E-12));
            manualData.Add(new XYData(-0.0190000000000055, 1.05112574057507E-11));
            manualData.Add(new XYData(-0.018500000000131, 4.42769110377376E-11));
            manualData.Add(new XYData(-0.0180000000000291, 1.80444278425828E-10));
            manualData.Add(new XYData(-0.0174999999999272, 7.11462599027132E-10));
            manualData.Add(new XYData(-0.0170000000000528, 2.713964676405E-09));
            manualData.Add(new XYData(-0.0164999999999509, 1.00161187465049E-08));
            manualData.Add(new XYData(-0.0160000000000764, 3.57633255625202E-08));
            manualData.Add(new XYData(-0.0154999999999745, 1.23543381819873E-07));
            manualData.Add(new XYData(-0.0150000000001, 4.12899381286205E-07));
            manualData.Add(new XYData(-0.0144999999999982, 1.33509489865136E-06));
            manualData.Add(new XYData(-0.0140000000001237, 4.17660302304908E-06));
            manualData.Add(new XYData(-0.0135000000000218, 1.26408831881181E-05));
            manualData.Add(new XYData(-0.01299999999992, 3.70147478602181E-05));
            manualData.Add(new XYData(-0.0125000000000455, 0.000104861322392548));
            manualData.Add(new XYData(-0.0119999999999436, 0.000287408112628826));
            manualData.Add(new XYData(-0.0115000000000691, 0.000762124411827605));
            manualData.Add(new XYData(-0.0109999999999673, 0.00195522112375824));
            manualData.Add(new XYData(-0.0105000000000928, 0.00485298608399772));
            manualData.Add(new XYData(-0.00999999999999091, 0.0116537411913572));
            manualData.Add(new XYData(-0.00950000000011642, 0.0270747745877448));
            manualData.Add(new XYData(-0.00900000000001455, 0.0608565723566319));
            manualData.Add(new XYData(-0.00849999999991269, 0.132340656182637));
            manualData.Add(new XYData(-0.0080000000000382, 0.278433981159247));
            manualData.Add(new XYData(-0.00749999999993634, 0.566753679374825));
            manualData.Add(new XYData(-0.00700000000006185, 1.11611672731448));
            manualData.Add(new XYData(-0.00649999999995998, 2.1265130063928));
            manualData.Add(new XYData(-0.00600000000008549, 3.91985161576496));
            manualData.Add(new XYData(-0.00549999999998363, 6.99059865212247));
            manualData.Add(new XYData(-0.00500000000010914, 12.0615262993972));
            manualData.Add(new XYData(-0.00450000000000728, 20.134151167064));
            manualData.Add(new XYData(-0.00400000000013279, 32.5167805045449));
            manualData.Add(new XYData(-0.00350000000003092, 50.8071590602943));
            manualData.Add(new XYData(-0.00299999999992906, 76.8042833615195));
            manualData.Add(new XYData(-0.00250000000005457, 112.328285086427));
            manualData.Add(new XYData(-0.00199999999995271, 158.94101186222));
            manualData.Add(new XYData(-0.00150000000007822, 217.583508852044));
            manualData.Add(new XYData(-0.000999999999976353, 288.176880819004));
            manualData.Add(new XYData(-0.000500000000101863, 369.262711283543));
            manualData.Add(new XYData(0, 457.77801283273));
            manualData.Add(new XYData(0.00049999999987449, 549.057164189064));
            manualData.Add(new XYData(0.000999999999976353, 637.123102780412));
            manualData.Add(new XYData(0.00150000000007822, 715.273774076305));
            manualData.Add(new XYData(0.00199999999995271, 776.898723053241));
            manualData.Add(new XYData(0.00250000000005457, 816.393749857501));
            manualData.Add(new XYData(0.00299999999992906, 830));
            manualData.Add(new XYData(0.00350000000003092, 816.393749857452));
            manualData.Add(new XYData(0.00399999999990541, 776.898723053147));
            manualData.Add(new XYData(0.00450000000000728, 715.273774076175));
            manualData.Add(new XYData(0.00499999999988177, 637.123102780258));
            manualData.Add(new XYData(0.00549999999998363, 549.057164188898));
            manualData.Add(new XYData(0.00600000000008549, 457.778012832564));
            manualData.Add(new XYData(0.00649999999995998, 369.262711283387));
            manualData.Add(new XYData(0.00700000000006185, 288.176880818865));
            manualData.Add(new XYData(0.00749999999993634, 217.583508851926));
            manualData.Add(new XYData(0.0080000000000382, 158.941011862124));
            manualData.Add(new XYData(0.00849999999991269, 112.328285086353));
            manualData.Add(new XYData(0.00900000000001455, 76.8042833614638));
            manualData.Add(new XYData(0.00949999999988904, 50.8071590602543));
            manualData.Add(new XYData(0.00999999999999091, 32.5167805045173));
            manualData.Add(new XYData(0.0104999999998654, 20.1341511670458));
            manualData.Add(new XYData(0.0109999999999673, 12.0615262993856));
            manualData.Add(new XYData(0.0115000000000691, 6.99059865211528));
            manualData.Add(new XYData(0.0119999999999436, 3.9198516157607));
            manualData.Add(new XYData(0.0125000000000455, 2.12651300639036));
            manualData.Add(new XYData(0.01299999999992, 1.11611672731313));
            manualData.Add(new XYData(0.0135000000000218, 0.566753679374106));
            manualData.Add(new XYData(0.0139999999998963, 0.278433981158876));
            manualData.Add(new XYData(0.0144999999999982, 0.132340656182453));
            manualData.Add(new XYData(0.0149999999998727, 0.0608565723565436));
            manualData.Add(new XYData(0.0154999999999745, 0.0270747745877039));
            manualData.Add(new XYData(0.0160000000000764, 0.0116537411913389));
            manualData.Add(new XYData(0.0164999999999509, 0.0048529860839898));
            manualData.Add(new XYData(0.0170000000000528, 0.00195522112375493));
            manualData.Add(new XYData(0.0174999999999272, 0.000762124411826269));
            manualData.Add(new XYData(0.0180000000000291, 0.000287408112628304));
            manualData.Add(new XYData(0.0184999999999036, 0.000104861322392351));
            manualData.Add(new XYData(0.0190000000000055, 3.70147478601467E-05));
            manualData.Add(new XYData(0.0194999999998799, 1.26408831880929E-05));
            manualData.Add(new XYData(0.0199999999999818, 4.17660302304052E-06));
            manualData.Add(new XYData(0.0205000000000837, 1.33509489864854E-06));
            manualData.Add(new XYData(0.0209999999999582, 4.12899381285309E-07));
            manualData.Add(new XYData(0.02150000000006, 1.23543381819598E-07));
            manualData.Add(new XYData(0.0219999999999345, 3.57633255624383E-08));
            manualData.Add(new XYData(0.0225000000000364, 1.00161187464813E-08));
            manualData.Add(new XYData(0.0229999999999109, 2.71396467639845E-09));
            manualData.Add(new XYData(0.0235000000000127, 7.11462599025372E-10));
            manualData.Add(new XYData(0.0239999999998872, 1.80444278425371E-10));
            manualData.Add(new XYData(0.0244999999999891, 4.42769110376228E-11));
            manualData.Add(new XYData(0.0249999999998636, 1.05112574057228E-11));
            manualData.Add(new XYData(0.0254999999999654, 2.41421078489888E-12));
            manualData.Add(new XYData(0.0260000000000673, 5.36461868039152E-13));
            manualData.Add(new XYData(0.0264999999999418, 1.15330901566493E-13));
            manualData.Add(new XYData(0.0270000000000437, 2.39880902500561E-14));
            manualData.Add(new XYData(0.0274999999999181, 4.8271280417199E-15));
            manualData.Add(new XYData(0.02800000000002, 9.39777669137079E-16));





            return manualData;
        }

        private static List<XYData> GetManualDataShorter1Peak()
        {
            List<XYData> manualData = new List<XYData>();
            manualData.Add(new XYData(-0.0219999999999345, 9.3977766913991E-16));
            manualData.Add(new XYData(-0.02150000000006, 2.39880902501257E-14));
            manualData.Add(new XYData(-0.0209999999999582, 5.36461868040639E-13));
            manualData.Add(new XYData(-0.0205000000000837, 1.05112574057507E-11));
            manualData.Add(new XYData(-0.0199999999999818, 1.80444278425828E-10));
            manualData.Add(new XYData(-0.0195000000001073, 2.713964676405E-09));
            manualData.Add(new XYData(-0.0190000000000055, 3.57633255625202E-08));
            manualData.Add(new XYData(-0.018500000000131, 4.12899381286205E-07));
            manualData.Add(new XYData(-0.0180000000000291, 4.17660302304908E-06));
            manualData.Add(new XYData(-0.0174999999999272, 3.70147478602181E-05));
            manualData.Add(new XYData(-0.0170000000000528, 0.000287408112628826));
            manualData.Add(new XYData(-0.0164999999999509, 0.00195522112375824));
            manualData.Add(new XYData(-0.0160000000000764, 0.0116537411913572));
            manualData.Add(new XYData(-0.0154999999999745, 0.0608565723566319));
            manualData.Add(new XYData(-0.0150000000001, 0.278433981159247));
            manualData.Add(new XYData(-0.0144999999999982, 1.11611672731448));
            manualData.Add(new XYData(-0.0140000000001237, 3.91985161576496));
            manualData.Add(new XYData(-0.0135000000000218, 12.0615262993972));
            manualData.Add(new XYData(-0.01299999999992, 32.5167805045449));
            manualData.Add(new XYData(-0.0125000000000455, 76.8042833615195));
            manualData.Add(new XYData(-0.0119999999999436, 158.94101186222));
            manualData.Add(new XYData(-0.0115000000000691, 288.176880819004));
            manualData.Add(new XYData(-0.0109999999999673, 457.77801283273));
            manualData.Add(new XYData(-0.0105000000000928, 637.123102780412));
            manualData.Add(new XYData(-0.00999999999999091, 776.898723053241));
            manualData.Add(new XYData(-0.00950000000011642, 830));
            manualData.Add(new XYData(-0.00900000000001455, 776.898723053147));
            manualData.Add(new XYData(-0.00849999999991269, 637.123102780258));
            manualData.Add(new XYData(-0.0080000000000382, 457.778012832564));
            manualData.Add(new XYData(-0.00749999999993634, 288.176880818865));
            manualData.Add(new XYData(-0.00700000000006185, 158.941011862124));
            manualData.Add(new XYData(-0.00649999999995998, 76.8042833614638));
            manualData.Add(new XYData(-0.00600000000008549, 32.5167805045173));
            manualData.Add(new XYData(-0.00549999999998363, 12.0615262993856));
            manualData.Add(new XYData(-0.00500000000010914, 3.9198516157607));
            manualData.Add(new XYData(-0.00450000000000728, 1.11611672731313));
            manualData.Add(new XYData(-0.00400000000013279, 0.278433981158876));
            manualData.Add(new XYData(-0.00350000000003092, 0.0608565723565436));
            manualData.Add(new XYData(-0.00299999999992906, 0.0116537411913389));
            manualData.Add(new XYData(-0.00250000000005457, 0.00195522112375493));
            manualData.Add(new XYData(-0.00199999999995271, 0.000287408112628304));
            manualData.Add(new XYData(-0.00150000000007822, 3.70147478601467E-05));
            manualData.Add(new XYData(-0.000999999999976353, 4.17660302304052E-06));
            manualData.Add(new XYData(-0.000500000000101863, 4.12899381285309E-07));
            manualData.Add(new XYData(0, 3.57633255624383E-08));
            manualData.Add(new XYData(0.00049999999987449, 2.71396467639845E-09));
            manualData.Add(new XYData(0.000999999999976353, 1.80444278425371E-10));
            manualData.Add(new XYData(0.00150000000007822, 1.05112574057228E-11));
            manualData.Add(new XYData(0.00199999999995271, 5.36461868039152E-13));
            manualData.Add(new XYData(0.00250000000005457, 2.39880902500561E-14));
            manualData.Add(new XYData(0.00299999999992906, 9.39777669137079E-16));






            return manualData;
        }

        private static List<XYData> GetManualDataShorter2Peak()
        {
            List<XYData> manualData = new List<XYData>();
            manualData.Add(new XYData(0.00499999999988177, 9.3977766913991E-16));
            manualData.Add(new XYData(0.00549999999998363, 2.39880902501257E-14));
            manualData.Add(new XYData(0.00600000000008549, 5.36461868040639E-13));
            manualData.Add(new XYData(0.00649999999995998, 1.05112574057507E-11));
            manualData.Add(new XYData(0.00700000000006185, 1.80444278425828E-10));
            manualData.Add(new XYData(0.00749999999993634, 2.713964676405E-09));
            manualData.Add(new XYData(0.0080000000000382, 3.57633255625202E-08));
            manualData.Add(new XYData(0.00849999999991269, 4.12899381286205E-07));
            manualData.Add(new XYData(0.00900000000001455, 4.17660302304908E-06));
            manualData.Add(new XYData(0.00949999999988904, 3.70147478611579E-05));
            manualData.Add(new XYData(0.00999999999999091, 0.000287408112652814));
            manualData.Add(new XYData(0.0104999999998654, 0.0019552211242947));
            manualData.Add(new XYData(0.0109999999999673, 0.0116537412018684));
            manualData.Add(new XYData(0.0115000000000691, 0.0608565725370762));
            manualData.Add(new XYData(0.0119999999999436, 0.278433983873211));
            manualData.Add(new XYData(0.0125000000000455, 1.11611676307781));
            manualData.Add(new XYData(0.01299999999992, 3.91985202866434));
            manualData.Add(new XYData(0.0135000000000218, 12.0615304760002));
            manualData.Add(new XYData(0.0139999999998963, 32.5168175192927));
            manualData.Add(new XYData(0.0144999999999982, 76.8045707696321));
            manualData.Add(new XYData(0.0149999999998727, 158.942967083344));
            manualData.Add(new XYData(0.0154999999999745, 288.188534560195));
            manualData.Add(new XYData(0.0160000000000764, 457.838869405087));
            manualData.Add(new XYData(0.0164999999999509, 637.401536761571));
            manualData.Add(new XYData(0.0170000000000528, 778.014839780556));
            manualData.Add(new XYData(0.0174999999999272, 833.919851615765));
            manualData.Add(new XYData(0.0180000000000291, 788.960249352545));
            manualData.Add(new XYData(0.0184999999999036, 669.639883284803));
            manualData.Add(new XYData(0.0190000000000055, 534.582296194084));
            manualData.Add(new XYData(0.0194999999998799, 447.117892681085));
            manualData.Add(new XYData(0.0199999999999818, 447.117892681128));
            manualData.Add(new XYData(0.0205000000000837, 534.582296194194));
            manualData.Add(new XYData(0.0209999999999582, 669.639883284929));
            manualData.Add(new XYData(0.02150000000006, 788.960249352627));
            manualData.Add(new XYData(0.0219999999999345, 833.919851615761));
            manualData.Add(new XYData(0.0225000000000364, 778.014839780461));
            manualData.Add(new XYData(0.0229999999999109, 637.401536761417));
            manualData.Add(new XYData(0.0235000000000127, 457.838869404921));
            manualData.Add(new XYData(0.0239999999998872, 288.188534560056));
            manualData.Add(new XYData(0.0244999999999891, 158.942967083248));
            manualData.Add(new XYData(0.0249999999998636, 76.8045707695764));
            manualData.Add(new XYData(0.0254999999999654, 32.5168175192652));
            manualData.Add(new XYData(0.0260000000000673, 12.0615304759886));
            manualData.Add(new XYData(0.0264999999999418, 3.91985202866008));
            manualData.Add(new XYData(0.0270000000000437, 1.11611676307646));
            manualData.Add(new XYData(0.0274999999999181, 0.278433983872841));
            manualData.Add(new XYData(0.02800000000002, 0.0608565725369878));
            manualData.Add(new XYData(0.0284999999998945, 0.0116537412018501));
            manualData.Add(new XYData(0.0289999999999964, 0.00195522112429139));
            manualData.Add(new XYData(0.0294999999998709, 0.000287408112652293));
            manualData.Add(new XYData(0.0299999999999727, 3.70147478610865E-05));
            manualData.Add(new XYData(0.0305000000000746, 4.17660302304052E-06));
            manualData.Add(new XYData(0.0309999999999491, 4.12899381285309E-07));
            manualData.Add(new XYData(0.0315000000000509, 3.57633255624383E-08));
            manualData.Add(new XYData(0.0319999999999254, 2.71396467639845E-09));
            manualData.Add(new XYData(0.0325000000000273, 1.80444278425371E-10));
            manualData.Add(new XYData(0.0329999999999018, 1.05112574057228E-11));
            manualData.Add(new XYData(0.0335000000000036, 5.36461868039152E-13));
            manualData.Add(new XYData(0.0339999999998781, 2.39880902500561E-14));
            manualData.Add(new XYData(0.03449999999998, 9.39777669137079E-16));







            return manualData;
        }
    }
}
