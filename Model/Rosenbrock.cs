using System;
using System.Collections.Generic;

namespace Model
{
    public enum OptimizationDirection
    {
        ToMin,ToMax
    }

    public class Rosenbrock
    {
        private readonly Func<double[], double> f;
        private readonly double epsilon;
        private Vector y;
        private Vector[] d;
        private readonly int n;
        private readonly List<IterationDetails> details = new List<IterationDetails>();
        public OptimizationDirection Direction = OptimizationDirection.ToMin;

        public IEnumerable<IterationDetails> Details { get { return new List<IterationDetails>(details); } }

        public Rosenbrock(Func<double[], double> f, Vector startPoint, double epsilon)
        {
            if (startPoint.IsEmpty)
                throw new ArgumentException("startPoint");
            n = startPoint.Length;
            this.f = x => Direction == OptimizationDirection.ToMin ? f(x) : -f(x);
            this.epsilon = epsilon;
            y = startPoint;
            d = new Vector[n];
            for (int i = 0; i < n; i++)
            {
                d[i] = new Vector(n);
                d[i][i] = 1;
            }
            SmartPoint.SetFunction(f);
        }

        public Vector Solve()
        {
            int k = 0;
            details.Clear();
            var lambda = new Vector(n);
            Vector previousIterationY;
            do
            {
                k++;
                previousIterationY = y;
                var iterDetails = new IterationDetails(y, k);
                for (int j = 0; j < n; j++)
                {
                    var previousStepY = y;
                    int i = j;
                    lambda[j] = Dichotomy(x => LambdaSolve(x, i), y[j]);
                    y += lambda[j] * d[j];
                    iterDetails.Add(new StepDetails(previousStepY, lambda[j], d[j], y, i+1));
                }
                details.Add(iterDetails);
                d = GramSchmidtProcess(lambda);
            } while ((y - previousIterationY).Norm >= epsilon);
            return y;
        }

        private double LambdaSolve(double x, int j)
        {
            return f(y + x * d[j]);
        }

        private Vector[] GramSchmidtProcess(Vector lambda)
        {
            var a = new Vector[n];
            for (int j = 0; j < n; j++)
                if (Math.Abs(lambda[j]) < epsilon) //Если ноль
                    a[j] = d[j];
                else
                {
                    a[j] = new Vector(n);
                    for (int i = j; i < n; i++)
                        a[j] += lambda[i] * d[i];
                }
            var b = (Vector[])a.Clone();
            b[0].Normalize();
            for (int j = 1; j < n; j++)
            {
                for (int i = 0; i < j; i++)
                    b[j] -= Vector.TransposeAndMultiply(a[j], b[i]) * b[i];
                b[j].Normalize();
            }
            return b;
        }

        private double Dichotomy(Func<double, double> func, double startCoord)
        {
            double deviation = Math.Abs(startCoord) < double.Epsilon ? 5 : Math.Abs(startCoord) * 0.5 + 10;
            double a = startCoord - deviation, b = startCoord + deviation;
            double delta = epsilon / 10;
            while (b - a >= epsilon)
            {
                double middle = (a + b) / 2;
                double lambda = middle - delta, mu = middle + delta;
                if (func(lambda) < func(mu))
                    b = mu;
                else
                    a = lambda;
            }
            return (a + b) / 2;
        }
    }
}
