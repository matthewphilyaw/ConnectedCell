using System;

namespace ConnectedGrid.Algorithm
{
    public class DepthFirstSearchRecursion : ILargestRegionFinder
    {
        public (int largestRegion, int[,] mutatedMatrix) GetLargestRegion(int[,] matrix) {
            var n = matrix.GetLength(0);
            var m = matrix.GetLength(1);

            var largestRegionSize = 0;
            for (int row = 0; row < n; row++) {
                for (int column = 0; column < m; column++) {
                    var regionSize = findRegionSize(row, column, matrix);

                    if (regionSize > largestRegionSize) {
                        largestRegionSize = regionSize;
                    }
                }
            }

            return (largestRegionSize, matrix);
        }

        private int findRegionSize(int row, int column, int[,] matrix) {
            var n = matrix.GetLength(0);
            var m = matrix.GetLength(1);

            if (row < 0 || row >= n || column < 0 || column >= m) {
                return 0;
            }

            if (matrix[row, column] != 1) {
                matrix[row, column] = -1;

                return 0;
            }

            // need to mark current as negative one before recursion
            matrix[row, column] = -1;
            var regionSize = 1;

            for (int neighborX = (row - 1); neighborX <= (row + 1); neighborX++) {
                for (int neighborY = (column - 1); neighborY <= (column + 1); neighborY++) {
                    if (neighborX == row && neighborY == column) {
                        // skip this cell which is current
                        continue;
                    }

                    regionSize += findRegionSize(neighborX, neighborY, matrix);
                }
            }

            return regionSize;
        }
    }
}