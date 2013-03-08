using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationFinder.DomainLogic
{
    public static class Parser
    {

        private static List<string> _firstOperators = new List<string>() { "*", "/" };
        private static List<string> _secondOperators = new List<string>() { "+", "-" };
        private static List<string> _parenthesis = new List<string>() { "(", ")" };

        public static decimal ParseEquation(string equation) 
        {

            //set the equation
            equation = equation.Replace(" ", "").Trim();

            //parse out the equation
            equation = Parser.ProcessParenthesis(equation);
            equation = Parser.ProcessOperators(Parser._firstOperators, equation);
            equation = Parser.ProcessOperators(Parser._secondOperators, equation);

            //return the result
            return Convert.ToDecimal(equation);
            
        }

        private static string ProcessParenthesis(string equation)
        {

            //get the index of a parenthesis
            var index = equation.LastIndexOf("(");

            //while we found our opening parenthesis
            while (index >= 0)
            {

                //get the subequation that we need to process
                var subEquation = equation.Substring(index);

                //get the ending parentheses
                var indexRight = subEquation.IndexOf(")");

                //throw and error if the closing parenthesis isn't found
                if (indexRight < 0)
                    throw new Exception();

                //get the equation we need to process
                var equationToProcess = subEquation.Substring(0, indexRight).Trim(new char[] { '(' });

                //parse the equation
                var result = Parser.ParseEquation(equationToProcess);

                //update our equation
                equation = equation.Replace("(" + equationToProcess + ")", result.ToString());

                //get the next parenthesis that we need to process
                index = equation.LastIndexOf("(");

            }

            return equation;

        }

        private static string ProcessOperators(List<string> operators, string equation)
        {

            //for each operator
            foreach(var op in operators) 
            {

                //find the operator
                var index = equation.IndexOf(op);

                //while we found our operator
                while (index > 0)
                {

                    //get my two number
                    var leftNumber = Parser.GetNumber(index, true, equation);
                    var rightNumber = Parser.GetNumber(index, false, equation);

                    //get the result of the operations
                    var result = Parser.ProcessOperator(leftNumber, rightNumber, op);

                    //update the equation
                    equation = equation.Replace(string.Format("{0}{1}{2}", leftNumber.ToString(), op, rightNumber.ToString()), result.ToString());

                    //try and see if we have that operator again
                    index = equation.IndexOf(op);

                }

            }

            //return the parsed equation
            return equation;

        }

        private static decimal ProcessOperator(decimal leftNumber, decimal rightNumber, string op)
        {

            if (op == "+")
                return leftNumber + rightNumber;
            else if (op == "-")
                return leftNumber - rightNumber;
            else if (op == "/")
                return leftNumber / rightNumber;
            else if (op == "*")
                return leftNumber * rightNumber;
            else
                throw new Exception();

        }

        private static decimal GetNumber(int index, bool left, string equation)
        {
            
            //go back or forth one digit
            if (left)
                index--;
            else
                index++;
            
            //setup our number
            var number = 0;

            if (int.TryParse(equation[index].ToString(), out number))
            {

                int i = index;
                string value = "";
                while (int.TryParse(equation[i].ToString(), out number))
                {
                    
                    //go back or forth one digit
                    if (left)
                    {
                        value = string.Format("{0}{1}", number.ToString(), value);
                        i--;
                    }
                    else
                    {
                        value = string.Format("{0}{1}", value, number.ToString());
                        i++;
                    }

                    if (i >= equation.Length || i < 0)
                        return Convert.ToDecimal(value);
                    
                }

                //return the number
                return Convert.ToDecimal(value);

            }
            else
            {

                throw new Exception();

            }

        }

    }
}
