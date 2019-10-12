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
    public partial class DevicePicker : Form
    {
        private FSRAudioSwitcher _parentApp;
        public DevicePicker(FSRAudioSwitcher parentApp)
        {
            InitializeComponent();
            _parentApp = parentApp;
        }

        List<RadioButton> _headphonesRadios;
        List<RadioButton> _speakersRadios;
        List<(string, string)> _devices;

        private void Form1_Load(object sender, EventArgs e)
        {
            PictureBox headphonesBox = new PictureBox()
            {
                Image = Resources.headphone,
                BackColor = Color.Transparent,
                Parent = panel,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            PictureBox speakersBox = new PictureBox()
            {
                Image = Resources.speakers,
                BackColor = Color.Transparent,
                Parent = panel,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            _headphonesRadios = new List<RadioButton>();
            _speakersRadios = new List<RadioButton>();

            panel.ColumnCount = 3;
            panel.RowCount = 1;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            panel.Controls.Add(new Label() { Text = "Device Name" }, 0, 0);
            panel.Controls.Add(headphonesBox, 1, 0);
            panel.Controls.Add(speakersBox, 2, 0);

            _devices = FSRAudioSwitcher.GetAllAudioDeviceNames();
            foreach((string, string) dev in _devices)
            {
                RadioButton headphonesRadio = new RadioButton()
                {
                    AutoCheck = false,
                    Name = "HeadphonesSelection",
                    Checked = FSRAudioSwitcher._headphonesID == dev.Item2
                };
                RadioButton speakersRadio = new RadioButton()
                {
                    AutoCheck = false,
                    Name = "SpeakersSelection",
                    Checked = FSRAudioSwitcher._speakersID == dev.Item2
                };

                _headphonesRadios.Add(headphonesRadio);
                _speakersRadios.Add(speakersRadio);

                headphonesRadio.Click += (o, ev) => { headphonesRadio.Checked = true; };
                headphonesRadio.CheckedChanged += (o, ev) =>
                {
                    if(headphonesRadio.Checked)
                    {
                        _headphonesRadios.ForEach(rb => rb.Checked = rb == headphonesRadio);
                    }
                };
                speakersRadio.Click += (o, ev) => { speakersRadio.Checked = true; };
                speakersRadio.CheckedChanged += (o, ev) =>
                {
                    if (speakersRadio.Checked)
                    {
                        _speakersRadios.ForEach(rb => rb.Checked = rb == speakersRadio);
                    }
                };

                panel.RowCount = panel.RowCount + 1;
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
                panel.Controls.Add(new Label() { Text = dev.Item1 }, 0, panel.RowCount - 1);
                panel.Controls.Add(headphonesRadio, 1, panel.RowCount - 1);
                panel.Controls.Add(speakersRadio, 2, panel.RowCount - 1);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) // OK
        {
            (string, string) headphonesDev = _devices[_headphonesRadios.FindIndex(o => o.Checked)];
            (string, string) speakersDev = _devices[_speakersRadios.FindIndex(o => o.Checked)];

            Console.WriteLine("setting new headphones: " + headphonesDev.Item1);
            Console.WriteLine("setting new speakers: " + speakersDev.Item1);

            FSRAudioSwitcher._headphones = headphonesDev.Item1;
            FSRAudioSwitcher._headphonesID = headphonesDev.Item2;

            FSRAudioSwitcher._speakers = speakersDev.Item1;
            FSRAudioSwitcher._speakersID = speakersDev.Item2;

            _parentApp.SaveSettings();

            this.Dispose();
            this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e) // Cancel
        {
            this.Dispose();
            this.Close();
        }
    }
}
