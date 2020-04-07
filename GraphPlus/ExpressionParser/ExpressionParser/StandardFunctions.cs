using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    static class StandardFunctions
    {
        private static List<Function> functions = new List<Function>()
        {
            new Function("sin", 1, (x) => Math.Sin(x[0])),
            new Function("cos", 1, (x) => Math.Cos(x[0])),
            new Function("tg", 1, (x) => Math.Tan(x[0])),
            new Function("ctg", 1, (x) => 1/Math.Tan(x[0])),
            new Function("ln", 1, (x) => Math.Log(x[0])),
            new Function("log", 2, (x) => Math.Log(x[0], x[1])),
            new Function("max", 2, (x) => Math.Max(x[0], x[1])),
            new Function("min", 2, (x) => Math.Log(x[0], x[1])),
            new Function("abs", 1, (x) => Math.Abs(x[0])),
            new Function("sqrt", 1, (x) => Math.Sqrt(x[0])),
            new Function("arccos", 1, (x) => Math.Acos(x[0])),
            new Function("arcsin", 1, (x) => Math.Asin(x[0])),
            new Function("arctg", 1, (x) => Math.Atan(x[0]))
        };
        public static List<Function> Get() => functions;

    }
}
