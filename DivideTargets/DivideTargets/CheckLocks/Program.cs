using System;

namespace CheckLocks
{
    class Program
    {
        static void Main(string[] args)
        {

            string folderWithLocks = @"E:\ScottK\IQ\RunFiles\LocksFolder";

            string pathToController = folderWithLocks + @"\" + "LockController.txt";

            bool areLocksReady = LockCheck.AreLocksReady(pathToController, folderWithLocks, pathToController);

           
            Console.ReadKey();
        }

        
    }
}
