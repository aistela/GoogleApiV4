
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.Services;
using System.Collections.Generic;
using System.Linq;
//using Google.Api;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json;
using Grpc.Core;

namespace GoogleAPI
{
    public class GoogleAPIclass
    {
        public static void GetReportData(UserCredential credential, string propertyId, string startDate, string endDate, string dalis)
        {
            int offset = 0;
            int limit = int.Parse(ConfigurationManager.AppSettings["limit"]);
            int? rowCount = 1;
            string resultJson;
            GetReportsResponse response;
            using (var svc = new AnalyticsReportingService(
                    new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "Google Analytics API Console"
                    }))
            {
                var dateRange = new DateRange
                {
                    StartDate = startDate,
                    EndDate = endDate
                };
                var sessions = new Metric
                {
                    Expression = "ga:productDetailViews"
                };

                do
                {
                    var reportRequest = new ReportRequest
                    {
                        DateRanges = new List<DateRange> { dateRange },
                        Dimensions = new List<Dimension> { new Dimension { Name = "ga:productSku" }, new Dimension { Name = "ga:eventAction" } },
                        Metrics = new List<Metric> { sessions },
                        ViewId = propertyId,
                        PageToken = offset.ToString(),
                        PageSize = limit

                    };
                    var getReportsRequest = new GetReportsRequest
                    {
                        ReportRequests = new List<ReportRequest> { reportRequest }
                    };
                    var batchRequest = svc.Reports.BatchGet(getReportsRequest);
                    response = batchRequest.Execute();
                    resultJson = JsonConvert.SerializeObject(response.Reports.First().Data.Rows);
                    WriteToFile(resultJson, startDate, offset, propertyId);
                    SaveToDb(resultJson, startDate, endDate, dalis);
                    rowCount = response.Reports.First().Data.RowCount;

                    offset = offset + limit;
                } while (offset < rowCount);


                //string strJson = JsonConvert.SerializeObject(response.Reports.First().Data);
                //Console.WriteLine(strJson);
            }
        }

        public static void SaveToDb(string response, string startDate, string endDate, string dalis)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnString_Analitika"].ConnectionString);
                SqlCommand cmd = new SqlCommand("GoogleAPI_old_save", conn);
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
                //Console.WriteLine("Response: " + response);
                Console.WriteLine("Error: " + ex);
            }
        }

        public static void DeleteFromDb(string startDate, string endDate, string dalis)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnString_Analitika"].ConnectionString);
           
                SqlCommand cmd = new SqlCommand("GoogleAPI_old_delete", conn);
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

        public static void WriteToFile(string text, string startDate, int offset, string propertyId )
        {
            string fileName = ConfigurationManager.AppSettings["ResposeFolder"] + string.Format("\\LemonaAPI_response_{0}_{1}_offset_{2}.json", propertyId, startDate, offset.ToString());
            Console.WriteLine("Saving file: " + fileName);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(text);
            }
            Console.WriteLine("File saved");
        }
    }
}
