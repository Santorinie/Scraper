using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Statistics stats = new Statistics();
        }
    }

    class WebScraper
    {
        private const string FirstData = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/01-22-2020.csv";
        public static DateTime FirstDate = new DateTime(2020, 1, 22);
        // Uses a Database format since this date
        
        public static DateTime NewFormat = new DateTime(2020,7,17);


        public WebScraper()
        {

        }

        public static dynamic GetPageContent_All(bool WriteToFile)
        {

            if (WriteToFile)
            {
                var DaysPassed = DateTime.Now - FirstDate;
                DateTime currentDate;
                string currentDateString;
                for (int i = 0; i < DaysPassed.Days; i++)
                {
                    currentDate = FirstDate.AddDays(i);
                    currentDateString = $"{currentDate.Year}-{currentDate.Month.ToString("d2")}-{currentDate.Day.ToString("d2")}.txt";
                    if (!File.Exists(currentDateString))
                    {
                        File.WriteAllTextAsync(currentDateString, GetPageContent_Single(GetLink_Custom(FirstDate.AddDays(i))));

                        //client.DownloadFile(GetLink_Custom(FirstDate.AddDays(i)), currentDateString);
                    }
                }

                return null;
            }
            else
            {
                var DaysPassed = DateTime.Now - FirstDate;

                List<string> PageContents = new List<string>();

                for (int i = 0; i < DaysPassed.Days; i++)
                {
                    PageContents.Add(GetPageContent_Single(GetLink_Custom(FirstDate.AddDays(i))));
                    Console.WriteLine(i);
                }

                return PageContents;

            }
            
 
        }

        public static string GetFileName(DateTime date)
        {
            
            return $"{date.Year}-{date.Month.ToString("d2")}-{date.Day.ToString("d2")}.txt";

        }

        public static string GetPageContent_Single(string url)
        {
            var httpClient = new HttpClient();
            return httpClient.GetStringAsync(url).Result;
        }

        public static string GetLink_Custom(DateTime customDate)
        {
            
            string date = $"{customDate.Month.ToString("d2")}-{customDate.Day.ToString("d2")}-{customDate.Year}";
            return $"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/{date}.csv";

        }

        public static string GetLink_Today()
        {
            var today = DateTime.Now;
            string date = $"{today.Month.ToString("d2")}-{today.Day.ToString("d2")}-{today.Year}";
            return $"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/{date}.csv";
        }


    }

    class Statistics
    {
        
        public Statistics()
        {
            

            Console.WriteLine();
        }

        private static void Parse(DateTime date)
        {
            var inputLine1 = File.ReadAllLines(WebScraper.GetFileName(date));
            var input = inputLine1.Skip(1).First();
            
            var temp = input.Split(",");

            NewStatsModel model;
            if (DetermineFormat(inputLine1.First()) == "new")
            {
                model = new NewStatsModel()
                {
                    FIPS = temp[0],
                    Admin2 = temp[1],
                    Province_State = temp[2],
                    Country_Region = temp[3],
                    Last_Update = GetCorrectDateFormat(temp[4], "new"),
                    Latitude = double.Parse(temp[5]),
                    Longitude = double.Parse(temp[6]),
                    Confirmed = double.Parse(temp[7]),
                    Deaths = double.Parse(temp[8]),
                    Recovered = double.Parse(temp[9]),
                    Active = double.Parse(temp[10]),
                    Combined_Key = temp[11],
                    Incident_Rate = double.Parse(temp[12]),
                    Case_Fatality_Ratio = double.Parse(temp[13]),
                    Format = "new"

                };
            }
            else
            {

                model = new NewStatsModel()
                {


                    Province_State = temp[0],
                    Country_Region = temp[1],
                    Last_Update = GetCorrectDateFormat(temp[2], "old"),
                    Confirmed = double.TryParse(temp[3], out _) ? double.Parse(temp[3]) : null,
                    Deaths = double.TryParse(temp[4], out _) ? double.Parse(temp[4]) : null,
                    Recovered = double.TryParse(temp[5], out _) ? double.Parse(temp[5]) : null,
                    Format = "old"

                };
            }
        }



        private static string DetermineFormat(string input)
        {

            return input.Contains("FIPS") ? "new" : "old";
        }

        public static DateTime GetCorrectDateFormat(string datestring, string format)
        {


            return format == "new"

                  ? DateTime.ParseExact(datestring, "yyyy-MM-dd HH:mm:ss",
                      System.Globalization.CultureInfo.InvariantCulture)

                  : DateTime.ParseExact(datestring, "M/dd/yyyy HH:mm",
                      System.Globalization.CultureInfo.InvariantCulture);


        }
    }

    class NewStatsModel
    {

        //FIPS,Admin2,Province_State,Country_Region,Last_Update,Lat,Long_,Confirmed,Deaths,Recovered,Active,Combined_Key,Incident_Rate,Case_Fatality_Ratio

        public string FIPS { get; set; }
        public string Admin2 { get; set; }
        public string Province_State { get; set; }
        public string Country_Region { get; set; }
        public DateTime Last_Update { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Confirmed { get; set; }
        public double? Deaths { get; set; }
        public double? Recovered { get; set; }
        public double Active { get; set; }
        public string Combined_Key { get; set; }
        public double Incident_Rate { get; set; }
        public double Case_Fatality_Ratio { get; set; }
        public string Format { get; set; }


    }

    
}
