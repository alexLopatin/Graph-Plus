﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    static class StandardConstants
    {
        private static List<Variable> constants = new List<Variable>()
        {
            new Variable("e", Math.E),
            new Variable("pi", Math.PI)
        };
        public static List<Variable> Get() => constants;

    }
}
