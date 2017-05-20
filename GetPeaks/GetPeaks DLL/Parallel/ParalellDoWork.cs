using System;
using Parallel.THRASH;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Parallel.DoWork;

namespace GetPeaks_DLL.Parallel
{
    public static class ParalellDoWork
    {
        public static ParalellResults DoWorkWithController(int threadNumber)
        {
            try
            {
                ParalellResults objectInsideThread = DoWorkTHREAD.WorkWork(threadNumber);
                objectInsideThread.ThreadNumber = threadNumber;
                return objectInsideThread;
            }
            catch 
            {
                ResultsTHRASH objectInsideThread = new ResultsTHRASH(threadNumber);
                objectInsideThread.ThreadNumber = threadNumber;
                return objectInsideThread;
            }
        }

        public static ParalellResults DoWorkWithController(ParalellThreadData data)
        {
            ParalellThreadData currentParameterObject = (ParalellThreadData)data;

            try
            {
                ParalellResults objectInsideThread = DoWorkTHREAD.WorkWork((ParalellThreadDataTHRASH)data);
                objectInsideThread.EngineNumber = data.Engine.EngineNumber;
                return objectInsideThread;
            }
            catch 
            {
                Console.WriteLine("Fail!!!!!!!!!!!!!!!!!!!!");
                Console.ReadKey();
                ResultsTHRASH objectInsideThread = new ResultsTHRASH(currentParameterObject.Scan);
                objectInsideThread.EngineNumber = data.Engine.EngineNumber;
                return objectInsideThread;
            }
        }

        public static ParalellResults DoWorkWithControllerThrash(ParalellThreadData data)
        {
            try
            {
                ParalellResults objectInsideThread = DoWorkTHRASH.WorkWork((ParalellThreadDataTHRASH)data);
                objectInsideThread.EngineNumber = data.Engine.EngineNumber;
                return objectInsideThread;
            }
            catch
            {
                Console.WriteLine("Fail!!!!!!!!!!!!!!!!!!!!");
                Console.ReadKey();
                ResultsTHRASH objectInsideThread = new ResultsTHRASH(data.Scan);
                objectInsideThread.EngineNumber = data.Engine.EngineNumber;
                return objectInsideThread;
            }
        }
    }
}
