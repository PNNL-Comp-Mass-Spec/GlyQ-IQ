using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace GetPeaks_DLL.Functions
{
    public class LabTimer
    {
        int duration = 0;
        string lblCountdown = "";
        int ticks = 0;
        bool timeup = false;
        //System.Timers.Timer timer { get; set; }

        public LabTimer()
        {
           //LabTimer = new System.Timers.Timer();
        }

        private EventHandler alarmEvent;
        private System.Timers.Timer timer;
        private DateTime alarmTime;
        private bool enabled;

        public void Start(int hours, int minutes, int seconds)
        {
            DateTime timeTOGoOff = DateTime.Now;

            System.Timers.Timer timer = new System.Timers.Timer();
            alarmTime = timeTOGoOff;
            duration = 10; //secs
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 1000 * duration;//ms
            timer.Start();

            enabled = true;
        }

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    duration--;
        //    lblCountdown = duration.ToString();
        //    if (duration == ticks) timeup = true;

        //    //To beep, call the Visual Basic library, remember to add a reference to it 
        //    //in the Reference section of Solution Explorer    
        //    if (timeup)
        //    {
        //        Console.WriteLine("end");
        //    }
        //}

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        { 
            if (enabled && DateTime.Now > alarmTime)
            {
                enabled = false;
                OnAlarm();
                timer.Stop();
            }
        }

        protected virtual void OnAlarm()
        {
            if (alarmEvent != null)
            {
                alarmEvent(this, EventArgs.Empty);
            }
        }

        public event EventHandler Alarm 
        {
            add { alarmEvent += value; }
            remove { alarmEvent -= value; }
        } 
    }
}
