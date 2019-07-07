using System;
using System.Windows;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace desktop_dotnet.win32
{
    public class BehaviourMonitor
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        public string GetActiveProcessFileName()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return p.MainModule.FileName;
        }

        public Icon getIcon(string path)
        {
            return Icon.ExtractAssociatedIcon(path);
        }

        public Bitmap getScreenshot()
        {
            Rectangle bounds = new Rectangle(0, 0, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight);
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(
                    new System.Drawing.Point((int)SystemParameters.VirtualScreenLeft, (int)SystemParameters.VirtualScreenTop),
                    System.Drawing.Point.Empty, bounds.Size);
            }
            return bitmap;
        }

        public DateTime GetLastInputTime()
        {
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            GetLastInputInfo(ref lastInputInfo);

            return DateTime.Now.AddMilliseconds(
                -(Environment.TickCount - lastInputInfo.dwTime));
        }
    }
}
