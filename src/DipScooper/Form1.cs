using System.Data;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Globalization;
using System.Diagnostics;

namespace DipScooper
{
    public partial class Form1 : Form
    {
        private StockCalculator stockCalculator;
        private ApiClient apiClient;
        public Form1()
        {
            InitializeComponent();
            stockCalculator = new StockCalculator();
            apiClient = new ApiClient();
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Searching...";
            lblStatus.ForeColor = System.Drawing.Color.Blue;
            string symbol = textBoxSearch.Text.Trim();
            string frequency = comboBoxFrequency.SelectedItem.ToString().ToLower();

            if (string.IsNullOrEmpty(symbol))
            {
                lblStatus.Text = "Please enter a valid stock symbol.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                StartStatusTimer();
                return;
            }

            ApiClient apiClient = new ApiClient();
            string jsonData = await apiClient.GetTimeSeriesAsync(symbol, "compact", frequency);

            if (string.IsNullOrEmpty(jsonData))
            {
                lblStatus.Text = "Failed to retrieve data or symbol not found.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                StartStatusTimer();
                return;
            }

            lblStatus.Text = "Data loaded successfully.";
            lblStatus.ForeColor = System.Drawing.Color.Green;
            StartStatusTimer();

            UpdateDataGridView(jsonData);
        }

        private void UpdateDataGridView(string jsonData)
        {
            try
            {
                Debug.WriteLine(jsonData);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                using (var jsonDocument = JsonDocument.Parse(jsonData))
                {
                    string timeSeriesKey = "";
                    switch (comboBoxFrequency.SelectedItem.ToString().ToLower())
                    {
                        case "weekly":
                            timeSeriesKey = "Weekly Time Series";
                            break;
                        case "monthly":
                            timeSeriesKey = "Monthly Time Series";
                            break;
                        case "daily":
                        default:
                            timeSeriesKey = "Time Series (Daily)"; // Juster dette til korrekt API-nøkkel hvis nødvendig
                            break;
                    }

                    var root = jsonDocument.RootElement.GetProperty(timeSeriesKey);
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("Date", typeof(string));
                    dataTable.Columns.Add("Open", typeof(double));
                    dataTable.Columns.Add("High", typeof(double));
                    dataTable.Columns.Add("Low", typeof(double));
                    dataTable.Columns.Add("Close", typeof(double));
                    dataTable.Columns.Add("Volume", typeof(long));

                    foreach (var element in root.EnumerateObject())
                    {
                        var row = dataTable.NewRow();
                        row["Date"] = element.Name;
                        row["Open"] = double.Parse(element.Value.GetProperty("1. open").GetString(), CultureInfo.InvariantCulture);
                        row["High"] = double.Parse(element.Value.GetProperty("2. high").GetString(), CultureInfo.InvariantCulture);
                        row["Low"] = double.Parse(element.Value.GetProperty("3. low").GetString(), CultureInfo.InvariantCulture);
                        row["Close"] = double.Parse(element.Value.GetProperty("4. close").GetString(), CultureInfo.InvariantCulture);
                        row["Volume"] = long.Parse(element.Value.GetProperty("5. volume").GetString(), CultureInfo.InvariantCulture);
                        dataTable.Rows.Add(row);
                    }

                    dataGridView_stocks.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to parse JSON: " + jsonData); // Viser de rå dataene som førte til feilen
                lblStatus.Text = "Error parsing data: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }



        // timer for lblStatus
        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            lblStatus.Text = string.Empty;
            TimerStatus.Stop();
        }

        private void StartStatusTimer()
        {
            TimerStatus.Start();
        }



        private async void btnCalculate_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Calculating...";
            lblStatus.ForeColor = System.Drawing.Color.Blue;

            string symbol = textBoxSearch.Text.Trim();
            if (string.IsNullOrEmpty(symbol))
            {
                DisplayError("Please enter a valid stock symbol.");
                return;
            }

            try
            {
                string frequency = comboBoxFrequency.SelectedItem.ToString().ToLower();
                Debug.WriteLine($"Calculating P/E Ratio for {frequency}");

                if (checkBoxPERatio.Checked)
                {
                    double marketPrice = await GetLatestMarketPriceAsync(symbol);
                    Debug.WriteLine($"Market price for {frequency}: {marketPrice}");

                    double earningsPerShare = await apiClient.GetEarningsPerShareAsync(symbol);
                    double peRatio = stockCalculator.CalculatePERatio(marketPrice, earningsPerShare);

                    lblStatus.Text = "P/E Ratio calculated successfully.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    DisplayCalculationResult("P/E Ratio", peRatio);
                }

                if (checkBoxPBRatio.Checked)
                {
                    double marketPrice = await GetLatestMarketPriceAsync(symbol);
                    double bookValuePerShare = await apiClient.GetBookValuePerShareAsync(symbol);
                    double pbRatio = stockCalculator.CalculatePBRatio(marketPrice, bookValuePerShare);

                    lblStatus.Text = "P/B Ratio calculated successfully.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    DisplayCalculationResult("P/B Ratio", pbRatio);
                }

                // Implementer flere beregninger her for DCF og Dividend Discount Model
            }
            catch (Exception ex)
            {
                DisplayError($"Error calculating: {ex.Message}");
                Debug.WriteLine($"Error calculating: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            StartStatusTimer();
        }

        private async Task<double> GetLatestMarketPriceAsync(string symbol)
        {
            string frequency = comboBoxFrequency.SelectedItem.ToString().ToLower();
            string jsonData = await apiClient.GetTimeSeriesAsync(symbol, "compact", frequency);
            Debug.WriteLine($"Fetched JSON data for {frequency}: {jsonData}");
            using (var jsonDocument = JsonDocument.Parse(jsonData))
            {
                string timeSeriesKey = "";
                switch (frequency)
                {
                    case "weekly":
                        timeSeriesKey = "Weekly Time Series";
                        break;
                    case "monthly":
                        timeSeriesKey = "Monthly Time Series";
                        break;
                    case "daily":
                    default:
                        timeSeriesKey = "Time Series (Daily)";
                        break;
                }

                var root = jsonDocument.RootElement.GetProperty(timeSeriesKey);
                var latestData = root.EnumerateObject().First().Value;
                var closeElement = latestData.GetProperty("4. close");

                if (closeElement.ValueKind == JsonValueKind.String && double.TryParse(closeElement.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double closeValue))
                {
                    Debug.WriteLine($"Latest market price for {frequency}: {closeValue}");
                    return closeValue;
                }
                else if (closeElement.ValueKind == JsonValueKind.Number)
                {
                    double closeValueNum = closeElement.GetDouble();
                    Debug.WriteLine($"Latest market price for {frequency}: {closeValueNum}");
                    return closeValueNum;
                }
                else
                {
                    Debug.WriteLine("Unexpected close value type.");
                    return 0.0;
                }
            }
        }

        /*
        private async Task<double> GetLatestMarketPriceAsync(string symbol)
        {
            string jsonData = await apiClient.GetDailyTimeSeriesAsync(symbol);
            using (var jsonDocument = JsonDocument.Parse(jsonData))
            {
                var root = jsonDocument.RootElement.GetProperty("Time Series (Daily)");
                var latestData = root.EnumerateObject().First().Value;

                // Logge tilgjengelige nøkler i JSON-objektet
                foreach (var property in latestData.EnumerateObject())
                {
                    Debug.WriteLine($"Property: {property.Name} = {property.Value}");
                }

                if (latestData.TryGetProperty("4. close", out JsonElement closeElement))
                {
                    // Logge typen av '4. close'
                    Debug.WriteLine("4. close Type: ");
                    Debug.WriteLine(closeElement.ValueKind.ToString());

                    if (closeElement.ValueKind == JsonValueKind.String)
                    {
                        var closeString = closeElement.GetString();
                        Debug.WriteLine("4. close (String): ");
                        Debug.WriteLine(closeString);

                        if (double.TryParse(closeString, NumberStyles.Any, CultureInfo.InvariantCulture, out double closeValue))
                        {
                            return closeValue;
                        }
                        else
                        {
                            Debug.WriteLine("Error parsing close value.");
                            return 0.0;
                        }
                    }
                    else if (closeElement.ValueKind == JsonValueKind.Number)
                    {
                        var closeValue = closeElement.GetDouble();
                        Debug.WriteLine("4. close (Number): ");
                        Debug.WriteLine(closeValue.ToString(CultureInfo.InvariantCulture));
                        return closeValue;
                    }
                    else
                    {
                        Debug.WriteLine("Unexpected close value type.");
                        return 0.0;
                    }
                }
                else
                {
                    Debug.WriteLine("Key '4. close' not found.");
                    return 0.0;
                }
            }
        }
        */

        private void DisplayCalculationResult(string calculationName, double result)
        {
            var dataTable = dataGridView_analyze.DataSource as DataTable;
            if (dataTable == null)
            {
                dataTable = new DataTable();
                dataTable.Columns.Add("Calculation", typeof(string));
                dataTable.Columns.Add("Result", typeof(double));
                dataGridView_analyze.DataSource = dataTable;
            }

            var row = dataTable.NewRow();
            row["Calculation"] = calculationName;
            row["Result"] = result;
            dataTable.Rows.Add(row);
        }

        /*
        private void DisplayCalculationResult(string calculationName, double result)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Calculation", typeof(string));
            dataTable.Columns.Add("Result", typeof(double));

            var row = dataTable.NewRow();
            row["Calculation"] = calculationName;
            row["Result"] = result;
            dataTable.Rows.Add(row);

            dataGridView_analyze.DataSource = dataTable;
        }
        */

        private void DisplayError(string message)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = System.Drawing.Color.Red;

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(lblStatus, message); // Viser hele meldingen som et verktøytips

            //StartStatusTimer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxFrequency.SelectedIndex = 0;
        }
    }
}