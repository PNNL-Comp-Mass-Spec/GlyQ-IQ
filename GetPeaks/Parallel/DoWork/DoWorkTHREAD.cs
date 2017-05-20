using Parallel.THRASH;

namespace Parallel.DoWork
{
    public static class DoWorkTHREAD
    {
        public static ParalellResults WorkWork(ParalellThreadData data)
        {
            ParalellThreadData currentParameterObject = (ParalellThreadData)data;

            //int largeNumber = 10000000;
            int largeNumber = 1;
            double sum = 0;

            //ParalellResults objectInsideThread = new ParalellResults(data.Scan);
            ResultsTHRASH objectInsideThread = new ResultsTHRASH(currentParameterObject.Scan);

            for (int j = 0; j < largeNumber; j++)
            {
                sum += j;
            }
            objectInsideThread.Sum = sum;

            return objectInsideThread;
        }

        public static ParalellResults WorkWork(int scan)
        {
            int largeNumber = 1;
            double sum = 0;

            //ParalellResults objectInsideThread = new ParalellResults(scan);
            ResultsTHRASH objectInsideThread = new ResultsTHRASH(scan);

            for (int j = 0; j < largeNumber; j++)
            {
                sum += j;
            }
            objectInsideThread.Sum = sum;

            return objectInsideThread;
        }
    }
}
