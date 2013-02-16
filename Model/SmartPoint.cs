using System;

namespace Model
{
    public struct SmartPoint
    {
        public SmartPoint(Vector value)
            : this()
        {
            Coordinates = value;
        }

        public static void SetFunction(Func<double[], double> fun)
        {
            function = fun;
        }


        private static Func<double[], double> function;
        public Vector Coordinates { get; private set; }
        public double FunctionValue { get { return function(Coordinates); } }
    }
}