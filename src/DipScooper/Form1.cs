using System.Text.Json;
using System.Diagnostics;
using DipScooper.Services;
using DevExpress.XtraCharts;
using DipScooper.Data;
using DipScooper.Models;
using MongoDB.Driver;

namespace DipScooper
{
    public partial class Form1 : Form
    {
        private ApiClient apiClient;
        private StockService stockService;
        private DbContext dbContext;

        public Form1()
        {
            InitializeComponent();
            apiClient = new ApiClient();
            stockService = new StockService();
            dbContext = new DbContext();

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
        /// H�ndterer klikkhendelsen for beregningsknappen.
        /// Utf�rer finansielle beregninger basert p� brukerens valg.
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
        /// H�ndterer klikkhendelsen for s�keknappen.
        /// Starter en asynkron s�kingsprosess for � hente aksjedata.
        /// </summary>
        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string symbol = textBoxSearch.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Please enter a valid stock symbol.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            progressBar_search.Visible = true;
            progressBar_search.Value = 0;
            lblStatus.Text = "Searching...";
            lblStatus.ForeColor = System.Drawing.Color.Blue;
            dataGridView_stocks.Rows.Clear();

            var stock = await dbContext.Stocks.Find(s => s.Symbol == symbol).FirstOrDefaultAsync();
            if (stock == null)
            {
                stock = new Stock { Symbol = symbol, Name = "Unknown", Market = "Unknown" };
                await dbContext.Stocks.InsertOneAsync(stock);
            }

            DateTime startDate = dateTimePickerStart.Value.Date;
            DateTime endDate = dateTimePickerEnd.Value.Date;

            try
            {
                var historicalDataList = await LoadDataToGrid(stock.Id, startDate, endDate);
                if (historicalDataList.Any())
                {
                    UpdateChartWithData(historicalDataList);
                    chartControlStocks.Refresh();  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("Exception: " + ex.Message + "\nStackTrace: " + ex.StackTrace);
            }
            finally
            {
                progressBar_search.Visible = false;
                lblStatus.Text = "Ready";
                lblStatus.ForeColor = System.Drawing.Color.Black;
            }
        }

        /// <summary>
        /// Kj�rer bakgrunnss�k asynkront for � hente aksjedata.
        /// </summary>
        private async Task RunBackgroundSearchAsync(string symbol, string stockId, DateTime startDate, DateTime endDate)
        {
            try
            {
                string jsonData = await apiClient.GetTimeSeriesAsync(symbol, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
                Debug.WriteLine(jsonData);
                if (string.IsNullOrEmpty(jsonData))
                {
                    throw new Exception("No data retrieved from the API.");
                }

                var historicalData = ProcessJsonData(jsonData, stockId);

                var existingData = await dbContext.HistoricalData
                    .Find(hd => hd.StockId == stockId && hd.Date >= startDate && hd.Date <= endDate)
                    .ToListAsync();

                var newData = historicalData
                    .Where(newData => !existingData.Any(existing => existing.Date == newData.Date))
                    .ToList();

                if (newData.Any())
                {
                    await dbContext.HistoricalData.InsertManyAsync(newData);
                }

                Invoke(new Action(() =>
                {
                    foreach (var data in newData)
                    {
                        dataGridView_stocks.Rows.Add(CreateRowFromData(data));
                    }
                    UpdateChartWithData(existingData.Concat(newData).ToList());
                    lblStatus.Text = "Data loaded successfully.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
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

        private List<(DateTime, DateTime)> GetMissingDateRanges(DateTime startDate, DateTime endDate, List<DateTime> existingDates)
        {
            List<(DateTime, DateTime)> missingRanges = new List<(DateTime, DateTime)>();

            DateTime current = startDate;
            while (current <= endDate)
            {
                if (!existingDates.Contains(current))
                {
                    DateTime rangeStart = current;
                    while (current <= endDate && !existingDates.Contains(current))
                    {
                        current = current.AddDays(1);
                    }
                    DateTime rangeEnd = current.AddDays(-1);
                    missingRanges.Add((rangeStart, rangeEnd));
                }
                current = current.AddDays(1);
            }

            return missingRanges;
        }

        /// <summary>
        /// Prosesserer JSON-data mottatt fra APIet og konverterer det til rader som kan vises i dataGridView_stocks.
        /// </summary>
        /// <param name="jsonData">JSON-data mottatt fra APIet.</param>
        /// <returns>Liste over DataGridViewRow objekter.</returns>
        private List<HistoricalData> ProcessJsonData(string jsonData, string stockId)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new ArgumentException("JSON data is null or empty.");

            var jsonDocument = JsonDocument.Parse(jsonData);
            var root = jsonDocument.RootElement.GetProperty("results");
            List<HistoricalData> historicalDataList = new List<HistoricalData>();

            foreach (var result in root.EnumerateArray())
            {
                try
                {
                    if (result.TryGetProperty("t", out var tProperty) &&
                        result.TryGetProperty("o", out var oProperty) &&
                        result.TryGetProperty("h", out var hProperty) &&
                        result.TryGetProperty("l", out var lProperty) &&
                        result.TryGetProperty("c", out var cProperty) &&
                        result.TryGetProperty("v", out var vProperty))
                    {
                        HistoricalData historicalData = new HistoricalData
                        {
                            StockId = stockId,
                            Date = DateTimeOffset.FromUnixTimeMilliseconds(tProperty.GetInt64()).DateTime,
                            Open = oProperty.GetDouble(),
                            High = hProperty.GetDouble(),
                            Low = lProperty.GetDouble(),
                            Close = cProperty.GetDouble(),
                            Volume = Convert.ToInt64(vProperty.GetDouble())
                        };

                        historicalDataList.Add(historicalData);
                        Debug.WriteLine($"Processed data for date: {historicalData.Date}");
                    }
                    else
                    {
                        Debug.WriteLine("Missing one or more properties in the result element.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing result: {result}");
                    Debug.WriteLine($"Exception: {ex.Message}");
                    Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                    throw;
                }
            }

            Debug.WriteLine("Finished processing JSON data.");
            return historicalDataList;
        }

        private async Task<List<HistoricalData>> LoadDataToGrid(string stockId, DateTime startDate, DateTime endDate)
        {
            var historicalDataList = await dbContext.HistoricalData
                .Find(hd => hd.StockId == stockId && hd.Date >= startDate && hd.Date <= endDate)
                .ToListAsync();

            var existingDates = historicalDataList.Select(hd => hd.Date).ToList();

            if (historicalDataList.Count > 0)
            {
                foreach (var data in historicalDataList)
                {
                    if (!dataGridView_stocks.Rows.Cast<DataGridViewRow>().Any(r => r.Cells["Date"].Value.ToString() == data.Date.ToString("yyyy-MM-dd")))
                    {
                        dataGridView_stocks.Rows.Add(CreateRowFromData(data));
                    }
                }
            }

            var missingRanges = GetMissingDateRanges(startDate, endDate, existingDates);

            foreach (var (rangeStart, rangeEnd) in missingRanges)
            {
                await RunBackgroundSearchAsync(textBoxSearch.Text.Trim().ToUpper(), stockId, rangeStart, rangeEnd);
            }

            return historicalDataList;
        }

        private DataGridViewRow CreateRowFromData(HistoricalData data)
        {
            var row = new DataGridViewRow();
            row.CreateCells(dataGridView_stocks);

            row.Cells[0].Value = data.Date.ToString("yyyy-MM-dd");
            row.Cells[1].Value = data.Open;
            row.Cells[2].Value = data.High;
            row.Cells[3].Value = data.Low;
            row.Cells[4].Value = data.Close;
            row.Cells[5].Value = data.Volume;

            return row;
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

        private void UpdateChartWithData(List<HistoricalData> historicalDataList)
        {
            if (dataGridView_stocks.Rows.Count == 0)
                return;

            Series volumeSeries = chartControlStocks.Series["Volume"];
            Series rsiSeries = chartControlStocks.Series["RSI"];
            Series candlestickSeries = chartControlStocks.Series["Price"];

            Series sma50Series = chartControlStocks.Series["SMA50"];
            if (sma50Series == null)
            {
                sma50Series = new Series("SMA50", ViewType.Line);
                chartControlStocks.Series.Add(sma50Series);
            }

            Series sma200Series = chartControlStocks.Series["SMA200"];
            if (sma200Series == null)
            {
                sma200Series = new Series("SMA200", ViewType.Line);
                chartControlStocks.Series.Add(sma200Series);
            }

            volumeSeries.Points.Clear();
            rsiSeries.Points.Clear();
            candlestickSeries.Points.Clear();
            sma50Series.Points.Clear();
            sma200Series.Points.Clear();

            List<double> closePrices = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            foreach (var data in historicalDataList)
            {
                if (data != null)
                {
                    DateTime date = data.Date;
                    double openPrice = data.Open;
                    double highPrice = data.High;
                    double lowPrice = data.Low;
                    double closePrice = data.Close;
                    double volume = data.Volume;

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