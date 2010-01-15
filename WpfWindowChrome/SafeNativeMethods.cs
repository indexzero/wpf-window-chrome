using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WpfWindowChrome
{
    public static class SafeNativeMethods
    {
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        internal const int WS_CHILD = 0x40000000;
        internal const int WS_VISIBLE = 0x10000000;
        internal const int LBS_NOTIFY = 0x00000001;
        internal const int HOST_ID = 0x00000002;
        internal const int LISTBOX_ID = 0x00000001;
        internal const int WS_VSCROLL = 0x00200000;
        internal const int WS_BORDER = 0x00800000;


        public const int GWL_EXSTYLE = -20;
        public const int GWL_STYLE = -16;
        public const int WS_EX_DLGMODALFRAME = 0x0001;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const uint WM_SETICON = 0x0080;
        public const int WS_DLGFRAME = 0x00400000;

        public const int WS_SYSMENU = 0x80000;
        public const int WS_MINIMIZEBOX = 0x20000;
        public const int WS_MAXIMIZEBOX = 0x10000;
    }
}
