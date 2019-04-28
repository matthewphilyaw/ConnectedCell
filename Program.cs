using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConnectedGrid
{
    class Program
    {

        static (int, int[,]) ConnectedCell(int[,] matrix) {
            // keep track of regions by id
            var regionSize = new Dictionary<int, int>();

            var currentRegionId = 0;

            var n = matrix.GetLength(0);
            var m = matrix.GetLength(1);

            for (var i = 0; i < n; i++) {
                for (var j = 0; j < m; j++) {
                    bool cellFilled = matrix[i, j] == 1;
                    int cellRegionId = -1;

                    if (!cellFilled) { 
                        // fill with -1 for region id
                        // which is not a valid region id
                        matrix[i, j] = cellRegionId;
                        continue;
                    }

                    cellRegionId = getCellIdAndMergeRegionsIfNeeded(cellRegionId, i, j - 1, regionSize, matrix);
                    cellRegionId = getCellIdAndMergeRegionsIfNeeded(cellRegionId, i - 1, j - 1, regionSize, matrix);
                    cellRegionId = getCellIdAndMergeRegionsIfNeeded(cellRegionId, i - 1, j, regionSize, matrix);
                    cellRegionId = getCellIdAndMergeRegionsIfNeeded(cellRegionId, i - 1, j + 1, regionSize, matrix);

                    // Cell region id hasn't been set so new region
                    if (cellRegionId == -1) {
                        cellRegionId = currentRegionId++;
                        regionSize.Add(cellRegionId, 0); 
                    }

                    // Set the region id for this cell
                    matrix[i, j] = cellRegionId;

                    // add one for the current cell
                    regionSize[cellRegionId]++;
                }
            } 

            var maxRegion = regionSize.Values.DefaultIfEmpty(0).Max();
            return (maxRegion, matrix);
        }

        static int getCellIdAndMergeRegionsIfNeeded(int currentCellRegionId, int row, int column, Dictionary<int,int> regionSize, int[,] matrix) {
            var id = currentCellRegionId;

            var m = matrix.GetLength(1);

            // index is out of bounds
            // row will not be greater than n, no check
            if (row < 0 || column < 0 || column >= m) {
                return id;
            }

            var connectedCellRegionId = matrix[row, column];

            // This would be a non filled cell, so nothing to do here
            if (connectedCellRegionId == -1) {
                return id;
            }

            // If cellRegionId is not set then set it
            if (id == -1) {
                id = connectedCellRegionId;
            } 

            // Otherwise if it is set and not equal the topLeftCellId
            // then merge regions
            else if (id != connectedCellRegionId) {
                // Region id gets removed, so if already removed from another cell in that region
                // so need to check
                if (regionSize.ContainsKey(connectedCellRegionId)) {
                    regionSize[currentCellRegionId] += regionSize[connectedCellRegionId];
                    regionSize.Remove(connectedCellRegionId);
                }

                // set the connected cell id to the new region id
                // always need to set this
                matrix[row, column] = id;
            }

            return id;
        }

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

        static void Main(string[] args)
        {
            // This assumes the files are all corect not validation done
            var files = Directory.EnumerateFiles("tests");

            foreach (var file in files) {
                try {
                    var (expectedAnswer, matrix) = loadMatrix(file);

                    Console.WriteLine("Matrix before computation:");
                    printMatrix(matrix);

                    var (maxRegion, resultingMatrix) = ConnectedCell(matrix);

                    Console.WriteLine("Matrix after computation:");
                    printMatrix(resultingMatrix);

                    Console.WriteLine($"Computed answer: {maxRegion}");
                    Console.WriteLine($"Passed: {expectedAnswer == maxRegion}");
                    Console.WriteLine();
                } 
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    continue;
                }

            }
        }
    }
}
