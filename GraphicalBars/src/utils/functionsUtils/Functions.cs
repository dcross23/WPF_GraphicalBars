using System;

namespace GraphicalBars.src.utils.functionsUtils
{
    //Function that calculates the value of x in a polynomial
    public class PolynomialFunction : AbstractFunction
    {
        public PolynomialFunction() 
        {
            this.Values = new double[6];
            this.Formula = "a(x^5)+b(x^4)+c(x^3)+d(x^2)+e(x)+f";
        }

        public override double calculate(double x)
        {
            return (Values[0]*Math.Pow(x,5) + Values[1] * Math.Pow(x, 4) + Values[2] * Math.Pow(x, 3) + Values[3] * Math.Pow(x, 2) + Values[4] * x + Values[5]);
        }
    }

    //Function that calculates the cos of x
    public class CosFunction : AbstractFunction
    {
        public CosFunction()
        {
            this.Values = this.Values = new double[6];
            this.Formula = "a + b*cos(x)";
        }

        public override double calculate(double x)
        {
            return (Values[0] + Values[1] * Math.Cos(x));
        }
    }

    //Function that calculates the sin of x
    public class SinFunction : AbstractFunction
    {
        public SinFunction()
        {
            this.Values = new double[6];
            this.Formula = "a + b*sen(x)";
        }

        public override double calculate(double x)
        {
            return (Values[0] + Values[1] * Math.Sin(x));
        }
    }

    //Function that calculates the absolute value of x
    public class AbsFunction : AbstractFunction
    {
        public AbsFunction()
        {
            this.Values = new double[6];
            this.Formula = "|x|";
        }

        public override double calculate(double x)
        {
            return Math.Abs(x);
        }
    }
}
