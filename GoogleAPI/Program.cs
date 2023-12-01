
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Google.Apis.AnalyticsReporting.v4;

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
                Console.WriteLine("Nenurodyta data. STOP.");
                return;
            }
            else
            {
                _ = DateTime.TryParse(args[0], out data);
            }

            //data = DateTime.Now.AddDays(-680);
            Console.WriteLine("Start " + data);

            var UserCredential = GetCredential().Result;
            GoogleAPIclass.GetReportData(UserCredential, ConfigurationManager.AppSettings["propertyId-LT"], data.ToString("yyyy-MM-dd"), data.ToString("yyyy-MM-dd"), "LT");
            GoogleAPIclass.GetReportData(UserCredential, ConfigurationManager.AppSettings["propertyId-LV"], data.ToString("yyyy-MM-dd"), data.ToString("yyyy-MM-dd"), "LV");
            GoogleAPIclass.GetReportData(UserCredential, ConfigurationManager.AppSettings["propertyId-EE"], data.ToString("yyyy-MM-dd"), data.ToString("yyyy-MM-dd"), "EE");
        }


        static async Task<UserCredential> GetCredential()
        {
            using (var stream = new FileStream("\\\\rex\\SQL_XML\\FILES\\GoogleAPI\\GoogleAPIanalyticsOldData.json",
                 FileMode.Open, FileAccess.Read))
            {
                const string loginEmailAddress = "lemona.api@gmail.com";
                return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { AnalyticsReportingService.Scope.Analytics },
                    loginEmailAddress, CancellationToken.None,
                    new FileDataStore("GoogleAnalyticsApiConsole"));
            }
        }

    }
}
