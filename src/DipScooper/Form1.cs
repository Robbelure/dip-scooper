using System.Data;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Forms;
using DipScooper.Calculators;
using DipScooper.Services;

namespace DipScooper
{
    public partial class Form1 : Form
    {
        private ApiClient apiClient;
        private StockService stockService;

        public Form1()
        {
            InitializeComponent();
            apiClient = new ApiClient();
            stockService = new StockService();
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
                    await stockService.CalculatePERatio(symbol, dataTable);
                }

                if (checkBoxPBRatio.Checked)
                {
                    await stockService.CalculatePBRatio(symbol, dataTable);
                }

                if (checkBoxDCF.Checked)
                {
                    await stockService.CalculateDCF(symbol, dataTable, discountRate);
                }

                if (checkBoxDDM.Checked)
                {
                    await stockService.CalculateDDM(symbol, dataTable, growthRate, discountRate);
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
                    long timestamp = result.GetProperty("t").GetInt64();
                    DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                    row["Date"] = date.ToString("yyyy-MM-dd");
                    row["Open"] = result.GetProperty("o").GetDouble();
                    row["High"] = result.GetProperty("h").GetDouble();
                    row["Low"] = result.GetProperty("l").GetDouble();
                    row["Close"] = result.GetProperty("c").GetDouble();
                    row["Volume"] = result.GetProperty("v").GetDouble();
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