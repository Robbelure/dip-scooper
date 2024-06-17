using System.Data;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Forms;

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

        private async void btnCalculate_Click(object sender, EventArgs e)
        {
            string symbol = textBoxSearch.Text.Trim();
            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Please enter a valid stock symbol.");
                return;
            }

            double discountRate = 0.1;
            double growthRate = 0.05;

            DataTable dataTable;
            if (dataGridView_analyze.DataSource == null)
            {
                dataTable = new DataTable();
                dataTable.Columns.Add("Calculation", typeof(string));
                dataTable.Columns.Add("Result", typeof(double));
            }
            else
            {
                dataTable = (DataTable)dataGridView_analyze.DataSource;
            }

            try
            {
                if (checkBoxPERatio.Checked)
                {
                    double marketPrice = await apiClient.GetLatestMarketPriceAsync(symbol);
                    double earningsPerShare = await apiClient.GetEarningsPerShareAsync(symbol);

                    if (earningsPerShare == 0)
                    {
                        MessageBox.Show("EPS data not available.");
                        return;
                    }

                    double peRatio = stockCalculator.CalculatePERatio(marketPrice, earningsPerShare);
                    DataRow rowPERatio = dataTable.NewRow();
                    rowPERatio["Calculation"] = "P/E Ratio";
                    rowPERatio["Result"] = peRatio;
                    dataTable.Rows.Add(rowPERatio);

                    // Beregning av Trailing P/E Ratio
                    List<double> trailingEPS = await apiClient.GetTrailingEPSAsync(symbol);
                    if (trailingEPS == null || trailingEPS.Count < 4)
                    {
                        MessageBox.Show("Not enough EPS data available for trailing P/E.");
                        return;
                    }
                    double totalEPS = trailingEPS.Sum();
                    double trailingPERatio = marketPrice / totalEPS;
                    DataRow rowTrailingPERatio = dataTable.NewRow();
                    rowTrailingPERatio["Calculation"] = "Trailing P/E Ratio";
                    rowTrailingPERatio["Result"] = trailingPERatio;
                    dataTable.Rows.Add(rowTrailingPERatio);
                }

                if (checkBoxPBRatio.Checked)
                {
                    double marketPrice = await apiClient.GetLatestMarketPriceAsync(symbol);
                    double bookValuePerShare = await apiClient.GetBookValuePerShareAsync(symbol);

                    if (bookValuePerShare == 0)
                    {
                        MessageBox.Show("Book value per share data not available.");
                        return;
                    }

                    double pbRatio = stockCalculator.CalculatePBRatio(marketPrice, bookValuePerShare);
                    DataRow rowPBRatio = dataTable.NewRow();
                    rowPBRatio["Calculation"] = "P/B Ratio";
                    rowPBRatio["Result"] = pbRatio;
                    dataTable.Rows.Add(rowPBRatio);
                }

                if (checkBoxDCF.Checked)
                {
                    List<double> cashFlows = await apiClient.GetCashFlowsAsync(symbol);
                    Debug.WriteLine($"Cash Flows for {symbol}: {string.Join(", ", cashFlows)}");

                    if (cashFlows == null || cashFlows.Count == 0)
                    {
                        MessageBox.Show("No cash flow data available.");
                        return;
                    }


                    double dcfValue = stockCalculator.CalculateDCF(cashFlows, discountRate);
                    DataRow rowDCF = dataTable.NewRow();
                    rowDCF["Calculation"] = "DCF Value";
                    rowDCF["Result"] = dcfValue;
                    dataTable.Rows.Add(rowDCF);
                }

                if (checkBoxDDM.Checked)
                {
                    double lastDividend = await apiClient.GetLastDividendAsync(symbol);
                    if (lastDividend == 0)
                    {
                        MessageBox.Show("Dividend data not available.");
                        return;
                    }

                    double ddmValue = stockCalculator.CalculateDDM(lastDividend, growthRate, discountRate);

                    DataRow row = dataTable.NewRow();
                    row["Calculation"] = "Dividend Discount Model Value";
                    row["Result"] = ddmValue;
                    dataTable.Rows.Add(row);
                }


                dataGridView_analyze.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error performing calculations: {ex.Message}\nStackTrace: {ex.StackTrace}");
                Debug.WriteLine($"Error performing calculations: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Searching...";
            lblStatus.ForeColor = System.Drawing.Color.Blue;
            string symbol = textBoxSearch.Text.Trim();
            string startDate = dateTimePickerStart.Value.ToString("yyyy-MM-dd");
            string endDate = dateTimePickerEnd.Value.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(symbol))
            {
                lblStatus.Text = "Please enter a valid stock symbol.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                StartStatusTimer();
                return;
            }

            string jsonData = await apiClient.GetTimeSeriesAsync(symbol, startDate, endDate);

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
                var jsonDocument = JsonDocument.Parse(jsonData);
                var root = jsonDocument.RootElement.GetProperty("results");
                var dataTable = new DataTable();
                dataTable.Columns.Add("Date", typeof(string));
                dataTable.Columns.Add("Open", typeof(double));
                dataTable.Columns.Add("High", typeof(double));
                dataTable.Columns.Add("Low", typeof(double));
                dataTable.Columns.Add("Close", typeof(double));
                dataTable.Columns.Add("Volume", typeof(long));

                foreach (var result in root.EnumerateArray())
                {
                    var row = dataTable.NewRow();
                    long timestamp = result.GetProperty("t").GetInt64();  // Hent timestamp som en long
                    DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;  // Konverter timestamp til DateTime
                    row["Date"] = date.ToString("yyyy-MM-dd");  // Formatér dato som string
                    row["Open"] = result.GetProperty("o").GetDouble();
                    row["High"] = result.GetProperty("h").GetDouble();
                    row["Low"] = result.GetProperty("l").GetDouble();
                    row["Close"] = result.GetProperty("c").GetDouble();
                    row["Volume"] = result.GetProperty("v").GetDouble();  // Antar at volum kan være et stort tall, så bruk GetDouble() hvis nødvendig
                    dataTable.Rows.Add(row);
                }

                dataGridView_stocks.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to parse JSON: " + jsonData);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxFrequency.SelectedIndex = 0;
        }
    }
}