using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RegistryDemo
{
    public class MouseOperate
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        static bool SetCursorPosEx(int x, int y)
        {
           return SetCursorPos(x, y);
        }

     

        [Flags]
        enum MouseEventFlag : uint
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


        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);
        static void MouseLeftDown(int x, int y)
        {
           mouse_event(MouseEventFlag.Absolute|MouseEventFlag.LeftDown,x,y,0,UIntPtr.Zero);
        }

        static void MouseLeftUp(int x, int y)
        {
            mouse_event(MouseEventFlag.Absolute|MouseEventFlag.LeftUp,x,y,0,UIntPtr.Zero);
        }

        static void MouseRightDown(int x, int y)
        {
            mouse_event(MouseEventFlag.Absolute|MouseEventFlag.RightDown,x,y,0,UIntPtr.Zero);
        }

        static void MouseRightUp(int x, int y)
        {
            mouse_event(MouseEventFlag.Absolute|MouseEventFlag.RightUp,x,y,0,UIntPtr.Zero);
        }
    }
}
