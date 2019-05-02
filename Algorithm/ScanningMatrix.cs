using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectedGrid.Algorithm
{
    public class ScanningMatrix : ILargestRegionFinder
    {
        public (int largestRegion, int[,] mutatedMatrix) GetLargestRegion(int[,] matrix)
        {
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
            else if (id != connectedCellRegionId) {
                // Otherwise if it is set and not equal the topLeftCellId
                // then merge regions

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
    }
}