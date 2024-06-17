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
            dataGridView_stocks = new DataGridView();
            BtnSearch = new Button();
            lblStatus = new Label();
            textBoxSearch = new TextBox();
            TimerStatus = new System.Windows.Forms.Timer(components);
            checkBoxPERatio = new CheckBox();
            checkBoxPBRatio = new CheckBox();
            checkBoxDCF = new CheckBox();
            checkBoxDividendDiscountModel = new CheckBox();
            btnCalculate = new Button();
            dataGridView_analyze = new DataGridView();
            comboBoxFrequency = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView_stocks).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView_analyze).BeginInit();
            SuspendLayout();
            // 
            // dataGridView_stocks
            // 
            dataGridView_stocks.BackgroundColor = SystemColors.Info;
            dataGridView_stocks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_stocks.GridColor = SystemColors.Info;
            dataGridView_stocks.Location = new Point(12, 91);
            dataGridView_stocks.Name = "dataGridView_stocks";
            dataGridView_stocks.ReadOnly = true;
            dataGridView_stocks.RowTemplate.Height = 25;
            dataGridView_stocks.Size = new Size(463, 362);
            dataGridView_stocks.TabIndex = 1;
            // 
            // BtnSearch
            // 
            BtnSearch.BackColor = Color.LightGray;
            BtnSearch.Location = new Point(164, 12);
            BtnSearch.Name = "BtnSearch";
            BtnSearch.Size = new Size(127, 33);
            BtnSearch.TabIndex = 2;
            BtnSearch.Text = "Search";
            BtnSearch.UseVisualStyleBackColor = false;
            BtnSearch.Click += BtnSearch_Click;
            // 
            // lblStatus
            // 
            lblStatus.ForeColor = Color.Lime;
            lblStatus.Location = new Point(3, 471);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(903, 24);
            lblStatus.TabIndex = 3;
            // 
            // textBoxSearch
            // 
            textBoxSearch.Location = new Point(12, 18);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.Size = new Size(146, 23);
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
            checkBoxPERatio.Location = new Point(539, 12);
            checkBoxPERatio.Name = "checkBoxPERatio";
            checkBoxPERatio.Size = new Size(82, 19);
            checkBoxPERatio.TabIndex = 5;
            checkBoxPERatio.Text = "checkBox1";
            checkBoxPERatio.UseVisualStyleBackColor = true;
            // 
            // checkBoxPBRatio
            // 
            checkBoxPBRatio.AutoSize = true;
            checkBoxPBRatio.Location = new Point(539, 37);
            checkBoxPBRatio.Name = "checkBoxPBRatio";
            checkBoxPBRatio.Size = new Size(82, 19);
            checkBoxPBRatio.TabIndex = 6;
            checkBoxPBRatio.Text = "checkBox2";
            checkBoxPBRatio.UseVisualStyleBackColor = true;
            // 
            // checkBoxDCF
            // 
            checkBoxDCF.AutoSize = true;
            checkBoxDCF.Location = new Point(627, 12);
            checkBoxDCF.Name = "checkBoxDCF";
            checkBoxDCF.Size = new Size(82, 19);
            checkBoxDCF.TabIndex = 7;
            checkBoxDCF.Text = "checkBox3";
            checkBoxDCF.UseVisualStyleBackColor = true;
            // 
            // checkBoxDividendDiscountModel
            // 
            checkBoxDividendDiscountModel.AutoSize = true;
            checkBoxDividendDiscountModel.Location = new Point(627, 37);
            checkBoxDividendDiscountModel.Name = "checkBoxDividendDiscountModel";
            checkBoxDividendDiscountModel.Size = new Size(82, 19);
            checkBoxDividendDiscountModel.TabIndex = 8;
            checkBoxDividendDiscountModel.Text = "checkBox4";
            checkBoxDividendDiscountModel.UseVisualStyleBackColor = true;
            // 
            // btnCalculate
            // 
            btnCalculate.Location = new Point(539, 62);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new Size(95, 30);
            btnCalculate.TabIndex = 9;
            btnCalculate.Text = "Calculate";
            btnCalculate.UseVisualStyleBackColor = true;
            btnCalculate.Click += btnCalculate_Click;
            // 
            // dataGridView_analyze
            // 
            dataGridView_analyze.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_analyze.Location = new Point(539, 98);
            dataGridView_analyze.Name = "dataGridView_analyze";
            dataGridView_analyze.RowTemplate.Height = 25;
            dataGridView_analyze.Size = new Size(367, 315);
            dataGridView_analyze.TabIndex = 10;
            // 
            // comboBoxFrequency
            // 
            comboBoxFrequency.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFrequency.FormattingEnabled = true;
            comboBoxFrequency.Items.AddRange(new object[] { "Daily", "Weekly", "Monthly" });
            comboBoxFrequency.Location = new Point(12, 69);
            comboBoxFrequency.Name = "comboBoxFrequency";
            comboBoxFrequency.Size = new Size(121, 23);
            comboBoxFrequency.TabIndex = 11;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkGray;
            ClientSize = new Size(918, 495);
            Controls.Add(comboBoxFrequency);
            Controls.Add(dataGridView_analyze);
            Controls.Add(btnCalculate);
            Controls.Add(checkBoxDividendDiscountModel);
            Controls.Add(checkBoxDCF);
            Controls.Add(checkBoxPBRatio);
            Controls.Add(checkBoxPERatio);
            Controls.Add(textBoxSearch);
            Controls.Add(lblStatus);
            Controls.Add(BtnSearch);
            Controls.Add(dataGridView_stocks);
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
        private CheckBox checkBoxDividendDiscountModel;
        private Button btnCalculate;
        private DataGridView dataGridView_analyze;
        private ComboBox comboBoxFrequency;
    }
}