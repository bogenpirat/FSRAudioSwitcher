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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void ComPortPicker_Load(object sender, EventArgs e)
        {
            thresholdNumber.Value = FSRAudioSwitcher._threshold;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            FSRAudioSwitcher._threshold = Convert.ToInt32(thresholdNumber.Value);
            _parentApp.SaveSettings();

            this.Dispose();
            this.Close();
        }
    }
}
