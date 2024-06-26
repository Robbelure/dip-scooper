using System.Diagnostics;
using System.Text.Json;
using DipScooper.Models.APIModels;

namespace DipScooper.DAL.API
{
    // handles API calls to the Polygon API to fetch financial data
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly string _apiKey = "tV1cMGsjpXHTjbTpyLHJ_45W2ucj_eSF";  // Polygon API-key
        private readonly string _baseUrl = "https://api.polygon.io";

        public ApiClient()
        {
            _client = new HttpClient();
        }

        /// <summary>
        /// Retrieves time series data for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="startDate">The start date for the data.</param>
        /// <param name="endDate">The end date for the data.</param>
        /// <returns>The JSON response as a string, or null if an error occurs.</returns>
        public async Task<string?> GetTimeSeriesAsync(string symbol, string startDate, string endDate)
        {
            var url = $"{_baseUrl}/v2/aggs/ticker/{symbol}/range/1/day/{startDate}/{endDate}?adjusted=true&apiKey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching data: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the latest market price for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The latest market price as a double, or 0.0 if an error occurs.</returns>
        public async Task<double> GetLatestMarketPriceAsync(string symbol)
        {
            var url = $"{_baseUrl}/v2/aggs/ticker/{symbol}/prev?apiKey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("Market Price JSON response: " + responseBody);

                var jsonDocument = JsonDocument.Parse(responseBody);
                var results = jsonDocument.RootElement.GetProperty("results");

                if (results.GetArrayLength() > 0)
                {
                    var latestData = results[0];
                    double closePrice = latestData.GetProperty("c").GetDouble();

                    Debug.WriteLine($"Close Price for {symbol}: {closePrice}");

                    return closePrice;
                }
                else
                {
                    throw new Exception("No market price data found.");
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching market price: {e.Message}");
                return 0.0;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An unexpected error occurred: {e.Message}");
                return 0.0;
            }
        }

        /// <summary>
        /// Retrieves the book value per share for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The book value per share as a double, or 0.0 if an error occurs.</returns>
        public async Task<double> GetBookValuePerShareAsync(string symbol)
        {
            var url = $"{_baseUrl}/vX/reference/financials?ticker={symbol}&limit=1&timeframe=annual&apiKey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("Financials JSON response: " + responseBody);

                var jsonDocument = JsonDocument.Parse(responseBody);
                var financials = jsonDocument.RootElement.GetProperty("results");

                if (financials.GetArrayLength() > 0)
                {
                    var latestFinancial = financials[0];
                    var balanceSheet = latestFinancial.GetProperty("financials").GetProperty("balance_sheet");
                    double totalEquity = balanceSheet.GetProperty("equity").GetProperty("value").GetDouble();
                    var incomeStatement = latestFinancial.GetProperty("financials").GetProperty("income_statement");
                    double sharesOutstanding = incomeStatement.GetProperty("basic_average_shares").GetProperty("value").GetDouble();

                    return totalEquity / sharesOutstanding;
                }
                else
                {
                    throw new Exception("No financial data found.");
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching financial data: {e.Message}");
                return 0.0;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An unexpected error occurred: {e.Message}");
                return 0.0;
            }
        }

        /// <summary>
        /// Retrieves the earnings per share (EPS) for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <param name="limit">The number of periods to retrieve.</param>
        /// <param name="timeframe">The timeframe for the data (e.g., "annual" or "quarterly").</param>
        /// <returns>A list of EPS values, or null if an error occurs.</returns>
        public async Task<List<double>?> GetEPSAsync(string symbol, int limit = 1, string timeframe = "annual")
        {
            var url = $"{_baseUrl}/vX/reference/financials?ticker={symbol}&limit={limit}&timeframe={timeframe}&apiKey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("Financials JSON response: " + responseBody);

                var jsonDocument = JsonDocument.Parse(responseBody);
                var financials = jsonDocument.RootElement.GetProperty("results");

                List<double> epsValues = new List<double>();

                foreach (var financial in financials.EnumerateArray())
                {
                    var incomeStatement = financial.GetProperty("financials").GetProperty("income_statement");

                    if (incomeStatement.TryGetProperty("basic_earnings_per_share", out JsonElement epsElement))
                    {
                        epsValues.Add(epsElement.GetProperty("value").GetDouble());
                    }
                    else
                    {
                        throw new Exception("No EPS data found for one or more periods.");
                    }
                }

                return epsValues;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching financial data: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An unexpected error occurred: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves the earnings per share (EPS) for the latest period for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The latest EPS value as a double, or 0.0 if an error occurs.</returns>
        public async Task<double> GetEarningsPerShareAsync(string symbol)
        {
            var epsValues = await GetEPSAsync(symbol, 1, "annual");
            return epsValues != null && epsValues.Count > 0 ? epsValues[0] : 0.0;
        }

        /// <summary>
        /// Retrieves the trailing earnings per share (EPS) for the last four quarters for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A list of EPS values for the last four quarters.</returns>
        public async Task<List<double>> GetTrailingEPSAsync(string symbol)
        {
            return await GetEPSAsync(symbol, 4, "quarterly");
        }

        /// <summary>
        /// Retrieves the cash flow data for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>A list of CashFlowResponse objects containing net cash flow data.</returns>
        public async Task<List<CashFlowResponse>> GetCashFlowsAsync(string symbol)
        {
            var url = $"{_baseUrl}/vX/reference/financials?ticker={symbol}&limit=5&timeframe=annual&apiKey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("Financials JSON response: " + responseBody);

                var jsonDocument = JsonDocument.Parse(responseBody);
                var financials = jsonDocument.RootElement.GetProperty("results");

                if (financials.GetArrayLength() > 0)
                {
                    List<CashFlowResponse> cashFlows = new List<CashFlowResponse>();
                    foreach (var financial in financials.EnumerateArray())
                    {
                        var cashFlowStatement = financial.GetProperty("financials").GetProperty("cash_flow_statement");
                        double netCashFlow = cashFlowStatement.GetProperty("net_cash_flow").GetProperty("value").GetDouble();
                        DateTime date = financial.GetProperty("start_date").GetDateTime();
                        cashFlows.Add(new CashFlowResponse { NetCashFlow = netCashFlow, Date = date });
                    }
                    return cashFlows;
                }
                else
                {
                    throw new Exception("No financial data found.");
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching cash flow data: {e.Message}");
                return new List<CashFlowResponse>();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An unexpected error occurred: {e.Message}");
                return new List<CashFlowResponse>();
            }
        }

        /// <summary>
        /// Retrieves the latest dividend amount for a given stock symbol.
        /// </summary>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The latest dividend amount as a double, or 0.0 if an error occurs.</returns>
        public async Task<double> GetLastDividendAsync(string symbol)
        {
            var url = $"{_baseUrl}/v2/reference/dividends/{symbol}?apiKey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("Dividends JSON response: " + responseBody);

                var jsonDocument = JsonDocument.Parse(responseBody);
                var results = jsonDocument.RootElement.GetProperty("results");

                if (results.GetArrayLength() > 0)
                {
                    var latestDividend = results[0];
                    return latestDividend.GetProperty("amount").GetDouble();
                }
                else
                {
                    throw new Exception("No dividend data found.");
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"An error occurred while fetching dividend data: {e.Message}");
                return 0.0;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"An unexpected error occurred: {e.Message}");
                return 0.0;
            }
        }
    }
}
