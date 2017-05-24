using System;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace WindowScreenshotCapture
{
    public struct KeyCallBack
    {
        public Keys Key { get; set; }
        public Keys Modifiers { get; set; }
    }

    public class KeyHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private IntPtr _hookID = IntPtr.Zero;

        private EventHandler<KeyCallBack> _callback;

        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;
        private const int VK_CAPITAL = 0x14;

        private Dictionary<int, int> _modifiersMap = new Dictionary<int, int>
        {
            { VK_SHIFT,     0x8000 },
            { VK_CONTROL,   0x8000 },
            { VK_MENU,      0x8000 },
            { VK_CAPITAL,   0x0001 },
        };

        public KeyHook(EventHandler<KeyCallBack> callback)
        {
            _callback = callback;
            _hookID = SetHook(HookCallback);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                var key = (Keys)vkCode;

                var modifiers = Keys.None;

                foreach (var pressedModifier in _modifiersMap.Where(kvp => (GetKeyState(kvp.Key) & kvp.Value) != 0)
                    .Select(kvp => (Keys)kvp.Key))
                {
                    modifiers |= pressedModifier;
                }

                _callback.Invoke(this, new KeyCallBack { Key = key, Modifiers = modifiers});
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
    }
}