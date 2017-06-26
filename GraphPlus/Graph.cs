using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


namespace GraphPlus
{
    public class Graph
    {
        public Canvas graphCanvas;
        public double scale = 1;
        int g = 0;
        double average = 50;
        int scaleLevel = 1;
        bool scaleChangedTo = true;
        public Graph(Canvas graphCanvas)
        {
            this.graphCanvas = graphCanvas;
            graphInitialize();
        }
        /// <summary>
        /// Инициализация графика
        /// </summary>
        public void graphInitialize()
        {
            Line absciss = new Line();
            Line ordinate = new Line();

            absciss.Name = "absciss";
            ordinate.Name = "ordinate";

            absciss.Stroke = Brushes.Black;
            ordinate.Stroke = Brushes.Black;
            absciss.StrokeThickness = 1;
            ordinate.StrokeThickness = 1;

            var deltaX = (graphCanvas.ActualWidth / 2 + moveDeltaX) - absciss.X1;
            var deltaY = (graphCanvas.ActualHeight / 2 + moveDeltaY) - ordinate.Y1;

            ordinate.X1 = graphCanvas.ActualWidth / 2 + moveDeltaX;
            ordinate.Y1 = 0;
            ordinate.X2 = graphCanvas.ActualWidth / 2 + moveDeltaX;
            ordinate.Y2 = graphCanvas.ActualHeight;

            absciss.X1 = 0;
            absciss.Y1 = graphCanvas.ActualHeight / 2 + moveDeltaY;
            absciss.X2 = graphCanvas.ActualWidth;
            absciss.Y2 = graphCanvas.ActualHeight / 2 + moveDeltaY;

            graphCanvas.Children.Add(ordinate);
            graphCanvas.Children.Add(absciss);

            Label labelX = new Label();
            Label labelY = new Label();

            labelX.Content = "x";
            labelY.Content = "y";

            labelX.FontSize = 14;
            labelY.FontSize = 14;

            graphCanvas.Children.Add(labelX);
            graphCanvas.Children.Add(labelY);

            Canvas.SetLeft(labelX, graphCanvas.ActualWidth - 15);
            Canvas.SetTop(labelX, graphCanvas.ActualHeight / 2 - 27 + moveDeltaY);
            Canvas.SetLeft(labelY, graphCanvas.ActualWidth / 2 + moveDeltaX + 3);
            Canvas.SetTop(labelY, -10);
        }
        public void graphUpdate()
        {
            var ordinate = graphCanvas.Children.OfType<Line>().Where(p => p.Name == "ordinate").ToList()[0];
            var absciss = graphCanvas.Children.OfType<Line>().Where(p => p.Name == "absciss").ToList()[0];
            graphDeleteChilds("ordinate");
            graphDeleteChilds("absciss");
            ordinate.X1 = graphCanvas.ActualWidth / 2 + moveDeltaX;
            ordinate.Y1 = 0;
            ordinate.X2 = graphCanvas.ActualWidth / 2 + moveDeltaX;
            ordinate.Y2 = graphCanvas.ActualHeight;

            absciss.X1 = 0;
            absciss.Y1 = graphCanvas.ActualHeight / 2 + moveDeltaY;
            absciss.X2 = graphCanvas.ActualWidth;
            absciss.Y2 = graphCanvas.ActualHeight / 2 + moveDeltaY;

            graphCanvas.Children.Add(ordinate);
            graphCanvas.Children.Add(absciss);
        }
        public void graphScale()
        {

        }
        public void graphMove()
        {
        
        }
        /// <summary>
        /// Удаляет дочерние объекты типа Line в элементе graphCanvas
        /// </summary>
        /// <param name="startsWith">Префикс в названии элементов, которые будут удалены</param>
        private void graphDeleteChilds(string startsWith= "")
        {
            if (startsWith == "")
            {
                var listOfLines = graphCanvas.Children.OfType<Line>().ToList();
                for(int i = 0; i<=listOfLines.Count-1; i++)
                {
                    graphCanvas.Children.RemoveAt(i);
                }
                return;
            }
            else
            {
                var listOfLines = graphCanvas.Children.OfType<Line>().Where(p=> p.Name.StartsWith(startsWith)).ToList();
                for (int i = 0; i <= listOfLines.Count - 1; i++)
                {
                    graphCanvas.Children.RemoveAt(i);
                }
                return;
            }
            
            
        }
        public void move()
        {
            if (Math.Log10(scaleLevel) < 8)
            {
                if (scaleChangedTo)
                {
                    if (average <= scaleLevel.ToString().Count() * 10 + 30)
                    {
                        average *= 10;
                        scaleLevel *= 10;
                    }
                }
                else
                {
                    if (average >= graphCanvas.ActualWidth / 5)
                    {
                        average /= 10;
                        scaleLevel /= 10;
                    }

                }

            }
            Line absciss = new Line();
            Line ordinate = new Line();
            absciss.Stroke = Brushes.Black;
            ordinate.Stroke = Brushes.Black;
            absciss.StrokeThickness = 1;
            ordinate.StrokeThickness = 1;
            var deltaX = (graphCanvas.ActualWidth / 2 + moveDeltaX) - absciss.X1;
            var deltaY = (graphCanvas.ActualHeight / 2 + moveDeltaY) - ordinate.Y1;

            ordinate.X1 = graphCanvas.ActualWidth / 2 + moveDeltaX;
            ordinate.Y1 = 0;
            ordinate.X2 = graphCanvas.ActualWidth / 2 + moveDeltaX;
            ordinate.Y2 = graphCanvas.ActualHeight;

            absciss.X1 = 0;
            absciss.Y1 = graphCanvas.ActualHeight / 2 + moveDeltaY;
            absciss.X2 = graphCanvas.ActualWidth;
            absciss.Y2 = graphCanvas.ActualHeight / 2 + moveDeltaY;


            if (absciss.Y1 > graphCanvas.ActualHeight | absciss.Y1 < 0)
            {
                absciss.Opacity = 0;
            }
            else
            {
                absciss.Opacity = 1;
            }


            if (ordinate.X1 > graphCanvas.ActualWidth | ordinate.X1 < 0)
            {
                ordinate.Opacity = 0;
            }
            else
            {
                ordinate.Opacity = 1;
            }

            var byX = graphCanvas.Children.OfType<Line>().Where(p => p.Name.StartsWith("segmentionX")).ToList();
            var byY = graphCanvas.Children.OfType<Line>().Where(p => p.Name.StartsWith("segmentionY")).ToList();
            var labels = graphCanvas.Children.OfType<Label>().Select(p => p).ToList();
            for (int i = 0; i <= labels.Count - 1; i++)
            {
                graphCanvas.Children.Remove(labels[i]);
            }
            for (int i = 0; i <= byY.Count - 1; i++)
            {
                graphCanvas.Children.Remove(byY[i]);
            }
            for (int i = 0; i <= byX.Count - 1; i++)
            {
                graphCanvas.Children.Remove(byX[i]);
            }

            Label labelX = new Label();
            Label labelY = new Label();
            labelX.Content = "x";
            labelY.Content = "y";
            labelX.FontSize = 14;
            labelY.FontSize = 14;
            graphCanvas.Children.Add(labelX);
            graphCanvas.Children.Add(labelY);
            Canvas.SetLeft(labelX, graphCanvas.ActualWidth - 15);
            Canvas.SetTop(labelX, graphCanvas.ActualHeight / 2 - 27 + moveDeltaY);
            Canvas.SetLeft(labelY, graphCanvas.ActualWidth / 2 + moveDeltaX + 3);
            Canvas.SetTop(labelY, -10);

            double r = 0;
            double rSt = 0;

            int g = 0;
            for (double i = graphCanvas.ActualWidth / 2 - average; i >= -Math.Abs(moveDeltaX); i -= average)
            {
                Label lbl = new Label();
                lbl.FontSize = 14;
                r -= scaleLevel;
                if (Math.Log10(scaleLevel) >= 5)
                {
                    rSt--;
                    lbl.Content = rSt + " * 10^" + Math.Log10(scaleLevel);
                }
                else
                {
                    lbl.Content = r;
                }

                Line segmention = new Line();
                segmention.X2 = i + moveDeltaX;
                segmention.Y1 = graphCanvas.ActualHeight / 2 + 5 + moveDeltaY;
                segmention.Y2 = graphCanvas.ActualHeight / 2 - 5 + moveDeltaY;
                segmention.X1 = i + moveDeltaX;
                if (segmention.X1 < 0 | segmention.X1 > graphCanvas.ActualWidth | segmention.Y2 < 0 | segmention.Y1 > graphCanvas.ActualHeight)
                {
                    segmention = null;
                    continue;
                }
                segmention.Stroke = Brushes.Black;
                segmention.StrokeThickness = 1;
                segmention.Name = "segmention" + "X" + g;
                graphCanvas.Children.Add(segmention);
                graphCanvas.Children.Add(lbl);
                Canvas.SetLeft(lbl, i + moveDeltaX - 15);
                Canvas.SetTop(lbl, graphCanvas.ActualHeight / 2 + 5 + moveDeltaY);
                g++;
            }
            r = 0;
            rSt = 0;
            for (double i = graphCanvas.ActualWidth / 2 + average; i <= graphCanvas.ActualWidth - moveDeltaX; i += average)
            {
                Label lbl = new Label();
                lbl.FontSize = 14;
                r += scaleLevel;
                if (Math.Log10(scaleLevel) >= 5)
                {
                    rSt++;
                    lbl.Content = rSt + " * 10^" + Math.Log10(scaleLevel);
                }
                else
                {
                    lbl.Content = r;
                }

                Line segmention = new Line();
                segmention.X2 = i + moveDeltaX;
                segmention.Y1 = graphCanvas.ActualHeight / 2 + 5 + moveDeltaY;
                segmention.Y2 = graphCanvas.ActualHeight / 2 - 5 + moveDeltaY;
                segmention.X1 = i + moveDeltaX;
                if (segmention.X1 < 0 | segmention.X1 > graphCanvas.ActualWidth | segmention.Y2 < 0 | segmention.Y1 > graphCanvas.ActualHeight)
                {
                    segmention = null;
                    continue;
                }
                segmention.Stroke = Brushes.Black;
                segmention.StrokeThickness = 1;
                segmention.Name = "segmention" + "X" + g;
                graphCanvas.Children.Add(lbl);
                Canvas.SetLeft(lbl, i + moveDeltaX - 15);
                Canvas.SetTop(lbl, graphCanvas.ActualHeight / 2 + 5 + moveDeltaY);
                graphCanvas.Children.Add(segmention);
                g++;
            }
            r = 0;
            rSt = 0;
            for (double i = graphCanvas.ActualHeight / 2 + average; i <= graphCanvas.ActualHeight - moveDeltaY; i += average)
            {
                r -= scaleLevel;
                Label lbl = new Label();
                lbl.FontSize = 14;
                int o = r.ToString().Count();
                if (Math.Log10(scaleLevel) >= 5)
                {
                    rSt--;
                    o = rSt.ToString().Count();
                    lbl.Content = rSt + " * 10^" + Math.Log10(scaleLevel);
                    Canvas.SetLeft(lbl, graphCanvas.ActualWidth / 2 - (9 * o) + moveDeltaX - 70);
                    Canvas.SetTop(lbl, i + moveDeltaY - 15);
                }
                else
                {
                    Canvas.SetLeft(lbl, graphCanvas.ActualWidth / 2 - (9 * o) + moveDeltaX - 21);
                    Canvas.SetTop(lbl, i + moveDeltaY - 15);
                    lbl.Content = r;
                }

                Line segmention = new Line();
                segmention.Y1 = i + moveDeltaY;
                segmention.Y2 = i + moveDeltaY;
                segmention.X1 = graphCanvas.ActualWidth / 2 + 5 + moveDeltaX;
                segmention.X2 = graphCanvas.ActualWidth / 2 - 5 + moveDeltaX;
                if (segmention.Y1 < 0 | segmention.Y1 > graphCanvas.ActualHeight | segmention.X2 < 0 | segmention.X1 > graphCanvas.ActualWidth)
                {
                    segmention = null;
                    continue;
                }

                segmention.Stroke = Brushes.Black;
                segmention.StrokeThickness = 1;
                segmention.Name = "segmention" + "Y" + g;
                graphCanvas.Children.Add(segmention);

                graphCanvas.Children.Add(lbl);

                g++;
            }
            r = 0;
            rSt = 0;
            for (double i = graphCanvas.ActualHeight / 2 - average; i >= -Math.Abs(moveDeltaY); i -= average)
            {
                r += scaleLevel;
                Label lbl = new Label();
                lbl.FontSize = 14;
                int o = r.ToString().Count();
                if (Math.Log10(scaleLevel) >= 5)
                {
                    rSt++;
                    o = rSt.ToString().Count();
                    lbl.Content = rSt + " * 10^" + Math.Log10(scaleLevel);
                    Canvas.SetLeft(lbl, graphCanvas.ActualWidth / 2 - (9 * o) + moveDeltaX - 70);
                    Canvas.SetTop(lbl, i + moveDeltaY - 15);
                }
                else
                {
                    Canvas.SetLeft(lbl, graphCanvas.ActualWidth / 2 - (9 * o) + moveDeltaX - 21);
                    Canvas.SetTop(lbl, i + moveDeltaY - 15);
                    lbl.Content = r;
                }
                Line segmention = new Line();
                segmention.Y1 = i + moveDeltaY;
                segmention.Y2 = i + moveDeltaY;
                segmention.X1 = graphCanvas.ActualWidth / 2 + 5 + moveDeltaX;
                segmention.X2 = graphCanvas.ActualWidth / 2 - 5 + moveDeltaX;
                if (segmention.Y1 < 0 | segmention.Y1 > graphCanvas.ActualHeight | segmention.X2 < 0 | segmention.X1 > graphCanvas.ActualWidth)
                {
                    segmention = null;
                    continue;
                }
                segmention.Stroke = Brushes.Black;
                segmention.StrokeThickness = 1;
                segmention.Name = "segmention" + "Y" + g;
                graphCanvas.Children.Add(lbl);
                graphCanvas.Children.Add(segmention);
                g++;
            }
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
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        double moveDeltaX = 0;
        double moveDeltaY = 0;
        Point oldPosition;
        private void graphCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            oldPosition = GetMousePosition();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Threading.Thread.Sleep(10);
                var newPosition = GetMousePosition();
                moveDeltaX = newPosition.X - oldPosition.X + moveDeltaX;
                moveDeltaY = newPosition.Y - oldPosition.Y + moveDeltaY;
                move();
            }
        }

        private void graphCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta > 0)
                {
                    average *= 1.1;
                    scale += 0.1;
                    scaleChangedTo = false;
                }
                else
                {
                    average /= 1.1;
                    scale -= 0.1;
                    scaleChangedTo = true;
                }
                move();
            }

        }
    }
}
