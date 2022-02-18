using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        public static DateTime NewFormat = new DateTime(2020,5,29); // in file - last updated

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
            GetPageContent_All(); // fetch
        }

        public static void Categorize_All()
        {            
            string filename;
            IEnumerable<string> input;
            StreamWriter sw;
            Regex regex;
            for (int i = 0; i < DaysPassed; i++)
            {
                Console.WriteLine(i);
                filename = GetFileName(FirstDate.AddDays(i));
                if (DateTime.ParseExact(filename.Replace(".txt", ""), "yyyy-MM-dd", CultureInfo.InvariantCulture) >= NewFormat)
                {


                    input = File.ReadAllLines(filename).Skip(1);

                    foreach (var sor in input)
                    {
                        regex = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}");
                        if (regex.IsMatch(sor) == false)
                        {
                            regex = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}");
                        }
                        var result = sor.Replace(regex.Match(sor).ToString(),filename.Replace(".txt",""));
                        foreach (var country in countries)
                        {
                            if (result.Contains($",{country},"))
                            {
                                sw = new StreamWriter($"{country}.txt", true, Encoding.UTF8);
                                sw.WriteLine(result);
                                sw.Close();

                            }
                        }
                    }
                }
            }

        }

        public static void Categorize_Single(IEnumerable<string> data,string filename)
        {
            StreamWriter sw;
           
            Regex regex;
            foreach (var line in data.Skip(1))
            {
                foreach (var country in countries)
                {
                    regex = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}");
                    if (regex.IsMatch(line) == false)
                    {
                        regex = new Regex(@"[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}");
                    }
                    var result = line.Replace(regex.Match(line).ToString(), filename.Replace(".txt", ""));
                    if (line.Contains($",{country},"))
                    {
                        sw = new StreamWriter($"{country}.txt",true,Encoding.UTF8);
                        sw.WriteLine(result);
                        sw.Close();
                    }
                }
            }
        }

        private static List<string> GetCountries()
        {
            
            HashSet<string> countries = new HashSet<string>();
            List<StatsModel> data = new List<StatsModel>();


            data = Parse(FirstDate.AddDays(365)).ToList();
            foreach (var item in data)
            {
                countries.Add(item.Country_Region);
            }


            return countries.ToList();

        }

        public static IEnumerable<StatsModel> Parse(DateTime date)
        {
            var inputLine1 = File.ReadAllLines(WebScraper.GetFileName(date));
            var input = inputLine1.Skip(1);
            List<StatsModel> models = new List<StatsModel>();

            foreach (var item in input)
            {

                var temp = item.Replace(", ", ";").Replace("\"", "").Split(",");

                models.Add(new StatsModel(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], temp[6], temp[7], temp[8], temp[9]
                    , temp[10], temp[11], temp[12], temp[13]));

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
                

                    models.Add(new StatsModel(temp[0], temp[1], temp[2], temp[3], temp[4], temp[5], temp[6], temp[7], temp[8], temp[9]
                        , temp[10], temp[11], temp[12], temp[13]));

                

               
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

        public static void GetPageContent_All()
        {

                var DaysPassed = DateTime.Now - FirstDate;
                DateTime currentDate;

                for (int i = 0; i < DaysPassed.Days; i++)
                {
                    currentDate = FirstDate.AddDays(i);
                    if (!File.Exists(GetFileName(GetLink_Custom(currentDate))))
                    {
                        GetPageContent_Single(GetLink_Custom(currentDate), true,true); // downloads file
                        
                    }
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
                Categorize_Single(File.ReadAllLines(GetFileName(url)),GetFileName(url));
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

            Console.WriteLine(new DateTime(2021, 3, 9).ToShortDateString());
            Console.WriteLine(NewCases(ref result, new DateTime(2021,3,9)));
            Console.WriteLine(
                SevenDayAverage_Cases(ref result, new DateTime(2021, 3, 9))
                );
            Console.WriteLine(NewDeaths(ref result, new DateTime(2021, 3, 9)));

        }


        public static double SevenDayAverage_Cases(ref IEnumerable<StatsModel> lista,DateTime endDate)
        {
            
            List<double> dailyCases = new List<double>();
            for (int i = 0; i >= -6; i--)
            {
                DateTime currentDate = endDate.AddDays(i);
                DateTime dayBeforeCurrentDate = endDate.AddDays(i - 1);

                var adat1 = lista.First(x => x.Last_Update == currentDate);
                var adat2 = lista.First(x => x.Last_Update == dayBeforeCurrentDate);

                dailyCases.Add(adat1.Confirmed-adat2.Confirmed);
            }
            return Math.Round(dailyCases.Average());
            
        }

        public static double SevenDayAverage_Deaths(ref IEnumerable<StatsModel> lista, DateTime endDate)
        {

            List<double> dailyCases = new List<double>();
            for (int i = 0; i >= -6; i--)
            {
                DateTime currentDate = endDate.AddDays(i);
                DateTime dayBeforeCurrentDate = endDate.AddDays(i - 1);

                var adat1 = lista.First(x => x.Last_Update == currentDate);
                var adat2 = lista.First(x => x.Last_Update == dayBeforeCurrentDate);

                dailyCases.Add(adat1.Deaths - adat2.Deaths);
            }
            return Math.Round(dailyCases.Average());
            
        }

        public static double NewCases(ref IEnumerable<StatsModel> lista, DateTime date)
        {
            StatsModel day = lista.First(x => x.Last_Update == date);
            StatsModel theDayBefore = lista.First(x => x.Last_Update == date.AddDays(-1));

            return day.Confirmed - theDayBefore.Confirmed;

        }

        public static double NewDeaths(ref IEnumerable<StatsModel> lista, DateTime date)
        {
            StatsModel day = lista.First(x => x.Last_Update == date);
            StatsModel theDayBefore = lista.First(x => x.Last_Update == date.AddDays(-1));

            return day.Deaths - theDayBefore.Deaths;

        }


    }

    class StatsModel
    {

        public string FIPS { get; set; }
        public string Admin2 { get; set; }
        public string Province_State { get; set; }
        public string Country_Region { get; set; }
        public DateTime? Last_Update { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Confirmed { get; set; }
        public double Deaths { get; set; }
        public double Recovered { get; set; }
        public double Active { get; set; }
        public string Combined_Key { get; set; }
        public double Incident_Rate { get; set; }
        public double Case_Fatality_Ratio { get; set; }
        


        public StatsModel(string fips,string admin2, string provincestate, string country, string lastupdate, string latitude,
            string longitude, string confirmed, string deaths, string recovered, string active, string combinedkey, string incidentrate,
            string casefatalityratio)
        {
            this.FIPS = fips;
            this.Admin2 = admin2;
            this.Province_State = provincestate;
            this.Country_Region = country;
            this.Last_Update = DateTime.TryParseExact(lastupdate, "yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out _) ? DateTime.ParseExact(lastupdate, "yyyy-MM-dd",CultureInfo.InvariantCulture) : DateTime.MinValue;
            this.Latitude = latitude != "" && double.TryParse(latitude, out _) ? Convert.ToDouble(latitude) : 0;
            this.Longitude = longitude != "" && double.TryParse(longitude, out _) ? Convert.ToDouble(longitude) : 0;
            this.Confirmed = confirmed != "" && double.TryParse(confirmed, out _) ? Convert.ToDouble(confirmed) : 0;
            this.Deaths = deaths != "" && double.TryParse(deaths, out _) ? Convert.ToDouble(deaths) : 0;
            this.Recovered = recovered != "" && double.TryParse(recovered, out _) ? Convert.ToDouble(recovered) : 0;
            this.Active = active != "" && double.TryParse(active, out _) ? Convert.ToDouble(active) : 0;
            this.Combined_Key = combinedkey;
            this.Incident_Rate = incidentrate != "" && double.TryParse(incidentrate, out _) ? Convert.ToDouble(incidentrate) : 0;
            this.Case_Fatality_Ratio = casefatalityratio != "" && double.TryParse(casefatalityratio, out _) ? Convert.ToDouble(casefatalityratio) : 0;
            
        }


    }

    
}
