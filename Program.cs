using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ConnectedGrid.Algorithm;

namespace ConnectedGrid
{
    class Program
    {
        static (int answer, int[,] matrix) loadMatrix(string file) {
            var lines = File.ReadAllLines(file);

            var size = lines[0].Split(' ').Select(p => Int32.Parse(p)).ToArray();
            var answer = Int32.Parse(lines[1]);

            var n = size[0];
            var m = size[1];
            var matrix = new int[n, m];

            Console.WriteLine($"Loading matrix: {file}");
            Console.WriteLine($"Size: {n} x {m}");
            Console.WriteLine($"Expected Answer: {answer}");
            for (int i = 0; i < n; i++) {
                // have to offset lines 2
                // Assumes the width is correct
                var columns = lines[i + 2].Split(' ').Select(p => Int32.Parse(p)).ToArray();

                if (columns.Length != m) {
                    throw new Exception($"Detected invalid row, column count {columns.Length} doesn't match expected size of {m}");
                }

                for (var j = 0; j < m; j++) {
                    matrix[i, j] = columns[j];
                }
            }

            return (answer, matrix);
        }

        static void printMatrix(int[,] matrix) {
            for (var i = 0; i < matrix.GetLength(0); i++) {
                for (var j = 0; j < matrix.GetLength(1); j++) {
                    var cell = matrix[i, j];
                    if (cell < 0) {
                        Console.Write($"{cell} ");
                    }
                    else {
                        Console.Write($" {cell} ");
                    }
                }
                Console.WriteLine();
            }
        }

        static void printBeforeAfter(int[,] before, int[,] after) {
            if (before.GetLength(0) != after.GetLength(0) ||
                before.GetLength(1) != after.GetLength(1)) {
                throw new Exception("before and after matrix should match in dimensions");
            }


            for (var i = 0; i < before.GetLength(0); i++) {
                for (var j = 0; j < before.GetLength(1); j++) {
                    var cell = before[i, j];
                    if (cell < 0) {
                        Console.Write($"{cell} ");
                    }
                    else {
                        Console.Write($" {cell} ");
                    }
                }

                Console.Write("    ->    ");

                for (var j = 0; j < after.GetLength(1); j++) {
                    var cell = after[i, j];
                    if (cell < 0) {
                        Console.Write($"{cell} ");
                    }
                    else {
                        Console.Write($" {cell} ");
                    }
                }
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            // This assumes the files are all corect not validation done
            var files = Directory.EnumerateFiles("tests");

            var algorithms = new List<ILargestRegionFinder>() {
                new ScanningMatrix(),
                new DepthFirstSearchRecursion(),
                new DepthFirstSearchNoRecursion()
            };

            foreach (var file in files) {
                try {
                    var (expectedAnswer, matrix) = loadMatrix(file);

                    foreach (var alg in algorithms) {
                        Console.WriteLine($"Running algorithm: {alg}");

                        // make a copy to print before and after
                        var (maxRegion, resultingMatrix) = alg.GetLargestRegion(matrix.Clone() as int[,]);

                        Console.WriteLine("before -> after");
                        printBeforeAfter(matrix, resultingMatrix);

                        Console.WriteLine($"Computed answer: {maxRegion}");
                        Console.WriteLine($"Passed: {expectedAnswer == maxRegion}");
                        Console.WriteLine();
                    }
                } 
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    continue;
                }

            }
        }
    }
}
