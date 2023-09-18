using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace MonKey.Internal
{
    //big thanks to keyboardP
    //https://stackoverflow.com/questions/17579658/
    //how-to-intercept-all-the-keyboard-events-and-prevent-losing-focus-in-a-winforms
    internal class KeyboardHook
    {
        private const int WhKeyboardLL = 13;
        private const int WMKeyDown = 0x0100;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private readonly IntPtr hookID = IntPtr.Zero;

   

        public KeyboardHook()
        {
            UnhookWindowsHookEx(hookID);
            hookID = SetHook(HookCallback);
        }

        ~KeyboardHook()
        {
            UnhookWindowsHookEx(hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WhKeyboardLL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {

            try
            {
                if (nCode >= 0 && wParam == (IntPtr)WMKeyDown)
                {
                    int vkCode = Marshal.ReadInt32(lParam);

                     WindowsKeyCode code = (WindowsKeyCode) vkCode;
                    UnityEngine.Debug.Log(code);
                }
                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }
            catch (Exception)
            {
                UnityEngine.Debug.Log("Failed to parse the key");

                return CallNextHookEx(hookID, nCode, wParam, lParam);
            }
        }
    }

    //Copy from windows form to not have to use a reference to it
    public enum WindowsKeyCode
    {
        LEFT_MOUSE,
        RIGHT_MOUSE,
        CONTROL_BREAK_PROCESSING,
        MIDDLE_MOUSE,
        X1_MOUSE,
        X2_MOUSE,
        UNDEFINED,
        BACKSPACE,
        TAB,
        RESERVED,
        CLEAR,
        ENTER,
        UNDEFINED2,
        SHIFT,
        CTRL,
        ALT,
        PAUSE,
        CAPS_LOCK,
        IME_KANA_MODE,
        IME_HANGUEL_MODE,
        IME_HANGUL_MODE,
        UNDEFINED3,
        IME_JUNJA_MODE,
        IME_FINAL_MODE,
        IME_HANJA_MODE,
        IME_KANJI_MODE,
        UNDEFINED4,
        ESC,
        IME_CONVERT,
        IME_NONCONVERT,
        IME_ACCEPT,
        IME_MODE_CHANGE_REQUEST,
        SPACEBAR,
        PAGE_UP,
        PAGE_DOWN,
        END,
        HOME,
        LEFT_ARROW,
        UP_ARROW,
        RIGHT_ARROW,
        DOWN_ARROW,
        SELECT,
        PRINT,
        EXECUTE,
        PRINT_SCREEN,
        INS,
        DEL,
        HELP,
        ZERO,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        UNDEFINED5,
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        LEFT_WINDOWS,
        RIGHT_WINDOWS,
        APPLICATIONS,
        RESERVED2,
        COMPUTER_SLEEP,
        NUMERICPAD_0,
        NUMERICPAD_1,
        NUMERICPAD_2,
        NUMERICPAD_3,
        NUMERICPAD_4,
        NUMERICPAD_5,
        NUMERICPAD_6,
        NUMERICPAD_7,
        NUMERICPAD_8,
        NUMERICPAD_9,
        MULTIPLY,
        ADD,
        SEPARATOR,
        SUBTRACT,
        DECIMAL,
        DIVIDE,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        F13,
        F14,
        F15,
        F16,
        F17,
        F18,
        F19,
        F20,
        F21,
        F22,
        F23,
        F24,
        UNASSIGNED6,
        NUM_LOCK,
        SCROLL_LOCK,
        OEM_SPECIFIC2,
        UNASSIGNED7,
        LEFT_SHIFT,
        RIGHT_SHIFT,
        LEFT_CONTROL,
        RIGHT_CONTROL,
        LEFT_MENU,
        RIGHT_MENU,
        BROWSER_BACK,
        BROWSER_FORWARD,
        BROWSER_REFRESH,
        BROWSER_STOP,
        BROWSER_SEARCH,
        BROWSER_FAVORITES,
        BROWSER_START_AND_HOME,
        VOLUME_MUTE,
        VOLUME_DOWN,
        VOLUME_UP,
        NEXT_TRACK,
        PREVIOUS_TRACK,
        STOP_MEDIA,
        PLAY_PAUSE_MEDIA,
        START_MAIL,
        SELECT_MEDIA,
        START_APPLICATION_1,
        START_APPLICATION_2,
        RESERVED3,
        SEMICOLON,
        PLUS,
        COMMA,
        MINUS,
        DOT,
        MISC1,
        MISC2,
        MISC3,
        MISC4,
        MISC5,
        MISC6,
        MISC7,
        MISC8,
        MISC9,
        RESERVED8,
        OEM_SPECIFIC3,
        ANGLE_BRACKET,
        OEM_SPECIFIC4,
        IME_PROCESS,
        OEM_SPECIFIC5,
        NONE,
        UNASSIGNED9,
        OEM_SPECIFI6,
        ATTN,
        CR_SEL,
        EX_SEL,
        ERASE_EOF,
        PLAY,
        ZOOM,
        RESERVED4,
        PA1,
        CLEAR2,
    }

}




