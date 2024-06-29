using System.Diagnostics;
using DevExpress.XtraCharts;
using MongoDB.Driver;
using System.Windows.Forms;
using DipScooper.Models.Models;
using System.Collections.Generic;
using System;
using System.Drawing;
using DipScooper.BLL;
using System.Linq;
using DipScooper.Models;

namespace DipScooper.UI
{
    /// <summary>
    /// Form1 handles the main user interface for the DipScooper application, managing user interactions and displaying data.
    ///
    /// **Core Responsibilities**:
    /// 1. **User Interactions**:
    ///    - Handles user input for stock symbol and date range selection.
    ///    - Triggers data retrieval and analysis based on user actions (e.g., search button click).
    /// 2. **Data Display**:
    ///    - Updates data grids and charts to display historical stock data and dip signals.
    ///    - Reflects user-configured dip thresholds in real-time.
    ///
    /// **Key Methods**:
    /// - `BtnSearch_Click`: Initiates data retrieval and analysis based on user input.
    /// - `CheckForDipSignals`: Updates the dip signals data grid based on the latest analysis.
    /// - `SettingsForm_SettingsSaved`: Responds to changes in user-configured dip thresholds and updates the dip signals.
    ///
    /// **Dependencies**:
    /// - Utilizes `StockService` for business logic and data retrieval.
    /// - Interacts with `SettingsForm` for configuring dip thresholds.
    /// </summary>
    public partial class Form1 : Form
    {
        private StockService stockService;
        private List<HistoricalData> historicalDataList;
        private static SettingsForm settingsFormInstance;

        public Form1()
        {
            InitializeComponent();
            stockService = new StockService();
            InitializeControls();

            Debug.WriteLine("Form1 initialized.");
        }

        private void InitializeControls()
        {
            InitializeDateTimePickers();
            InitializeTextBoxSearch();
            InitializeDataGridViews();
            InitializeChartControl();

            BtnSearch.Click += BtnSearch_Click;
            BtnOpenSettings.Click += BtnOpenSettings_Click;
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string symbol = textBoxSearch.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Please enter a valid stock symbol.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            progressBar_search.Visible = true;
            progressBar_search.Minimum = 0;
            dataGridView_stocks.Rows.Clear();
            dataGridView_dipSignals.Rows.Clear();

            try
            {
                var progress = new Progress<int>(value => progressBar_search.Value = value);

                DateTime startDate = dateTimePickerStart.Value.Date;
                DateTime endDate = dateTimePickerEnd.Value.Date;

                historicalDataList = await stockService.LoadDataWithProgressAsync(symbol, startDate, endDate, progress);

                if (historicalDataList.Any())
                {
                    Debug.WriteLine("Historical data loaded successfully.");
                    InitializeChartControl();
                    UpdateChartWithData(historicalDataList);
                    chartControlStocks.Refresh();

                    foreach (var data in historicalDataList)
                    {
                        dataGridView_stocks.Rows.Add(CreateRowFromData(data));
                    }

                    CheckForDipSignals(historicalDataList);
                }
                else
                {
                    Debug.WriteLine("No historical data found for the given symbol and date range.");
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

            try
            {
                List<CalculationResult> results = new List<CalculationResult>();

                if (checkBoxPERatio.Checked)
                {
                    results.AddRange(await stockService.CalculatePERatio(symbol));
                }

                if (checkBoxPBRatio.Checked)
                {
                    results.AddRange(await stockService.CalculatePBRatio(symbol));
                }

                if (checkBoxDCF.Checked)
                {
                    results.AddRange(await stockService.CalculateDCF(symbol, discountRate));
                }

                if (checkBoxDDM.Checked)
                {
                    results.AddRange(await stockService.CalculateDDM(symbol, growthRate, discountRate));
                }

                UpdateUIComponents(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error performing calculations: {ex.Message}\nStackTrace: {ex.StackTrace}");
                Debug.WriteLine($"Error performing calculations: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private void BtnOpenSettings_Click(object sender, EventArgs e)
        {
            if (settingsFormInstance == null || settingsFormInstance.IsDisposed)
            {
                Debug.WriteLine("Opening SettingsForm...");
                settingsFormInstance = new SettingsForm();
                settingsFormInstance.SettingsSaved += SettingsForm_SettingsSaved;
                settingsFormInstance.FormClosed += SettingsFormInstance_FormClosed;
                settingsFormInstance.Show();
            }
        }

        private void SettingsFormInstance_FormClosed(object sender, FormClosedEventArgs e)
        {
            settingsFormInstance.SettingsSaved -= SettingsForm_SettingsSaved;
            settingsFormInstance.FormClosed -= SettingsFormInstance_FormClosed;
            settingsFormInstance = null;
            Debug.WriteLine("SettingsForm closed.");
        }

        private void SettingsForm_SettingsSaved(object sender, EventArgs e)
        {
            Debug.WriteLine("SettingsForm_SettingsSaved event triggered.");

            // Kall CheckForDipSignals på nytt med de nye innstillingene
            if (historicalDataList != null && historicalDataList.Any())
            {
                CheckForDipSignals(historicalDataList);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar_search.Visible = false;
        }

        #region Initialization Methods

        private void InitializeDateTimePickers()
        {
            InitializeDateTimePicker(dateTimePickerStart);
            InitializeDateTimePicker(dateTimePickerEnd);
        }

        private void InitializeDateTimePicker(DateTimePicker dateTimePicker)
        {
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = "'Select Date...'";
            dateTimePicker.Font = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dateTimePicker.Tag = false;

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
                dateTimePicker.Tag = true;
            };

            dateTimePicker.Leave += (s, e) =>
            {
                ActiveControl = null;
            };
        }

        private void InitializeTextBoxSearch()
        {
            SetPlaceholderText(textBoxSearch, "Enter ticker symbol (example: Tesla = TSLA)");
            textBoxSearch.GotFocus += RemovePlaceholderText;
            textBoxSearch.LostFocus += ShowPlaceholderText;
        }

        private void InitializeDataGridViews()
        {
            dataGridView_stocks.AutoGenerateColumns = false;
            InitializeDataGridViewStocksColumns();

            dataGridView_analyze.AutoGenerateColumns = false;
            InitializeDataGridViewAnalyzeColumns();

            InitializeDataGridViewDipSignals();
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

            // Sett fargene for kolonneoverskriftene
            dataGridView_stocks.ColumnHeadersDefaultCellStyle.BackColor = Color.DimGray;
            dataGridView_stocks.ColumnHeadersDefaultCellStyle.ForeColor = Color.AntiqueWhite;

            // Sett fargene for radene
            dataGridView_stocks.RowsDefaultCellStyle.BackColor = Color.Black;
            dataGridView_stocks.RowsDefaultCellStyle.ForeColor = Color.AntiqueWhite;

            dataGridView_stocks.EnableHeadersVisualStyles = false;

            // Oppdater DataGridView for å vise endringene
            dataGridView_stocks.Refresh();
        }

        private void InitializeDataGridViewAnalyzeColumns()
        {
            dataGridView_analyze.Columns.Clear();
            dataGridView_analyze.Columns.Add("Calculation", "Calculation");
            dataGridView_analyze.Columns.Add("Result", "Result");

            // Sett fargene for kolonneoverskriftene
            dataGridView_analyze.ColumnHeadersDefaultCellStyle.BackColor = Color.DimGray;
            dataGridView_analyze.ColumnHeadersDefaultCellStyle.ForeColor = Color.AntiqueWhite;

            // Sett fargene for radene
            dataGridView_analyze.RowsDefaultCellStyle.BackColor = Color.Black;
            dataGridView_analyze.RowsDefaultCellStyle.ForeColor = Color.AntiqueWhite;

            dataGridView_analyze.EnableHeadersVisualStyles = false;

            // Oppdater DataGridView for å vise endringene
            dataGridView_analyze.Refresh();
        }

        private void InitializeDataGridViewDipSignals()
        {
            dataGridView_dipSignals.Columns.Clear();
            dataGridView_dipSignals.Columns.Add("Date", "Date");
            dataGridView_dipSignals.Columns.Add("DipType", "Dip Type");
            dataGridView_dipSignals.Columns.Add("Signal", "Signal");
            dataGridView_dipSignals.Columns.Add("Value", "Value");
        }

        private void InitializeChartControl()
        {
            chartControlStocks.Series.Clear();

            Series closeSeries = new Series("Close", ViewType.Line);
            Series volumeSeries = new Series("Volume", ViewType.Bar);
            Series rsiSeries = new Series("RSI", ViewType.Line);
            Series candlestickSeries = new Series("Price", ViewType.CandleStick);
            Series sma50Series = new Series("SMA50", ViewType.Line);
            Series sma200Series = new Series("SMA200", ViewType.Line);

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
            candleStickView.LineThickness = 2;
            candleStickView.LevelLineLength = 0.5;
            candleStickView.Color = Color.Green;

            XYDiagram diagram = (XYDiagram)chartControlStocks.Diagram;

            // Set the background color for the diagram area
            diagram.DefaultPane.BackColor = Color.DimGray;
            diagram.DefaultPane.FillStyle.FillMode = FillMode.Solid;

            diagram.SecondaryAxesY.Clear();


            diagram.AxisX.DateTimeScaleOptions.WorkdaysOnly = true;
            diagram.AxisX.DateTimeScaleOptions.AggregateFunction = AggregateFunction.None;
            diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Day;
            diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Month;

            diagram.AxisX.Label.TextPattern = "{A:MMM-yyyy}";
            diagram.AxisX.Label.Angle = -45;
            diagram.AxisX.Label.ResolveOverlappingOptions.AllowRotate = false;
            diagram.AxisX.Label.ResolveOverlappingOptions.AllowStagger = true;
            diagram.AxisX.Label.TextColor = Color.White;
            diagram.AxisX.Label.Font = new Font("Segoe UI", 8, FontStyle.Regular);

            diagram.AxisX.Title.Text = "Date";
            diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisX.Title.TextColor = Color.White;
            diagram.AxisX.Title.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            diagram.AxisY.Title.Text = "Price/RSI";
            diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisY.Title.TextColor = Color.White;
            diagram.AxisY.Title.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            diagram.AxisY.Label.TextColor = Color.White;
            diagram.AxisY.Label.Font = new Font("Segoe UI", 8, FontStyle.Regular);

            SecondaryAxisY secondaryAxisYVolume = new SecondaryAxisY("Volume Axis");
            secondaryAxisYVolume.WholeRange.SetMinMaxValues(0, 1000000000);
            secondaryAxisYVolume.Label.TextColor = Color.White;
            secondaryAxisYVolume.Label.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            secondaryAxisYVolume.Label.TextPattern = "{V:#,0,,}M";
            diagram.SecondaryAxesY.Add(secondaryAxisYVolume);
            ((BarSeriesView)volumeSeries.View).AxisY = secondaryAxisYVolume;


            ((LineSeriesView)rsiSeries.View).AxisY = diagram.AxisY;

            chartControlStocks.Titles.Clear();
            ChartTitle chartTitle = new ChartTitle();
            chartTitle.Text = "Stock Data Chart";
            chartTitle.TextColor = Color.White;
            chartTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            chartControlStocks.Titles.Add(chartTitle);
            chartControlStocks.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;

            chartControlStocks.Legend.BackColor = Color.DimGray;
            chartControlStocks.Legend.FillStyle.FillMode = FillMode.Solid;
            chartControlStocks.Legend.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            chartControlStocks.Legend.TextColor = Color.White;

            BarSeriesView? barView = volumeSeries.View as BarSeriesView;
            if (barView != null)
            {
                barView.BarWidth = 0.3;
            }
        }

        #endregion

        #region UI Update Methods

        private void UpdateUIComponents(List<CalculationResult> results)
        {
            dataGridView_analyze.Rows.Clear();
            foreach (var result in results)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView_analyze, result.Name, result.Value, result.Value.ToString("F5"));
                dataGridView_analyze.Rows.Add(row);
            }
        }

        private void UpdateChartWithData(List<HistoricalData> historicalDataList)
        {
            if (historicalDataList == null || !historicalDataList.Any())
                return;

            // Finn seriene
            Series volumeSeries = chartControlStocks.Series["Volume"];
            Series rsiSeries = chartControlStocks.Series["RSI"];
            Series candlestickSeries = chartControlStocks.Series["Price"];
            Series sma50Series = chartControlStocks.Series["SMA50"];
            Series sma200Series = chartControlStocks.Series["SMA200"];

            // Tøm eksisterende punkter
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

            // Beregn RSI, SMA50 og SMA200
            List<CalculationResult> rsiResults = stockService.CalculateRSI(closePrices);
            for (int i = 0; i < rsiResults.Count; i++)
            {
                if (i + 14 < dates.Count)
                {
                    rsiSeries.Points.Add(new SeriesPoint(dates[i + 14], rsiResults[i].Value));
                    Debug.WriteLine($"Added SeriesPoint to rsiSeries: Date={dates[i + 14]}, Value={rsiResults[i].Value}");
                }
            }

            List<CalculationResult> sma50Results = stockService.CalculateSMA(closePrices, 50);
            List<CalculationResult> sma200Results = stockService.CalculateSMA(closePrices, 200);
            for (int i = 0; i < dates.Count; i++)
            {
                if (i < sma50Results.Count)
                {
                    sma50Series.Points.Add(new SeriesPoint(dates[i], sma50Results[i].Value));
                    Debug.WriteLine($"Added SeriesPoint to sma50Series: Date={dates[i]}, Value={sma50Results[i].Value}");
                }
                if (i < sma200Results.Count)
                {
                    sma200Series.Points.Add(new SeriesPoint(dates[i], sma200Results[i].Value));
                    Debug.WriteLine($"Added SeriesPoint to sma200Series: Date={dates[i]}, Value={sma200Results[i].Value}");
                }
            }

            chartControlStocks.Refresh();
        }

        private DataGridViewRow CreateRowFromData(HistoricalData data)
        {
            var row = new DataGridViewRow();
            row.CreateCells(dataGridView_stocks);

            row.Cells[0].Value = data.Date.ToString("yyyy-MM-dd");
            row.Cells[1].Value = data.Open.ToString("F1");
            row.Cells[2].Value = data.High.ToString("F1");
            row.Cells[3].Value = data.Low.ToString("F1");
            row.Cells[4].Value = data.Close.ToString("F1");
            row.Cells[5].Value = data.Volume.ToString("N0");

            return row;
        }

        private void CheckForDipSignals(List<HistoricalData> historicalDataList)
        {
            dataGridView_dipSignals.Rows.Clear();

            int normalDipThreshold = Properties.Settings.Default.NormalDipThreshold;
            int bigDipThreshold = Properties.Settings.Default.BigDipThreshold;
            int superDipThreshold = Properties.Settings.Default.SuperDipThreshold;

            Debug.WriteLine($"CheckForDipSignals called with thresholds: NormalDipThreshold={normalDipThreshold}, BigDipThreshold={bigDipThreshold}, SuperDipThreshold={superDipThreshold}");

            List<DipSignal> dipSignals = stockService.CalculateDipSignals(historicalDataList, normalDipThreshold, bigDipThreshold, superDipThreshold);

            foreach (var signal in dipSignals)
            {
                var rowIndex = dataGridView_dipSignals.Rows.Add(signal.Date, signal.DipType, signal.Signal, signal.Value);
                if (signal.DipType == "SUPER-DIP")
                {
                    dataGridView_dipSignals.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Purple;
                }
            }
            Debug.WriteLine($"CheckForDipSignals completed. {dipSignals.Count} dip signals found.");

        }

        #endregion

        #region Placeholder Text Methods

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
                textBox.ForeColor = Color.AntiqueWhite;
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

        #endregion

        #region Status Timer Methods

        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            TimerStatus.Stop();
        }

        private void StartStatusTimer()
        {
            TimerStatus.Start();
        }

        #endregion


    }
}