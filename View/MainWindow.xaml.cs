using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Model;
using Vector = Model.Vector;

namespace View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Rosenbrock rosenbrock;
        private Func<double[], double> function;

        private bool ToMin
        {
            get { return ToMinRadio.IsChecked ?? true; }
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Epsilon { get; set; }
        private readonly BackgroundWorker worker;
        private bool isModified;

        public MainWindow()
        {
            InitializeComponent();
            MinWidth = Width;
            MinHeight = Height;
            X = Y = 0;
            Epsilon = 0.001;
            Spn.DataContext = this;
            EpsilonTB.DataContext = this;
            ToMinRadio.IsChecked = true;
            FirstFunctionRadio.IsChecked = true;
            worker = new BackgroundWorker();
            worker.DoWork += (s, e) => e.Result = rosenbrock.Solve();
            worker.RunWorkerCompleted += (o, eventArgs) =>
            {
                if (!eventArgs.Cancelled)
                    OnSolve((Vector)eventArgs.Result);
            };
        }


        private void SolveClick(object sender, RoutedEventArgs args)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("Обрабатывается предыдущий запрос. Пожалуйста, подождите");
                return;
            }
            if (function == null)
            {
                MessageBox.Show("Неверно задана функция");
                return;
            }
            rosenbrock = new Rosenbrock(function,new Vector(X,Y), Epsilon)
                {
                    Direction = ToMin ? OptimizationDirection.ToMin : OptimizationDirection.ToMax
                };
            worker.RunWorkerAsync();
        }

        private void OnSolve(Vector vector)
        {
            ResultPanel.DataContext =
                new
                    {
                        Result = string.Format("[{0:N6} ; {1:N6}]", vector[0], vector[1]),
                        FunctionValue = function(vector),
                        Iterations = rosenbrock.Details.Count()
                    };
            ResultDetails.ItemsSource = rosenbrock.Details;
        }

        private void SetFirstFunction(object sender, RoutedEventArgs e)
        {
            function = x => Math.Pow(3*x[0] - x[1], 2) + Math.Pow(2*x[0] - 3 * x[1], 2);
        }

        private void SetSecondFunction(object sender, RoutedEventArgs e)
        {
            function = x => 9*x[0]*(x[0] - 10) + 16*x[1]*(x[1] - 8);
        }

        private void SetCustomFunction(object sender, MouseEventArgs e)
        {
            var textBox = (TextBox) sender;
            if (textBox.Text == string.Empty || !isModified)
                return;
            isModified = false;
            try
            {
                var eva = new MathEvaluator(textBox.Text);
                function = eva.Invoke;
            }
            catch (ArgumentException)
            {
                function = null;
            }
        }

        private void SetModified(object sender, TextChangedEventArgs e)
        {
            isModified = true;
        }
    }
}
