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
        public Bitmap Concat(string operation, Bitmap left, Bitmap right)
        {
            int height = Math.Max(left.Height, right.Height);
            int width = left.Width + right.Width + 40;
            Bitmap res = new Bitmap(width, height);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    res.SetPixel(i, j, Color.White);
            var g = Graphics.FromImage(res);
            g.DrawImage(left, 0, (height - left.Height)/2);
            g.DrawImage(right, left.Width + 40, (height - right.Height)/2);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(operation, new Font("Tahoma", 16, FontStyle.Regular), Brushes.Black, left.Width+10, height/2+3, sf);
            return res;
        }
        public Bitmap ToImage(string text)
        {
            Bitmap res = new Bitmap(1, 1);
            
            var g = Graphics.FromImage(res);
            var size = g.MeasureString(text, new Font("Tahoma", 16, FontStyle.Regular)).ToSize();
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
            g.DrawString(text, new Font("Tahoma", 16, FontStyle.Regular), Brushes.Black, 0, size.Height/2, sf);
            return res;
        }
        Bitmap image;
        public Bitmap GetImage(ExpressionNode node = null)
        {
            if (node == null)
                node = (ExpressionNode)expression.expTree.head;
            if (node.Operation is string)
            {
                Bitmap first = null;
                Bitmap second = null;
                if (node.Children[0] is ExpressionNode)
                    first = GetImage(node.Children[0] as ExpressionNode);
                else
                    first = ToImage(node.Children[0].ToString());

                if (node.Children[1] is ExpressionNode)
                    second = GetImage(node.Children[1] as ExpressionNode);
                else
                    second = ToImage(node.Children[1].ToString());

                return Concat(node.Operation as string, first, second);
            }
            return image;
        }
    }
}
