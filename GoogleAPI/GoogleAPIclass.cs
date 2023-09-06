using Google.Analytics.Data.V1Beta;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace GoogleAPI
{
    public class GoogleAPIclass
    {
        public static void SampleRunReport(string propertyId, string data, string dalis)
        {
            DeleteFromDb(data, data, dalis);
            Console.WriteLine("propertyId " + propertyId); 
            Console.WriteLine("Deleted " + dalis);


            /**
             * TODO(developer): Uncomment this variable and replace with your
             *  Google Analytics 4 property ID before running the sample.
             */
            // propertyId = "YOUR-GA4-PROPERTY-ID";

            // Using a default constructor instructs the client to use the credentials
            // specified in GOOGLE_APPLICATION_CREDENTIALS environment variable.
            //BetaAnalyticsDataClient client = BetaAnalyticsDataClient.Create();

            Console.WriteLine("creating client: " + ConfigurationManager.AppSettings["ConnectionFile"]);  
            BetaAnalyticsDataClient client = new BetaAnalyticsDataClientBuilder() { JsonCredentials = File.ReadAllText(ConfigurationManager.AppSettings["ConnectionFile"] ?? "") }.Build();
            Console.WriteLine("clien created ");
            RunReportRequest request;


            int offset = 0;
            int limit = Int32.Parse(ConfigurationManager.AppSettings["limit"]);
            string startDate = data;
            string endDate = data;

            int rowCount = 1;
            RunReportResponse response;

            do
            {
                request = new RunReportRequest
                {
                    Property = "properties/" + propertyId,
                    Dimensions = { new Dimension { Name = "itemId" }, },
                    Metrics = { new Metric { Name = "sessions" }, },
                    DateRanges = { new DateRange { StartDate = startDate, EndDate = endDate }, },
                    Limit = limit,
                    Offset = offset,
                };
                response = client.RunReport(request);
                WriteToFile(response.Rows.ToString(), startDate, endDate, offset);
                SaveToDb(response.Rows.ToString(), startDate, endDate, dalis);
                Console.WriteLine("Offset {0}, rowcount{1}", offset, rowCount);
                rowCount = response.RowCount;
                offset = offset + limit;
            } while (offset < rowCount);

        }

        public static void SaveToDb(string response, string startDate, string endDate, string dalis)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnString_Analitika"].ConnectionString);
                SqlCommand cmd = new SqlCommand("GoogleAPI_save", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dataNuo", startDate);
                cmd.Parameters.AddWithValue("@dataIki", endDate);
                cmd.Parameters.AddWithValue("@body", response);
                cmd.Parameters.AddWithValue("@dalis", dalis);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("---------");
                Console.WriteLine("Data nuo: " + startDate);
                Console.WriteLine("Data iki: " + endDate);
                Console.WriteLine("Dalis: " + dalis);
                Console.WriteLine("Response: " + response);
                Console.WriteLine("Error: " + ex);
            }
        }

        public static void DeleteFromDb(string startDate, string endDate, string dalis)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnString_Analitika"].ConnectionString);
                SqlCommand cmd = new SqlCommand("GoogleAPI_delete", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dataNuo", startDate);
                cmd.Parameters.AddWithValue("@dataIki", endDate);
                cmd.Parameters.AddWithValue("@dalis", dalis);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("---------");
                Console.WriteLine("Data nuo: " + startDate);
                Console.WriteLine("Data iki: " + endDate);
                Console.WriteLine("Action: delete");
                Console.WriteLine("Error: " + ex);
            }
        }

        public static void WriteToFile(string text, string startDate, string endDate, int offset)
        {
            // Console.WriteLine("{0}, {1}", \\\\magento\\responses\\Temp\\LemonaAPI_response_{0}_{1}_offset_{2}.json", startDate, endDate, offset.ToString());
            string fileName = string.Format("\\\\magento\\responses\\GoogleAPI\\LemonaAPI_response_{0}_{1}_offset_{2}.json", startDate, endDate, offset.ToString());
            Console.WriteLine("Saving file: " + fileName);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(text);
            }
            Console.WriteLine("File saved");

        }

    }
}
