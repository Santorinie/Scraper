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
using Newtonsoft.Json;

namespace Scraper
{

    public class WebScraper
    {
        public List<CountryModel> _cache { get; set; } = new();

        private HttpClient client = new HttpClient();

        private string ConfirmedRAW;
        private string DeathsRAW;
        private string RecoveredRAW;

        public List<Tuple<string,string>> CountryStatePairs;

        private static readonly WebScraper instance = new WebScraper();

        private WebScraper()
        {
            Init();
            Console.WriteLine("Webscraper init done!");

        }

        // Return a singleton WebScraper instance

        public static WebScraper GetInstance()
        {
            return instance;
        }

        // Initializes and Fetches data

        private void Init()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (GetDataSheets())
            {
                var result = ParseFile(ConfirmedRAW, nameof(ConfirmedRAW));

                this.CountryStatePairs = GetCountries(ref result);

                if (IsUpdateAvailable())
                {
                    WriteDataSheets();
                    
                    WriteData(result);
                    WriteData(ParseFile(DeathsRAW, nameof(DeathsRAW)));
                    WriteData(ParseFile(RecoveredRAW, nameof(RecoveredRAW)));
                }


            }
            else
            {
                Console.WriteLine("Couldn't Fetch data");
            }



            sw.Stop();
            Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
        }

        // Downloads Deaths, Recoveries, Confirmed unified Datasheets from github.
        // Returns with a boolean that indicates the result of the operation.

        public bool GetDataSheets()
        {


            try
            {
                var Confirmed = client.GetStringAsync(Source.Confirmed).Result;
                var Deaths = client.GetStringAsync(Source.Deaths).Result;
                var Recovered = client.GetStringAsync(Source.Recovered).Result;

                ConfirmedRAW = Confirmed;
                DeathsRAW = Deaths;
                RecoveredRAW = Recovered;

                return true;
            }
            catch (Exception)
            {
                Debug.WriteLine("Data could not be read at this time.");
                return false;
            }


        }

        // Writes datasheets

        private void WriteDataSheets()
        {
            FileManipulation.WriteFile("Confirmed.txt", ConfirmedRAW, false);
            FileManipulation.WriteFile("Deaths.txt", DeathsRAW, false);
            FileManipulation.WriteFile("Recovered.txt", RecoveredRAW, false);
        }

        // Reads the given file and generates all the CountryModels
        // Return a list with populated countrymodels

        public IEnumerable<CountryModel> ParseFile(string file, string varName)
        {
            string category = varName.Replace("RAW", "");

            //var result = category == "Recovered" ? file.Split("\r\n") : file.Split("\n"); // Possible bug in sheets
            var result = file.Split("\n");

            List <CountryModel> models = new List<CountryModel>();
            var props = result[0].Split(",").Take(4);
            var dates = result[0].Split(",").Skip(4).ToList();
            Regex regex = new("(?<=\")(.*?)(?=\")");
            foreach (var orszag in result.Skip(1))
            {
                if (orszag == "")
                {
                    continue;
                }
                string Iterator = orszag.Replace("*","");
                if (regex.IsMatch(orszag))
                {
                    var replacement = regex.Match(orszag).ToString().Replace(",","");
                    Iterator = regex.Replace(orszag, replacement).Replace("\"","");
                }

                var temp = Iterator.Split(",");


                List<(DateTime Date, double Cases)> Data = new List<(DateTime Date, double Cases)>();

                var numbers = temp.Skip(4).ToList();

                for (int i = 0; i < numbers.Count; i++)
                {
                    Data.Add((DateHelper.DateParser(dates[i]),Convert.ToDouble(numbers[i])));
                        
                   
                }

                models.Add(new CountryModel()
                {
                    Province_State = temp[0],
                    Country = temp[1],
                    Latitude = temp[2],
                    Longitude = temp[3],
                    Category = category,
                    Data = Data
                });
                



            }
            return models;


        }

        // Converts all the countryModels into JSON and writes them with the correct Province and category label

        public void WriteData(IEnumerable<CountryModel> modelList)
        {
            foreach (var country in modelList)
            {
                string filename = country.Province_State == ""
                                        ? $"{country.Country},{country.Category}.json"
                                        : $"{country.Country},{country.Province_State},{country.Category}.json";


                var json = JsonConvert.SerializeObject(country,Formatting.Indented);

                FileManipulation.WriteFile(filename,json,false);
            }
        }

        // Gets the country names and the matching provinces/states

        public List<Tuple<string,string>> GetCountries(ref IEnumerable<CountryModel> modelList)
        {
            List<Tuple<string, string>> countriesLocal = new List<Tuple<string, string>>();
           
            foreach (var orszag in modelList)
            {
                countriesLocal.Add(new Tuple<string, string>(orszag.Country,orszag.Province_State));
            }
            return countriesLocal;
        }

        // Reads a country and converts JSON into CountryModel

        public CountryModel ReadData(string Country,string Province, string Category)
        {
            var read = FileManipulation.ReadFileString(FileManipulation.FileNameBuilder(Country,Province,Category));

            return JsonConvert.DeserializeObject<CountryModel>(read);
        }

        // Reads the country flag codes and parses them into a Dictionary

        public Dictionary<string,string> GetCountryFlags()
        {
            var result = File.ReadAllText(@"AppData/countrycodes.txt");

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        }

        // Writes out the country flag codes into a text file

        public void WriteCountryFlags()
        {
            HttpClient client = new HttpClient();

            var result = client.GetStringAsync(@"https://flagcdn.com/en/codes.json").Result;

            FileManipulation.WriteFile("countryflags.txt", result, false);

            
        }

        // Check if theres an update available

        private bool IsUpdateAvailable()
        {
           var res1 = FileManipulation.ReadFileString("Confirmed.txt");
           var res2 = FileManipulation.ReadFileString("Deaths.txt");
           var res3 = FileManipulation.ReadFileString("Recovered.txt");


            if (res1 != ConfirmedRAW)
            {
                return true;
            }
            if (res2 != DeathsRAW)
            {
                return true;
            }
            if (res3 != RecoveredRAW)
            {
                return true;
            }
            return false;
        }


        //public void testxd()
        //{
        //    StreamWriter sw = new StreamWriter("vars.txt",false,Encoding.UTF8);
        //    List<string> usedCountry = new List<string>();
        //    foreach (var item in CountryStatePairs)
        //    {
        //        if (usedCountry.Contains(item.country) == false)
        //        {
        //            sw.WriteLine($"public const string {item.country.Replace(" ","_")} = \"{item.country}\";");
        //            usedCountry.Add(item.country);

        //        }
        //    }
        //    sw.Close();
        //}


    }

    public class CountryModel
    {
        public string Province_State { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Category { get; set; }
        public List<(DateTime Date, double Cases)> Data { get; set; }

    }

    public class Statistics
    {
        private WebScraper webscraper = WebScraper.GetInstance();



        public Statistics()
        {
           // webscraper.ReadData(Countries.Hungary,"",Category.Confirmed);
            
        }

        public double Average(string Country, int days, string Province, string category)
        {
            if (days <= 0)
            {
                days = 1;
            }
            if (webscraper.CountryStatePairs.Exists(x => x.Item1 == Country && x.Item2 == Province) == false)
            {
                return 0;
            }

            CountryModel model;
            if (webscraper._cache.Exists(x => x.Country == Country && x.Province_State == Province && x.Category == category))
            {
               model = webscraper._cache.First(x => x.Country == Country && x.Province_State == Province && x.Category == category);

            }
            else
            {
                model = webscraper.ReadData(Country,Province,category);
                webscraper._cache.Add(model);
            }

            var lista = model.Data.OrderByDescending(x => x.Date).ToList();

            List<double> numbers = new();

            for (int i = 0; i < lista.Count()-1; i++)
            {
                var today = lista[i];
                var yesterday = lista[i + 1];

                numbers.Add(today.Cases - yesterday.Cases);
            }
            if (numbers.Count == 0)
            {
                return 0;
            }
            return Math.Round(numbers.Take(days).Average());
        }







    }

   


    


}
