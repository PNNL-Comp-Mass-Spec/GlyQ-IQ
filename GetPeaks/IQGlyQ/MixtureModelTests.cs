using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.MachineLearning;
using Accord.Statistics.Distributions.Multivariate;
using NUnit.Framework;

namespace AccordTest
{
	public class MixtureModelTests
	{
		/// <summary>
		/// Test to show how to create a 2-D Mixture Model and solve for the 2 gaussian distributions that are contained in the mixture model.
		/// Note that I am not asserting anything and just printing some results to the console.
		/// </summary>
		[Test]
		public void TestCreate2DMixtureModelFromPDF()
		{
			double[] xValues = { -2, -1.9, -1.8, -1.7, -1.6, -1.5, -1.4, -1.3, -1.2, -1.1, -1, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 2.8, 2.9, 3, 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9 };
			int[] yValues = { 0, 0, 0, 0, 0, 1, 2, 5, 11, 23, 44, 79, 135, 216, 325, 458, 608, 759, 892, 988, 1032, 1023, 968, 884, 793, 716, 667, 651, 667, 704, 748, 786, 805, 799, 765, 705, 625, 532, 436, 343, 259, 188, 131, 88, 57, 35, 21, 12, 7, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0, 0 };

			List<double> sampleList = new List<double>();

			for (int i = 0; i < xValues.Length; i++)
			{
				int numObservations = yValues[i];
				double valueToObserve = xValues[i];

				for (int j = 0; j < numObservations; j++)
				{
					sampleList.Add(valueToObserve);
				}
			}

			double[][] sampleArray = new double[sampleList.Count][];
			for (int i = 0; i < sampleList.Count; i++)
			{
				sampleArray[i] = new double[] { sampleList[i] };
			}

			GaussianMixtureModel gmm = new GaussianMixtureModel(2);
			gmm.Compute(sampleArray, 0.000001);

			MultivariateMixture<MultivariateNormalDistribution> mixture = gmm.ToMixtureDistribution();
			MultivariateNormalDistribution[] mixtureComponents = mixture.Components;

			MultivariateNormalDistribution normalDistribution1 = mixtureComponents[0];
			MultivariateNormalDistribution normalDistribution2 = mixtureComponents[1];

			for (double j = -2; j < 4; j += 0.1)
			{
				Console.WriteLine(j + "\t" + normalDistribution1.ProbabilityDensityFunction(new double[] { j }) + "\t" + normalDistribution2.ProbabilityDensityFunction(new double[] { j }));
			}
		}

		/// <summary>
		/// Test to show how to create a 3-D Mixture Model and solve for the 2 3-D gaussian distributions that are contained in the mixture model.
		/// Note that I am not asserting anything and just printing some results to the console.
		/// </summary>
		[Test]
		public void TestCreate3DMixtureModelFromPDF()
		{
			int[][] rawValues = new int[13][];
			rawValues[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			rawValues[1] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			rawValues[2] = new int[] { 0, 0, 0, 0, 1, 2, 1, 0, 0, 0, 0, 0, 0 };
			rawValues[3] = new int[] { 0, 0, 0, 3, 13, 22, 14, 5, 1, 0, 0, 0, 0 };
			rawValues[4] = new int[] { 0, 0, 1, 13, 59, 99, 72, 35, 14, 3, 0, 0, 0 };
			rawValues[5] = new int[] { 0, 0, 2, 22, 98, 172, 155, 118, 60, 13, 1, 0, 0 };
			rawValues[6] = new int[] { 0, 0, 1, 13, 60, 118, 155, 172, 98, 22, 2, 0, 0 };
			rawValues[7] = new int[] { 0, 0, 0, 3, 14, 35, 72, 99, 59, 13, 1, 0, 0 };
			rawValues[8] = new int[] { 0, 0, 0, 0, 1, 5, 14, 22, 13, 3, 0, 0, 0 };
			rawValues[9] = new int[] { 0, 0, 0, 0, 0, 0, 1, 2, 1, 0, 0, 0, 0 };
			rawValues[10] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			rawValues[11] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			rawValues[12] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

			List<double[]> listOfSamples = new List<double[]>();

			for (int i = 0; i < rawValues.Length; i++)
			{
				int[] currentRawValues = rawValues[i];
				for (int j = 0; j < currentRawValues.Length; j++)
				{
					int numObservations = currentRawValues[j];
					for (int k = 0; k < numObservations; k++)
					{
						listOfSamples.Add(new double[] { i, j });
					}
				}
			}

			GaussianMixtureModel gmm = new GaussianMixtureModel(2);
			gmm.Compute(listOfSamples.ToArray(), 0.000001);

			MultivariateMixture<MultivariateNormalDistribution> mixture = gmm.ToMixtureDistribution();
			MultivariateNormalDistribution[] mixtureComponents = mixture.Components;

			MultivariateNormalDistribution normalDistribution1 = mixtureComponents[0];
			MultivariateNormalDistribution normalDistribution2 = mixtureComponents[1];

			for (int i = 0; i < 13; i++)
			{
				for (int j = 0; j < 13; j++)
				{
					Console.Write(normalDistribution1.ProbabilityDensityFunction(new double[] { i, j }) + "\t");
				}
				Console.Write("\n");
			}

			Console.Write("\n");
			Console.Write("\n");
			Console.Write("\n");

			for (int i = 0; i < 13; i++)
			{
				for (int j = 0; j < 13; j++)
				{
					Console.Write(normalDistribution2.ProbabilityDensityFunction(new double[] { i, j }) + "\t");
				}
				Console.Write("\n");
			}
		}
	}
}
