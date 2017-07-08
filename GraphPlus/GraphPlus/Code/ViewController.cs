using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlus
{
    public class ViewController
    {
        public NativeWindow RenderWindow;

        public ViewController(NativeWindow renderWindow)
        {
            RenderWindow = renderWindow;
        }

        public void MoveCamera(float x, float y)
        {
            RenderWindow.scene.MoveCamera(x, y);
        }

        public void ZoomIn()
        {
            RenderWindow.scene.ZoomIn();
        }
        public void ZoomOut()
        {
            RenderWindow.scene.ZoomOut();
        }
        public void Draw()
        {
            RenderWindow.scene.Draw();
        }
    }
}