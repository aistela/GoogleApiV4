using System;
using System.Configuration;

namespace GoogleAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DateTime data;

            _ = int.TryParse(ConfigurationManager.AppSettings["delayInDays"] ?? "1", out int delayInDays);
            if (args.Length == 0)
            {
               data = DateTime.Now.AddDays(-1 * delayInDays);
            }
            else
            {
                _ = DateTime.TryParse(args[0], out data);
            }

            //data = DateTime.Now.AddDays(-14);
            Console.WriteLine("Start " + data);

            GoogleAPIclass.SampleRunReport(ConfigurationManager.AppSettings["propertyId-LT"], data.ToString("yyyy-MM-dd"), "LT");
            GoogleAPIclass.SampleRunReport(ConfigurationManager.AppSettings["propertyId-LV"], data.ToString("yyyy-MM-dd"), "LV");
            GoogleAPIclass.SampleRunReport(ConfigurationManager.AppSettings["propertyId-EE"], data.ToString("yyyy-MM-dd"), "EE");
            GoogleAPIclass.SampleRunReport(ConfigurationManager.AppSettings["propertyId-EU"], data.ToString("yyyy-MM-dd"), "EU");
        }
    }
}
