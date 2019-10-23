using FSRAudioSwitcher.Properties;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using NAudio.CoreAudioApi;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Collections.Generic;

namespace  FSRAudioSwitcher { 
    public class FSRAudioSwitcher : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private readonly Icon onIcon = new Icon(Resources.speaker, 40, 40);
        private readonly Icon offIcon = new Icon(Resources.speakerOff, 40, 40);

        DevicePicker _devicePicker;
        volatile ThresholdPicker _thresholdPicker;
        ComPortPicker _comPortPicker;

        public FSRAudioSwitcher()
        {
            // experimentally determined threshold for triggering. in ohms.
            // any less than this and the application thinks that the headphones
            // are hung on the stand, meaning we can switch to the speakers.
            _threshold = Settings.Default.switchingThreshold;

            _speakers = Settings.Default.speakersName;
            _speakersID = Settings.Default.speakersID;
            _headphones = Settings.Default.headphonesName;
            _headphonesID = Settings.Default.headphonesID;
            _doHandleSwitching = Settings.Default.doHandleSwitching;

            trayMenu = new ContextMenu();
            trayIcon = new NotifyIcon();
            SetTrayIcon(); // icon and text
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            trayIcon.ContextMenu.Popup += GenerateContextMenu;
            trayIcon.MouseUp += new MouseEventHandler(TrayIcon_LeftClick);
        }

        static bool _continue;
        static SerialPort _serialPort;
        static int _currentMeasurement;

        delegate void SafeCallDelegate(int measurement);

        public static string _portName { get; set; }
        public static int _threshold { get; set; }
        public static string _speakers { get; set; }
        public static string _speakersID { get; set; }
        public static string _headphones { get; set; }
        public static string _headphonesID { get; set; }
        public static string _currentAudioDevice;
        public static string _currentAudioDeviceID;
        static bool _doHandleSwitching;

        [STAThread]
        public static void Main()
        {
            _continue = true;

            // grab the currently active audio device
            UpdateCurrentDefaultAudioDevice();

            FSRAudioSwitcher application = new FSRAudioSwitcher();
            
            Thread readThread = new Thread(application.COMRead);

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();

            // check port setup
            _portName = GetPortName(Settings.Default.comPortName);
            application.SaveSettings();
            
            _serialPort.BaudRate = 9600;

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 2000;
            _serialPort.WriteTimeout = 2000;
        
            readThread.Start();

            Application.Run(application);

            readThread.Join();
            _serialPort.Close();
        }

        /**
         * read lines from the com port. these should contain integer values indicating
         * the force sensitive resistor's current resistance.
         * 
         * without any force exerted on the sensor, the resistance on the sensor is
         * infinite (FSR-406: >10M Ohms). as the force increases, its resistance decreases.
         * 
         * experimentally, a beyerdynamic dt 770 pro (somewhere around 300g, so ~3kN of force)
         * lowers the resistance of the FSR down to about 2.1 kOhm. the change in resistance
         * is hyperbolic, so it's best to figure out a threshold resistance to trigger for.
         */
        public void COMRead()
        {
            _serialPort.PortName = _portName;
            // open the port; start reading loop
            try
            {
                _serialPort.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("No COM ports available. Attach the hardware.");
                Application.Exit();
                return;
            }

            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine().Trim();
                    //Console.WriteLine("rawsistance: {0}", message);

                    if ("inf".Equals(message))
                    {
                        message = Int32.MaxValue + "";
                    }

                    int currResistance = Int32.Parse(message);

                    _currentMeasurement = currResistance;
                    SetCurrentMeasurementField(_currentMeasurement);
                    //Console.WriteLine("resistance: {0}", currResistance);

                    if (_doHandleSwitching) {
                        if (currResistance < _threshold && _currentAudioDeviceID != _speakersID)
                        {
                            AudioSwitch(_speakersID);
                        }
                        else if(currResistance >= _threshold && _currentAudioDeviceID != _headphonesID)
                        {
                            AudioSwitch(_headphonesID);
                        }
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Exited due to a read timeout.");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Couldn't parse resistance value into an integer.");
                }
                catch(InvalidOperationException)
                {
                    Console.WriteLine("COM port is not accessible");
                    return;
                }
            }
        }

        private void SetCurrentMeasurementField(int currentMeasurement)
        {
            if (_thresholdPicker != null)
            {
                var field = _thresholdPicker.GetCurrentMeasurementField();
                if (field.InvokeRequired)
                {
                    var d = new SafeCallDelegate(SetCurrentMeasurementField);
                    field.Invoke(d, new object[] { _currentMeasurement });
                } else
                {
                    field.Text = _currentMeasurement + "";
                }
            }
        }

        /**
         * switches the standard audio device using the undocumented COM API
         */
        public void AudioSwitch(string AudioDeviceID)
        {
            AudioSwitchCOM(AudioDeviceID);
            UpdateCurrentDefaultAudioDevice();
            SetTrayIcon();
        }

        /**
         * switches the standard audio device using the undocumented COM API
         */
        public static void AudioSwitchCOM(string audioDeviceID)
        {
            ChangeAudioDeviceByID(new StringBuilder(audioDeviceID));
        }

        [DllImport("EndPointController.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ChangeAudioDevice")]
        static extern int ChangeAudioDevice(int option);

        [DllImport("EndPointController.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ChangeAudioDeviceByName", CharSet = CharSet.Unicode)]
        static extern void ChangeAudioDeviceByName(StringBuilder DevName);

        [DllImport("EndPointController.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ChangeAudioDeviceByID", CharSet = CharSet.Unicode)]
        static extern void ChangeAudioDeviceByID(StringBuilder DevID);

        /**
         * switches the standard audio device using NirCmd
         */
        public static void AudioSwitchNirCmd(string AudioDeviceName)
        {
            Console.WriteLine("tasked with switching to {0}", AudioDeviceName);
            UpdateCurrentDefaultAudioDevice();
            Console.WriteLine("current device: {0}", _currentAudioDevice);

            if (AudioDeviceName != _currentAudioDevice)
            {
                Console.WriteLine("switching to audio device {0}", AudioDeviceName);

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = Settings.Default.nirCmdExecutable,
                    Arguments = String.Format("setdefaultsounddevice \"{0}\" 1", AudioDeviceName)
                };
                process.StartInfo = startInfo;
                process.Start();
            }
        }

        /** 
         * gets the currently active default audio device's name from naudio
         */
        public static void UpdateCurrentDefaultAudioDevice()
        {
            var enumerator = new MMDeviceEnumerator();
            MMDevice defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            _currentAudioDevice = defaultDevice.Properties.GetValue(1).Value.ToString();
            _currentAudioDeviceID = defaultDevice.ID;
            Console.WriteLine("Checked default audio interface: " + _currentAudioDevice);
        }

        public static List<(string, string)> GetAllAudioDeviceNames()
        {
            List<(string, string)> list = new List<(string, string)>();
            var enumerator = new MMDeviceEnumerator();
            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                list.Add(( device.Properties.GetValue(1).Value.ToString(), device.ID ));
            }

            return list;
        }

        public void SetHeadphonesDevice((string, string) Dev)
        {
            _headphones = Dev.Item1;
            _headphonesID = Dev.Item2;

            SaveSettings();
        }

        public void SetSpeakersDevice((string, string) Dev)
        {
            _speakers = Dev.Item1;
            _speakersID = Dev.Item2;

            SaveSettings();
        }

        /**
         * takes a default value, checks whether this is a valid com port, then sets it,
         * or, if it is not, checks which com ports are available and uses the first one.
         */
        public static string GetPortName(string defName = null)
        {
            string portName = null;

            if (Array.IndexOf(SerialPort.GetPortNames(), defName) > -1)
            {
                portName = defName;
            }

            Console.WriteLine("Available COM ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine(" {0}", s);
                if (portName == null)
                {
                    portName = s;
                }
            }

            Console.WriteLine("Selected port: {0}", portName);

            return portName;
        }

        /**
         * takes a default value, checks whether this is a valid com port, then sets it,
         * or, if it is not, checks which com ports are available and uses the first one.
         */
        public static List<string> GetPortNames()
        {
            List<string> list = new List<string>();

            list.AddRange(SerialPort.GetPortNames());
            
            return list;
        }

        #region ui crap

        private void TrayIcon_LeftClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (_doHandleSwitching)
                {
                    // disable handling
                    case true:
                        _doHandleSwitching = false;
                        break;

                    // enable handling
                    case false:
                    default:
                        _doHandleSwitching = true;
                        break;
                }

                SetTrayIcon();
            }

            SaveSettings();
        }

        private void SetTrayIcon()
        {
            if (_continue)
            {
                if (_doHandleSwitching)
                {
                    // disable handling
                    trayIcon.Text = "Active device: " + _currentAudioDevice;
                    trayIcon.Icon = onIcon;
                } else
                {
                    // enable handling
                    trayIcon.Text = "[OFF] Active device: " + _currentAudioDevice;
                    trayIcon.Icon = offIcon;
                }
            }
        }

        private void GenerateContextMenu(object sender, EventArgs e)
        {
            // Empty menu to prevent stuff to pile up
            trayMenu.MenuItems.Clear();

            // display current operation status
            MenuItem _statusItem = new MenuItem { Enabled = false, Text = _doHandleSwitching ? "[ON]" : "[OFF]" };
            trayMenu.MenuItems.Add(_statusItem);

            trayMenu.MenuItems.Add("-");


            // add knobs for changing settings
            MenuItem _selectDevicesItem = new MenuItem { Text = "Select devices" };
            _selectDevicesItem.Click += delegate (object send, EventArgs ea) {
                _devicePicker = new DevicePicker(this);
                _devicePicker.ShowDialog();
            };
            trayMenu.MenuItems.Add(_selectDevicesItem);

            MenuItem _selectCOMPortItem = new MenuItem { Text = "Select COM port" };
            _selectCOMPortItem.Click += delegate (object send, EventArgs ea) {
                _comPortPicker = new ComPortPicker(this);
                _comPortPicker.ShowDialog();
            };
            trayMenu.MenuItems.Add(_selectCOMPortItem);

            MenuItem _selectSwitchingThresholdItem = new MenuItem { Text = "Select switching threshold" };
            _selectSwitchingThresholdItem.Click += delegate (object send, EventArgs ea) {
                _thresholdPicker = new ThresholdPicker(this);
                _thresholdPicker.ShowDialog();
            };
            trayMenu.MenuItems.Add(_selectSwitchingThresholdItem);


            trayMenu.MenuItems.Add("-");

            // output current settings
            MenuItem _measurementItem = new MenuItem { Enabled = false, Text = "Current measurement: " + _currentMeasurement.ToString() };
            trayMenu.MenuItems.Add(_measurementItem);
            MenuItem _comPortItem = new MenuItem { Enabled = false, Text = "COM port: " + _portName };
            trayMenu.MenuItems.Add(_comPortItem);

            trayMenu.MenuItems.Add("-");

            UpdateCurrentDefaultAudioDevice();
            // Add all active devices
            foreach ((string,string) dev in GetAllAudioDeviceNames())
            {
                var isInUse = _currentAudioDeviceID == dev.Item2;

                var item = new MenuItem { Checked = isInUse, Text = dev.Item1, Enabled = false };

                trayMenu.MenuItems.Add(item);
            }


            trayMenu.MenuItems.Add("-");
            // Add an exit button
            var exitItem = new MenuItem { Text = Resources.LABEL_EXIT };
            exitItem.Click += OnExit;
            trayMenu.MenuItems.Add(exitItem);
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        public void SaveSettings()
        {
            Settings.Default.doHandleSwitching = _doHandleSwitching;
            Settings.Default.headphonesName = _headphones;
            Settings.Default.headphonesID = _headphonesID;
            Settings.Default.speakersName = _speakers;
            Settings.Default.speakersID = _speakersID;
            Settings.Default.comPortName = _portName;
            Settings.Default.switchingThreshold = _threshold;

            Settings.Default.Save();
        }

        private void OnExit(object sender = null, EventArgs e = null)
        {
            SaveSettings();

            _continue = false;

            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        #endregion
    }
}