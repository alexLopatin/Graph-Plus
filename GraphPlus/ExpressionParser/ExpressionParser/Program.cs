using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionParser
{
    class Derivative
    {
        ExpressionTree tree;
        
        public ExpressionTree Calculate()
        {

            return tree;

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Expression expression = new Expression();
            List<Variable> variables = new List<Variable>() { new Variable("x", 0, true) };
            expression.Parse("(x+1 )^ ln(x*0+1)", variables);
            variables[0].Value = 5;
            var val = expression.GetValue();
            expression.Simplify();
            Console.WriteLine(expression.ToString());
            Console.WriteLine("Ready");
        }
    }
}
