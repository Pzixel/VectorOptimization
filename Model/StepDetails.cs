namespace Model
{
    public class StepDetails
    {
        public double Lambda { get; private set; }
        public Vector D { get; private set; }
        public int J { get; private set; }
        public Vector StartPoint { get { return start.Coordinates; } }
        public Vector NewPoint { get { return end.Coordinates; } }
        public double OldFuncValue { get { return start.FunctionValue; } }
        public double NewFuncValue { get { return end.FunctionValue; } }
        private readonly SmartPoint start, end;

        public StepDetails(Vector startpoint, double lambda, Vector d, Vector newPoint, int j)
        {
            Lambda = lambda;
            D = d;
            J = j;
            start = new SmartPoint(startpoint);
            end = new SmartPoint(newPoint);
        }
    }
}