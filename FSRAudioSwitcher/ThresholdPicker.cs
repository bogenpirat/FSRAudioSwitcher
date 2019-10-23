using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FSRAudioSwitcher
{
    public partial class ThresholdPicker : Form
    {
        FSRAudioSwitcher _parentApp;
        public ThresholdPicker(FSRAudioSwitcher parentApp)
        {
            InitializeComponent();
            _parentApp = parentApp;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void ComPortPicker_Load(object sender, EventArgs e)
        {
            thresholdNumber.Value = FSRAudioSwitcher._threshold;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            FSRAudioSwitcher._threshold = Convert.ToInt32(thresholdNumber.Value);
            _parentApp.SaveSettings();

            this.Dispose();
            this.Close();
        }

        private void PickButton_Click(object sender, EventArgs e)
        {
            if(CurrentMeasurementBox.Text.Length > 0)
            {
                thresholdNumber.Value = Convert.ToDecimal(CurrentMeasurementBox.Text) * ((decimal)1.20);
            }
        }

        private void CurrentMeasurementBox_Click(object sender, EventArgs e)
        {

        }

        public TextBox GetCurrentMeasurementField()
        {
            return CurrentMeasurementBox;
        }
    }
}
