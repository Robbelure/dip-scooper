namespace DipScooper
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
            dataGridView_stocks = new DataGridView();
            DateColumn = new DataGridViewTextBoxColumn();
            OpenColumn = new DataGridViewTextBoxColumn();
            HighColumn = new DataGridViewTextBoxColumn();
            LowColumn = new DataGridViewTextBoxColumn();
            CloseColumn = new DataGridViewTextBoxColumn();
            VolumeColumn = new DataGridViewTextBoxColumn();
            BtnSearch = new Button();
            lblStatus = new Label();
            textBoxSearch = new TextBox();
            TimerStatus = new System.Windows.Forms.Timer(components);
            checkBoxPERatio = new CheckBox();
            checkBoxPBRatio = new CheckBox();
            checkBoxDCF = new CheckBox();
            checkBoxDDM = new CheckBox();
            btnCalculate = new Button();
            dataGridView_analyze = new DataGridView();
            dateTimePickerStart = new DateTimePicker();
            dateTimePickerEnd = new DateTimePicker();
            CalculationColumn = new DataGridViewTextBoxColumn();
            ResultColumn = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView_stocks).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_analyze).BeginInit();
            SuspendLayout();
            // 
            // dataGridView_stocks
            // 
            dataGridView_stocks.AllowUserToAddRows = false;
            dataGridView_stocks.AllowUserToDeleteRows = false;
            dataGridView_stocks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_stocks.BackgroundColor = Color.DimGray;
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
            dataGridView_stocks.GridColor = Color.Gold;
            dataGridView_stocks.Location = new Point(12, 68);
            dataGridView_stocks.Name = "dataGridView_stocks";
            dataGridView_stocks.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.Gray;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = Color.Gold;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dataGridView_stocks.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dataGridView_stocks.RowHeadersVisible = false;
            dataGridView_stocks.RowTemplate.Height = 25;
            dataGridView_stocks.Size = new Size(439, 164);
            dataGridView_stocks.TabIndex = 1;
            // 
            // DateColumn
            // 
            DateColumn.HeaderText = "Date";
            DateColumn.Name = "DateColumn";
            DateColumn.ReadOnly = true;
            // 
            // OpenColumn
            // 
            OpenColumn.HeaderText = "Open";
            OpenColumn.Name = "OpenColumn";
            OpenColumn.ReadOnly = true;
            // 
            // HighColumn
            // 
            HighColumn.HeaderText = "High";
            HighColumn.Name = "HighColumn";
            HighColumn.ReadOnly = true;
            // 
            // LowColumn
            // 
            LowColumn.HeaderText = "Low";
            LowColumn.Name = "LowColumn";
            LowColumn.ReadOnly = true;
            // 
            // CloseColumn
            // 
            CloseColumn.HeaderText = "Close";
            CloseColumn.Name = "CloseColumn";
            CloseColumn.ReadOnly = true;
            // 
            // VolumeColumn
            // 
            VolumeColumn.HeaderText = "Volume";
            VolumeColumn.Name = "VolumeColumn";
            VolumeColumn.ReadOnly = true;
            // 
            // BtnSearch
            // 
            BtnSearch.BackColor = Color.Black;
            BtnSearch.FlatStyle = FlatStyle.Flat;
            BtnSearch.ForeColor = Color.Gold;
            BtnSearch.Location = new Point(12, 32);
            BtnSearch.Name = "BtnSearch";
            BtnSearch.Size = new Size(114, 30);
            BtnSearch.TabIndex = 2;
            BtnSearch.Text = "Search";
            BtnSearch.UseVisualStyleBackColor = false;
            BtnSearch.Click += BtnSearch_Click;
            // 
            // lblStatus
            // 
            lblStatus.ForeColor = Color.Lime;
            lblStatus.Location = new Point(196, 258);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(255, 24);
            lblStatus.TabIndex = 3;
            // 
            // textBoxSearch
            // 
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
            checkBoxPERatio.BackColor = Color.DimGray;
            checkBoxPERatio.ForeColor = Color.Gold;
            checkBoxPERatio.Location = new Point(12, 238);
            checkBoxPERatio.Name = "checkBoxPERatio";
            checkBoxPERatio.Size = new Size(73, 19);
            checkBoxPERatio.TabIndex = 5;
            checkBoxPERatio.Text = "P/E-ratio";
            checkBoxPERatio.UseVisualStyleBackColor = false;
            // 
            // checkBoxPBRatio
            // 
            checkBoxPBRatio.AutoSize = true;
            checkBoxPBRatio.BackColor = Color.DimGray;
            checkBoxPBRatio.ForeColor = Color.Gold;
            checkBoxPBRatio.Location = new Point(12, 263);
            checkBoxPBRatio.Name = "checkBoxPBRatio";
            checkBoxPBRatio.Size = new Size(74, 19);
            checkBoxPBRatio.TabIndex = 6;
            checkBoxPBRatio.Text = "P/B-ratio";
            checkBoxPBRatio.UseVisualStyleBackColor = false;
            // 
            // checkBoxDCF
            // 
            checkBoxDCF.AutoSize = true;
            checkBoxDCF.BackColor = Color.DimGray;
            checkBoxDCF.ForeColor = Color.Gold;
            checkBoxDCF.Location = new Point(95, 238);
            checkBoxDCF.Name = "checkBoxDCF";
            checkBoxDCF.Size = new Size(48, 19);
            checkBoxDCF.TabIndex = 7;
            checkBoxDCF.Text = "DCF";
            checkBoxDCF.UseVisualStyleBackColor = false;
            // 
            // checkBoxDDM
            // 
            checkBoxDDM.AutoSize = true;
            checkBoxDDM.BackColor = Color.DimGray;
            checkBoxDDM.ForeColor = Color.Gold;
            checkBoxDDM.Location = new Point(95, 263);
            checkBoxDDM.Name = "checkBoxDDM";
            checkBoxDDM.Size = new Size(53, 19);
            checkBoxDDM.TabIndex = 8;
            checkBoxDDM.Text = "DDM";
            checkBoxDDM.UseVisualStyleBackColor = false;
            // 
            // btnCalculate
            // 
            btnCalculate.BackColor = Color.Black;
            btnCalculate.FlatStyle = FlatStyle.Flat;
            btnCalculate.ForeColor = Color.Gold;
            btnCalculate.Location = new Point(12, 288);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new Size(126, 30);
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
            dataGridView_analyze.BackgroundColor = Color.DimGray;
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
            dataGridViewCellStyle5.BackColor = Color.Gray;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = Color.Gold;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            dataGridView_analyze.DefaultCellStyle = dataGridViewCellStyle5;
            dataGridView_analyze.GridColor = Color.Gold;
            dataGridView_analyze.Location = new Point(12, 324);
            dataGridView_analyze.Name = "dataGridView_analyze";
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.Gray;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = Color.Gold;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.True;
            dataGridView_analyze.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            dataGridView_analyze.RowHeadersVisible = false;
            dataGridView_analyze.RowTemplate.Height = 25;
            dataGridView_analyze.Size = new Size(200, 159);
            dataGridView_analyze.TabIndex = 10;
            // 
            // dateTimePickerStart
            // 
            dateTimePickerStart.CalendarForeColor = Color.Gold;
            dateTimePickerStart.CalendarMonthBackground = Color.Black;
            dateTimePickerStart.CalendarTitleBackColor = Color.Black;
            dateTimePickerStart.CalendarTitleForeColor = Color.Gold;
            dateTimePickerStart.CalendarTrailingForeColor = Color.DarkGray;
            dateTimePickerStart.CustomFormat = "\"\"";
            dateTimePickerStart.Format = DateTimePickerFormat.Custom;
            dateTimePickerStart.Location = new Point(132, 39);
            dateTimePickerStart.Name = "dateTimePickerStart";
            dateTimePickerStart.Size = new Size(101, 23);
            dateTimePickerStart.TabIndex = 12;
            // 
            // dateTimePickerEnd
            // 
            dateTimePickerEnd.Location = new Point(239, 39);
            dateTimePickerEnd.Name = "dateTimePickerEnd";
            dateTimePickerEnd.Size = new Size(101, 23);
            dateTimePickerEnd.TabIndex = 12;
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            ClientSize = new Size(463, 495);
            Controls.Add(dateTimePickerEnd);
            Controls.Add(dateTimePickerStart);
            Controls.Add(dataGridView_analyze);
            Controls.Add(btnCalculate);
            Controls.Add(checkBoxDDM);
            Controls.Add(checkBoxDCF);
            Controls.Add(checkBoxPBRatio);
            Controls.Add(checkBoxPERatio);
            Controls.Add(textBoxSearch);
            Controls.Add(lblStatus);
            Controls.Add(BtnSearch);
            Controls.Add(dataGridView_stocks);
            ForeColor = Color.Gold;
            Name = "Form1";
            Text = "DipScooper";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView_stocks).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_analyze).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView_stocks;
        private Button BtnSearch;
        private Label lblStatus;
        private TextBox textBoxSearch;
        private System.Windows.Forms.Timer TimerStatus;
        private CheckBox checkBoxPERatio;
        private CheckBox checkBoxPBRatio;
        private CheckBox checkBoxDCF;
        private CheckBox checkBoxDDM;
        private Button btnCalculate;
        private DataGridView dataGridView_analyze;
        private DateTimePicker dateTimePickerStart;
        private DateTimePicker dateTimePickerEnd;
        private DataGridViewTextBoxColumn DateColumn;
        private DataGridViewTextBoxColumn OpenColumn;
        private DataGridViewTextBoxColumn HighColumn;
        private DataGridViewTextBoxColumn LowColumn;
        private DataGridViewTextBoxColumn CloseColumn;
        private DataGridViewTextBoxColumn VolumeColumn;
        private DataGridViewTextBoxColumn CalculationColumn;
        private DataGridViewTextBoxColumn ResultColumn;
    }
}