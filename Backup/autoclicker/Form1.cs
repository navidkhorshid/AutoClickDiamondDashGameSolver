using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace autoclicker
{
    public partial class Form1 : Form
    {
        //mouse event constants
        const int MOUSEEVENTF_LEFTDOWN = 2;
        const int MOUSEEVENTF_LEFTUP = 4;
        const int MOUSEEVENTF_RIGHTDOWN = 8;
        const int MOUSEEVENTF_RIGHT_UP = 16;
        //input type constant
        const int INPUT_MOUSE = 0;

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

        Point clickLocation;

        public Form1()
        {
            InitializeComponent();
            this.Text = "autoclicker " + clickLocation.ToString();
        }

        private void btnSetPoint_Click(object sender, EventArgs e)
        {
            timerPoint.Interval = 5000;
            timerPoint.Start();
        }

        private void timerPoint_Tick(object sender, EventArgs e)
        {

            clickLocation = Cursor.Position;
            this.Text = "autoclicker " + clickLocation.ToString();
            timerPoint.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //set cursor position to memorized location
            Cursor.Position = clickLocation;
            //set up the INPUT struct and fill it for the mouse down
            INPUT i = new INPUT();
            i.type = INPUT_MOUSE;
            i.mi.dx = 0; //clickLocation.X;
            i.mi.dy = 0; // clickLocation.Y;
            i.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            i.mi.dwExtraInfo = IntPtr.Zero;
            i.mi.mouseData = 0;
            i.mi.time = 0;
            //send the input
            SendInput(1, ref i, Marshal.SizeOf(i));
            //set the INPUT for mouse up and send
            i.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            SendInput(1, ref i, Marshal.SizeOf(i));
        }

        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        public struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        };

        private void btnStart_Click(object sender, EventArgs e)
        {
            //set appropriate timer interval - if user selected seconds or milliseconds
            if(rbMilliseconds.Checked)
                timer1.Interval = (int)numericUpDown1.Value;
            else
                timer1.Interval = (int)numericUpDown1.Value * 1000;

            if (!timer1.Enabled)
            {
                timer1.Start();
                this.Text = "autoclicker - started";
            }
            else
            {
                timer1.Stop();
                this.Text = "autoclicker - stopped";
            }
        }
    }
}