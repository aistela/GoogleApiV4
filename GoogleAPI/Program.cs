using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DateTime data;
            if (args.Length == 0)
            {
               data = DateTime.Now.AddDays(-1);
            }
            else
            {
                _ = DateTime.TryParse(args[0], out data);
            }

            Console.WriteLine("Start " + data);

            GoogleAPIclass.SampleRunReport(ConfigurationManager.AppSettings["propertyId-LT"], data.ToString("yyyy-MM-dd"), "LT");
            GoogleAPIclass.SampleRunReport(ConfigurationManager.AppSettings["propertyId-LV"], data.ToString("yyyy-MM-dd"), "LV");
            GoogleAPIclass.SampleRunReport(ConfigurationManager.AppSettings["propertyId-EE"], data.ToString("yyyy-MM-dd"), "EE");
            GoogleAPIclass.SampleRunReport(ConfigurationManager.AppSettings["propertyId-EU"], data.ToString("yyyy-MM-dd"), "EU");
        }
    }
}
