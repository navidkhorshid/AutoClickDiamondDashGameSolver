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
        int t;
        int[,] grid = new int[9, 10];
        int[,] mark = new int[9, 10];
        int[,] dir = new int[4, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };
        int findthree = 1;
        Point TopLeft = new Point();
        Point DownRight = new Point();
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

        public Form1()
        {
            InitializeComponent();
            this.Text = "Diamond Dash";
        }

        private void colorgrid() 
        {
            for(int i = 0 ; i < 9 ; i++)
                for (int j = 0; j < 10; j++) 
                {
                    mark[i, j] = 0;
                    Point tmp = new Point();
                    tmp = (Point)listBox1.Items[i*10+j];
                    for (int k = 0; k < listBox2.Items.Count; k++) 
                    {
                        if(CursorHue.AreSimilar(tmp,(Point)listBox2.Items[k]))
                        {
                            grid[i, j] = k;
                            break;
                        }
                        else
                        {
                            grid[i, j] = 666;
                        }
                    }
                }
        }

        private void dfs(int a, int b, int diamondcolor) 
        {
            mark[a, b] = 1;
            for (int i = 0; i < 4; i++)
            {
                if(a + dir[i, 0] >= 0 && a + dir[i, 0] <= 8 && b + dir[i, 1] >= 0 && b + dir[i, 1] <= 9) 
                if (grid[a + dir[i, 0], b + dir[i, 1]] == diamondcolor && mark[a + dir[i, 0], b + dir[i, 1]] == 0 ) 
                {
                    findthree++;
                    dfs(a + dir[i, 0], b + dir[i, 1], diamondcolor);
                }
            }

        }

        private Point findconnecteddiamond() 
        {
            Point tmp = new Point(0,0);
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (mark[i, j] == 0 && grid[i, j] != 666)
                    {
                        findthree = 1;
                        dfs(i, j, grid[i, j]);
                        if (findthree >= 3) 
                        {
                            tmp = (Point)listBox1.Items[i*10+j];
                            return tmp;
                        }
                    }
                }

            }
            return tmp; 
        }

        void printgrid() 
        {
            textBox2.Clear();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    textBox2.Text += grid[i,j].ToString() + " ";  
                }
                textBox2.Text += Environment.NewLine;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if ((DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) - t > Convert.ToInt32(textBox1.Text) || (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) - t < 0)
            {
                timer1.Stop(); this.Text = "diamond dash - stopped";
            }
            //update the color matrice
            colorgrid();
            printgrid();
            //dfs and find the first 3 component
            //i have to select a a correct point to click and pass it to cursor.position
            Point p = new Point();
            p = findconnecteddiamond();//point
            if (p.X != 0 && p.Y != 0)
            {
                Cursor.Position = p;
                INPUT i = new INPUT();
                i.type = INPUT_MOUSE;
                i.mi.dx = 0;
                i.mi.dy = 0;
                i.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
                i.mi.dwExtraInfo = IntPtr.Zero;
                i.mi.mouseData = 0;
                i.mi.time = 0;
                SendInput(1, ref i, Marshal.SizeOf(i));
                i.mi.dwFlags = MOUSEEVENTF_LEFTUP;
                SendInput(1, ref i, Marshal.SizeOf(i));
            }
            //System.Threading.Thread.Sleep(1);
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
            if(rbMilliseconds.Checked)
                timer1.Interval = (int)numericUpDown1.Value;
            else
                timer1.Interval = (int)numericUpDown1.Value * 1000;

            if (!timer1.Enabled)
            {
                t = DateTime.Now.Hour * 3600 + DateTime.Now.Minute*60 + DateTime.Now.Second;

                timer1.Start();
                this.Text = "diamond dash - started";
            }
            else
            {
                timer1.Stop();
                this.Text = "diamond dash - stopped";
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('p'))
            {
                listBox1.Items.Add(Cursor.Position);
            }
            if (e.KeyChar.Equals('c'))
            {
                listBox2.Items.Add(Cursor.Position);
            }
            if (e.KeyChar.Equals('t'))
            {
                TopLeft = Cursor.Position;
            }
            if (e.KeyChar.Equals('d'))
            {
                DownRight = Cursor.Position;
            }
            else if (e.KeyChar.Equals('q'))
            {
                if (timer1.Enabled)
                {
                    timer1.Stop();
                    this.Text = "diamond dash - stopped";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox2.Items.Remove(listBox2.SelectedItem);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int x, y, dx, dy;
            x = DownRight.X - TopLeft.X;
            y = DownRight.Y - TopLeft.Y;
            dx = x / 10;// j's
            dy = y / 9; // i's
            Point lsttmp = new Point();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    lsttmp.X = TopLeft.X + (j * dx + (dx / 3));
                    lsttmp.Y = TopLeft.Y + (i * dy + (dy / 3));
                    listBox1.Items.Add(lsttmp);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox2.Items.Add((Point)listBox1.SelectedItem);
        }
    }
}