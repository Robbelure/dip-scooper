using System.Text.Json;
using System.Diagnostics;
using DevExpress.XtraCharts;
using MongoDB.Driver;
using System.Windows.Forms;
using DipScooper.Models.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Drawing;
using DipScooper.DAL.API;
using DipScooper.BLL;
using DipScooper.Dal.Data;
using System.Linq;
using DipScooper.Models;

namespace DipScooper.UI
{
    public partial class Form1 : Form
    {
        private StockService stockService;

        public Form1()
        {
            InitializeComponent();
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

                // Oppdater DataGridView med resultatene
                UpdateUIComponents(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error performing calculations: {ex.Message}\nStackTrace: {ex.StackTrace}");
                Debug.WriteLine($"Error performing calculations: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
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
            progressBar_search.Value = 0;
            lblStatus.Text = "Searching...";
            lblStatus.ForeColor = System.Drawing.Color.Blue;
            dataGridView_stocks.Rows.Clear();

            try
            {
                var stock = await stockService.GetOrCreateStockAsync(symbol);

                DateTime startDate = dateTimePickerStart.Value.Date;
                DateTime endDate = dateTimePickerEnd.Value.Date;

                var historicalDataList = await stockService.LoadHistoricalDataAsync(stock.Id, symbol, startDate, endDate);
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

        private void UpdateUIComponents(List<CalculationResult> results)
        {
            dataGridView_analyze.Rows.Clear();
            foreach (var result in results)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView_analyze, result.Name, result.Value);
                dataGridView_analyze.Rows.Add(row);

                UpdateChartWithResult(result);
            }

            chartControlStocks.Refresh();
        }

        private void UpdateChartWithResult(CalculationResult result)
        {
            var series = chartControlStocks.Series.FirstOrDefault(s => s.Name == result.Name) as Series;
            if (series == null)
            {
                series = new Series(result.Name, ViewType.Line);
                chartControlStocks.Series.Add(series);
            }
            series.Points.Add(new SeriesPoint(DateTime.Now, result.Value));
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

            // T�m eksisterende punkter
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
            candleStickView.LineThickness = 3;
            candleStickView.LevelLineLength = 0.7;
            candleStickView.Color = Color.Green;

            XYDiagram diagram = (XYDiagram)chartControlStocks.Diagram;

            diagram.AxisX.DateTimeScaleOptions.WorkdaysOnly = true;
            diagram.AxisX.DateTimeScaleOptions.AggregateFunction = AggregateFunction.None;
            diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Week;
            diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Week;

            diagram.AxisX.Title.Text = "Date";
            diagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            diagram.AxisY.Title.Text = "Price";
            diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;

            SecondaryAxisY secondaryAxisYVolume = new SecondaryAxisY("Volume Axis");
            secondaryAxisYVolume.WholeRange.Auto = true;
            secondaryAxisYVolume.WholeRange.SetMinMaxValues(500000, 3000000000);
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