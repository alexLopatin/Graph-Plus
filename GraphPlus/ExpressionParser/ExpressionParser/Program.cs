using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionParser
{
    class ExpressionParser
    {
        string Expression;
        int curIndex = 0;
        const string EndOfExpression = "?EOE";
        Dictionary<char, int> operators = new Dictionary<char, int>()
        {
            { '+', 1 },
            { '-', 1 },
            { '*', 2 },
            { '/', 2 },
            { '^', 3 },
            { '!', 3 }
        };
        public ExpressionParser(string expression)
        {
            Expression = expression.Replace("!", "!1");
        }
        bool IsOperator(char c)
        {
            return (c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c =='!');
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
            return (token == "sin");
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
            string token = "";
            while((token = ReadNext()) != EndOfExpression)
            {
                if (Double.TryParse(token, out _) || token == "x")
                    queue.Enqueue(token);
                if (IsFunction(token))
                    stack.Push(token);
                if (IsComma(token[0]))
                    while ( stack.Count > 0 &&  stack.Peek() as string != "(")
                        queue.Enqueue(stack.Pop());
                if(IsOperator(token[0]))
                {
                    while (stack.Count > 0 
                        && IsOperator(((string)stack.Peek())[0])
                        && operators[((string)stack.Peek())[0]] >= operators[token[0] ])
                        queue.Enqueue(stack.Pop());
                    stack.Push(token);
                }
                if (token[0] == '(')
                    stack.Push(token);
                if (token[0] == ')')
                {
                    while (stack.Count > 0 && stack.Peek() as string != "(")
                        queue.Enqueue(stack.Pop());
                    if(stack.Count > 0)
                        stack.Pop();
                    if(stack.Count > 0 && IsFunction(stack.Peek() as string))
                        queue.Enqueue(stack.Pop());
                }
            }
            while (stack.Count > 0)
                queue.Enqueue(stack.Pop());
            return queue;
        }
    }
    class Expression
    {
        Queue queue;
        ArrayList listQueue;
        public void Parse(string textExp)
        {
            queue = new ExpressionParser(textExp).Build();
            listQueue = new ArrayList(queue);
        }
        public bool IsOperator(char c)
        {
            return (c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '!');
        }
        public double Factorial(double f)
        {
            if (f == 0)
                return 1;
            else
                return f * Factorial(f - 1);
        }
        public double GetValue(double x)
        {
            int cur = 0;
            Stack s = new Stack();
            while(cur < listQueue.Count)
            {
                if (IsOperator(((string)listQueue[cur])[0]))
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
                else if ((string)listQueue[cur] == "sin")
                {
                    cur++;
                    double a = (Double)s.Pop();
                    s.Push(Math.Sin(a));
                }
                else if ((string)listQueue[cur] == "x")
                {
                    cur++;
                    s.Push(x);
                }  
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
                if (IsOperator(((string)listQueue[cur])[0]))
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
                else if ((string)listQueue[cur] == "sin")
                {
                    cur++;
                    var a = s.Pop();
                    s.Push(new ExpressionNode("sin", a));
                }
                else if ((string)listQueue[cur] == "x")
                {
                    cur++;
                    s.Push("x");
                }
                else
                    s.Push(Double.Parse((string)listQueue[cur++]));
            }
            tree.head = (ExpressionNode)s.Peek();
            return tree;
        }
    }
    class Derivative
    {
        ExpressionTree tree;
        
        public ExpressionTree Calculate()
        {

            return tree;

        }
    }
    class ExpressionNode
    {
        public string Operation;
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
        public override string ToString()
        {
            if (Children.Count == 1)
                return Operation + "(" + Children[0].ToString() + ")";
            else if (Children.Count == 2)
                return "(" + Children[0].ToString() +  Operation + Children[1].ToString() + ")";
            else
                return "";
        }
    }
    class ExpressionTree
    {
        public object head;
        public void Simplify(ExpressionNode node = null, ExpressionNode parent = null)
        {
            if (node == null)
                node = head as ExpressionNode;
            for (int i = 0; i < node.Children.Count; i++)
                if (node.Children[i] is ExpressionNode)
                    Simplify((ExpressionNode)node.Children[i], node);
            double val = 0;
            if (node.Children.Count == 1 && node.Children[0] is double)
            {
                val = Math.Sin((double)node.Children[0]);
                if (parent != null)
                {
                    int index = parent.Children.IndexOf(node);
                    parent.Children[index] = val;
                }
                else
                    head = val;
            }
                
            if (node.Children.Count == 2)
            {
                if(node.Children[0] is double && node.Children[1] is double)
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
                            val = Math.Pow( (double)node.Children[0], (double)node.Children[1]);
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
            
        }
        public override string ToString()
        {
            return head.ToString();
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            //ExpressionParser parser = new ExpressionParser("2 +3-4* (1 + 2 *(sin(5)))^  2");
            //ExpressionParser parser = new ExpressionParser("2 + 1");
            Expression expression = new Expression();
            expression.Parse("2 +3-4* (1 + 2 *(sin(5-8^3)))^  x");
            //for (double i = 0; i < 90; i+= 0.01)
            //    Console.WriteLine(expression.GetValue(i));
            var val = expression.GetValue(3);
            var tree = expression.ToExpressionTree();
            tree.Simplify();
            Console.WriteLine(tree.ToString());
            Console.WriteLine("Ready");
        }
    }
}
