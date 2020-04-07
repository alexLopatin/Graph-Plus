using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    class Expression
    {
        Queue queue;
        ArrayList listQueue;
        public List<Variable> Variables;
        public void Parse(string textExp, List<Variable> variables)
        {
            queue = new ExpressionParser(textExp, 
                StandardFunctions.Get(),
                StandardConstants.Get(),
                variables
                ).Build();
            Variables = variables;
            listQueue = new ArrayList(queue);
        }
        public bool IsOperator(object obj)
        {
            if(obj is string)
            {
                char c = ((string)obj)[0];
                return (c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '!');
            }
            return false;
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
            int cur = 0;
            Stack s = new Stack();
            while (cur < listQueue.Count)
            {
                if (IsOperator(listQueue[cur]))
                {
                    char op = ((string)listQueue[cur++])[0];
                    double b = (Double)s.Pop();
                    double a = (Double)s.Pop();
                    switch (op)
                    {
                        case '+':
                            s.Push(a + b);
                            break;
                        case '-':
                            s.Push(a - b);
                            break;
                        case '*':
                            s.Push(a * b);
                            break;
                        case '/':
                            s.Push(a / b);
                            break;
                        case '^':
                            s.Push(Math.Pow(a, b));
                            break;
                        case '!':
                            s.Push(Factorial((int)a));
                            break;
                    }
                }
                else if (listQueue[cur] is Function)
                {
                    Function func = (Function)listQueue[cur++];
                    double[] arguments = new double[func.CountOfArguments];
                    for(int i = 0; i < func.CountOfArguments; i++)
                        arguments[i] = (Double)s.Pop();
                    s.Push(func.GetValue(arguments));
                }
                else if (listQueue[cur] is Variable)
                    s.Push(((Variable)listQueue[cur++]).Value);
                else
                    s.Push(Double.Parse((string)listQueue[cur++]));
            }
            return (double)s.Peek();
        }
        public ExpressionTree ToExpressionTree()
        {
            ExpressionTree tree = new ExpressionTree();
            int cur = 0;
            Stack s = new Stack();
            while (cur < listQueue.Count)
            {
                if (IsOperator(listQueue[cur]))
                {
                    char op = ((string)listQueue[cur++])[0];
                    var b = s.Pop();
                    var a = s.Pop();
                    switch (op)
                    {
                        case '+':
                            s.Push(new ExpressionNode("+", a, b));
                            break;
                        case '-':
                            s.Push(new ExpressionNode("-", a, b));
                            break;
                        case '*':
                            s.Push(new ExpressionNode("*", a, b));
                            break;
                        case '/':
                            s.Push(new ExpressionNode("/", a, b));
                            break;
                        case '^':
                            s.Push(new ExpressionNode("^", a, b));
                            break;
                        case '!':
                            s.Push(new ExpressionNode("!", a));
                            break;
                    }
                }
                else if (listQueue[cur] is Function)
                {
                    Function func = (Function)listQueue[cur++];
                    ArrayList arguments = new ArrayList();
                    for (int i = 0; i < func.CountOfArguments; i++)
                        arguments.Add(s.Pop());
                    s.Push( new ExpressionNode(func, arguments));
                }
                else if (listQueue[cur] is Variable)
                    s.Push(listQueue[cur++]);
                else
                    s.Push(Double.Parse((string)listQueue[cur++]));
            }
            tree.head = (ExpressionNode)s.Peek();
            return tree;
        }
    }
}
