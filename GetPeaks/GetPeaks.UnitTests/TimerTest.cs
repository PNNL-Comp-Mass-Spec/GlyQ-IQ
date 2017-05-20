using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GetPeaks_DLL.Functions;

namespace GetPeaks.UnitTests
{
    public class TimerTest
    {
        [Test]
        public void timerTest()
        {
            //LabTimer timer = new LabTimer();
            //timer.Start(0, 0, 30);

           // timer.Alarm += (sender, e) => MessageBox.Show("Wake up!");
            //timer.Alarm += (sender, e) => Console.WriteLine("Wake Up");

            LabTimer clock = new LabTimer();
            clock.Start(0, 0, 30);
            clock.Alarm += (sender, e) => Console.WriteLine("Wake Up"); Console.ReadKey(); 
        }
    }
}
