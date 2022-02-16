using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

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
        private static DateTime firstDate = new DateTime(2020, 1, 22);
        private static List<string> countries;

        public Statistics()
        {
            countries = GetCountries();
            DoSomeCrazyStuff();


            Console.WriteLine();
        }


        private static void DoSomeCrazyStuff()
        {
            // parse a file
            // Foreach them
            // save every data to its corresponding txt


            var dayspassed = DateTime.Now - firstDate;
            string filename;
            IEnumerable<string> input;
            StreamWriter sw;
            for (int i = 0; i < dayspassed.Days; i++)
            {
                Console.WriteLine(i);
                filename = WebScraper.GetFileName(firstDate.AddDays(i));
                input = File.ReadAllLines(filename).Skip(1);

                foreach (var sor in input)
                {
                    foreach (var country in countries)
                    {
                        if (sor.Contains(country))
                        {
                            sw = new StreamWriter($"{country}.txt",true,Encoding.UTF8);
                            sw.WriteLine(sor);
                            sw.Close();

                        }
                    }
                }
            }

        }

        private static List<string> GetCountries()
        {
            //var DaysPassed = DateTime.Now - firstDate;
            HashSet<string> countries = new HashSet<string>();
            List<StatsModel> data = new List<StatsModel>();
            for (int i = 250; i < 376; i++)
            {

                data = Parse(firstDate.AddDays(i));
                foreach (var item in data)
                {
                    countries.Add(item.Country_Region);
                }
                Console.WriteLine(i);

            }




            return countries.ToList();

        }

        private static List<StatsModel> Parse(DateTime date)
        {
            var inputLine1 = File.ReadAllLines(WebScraper.GetFileName(date));
            var input = inputLine1.Skip(1);
            List<StatsModel> models = new List<StatsModel>();
            var result = DetermineFormat(inputLine1.First());
            foreach (var item in input)
            {
                
                var temp = item.Replace(", ", ";").Replace("\"","").Split(",");
                if (result == "new")
                {
                    models.Add(new StatsModel(temp[0],temp[1], temp[2], temp[3], temp[4], temp[5], temp[6], temp[7], temp[8], temp[9]
                        , temp[10], temp[11], temp[12], temp[13], result, date));

                }
                else
                {
                    models.Add(new StatsModel(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], result, date));
                }


            }
            return models;
        }


           

        private static string DetermineFormat(string input)
        {

            return input.Contains("FIPS") ? "new" : "old";
        }

        public static DateTime GetCorrectDateFormat(string datestring, string format)
        {
            datestring = datestring.Split(" ").First();

                

            try
            {
                return format == "new"

                  ? DateTime.ParseExact(datestring, "yyyy-MM-dd",
                      System.Globalization.CultureInfo.InvariantCulture)

                  : DateTime.ParseExact(datestring, "M/dd/yyyy",
                      System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {

            }
            try
            {
                return format == "new"

                  ? DateTime.ParseExact(datestring, "yyyy-MM-dd",
                      System.Globalization.CultureInfo.InvariantCulture)

                  : DateTime.ParseExact(datestring, "M/dd/yy",
                      System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {

            }
            return DateTime.Now;

        }
    }

    class StatsModel
    {

        //FIPS,Admin2,Province_State,Country_Region,Last_Update,Lat,Long_,Confirmed,Deaths,Recovered,Active,Combined_Key,Incident_Rate,Case_Fatality_Ratio

        public string FIPS { get; set; }
        public string Admin2 { get; set; }
        public string Province_State { get; set; }
        public string Country_Region { get; set; }
        public DateTime? Last_Update { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Confirmed { get; set; }
        public double? Deaths { get; set; }
        public double? Recovered { get; set; }
        public double? Active { get; set; }
        public string Combined_Key { get; set; }
        public double? Incident_Rate { get; set; }
        public double? Case_Fatality_Ratio { get; set; }
        public string Format { get; set; }


        public StatsModel(string fips,string admin2, string provincestate, string country, string lastupdate, string latitude,
            string longitude, string confirmed, string deaths, string recovered, string active, string combinedkey, string incidentrate,
            string casefatalityratio, string format,DateTime sender)
        {
            this.FIPS = fips;
            this.Admin2 = admin2;
            this.Province_State = provincestate;
            this.Country_Region = country;
            this.Last_Update = sender;
            this.Latitude = latitude != "" && double.TryParse(latitude, out _) ? Convert.ToDouble(latitude) : null;
            this.Longitude = longitude != "" && double.TryParse(longitude, out _) ? Convert.ToDouble(longitude) : null;
            this.Confirmed = confirmed != "" && double.TryParse(confirmed, out _) ? Convert.ToDouble(confirmed) : null;
            this.Deaths = deaths != "" && double.TryParse(deaths, out _) ? Convert.ToDouble(deaths) : null;
            this.Recovered = recovered != "" && double.TryParse(recovered, out _) ? Convert.ToDouble(recovered) : null;
            this.Active = active != "" && double.TryParse(active, out _) ? Convert.ToDouble(active) : null;
            this.Combined_Key = combinedkey;
            this.Incident_Rate = incidentrate != "" && double.TryParse(incidentrate, out _) ? Convert.ToDouble(incidentrate) : null;
            this.Case_Fatality_Ratio = casefatalityratio != "" && double.TryParse(casefatalityratio, out _) ? Convert.ToDouble(casefatalityratio) : null;
            this.Format = format;
        }

        public StatsModel(string province, string country, string lastupdate, string confirmed, string deaths, string recovered,string format, DateTime sender)
        {
            // Province/State,Country/Region,Last Update,Confirmed,Deaths,Recovered

            this.Province_State = province;
            this.Country_Region = country;
            this.Last_Update = sender;
            this.Confirmed = confirmed != "" && double.TryParse(confirmed, out _) ? Convert.ToDouble(confirmed) : null;
            this.Deaths = deaths != "" && double.TryParse(deaths, out _) ? Convert.ToDouble(deaths) : null;
            this.Recovered = recovered != "" && double.TryParse(recovered, out _) ? Convert.ToDouble(recovered) : null;
            this.Format = format;
        }


    }

    
}
