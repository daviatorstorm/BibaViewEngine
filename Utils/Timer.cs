using System;

namespace BibaViewEngine.Utils
{
    public class Timer
    {
        DateTime counter;
        string CounterName { get; set; }

        private Timer(string name)
        {
            counter = DateTime.Now;
            CounterName = name;
        }

        public static Timer Start(string counterName)
        {
            var timer = new Timer(counterName);
            System.Console.WriteLine("{0} started", counterName);
            return timer;
        }

        public TimeSpan Stop()
        {
            var timeSpared = DateTime.Now - counter;
            System.Console.WriteLine("Name: {0}. Time spared: {1}", CounterName, timeSpared.TotalMilliseconds);
            return timeSpared;
        }
    }
}