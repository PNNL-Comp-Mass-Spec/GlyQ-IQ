using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ
{
    //class LBalg1
    //{
    //}



    /** LMdemo Levenberg Marquardt demonstrator
      *
      * Demonstrates fitting a 2D circular Gaussian to noisy data. 
      * Five parameters: Xcenter, Ycenter, TotalSignal, Sigma, Background.
      *
      * This one file contains several classes:
      *     LMdemo        contains only main(); creates a FitHost.
      *     FitHost       defines the fitting problem.
      *     LMhost        defines interface methods required by LM. 
      *     LM            The Levenberg-Marquardt routine.
      *
      *  M.Lampton UCB SSL (c) 1996; Java edition 2007, 2011
      *  see M.Lampton, 1997 Computers In Physics v.11 #10 110-115.
      */

    public class LMdemo
    {
        public static void main(String[] args)
        {
            new FitHost();
        }
    }


    //http://www.ssl.berkeley.edu/~mlampton/LMdemo.java
    //http://code.google.com/p/efficient-java-matrix-library/w/list

    /**  FitHost 
      * Fits a five parameter Gaussian to pixellized noisy data
      * No subpixel integrations, for simplicity
      *
      * Triggers LM by instantiating an object of class LM
      * Supplies four callback methods mandated by interface LMhost
      */

    internal class FitHost : LMhost
    {
        //--------constants for LM---------------
        protected double DELTAP = 1e-6; // parm step
        protected double BIGVAL = 9e99; // fault flag

        ////---constants for fitting: pixels & parameters-----
        protected int NX = 50, NY = 50; // Npixels, x and y
        protected int NPTS { get; set; } // total data points


        protected int NPARMS { get; set; } // xc,   yc,  signal,  sigma, bkg/pix. 
        private double[] truep = {26.2, 24.7, 50000.0, 2.0, 1000.0};
        private double[] parms { get; set; } // = new double[NPARMS]; 
        private double[] data { get; set; } // = new double[NPTS]; 
        private double[] model { get; set; } // = new double[NPTS]; 
        private double[] rtwt { get; set; } // = new double[NPTS]; 
        private double[] resid { get; set; } // = new double[NPTS]; 
        private double[,] jac { get; set; } // = new double[NPTS,NPARMS];  

        private Random rnd1 = new Random();


        //public int[] MyNumbers {get;set;}


        public FitHost()
        {
            NPTS = NX*NY; // total data points
            NPARMS = 5;
            truep = new double[5] {26.2, 24.7, 50000.0, 2.0, 1000.0};
            parms = new double[NPTS];
            data = new double[NPTS];
            model = new double[NPTS];
            rtwt = new double[NPTS];
            resid = new double[NPTS];
            jac = new double[NPTS,NPARMS];



            listParms("True Parms", truep);
            putNoisyData(); // create the noisy data

            
            putRootWeights(); // put the pixel weights

            foreach (var d in data)
            {
                Console.WriteLine(d);
            }
            putStartParms(); // put starting point
            listParms("Start Parms", parms);
            //LM myLM = new LM(this, NPARMS, NPTS);
            LM myLM = new LM(this, NPARMS, NPTS);
            listParms("Fitted Parms", parms);
        }

        //------------mathematical helpers for FitHost--------------

        private int getIndex(int ix, int iy) // convert 2D to 1D index
        {
            return ix + iy*NX;
        }

        private int getIX(int index) // convert 1D index to 2D,X
        {
            return index%NX;
        }

        private int getIY(int index) // convert 1D index to 2D,Y
        {
            return index/NX;
        }

        private void listParms(String title, double[] p)
        {
            Console.WriteLine(title + "----");
            Console.WriteLine("   X=" + p[0]);
            Console.WriteLine("   Y=" + p[1]);
            Console.WriteLine("   M=" + p[2]);
            Console.WriteLine("   S=" + p[3]);
            Console.WriteLine("   B=" + p[4]);
            Console.WriteLine();
        }

        private void putStartParms()
            // Initializes parms[] to a reasonable starting location
        {
            double biggest = 0.0;
            int ibiggest = 0;
            for (int i = 0; i < NPTS; i++)
                if (data[i] > biggest)
                {
                    ibiggest = i;
                    biggest = data[i];
                }
            parms[0] = getIX(ibiggest);
            parms[1] = getIY(ibiggest);
            parms[2] = 100000;
            parms[3] = 1;
            parms[4] = 100;
        }

        private void putNoiselessModel(double[] p, double[] result)
            // This generates the noiseless fitting function for any p[].
        {
            for (int i = 0; i < NPTS; i++)
            {
                double x = getIX(i) - p[0];
                double y = getIY(i) - p[1];
                double r = Math.Sqrt(x*x + y*y);
                result[i] = p[4] + p[2]*gauss2D(r, p[3]);
            }
        }

        private void putNoisyData()
            // This produces the data array to be fitted.
            // For actual data it would read in a file of numbers.
            // For this demo I use the noiseless model at truep[], and add noise. 
        {
            putNoiselessModel(truep, data); // true noiseless array
            for (int i = 0; i < NPTS; i++) // add noise
                data[i] += gaussrand()*Math.Sqrt(data[i]);
        }

        private void putRootWeights()
            // Here I evaluate Poisson rms variations for the pixels. 
        {
            for (int i = 0; i < NPTS; i++)
            {
                if (data[i] <= 0.0)
                    rtwt[i] = 0.0;
                else
                    rtwt[i] = 1/Math.Sqrt(data[i]);
            }
        }

        private double gauss2D(double r, double sigma)
            // Returns Gaussian 2D density normalized to unit total mass. 
        {
            if (sigma <= 0.0)
                return 0.0;
            double arg = 0.5*r*r/(sigma*sigma);
            if (arg > 15)
                return 0.0;
            return Math.Exp(-arg)/(2*Math.PI*sigma*sigma);
        }

        private double gaussrand()
            // Returns a 1D gaussian variate, unit variance, zero mean
        {
            double sum = -6.0;
            for (int i = 0; i < 12; i++)
                //sum += Math.random(); 
                sum += rnd1.Next();
            return sum;
        }

        private double dComputeResiduals()
            // Uses current parms[] vector; 
            // Evaluates resid[i] = (model[i] - data[i])*rootWeight[i]. 
            // Returns sum-of-squares. 
        {
            putNoiselessModel(parms, model);

            double sumsq = 0.0;
            for (int i = 0; i < NPTS; i++)
            {
                resid[i] = (model[i] - data[i])*rtwt[i];
                sumsq += resid[i]*resid[i];
            }
            return sumsq;
        }

        //------the four mandated interface methods------------

        public double dNudge(double[] dp)
            // Allows LM to modify parms[] and reevaluate.
            // Returns sum-of-squares for nudged params.
            // This is the only place that parms[] are modified.
            // If NADJ<NPARMS, this is the place for your LUT.
        {
            for (int j = 0; j < NPARMS; j++)
                parms[j] += dp[j];
            return dComputeResiduals();
        }

        public bool bBuildJacobian()
            // Allows LM to compute a new Jacobian.
            // Uses current parms[] and two-sided finite difference.
            // If current parms[] is bad, returns false.  
        {
            double[] delta = new double[NPARMS];
            double FACTOR = 0.5/DELTAP;
            double d = 0;

            for (int j = 0; j < NPARMS; j++)
            {
                for (int k = 0; k < NPARMS; k++)
                    delta[k] = (k == j) ? DELTAP : 0.0;

                d = dNudge(delta); // resid at pplus
                if (d == BIGVAL)
                {
                    Console.WriteLine("Bad dBuildJacobian() exit 2");
                    return false;
                }
                for (int i = 0; i < NPTS; i++)
                    jac[i, j] = dGetResid(i);

                for (int k = 0; k < NPARMS; k++)
                    delta[k] = (k == j) ? -2*DELTAP : 0.0;

                d = dNudge(delta); // resid at pminus
                if (d == BIGVAL)
                {
                    Console.WriteLine("Bad dBuildJacobian() exit 3");
                    return false;
                }

                for (int i = 0; i < NPTS; i++)
                    jac[i, j] -= dGetResid(i); // fetches resid[]

                for (int i = 0; i < NPTS; i++)
                    jac[i, j] *= FACTOR;

                for (int k = 0; k < NPARMS; k++)
                    delta[k] = (k == j) ? DELTAP : 0.0;

                d = dNudge(delta);
                if (d == BIGVAL)
                {
                    Console.WriteLine("Bad dBuildJacobian() exit 4");
                    return false;
                }
            }
            return true;
        }

        public double dGetResid(int i)
            // Allows LM to see one element of the resid[] vector. 
        {
            return resid[i];
        }

        public double dGetJac(int i, int j)
            // Allows LM to see one element of the Jacobian matrix. 
        {
            return jac[i, j];
        }
    }





    /** LMhost interface class declares four abstract methods
      * These allow LM to request numerical work
      * M.Lampton UCB SSL 2005
      */

    public interface LMhost
    {
        double dNudge(double[] dp);
        // Allows myLM.bLMiter to modify parms[] and reevaluate.
        // This is the only modifier of parms[].
        // So, if NADJ<NPARMS, put your LUT here. 

        bool bBuildJacobian();
        // Allows LM to request a new Jacobian.

        double dGetResid(int i);
        // Allows LM to see one element of the resid[] vector. 

        double dGetJac(int i, int j);
        // Allows LM to see one element of the Jacobian matrix. 
    }





    /**
      *  class LM   Levenberg Marquardt w/ Lampton improvements
      *  M.Lampton, 1997 Computers In Physics v.11 #10 110-115.
      *
      *  Constructor is used to set up all parms including host for callback.
      *  bLMiter() performs one iteration.
      *  Host arrays parms[], resid[], jac[,] are unknown here.
      *  Callback method uses CallerID to access four host methods:
      *
      *    double dNudge(dp);         Moves parms, builds resid[], returns sos.
      *    bool bBuildJacobian();  Builds Jacobian, returns false if parms NG.
      *    double dGetJac(i,j);       Fetches one value of host Jacobian.
      *    double dGetResid(i);       Fetches one value of host residual.
      *
      *  Exit leaves host with optimized parms[]. 
      *
      *  @author: M.Lampton UCB SSL (c) 2005
      */

    public class LM
    {
        private int LMITER = 100; // max number of L-M iterations
        private double LMBOOST = 2.0; // damping increase per failed step
        private double LMSHRINK = 0.10; // damping decrease per successful step
        private double LAMBDAZERO = 0.001; // initial damping
        private double LAMBDAMAX = 1E9; // max damping
        private double LMTOL = 1E-12; // exit tolerance
        private double BIGVAL = 9e99; // trouble flag 

        private double sos, sosprev, lambda;

        private LMhost myH = null; // overwritten by constructor
        private int nadj = 0; // overwritten by constructor
        private int npts = 0; // overwritten by constructor

        private double[] delta; // local parm change
        private double[] beta; // local
        private double[,] alpha; // local
        private double[,] amatrix; // local 

        public LM(LMhost gH, int gnadj, int gnpts)
            // Constructor sets up fields and drives iterations. 
        {
            myH = gH;
            nadj = gnadj;
            npts = gnpts;
            delta = new double[nadj];
            beta = new double[nadj];
            alpha = new double[nadj,nadj];
            amatrix = new double[nadj,nadj];
            lambda = LAMBDAZERO;
            int niter = 0;
            bool done = false;
            do
            {
                done = bLMiter();
                niter++;
            } while (!done && (niter < LMITER));
        }

        private bool bLMiter()
            // Each call performs one LM iteration. 
            // Returns true if done with iterations; false=wants more. 
            // Global nadj, npts; needs nadj, myH to be preset. 
            // Ref: M.Lampton, Computers in Physics v.11 pp.110-115 1997.
        {
            for (int k = 0; k < nadj; k++)
                delta[k] = 0.0;
            sos = myH.dNudge(delta);
            if (sos == BIGVAL)
            {
                Console.WriteLine("bLMiter finds faulty initial dNudge()");
                return false;
            }
            sosprev = sos;

            Console.WriteLine("bLMiter..SumOfSquares= " + sos);
            if (!myH.bBuildJacobian())
            {
                Console.WriteLine("bLMiter finds bBuildJacobian()=false");
                return false;
            }
            for (int k = 0; k < nadj; k++) // get downhill gradient beta
            {
                beta[k] = 0.0;
                for (int i = 0; i < npts; i++)
                    beta[k] -= myH.dGetResid(i)*myH.dGetJac(i, k);
            }
            for (int k = 0; k < nadj; k++) // get curvature matrix alpha
                for (int j = 0; j < nadj; j++)
                {
                    alpha[j, k] = 0.0;
                    for (int i = 0; i < npts; i++)
                        alpha[j, k] += myH.dGetJac(i, j)*myH.dGetJac(i, k);
                }
            double rrise = 0;
            do /// damping loop searches for one downhill step
            {
                for (int k = 0; k < nadj; k++) // copy and damp it
                    for (int j = 0; j < nadj; j++)
                        amatrix[j, k] = alpha[j, k] + ((j == k) ? lambda : 0.0);

                gaussj(amatrix, nadj); // invert

                for (int k = 0; k < nadj; k++) // compute delta[]
                {
                    delta[k] = 0.0;
                    for (int j = 0; j < nadj; j++)
                        delta[k] += amatrix[j, k]*beta[j];
                }
                sos = myH.dNudge(delta); // try it out.
                if (sos == BIGVAL)
                {
                    Console.WriteLine("LMinner failed SOS step");
                    return false;
                }
                rrise = (sos - sosprev)/(1 + sos);
                if (rrise <= 0.0) // good step!
                {
                    lambda *= LMSHRINK; // shrink lambda
                    break; // leave lmInner.
                }
                for (int q = 0; q < nadj; q++) // reverse course!
                    delta[q] *= -1.0;
                myH.dNudge(delta); // sosprev should still be OK
                if (rrise < LMTOL) // finished but keep prev parms
                    break; // leave inner loop
                lambda *= LMBOOST; // else try more damping.
            } while (lambda < LAMBDAMAX);
            bool done = (rrise > -LMTOL) || (lambda > LAMBDAMAX);
            return done;
        }

        private double gaussj(double[,] a, int N)
            // Inverts the double array a[N,N] by Gauss-Jordan method
            // M.Lampton UCB SSL (c)2003, 2005
        {
            double det = 1.0, big, save;
            int i, j, k, L;
            int[] ik = new int[100];
            int[] jk = new int[100];
            for (k = 0; k < N; k++)
            {
                big = 0.0;
                for (i = k; i < N; i++)
                    for (j = k; j < N; j++) // find biggest element
                        if (Math.Abs(big) <= Math.Abs(a[i, j]))
                        {
                            big = a[i, j];
                            ik[k] = i;
                            jk[k] = j;
                        }
                if (big == 0.0) return 0.0;
                i = ik[k];
                if (i > k)
                    for (j = 0; j < N; j++) // exchange rows
                    {
                        save = a[k, j];
                        a[k, j] = a[i, j];
                        a[i, j] = -save;
                    }
                j = jk[k];
                if (j > k)
                    for (i = 0; i < N; i++)
                    {
                        save = a[i, k];
                        a[i, k] = a[i, j];
                        a[i, j] = -save;
                    }
                for (i = 0; i < N; i++) // build the inverse
                    if (i != k)
                        a[i, k] = -a[i, k]/big;
                for (i = 0; i < N; i++)
                    for (j = 0; j < N; j++)
                        if ((i != k) && (j != k))
                            a[i, j] += a[i, k]*a[k, j];
                for (j = 0; j < N; j++)
                    if (j != k)
                        a[k, j] /= big;
                a[k, k] = 1.0/big;
                det *= big; // bomb point
            } // end k loop
            for (L = 0; L < N; L++)
            {
                k = N - L - 1;
                j = ik[k];
                if (j > k)
                    for (i = 0; i < N; i++)
                    {
                        save = a[i, k];
                        a[i, k] = -a[i, j];
                        a[i, j] = save;
                    }
                i = jk[k];
                if (i > k)
                    for (j = 0; j < N; j++)
                    {
                        save = a[k, j];
                        a[k, j] = -a[i, j];
                        a[i, j] = save;
                    }
            }
            return det;
        }
    }

    //-----------end of class LM--------------------
}

