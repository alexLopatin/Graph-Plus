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
        public double Factorial(double f)
        {
            if (f == 0)
                return 1;
            else
                return f * Factorial(f - 1);
        }
        public double GetValue()
        {
            if(Operation is Function)
            {
                Function func = Operation as Function;
                double[] args = new double[func.CountOfArguments];
                for (int i = 0; i < func.CountOfArguments; i++)
                    if(Children[i] is ExpressionNode)
                        args[i] = ((ExpressionNode)Children[i]).GetValue();
                    else if(Children[i] is Variable)
                        args[i] = ((Variable)Children[i]).Value;
                    else
                        args[i] = (double)Children[i];
                return func.GetValue(args);
            }
            if (Operation is string)
            {
                string op = (string)Operation;
                double b = 0;
                double a = 0;
                if (Children[0] is ExpressionNode)
                    a = ((ExpressionNode)Children[0]).GetValue();
                else if (Children[0] is Variable)
                    a = ((Variable)Children[0]).Value;
                else
                    a = (double)Children[0];
                if(op != "!")
                {
                    if (Children[1] is ExpressionNode)
                        b = ((ExpressionNode)Children[1]).GetValue();
                    else if (Children[1] is Variable)
                        b = ((Variable)Children[1]).Value;
                    else
                        b = (double)Children[1];
                }
                
                double val = 0;
                switch (op)
                {
                    case "+":
                        val = a + b;
                        break;
                    case "-":
                        val = a - b;
                        break;
                    case "*":
                        val = a * b;
                        break;
                    case "/":
                        val = a / b;
                        break;
                    case "^":
                        val = Math.Pow(a, b);
                        break;
                    case "!":
                        val = Factorial((int)a);
                        break;
                }
                return val;
            }
            return 0;
        }
    }
    class ExpressionTree
    {
        public object head;
        public void Simplify()
        {
            bool changed = true;
            while(changed)
            {
                changed = false;
                SimplifyCalculations(ref changed);
                SimplifyEquilent(ref changed);
            } 
        }
        private void SimplifyCalculations(ref bool changed, ExpressionNode node = null, ExpressionNode parent = null)
        {
            if (!(head is ExpressionNode))
                return;
            if (node == null)
                node = head as ExpressionNode;
            for (int i = 0; i < node.Children.Count; i++)
                if (node.Children[i] is ExpressionNode)
                    SimplifyCalculations(ref changed, (ExpressionNode)node.Children[i], node);

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
                    changed = true;
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
                    changed = true;
                }
            }
        }
        private void SimplifyEquilent(ref bool changed, ExpressionNode node = null, ExpressionNode parent = null)
        {
            if (!(head is ExpressionNode))
                return;
            if (node == null)
                node = head as ExpressionNode;
            for (int i = 0; i < node.Children.Count; i++)
                if (node.Children[i] is ExpressionNode)
                    SimplifyEquilent(ref changed, (ExpressionNode)node.Children[i], node);

            // 0 + ...
            {
                if (node.Operation is string
               && node.Children.Count == 2
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
                    changed = true;
                }
                if (node.Operation is string
                    && node.Children.Count == 2
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
                    changed = true;
                }
            }

            // 0 - ...
            {
                if (node.Operation is string
                    && node.Children.Count == 2
                    && node.Children[0] is double
                    && (string)node.Operation == "-"
                    && (double)node.Children[0] == 0)
                {
                    node.Operation = "*";
                    node.Children[0] = -1d;
                    changed = true;
                }
                if (node.Operation is string
                    && node.Children.Count == 2
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
                    changed = true;
                }
            }

            // 1 * ...
            {
                if (node.Operation is string
                    && node.Children.Count == 2
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
                    changed = true;
                }
                if (node.Operation is string
                    && node.Children.Count == 2
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
                    changed = true;
                }
            }

            // ... / 1
            {
                if (node.Operation is string
                    && node.Children.Count == 2
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
                    changed = true;
                }
            }

            // 0 * ...
            {
                if (node.Operation is string
                    && node.Children.Count == 2
                    && node.Children[0] is double
                    && (string)node.Operation == "*"
                    && (double)node.Children[0] == 0)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = 0d;
                    }
                        
                    else
                        head = 0d;
                    changed = true;
                }
                if (node.Operation is string
                    && node.Children.Count == 2
                    && node.Children[1] is double
                    && (string)node.Operation == "*"
                    && (double)node.Children[1] == 0)
                {
                    if (parent != null)
                    {
                        int index = parent.Children.IndexOf(node);
                        parent.Children[index] = 0d;
                    }
                    else
                        head = 0d;
                    changed = true;
                }
            }

            // ... ^ 0
            {
                if (node.Operation is string
                    && node.Children.Count == 2
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
                    changed = true;
                }
            }

            // ... ^ 0
            {
                if (node.Operation is string
                    && node.Children.Count == 2
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
                    changed = true;
                }
            }

        }
        public override string ToString()
        {
            return head.ToString();
        }
        public double GetValue()
        {
            if (head is ExpressionNode)
                return ((ExpressionNode)head).GetValue();
            else if (head is Variable)
                return ((Variable)head).Value;
            else
                return (double)head;
        }
    }
}
