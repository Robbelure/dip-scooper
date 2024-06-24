﻿using System.Diagnostics;
using System.Text.Json;
using DipScooper.Models.APIModels;

namespace DipScooper.DAL.API
{
    /// <summary>
    /// Klasse for å håndtere API-kall til Polygon API for å hente finansielle data.
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly string _apiKey = "tV1cMGsjpXHTjbTpyLHJ_45W2ucj_eSF";  // Polygon API-nøkkel
        private readonly string _baseUrl = "https://api.polygon.io";

        public ApiClient()
        {
            _client = new HttpClient();
        }

        /// <summary>
        /// Henter tidsserie data for en gitt aksje innenfor et spesifisert datointervall.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <param name="startDate">Startdato for tidsintervallet.</param>
        /// <param name="endDate">Sluttdato for tidsintervallet.</param>
        /// <returns>JSON data som en streng.</returns>
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
        /// Henter den siste markedsprisen for en gitt aksje.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <returns>Den siste markedsprisen.</returns>
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
        /// Henter bokført verdi per aksje for en gitt aksje.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <returns>Bokført verdi per aksje.</returns>
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
        /// Henter fortjeneste per aksje (EPS) for en gitt aksje.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <param name="limit">Antall perioder å hente data for.</param>
        /// <param name="timeframe">Tidsramme (annual, quarterly, etc.).</param>
        /// <returns>Liste over EPS verdier.</returns>
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
        /// Henter den siste fortjenesten per aksje (EPS) for en gitt aksje.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <returns>Den siste EPS verdien.</returns>
        public async Task<double> GetEarningsPerShareAsync(string symbol)
        {
            var epsValues = await GetEPSAsync(symbol, 1, "annual");
            return epsValues != null && epsValues.Count > 0 ? epsValues[0] : 0.0;
        }

        /// <summary>
        /// Henter fortjeneste per aksje (EPS) for de siste fire kvartalene.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <returns>Liste over EPS verdier for de siste fire kvartalene.</returns>
        public async Task<List<double>> GetTrailingEPSAsync(string symbol)
        {
            return await GetEPSAsync(symbol, 4, "quarterly");
        }

        /// <summary>
        /// Henter kontantstrøm data for en gitt aksje.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <returns>Liste over kontantstrømresponsobjekter.</returns>
        public async Task<List<CashFlowResponse>> GetCashFlowsAsync(string symbol)
        {
            var url = $"{_baseUrl}/vX/reference/financials?ticker={symbol}&limit=5&timeframe=annual&apiKey={_apiKey}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Log JSON response for debugging
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
        /// Henter det siste utbyttet for en gitt aksje.
        /// </summary>
        /// <param name="symbol">Aksjesymbol.</param>
        /// <returns>Det siste utbyttebeløpet.</returns>
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