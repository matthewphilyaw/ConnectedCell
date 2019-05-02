using System;
using System.Collections.Generic;

namespace ConnectedGrid.Algorithm
{
    public class DepthFirstSearchNoRecursion : ILargestRegionFinder
    {
        public (int largestRegion, int[,] mutatedMatrix) GetLargestRegion(int[,] matrix) {
            var n = matrix.GetLength(0);
            var m = matrix.GetLength(1);

            var largestRegionSize = 0;

            var stack = new Stack<(int, int)>();
            for (int row = 0; row < n; row++) {
                for (int column = 0; column < m; column++) {

                    stack.Push((row, column));

                    var regionSize = 0;
                    while (stack.Count > 0) {
                        var (curRow, curCol) = stack.Pop();

                        if (curRow < 0 || curRow >= n || curCol < 0 || curCol >= m) {
                            continue;
                        }

                        if (matrix[curRow, curCol] == 1) {
                            regionSize++;

                            // Add neigbors
                            for (int neighborX = (curRow - 1); neighborX <= (curRow + 1); neighborX++) {
                                for (int neighborY = (curCol - 1); neighborY <= (curCol + 1); neighborY++) {
                                    if (neighborX == curRow && neighborY == curCol) {
                                        // skip this cell which is current
                                        continue;
                                    }

                                    stack.Push((neighborX, neighborY));
                                }
                            }
                        }

                        matrix[curRow, curCol] = -1;
                    }

                    if (regionSize > largestRegionSize) {
                        largestRegionSize = regionSize;
                    }
                }
            }

            return (largestRegionSize, matrix);
        }
    }
}