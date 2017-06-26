using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphPlus
{
    class MathExpression
    {
        public List<string> structure; 
        public bool IsNumber(char c)
        {
            char[] numbers = new char[] {'1', '2', '3', '4', '5', '6', '7', '8', '9' };
            
            return numbers.ToList().Exists(p=>p==c);
        }
        public MathExpression(string expression)
        {
            bool lastNumber = false;
            List<string> local = new List<string>();
            List<string> output = new List<string>();
            expression = expression.Insert(expression.Count(), "|");
            while (expression[0] != '|')
            {
                if (IsNumber(expression[0]))
                    if (lastNumber)
                        output[output.Count - 1] = output[output.Count - 1] + expression[0];
                    else
                    {
                        output.Add(expression[0].ToString());
                        lastNumber = true;
                    }
                        
                else
                    lastNumber = false;





                expression = expression.Remove(0, 1);
            }
            structure = output;
        }
         
    }
}
