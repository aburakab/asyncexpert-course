using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Dotnetos.AsyncExpert.Homework.Module01.Benchmark
{
    [DisassemblyDiagnoser(exportCombinedDisassemblyReport: true, exportGithubMarkdown: true)]
    [MemoryDiagnoser]
    [NativeMemoryProfiler]
    public class FibonacciCalc
    {
        // HOMEWORK:
        // 1. Write implementations for RecursiveWithMemoization and Iterative solutions
        // 2. Add MemoryDiagnoser to the benchmark
        // 3. Run with release configuration and compare results
        // 4. Open disassembler report and compare machine code
        // 
        // You can use the discussion panel to compare your results with other students

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public ulong Recursive(ulong n)
        {
            if (n == 1 || n == 2)
                return 1;
            return Recursive(n - 2) + Recursive(n - 1);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong RecursiveWithMemoization(ulong n)
        {
            var fibs = new ulong[n + 1];
            return Fib(n, fibs);
        }
        private static ulong Fib(ulong n, ulong[] fibs)
        {
            if (n == 0 || n == 1)
                return n;

            fibs[n] = Fib(n - 1, fibs) + Fib(n - 2, fibs);
            return fibs[n];
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong Iterative(ulong n)
        {
            ulong a = 0;
            ulong b = 1;
            // In N steps compute Fibonacci sequence iteratively.
            for (ulong i = 0; i < n; i++)
            {
                ulong temp = a;
                a = b;
                b = temp + b;
            }
            return a;
        }

        public IEnumerable<ulong> Data()
        {
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
        }
    }
}
