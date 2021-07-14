namespace GraphicalBars.src.utils
{
    public abstract class AbstractFunction
    {
        //values -> 0:a, 1:b, 2:c, 3:d, 4:e, 5:f
        public double[] Values { get; set; }
        public string Formula { get; set; }

        public abstract double calculate(double x);
    }
}
