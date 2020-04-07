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
            expression.Parse("1 + 2*ln(x+0)", variables);
            variables[0].Value = 3;
            var val = expression.GetValue();
            var tree = expression.ToExpressionTree();
            tree.Simplify();
            Console.WriteLine(tree.ToString());
            Console.WriteLine("Ready");
        }
    }
}
