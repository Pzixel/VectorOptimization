using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace View
{
    public class MathEvaluator
    {
        private readonly Delegate parsedFunction;
        private readonly string normalized;
        public double Invoke(double[] x)
        {
            if (parsedFunction == null)
                throw new NullReferenceException("No function to invoke");
            return (double)parsedFunction.DynamicInvoke(x);
        }
        private const string Begin =
            @"using System;
namespace MyNamespace
{
    public static class LambdaCreator 
    {
        public static Func<double[],double> Create()
        {
            return (x)=>";
        private const string End = @";
        }
    }
}";

        public MathEvaluator(string input)
        {
            input = input.Replace("x", "(x[0])").Replace("y", "(x[1])");
            input = Regex.Replace(input, @"([-\+])", @" $1 ");
            normalized = Normalize(input);
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters { GenerateInMemory = true };
            parameters.ReferencedAssemblies.Add("System.dll");
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, Begin + normalized + End);
            try
            {
                var cls = results.CompiledAssembly.GetType("MyNamespace.LambdaCreator");
                var method = cls.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
                parsedFunction = (method.Invoke(null, null) as Delegate);
            }
            catch (FileNotFoundException)
            {
                throw new ArgumentException();
            }
        }

        private string Normalize(string input)
        {
            string result = input.ReplaceMath().ReplaceMultipling().ReplacePow().ReplaceToDoubles();
            return result == input ? result : Normalize(result);
        }
    }


    public static class StringHelper
    {
        public static string ReplaceMultipling(this string input)
        {
            string result = Regex.Replace(input, @"(\d)([A-Za-z])", @"$1*$2");
            result = Regex.Replace(result, @"(?<=\W)(\w)\(", @"$1*(");
            result = Regex.Replace(result, @"(x\[\d\])\(", @"$1*(");
            return Regex.Replace(result, @"\)(\w)", @")*$1");
        }

        public static string ReplacePow(this string input)
        {
            var result = input.ReplacePow(@"\(([^\^]+)\)\^(\S+)");
            result = result.ReplacePow(@"(\d*(x\[\d\])?)\^(\(\S+\))");
            result = result.ReplacePow(@"(\d+)\^(\d*(x\[\d\])?)");
            return result.ReplacePow(@"(\d*(x\[\d\])?)\^(\d+\.?\d*)");
        }

        private static string ReplacePow(this string input, string toReplace)
        {
            return Regex.Replace(input, toReplace, "Math.Pow($1,$2)");
        }

        public static string ReplaceToDoubles(this string input)
        {
            return Regex.Replace(input, @"(?<![.\[])(\d+)(?![\]\.])", "$1.0");
        }

        public static string ReplaceMath(this string input)
        {
            return
                input.ReplaceMath("sin", @"Math.Sin")
                     .ReplaceMath("cos", @"Math.Cos")
                     .ReplaceMath("ctg", @"1.0/Math.Tan")
                     .ReplaceMath("tg", @"Math.Tan");
        }

        private static string ReplaceMath(this string input, string name, string dotNetName)
        {
            return Regex.Replace(input, @"(?<!Math\.)" + name, dotNetName, RegexOptions.IgnoreCase);
        }
    }
}