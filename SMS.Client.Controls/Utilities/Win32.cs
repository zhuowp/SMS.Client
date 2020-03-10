using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SMS.Client.Controls
{
    class Win32
    {
        [DllImport("user32.dll", SetLastError = true)] 
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint); 
    }
}
