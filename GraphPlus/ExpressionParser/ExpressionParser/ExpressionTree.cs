using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    class ExpressionNode
    {
        public object Operation;
        public ArrayList Children = new ArrayList();
        public ExpressionNode() { }
        public ExpressionNode(string operation, object first, object second)
        {
            Operation = operation;
            Children.Add(first);
            Children.Add(second);
        }
        public ExpressionNode(string operation, object first)
        {
            Operation = operation;
            Children.Add(first);
        }
        public ExpressionNode(object operation, ArrayList children)
        {
            Operation = operation;
            Children = children;
        }
        public override string ToString()
        {
            if (Children.Count == 1)
                return Operation + "(" + Children[0].ToString() + ")";
            else if (Children.Count == 2)
                return "(" + Children[0].ToString() + Operation + Children[1].ToString() + ")";
            else
                return "";
        }
    }
    class ExpressionTree
    {
        public object head;
        public void Simplify()
        {
            SimplifyCalculations();
            SimplifyEquilent();
            SimplifyCalculations();
        }
        private void SimplifyCalculations(ExpressionNode node = null, ExpressionNode parent = null)
        {
            if (!(head is ExpressionNode))
                return;
            if (node == null)
                node = head as ExpressionNode;
            for (int i = 0; i < node.Children.Count; i++)
                if (node.Children[i] is ExpressionNode)
                    SimplifyCalculations((ExpressionNode)node.Children[i], node);

            double val = 0;
            if (node.Operation is string)
            {
                if (node.Children[0] is double && node.Children[1] is double)
                {
                    switch (node.Operation)
                    {
                        case "+":
                            val = (double)node.Children[0] + (double)node.Children[1];
                            break;
                        case "-":
                            val = (double)node.Children[0] - (double)node.Children[1];
                            break;
                        case "*":
                            val = (double)node.Children[0] * (double)node.Children[1];
                            break;
                        case "/":
                            val = (double)node.Children[0] / (double)node.Children[1];
                            break;
                        case "^":
                            val = Math.Pow((double)node.Children[0], (double)node.Children[1]);
                            break;
                    }
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = val;
                    }
                    else
                        head = val;
                }
            }
            if (node.Operation is Function)
            {
                int countOfTrue = 0;
                Function func = (Function)node.Operation;
                double[] args = new double[node.Children.Count];
                foreach (object operand in node.Children)
                    if (operand is double)
                        countOfTrue++;
                    else if (operand is Variable && !((Variable)operand).IsParameter)
                        countOfTrue++;
                if(countOfTrue == node.Children.Count)
                {
                    for (int i = 0; i < node.Children.Count; i++)
                        if (node.Children[i] is double)
                            args[i] = (double)node.Children[i];
                        else if(node.Children[i] is Variable)
                            args[i] = ((Variable)node.Children[i]).Value;
                    val = func.GetValue(args);
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = val;
                    }
                    else
                        head = val;
                }
            }
        }
        private void SimplifyEquilent(ExpressionNode node = null, ExpressionNode parent = null)
        {
            if (!(head is ExpressionNode))
                return;
            if (node == null)
                node = head as ExpressionNode;
            for (int i = 0; i < node.Children.Count; i++)
                if (node.Children[i] is ExpressionNode)
                    SimplifyEquilent((ExpressionNode)node.Children[i], node);

            // 0 + ...
            {
                if (node.Operation is string
               && node.Children[0] is double
               && (string)node.Operation == "+"
               && (double)node.Children[0] == 0)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = node.Children[1];
                    }
                    else
                        head = node.Children[1];
                }
                if (node.Operation is string
                    && node.Children[1] is double
                    && (string)node.Operation == "+"
                    && (double)node.Children[1] == 0)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = node.Children[0];
                    }
                    else
                        head = node.Children[0];
                }
            }

            // 0 - ...
            {
                if (node.Operation is string
                && node.Children[0] is double
                && (string)node.Operation == "-"
                && (double)node.Children[0] == 0)
                {
                    node.Operation = "*";
                    node.Children[0] = -1d;
                }
                if (node.Operation is string
                    && node.Children[1] is double
                    && (string)node.Operation == "-"
                    && (double)node.Children[1] == 0)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = node.Children[0];
                    }
                    else
                        head = node.Children[0];
                }
            }

            // 1 * ...
            {
                if (node.Operation is string
                && node.Children[0] is double
                && (string)node.Operation == "*"
                && (double)node.Children[0] == 1)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = node.Children[1];
                    }
                    else
                        head = node.Children[1];
                }
                if (node.Operation is string
                    && node.Children[1] is double
                    && (string)node.Operation == "*"
                    && (double)node.Children[1] == 1)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = node.Children[0];
                    }
                    else
                        head = node.Children[0];
                }
            }

            // ... / 1
            {
                if (node.Operation is string
                    && node.Children[1] is double
                    && (string)node.Operation == "/"
                    && (double)node.Children[1] == 1)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = node.Children[0];
                    }
                    else
                        head = node.Children[0];
                }
            }

            // 0 * ...
            {
                if (node.Operation is string
                && node.Children[0] is double
                && (string)node.Operation == "*"
                && (double)node.Children[0] == 0)
                {
                    if (parent != null)
                        parent.Children.Remove(node);
                    else
                        head = 0d;
                }
                if (node.Operation is string
                    && node.Children[1] is double
                    && (string)node.Operation == "*"
                    && (double)node.Children[1] == 0)
                {
                    if (parent != null)
                        parent.Children.Remove(node);
                    else
                        head = 0;
                }
            }

            // ... ^ 0
            {
                if (node.Operation is string
                    && node.Children[1] is double
                    && (string)node.Operation == "^"
                    && (double)node.Children[1] == 0)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = 1d;
                    }
                    else
                        head = 1d;
                }
            }

            // ... ^ 0
            {
                if (node.Operation is string
                    && node.Children[1] is double
                    && (string)node.Operation == "^"
                    && (double)node.Children[1] == 1d)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = node.Children[0];
                    }
                    else
                        head = node.Children[0];
                }
            }

        }
        public override string ToString()
        {
            return head.ToString();
        }
    }
}
