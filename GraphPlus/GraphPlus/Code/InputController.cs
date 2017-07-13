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
using System.Diagnostics;

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

                var mouseY = (short)((int)m.lParam & 0xFFFF);
                var mouseX = ((short)((int)m.lParam >> 16));
                if (!MCThread.IsAlive&IsMouseOverRenderWindow())
                {
                    
                    stopThread = false;
                    MCThread = new Thread(new ThreadStart(MouseController));
                    MCThread.Start();
                }
                else
                {
                    stopThread = true;
                }
                //handled = true;

            }
            if (m.message == (int)win32Message.WM_RBUTTONUP)
            {
                if (MCThread.IsAlive)
                    stopThread = true;
                //handled = true;
            }
            
        }
        private static POINT oldPosition;


        static volatile bool stopThread = false;

        public void StopMouseControl()
        {
            if (MCThread.IsAlive)
                stopThread = true;
        }

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
        public void AddFunction(Function func)
        {
            VC.RenderWindow.scene.AddFunction(func);
            VC.Draw();
        }
        public void RemoveFunction(Function func)
        {
            VC.RenderWindow.scene.RemoveFunction(func);
            VC.Draw();
        }
        public bool IsMouseOverRenderWindow()
        {
            short x = (short)System.Windows.Forms.Cursor.Position.X;
            short y = (short)System.Windows.Forms.Cursor.Position.Y;

            Point upLeft = VC.RenderWindow.PointToScreen(new Point(0, 0));
            Point rightDown = VC.RenderWindow.PointToScreen(new Point(0, 0));
            double width = VC.RenderWindow.ActualWidth;
            double height = VC.RenderWindow.ActualHeight;
            rightDown.X = rightDown.X + width;
            rightDown.Y = rightDown.Y + height;
            if (x <= rightDown.X & x >= upLeft.X & y <= rightDown.Y & y >= upLeft.Y)
                return true;
            return false;
        }
        public void Draw()
        {
            if(!MCThread.IsAlive)
                VC.Draw();
        }
    }
}