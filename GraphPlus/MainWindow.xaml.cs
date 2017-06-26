using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms.Integration;
using OpenTK.Graphics.OpenGL;
using QuickFont;
using OpenTK.Graphics;
using OpenTK.Input;
using System.Drawing;
using System.Drawing.Imaging;



namespace GraphPlus
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        int g = 0;
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;


            MathExpression ME = new MathExpression("2134+44+2+56-67");
            Thread.Sleep(2);
            

        }
        static double NthRoot(double A, int N)
        {
            return Math.Pow(A, 1.0 / N);
        }
        





        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };



        MouseState current, previous;
        int xdelta = 0;
        int ydelta = 0;
        double average = 0.1;
        double scaleLevel = 1;
        List<OpenTK.Vector2> graphicPoints = new List<OpenTK.Vector2>();
        private void glControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            GL.ClearColor(System.Drawing.Color.White);


            if (average <= 0.05)
            {
                scaleLevel /= 2;
                average = 0.1;
            }
            else if (average >= 0.2)
            {
                scaleLevel *= 2;
                average = 0.1;
            }



            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Translate((float)xdelta / 500, (float)ydelta / 500, 0);

            GL.Viewport(GLC.ClientRectangle);

            //average = (GLC.Width / GLC.Height) * 0.1;

            GL.LineWidth(1.25f);

            

            Bitmap bmp = new Bitmap(50, 50, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            var gfx = Graphics.FromImage(bmp);
            //gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;



            var texture = GL.GenTexture();


            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            Font font = new Font("Cambria", 12.0f);
            
            gfx.Clear(System.Drawing.Color.Transparent);
            gfx.DrawString("1", font, System.Drawing.Brushes.Black, new PointF { X =0, Y = 0 });

            BitmapData data =bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 50, 50, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);



            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0f, 1f); GL.Vertex2(0f, 0f);
            GL.TexCoord2(1f, 1f); GL.Vertex2(1f, 0f);
            GL.TexCoord2(1f, 0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0f, 0f); GL.Vertex2(0f, 1f);

            GL.End();

            GL.Disable(EnableCap.Texture2D);

            bmp.Dispose();
            gfx.Dispose();
            
            
            DrawFunction();
            DrawElseFunction();


            #region Abscissa

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(System.Drawing.Color.MidnightBlue);
            
            GL.Vertex2(-GLC.Width, 0);
            GL.Vertex2(GLC.Width , 0);

            GL.End();

            for(double i =0; i<= GLC.Width; i += average)
            {
                
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(System.Drawing.Color.MidnightBlue);

                GL.Vertex2(i, 0.01);
                GL.Vertex2(i, -0.01);


                GL.End();
            }

            for (double i = 0; i >= -GLC.Width; i -= average)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(System.Drawing.Color.MidnightBlue);
                
                GL.Vertex2(i, 0.01);
                GL.Vertex2(i, -0.01);


                GL.End();
            }

            #endregion

            #region Ordinate

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(System.Drawing.Color.MidnightBlue);

            GL.Vertex2(0, -GLC.Height);
            GL.Vertex2(0, GLC.Height);

            GL.End();

            for (double i = 0; i <= GLC.Height; i += average)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(System.Drawing.Color.MidnightBlue);

                GL.Vertex2(0.01, i);
                GL.Vertex2(-0.01, i);


                GL.End();
            }

            for (double i = 0; i >= -GLC.Height; i -= average)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(System.Drawing.Color.MidnightBlue);

                GL.Vertex2(0.01, i);
                GL.Vertex2(-0.01, i);


                GL.End();
            }

            #endregion

            GL.LineWidth(1f);
            


            GLC.SwapBuffers();


        }

        private void glControl_Resize(object sender, EventArgs e)
        {

            GLC.Invalidate();

        }
        
        private void glControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(System.Drawing.Color.White);
            



        }

        private void GLC_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            current = OpenTK.Input.Mouse.GetState();
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                
                xdelta = current.X - previous.X + xdelta;
                ydelta = -(current.Y - previous.Y) + ydelta;
                //double zdelta = current.Wheel - previous.Wheel;

                GLC.Invalidate();
                
            }
            previous = current;
        }

        private void GLC_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                average += 0.01;
                GLC.Invalidate();
            }
            else
            {
                average -= 0.01;
                GLC.Invalidate();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        #region Douglas-Peucker Algorithm

        /// <span class="code-SummaryComment"><summary></span>
        /// Uses the Douglas Peucker algorithm to reduce the number of points.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="Points">The points.</param></span>
        /// <span class="code-SummaryComment"><param name="Tolerance">The tolerance.</param></span>
        /// <span class="code-SummaryComment"><returns></returns></span>
        public static List<OpenTK.Vector2> DouglasPeuckerReduction
            (List<OpenTK.Vector2> Points, Double Tolerance)
        {
            if (Points == null || Points.Count < 3)
                return Points;

            Int32 firstPoint = 0;
            Int32 lastPoint = Points.Count - 1;
            List<Int32> pointIndexsToKeep = new List<Int32>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            //The first and the last point cannot be the same
            while (Points[firstPoint].Equals(Points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(Points, firstPoint, lastPoint,
            Tolerance, ref pointIndexsToKeep);

            List<OpenTK.Vector2> returnPoints = new List<OpenTK.Vector2>();
            pointIndexsToKeep.Sort();
            foreach (Int32 index in pointIndexsToKeep)
            {
                returnPoints.Add(Points[index]);
            }

            return returnPoints;
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Douglases the peucker reduction.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="points">The points.</param></span>
        /// <span class="code-SummaryComment"><param name="firstPoint">The first point.</param></span>
        /// <span class="code-SummaryComment"><param name="lastPoint">The last point.</param></span>
        /// <span class="code-SummaryComment"><param name="tolerance">The tolerance.</param></span>
        /// <span class="code-SummaryComment"><param name="pointIndexsToKeep">The point index to keep.</param></span>
        private static void DouglasPeuckerReduction(List<OpenTK.Vector2>
            points, Int32 firstPoint, Int32 lastPoint, Double tolerance,
            ref List<Int32> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            Int32 indexFarthest = 0;

            for (Int32 index = firstPoint; index < lastPoint; index++)
            {
                Double distance = PerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint,
                indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest,
                lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// The distance of a point from a line made from point1 and point2.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="pt1">The PT1.</param></span>
        /// <span class="code-SummaryComment"><param name="pt2">The PT2.</param></span>
        /// <span class="code-SummaryComment"><param name="p">The p.</param></span>
        /// <span class="code-SummaryComment"><returns></returns></span>
        public static Double PerpendicularDistance
            (OpenTK.Vector2 Point1, OpenTK.Vector2 Point2, OpenTK.Vector2 Point)
        {
            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base

            Double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X *
            Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X *
            Point2.Y - Point1.X * Point.Y));
            Double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) +
            Math.Pow(Point1.Y - Point2.Y, 2));
            Double height = area / bottom * 2;

            return height;

            //Another option
            //Double A = Point.X - Point1.X;
            //Double B = Point.Y - Point1.Y;
            //Double C = Point2.X - Point1.X;
            //Double D = Point2.Y - Point1.Y;

            //Double dot = A * C + B * D;
            //Double len_sq = C * C + D * D;
            //Double param = dot / len_sq;

            //Double xx, yy;

            //if (param < 0)
            //{
            //    xx = Point1.X;
            //    yy = Point1.Y;
            //}
            //else if (param > 1)
            //{
            //    xx = Point2.X;
            //    yy = Point2.Y;
            //}
            //else
            //{
            //    xx = Point1.X + param * C;
            //    yy = Point1.Y + param * D;
            //}

            //Double d = DistanceBetweenOn2DPlane(Point, new Point(xx, yy));
        }
        #endregion
        public void DrawFunction()
        {

            GL.Color3(System.Drawing.Color.Green);
            GL.Begin(PrimitiveType.LineStrip);

            if (scaleLevel <= 1)
            {
                for (double i = 0; i >= -GLC.Width / average; i -= 0.2 / scaleLevel)
                {


                    GL.Vertex2(i * average * scaleLevel, i * i * average * scaleLevel);



                }
            }
            else
            {
                for (double i = 0; i >= -GLC.Width / average; i -= average / scaleLevel * Math.Sqrt(scaleLevel * 3))
                {


                    GL.Vertex2(i * average * scaleLevel, i * i * average * scaleLevel);



                }
            }


            GL.End();

            GL.Begin(PrimitiveType.LineStrip);

            if (scaleLevel <= 1)
            {

                for (double i = 0; i <= GLC.Width / average; i += 0.2 / scaleLevel)
                {


                    GL.Vertex2(i * average * scaleLevel, i * i * average * scaleLevel);



                }

            }
            else
            {
                for (double i = 0; i <= GLC.Width / average; i += average / scaleLevel * Math.Sqrt(scaleLevel * 3))
                {


                    GL.Vertex2(i * average * scaleLevel, i * i * average * scaleLevel);



                }
            }



            GL.End();

        }
        public void DrawElseFunction()
        {

            GL.Color3(System.Drawing.Color.Red);
            GL.Begin(PrimitiveType.LineStrip);

            if (scaleLevel <= 1)
            {
                for (double i = 0; i >= -GLC.Width / average; i -= 0.2 / scaleLevel)
                {


                    GL.Vertex2(i * average * scaleLevel, (Math.Sin(i)) * average * scaleLevel);



                }
            }
            else
            {
                for (double i = 0; i >= -GLC.Width / average; i -= average / scaleLevel * Math.Sqrt(scaleLevel * 3))
                {


                    GL.Vertex2(i * average * scaleLevel, (Math.Sin(i)) * average * scaleLevel);



                }
            }


            GL.End();

            GL.Begin(PrimitiveType.LineStrip);

            if (scaleLevel <= 1)
            {

                for (double i = 0; i <= GLC.Width / average; i += 0.2 / scaleLevel)
                {


                    GL.Vertex2(i * average * scaleLevel, (Math.Sin(i)) * average * scaleLevel);



                }

            }
            else
            {
                for (double i = 0; i <= GLC.Width / average; i += average / scaleLevel * Math.Sqrt(scaleLevel * 3))
                {


                    GL.Vertex2(i * average * scaleLevel, (Math.Sin(i)) * average * scaleLevel);



                }
            }



            GL.End();

        }
    }








}


