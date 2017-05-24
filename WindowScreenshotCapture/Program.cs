using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowScreenshotCapture
{
    class Program
    {
        private static string _folder = @"D:\Screenshots\";

        static void Main(string[] args)
        {
            new KeyHook(OnKeyPress);

            Console.ReadLine();
        }

        public static void OnKeyPress(object sender, KeyCallBack e)
        {
            if (e.Key == Keys.PrintScreen 
                && e.Modifiers.HasFlag(Keys.ControlKey)
                && e.Modifiers.HasFlag(Keys.Menu))
            {
                if (!Directory.Exists(_folder))
                    Directory.CreateDirectory(_folder);

                ScreenCapturer.CaptureAndSave($"{Path.Combine(_folder, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"))}");
            }
        }
    }
}
