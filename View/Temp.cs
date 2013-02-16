using Model;

namespace View
{
    class Temp1
    {
        public Temp1(int i, string value, Temp2[] step)
        {
            Step = step;
            I = i;
            Value = value;
        }

        public int I { get; private set; }
        public string Value { get; private set; }
        public Temp2[] Step { get; private set; }

    }

    class Temp2
    {
        public Temp2(int j, string startValue, Vector d, double lambda, string newValue)
        {
            J = j;
            StartValue = startValue;
            D = d;
            Lambda = lambda;
            NewValue = newValue;
        }
        public int J { get; private set; }
        public string StartValue { get; private set; }
        public Vector D { get; private set; }
        public double Lambda { get; private set; }
        public string NewValue { get; private set; }
    }
}