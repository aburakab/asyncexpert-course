using System;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace Dotnetos.AsyncExpert.Homework.Module01.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
#if RELEASE
            BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
#else
            while(true)
            {
                Console.WriteLine("Type 0 to exit!");
                Console.Write("Number: ");
                var strNumber = Console.ReadLine();
                if (strNumber == "0")
                    break;

                if (ulong.TryParse(strNumber, out ulong number))
                {
                    var calc = new FibonacciCalc();
                    Console.WriteLine(calc.Recursive(number));
                    Console.WriteLine(calc.RecursiveWithMemoization(number));
                    Console.WriteLine(calc.Iterative(number));
                }
            }
#endif
        }
    }
}
