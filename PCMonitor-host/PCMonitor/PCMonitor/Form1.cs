using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using OpenHardwareMonitor.Hardware;


namespace PCMonitor
{
    public partial class Form1 : Form
    {
        SerialPort port;

        OHWWrapper PCMonitorObject;

        LiquidCrystal LCD;
        
        public Form1()
        {
            /* Init OpenHardwareMonitor object */
            PCMonitorObject = new OHWWrapper();
            port = new SerialPort();
            
            LCD = new LiquidCrystal(port);

            InitializeComponent();

            PCMonitorObject.Init(portsCombo, port, checkBoxAutoConnect, timer1);
            
            if(timer1.Enabled == true)
            {
                groupBoxLCD.Enabled = true;
                LCD.Connect();
                trackBar1.Value = 120;
                connectBtn.Enabled = false;
                disconnectBtn.Enabled = true;
            }

            if (checkBoxAutoConnect.Enabled == false)
            {
                toolStripLabel.Text = "Initialized.";
            }
            else
            {
                toolStripLabel.Text = "Connected.";
            }
        }


        #region CONNECT BUTTON
        private void connectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if(!port.IsOpen)
                {
                    port.PortName = portsCombo.Text;
                    port.Open();
                    //TODO: setup
                    timer1.Interval = 1000;
                    timer1.Enabled = true;
                    connectBtn.Enabled = false;
                    checkBoxAutoConnect.Enabled = false;
                    disconnectBtn.Enabled = true;
                    //PCMonitorObject.updateTreeView(treeView1);
                    toolStripLabel.Text = "Build up tree...";
                    LCD.Connect();
                    groupBoxLCD.Enabled = true;
                    trackBar1.Value = 120;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region DISCONNECT BUTTON
        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (port.IsOpen == true)
                {
                    LCD.Disconnect();
                    port.Close();
                }
                timer1.Enabled = false;
                toolStripLabel.Text = "Disconnected.";
                disconnectBtn.Enabled = false;
                checkBoxAutoConnect.Enabled = true;
                connectBtn.Enabled = true;
                groupBoxLCD.Enabled = false;
            }
            catch(Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }
        }
        #endregion 

        #region TIMER HANDLER
        private void timer1_Tick(object sender, EventArgs e)
        {
            //PCMonitorObject.updateTreeView(treeView1);
            PCMonitorObject.Status();
            LCD.BuildScreen();
            toolStripLabel.Text = "Updated data at: " + DateTime.Now.ToString("YYYY.MM.DD. (HH.MM.SS)");
        }
        #endregion

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            LCD.SetBacklight((uint)trackBar1.Value);
            toolStripLabel.Text = ("LCD backlight changed: " + (uint)trackBar1.Value + "\\" + trackBar1.Maximum.ToString());
        }





    }
}
