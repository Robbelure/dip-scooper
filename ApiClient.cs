using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DipScooper
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        //private readonly string _apiKey = "4N6LJVG58HJN7M7G";  // Din API-nøkkel
        private readonly string _apiKey = "IUD16LLDLEU871KJ";
        private readonly string _baseUrl = "https://www.alphavantage.co/query";

        public ApiClient()
        {
            _client = new HttpClient(); 
        }

        // Method to fetch daily time series data
        public async Task<string> GetDailyTimeSeriesAsync(string symbol, string outputsize = "compact")
        {
            var url = $"{_baseUrl}?function=TIME_SERIES_DAILY&symbol={symbol}&outputsize={outputsize}&apikey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching stock data: {e.Message}");
                return null;
            }
        }

        public async Task<string> GetTimeSeriesAsync(string symbol, string outputsize, string frequency)
        {
            string function;
            switch (frequency.ToLower())
            {
                case "weekly":
                    function = "TIME_SERIES_WEEKLY";
                    break;
                case "monthly":
                    function = "TIME_SERIES_MONTHLY";
                    break;
                case "daily":
                default:
                    function = "TIME_SERIES_DAILY";
                    break;
            }

            var url = $"{_baseUrl}?function={function}&symbol={symbol}&outputsize={outputsize}&apikey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching stock data: {e.Message}");
                return null;
            }
        }

        // Method to fetch EPS data
        public async Task<double> GetEarningsPerShareAsync(string symbol)
        {
            var url = $"{_baseUrl}?function=EARNINGS&symbol={symbol}&apikey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Logge hele JSON-responsen for å inspisere strukturen
                Debug.WriteLine("JSON response: ");
                Debug.WriteLine(responseBody);

                using (var jsonDocument = JsonDocument.Parse(responseBody))
                {
                    var root = jsonDocument.RootElement.GetProperty("quarterlyEarnings");

                    // Logge strukturen til 'quarterlyEarnings'
                    Debug.WriteLine("quarterlyEarnings: ");
                    Debug.WriteLine(root.ToString());

                    if (root.GetArrayLength() == 0)
                    {
                        Debug.WriteLine("No quarterly earnings data found.");
                        return 0.0;
                    }

                    var latestEpsElement = root[0].GetProperty("reportedEPS");

                    // Logge typen av 'reportedEPS'
                    Debug.WriteLine("reportedEPS Type: ");
                    Debug.WriteLine(latestEpsElement.ValueKind.ToString());

                    // Håndtere hvis 'reportedEPS' er en streng eller et tall
                    if (latestEpsElement.ValueKind == JsonValueKind.String)
                    {
                        var latestEpsString = latestEpsElement.GetString();
                        Debug.WriteLine("reportedEPS (String): ");
                        Debug.WriteLine(latestEpsString);

                        if (double.TryParse(latestEpsString, NumberStyles.Any, CultureInfo.InvariantCulture, out double latestEps))
                        {
                            return latestEps;
                        }
                        else
                        {
                            Debug.WriteLine("Error parsing EPS value.");
                            return 0.0;
                        }
                    }
                    else if (latestEpsElement.ValueKind == JsonValueKind.Number)
                    {
                        var latestEps = latestEpsElement.GetDouble();
                        Debug.WriteLine("reportedEPS (Number): ");
                        Debug.WriteLine(latestEps.ToString(CultureInfo.InvariantCulture));
                        return latestEps;
                    }
                    else
                    {
                        Debug.WriteLine("Unexpected EPS value type.");
                        return 0.0;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching EPS data: {e.Message}");
                return 0.0;
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine($"InvalidOperationException occurred: {e.Message}");
                Debug.WriteLine($"Stack Trace: {e.StackTrace}");
                return 0.0;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An unexpected error occurred: {e.Message}");
                Debug.WriteLine($"Stack Trace: {e.StackTrace}");
                return 0.0;
            }
        }

        // Method to fetch BVPS data
        public async Task<double> GetBookValuePerShareAsync(string symbol)
        {
            var url = $"{_baseUrl}?function=BALANCE_SHEET&symbol={symbol}&apikey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                using (var jsonDocument = JsonDocument.Parse(responseBody))
                {
                    var root = jsonDocument.RootElement.GetProperty("annualReports");
                    var latestReport = root[0];
                    double totalEquity = double.Parse(latestReport.GetProperty("totalShareholderEquity").GetString(), CultureInfo.InvariantCulture);
                    double sharesOutstanding = double.Parse(latestReport.GetProperty("commonStockSharesOutstanding").GetString(), CultureInfo.InvariantCulture);
                    return totalEquity / sharesOutstanding;
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching BVPS data: {e.Message}");
                return 0.0;
            }
        }
    }
}
