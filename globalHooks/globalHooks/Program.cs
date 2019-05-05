using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
// Explicitly add WindowsBase.dll reference in Console App
using System.Windows.Input;

namespace globalHooks
{
    public class KeyPressedArgs : EventArgs
    {
        public Key KeyPressed { get; private set; }

        public KeyPressedArgs(Key key)
        {            
            KeyPressed = key;
        }
    }
    public class globalHookHelper
    {
        public delegate IntPtr HookDelegate(
            Int32 Code,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            Int32 nCode,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr UnhookWindowsHookEx(
            IntPtr hhk);

        [DllImport("User32.dll")]
        public static extern IntPtr SetWindowsHookEx(
            Int32 idHook,
            HookDelegate lpfn,
            IntPtr hMod,
            Int32 dwThreadId);
    }

    public class keyboardHook : IDisposable
    {
        private globalHookHelper.HookDelegate kbHookDelegate;
        private IntPtr kbHookHandle = IntPtr.Zero;
        private const Int32 WH_KEYBOARD_LL = 13;
        private const Int32 WM_KEYDOWN = 0x0100;
        private const Int32 WM_SYSKEYDOWN = 0x0104;
        private bool disposed = false;

        public event EventHandler<KeyPressedArgs> KeyPressEvent;

        private IntPtr KeyboardHookDelegate(
            Int32 nCode, IntPtr wParam, IntPtr lParam)
        {
            if(nCode >=0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if(null != KeyPressEvent)
                {
                    KeyPressEvent(this, new KeyPressedArgs(KeyInterop.KeyFromVirtualKey(vkCode)));
                }
            }            

            return globalHookHelper.CallNextHookEx(
                kbHookHandle, nCode, wParam, lParam);
        }

        public keyboardHook()
        {
            kbHookDelegate = KeyboardHookDelegate;
            kbHookHandle = globalHookHelper.SetWindowsHookEx(
                WH_KEYBOARD_LL, kbHookDelegate, IntPtr.Zero, 0);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(kbHookHandle != IntPtr.Zero)
                {
                    globalHookHelper.UnhookWindowsHookEx(kbHookHandle);
                }
                disposed = true;
            }
        }

        ~keyboardHook()
        {
            Dispose(false);
        }

    }
    class Program
    {
        
        static void Main(string[] args)
        {
            ConsoleKeyInfo key;
            keyboardHook keyboard = new keyboardHook();
            keyboard.KeyPressEvent += Keyboard_KeyPressEvent;
            do
            {
                key = Console.ReadKey();
            }
            while (key.Key != ConsoleKey.X);
        }

        private static void Keyboard_KeyPressEvent(object sender, KeyPressedArgs e)
        {
            Console.Write(e.KeyPressed.ToString());
        }
    }
}
