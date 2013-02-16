using System.Collections.Generic;

namespace Model
{
    public class IterationDetails
    {
        public int I { get; private set; }
        public Vector StartPoint { get { return point.Coordinates; } }
        public double FunctionValue { get { return point.FunctionValue; } }
        public StepDetails[] StepDetails { get { return details.ToArray(); } }
        private readonly List<StepDetails> details;
        private SmartPoint point;
        

        public IterationDetails(Vector startPoint, int i)
        {
            I = i;
            point = new SmartPoint(startPoint);
            details = new List<StepDetails>();
        }

        public void Add(StepDetails sd)
        {
            details.Add(sd);
        }
    }
}