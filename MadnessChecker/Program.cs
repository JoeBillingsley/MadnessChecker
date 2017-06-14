using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace MadnessChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new System.Threading.Timer(
                e =>
                {
                    using (WebClient client = new WebClient())
                    {
                        var htmlCode = client.DownloadString("https://www.exeterguild.org/festival/tickets/");

                        var indexOfSpan = htmlCode.IndexOf("ctl00_Madness_lblDiscountPrice") + 48;
                        var priceString = htmlCode.Substring(indexOfSpan, 5);

                        var price = double.Parse(priceString);

                        Console.WriteLine("Boo. The ticket is £" + price);

                        if (price < 32)
                        {
                            Console.WriteLine("LOOK! LOOK! The ticket is only £" + price);
                            FlashWindow(Process.GetCurrentProcess().MainWindowHandle);
                        }
                    }
                },
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            Console.Read();
        }

        // Stolen from stack overflow: https://stackoverflow.com/questions/2923250/is-there-a-way-to-make-a-console-window-flash-in-the-task-bar-programatically
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public Int32 dwTimeout;
        }

        public const UInt32 FLASHW_ALL = 3;

        private static void FlashWindow(IntPtr hWnd)
        {
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_ALL;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            FlashWindowEx(ref fInfo);
        }
    }
}
