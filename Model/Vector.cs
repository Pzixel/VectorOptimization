using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public struct Vector : IEnumerable<double>
    {
        private readonly double[] value;

        public bool IsEmpty
        {
            get { return value == null || Length == 0; }
        }

        public double Norm { get { return Math.Sqrt(value.Select(x => x*x).Sum()); } }

        public Vector(int length)
            : this(new double[length])
        {
        }

        public Vector(params double[] value)
        {
            this.value = (double[])value.Clone();
        }

        public int Length
        {
            get { return value.Length; }
        }

        public void Normalize()
        {
            var norm = Norm;
            double[] v = this.Select(x => x / norm).ToArray();
            this = new Vector(v);
        }

        public static Vector operator *(double scalar, Vector vector)
        {
            return new Vector(vector.Select(x => x * scalar).ToArray());
        }

        public static Vector operator *(Vector vector, double scalar)
        {
            return scalar * vector;
        }

        public static Vector operator +(double scalar, Vector vector)
        {
            return new Vector(vector.Select(x => x + scalar).ToArray());
        }

        public static Vector operator +(Vector vector, double scalar)
        {
            return scalar + vector;
        }

        public static Vector operator +(Vector one, Vector another)
        {
            return new Vector(one.Zip(another, (x, y) => x + y).ToArray());
        }

        public static Vector operator -(Vector one, Vector another)
        {
            return new Vector(one.Zip(another, (x, y) => x - y).ToArray());
        }

        public static Vector operator /(Vector vector, double scalar)
        {
            return new Vector(vector.Select(x => x / scalar).ToArray());
        }

        public static double TransposeAndMultiply(Vector one, Vector another)
        {
            return one.Zip(another, (x, y) => x * y).Sum();
        }

        public double this[int index]
        {
            get { return value[index]; }
            set { this.value[index] = value; }
        }

        public IEnumerator<double> GetEnumerator()
        {
            return ((IEnumerable<double>)value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator double[](Vector vector)
        {
            return vector.value;
        }

        public static implicit operator Vector(double[] value)
        {
            return new Vector(value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder(Length * 5);
            sb.Append('[');
            foreach (var v in this)
            {
                sb.Append(double.IsInfinity(v) ? "∞" : double.IsNegativeInfinity(v) ? "-∞" : v.ToString("N3"));
                sb.Append(" ; ");
            }
            sb.Remove(sb.Length - 3, 3);
            sb.Append(']');
            return sb.ToString();
        }
    }
}