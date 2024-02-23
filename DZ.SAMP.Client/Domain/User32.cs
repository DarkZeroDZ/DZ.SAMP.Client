using System;
using System.Runtime.InteropServices;

namespace DZ.SAMP.Client.Domain
{
    public static class User32
    {
        [DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string cls, string win);
        [DllImport("user32")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32")]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32")]
        public static extern bool OpenIcon(IntPtr hWnd);
    }
}
