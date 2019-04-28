using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConnectedGrid
{
    class Program
    {

        static int ConnectedCell(int[,] matrix) {
            var cellRegionIds = new Dictionary<string, int>(); 
            var regionSize = new Dictionary<int, int>();

            // store mapping from old to new
            var oldToNewId = new Dictionary<int, int>();

            var currentRegionId = 0;

            var n = matrix.GetLength(0);
            var m = matrix.GetLength(1);

            for (var i = 0; i < n; i++) {
                for (var j = 0; j < m; j++) {
                    bool cellFilled = matrix[i, j] == 1;

                    if (!cellFilled) { 
                        continue;
                    }

                    int cellRegionId = -1;

                    var leftCellId = getRegionId(i, j - 1, cellRegionIds, oldToNewId);
                    var topLeftCellId = getRegionId(i - 1, j - 1, cellRegionIds, oldToNewId);
                    var topCellId = getRegionId(i - 1, j, cellRegionIds, oldToNewId);
                    var topRightCellId = getRegionId(i - 1, j + 1, cellRegionIds, oldToNewId);

                    if (leftCellId != -1) {
                        // could potentially have been merged
                        cellRegionId = leftCellId;
                    }

                    cellRegionId = getCellIdAndMergeRegionsIfNeeded(cellRegionId, topLeftCellId, regionSize, oldToNewId);
                    cellRegionId = getCellIdAndMergeRegionsIfNeeded(cellRegionId, topCellId, regionSize, oldToNewId);
                    cellRegionId = getCellIdAndMergeRegionsIfNeeded(cellRegionId, topRightCellId, regionSize, oldToNewId);

                    if (cellRegionId == -1) {
                        cellRegionId = currentRegionId++;
                        regionSize.Add(cellRegionId, 0); 
                    }

                    // add one for the current cell
                    cellRegionIds.Add(cordinatesToKey(i, j), cellRegionId);
                    regionSize[cellRegionId]++;
                }
            } 

            var maxRegion = regionSize.Values.DefaultIfEmpty(0).Max();

            return maxRegion;
        }

        static string cordinatesToKey(int i, int j) {
            return String.Format("{0}{1}", i.ToString(), j.ToString());
        }

        static int getRegionId(int i, int j, Dictionary<string, int> cellRegionIds, Dictionary<int, int> oldToNewMap) {
            var key = cordinatesToKey(i, j);

            if (!cellRegionIds.ContainsKey(key)) {
                return -1;
            }

            var id = cellRegionIds[key];

            // should never loop around, but potentially a bad idea
            while (oldToNewMap.ContainsKey(id)) {
                id = oldToNewMap[id];    
            };

            return id;
        }

        static int getCellIdAndMergeRegionsIfNeeded(int currentCellRegionId, int connectedCellRegionId, Dictionary<int,int> regionSize, Dictionary<int, int> oldToNewId) {
            var id = currentCellRegionId;

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
                regionSize[currentCellRegionId] += regionSize[connectedCellRegionId];
                regionSize.Remove(connectedCellRegionId);
                oldToNewId[connectedCellRegionId] = currentCellRegionId;
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
            for (int i = 0; i < n; i++) {
                // purely to print the lines
                Console.WriteLine(lines[i + 2]);

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
            Console.WriteLine($"Expected Answer: {answer}");

            return (answer, matrix);
        }

        static void Main(string[] args)
        {
            // This assumes the files are all corect not validation done
            var files = Directory.EnumerateFiles("tests");

            foreach (var file in files) {
                try {
                    var (expectedAnswer, matrix) = loadMatrix(file);
                    var maxRegion = ConnectedCell(matrix);
                    Console.WriteLine($"Computed answer: {maxRegion}");
                    Console.WriteLine($"Passed: {expectedAnswer == maxRegion}");
                } 
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    continue;
                }

            }
        }
    }
}
