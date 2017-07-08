using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlus
{
    enum win32Message : int
    {
        WM_PAINT = 0x000F,
        WM_SIZE = 0x0005,
        WM_CREATE = 0x0001,
        WM_RBUTTONDOWN = 0x0204,
        WM_KEYDOWN = 0x0100,
        WM_RBUTTONUP = 0x0205,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_MOUSEWHEEL = 0x020A
    }
}