using FSRAudioSwitcher.Properties;
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
    public partial class ComPortPicker : Form
    {
        private FSRAudioSwitcher _parentApp;
        public ComPortPicker(FSRAudioSwitcher parentApp)
        {
            InitializeComponent();
            _parentApp = parentApp;
        }

        private List<RadioButton> _comPorts = new List<RadioButton>();
        List<string> _ports;

        private void Form1_Load(object sender, EventArgs e)
        {

            panel.ColumnCount = 2;
            panel.RowCount = 1;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            panel.Controls.Add(new Label() { Text = "COM port" }, 0, 0);
            panel.Controls.Add(new Label() { Text = "#" }, 1, 0);

            _ports = FSRAudioSwitcher.GetPortNames();
            foreach(string port in _ports)
            {
                RadioButton comPortRadio = new RadioButton()
                {
                    AutoCheck = false,
                    Name = port,
                    Checked = FSRAudioSwitcher._portName == port
                };

                _comPorts.Add(comPortRadio);
                panel.RowCount = panel.RowCount + 1;
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
                panel.Controls.Add(new Label() { Text = port }, 0, panel.RowCount - 1);
                panel.Controls.Add(comPortRadio, 1, panel.RowCount - 1);
            }
        }

        private void Button1_Click(object sender, EventArgs e) // OK
        {
            string comPortDev = _comPorts[_comPorts.FindIndex(o => o.Checked)].Name;

            Console.WriteLine("setting new com port: " + comPortDev);

            FSRAudioSwitcher._portName = comPortDev;

            _parentApp.SaveSettings();

            this.Dispose();
            this.Close();
        }

        private void Button2_Click_1(object sender, EventArgs e) // Cancel
        {
            this.Dispose();
            this.Close();
        }
    }
}
