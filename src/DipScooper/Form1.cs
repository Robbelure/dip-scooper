using System.Data;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Forms;
using DipScooper.Calculators;
using DipScooper.Services;
using System.ComponentModel;
using DevExpress.XtraCharts;

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

            SetPlaceholderText(textBoxSearch, "Enter ticker symbol (example: Tesla = TSLA)");
            textBoxSearch.GotFocus += RemovePlaceholderText;
            textBoxSearch.LostFocus += ShowPlaceholderText;

            dataGridView_stocks.AutoGenerateColumns = false;
            InitializeDataGridViewStocksColumns();
            dataGridView_analyze.AutoGenerateColumns = false;
            InitializeDataGridViewAnalyzeColumns();

            // Konfigurer BackgroundWorker
            backgroundWorker_search.WorkerReportsProgress = true;
            backgroundWorker_search.DoWork += new DoWorkEventHandler(backgroundWorker_search_DoWork);
            backgroundWorker_search.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_search_ProgressChanged);
            backgroundWorker_search.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_search_RunWorkerCompleted);

            InitializeChartControl();
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

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string symbol = textBoxSearch.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Please enter a valid stock symbol.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!backgroundWorker_search.IsBusy)
            {
                progressBar_search.Value = 0;
                progressBar_search.Visible = true;
                lblStatus.Text = "Searching...";
                lblStatus.ForeColor = System.Drawing.Color.Blue;
                dataGridView_stocks.Rows.Clear(); // Clear existing rows
                backgroundWorker_search.RunWorkerAsync(textBoxSearch.Text.Trim());
            }
        }

        private List<DataGridViewRow> ProcessJsonData(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                throw new ArgumentException("JSON data is null or empty.");
            }

            var jsonDocument = JsonDocument.Parse(jsonData);
            var root = jsonDocument.RootElement.GetProperty("results");
            List<DataGridViewRow> rows = new List<DataGridViewRow>();

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

                rows.Add(row);
            }

            return rows;
        }

        private void InitializeChartControl()
        {
            // Initialiser ChartControl
            chartControlStocks.Series.Clear();

            Series closeSeries = new Series("Close", ViewType.Line);
            Series volumeSeries = new Series("Volume", ViewType.Bar);
            Series rsiSeries = new Series("RSI", ViewType.Line);

            chartControlStocks.Series.Add(closeSeries);
            chartControlStocks.Series.Add(volumeSeries);
            chartControlStocks.Series.Add(rsiSeries);

            // Juster farger og utseende på seriene
            ((LineSeriesView)closeSeries.View).Color = Color.Blue;
            ((BarSeriesView)volumeSeries.View).Color = Color.Red;
            ((LineSeriesView)rsiSeries.View).Color = Color.Black;

            XYDiagram diagram = (XYDiagram)chartControlStocks.Diagram;

            // Juster Y-akse for Volume
            SecondaryAxisY secondaryAxisYVolume = new SecondaryAxisY("Volume Axis");
            diagram.SecondaryAxesY.Add(secondaryAxisYVolume);
            ((BarSeriesView)volumeSeries.View).AxisY = secondaryAxisYVolume;

            // Juster Y-akse for RSI
            SecondaryAxisY secondaryAxisYRSI = new SecondaryAxisY("RSI Axis");
            diagram.SecondaryAxesY.Add(secondaryAxisYRSI);
            ((LineSeriesView)rsiSeries.View).AxisY = secondaryAxisYRSI;

            // Sett tittel og akseetiketter
            chartControlStocks.Titles.Clear();
            ChartTitle chartTitle = new ChartTitle();
            chartTitle.Text = "Stock Data Chart";
            chartControlStocks.Titles.Add(chartTitle);

            diagram.AxisX.Title.Text = "Date";
            diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisY.Title.Text = "Price";
            diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;

            //secondaryAxisYVolume.Title.Text = "Volume";
            //secondaryAxisYVolume.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;

            //secondaryAxisYRSI.Title.Text = "RSI";
            //secondaryAxisYRSI.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;

            chartControlStocks.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
        }

        private void UpdateChartWithData()
        {
            if (dataGridView_stocks.Rows.Count == 0)
                return;

            Series closeSeries = chartControlStocks.Series["Close"];
            Series volumeSeries = chartControlStocks.Series["Volume"];
            Series rsiSeries = chartControlStocks.Series["RSI"];

            closeSeries.Points.Clear();
            volumeSeries.Points.Clear();
            rsiSeries.Points.Clear();

            List<double> closePrices = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            foreach (DataGridViewRow row in dataGridView_stocks.Rows)
            {
                if (row.Cells["Date"].Value != null)
                {
                    DateTime date = DateTime.Parse(row.Cells["Date"].Value.ToString());
                    double closePrice = Convert.ToDouble(row.Cells["Close"].Value);
                    double volume = Convert.ToDouble(row.Cells["Volume"].Value);

                    // Legg til dato og lukkepris til listene
                    dates.Add(date);
                    closePrices.Add(closePrice);

                    closeSeries.Points.Add(new SeriesPoint(date, closePrice));
                    volumeSeries.Points.Add(new SeriesPoint(date, volume));

                    closePrices.Add(closePrice);
                }
            }

            // Beregn og legg til RSI
            List<double> rsiValues = stockService.CalculateRSI(closePrices);
            for (int i = 0; i < rsiValues.Count; i++)
            {
                if (i < dates.Count)
                {
                    rsiSeries.Points.Add(new SeriesPoint(dates[i], rsiValues[i]));
                }
            }

            chartControlStocks.Refresh();
        }

        private void backgroundWorker_search_DoWork(object sender, DoWorkEventArgs e)
        {
            // Hent inn verdier fra UI-tråden
            string symbol = (string)e.Argument;
            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentException("Please enter a valid stock symbol.");
            }// vi sender symbolet som et argument
            string startDate = dateTimePickerStart.Invoke(new Func<string>(() => dateTimePickerStart.Value.ToString("yyyy-MM-dd")));
            string endDate = dateTimePickerEnd.Invoke(new Func<string>(() => dateTimePickerEnd.Value.ToString("yyyy-MM-dd")));

            try
            {
                // Asynkrone operasjoner gjøres synkront ved å bruke Task.Run og Result
                string jsonData = Task.Run(async () => await apiClient.GetTimeSeriesAsync(symbol, startDate, endDate)).Result;

                if (string.IsNullOrEmpty(jsonData))
                {
                    throw new Exception("No data retrieved from the API.");
                }

                // Prosessér dataen
                var rows = ProcessJsonData(jsonData);

                // Rapporter fremdrift
                int totalRecords = rows.Count;
                for (int i = 0; i < totalRecords; i++)
                {
                    backgroundWorker_search.ReportProgress((i + 1) * 100 / totalRecords, rows[i]);
                }

                // Lagre resultatet
                e.Result = rows;
            }
            catch (Exception ex)
            {
                // Håndter eventuelle feil
                e.Result = ex;
            }
        }

        private void backgroundWorker_search_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar_search.Value = e.ProgressPercentage;
            if (e.UserState is DataGridViewRow row)
            {
                dataGridView_stocks.Rows.Add(row);
            }
        }

        private void backgroundWorker_search_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Result is Exception)
            {
                string? errorMessage = e.Error != null ? e.Error.Message : (e.Result as Exception)?.Message;
                lblStatus.Text = errorMessage;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                var rows = e.Result as List<DataGridViewRow>;
                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var newRow = new DataGridViewRow();
                        newRow.CreateCells(dataGridView_stocks, row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value, row.Cells[5].Value);
                        dataGridView_stocks.Rows.Add(newRow);
                    }
                    lblStatus.Text = "Data loaded successfully.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;

                    UpdateChartWithData();
                }
            }

            // Fullfør progressbaren til 100 % før den blir usynlig
            progressBar_search.Value = 100;
            Task.Delay(500).ContinueWith(t =>
            {
                if (progressBar_search.InvokeRequired)
                {
                    progressBar_search.Invoke(new Action(() =>
                    {
                        progressBar_search.Visible = false;
                    }));
                }
                else
                {
                    progressBar_search.Visible = false;
                }
            });
            StartStatusTimer();
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
            progressBar_search.Visible = false;
        }



        private void UpdateDataGridView(string jsonData)
        {
            try
            {
                Debug.WriteLine(jsonData);
                var rows = ProcessJsonData(jsonData);

                dataGridView_stocks.Rows.Clear();

                foreach (var row in rows)
                {
                    dataGridView_stocks.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to parse JSON: " + jsonData);
                lblStatus.Text = "Error parsing data: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}