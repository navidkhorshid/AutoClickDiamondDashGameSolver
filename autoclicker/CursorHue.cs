using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace autoclicker
{
    class CursorHue
    {
        [DllImport("gdi32")]
        public static extern uint GetPixel(IntPtr hDC, int XPos, int YPos);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        public static float GetHue(Point p)
        {
            IntPtr dc = GetWindowDC(IntPtr.Zero);

            long color = GetPixel(dc, p.X, p.Y);
            Color cc = Color.FromArgb((int)color);
            return cc.GetHue();
            
        }

        public static bool AreSimilar(Point p1, Point p2, float tolerance = 15f) 
        {
            return Math.Abs(GetHue(p1) - GetHue(p2)) <= tolerance;
        }
    }
}

//public static bool AreSimilar(Color c1, Color c2, float tolerance = 15f)
//{
//  return Math.Abs(c1.GetHue() - c2.GetHue()) <= tolerance;
//}