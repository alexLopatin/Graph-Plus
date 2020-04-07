﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    class ExpressionParser
    {
        string Expression;
        int curIndex = 0;
        const string EndOfExpression = "?EOE";
        List<Function> Functions;
        List<Variable> Constants;
        List<Variable> Variables;
        Dictionary<char, int> operators = new Dictionary<char, int>()
        {
            { '~', 2 },
            { '+', 1 },
            { '-', 1 },
            { '*', 2 },
            { '/', 2 },
            { '^', 3 },
            { '!', 3 }
        };
        public ExpressionParser(string expression, List<Function> functions, List<Variable> constants, List<Variable> variables)
        {
            Expression = expression.Replace("!", "!1 ");
            Functions = functions;
            Constants = constants;
            Variables = variables;
        }
        bool IsOperator(char c)
        {
            return (c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '!' || c == '~');
        }
        bool IsBracket(char c)
        {
            return (c == '(' || c == ')');
        }
        bool IsComma(char c)
        {
            return (c == ',');
        }
        bool IsFunction(string token)
        {
            return Functions.Exists(p => p.Name == token);
        }
        bool IsVariable(string token)
        {
            return Variables.Exists(p => p.Name == token);
        }
        bool IsConstant(string token)
        {
            return Constants.Exists(p => p.Name == token);
        }
        public string ReadNext()
        {
            while (curIndex != Expression.Length && Expression[curIndex] == ' ')
                curIndex++;
            if (curIndex == Expression.Length)
                return EndOfExpression;
            int length = 0;
            if (IsOperator(Expression[curIndex]) || IsBracket(Expression[curIndex]) || IsComma(Expression[curIndex]))
                return Expression[curIndex++].ToString();
            for (; curIndex < Expression.Length && Char.IsLetterOrDigit(Expression[curIndex]); curIndex++)
                length++;
            return Expression.Substring(curIndex - length, length);
        }
        public Queue Build()
        {
            Queue queue = new Queue();
            Stack stack = new Stack();
            string prevToken = "";
            string token = "";
            while ((token = ReadNext()) != EndOfExpression)
            {
                if (token == "-" && ((!Double.TryParse(prevToken, out _) && prevToken != ")") || prevToken == ""))
                    token = "~";
                if (Double.TryParse(token, out _))
                    queue.Enqueue(token);
                if(IsVariable(token))
                    queue.Enqueue(Variables.Find(p => p.Name == token));
                if (IsConstant(token))
                    queue.Enqueue(Constants.Find(p => p.Name == token));
                if (IsFunction(token))
                    stack.Push( Functions.Find(p=> p.Name == token));
                if (IsComma(token[0]))
                    while (stack.Count > 0 && stack.Peek() as string != "(")
                        queue.Enqueue(stack.Pop());
                if (IsOperator(token[0]))
                {
                    while (stack.Count > 0
                        && IsOperator(((string)stack.Peek())[0])
                        && operators[((string)stack.Peek())[0]] >= operators[token[0]])
                        queue.Enqueue(stack.Pop());
                    stack.Push(token);
                }
                if (token[0] == '(')
                    stack.Push(token);
                if (token[0] == ')')
                {
                    while (stack.Count > 0 && stack.Peek() as string != "(")
                        queue.Enqueue(stack.Pop());
                    if (stack.Count > 0)
                        stack.Pop();
                    if (stack.Count > 0 && stack.Peek() is Function)
                        queue.Enqueue(stack.Pop());
                }
                prevToken = token;
            }
            while (stack.Count > 0)
                queue.Enqueue(stack.Pop());
            return queue;
        }
    }
}
