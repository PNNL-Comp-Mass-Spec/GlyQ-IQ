using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Parallel
{
    public class ParalellAssignEngine
    {
        /// <summary>
        /// When all engines are being used, how many times we try in stage 2 before proceding to stage 3 back up plans
        /// </summary>
        public int AmountOfTries { get; set; }

        /// <summary>
        /// How long to wait between trys when all engines are full
        /// </summary>
        public int WaitTimeInMiliseconds { get; set; }

        private List<ParalellEngine> Engines { get; set; }

        private List<ParalellEngine> ExtraEngines { get; set; }

        public ParalellAssignEngine(ParalellEngineStation engineStation)
        {
            AmountOfTries = 1000;
            //WaitTimeInMiliseconds = 500;
            WaitTimeInMiliseconds = 1000;
            Engines = engineStation.Engines;
            ExtraEngines = engineStation.ExtraEngines;
        }

        public void ResetEngine(ParalellEngine engine, ParalellEngineStation engineStation)
        {
            engine.Active = false;
            engineStation.ErrorPile[engine.EngineNumber] = engine.ErrorLog;
        }

        /// <summary>
        /// Find and select an available engine for the thread
        /// </summary>
        /// <param name="engines">pre setup engines for possible use</param>
        /// <param name="extraEngines">bonuse engine outside of main engine pool used for waiting</param>
        /// <returns>open engine</returns>
        public ParalellEngine Assign()
        {
            ParalellEngine transformerMulti;//what to assign

            #region Level 1:  Simple assign when an engine is open

            int initialNumberOfTransformers = Engines.Count();
            int engineCounter = 0;

            while (engineCounter < initialNumberOfTransformers)
            {
                if (Engines[engineCounter].Active == false)
                {
                    Engines[engineCounter].Active = true;
                    transformerMulti = Engines[engineCounter];
                    engineCounter = 0;
                    Console.WriteLine("   Level 1");
                    return transformerMulti;
                }
                else
                {
                    engineCounter++;
                }
            }

            #endregion

            #region Backup Plan 1:  Evertything is being used so to to a waiting pool that iterates and waits

            int triesCounter = 0;
            while (triesCounter < AmountOfTries)
            {
                Thread.Sleep(WaitTimeInMiliseconds);//wait a bit and then try again

                engineCounter = 0;
                while (engineCounter < initialNumberOfTransformers)
                {
                    if (Engines[engineCounter].Active == false)
                    {
                        Engines[engineCounter].Active = true;
                        transformerMulti = Engines[engineCounter];
                        engineCounter = 0;
                        Console.WriteLine("       Level 2");
                        return transformerMulti;
                    }
                    else
                    {
                        engineCounter++;
                    }
                }

                triesCounter++;
                Console.WriteLine("loop again, all engines active... " + triesCounter);
            }

            #endregion

            #region Backup Plan 2:  grid lock so we wait till we can assign.  Utilize bonus engines seperate from normal pool for this

            Console.WriteLine("not enough threads");
            ParalellEngine tempEngine;

            if (ExtraEngines.Count > 0)//just incase this is not set up
            {
                tempEngine = ExtraEngines[0];

                switch (tempEngine.Active)
                {
                    case false:
                        {
                            transformerMulti = tempEngine;
                            Console.WriteLine("             Level 3 Once");
                            return transformerMulti;
                        }
                    case true:
                        {
                            while (tempEngine.Active == true)
                            {
                                Thread.Sleep(WaitTimeInMiliseconds);
                            }
                            Console.WriteLine("             Level 3 Force");
                            transformerMulti = tempEngine;
                            return transformerMulti;
                        }
                    default:
                        {
                            while (tempEngine.Active == true)
                            {
                                Thread.Sleep(WaitTimeInMiliseconds);
                            }
                            transformerMulti = tempEngine;
                            Console.WriteLine("             Level 3 Default");
                            return transformerMulti;
                        }
                }
            }
            else //grab the first engine and chew on it till it works
            {
                tempEngine = Engines[0];

                while (tempEngine.Active == true)
                {
                    Thread.Sleep(WaitTimeInMiliseconds);
                    tempEngine.Active = false;
                }
                transformerMulti = tempEngine;
                Console.WriteLine("             Level 3 Default");
                return transformerMulti;
            }

            #endregion
        }
    }
}
