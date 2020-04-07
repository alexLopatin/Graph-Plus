﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    class Expression
    {
        Queue queue;
        public ExpressionTree expTree;
        public List<Variable> Variables;
        public void Parse(string textExp, List<Variable> variables)
        {
            queue = new ExpressionParser(textExp, 
                StandardFunctions.Get(),
                StandardConstants.Get(),
                variables
                ).Build();
            Variables = variables;
            ToExpressionTree();
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
        public double GetValue() => expTree.GetValue();
        public void Simplify() => expTree.Simplify();
        private void ToExpressionTree()
        {
            expTree = new ExpressionTree();
            int cur = 0;
            Stack s = new Stack();
            ArrayList listQueue = new ArrayList(queue);
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
            expTree.head = s.Peek();
        }
        public override string ToString() => expTree.ToString();
    }
}
