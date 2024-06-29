using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DipScooper.UI
{
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// SettingsForm allows users to adjust the threshold values for different types of dips (Normal Dip, Big Dip, and SUPER-DIP).
        ///
        /// 1. **Saving Settings For Threshold Values**:
        ///    - When the user clicks the "Save" button (`btnSave`), the `btnSave_Click` method is triggered.
        ///    - This method reads the values from the text boxes (`txtNormalDipThreshold`, `txtBigDipThreshold`, `txtSuperDipThreshold`), parses them to integers, and saves them to application settings (`Properties.Settings.Default`).
        ///    - After saving, it triggers the `SettingsSaved` event to notify other parts of the application that the settings have been updated, then closes the form.
        ///
        /// 2. **Loading Settings**:
        ///    - When the form is loaded (`SettingsForm_Load`), it retrieves the current threshold values from application settings and populates the text boxes with these values.
        ///
        /// **Connections to other classes**:
        /// - `Form1`: The main form listens to the `SettingsSaved` event to update the dip signals in the data grid based on the new thresholds.
        /// </summary>

        public event EventHandler SettingsSaved;

        public SettingsForm()
        {
            InitializeComponent();
        }



        private void SettingsForm_Load(object sender, EventArgs e)
        {
            txtNormalDipThreshold.Text = Properties.Settings.Default.NormalDipThreshold.ToString();
            txtBigDipThreshold.Text = Properties.Settings.Default.BigDipThreshold.ToString();
            txtSuperDipThreshold.Text = Properties.Settings.Default.SuperDipThreshold.ToString();
        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                // Lagre innstillingene når brukeren trykker på lagre-knappen
                int normalDipThreshold = int.Parse(txtNormalDipThreshold.Text);
                int bigDipThreshold = int.Parse(txtBigDipThreshold.Text);
                int superDipThreshold = int.Parse(txtSuperDipThreshold.Text);

                Properties.Settings.Default.NormalDipThreshold = normalDipThreshold;
                Properties.Settings.Default.BigDipThreshold = bigDipThreshold;
                Properties.Settings.Default.SuperDipThreshold = superDipThreshold;
                Properties.Settings.Default.Save();

                Debug.WriteLine($"Settings saved: NormalDipThreshold={normalDipThreshold}, BigDipThreshold={bigDipThreshold}, SuperDipThreshold={superDipThreshold}");

                SettingsSaved?.Invoke(this, EventArgs.Empty);

                Debug.WriteLine("Invoking SettingsSaved event.");

                this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving settings: {ex.Message}");
                MessageBox.Show("Error saving settings. Please check the values and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
