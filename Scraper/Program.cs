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
            WebScraper scraper = new WebScraper();
            Statistics stats = new Statistics();
        }
    }

    class WebScraper
    {
        private const string FirstData = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/01-22-2020.csv";
        public static DateTime FirstDate = new DateTime(2020, 1, 22);
        private static List<string> countries;

        // Uses a new format since this date
        public static DateTime NewFormat = new DateTime(2020,5,30);

        private static DateTime LastFetched;
        private static double DaysPassed; 


        public WebScraper()
        {
            Init();
        }

        private static void Init()
        {
            DaysPassed = (DateTime.Now - FirstDate).Days;
            LastFetched = GetFetchDate();
            countries = GetCountries();
            GetPageContent_All(true); // fetch
        }

        public static void Categorize_All()
        {
            // parse a file
            // Foreach them
            // save every data to its corresponding txt


            var dayspassed = DateTime.Now - FirstDate;
            string filename;
            IEnumerable<string> input;
            StreamWriter sw;
            for (int i = 0; i < dayspassed.Days; i++)
            {
                Console.WriteLine(i);
                filename = WebScraper.GetFileName(FirstDate.AddDays(i));
                input = File.ReadAllLines(filename).Skip(1);

                foreach (var sor in input)
                {
                    foreach (var country in countries)
                    {
                        if (sor.Contains(country))
                        {
                            sw = new StreamWriter($"{country}.txt", true, Encoding.UTF8);
                            sw.WriteLine(sor);
                            sw.Close();

                        }
                    }
                }
            }

        }

        public static void Categorize_Single(IEnumerable<string> data)
        {
            StreamWriter sw;
            foreach (var line in data.Skip(1))
            {
                foreach (var country in countries)
                {
                    if (line.Contains($",{country},"))
                    {
                        sw = new StreamWriter($"{country}.txt",true,Encoding.UTF8);
                        sw.WriteLine(line);
                        sw.Close();
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

                data = Parse(FirstDate.AddDays(i));
                foreach (var item in data)
                {
                    countries.Add(item.Country_Region);
                }
                Console.WriteLine(i);

            }




            return countries.ToList();

        }

        public static List<StatsModel> Parse(DateTime date)
        {
            var inputLine1 = File.ReadAllLines(WebScraper.GetFileName(date));
            var input = inputLine1.Skip(1);
            List<StatsModel> models = new List<StatsModel>();
            var result = DetermineFormat(inputLine1.First());
            foreach (var item in input)
            {

                var temp = item.Replace(", ", ";").Replace("\"", "").Split(",");
                if (result == "new")
                {
                    models.Add(new StatsModel(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], temp[6], temp[7], temp[8], temp[9]
                        , temp[10], temp[11], temp[12], temp[13], result, date));

                }
                else
                {
                    models.Add(new StatsModel(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], result, date));
                }


            }
            return models;
        }

        public static IEnumerable<StatsModel> Parse(IEnumerable<string> data)
        {
            
            var input = data.Skip(1);
            List<StatsModel> models = new List<StatsModel>();
            //var result = DetermineFormat(inputLine1.First());
            
            foreach (var item in input)
            {

                var temp = item.Replace(", ", ";").Replace("\"", "").Split(",");
                
                if (GetCorrectDateFormat(temp[4],"new") >= NewFormat)
                {
                    models.Add(new StatsModel(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], temp[6], temp[7], temp[8], temp[9]
                        , temp[10], temp[11], temp[12], temp[13], "new", GetCorrectDateFormat(temp[4],"new")));

                }
                //else
                //{
                //    models.Add(new StatsModel(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], result, GetCorrectDateFormat(temp[2],"old")));
                //}

               
            }
            return models;
        }

        private static DateTime GetFetchDate()
        {
            for (int i = 0; i >= -DaysPassed; i--)
            {
                var currentDate = DateTime.Now.AddDays(i);
                if (File.Exists(GetFileName(currentDate)))
                {

                    return currentDate;
                }
            }
            return FirstDate;
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
            return FirstDate;

        }

        public static dynamic GetPageContent_All(bool WriteToFile)
        {

            if (WriteToFile)
            {
                var DaysPassed = DateTime.Now - FirstDate;
                DateTime currentDate;

                for (int i = 0; i < DaysPassed.Days; i++)
                {
                    currentDate = FirstDate.AddDays(i);
                    if (!File.Exists(GetFileName(GetLink_Custom(currentDate))))
                    {
                        //File.WriteAllTextAsync(currentDateString, GetPageContent_Single(GetLink_Custom(FirstDate.AddDays(i)),false));
                        GetPageContent_Single(GetLink_Custom(currentDate), true,true);
                        
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
                    PageContents.Add(GetPageContent_Single(GetLink_Custom(FirstDate.AddDays(i)),false,false));
                    Console.WriteLine(i);
                }

                return PageContents;

            }
            
 
        }

        public static string GetFileName(DateTime date)
        {
            
            return $"{date.Year}-{date.Month.ToString("d2")}-{date.Day.ToString("d2")}.txt";

        }
        public static string GetFileName(string url)
        {

            var temp = url.Replace(@"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/", "").Replace(".csv", "").Split("-"); ;
            

            return $"{temp[2]}-{temp[0]}-{temp[1]}.txt";
        }

        public static string GetPageContent_Single(string url,bool writefile,bool categorize)
        {
            var httpClient = new HttpClient();
            var result = httpClient.GetStringAsync(url).Result;
            
            
            if (writefile)
            {
                File.WriteAllText(GetFileName(url),result,Encoding.UTF8);
            }
            if (categorize)
            {
                Categorize_Single(File.ReadAllLines(GetFileName(url)));
            }

            return result;
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

        public static IEnumerable<string> ReadCountry(string country)
        {
            return File.ReadAllLines($"{country}.txt",Encoding.UTF8);
        }


    }

    class Statistics
    {
        private IEnumerable<string> CountryData;



        public Statistics()
        {
            CountryData = WebScraper.ReadCountry("Hungary");

            var result = WebScraper.Parse(CountryData);
            var dec31 = result.First(x => x.Last_Update == new DateTime(2021, 1, 1));
            var jan1 = result.First(x => x.Last_Update == new DateTime(2021, 1, 2));
            var sevenD = SevenDayAverage_Cases(ref result,new DateTime(2021,7,5));

            Console.WriteLine($"2020.12.31 és 2021.01.01 között {jan1.Confirmed-dec31.Confirmed} ember fertőződött meg");
            Console.WriteLine($"7 days átlag: {sevenD}");
            
        }


        public static double? SevenDayAverage_Cases(ref IEnumerable<StatsModel> lista,DateTime endDate)
        {
            
            List<double?> dailyCases = new List<double?>();
            for (int i = 0; i >= -6; i--)
            {
                DateTime currentDate = endDate.AddDays(i);
                DateTime dayBeforeCurrentDate = endDate.AddDays(i - 1);

                var adat1 = lista.First(x => x.Last_Update == currentDate);
                var adat2 = lista.First(x => x.Last_Update == dayBeforeCurrentDate);

                dailyCases.Add(adat2.Confirmed-adat2.Confirmed);
            }
            return dailyCases.Average();
            // U U U U U U U
        }

        public static double? SevenDayAverage_Deaths(ref IEnumerable<StatsModel> lista, DateTime endDate)
        {

            List<double?> dailyCases = new List<double?>();
            for (int i = 0; i >= -6; i--)
            {
                DateTime currentDate = endDate.AddDays(i);
                DateTime dayBeforeCurrentDate = endDate.AddDays(i - 1);

                var adat1 = lista.First(x => x.Last_Update == currentDate);
                var adat2 = lista.First(x => x.Last_Update == dayBeforeCurrentDate);

                dailyCases.Add(adat2.Deaths - adat2.Deaths);
            }
            return dailyCases.Average();
            // U U U U U U U
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
