using System.Text.Json;
using System.Diagnostics;
using DipScooper.Services;
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

            InitializeChartControl();
        }

        /// <summary>
        /// Håndterer klikkhendelsen for beregningsknappen.
        /// Utfører finansielle beregninger basert på brukerens valg.
        /// </summary>
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

        /// <summary>
        /// Håndterer klikkhendelsen for søkeknappen.
        /// Starter en asynkron søkingsprosess for å hente aksjedata.
        /// </summary>
        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string symbol = textBoxSearch.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Please enter a valid stock symbol.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (true)
            {
                progressBar_search.Value = 0;
                progressBar_search.Visible = true;
                lblStatus.Text = "Searching...";
                lblStatus.ForeColor = System.Drawing.Color.Blue;
                dataGridView_stocks.Rows.Clear(); 
                await RunBackgroundSearchAsync(symbol); 
            }
        }

        /// <summary>
        /// Kjører bakgrunnssøk asynkront for å hente aksjedata.
        /// </summary>
        private async Task RunBackgroundSearchAsync(string symbol)
        {
            string startDate = dateTimePickerStart.Value.ToString("yyyy-MM-dd");
            string endDate = dateTimePickerEnd.Value.ToString("yyyy-MM-dd");

            try
            {
                string jsonData = await apiClient.GetTimeSeriesAsync(symbol, startDate, endDate);
                if (string.IsNullOrEmpty(jsonData))
                {
                    throw new Exception("No data retrieved from the API.");
                }

                // Prosessérer dataen
                var rows = ProcessJsonData(jsonData);

                // Oppdater UI 
                Invoke(new Action(() =>
                {
                    foreach (var row in rows)
                    {
                        dataGridView_stocks.Rows.Add(row);
                    }
                    lblStatus.Text = "Data loaded successfully.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;

                    UpdateChartWithData();
                }));
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    lblStatus.Text = $"Error: {ex.Message}";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }));
            }
            finally
            {
                Invoke(new Action(() =>
                {
                    progressBar_search.Visible = false;
                }));
            }
        }

        /// <summary>
        /// Prosesserer JSON-data mottatt fra APIet og konverterer det til rader som kan vises i dataGridView_stocks.
        /// </summary>
        /// <param name="jsonData">JSON-data mottatt fra APIet.</param>
        /// <returns>Liste over DataGridViewRow objekter.</returns>
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
            chartControlStocks.Series.Clear();

            Series closeSeries       = new Series("Close", ViewType.Line);
            Series volumeSeries      = new Series("Volume", ViewType.Bar);
            Series rsiSeries         = new Series("RSI", ViewType.Line);
            Series candlestickSeries = new Series("Price", ViewType.CandleStick); 
            Series sma50Series       = new Series("SMA50", ViewType.Line);
            Series sma200Series      = new Series("SMA200", ViewType.Line);

            chartControlStocks.Series.Add(closeSeries);
            chartControlStocks.Series.Add(volumeSeries);
            chartControlStocks.Series.Add(rsiSeries);
            chartControlStocks.Series.Add(candlestickSeries); 
            chartControlStocks.Series.Add(sma50Series);
            chartControlStocks.Series.Add(sma200Series);

            ((LineSeriesView)closeSeries.View).Color = Color.Blue;
            ((BarSeriesView)volumeSeries.View).Color = Color.Red;
            ((LineSeriesView)rsiSeries.View).Color = Color.Black;
            ((LineSeriesView)sma50Series.View).Color = Color.Yellow;
            ((LineSeriesView)sma200Series.View).Color = Color.Purple;

            CandleStickSeriesView candleStickView = (CandleStickSeriesView)candlestickSeries.View;
            candleStickView.ReductionOptions.ColorMode = ReductionColorMode.OpenToCloseValue;
            candleStickView.ReductionOptions.Level = StockLevel.Close;
            candleStickView.LineThickness = 3;  
            candleStickView.LevelLineLength = 0.7;
            candleStickView.Color = Color.Green;

            XYDiagram diagram = (XYDiagram)chartControlStocks.Diagram;

            diagram.AxisX.DateTimeScaleOptions.WorkdaysOnly = true;
            diagram.AxisX.DateTimeScaleOptions.AggregateFunction = AggregateFunction.None;
            diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Day;
            diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Day;

            diagram.AxisX.Title.Text = "Date";
            diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisY.Title.Text = "Price";
            diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;

            SecondaryAxisY secondaryAxisYVolume = new SecondaryAxisY("Volume Axis");
            secondaryAxisYVolume.WholeRange.Auto = false;
            secondaryAxisYVolume.WholeRange.SetMinMaxValues(0, 3000000000);
            diagram.SecondaryAxesY.Add(secondaryAxisYVolume);
            ((BarSeriesView)volumeSeries.View).AxisY = secondaryAxisYVolume;

            SecondaryAxisY secondaryAxisYRSI = new SecondaryAxisY("RSI Axis");
            diagram.SecondaryAxesY.Add(secondaryAxisYRSI);
            ((LineSeriesView)rsiSeries.View).AxisY = secondaryAxisYRSI;

            chartControlStocks.Titles.Clear();
            ChartTitle chartTitle = new ChartTitle();
            chartTitle.Text = "Stock Data Chart";
            chartControlStocks.Titles.Add(chartTitle);
            chartControlStocks.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;

            BarSeriesView? barView = volumeSeries.View as BarSeriesView;
            if (barView != null)
            {
                barView.BarWidth = 0.3; 
            }
        }

        private void UpdateChartWithData()
        {
            if (dataGridView_stocks.Rows.Count == 0)
                return;

            Series volumeSeries = chartControlStocks.Series["Volume"];
            Series rsiSeries = chartControlStocks.Series["RSI"];
            Series candlestickSeries = chartControlStocks.Series["Price"]; 
            
            Series sma50Series = chartControlStocks.Series["sma50"];
            if (sma50Series == null)
            {
                sma50Series = new Series("sma50", ViewType.Line);
                chartControlStocks.Series.Add(sma50Series);
            }

            Series sma200Series = chartControlStocks.Series["sma200"];
            if (sma200Series == null)
            {
                sma200Series = new Series("sma200", ViewType.Line);
                chartControlStocks.Series.Add(sma200Series);
            }

            volumeSeries.Points.Clear();
            rsiSeries.Points.Clear();
            candlestickSeries.Points.Clear();
            sma50Series.Points.Clear();
            sma200Series.Points.Clear();

            List<double> closePrices = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            foreach (DataGridViewRow row in dataGridView_stocks.Rows)
            {
                if (row.Cells["Date"].Value != null)
                {
                    DateTime date = DateTime.Parse(row.Cells["Date"].Value.ToString());
                    double openPrice = Convert.ToDouble(row.Cells["Open"].Value);
                    double highPrice = Convert.ToDouble(row.Cells["High"].Value);
                    double lowPrice = Convert.ToDouble(row.Cells["Low"].Value);
                    double closePrice = Convert.ToDouble(row.Cells["Close"].Value);
                    double volume = Convert.ToDouble(row.Cells["Volume"].Value);

                    dates.Add(date);
                    closePrices.Add(closePrice);

                    volumeSeries.Points.Add(new SeriesPoint(date, volume));

                    SeriesPoint point = new SeriesPoint(date, new double[] { openPrice, highPrice, lowPrice, closePrice });
                    if (closePrice > openPrice)
                    {
                        point.Color = Color.Green;
                    }
                    else
                    {
                        point.Color = Color.Red;
                    }

                    candlestickSeries.Points.Add(point);
                }
            }

            List<double> rsiValues = stockService.CalculateRSI(closePrices);
            for (int i = 0; i < rsiValues.Count; i++)
            {
                if (i < dates.Count)
                {
                    rsiSeries.Points.Add(new SeriesPoint(dates[i], rsiValues[i]));
                }
            }

            List<double> sma50Values = stockService.CalculateSMA(closePrices, 50);
            List<double> sma200Values = stockService.CalculateSMA(closePrices, 200);
            for (int i = 0; i < dates.Count; i++)
            {
                sma50Series.Points.Add(new SeriesPoint(dates[i], sma50Values[i]));
                sma200Series.Points.Add(new SeriesPoint(dates[i], sma200Values[i]));
            }

            chartControlStocks.Refresh();
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
    }
}