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
namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        string motors = "";
        //"value" used as incoming serial data buffer
        string value = "";
        string y = "";
        string z = "";
        int ypos = 1;
        int zpos = 1;
        int mLeft = 0;
        int mRight = 0;
        private string inBuffer = String.Empty;
        //Default value to use for Y and Z values from IMU in case data is not sent through properly
        string valueActual = "0.0000!0.0000\n";
        int trackbarcount = 1;
        public Form1()
        {
            InitializeComponent();
            //Create a timer that will periodcally send new data to
            trackBar1.Maximum = 100;
            Timer t1 = new Timer();
            t1.Interval = 250;
            t1.Tick += new EventHandler(t1_Tick);
            t1.Start();
 
 
            serialPort1.DataReceived += robot;
            //Open both serial ports
            //Serial Port 1 for incoming "wheel" data
            serialPort1.Open();
            //Serial Port 2 for outgoing bluetooth serial data to robot
            serialPort2.Open();
        }
        private void t1_Tick(object sender, EventArgs e)
        {
            //Parse incoming serial data into Y and Z components for IMU
            int position = valueActual.IndexOf("!");
            y = valueActual.Substring(0, position);
            z = valueActual.Substring(position + 1, valueActual.Length - y.Length-2);
            //Convert serial data to integers between 0 and 90 for zpos, and -90 to 90 for ypos
            ypos = Convert.ToInt32(100*Convert.ToDouble(y));
            zpos = Convert.ToInt32(100*Convert.ToDouble(z));
            //Adjuct trackbar values for steering and acceleration based on Y and Z data
            trackBar1.Value = Math.Abs(zpos);
            trackBar2.Value = -ypos;
            //Calculate appropriate motor speeds based on ypos and zpos
 
            //Case: Move directly forward
            if(ypos==0)
            {
                mLeft = zpos;
                mRight = zpos;
            }
                //Case: Turn Right
            else if(ypos<0)
            {
                mLeft=zpos;
                mRight=10*zpos/(-ypos);
            }
                //Case: Turn Left
            else
            {
                mRight = zpos;
                mLeft = 10*zpos / (ypos);
            }
            //Format motor data into a 4 character string to be written out to robot
            if (mRight<10 && mLeft<10)
            {
                motors = "0"+mLeft.ToString()  +"0"+ mRight.ToString();
            }
            else if ((mRight < 10))
            {
                motors = mLeft.ToString()  + "0" + mRight.ToString();
            }
            else if (mLeft < 10)
            {
                motors = "0" + mLeft.ToString() +  mRight.ToString();
            }
            else
            {
                motors = mLeft.ToString() +  mRight.ToString();
            }
            //Write out motor data to robot via Bleutooth
            serialPort2.Write(motors);
            
        }
 
 
        //
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
 
 
            trackBar1.Value = 20;
         
        }
 
        //Read in serial data until newline character
        void robot(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort serialPort1 = sender as SerialPort;
 
            value = value+serialPort1.ReadExisting();
            if((value[value.Length-1])=='\n')
            {   //Throw out any "Bad" values, if serial data isn't sent received properly
                if ((value.Length > 16) && (value.Length < 23))
                {
                    //If data is valid, IMU data will be updated. Otherwise previous value
                    //for IMU data will be used.
                    valueActual = value;
          
                }
                //Clear input serial data buffer
                value = "";
            }
  
 
        }
 
        //Create labels for both of the trackbars.
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = "Steering";
        }
 
    }
 
}
