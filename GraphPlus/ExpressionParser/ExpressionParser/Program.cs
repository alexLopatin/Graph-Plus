using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace ExpressionParser
{
    class Program
    {
        static bool Test(string expr, Func<double, double> func)
        {
            Expression expression = new Expression();
            List<Variable> variables = new List<Variable>() { new Variable("x", 0, true) };
            expression.Parse(expr, variables);

            expression.Simplify();
            for (double i = -1000; i <= 1000; i += 0.1)
            {
                variables[0].Value = i;
                if (Math.Abs(expression.GetValue() - func(i)) > 0.1)
                    return false;
            }
            return true;
        }
        static void Main(string[] args)
        {
            Expression expression = new Expression();
            List<Variable> variables = new List<Variable>()
            { 
                new Variable("x", 1, true), 
                new Variable("y", 1, true),
                new Variable("z", 1, true)
            };
            //expression.Parse("1+x-(8+7*x)/(6+x) *5 ", variables);
            expression.Parse("(x-1)*2+5^(x+2*x-1/5)*(8+7-1*x)", variables);
            //expression.Simplify();
            Derivative der = new Derivative(expression);

            var d = der.PartialDerivative(variables[0]);
            var val = expression.GetValue();

            var renderer = new ExpressionRenderer(expression);
            //var image = renderer.Concat("+", (Bitmap)Bitmap.FromFile("1.bmp"), (Bitmap)Bitmap.FromFile("2.bmp"));
            //image = renderer.ToImage("something");
            var image = renderer.GetImage();
            image.Save("test.png");
            Console.WriteLine(d);
            Console.WriteLine("Ready");
        }
    }
}
