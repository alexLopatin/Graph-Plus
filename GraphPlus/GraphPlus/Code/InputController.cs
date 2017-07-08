using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Threading;
using lib;

namespace GraphPlus
{
    public class InputController
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {

                return new Point(point.X, point.Y);
            }
        }
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);


        private Thread MCThread;
        private ViewController VC;


        public InputController(NativeWindow renderWindow)
        {
            VC = new ViewController(renderWindow);
            MCThread = new Thread(new ThreadStart(MouseController));
        }

        public void MouseControl(ref MSG m, ref bool handled)
        {
            if (m.message == (int)win32Message.WM_MOUSEWHEEL)
            {
                var delta = ((short)((int)m.wParam >> 16));
                if (delta < 0)
                {
                    VC.ZoomIn();
                    if (!MCThread.IsAlive)
                    {
                        VC.Draw();
                    }
                }
                else
                {
                    VC.ZoomOut();
                    if (!MCThread.IsAlive)
                    {
                        VC.Draw();
                    }
                }

            }
            if (m.message == (int)win32Message.WM_RBUTTONDOWN)
            {

                if (!MCThread.IsAlive)
                {
                    stopThread = false;
                    MCThread = new Thread(new ThreadStart(MouseController));
                    MCThread.Start();
                }
                else
                {
                    stopThread = true;
                }
                handled = true;

            }
            if (m.message == (int)win32Message.WM_RBUTTONUP)
            {
                if (MCThread.IsAlive)
                    stopThread = true;
                handled = true;
            }
        }
        private static POINT oldPosition;


        static volatile bool stopThread = false;

        private void MouseController()
        {
            GetCursorPos(out oldPosition);
            while (true)
            {
                if (stopThread)
                    return;
                POINT newPosition;
                //Thread.Sleep(1);
                GetCursorPos(out newPosition);

                VC.MoveCamera(newPosition.X - oldPosition.X, newPosition.Y - oldPosition.Y);
                VC.Draw();
                oldPosition = newPosition;
            }
        }
        public void AddFunction(string function)
        {
            VC.RenderWindow.scene.AddFunction(function);
            VC.Draw();
        }

    }
}