using Google.Analytics.Data.V1Beta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI
{
    public class GoogleAPIclass
    {
        public static void SampleRunReport(string propertyId = "249719553")
        {
            /**
             * TODO(developer): Uncomment this variable and replace with your
             *  Google Analytics 4 property ID before running the sample.
             */
            // propertyId = "YOUR-GA4-PROPERTY-ID";

            // Using a default constructor instructs the client to use the credentials
            // specified in GOOGLE_APPLICATION_CREDENTIALS environment variable.
            //BetaAnalyticsDataClient client = BetaAnalyticsDataClient.Create();
            BetaAnalyticsDataClient client = new BetaAnalyticsDataClientBuilder() { JsonCredentials = File.ReadAllText("Z:\\Analitika\\Vienkartiniai\\Gediminas\\GoogleAPI\\LemonaAPI-4052fb0ed930.json") }.Build();

            // Initialize request argument(s)
            RunReportRequest request = new RunReportRequest
            {
                Property = "properties/" + propertyId,
                Dimensions = { new Dimension { Name = "city" }, },
                Metrics = { new Metric { Name = "activeUsers" }, },
                DateRanges = { new DateRange { StartDate = "2020-03-31", EndDate = "today" }, },
            };

            // Make the request
            var response = client.RunReport(request);

            Console.WriteLine("Report result:");
            foreach (Row row in response.Rows)
            {
                Console.WriteLine("{0}, {1}", row.DimensionValues[0].Value, row.MetricValues[0].Value);
            }
        }
        //static int Main(string[] args)
        //{
        //    SampleRunReport();
        //    return 0;
        //}
    }
}
