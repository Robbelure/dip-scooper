using System.Drawing;
using System.Windows.Forms;

namespace DipScooper.UI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            dataGridView_stocks = new DataGridView();
            DateColumn = new DataGridViewTextBoxColumn();
            OpenColumn = new DataGridViewTextBoxColumn();
            HighColumn = new DataGridViewTextBoxColumn();
            LowColumn = new DataGridViewTextBoxColumn();
            CloseColumn = new DataGridViewTextBoxColumn();
            VolumeColumn = new DataGridViewTextBoxColumn();
            BtnSearch = new Button();
            textBoxSearch = new TextBox();
            TimerStatus = new Timer(components);
            checkBoxPERatio = new CheckBox();
            checkBoxPBRatio = new CheckBox();
            checkBoxDCF = new CheckBox();
            checkBoxDDM = new CheckBox();
            btnCalculate = new Button();
            dataGridView_analyze = new DataGridView();
            CalculationColumn = new DataGridViewTextBoxColumn();
            ResultColumn = new DataGridViewTextBoxColumn();
            dateTimePickerStart = new DateTimePicker();
            dateTimePickerEnd = new DateTimePicker();
            progressBar_search = new ProgressBar();
            backgroundWorker_search = new System.ComponentModel.BackgroundWorker();
            chartControlStocks = new DevExpress.XtraCharts.ChartControl();
            dataGridView_dipSignals = new DataGridView();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            BtnOpenSettings = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView_stocks).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_analyze).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartControlStocks).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_dipSignals).BeginInit();
            SuspendLayout();
            // 
            // dataGridView_stocks
            // 
            dataGridView_stocks.AllowUserToAddRows = false;
            dataGridView_stocks.AllowUserToDeleteRows = false;
            dataGridView_stocks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_stocks.BackgroundColor = Color.Black;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.Gray;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = Color.Gold;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridView_stocks.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView_stocks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_stocks.Columns.AddRange(new DataGridViewColumn[] { DateColumn, OpenColumn, HighColumn, LowColumn, CloseColumn, VolumeColumn });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.DimGray;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.Gold;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dataGridView_stocks.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridView_stocks.GridColor = SystemColors.Window;
            dataGridView_stocks.Location = new Point(12, 71);
            dataGridView_stocks.Name = "dataGridView_stocks";
            dataGridView_stocks.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.DimGray;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = Color.Gold;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dataGridView_stocks.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dataGridView_stocks.RowHeadersVisible = false;
            dataGridView_stocks.RowTemplate.Height = 25;
            dataGridView_stocks.Size = new Size(328, 132);
            dataGridView_stocks.TabIndex = 1;
            // 
            // DateColumn
            // 
            DateColumn.FillWeight = 131.254349F;
            DateColumn.HeaderText = "Date";
            DateColumn.Name = "DateColumn";
            DateColumn.ReadOnly = true;
            // 
            // OpenColumn
            // 
            OpenColumn.FillWeight = 82.63412F;
            OpenColumn.HeaderText = "Open";
            OpenColumn.Name = "OpenColumn";
            OpenColumn.ReadOnly = true;
            // 
            // HighColumn
            // 
            HighColumn.FillWeight = 85.3231049F;
            HighColumn.HeaderText = "High";
            HighColumn.Name = "HighColumn";
            HighColumn.ReadOnly = true;
            // 
            // LowColumn
            // 
            LowColumn.FillWeight = 84.0873642F;
            LowColumn.HeaderText = "Low";
            LowColumn.Name = "LowColumn";
            LowColumn.ReadOnly = true;
            // 
            // CloseColumn
            // 
            CloseColumn.FillWeight = 79.64507F;
            CloseColumn.HeaderText = "Close";
            CloseColumn.Name = "CloseColumn";
            CloseColumn.ReadOnly = true;
            // 
            // VolumeColumn
            // 
            VolumeColumn.FillWeight = 137.0558F;
            VolumeColumn.HeaderText = "Volume";
            VolumeColumn.Name = "VolumeColumn";
            VolumeColumn.ReadOnly = true;
            // 
            // BtnSearch
            // 
            BtnSearch.BackColor = Color.Black;
            BtnSearch.FlatAppearance.MouseDownBackColor = Color.Teal;
            BtnSearch.FlatStyle = FlatStyle.Flat;
            BtnSearch.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            BtnSearch.ForeColor = Color.Gold;
            BtnSearch.Location = new Point(12, 38);
            BtnSearch.Name = "BtnSearch";
            BtnSearch.Size = new Size(114, 27);
            BtnSearch.TabIndex = 2;
            BtnSearch.Text = "Search";
            BtnSearch.UseVisualStyleBackColor = false;
            BtnSearch.Click += BtnSearch_Click;
            // 
            // textBoxSearch
            // 
            textBoxSearch.BackColor = SystemColors.InactiveCaptionText;
            textBoxSearch.ForeColor = Color.AntiqueWhite;
            textBoxSearch.Location = new Point(12, 7);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.Size = new Size(328, 23);
            textBoxSearch.TabIndex = 4;
            // 
            // TimerStatus
            // 
            TimerStatus.Interval = 3000;
            TimerStatus.Tick += TimerStatus_Tick;
            // 
            // checkBoxPERatio
            // 
            checkBoxPERatio.AutoSize = true;
            checkBoxPERatio.BackColor = Color.DarkSlateGray;
            checkBoxPERatio.ForeColor = SystemColors.Window;
            checkBoxPERatio.Location = new Point(12, 209);
            checkBoxPERatio.Name = "checkBoxPERatio";
            checkBoxPERatio.Size = new Size(76, 19);
            checkBoxPERatio.TabIndex = 5;
            checkBoxPERatio.Text = "P/E-Ratio";
            checkBoxPERatio.UseVisualStyleBackColor = false;
            // 
            // checkBoxPBRatio
            // 
            checkBoxPBRatio.AutoSize = true;
            checkBoxPBRatio.BackColor = Color.DarkSlateGray;
            checkBoxPBRatio.ForeColor = SystemColors.Window;
            checkBoxPBRatio.Location = new Point(94, 209);
            checkBoxPBRatio.Name = "checkBoxPBRatio";
            checkBoxPBRatio.Size = new Size(77, 19);
            checkBoxPBRatio.TabIndex = 6;
            checkBoxPBRatio.Text = "P/B-Ratio";
            checkBoxPBRatio.UseVisualStyleBackColor = false;
            // 
            // checkBoxDCF
            // 
            checkBoxDCF.AutoSize = true;
            checkBoxDCF.BackColor = Color.DarkSlateGray;
            checkBoxDCF.ForeColor = SystemColors.Window;
            checkBoxDCF.Location = new Point(177, 209);
            checkBoxDCF.Name = "checkBoxDCF";
            checkBoxDCF.Size = new Size(48, 19);
            checkBoxDCF.TabIndex = 7;
            checkBoxDCF.Text = "DCF";
            checkBoxDCF.UseVisualStyleBackColor = false;
            // 
            // checkBoxDDM
            // 
            checkBoxDDM.AutoSize = true;
            checkBoxDDM.BackColor = Color.DarkSlateGray;
            checkBoxDDM.ForeColor = SystemColors.Window;
            checkBoxDDM.Location = new Point(231, 209);
            checkBoxDDM.Name = "checkBoxDDM";
            checkBoxDDM.Size = new Size(53, 19);
            checkBoxDDM.TabIndex = 8;
            checkBoxDDM.Text = "DDM";
            checkBoxDDM.UseVisualStyleBackColor = false;
            // 
            // btnCalculate
            // 
            btnCalculate.BackColor = Color.Black;
            btnCalculate.FlatAppearance.MouseDownBackColor = Color.Teal;
            btnCalculate.FlatStyle = FlatStyle.Flat;
            btnCalculate.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            btnCalculate.ForeColor = Color.Gold;
            btnCalculate.Location = new Point(12, 234);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new Size(114, 27);
            btnCalculate.TabIndex = 9;
            btnCalculate.Text = "Calculate";
            btnCalculate.UseVisualStyleBackColor = false;
            btnCalculate.Click += btnCalculate_Click;
            // 
            // dataGridView_analyze
            // 
            dataGridView_analyze.AllowUserToAddRows = false;
            dataGridView_analyze.AllowUserToDeleteRows = false;
            dataGridView_analyze.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_analyze.BackgroundColor = Color.Black;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.Gray;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = Color.Gold;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            dataGridView_analyze.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridView_analyze.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_analyze.Columns.AddRange(new DataGridViewColumn[] { CalculationColumn, ResultColumn });
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.DimGray;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = Color.Gold;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            dataGridView_analyze.DefaultCellStyle = dataGridViewCellStyle5;
            dataGridView_analyze.GridColor = SystemColors.Window;
            dataGridView_analyze.Location = new Point(12, 267);
            dataGridView_analyze.Name = "dataGridView_analyze";
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.DimGray;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = Color.Gold;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.True;
            dataGridView_analyze.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            dataGridView_analyze.RowHeadersVisible = false;
            dataGridView_analyze.RowTemplate.Height = 25;
            dataGridView_analyze.Size = new Size(328, 99);
            dataGridView_analyze.TabIndex = 10;
            // 
            // CalculationColumn
            // 
            CalculationColumn.HeaderText = "Calculation";
            CalculationColumn.Name = "CalculationColumn";
            // 
            // ResultColumn
            // 
            ResultColumn.HeaderText = "Result";
            ResultColumn.Name = "ResultColumn";
            // 
            // dateTimePickerStart
            // 
            dateTimePickerStart.CalendarForeColor = Color.Gold;
            dateTimePickerStart.CalendarMonthBackground = Color.Black;
            dateTimePickerStart.CalendarTitleBackColor = Color.Black;
            dateTimePickerStart.CalendarTitleForeColor = Color.Gold;
            dateTimePickerStart.CalendarTrailingForeColor = Color.DarkGray;
            dateTimePickerStart.CustomFormat = "";
            dateTimePickerStart.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dateTimePickerStart.Format = DateTimePickerFormat.Custom;
            dateTimePickerStart.Location = new Point(132, 47);
            dateTimePickerStart.Name = "dateTimePickerStart";
            dateTimePickerStart.Size = new Size(101, 23);
            dateTimePickerStart.TabIndex = 12;
            // 
            // dateTimePickerEnd
            // 
            dateTimePickerEnd.Location = new Point(239, 47);
            dateTimePickerEnd.Name = "dateTimePickerEnd";
            dateTimePickerEnd.Size = new Size(101, 23);
            dateTimePickerEnd.TabIndex = 12;
            // 
            // progressBar_search
            // 
            progressBar_search.BackColor = SystemColors.WindowText;
            progressBar_search.Location = new Point(12, 27);
            progressBar_search.Name = "progressBar_search";
            progressBar_search.Size = new Size(328, 3);
            progressBar_search.TabIndex = 13;
            // 
            // chartControlStocks
            // 
            chartControlStocks.BackColor = Color.DimGray;
            chartControlStocks.Legend.BackColor = Color.SlateGray;
            chartControlStocks.Legend.Border.Color = Color.SlateGray;
            chartControlStocks.Location = new Point(346, 7);
            chartControlStocks.Name = "chartControlStocks";
            chartControlStocks.Size = new Size(656, 359);
            chartControlStocks.TabIndex = 14;
            // 
            // dataGridView_dipSignals
            // 
            dataGridView_dipSignals.AllowUserToAddRows = false;
            dataGridView_dipSignals.AllowUserToDeleteRows = false;
            dataGridView_dipSignals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_dipSignals.BackgroundColor = Color.DimGray;
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = Color.Gray;
            dataGridViewCellStyle7.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle7.ForeColor = Color.Gold;
            dataGridViewCellStyle7.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            dataGridView_dipSignals.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            dataGridView_dipSignals.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = Color.DarkSlateGray;
            dataGridViewCellStyle8.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle8.ForeColor = Color.Gold;
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.False;
            dataGridView_dipSignals.DefaultCellStyle = dataGridViewCellStyle8;
            dataGridView_dipSignals.GridColor = Color.Gold;
            dataGridView_dipSignals.Location = new Point(12, 397);
            dataGridView_dipSignals.Name = "dataGridView_dipSignals";
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = Color.DarkSlateGray;
            dataGridViewCellStyle9.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle9.ForeColor = Color.Gold;
            dataGridViewCellStyle9.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.True;
            dataGridView_dipSignals.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            dataGridView_dipSignals.RowHeadersVisible = false;
            dataGridView_dipSignals.RowTemplate.Height = 25;
            dataGridView_dipSignals.Size = new Size(990, 143);
            dataGridView_dipSignals.TabIndex = 15;
            // 
            // labelControl1
            // 
            labelControl1.Appearance.BackColor = Color.DarkSlateGray;
            labelControl1.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);
            labelControl1.Appearance.Options.UseBackColor = true;
            labelControl1.Appearance.Options.UseFont = true;
            labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            labelControl1.Location = new Point(458, 372);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new Size(109, 21);
            labelControl1.TabIndex = 16;
            labelControl1.Text = "Scoop the Dip!";
            // 
            // BtnOpenSettings
            // 
            BtnOpenSettings.BackColor = Color.Black;
            BtnOpenSettings.FlatAppearance.MouseDownBackColor = Color.Teal;
            BtnOpenSettings.FlatStyle = FlatStyle.Flat;
            BtnOpenSettings.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            BtnOpenSettings.ForeColor = Color.AntiqueWhite;
            BtnOpenSettings.Location = new Point(911, 372);
            BtnOpenSettings.Name = "BtnOpenSettings";
            BtnOpenSettings.Size = new Size(91, 25);
            BtnOpenSettings.TabIndex = 17;
            BtnOpenSettings.Text = "Customize";
            BtnOpenSettings.UseVisualStyleBackColor = false;
            BtnOpenSettings.Click += BtnOpenSettings_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkSlateGray;
            ClientSize = new Size(1014, 552);
            Controls.Add(BtnOpenSettings);
            Controls.Add(labelControl1);
            Controls.Add(dataGridView_dipSignals);
            Controls.Add(chartControlStocks);
            Controls.Add(progressBar_search);
            Controls.Add(dateTimePickerEnd);
            Controls.Add(dateTimePickerStart);
            Controls.Add(dataGridView_analyze);
            Controls.Add(btnCalculate);
            Controls.Add(checkBoxDDM);
            Controls.Add(checkBoxDCF);
            Controls.Add(checkBoxPBRatio);
            Controls.Add(checkBoxPERatio);
            Controls.Add(textBoxSearch);
            Controls.Add(BtnSearch);
            Controls.Add(dataGridView_stocks);
            ForeColor = Color.Gold;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "DipScooper";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView_stocks).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_analyze).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartControlStocks).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_dipSignals).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView_stocks;
        private Button BtnSearch;
        private TextBox textBoxSearch;
        private Timer TimerStatus;
        private CheckBox checkBoxPERatio;
        private CheckBox checkBoxPBRatio;
        private CheckBox checkBoxDCF;
        private CheckBox checkBoxDDM;
        private Button btnCalculate;
        private DataGridView dataGridView_analyze;
        private DateTimePicker dateTimePickerStart;
        private DateTimePicker dateTimePickerEnd;
        private DataGridViewTextBoxColumn CalculationColumn;
        private DataGridViewTextBoxColumn ResultColumn;
        private ProgressBar progressBar_search;
        private System.ComponentModel.BackgroundWorker backgroundWorker_search;
        private DevExpress.XtraCharts.ChartControl chartControlStocks;
        private DataGridViewTextBoxColumn DateColumn;
        private DataGridViewTextBoxColumn OpenColumn;
        private DataGridViewTextBoxColumn HighColumn;
        private DataGridViewTextBoxColumn LowColumn;
        private DataGridViewTextBoxColumn CloseColumn;
        private DataGridViewTextBoxColumn VolumeColumn;
        private DataGridView dataGridView_dipSignals;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private Button BtnOpenSettings;
    }
}