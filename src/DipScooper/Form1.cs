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

            InitializeDateTimePicker(dateTimePickerStart);
            InitializeDateTimePicker(dateTimePickerEnd);

            // Sett placeholder-tekst
            SetPlaceholderText(textBoxSearch, "Enter ticker symbol (example: Tesla = TSLA)");

            // Event handlers for focusing and unfocusing
            textBoxSearch.GotFocus += RemovePlaceholderText;
            textBoxSearch.LostFocus += ShowPlaceholderText;

            dataGridView_stocks.AutoGenerateColumns = false;
            InitializeDataGridViewStocksColumns();
            dataGridView_analyze.AutoGenerateColumns = false;
            InitializeDataGridViewAnalyzeColumns();
        }
        private void InitializeDateTimePicker(DateTimePicker dateTimePicker)
        {
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = "'Select Date...'";
            dateTimePicker.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dateTimePicker.Tag = false;  // Indikerer at ingen dato er valgt

            dateTimePicker.DropDown += (s, e) =>
            {
                dateTimePicker.CustomFormat = "dd.MM.yyyy";
            };

            dateTimePicker.CloseUp += (s, e) =>
            {
                if (!(bool)dateTimePicker.Tag)
                {
                    dateTimePicker.CustomFormat = "'Select Date...'";
                }
            };

            dateTimePicker.ValueChanged += (s, e) =>
            {
                dateTimePicker.Tag = true;  // Angir at en dato er valgt
            };

            dateTimePicker.Leave += (s, e) =>
            {
                ActiveControl = null;
            };
        }

        private void InitializeDataGridViewStocksColumns()
        {
            dataGridView_stocks.Columns.Clear();
            dataGridView_stocks.Columns.Add("Date", "Date");
            dataGridView_stocks.Columns.Add("Open", "Open");
            dataGridView_stocks.Columns.Add("High", "High");
            dataGridView_stocks.Columns.Add("Low", "Low");
            dataGridView_stocks.Columns.Add("Close", "Close");
            dataGridView_stocks.Columns.Add("Volume", "Volume");
        }

        private void InitializeDataGridViewAnalyzeColumns()
        {
            dataGridView_analyze.Columns.Clear();
            dataGridView_analyze.Columns.Add("Calculation", "Calculation");
            dataGridView_analyze.Columns.Add("Result", "Result");
        }

        private void SetPlaceholderText(TextBox textBox, string placeholderText)
        {
            textBox.Tag = placeholderText;
            textBox.Text = placeholderText;
            textBox.ForeColor = Color.Gray;
        }

        private void RemovePlaceholderText(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == (string)textBox.Tag)
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        }

        private void ShowPlaceholderText(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = (string)textBox.Tag;
                textBox.ForeColor = Color.Gray;
            }
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

            dataGridView_analyze.Rows.Clear();

            /*
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
            */
            try
            {
                if (checkBoxPERatio.Checked)
                {
                    await stockService.CalculatePERatio(symbol, dataGridView_analyze);
                }

                if (checkBoxPBRatio.Checked)
                {
                    await stockService.CalculatePBRatio(symbol, dataGridView_analyze);
                }

                if (checkBoxDCF.Checked)
                {
                    await stockService.CalculateDCF(symbol, dataGridView_analyze, discountRate);
                }

                if (checkBoxDDM.Checked)
                {
                    await stockService.CalculateDDM(symbol, dataGridView_analyze, growthRate, discountRate);
                }
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
                if (dataGridView_stocks.Columns.Count == 0)
                {
                    dataGridView_stocks.Columns.Add("Date", "Date");
                    dataGridView_stocks.Columns.Add("Open", "Open");
                    dataGridView_stocks.Columns.Add("High", "High");
                    dataGridView_stocks.Columns.Add("Low", "Low");
                    dataGridView_stocks.Columns.Add("Close", "Close");
                    dataGridView_stocks.Columns.Add("Volume", "Volume");
                }

                dataGridView_stocks.Rows.Clear();

                foreach (var result in root.EnumerateArray())
                {
                    var row = new DataGridViewRow();
                    row.CreateCells(dataGridView_stocks);

                    long timestamp = result.GetProperty("t").GetInt64();
                    DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                    row.Cells[0].Value = date.ToString("yyyy-MM-dd");
                    row.Cells[1].Value = result.GetProperty("o").GetDouble();
                    row.Cells[2].Value = result.GetProperty("h").GetDouble();
                    row.Cells[3].Value = result.GetProperty("l").GetDouble();
                    row.Cells[4].Value = result.GetProperty("c").GetDouble();
                    row.Cells[5].Value = result.GetProperty("v").GetDouble();

                    dataGridView_stocks.Rows.Add(row);
                }

                /*
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
                */


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
            //comboBoxFrequency.SelectedIndex = 0;
        }
    }
}