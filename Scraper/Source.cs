using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Scraper
{
    public interface Source
    {
        public const string Deaths = @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_deaths_global.csv";
        public const string Confirmed = @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";
        public const string Recovered = @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_recovered_global.csv";

       
    }

    public interface Category
    {
        public const string Deaths = "Deaths";
        public const string Recovered = "Recovered";
        public const string Confirmed = "Confirmed";
    }

    public interface DateHelper
    {
        private const string StartDateString = "1/22/20"; // 2020-01-22

        public static DateTime StartDate()
        {
            return DateTime.ParseExact(StartDateString, "M/dd/yy",CultureInfo.InvariantCulture);
        }

        public static DateTime DateParser(string datestring)
        {
            return DateTime.ParseExact(datestring, "M/d/yy", CultureInfo.InvariantCulture);
        }

    }

    public interface FileManipulation
    {
        public static void WriteFile(string filename, string content, bool append)
        {
            StreamWriter sw = new StreamWriter($"/Users/mac/Projects/Scraper/Scraper/AppData/{filename}", append, System.Text.Encoding.UTF8);
            sw.WriteLine(content);
            sw.Close();
        }

        public static void WriteFile(string filename, IEnumerable<string> content, bool append)
        {
            StreamWriter sw = new StreamWriter($"/Users/mac/Projects/Scraper/Scraper/AppData/{filename}", append, System.Text.Encoding.UTF8);
            sw.WriteLine(content);
            sw.Close(); 
        }

        public static IEnumerable<string> ReadFileEnumerable(string filename)
        {
            
            return File.ReadAllLines($"/Users/mac/Projects/Scraper/Scraper/AppData/{filename}");
        }

        public static string ReadFileString(string filename)
        {

            return File.ReadAllText($"/Users/mac/Projects/Scraper/Scraper/AppData/{filename}");
        }

        public static bool FileExists(string filename)
        {
            if (File.Exists($"/Users/mac/Projects/Scraper/Scraper/AppData/{filename}"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string FileNameBuilder(string Country, string Province, string Category)
        {
            return Province == "" ? $"{Country},{Category}.json" : $"{Country},{Province},{Category}.json" ;
        }
    }

    public interface Countries
    {
        public const string Afghanistan = "Afghanistan";
        public const string Albania = "Albania";
        public const string Algeria = "Algeria";
        public const string Andorra = "Andorra";
        public const string Angola = "Angola";
        public const string Antarctica = "Antarctica";
        public const string Antigua_and_Barbuda = "Antigua and Barbuda";
        public const string Argentina = "Argentina";
        public const string Armenia = "Armenia";
        public const string Australia = "Australia";
        public const string Austria = "Austria";
        public const string Azerbaijan = "Azerbaijan";
        public const string Bahamas = "Bahamas";
        public const string Bahrain = "Bahrain";
        public const string Bangladesh = "Bangladesh";
        public const string Barbados = "Barbados";
        public const string Belarus = "Belarus";
        public const string Belgium = "Belgium";
        public const string Belize = "Belize";
        public const string Benin = "Benin";
        public const string Bhutan = "Bhutan";
        public const string Bolivia = "Bolivia";
        public const string Bosnia_and_Herzegovina = "Bosnia and Herzegovina";
        public const string Botswana = "Botswana";
        public const string Brazil = "Brazil";
        public const string Brunei = "Brunei";
        public const string Bulgaria = "Bulgaria";
        public const string Burkina_Faso = "Burkina Faso";
        public const string Burma = "Burma";
        public const string Burundi = "Burundi";
        public const string Cabo_Verde = "Cabo Verde";
        public const string Cambodia = "Cambodia";
        public const string Cameroon = "Cameroon";
        public const string Canada = "Canada";
        public const string Central_African_Republic = "Central African Republic";
        public const string Chad = "Chad";
        public const string Chile = "Chile";
        public const string China = "China";
        public const string Colombia = "Colombia";
        public const string Comoros = "Comoros";
        public const string Congo_Brazzaville = "Congo (Brazzaville)";
        public const string Congo_Kinshasa = "Congo (Kinshasa)";
        public const string Costa_Rica = "Costa Rica";
        public const string Cote_d_Ivoire = "Cote d'Ivoire";
        public const string Croatia = "Croatia";
        public const string Cuba = "Cuba";
        public const string Cyprus = "Cyprus";
        public const string Czechia = "Czechia";
        public const string Denmark = "Denmark";
        public const string Diamond_Princess = "Diamond Princess";
        public const string Djibouti = "Djibouti";
        public const string Dominica = "Dominica";
        public const string Dominican_Republic = "Dominican Republic";
        public const string Ecuador = "Ecuador";
        public const string Egypt = "Egypt";
        public const string El_Salvador = "El Salvador";
        public const string Equatorial_Guinea = "Equatorial Guinea";
        public const string Eritrea = "Eritrea";
        public const string Estonia = "Estonia";
        public const string Eswatini = "Eswatini";
        public const string Ethiopia = "Ethiopia";
        public const string Fiji = "Fiji";
        public const string Finland = "Finland";
        public const string France = "France";
        public const string Gabon = "Gabon";
        public const string Gambia = "Gambia";
        public const string Georgia = "Georgia";
        public const string Germany = "Germany";
        public const string Ghana = "Ghana";
        public const string Greece = "Greece";
        public const string Grenada = "Grenada";
        public const string Guatemala = "Guatemala";
        public const string Guinea = "Guinea";
        public const string Guinea_Bissau = "Guinea-Bissau";
        public const string Guyana = "Guyana";
        public const string Haiti = "Haiti";
        public const string Holy_See = "Holy See";
        public const string Honduras = "Honduras";
        public const string Hungary = "Hungary";
        public const string Iceland = "Iceland";
        public const string India = "India";
        public const string Indonesia = "Indonesia";
        public const string Iran = "Iran";
        public const string Iraq = "Iraq";
        public const string Ireland = "Ireland";
        public const string Israel = "Israel";
        public const string Italy = "Italy";
        public const string Jamaica = "Jamaica";
        public const string Japan = "Japan";
        public const string Jordan = "Jordan";
        public const string Kazakhstan = "Kazakhstan";
        public const string Kenya = "Kenya";
        public const string Kiribati = "Kiribati";
        public const string Korea_South = "Korea South";
        public const string Kosovo = "Kosovo";
        public const string Kuwait = "Kuwait";
        public const string Kyrgyzstan = "Kyrgyzstan";
        public const string Laos = "Laos";
        public const string Latvia = "Latvia";
        public const string Lebanon = "Lebanon";
        public const string Lesotho = "Lesotho";
        public const string Liberia = "Liberia";
        public const string Libya = "Libya";
        public const string Liechtenstein = "Liechtenstein";
        public const string Lithuania = "Lithuania";
        public const string Luxembourg = "Luxembourg";
        public const string MS_Zaandam = "MS Zaandam";
        public const string Madagascar = "Madagascar";
        public const string Malawi = "Malawi";
        public const string Malaysia = "Malaysia";
        public const string Maldives = "Maldives";
        public const string Mali = "Mali";
        public const string Malta = "Malta";
        public const string Marshall_Islands = "Marshall Islands";
        public const string Mauritania = "Mauritania";
        public const string Mauritius = "Mauritius";
        public const string Mexico = "Mexico";
        public const string Micronesia = "Micronesia";
        public const string Moldova = "Moldova";
        public const string Monaco = "Monaco";
        public const string Mongolia = "Mongolia";
        public const string Montenegro = "Montenegro";
        public const string Morocco = "Morocco";
        public const string Mozambique = "Mozambique";
        public const string Namibia = "Namibia";
        public const string Nepal = "Nepal";
        public const string Netherlands = "Netherlands";
        public const string New_Zealand = "New Zealand";
        public const string Nicaragua = "Nicaragua";
        public const string Niger = "Niger";
        public const string Nigeria = "Nigeria";
        public const string North_Macedonia = "North Macedonia";
        public const string Norway = "Norway";
        public const string Oman = "Oman";
        public const string Pakistan = "Pakistan";
        public const string Palau = "Palau";
        public const string Panama = "Panama";
        public const string Papua_New_Guinea = "Papua New Guinea";
        public const string Paraguay = "Paraguay";
        public const string Peru = "Peru";
        public const string Philippines = "Philippines";
        public const string Poland = "Poland";
        public const string Portugal = "Portugal";
        public const string Qatar = "Qatar";
        public const string Romania = "Romania";
        public const string Russia = "Russia";
        public const string Rwanda = "Rwanda";
        public const string Saint_Kitts_and_Nevis = "Saint Kitts and Nevis";
        public const string Saint_Lucia = "Saint Lucia";
        public const string Saint_Vincent_and_the_Grenadines = "Saint Vincent and the Grenadines";
        public const string Samoa = "Samoa";
        public const string San_Marino = "San Marino";
        public const string Sao_Tome_and_Principe = "Sao Tome and Principe";
        public const string Saudi_Arabia = "Saudi Arabia";
        public const string Senegal = "Senegal";
        public const string Serbia = "Serbia";
        public const string Seychelles = "Seychelles";
        public const string Sierra_Leone = "Sierra Leone";
        public const string Singapore = "Singapore";
        public const string Slovakia = "Slovakia";
        public const string Slovenia = "Slovenia";
        public const string Solomon_Islands = "Solomon Islands";
        public const string Somalia = "Somalia";
        public const string South_Africa = "South Africa";
        public const string South_Sudan = "South Sudan";
        public const string Spain = "Spain";
        public const string Sri_Lanka = "Sri Lanka";
        public const string Sudan = "Sudan";
        public const string Summer_Olympics_2020 = "Summer Olympics 2020";
        public const string Suriname = "Suriname";
        public const string Sweden = "Sweden";
        public const string Switzerland = "Switzerland";
        public const string Syria = "Syria";
        public const string Taiwan = "Taiwan";
        public const string Tajikistan = "Tajikistan";
        public const string Tanzania = "Tanzania";
        public const string Thailand = "Thailand";
        public const string Timor_Leste = "Timor-Leste";
        public const string Togo = "Togo";
        public const string Tonga = "Tonga";
        public const string Trinidad_and_Tobago = "Trinidad and Tobago";
        public const string Tunisia = "Tunisia";
        public const string Turkey = "Turkey";
        public const string US = "US";
        public const string Uganda = "Uganda";
        public const string Ukraine = "Ukraine";
        public const string United_Arab_Emirates = "United Arab Emirates";
        public const string United_Kingdom = "United Kingdom";
        public const string Uruguay = "Uruguay";
        public const string Uzbekistan = "Uzbekistan";
        public const string Vanuatu = "Vanuatu";
        public const string Venezuela = "Venezuela";
        public const string Vietnam = "Vietnam";
        public const string West_Bank_and_Gaza = "West Bank and Gaza";
        public const string Winter_Olympics_2022 = "Winter Olympics 2022";
        public const string Yemen = "Yemen";
        public const string Zambia = "Zambia";
        public const string Zimbabwe = "Zimbabwe";

    }
}
