using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            List<Variable> variables = new List<Variable>() { new Variable("x", 0, true) };
            expression.Parse("ctg(x^2)", variables);
            Derivative der = new Derivative(expression);
            var d = der.Calculate();
            Console.WriteLine("Ready");
        }
    }
}
