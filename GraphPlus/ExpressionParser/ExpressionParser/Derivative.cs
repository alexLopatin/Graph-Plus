using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    class Derivative
    {
        Expression expression;
        public Derivative(Expression expr)
        {
            expression = new Expression(expr);
        }
        Variable DiffirentialVariable;
        public Expression Calculate(Variable diffirentialVariable)
        {
            DiffirentialVariable = diffirentialVariable;
            Der();
            expression.Simplify();
            return expression;
        }
        private ExpressionNode Der(ExpressionNode node = null)
        {
            if (!(expression.expTree.head is ExpressionNode))
                return null;
            if (node == null)
                node = expression.expTree.head as ExpressionNode;

            if (node.Operation is string && (((string)node.Operation) == "+" || ((string)node.Operation) == "-"))
            {
                for (int i = 0; i < node.Children.Count; i++)
                    if (node.Children[i] is ExpressionNode)
                        Der((ExpressionNode)node.Children[i]);
                    else if (node.Children[i] is Variable 
                        && ((Variable)node.Children[i]).IsParameter
                        && (Variable)node.Children[i] == DiffirentialVariable)
                        node.Children[i] = 1d;
                    else
                        node.Children[i] = 0d;
                return node;
            }

            if (node.Operation is string && (((string)node.Operation) == "~"))
            {
                    if (node.Children[0] is ExpressionNode)
                        Der((ExpressionNode)node.Children[0]);
                    else if (node.Children[0] is Variable
                        && ((Variable)node.Children[0]).IsParameter
                        && (Variable)node.Children[0] == DiffirentialVariable)
                        node.Children[0] = 1d;
                    else
                        node.Children[0] = 0d;
                return node;
            }

            if (node.Operation is string && (((string)node.Operation) == "*"))
            {
                object a = node.Children[0];
                object b = node.Children[1];

                if (a is ExpressionNode)
                    a = new ExpressionNode(a as ExpressionNode);
                if (b is ExpressionNode)
                    b = new ExpressionNode(b as ExpressionNode);

                ExpressionNode first;
                if (a is ExpressionNode)
                    first = new ExpressionNode("*", Der(new ExpressionNode(a as ExpressionNode)), b);
                else if (a is Variable
                    && ((Variable)a).IsParameter
                    && (Variable)a == DiffirentialVariable)
                    first = new ExpressionNode("*", 1d, b);
                else
                    first = new ExpressionNode("*", 0d, b);


                ExpressionNode second;
                if (b is ExpressionNode)
                    second = new ExpressionNode("*", Der(new ExpressionNode(b as ExpressionNode)), a);
                else if (b is Variable 
                    && ((Variable)b).IsParameter
                    && (Variable)b == DiffirentialVariable)
                    second = new ExpressionNode("*", 1d, a);
                else
                    second = new ExpressionNode("*", 0d, a);
                node.Operation = "+";
                node.Children[0] = first;
                node.Children[1] = second;
                return node;
            }

            if (node.Operation is string && (((string)node.Operation) == "/"))
            {
                object a = node.Children[0];
                object b = node.Children[1];

                if (a is ExpressionNode)
                    a = new ExpressionNode(a as ExpressionNode);
                if (b is ExpressionNode)
                    b = new ExpressionNode(b as ExpressionNode);

                ExpressionNode first;
                if (a is ExpressionNode)
                    first = new ExpressionNode("*", Der(new ExpressionNode(a as ExpressionNode)), b);
                else if (a is Variable 
                    && ((Variable)a).IsParameter
                    && (Variable)a == DiffirentialVariable)
                    first = new ExpressionNode("*", 1d, b);
                else
                    first = new ExpressionNode("*", 0d, b);


                ExpressionNode second;
                if (b is ExpressionNode)
                    second = new ExpressionNode("*", Der(new ExpressionNode(b as ExpressionNode)), a);
                else if (b is Variable 
                    && ((Variable)b).IsParameter
                    && (Variable)b == DiffirentialVariable)
                    second = new ExpressionNode("*", 1d, a);
                else
                    second = new ExpressionNode("*", 0d, a);

                object b1 = b;
                object b2 = b;
                if (b is ExpressionNode)
                {
                    b1 = new ExpressionNode(b as ExpressionNode);
                    b2 = new ExpressionNode(b as ExpressionNode);
                }
                ExpressionNode denominator = new ExpressionNode("*", b1, b2);
                ExpressionNode numerator = new ExpressionNode("-", first, second);

                node.Operation = "/";
                node.Children[0] = numerator;
                node.Children[1] = denominator;

                return node;
            }

            if (node.Operation is string && (((string)node.Operation) == "^"))
            {
                object a = node.Children[0];
                object b = node.Children[1];

                if (a is ExpressionNode)
                    a = new ExpressionNode(a as ExpressionNode);
                if (b is ExpressionNode)
                    b = new ExpressionNode(b as ExpressionNode);

                ExpressionNode first = new ExpressionNode("^", a, b);

                if (a is ExpressionNode)
                    a = new ExpressionNode(a as ExpressionNode);
                if (b is ExpressionNode)
                    b = new ExpressionNode(b as ExpressionNode);

                var ln = StandardFunctions.Get().Find(p => p.Name == "ln");
                ExpressionNode nodeFunction = new ExpressionNode(ln, new ArrayList() {a });
                ExpressionNode second = new ExpressionNode("*", b, nodeFunction);

                node.Operation = "*";
                node.Children[0] = first;
                node.Children[1] = Der(second);
                return node;
            }

            if (node.Operation is Function)
            {
                Function func = node.Operation as Function;
                object a = node.Children[0];
                if (a is ExpressionNode)
                    a = new ExpressionNode(a as ExpressionNode);

                node.Operation = "*";

                ExpressionNode d;
                if (a is ExpressionNode)
                    d = StandardFunctions.GetDerivative(func.Name, new ExpressionNode(a as ExpressionNode));
                else
                    d = StandardFunctions.GetDerivative(func.Name, a);

                if (node.Children.Count == 1)
                    node.Children.Add(d);
                else if (node.Children.Count > 1)
                    node.Children[1] = d;

                if (a is ExpressionNode)
                    node.Children[0] = Der(a as ExpressionNode);
                else if (a is Variable
                    && ((Variable)a).IsParameter
                    && (Variable)a == DiffirentialVariable)
                    node.Children[0] = 1d;
                else
                    node.Children[0] = 0d;
                return node;
            }

            return null;
        }
    }
}
