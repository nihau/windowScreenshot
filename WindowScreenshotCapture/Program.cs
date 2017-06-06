using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace WindowScreenshotCapture
{
    class Program
    {
        private static string _folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Screenshots");

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
                
                try
                {
                    var screenShotName = Path.Combine(_folder, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));

                    ScreenCapturer.CaptureAndSave($"{screenShotName}");

                    Console.WriteLine($"Successfully created screenshot {screenShotName}");
                }
                catch
                {
                    Console.WriteLine($"Failed to save screenshot");

                    throw;
                }
            }
        }
    }
}
