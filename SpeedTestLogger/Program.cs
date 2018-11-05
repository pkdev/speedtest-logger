using System;
using System.Globalization;
using System.Linq;
using SpeedTest;

namespace SpeedTestLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello SpeedTestLogger!");
            var config = new LoggerConfiguration();
            var runner = new SpeedTestRunner(config.LoggerLocation);
            runner.RunSpeedTest();          
        }
    }
}
