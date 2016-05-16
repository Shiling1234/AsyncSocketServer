using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace Client.Common
{
    public enum MouseEventFlag : uint
    {
        Move = 0x0001,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        XDown = 0x0080,
        XUp = 0x0100,
        Wheel = 0x0800,
        VirtualDesk = 0x4000,
        Absolute = 0x8000
    }
    public enum KeyEventFlag : int
    {
        Down = 0x0000,
        Up = 0x0002,
    }
    [SuppressUnmanagedCodeSecurityAttribute]
    public class MouseKeyOperate
    {
         //鼠标事件函数
        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        public static extern void mouse_event(MouseEventFlag dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
 
        //鼠标移动函数
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern int SetCursorPos(int x, int y);
 
        //键盘事件函数
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(Byte bVk, Byte bScan, KeyEventFlag dwFlags, Int32 dwExtraInfo);
 
        //定时器
        private System.Timers.Timer atimer = new System.Timers.Timer();
 
        //自动释放键值
        private Byte vbk;
 
        //初始化
        public MouseKeyOperate()
        {
 
            //设置定时器事件
            this.atimer.Elapsed += new ElapsedEventHandler(atimer_Elapsed);
            this.atimer.AutoReset = true;
        }
 
 
        //鼠标操作 _dx,_dy 是鼠标距离当前位置的二维移动向量
        public void mouse(MouseEventFlag _dwFlags,int _dx,int _dy)
        {
            mouse_event(_dwFlags, _dx, _dy, 0, 0);
        }

        public void MouseLeftDown(int x, int y)
        {
            mouse_event(MouseEventFlag.Absolute | MouseEventFlag.LeftDown, x, y, 0, 0);
        }

        public void MouseLeftUp(int x, int y)
        {
            mouse_event(MouseEventFlag.Absolute | MouseEventFlag.LeftUp, x, y, 0, 0);
        }

        public void MouseRightDown(int x, int y)
        {
            mouse_event(MouseEventFlag.Absolute | MouseEventFlag.RightDown, x, y, 0, 0);
        }

        public void MouseRightUp(int x, int y)
        {
            mouse_event(MouseEventFlag.Absolute | MouseEventFlag.RightUp, x, y, 0,0);
        }
 
        //鼠标操作
        public void mouse_click(string button="L",bool is_double=false)
         
        {
            switch (button){
                case "L": 
                    mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, 0);
                    mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, 0);
                    if (is_double) {
                        mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, 0);
                        mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, 0);
                    }
                    break;
                case "R":
                    mouse_event(MouseEventFlag.RightDown, 0, 0, 0, 0);
                    mouse_event(MouseEventFlag.RightUp, 0, 0, 0, 0);
                    if (is_double) {
                        mouse_event(MouseEventFlag.RightDown, 0, 0, 0, 0);
                        mouse_event(MouseEventFlag.RightUp, 0, 0, 0, 0);
                    }
                    break;
                case "M":
                    mouse_event(MouseEventFlag.MiddleDown, 0, 0, 0, 0);
                    mouse_event(MouseEventFlag.MiddleUp, 0, 0, 0, 0);
                    if (is_double)
                    {
                        mouse_event(MouseEventFlag.MiddleDown, 0, 0, 0, 0);
                        mouse_event(MouseEventFlag.MiddleUp, 0, 0, 0, 0);
                    }
                    break;
            }
        }
 
        //鼠标移动到 指定位置(_dx,_dy)
        public void mouse_move(int _dx, int _dy)
        {
            SetCursorPos(_dx, _dy);
        }
 
        //键盘操作
        public void keybd(Byte _bVk, KeyEventFlag _dwFlags)
        {
            keybd_event(_bVk, 0, _dwFlags, 0);
        }
 
        //键盘操作 带自动释放 dwFlags_time 单位:毫秒
        public void keybd(Byte __bVk, int dwFlags_time = 100)
        {
 
            this.vbk = __bVk;
            //设置定时器间隔时间
            this.atimer.Interval = dwFlags_time;
            keybd(this.vbk, KeyEventFlag.Down);
            this.atimer.Enabled = true;
        }
 
        //键盘操作 组合键 带释放
        public void keybd(Byte[] _bVk)
        {
            if (_bVk.Length >= 2)
            {
                //按下所有键
                foreach (Byte __bVk in _bVk){
                    keybd(__bVk, KeyEventFlag.Down);
                }
                //反转按键排序
                _bVk=(Byte[])_bVk.Reverse().ToArray();
 
                //松开所有键
                foreach (Byte __bVk in _bVk)
                {
                    keybd(__bVk, KeyEventFlag.Up);
                }
            }
        }
 
        void atimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.atimer.Enabled = false;
 
            //释放按键
            keybd(this.vbk, KeyEventFlag.Up);
        }
 
        //获取键码 这一部分 就是根据字符串 获取 键码 这里只列出了一部分 可以自己修改
        public Byte getKeys(string key)
        {
            switch (key) {
                   
                case "A": return (Byte)Key.A; 
                case "B": return (Byte)Key.B; 
                case "C": return (Byte)Key.C; 
                case "D": return (Byte)Key.D; 
                case "E": return (Byte)Key.E; 
                case "F": return (Byte)Key.F; 
                case "G": return (Byte)Key.G; 
                case "H": return (Byte)Key.H; 
                case "I": return (Byte)Key.I; 
                case "J": return (Byte)Key.J; 
                case "K": return (Byte)Key.K; 
                case "L": return (Byte)Key.L; 
                case "M": return (Byte)Key.M; 
                case "N": return (Byte)Key.N; 
                case "O": return (Byte)Key.O; 
                case "P": return (Byte)Key.P; 
                case "Q": return (Byte)Key.Q; 
                case "R": return (Byte)Key.R; 
                case "S": return (Byte)Key.S; 
                case "T": return (Byte)Key.T; 
                case "U": return (Byte)Key.U; 
                case "V": return (Byte)Key.V; 
                case "W": return (Byte)Key.W; 
                case "X": return (Byte)Key.X; 
                case "Y": return (Byte)Key.Y; 
                case "Z": return (Byte)Key.Z;
                case "Add": return (Byte)Key.Add;
                case "Back": return (Byte)Key.Back;
                case "Cancel": return (Byte)Key.Cancel;
                case "Capital": return (Byte)Key.Capital;
                case "CapsLock": return (Byte)Key.CapsLock;
                case "Clear": return (Byte)Key.Clear;
                case "Crsel": return (Byte)Key.CrSel;
               // case "ControlKey": return (Byte)Key.cr;
                case "D0": return (Byte)Key.D0;
                case "D1": return (Byte)Key.D1;
                case "D2": return (Byte)Key.D2;
                case "D3": return (Byte)Key.D3;
                case "D4": return (Byte)Key.D4;
                case "D5": return (Byte)Key.D5;
                case "D6": return (Byte)Key.D6;
                case "D7": return (Byte)Key.D7;
                case "D8": return (Byte)Key.D8;
                case "D9": return (Byte)Key.D9;
                case "Decimal": return (Byte)Key.Decimal;
                case "Delete": return (Byte)Key.Delete;
                case "Divide": return (Byte)Key.Divide;
                case "Down": return (Byte)Key.Down;
                case "End": return (Byte)Key.End;
                case "Enter": return (Byte)Key.Enter;
                case "Escape": return (Byte)Key.Escape;
                case "F1": return (Byte)Key.F1;
                case "F2": return (Byte)Key.F2;
                case "F3": return (Byte)Key.F3;
                case "F4": return (Byte)Key.F4;
                case "F5": return (Byte)Key.F5;
                case "F6": return (Byte)Key.F6;
                case "F7": return (Byte)Key.F7;
                case "F8": return (Byte)Key.F8;
                case "F9": return (Byte)Key.F9;
                case "F10": return (Byte)Key.F10;
                case "F11": return (Byte)Key.F11;
                case "F12": return (Byte)Key.F12;
                case "Help": return (Byte)Key.Help;
                case "Home": return (Byte)Key.Home;
                case "Insert": return (Byte)Key.Insert;
                //case "LButton": return (Byte)Key.LButton;
                case "LControl": return (Byte)Key.LeftCtrl;
                case "Left": return (Byte)Key.Left;
                //case "LMenu": return (Byte)Key.LMenu;
             
                case "LShift": return (Byte)Key.LeftShift;
                case "LWin": return (Byte)Key.LWin;
                //case "MButton": return (Byte)Key.mb;
                //case "Menu": return (Byte)Key.Menu;
                case "Multiply": return (Byte)Key.Multiply;
                case "Next": return (Byte)Key.Next;
                case "NumLock": return (Byte)Key.NumLock;
                case "NumPad0": return (Byte)Key.NumPad0;
                case "NumPad1": return (Byte)Key.NumPad1;
                case "NumPad2": return (Byte)Key.NumPad2;
                case "NumPad3": return (Byte)Key.NumPad3;
                case "NumPad4": return (Byte)Key.NumPad4;
                case "NumPad5": return (Byte)Key.NumPad5;
                case "NumPad6": return (Byte)Key.NumPad6;
                case "NumPad7": return (Byte)Key.NumPad7;
                case "NumPad8": return (Byte)Key.NumPad8;
                case "NumPad9": return (Byte)Key.NumPad9;
                case "PageDown": return (Byte)Key.PageDown;
                case "PageUp": return (Byte)Key.PageUp;
            //    case "Process": return (Byte)Key.P;
              //  case "RButton": return (Byte)Key.RButton;
                case "Right": return (Byte)Key.Right;
                case "RControl": return (Byte)Key.RightCtrl;
               // case "RMenu": return (Byte)Key.men;
                case "RShift": return (Byte)Key.RightShift;
                case "Scroll": return (Byte)Key.Scroll;
                case "Space": return (Byte)Key.Space;
                case "Tab": return (Byte)Key.Tab;
                case "Up": return (Byte)Key.Up;
            }
            return 0;
        }
    }
}
