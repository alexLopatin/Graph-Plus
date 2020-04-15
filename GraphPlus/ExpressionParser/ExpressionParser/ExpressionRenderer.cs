using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExpressionParser
{
    class ExpressionRenderer
    {
        Expression expression;
        public ExpressionRenderer(Expression expression)
        {
            this.expression = expression;
        }
        public Bitmap Concat(string operation, Bitmap left, Bitmap right, int emSize)
        {
            if (operation == "^")
            {
                int height = left.Height + right.Height - 10;
                int width = left.Width + right.Width;
                Bitmap res = new Bitmap(width, height);
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        res.SetPixel(i, j, Color.White);
                var g = Graphics.FromImage(res);

                g.DrawImage(left, 0, (right.Height - 10)/2);
                g.DrawImage(right, left.Width, 0);
                return res;
            }
            else if (operation == "/")
            {
                int height = left.Height + right.Height + 5;
                int width = Math.Max(left.Width, right.Width) + 10;
                Bitmap res = new Bitmap(width, height);
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        res.SetPixel(i, j, Color.White);
                var g = Graphics.FromImage(res);
                g.DrawLine(new Pen(Color.Black, 2f), 2, left.Height + 3, width - 2, left.Height + 3);
                g.DrawImage(left, (width - left.Width)/2, 0);
                g.DrawImage(right, (width - right.Width) / 2, left.Height + 10);
                return res;
            }
            else
            {
                int height = Math.Max(left.Height, right.Height);
                int width = left.Width + right.Width + 30;
                Bitmap res = new Bitmap(width, height);
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        res.SetPixel(i, j, Color.White);
                var g = Graphics.FromImage(res);
                g.DrawImage(left, 0, (height - left.Height) / 2);
                g.DrawImage(right, left.Width+30, (height - right.Height) / 2);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                if (operation == "*")
                    g.DrawString("⋅", new Font("Tahoma", emSize*1.5f, FontStyle.Regular), Brushes.Black, left.Width+2, height / 2, sf);
                else
                    g.DrawString(operation, new Font("Tahoma", emSize, FontStyle.Regular), Brushes.Black, left.Width+2, height / 2, sf);
                return res;
            }
            
        }
        public Bitmap ToImage(string text, int emSize)
        {
            Bitmap res = new Bitmap(1, 1);
            int offset = 0;
            if (!double.TryParse(text, out _))
                offset = -1;
            var g = Graphics.FromImage(res);
            var size = g.MeasureString(text, new Font("Tahoma", emSize, FontStyle.Regular)).ToSize();
            res = new Bitmap(size.Width, size.Height);
            for (int i = 0; i < size.Width; i++)
                for (int j = 0; j < size.Height; j++)
                    res.SetPixel(i, j, Color.White);
            g = Graphics.FromImage(res);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(text, new Font("Tahoma", emSize, FontStyle.Regular), Brushes.Black, 0, size.Height/2 + offset, sf);
            return res;
        }
        public Bitmap AddBrackets(Bitmap image)
        {
            Bitmap res = new Bitmap(image.Width + 2*10, image.Height);
            for (int i = 0; i < res.Width; i++)
                for (int j = 0; j < res.Height; j++)
                    res.SetPixel(i, j, Color.White);
            var g = Graphics.FromImage(res);
            g.DrawImage(image, 10, 0);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            Pen pen = new Pen(Color.Black, 1.8f);
            g.DrawArc(pen, 2, 2, 20, image.Height-4, 100, 160);
            g.DrawArc(pen, 4-10+image.Width, 2, 20, image.Height - 4, -80, 160);
            return res;
        }
        Bitmap image;
        public Bitmap GetImage(ExpressionNode node = null, int emSize = 16)
        {
            if (node == null)
                node = (ExpressionNode)expression.expTree.head;
            if (node.Operation is string)
            {
                Bitmap first = null;
                Bitmap second = null;
                if (node.Children[0] is ExpressionNode)
                {
                    first = GetImage(node.Children[0] as ExpressionNode, emSize);
                    if (node.Operation as string != "+" 
                        && node.Operation as string != "-"
                        && node.Operation as string != "/")
                        first = AddBrackets(first);
                }
                else
                    first = ToImage(node.Children[0].ToString(), emSize);

                if (node.Children[1] is ExpressionNode)
                {
                    if (node.Operation as string == "^")
                        second = GetImage(node.Children[1] as ExpressionNode, emSize/2 + 6);
                    else
                        second = GetImage(node.Children[1] as ExpressionNode, emSize);

                    if (node.Operation as string != "+" 
                        && node.Operation as string != "-"
                        && node.Operation as string != "^"
                        && node.Operation as string != "/")
                        second = AddBrackets(second);
                }
                else
                {
                    if(node.Operation as string == "^")
                        second = ToImage(node.Children[1].ToString(), emSize / 2 + 6);
                    else
                        second = ToImage(node.Children[1].ToString(), emSize);
                }

                return Concat(node.Operation as string, first, second, emSize);
            }
            return image;
        }
    }
}
