using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConnectedGrid
{
    class Program
    {

        static int ConnectedCell(int[][] matrix) {
            var cellRegionIds = new Dictionary<string, int>(); 
            var regionSize = new Dictionary<int, int>();

            // store mapping from old to new
            var oldToNewId = new Dictionary<int, int>();

            var currentRegionId = 0;
            for (var i = 0; i < matrix.Length; i++) {
                for (var j = 0; j < matrix[i].Length; j++) {
                    bool cellFilled = matrix[i][j] == 1;

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



        static (int answer, int[][] matrix) loadMatrix(string file) {
            var lines = File.ReadAllLines(file);

            var size = lines[0].Split(' ').Select(p => Int32.Parse(p)).ToArray();
            var answer = Int32.Parse(lines[1]);

            var matrix = new int[size[0]][];

            Console.WriteLine($"Loading matrix: {file}");
            for (int i = 0; i < size[0]; i++) {
                Console.WriteLine(lines[i + 2]);
                // have to offset lines 2
                // Assumes the width is correct
                matrix[i] = lines[i + 2].Split(' ').Select(p => Int32.Parse(p)).ToArray();
            }

            Console.WriteLine($"Expected Answer: {answer}");
            return (answer, matrix);
        }

        static void Main(string[] args)
        {
            // This assumes the files are all corect not validation done
            var files = Directory.EnumerateFiles("tests");

            foreach (var file in files) {
                var (expectedAnswer, matrix) = loadMatrix(file);
                var maxRegion = ConnectedCell(matrix);
                Console.WriteLine($"Computed answer: {maxRegion}");
                Console.WriteLine($"Passed: {expectedAnswer == maxRegion}");
            }
        }
    }
}
